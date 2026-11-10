using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.Forms;

namespace SemiconductorUi
{
    /// <summary>
    /// I/O 모니터 창 (유지보수 → I/O 모니터). 읽기 전용이며 장비에 명령을 보내지 않습니다.
    /// </summary>
    public partial class Form1
    {
        private IoMonitorForm ioMonitorForm;

        /// <summary>
        /// I/O 모니터를 엽니다 (모달 아님). 이미 떠 있으면 앞으로 가져옵니다.
        /// </summary>
        internal void ShowIoMonitorWindow()
        {
            try
            {
                if (ioMonitorForm == null || ioMonitorForm.IsDisposed)
                {
                    ioMonitorForm = new IoMonitorForm(() => EtherCAT_M, () => EthercatConnected) { Owner = this };
                    var area = Screen.FromControl(this).WorkingArea;
                    int x = Left + (Width - ioMonitorForm.Width) / 2;
                    int y = Top + (Height - ioMonitorForm.Height) / 2;
                    x = Math.Max(area.Left, Math.Min(x, area.Right - ioMonitorForm.Width));
                    y = Math.Max(area.Top, Math.Min(y, area.Bottom - ioMonitorForm.Height));
                    ioMonitorForm.Location = new Point(x, y);
                }

                if (!ioMonitorForm.Visible)
                {
                    ioMonitorForm.Show(this);
                }
                ioMonitorForm.BringToFront();
                ioMonitorForm.Activate();
                AddLogMessage("I/O 모니터 열기 (읽기 전용)", "INFO");
            }
            catch (Exception ex)
            {
                AddLogMessage($"I/O 모니터 오류: {ex.Message}", "WARN");
            }
        }
    }
}
