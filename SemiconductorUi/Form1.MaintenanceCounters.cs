using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.Forms;
using SemiconductorUi.Hardware;
using SemiconductorUi.Services;

namespace SemiconductorUi
{
    /// <summary>
    /// 유지보수 카운터 (유지보수 → 유지보수 카운터).
    /// 장비 창구(Ieg3268Device)가 실제로 보낸 출력·축 명령 횟수를 항목별로 모아 파일에 남깁니다.
    /// 장비 명령은 바꾸지 않고, 점검 주기에 도달해도 경고 로그만 남깁니다.
    /// </summary>
    public partial class Form1
    {
        private MaintenanceCounterService maintenanceCounters;
        private MaintenanceCounterForm maintenanceCounterForm;

        /// <summary>시작하지 못했으면 null</summary>
        internal MaintenanceCounterService MaintenanceCounters => maintenanceCounters;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            StartMaintenanceCounters();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // 마지막 10초 안에 센 값도 남도록 종료할 때 저장합니다.
            DisposeMaintenanceCounters();
            base.OnFormClosed(e);
        }

        private void StartMaintenanceCounters()
        {
            if (maintenanceCounters != null || IsDesignEnvironment())
            {
                return;
            }

            try
            {
                var usage = (EtherCAT_M as Ieg3268Device)?.Usage;
                if (usage == null)
                {
                    return;
                }

                maintenanceCounters = MaintenanceCounterService.CreateFromSettings(usage, AddLogMessage);
                maintenanceCounters.Start();
            }
            catch (Exception ex)
            {
                maintenanceCounters = null;
                AddLogMessage($"유지보수 카운터를 시작하지 못했습니다: {ex.Message}", "WARN");
            }
        }

        internal void DisposeMaintenanceCounters()
        {
            var service = maintenanceCounters;
            maintenanceCounters = null;
            service?.Dispose();
        }

        /// <summary>
        /// 유지보수 카운터 창을 엽니다 (모달 아님). 이미 떠 있으면 앞으로 가져옵니다.
        /// </summary>
        internal void ShowMaintenanceCounterWindow()
        {
            try
            {
                if (maintenanceCounters == null)
                {
                    AddLogMessage("유지보수 카운터가 시작되지 않아 창을 열 수 없습니다.", "WARN");
                    return;
                }

                if (maintenanceCounterForm == null || maintenanceCounterForm.IsDisposed)
                {
                    maintenanceCounterForm = new MaintenanceCounterForm(
                        maintenanceCounters,
                        () => IsLoggedIn,
                        () => CurrentUser,
                        () => CurrentRole == "관리자")
                    {
                        Owner = this
                    };
                    var area = Screen.FromControl(this).WorkingArea;
                    int x = Left + (Width - maintenanceCounterForm.Width) / 2;
                    int y = Top + (Height - maintenanceCounterForm.Height) / 2;
                    x = Math.Max(area.Left, Math.Min(x, area.Right - maintenanceCounterForm.Width));
                    y = Math.Max(area.Top, Math.Min(y, area.Bottom - maintenanceCounterForm.Height));
                    maintenanceCounterForm.Location = new Point(x, y);
                }

                if (!maintenanceCounterForm.Visible)
                {
                    maintenanceCounterForm.Show(this);
                }
                maintenanceCounterForm.BringToFront();
                maintenanceCounterForm.Activate();
            }
            catch (Exception ex)
            {
                AddLogMessage($"유지보수 카운터 창 오류: {ex.Message}", "WARN");
            }
        }
    }
}
