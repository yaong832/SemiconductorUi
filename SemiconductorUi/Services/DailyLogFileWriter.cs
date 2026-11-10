using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace SemiconductorUi.Services
{
    /// <summary>
    /// 로그를 한 줄씩 날짜별 CSV 파일에 바로 씁니다.
    /// - 파일: {폴더}\Log_yyyyMMdd.csv, 열: Date,Time,Level,Message (Time은 밀리초까지)
    /// - 한 줄 쓸 때마다 파일로 내보내므로, 프로그램이 닫히거나 멈춰도 그때까지 쓴 줄은 남습니다.
    /// - WARN 이상(WARN, ALARM, ERROR, CRITICAL)은 디스크까지 바로 기록해 정전에도 남게 합니다.
    /// - 보관 기간이 지난 Log_yyyyMMdd.csv 파일만 지웁니다 (파일 이름의 날짜 기준). 0이면 지우지 않습니다.
    /// - 쓰기에 실패해도 예외를 밖으로 던지지 않습니다. 화면 로그와 장비 동작은 그대로 이어 갑니다.
    /// </summary>
    public sealed class DailyLogFileWriter : IDisposable
    {
        internal const string FilePrefix = "Log_";
        internal const string FileExtension = ".csv";
        internal const string Header = "Date,Time,Level,Message";

        /// <summary>
        /// 쓰기에 실패한 뒤 다시 파일을 열어 보기까지 기다리는 시간
        /// </summary>
        private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(10);

        private readonly object _lock = new object();
        private readonly string _folder;
        private readonly int _retentionDays;

        private StreamWriter _writer;
        private DateTime _currentDate = DateTime.MinValue;
        private DateTime _nextRetryTime = DateTime.MinValue;
        private bool _disposed;

        /// <param name="folder">로그 폴더</param>
        /// <param name="retentionDays">보관 일수. 0 이하이면 지우지 않습니다.</param>
        public DailyLogFileWriter(string folder, int retentionDays)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException("로그 폴더가 비어 있습니다.", nameof(folder));
            }

            _folder = folder;
            _retentionDays = retentionDays;
        }

        /// <summary>
        /// 기본 로그 폴더: 문서\SemiconductorUi_Logs\Daily
        /// </summary>
        public static string DefaultFolder =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SemiconductorUi_Logs",
                "Daily");

        /// <summary>
        /// 로그 폴더
        /// </summary>
        public string Folder => _folder;

        /// <summary>
        /// 마지막 쓰기가 실패했는지 여부. 다음 쓰기에 성공하면 false로 돌아갑니다.
        /// </summary>
        public bool IsFailing { get; private set; }

        /// <summary>
        /// 마지막 실패 이유 (실패 중일 때만 의미 있음)
        /// </summary>
        public string LastError { get; private set; }

        /// <summary>
        /// 쓰기 결과가 실패↔정상으로 바뀌었을 때 알려 줍니다 (true = 실패 시작, false = 다시 정상).
        /// 잠금 안에서 부르므로, 받는 쪽에서 이 객체의 Write를 다시 부르면 안 됩니다.
        /// </summary>
        public event Action<bool, string> FailureStateChanged;

        /// <summary>
        /// 날짜에 해당하는 파일 경로
        /// </summary>
        public string GetFilePath(DateTime date)
        {
            return Path.Combine(_folder, FilePrefix + date.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + FileExtension);
        }

        /// <summary>
        /// 로그 한 줄을 씁니다.
        /// </summary>
        public void Write(DateTime time, string level, string message)
        {
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }

                if (IsFailing && DateTime.Now < _nextRetryTime)
                {
                    return;
                }

                try
                {
                    EnsureWriter(time.Date);

                    _writer.WriteLine(FormatLine(time, level, message));
                    _writer.Flush();

                    if (IsImportantLevel(level))
                    {
                        var fileStream = _writer.BaseStream as FileStream;
                        fileStream?.Flush(true);
                    }

                    if (IsFailing)
                    {
                        IsFailing = false;
                        LastError = null;
                        RaiseFailureStateChanged(false, null);
                    }
                }
                catch (Exception ex)
                {
                    CloseWriter();
                    _nextRetryTime = DateTime.Now + RetryInterval;
                    LastError = ex.Message;
                    if (!IsFailing)
                    {
                        IsFailing = true;
                        RaiseFailureStateChanged(true, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 보관 기간이 지난 로그 파일을 지웁니다. 지운 파일 수를 돌려줍니다.
        /// 파일 이름이 Log_yyyyMMdd.csv 형식인 파일만 대상으로 합니다.
        /// </summary>
        public int DeleteExpiredFiles(DateTime today)
        {
            if (_retentionDays <= 0)
            {
                return 0;
            }

            int deleted = 0;
            try
            {
                if (!Directory.Exists(_folder))
                {
                    return 0;
                }

                // 오늘을 포함해 보관 일수만큼 남깁니다. 예: 90일이면 오늘부터 89일 전까지 남깁니다.
                var oldestKept = today.Date.AddDays(-(_retentionDays - 1));
                foreach (var path in Directory.GetFiles(_folder, FilePrefix + "*" + FileExtension))
                {
                    DateTime fileDate;
                    if (!TryParseFileDate(Path.GetFileName(path), out fileDate))
                    {
                        continue;
                    }

                    if (fileDate >= oldestKept)
                    {
                        continue;
                    }

                    try
                    {
                        File.Delete(path);
                        deleted++;
                    }
                    catch
                    {
                        // 열려 있는 파일 등은 다음 기회에 지웁니다.
                    }
                }
            }
            catch
            {
                // 폴더를 읽지 못해도 로그 쓰기에는 영향을 주지 않습니다.
            }

            return deleted;
        }

        /// <summary>
        /// 파일 이름에서 날짜를 읽습니다. Log_yyyyMMdd.csv 형식이 아니면 false.
        /// </summary>
        internal static bool TryParseFileDate(string fileName, out DateTime date)
        {
            date = DateTime.MinValue;
            if (string.IsNullOrEmpty(fileName)
                || !fileName.StartsWith(FilePrefix, StringComparison.OrdinalIgnoreCase)
                || !fileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var datePart = fileName.Substring(FilePrefix.Length, fileName.Length - FilePrefix.Length - FileExtension.Length);
            return datePart.Length == 8
                && DateTime.TryParseExact(datePart, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        /// <summary>
        /// CSV 한 줄: 날짜,시각(밀리초),레벨,"메시지"
        /// </summary>
        internal static string FormatLine(DateTime time, string level, string message)
        {
            var safeLevel = string.IsNullOrWhiteSpace(level) ? "INFO" : level.Trim();
            var safeMessage = (message ?? string.Empty).Replace("\"", "\"\"");
            return time.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ","
                + time.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture) + ","
                + safeLevel + ",\""
                + safeMessage + "\"";
        }

        private static bool IsImportantLevel(string level)
        {
            if (string.IsNullOrWhiteSpace(level))
            {
                return false;
            }

            switch (level.Trim().ToUpperInvariant())
            {
                case "WARN":
                case "ALARM":
                case "ERROR":
                case "CRITICAL":
                    return true;
                default:
                    return false;
            }
        }

        private void EnsureWriter(DateTime date)
        {
            if (_writer != null && date == _currentDate)
            {
                return;
            }

            bool dayChanged = _currentDate != DateTime.MinValue && date != _currentDate;
            CloseWriter();

            Directory.CreateDirectory(_folder);
            var path = GetFilePath(date);

            // 다른 프로그램(메모장, 엑셀 읽기 전용)이 실행 중에 파일을 열어 볼 수 있게 공유를 허용합니다.
            var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
            try
            {
                // 새 파일이면 엑셀에서 한글이 깨지지 않도록 BOM과 머리글을 씁니다.
                // 이어 쓰는 파일(위치 > 0)에는 StreamWriter가 BOM을 다시 쓰지 않습니다.
                bool isNewFile = stream.Length == 0;
                var writer = new StreamWriter(stream, new UTF8Encoding(true));
                if (isNewFile)
                {
                    writer.WriteLine(Header);
                    writer.Flush();
                }

                _writer = writer;
                _currentDate = date;
            }
            catch
            {
                stream.Dispose();
                throw;
            }

            if (dayChanged)
            {
                DeleteExpiredFiles(date);
            }
        }

        private void CloseWriter()
        {
            if (_writer == null)
            {
                return;
            }

            try
            {
                _writer.Dispose();
            }
            catch
            {
                // 닫다가 실패해도 무시합니다.
            }

            _writer = null;
        }

        private void RaiseFailureStateChanged(bool failing, string reason)
        {
            try
            {
                FailureStateChanged?.Invoke(failing, reason);
            }
            catch
            {
                // 알림 처리 오류가 로그 쓰기를 막지 않게 합니다.
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                CloseWriter();
            }
        }
    }
}
