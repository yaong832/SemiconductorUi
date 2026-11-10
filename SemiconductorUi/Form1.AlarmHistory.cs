using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.Forms;
using SemiconductorUi.Services;

namespace SemiconductorUi
{
    /// <summary>
    /// 알람 이력: 알람이 생기는 곳에서 RecordAlarm을, 알람 리셋에서 ClearRecordedAlarms를 부릅니다.
    /// 기록만 하며 장비 동작, 공정 흐름, 알람 리셋 판정은 바꾸지 않습니다.
    /// 상단 바의 알람 표시를 누르면 현재 알람과 이력 창을 엽니다.
    /// </summary>
    public partial class Form1
    {
        private readonly object alarmHistoryLock = new object();
        private AlarmHistoryService alarmHistory;
        private AlarmHistoryForm alarmHistoryForm;

        /// <summary>
        /// 알람 이력 서비스 (처음 쓸 때 만듭니다)
        /// </summary>
        internal AlarmHistoryService AlarmHistory
        {
            get
            {
                lock (alarmHistoryLock)
                {
                    if (alarmHistory == null)
                    {
                        alarmHistory = AlarmHistoryService.CreateFromSettings();
                    }
                    return alarmHistory;
                }
            }
        }

        /// <summary>
        /// 알람 발생을 이력에 남깁니다. 같은 알람이 이미 걸려 있으면 다시 남기지 않습니다.
        /// 기록 실패는 무시합니다 (장비 동작에 영향 없음).
        /// </summary>
        internal void RecordAlarm(int code, string unit, string detail)
        {
            try
            {
                AlarmHistory.Raise(code, unit, detail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"알람 이력 기록 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 알람 리셋 때 부릅니다. emergencyStopGroupOnly이면 비상정지·서보 알람만 해제합니다.
        /// </summary>
        internal void ClearRecordedAlarms(bool emergencyStopGroupOnly)
        {
            try
            {
                string user = IsLoggedIn ? CurrentUser : "-";
                int cleared = emergencyStopGroupOnly
                    ? AlarmHistory.ClearEmergencyStopGroup(user)
                    : AlarmHistory.ClearAll(user);
                if (cleared > 0)
                {
                    AddLogMessage($"알람 이력: {cleared}건 해제 (사용자: {user})", "INFO");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"알람 이력 해제 기록 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 상단 바 알람 표시를 누르면 알람 창을 엽니다.
        /// </summary>
        private void AttachAlarmHistoryChip()
        {
            if (shellChipAlarm == null)
            {
                return;
            }

            shellChipAlarm.Cursor = Cursors.Hand;
            shellChipAlarm.Click += (s, e) => ShowAlarmHistoryWindow();
        }

        /// <summary>
        /// 현재 알람과 이력 창을 엽니다 (모달 아님).
        /// </summary>
        internal void ShowAlarmHistoryWindow()
        {
            try
            {
                if (alarmHistoryForm == null || alarmHistoryForm.IsDisposed)
                {
                    alarmHistoryForm = new AlarmHistoryForm(AlarmHistory) { Owner = this };
                    PlaceAlarmHistoryWindow(alarmHistoryForm);
                }

                alarmHistoryForm.RefreshAll();
                if (!alarmHistoryForm.Visible)
                {
                    alarmHistoryForm.Show(this);
                }
                alarmHistoryForm.BringToFront();
                alarmHistoryForm.Activate();
            }
            catch (Exception ex)
            {
                AddLogMessage($"알람 창 오류: {ex.Message}", "WARN");
            }
        }

        private void PlaceAlarmHistoryWindow(Form window)
        {
            var area = Screen.FromControl(this).WorkingArea;
            var center = new Point(Left + Width / 2, Top + Height / 2);
            int x = center.X - window.Width / 2;
            int y = center.Y - window.Height / 2;
            x = Math.Max(area.Left, Math.Min(x, area.Right - window.Width));
            y = Math.Max(area.Top, Math.Min(y, area.Bottom - window.Height));
            window.Location = new Point(x, y);
        }

        /// <summary>
        /// 프로그램 종료 때 이력 파일을 닫습니다.
        /// </summary>
        internal void DisposeAlarmHistory()
        {
            lock (alarmHistoryLock)
            {
                alarmHistory?.Dispose();
            }
        }
    }
}
