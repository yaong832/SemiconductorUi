using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.Forms;
using SemiconductorUi.Services;

namespace SemiconductorUi
{
    /// <summary>
    /// 변경 이력 (보고서 → 변경 이력). 저장 동작은 그대로 두고, 바뀐 값과 사용자만 파일에 남깁니다.
    /// </summary>
    public partial class Form1
    {
        private ChangeHistoryForm changeHistoryForm;

        private void InitializeChangeHistory()
        {
            if (IsDesignEnvironment())
            {
                return;
            }

            try
            {
                ChangeHistory.Initialize(AppSettings.ChangeHistoryFolder, () => IsLoggedIn ? CurrentUser : "(로그인 안 함)", () => IsLoggedIn ? CurrentRole : "", AddLogMessage);
            }
            catch (Exception ex)
            {
                AddLogMessage($"변경 이력을 시작하지 못했습니다: {ex.Message}", "WARN");
            }
        }

        /// <summary>
        /// 변경 이력 창을 엽니다 (모달 아님). 이미 떠 있으면 앞으로 가져와 새로 읽습니다.
        /// </summary>
        internal void ShowChangeHistoryWindow()
        {
            try
            {
                if (changeHistoryForm == null || changeHistoryForm.IsDisposed)
                {
                    changeHistoryForm = new ChangeHistoryForm { Owner = this };
                    var area = Screen.FromControl(this).WorkingArea;
                    int x = Left + (Width - changeHistoryForm.Width) / 2;
                    int y = Top + (Height - changeHistoryForm.Height) / 2;
                    x = Math.Max(area.Left, Math.Min(x, area.Right - changeHistoryForm.Width));
                    y = Math.Max(area.Top, Math.Min(y, area.Bottom - changeHistoryForm.Height));
                    changeHistoryForm.Location = new Point(x, y);
                }

                if (!changeHistoryForm.Visible)
                {
                    changeHistoryForm.Show(this);
                }
                else
                {
                    changeHistoryForm.RefreshRows();
                }
                changeHistoryForm.BringToFront();
                changeHistoryForm.Activate();
            }
            catch (Exception ex)
            {
                AddLogMessage($"변경 이력 창 오류: {ex.Message}", "WARN");
            }
        }
    }
}
