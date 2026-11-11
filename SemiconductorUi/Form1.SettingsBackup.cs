using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.Forms;

namespace SemiconductorUi
{
    /// <summary>
    /// 설정 백업·복원 창 (설정 → 백업 · 복원). 백업·복원 파일 작업은 Services/SettingsBackupService.cs에 있습니다.
    /// </summary>
    public partial class Form1
    {
        private SettingsBackupForm settingsBackupForm;

        /// <summary>
        /// 설정 백업·복원 창을 엽니다 (모달 아님). 이미 떠 있으면 앞으로 가져와 새로 읽습니다.
        /// </summary>
        internal void ShowSettingsBackupWindow()
        {
            try
            {
                if (settingsBackupForm == null || settingsBackupForm.IsDisposed)
                {
                    settingsBackupForm = new SettingsBackupForm(
                        () => IsLoggedIn,
                        () => CurrentUser,
                        () => IsLoggedIn && CurrentRole == "관리자",
                        AddLogMessage)
                    {
                        Owner = this
                    };
                    var area = Screen.FromControl(this).WorkingArea;
                    int x = Left + (Width - settingsBackupForm.Width) / 2;
                    int y = Top + (Height - settingsBackupForm.Height) / 2;
                    x = Math.Max(area.Left, Math.Min(x, area.Right - settingsBackupForm.Width));
                    y = Math.Max(area.Top, Math.Min(y, area.Bottom - settingsBackupForm.Height));
                    settingsBackupForm.Location = new Point(x, y);
                }

                if (!settingsBackupForm.Visible)
                {
                    settingsBackupForm.Show(this);
                }
                else
                {
                    settingsBackupForm.RefreshAll();
                }
                settingsBackupForm.BringToFront();
                settingsBackupForm.Activate();
            }
            catch (Exception ex)
            {
                AddLogMessage($"설정 백업 창 오류: {ex.Message}", "WARN");
            }
        }
    }
}
