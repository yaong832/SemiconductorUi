using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.Controls;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Forms
{
    /// <summary>
    /// 챔버나 FOUP의 상세 정보를 보여 주는 별도 창입니다.
    /// 레시피 창과 달리 메인 화면을 잠그지 않으므로(모달 아님), 떠 있는 동안에도
    /// 긴급 정지, 일시 정지 같은 버튼을 그대로 누를 수 있습니다.
    /// 창 이동은 Windows 제목 줄로 하며, 내용은 UnitDetailPopup이 그립니다.
    /// 화면 표시만 하며 장비에는 아무 명령도 보내지 않습니다.
    /// </summary>
    internal sealed class UnitDetailForm : Form
    {
        private readonly UnitDetailPopup content;

        /// <summary>제목 줄의 X로 닫을 때 (창은 없애지 않고 숨김)</summary>
        public event EventHandler CloseRequested;

        public UnitDetailForm()
        {
            Text = "장비 상세";
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = UiTheme.Surface;
            KeyPreview = true;

            content = new UnitDetailPopup
            {
                Name = "unitDetailContent",
                Location = Point.Empty
            };
            content.SizeChanged += (s, e) => FitToContent();
            Controls.Add(content);
            FitToContent();
        }

        /// <summary>창을 띄워도 메인 화면의 입력 초점을 빼앗지 않습니다.</summary>
        protected override bool ShowWithoutActivation => true;

        /// <summary>지금 보여 주는 대상 (예: "PMA", "FOUP A"). 숨겨져 있으면 null</summary>
        public string UnitKey
        {
            get => content.UnitKey;
            set => content.UnitKey = value;
        }

        public float DetailScale
        {
            get => content.DetailScale;
            set => content.DetailScale = value;
        }

        public void SetContent(UnitDetailContent detail)
        {
            content.SetContent(detail);
            string title = string.IsNullOrWhiteSpace(detail?.Title) ? "장비" : detail.Title;
            string text = title + " 상세";
            if (Text != text)
            {
                Text = text;
            }
        }

        private void FitToContent()
        {
            if (ClientSize != content.Size)
            {
                ClientSize = content.Size;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                CloseRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 사용자가 X로 닫으면 숨기기만 하고 다음에 다시 씁니다.
            // 메인 화면이 닫히거나 Windows가 끝날 때는 그대로 닫습니다.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                CloseRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            base.OnFormClosing(e);
        }
    }
}
