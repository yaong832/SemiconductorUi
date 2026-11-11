using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SemiconductorUi.Services;

namespace SemiconductorUi
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += (s, e) =>
            {
                ExceptionHandler.HandleException(e.Exception, "UI Thread");
            };
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    ExceptionHandler.HandleException(ex, "Unhandled");
                }
            };

            // 설정 파일을 읽기 전에: 예약된 설정 복원, 하루 한 번 자동 백업
            var startupNotes = SettingsBackup.RunStartupTasks(DateTime.Now, out bool settingsRestored);
            var restoreNotes = startupNotes.Where(n => n.AboutRestore).Select(n => n.Message).ToList();
            if (restoreNotes.Count > 0)
            {
                MessageBox.Show(
                    string.Join("\n\n", restoreNotes),
                    "설정 복원",
                    MessageBoxButtons.OK,
                    settingsRestored ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }

            var mainForm = new Form1();
            foreach (var note in startupNotes)
            {
                mainForm.AddLogMessage(note.Message, note.Level);
            }
            Application.Run(mainForm);

            // 날짜별 로그 파일을 닫습니다. 종료 직전 로그까지 이미 파일에 쓰여 있습니다.
            var logger = mainForm.LoggerService;
            if (logger != null)
            {
                logger.Log("프로그램 종료", "INFO");
                (logger as IDisposable)?.Dispose();
            }
            mainForm.DisposeAlarmHistory();
        }
    }
}
