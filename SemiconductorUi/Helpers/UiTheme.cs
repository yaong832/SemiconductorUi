using System.Drawing;
using System.Windows.Forms;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// 메인 화면 공통 색상표 (밝은 톤).
    /// 장비 시뮬레이터(EquipmentCanvasStyler)와 같은 계열의 색을 쓴다.
    /// </summary>
    public static class UiTheme
    {
        public static readonly Color AppBack = Color.FromArgb(226, 231, 238);
        public static readonly Color Surface = Color.White;
        public static readonly Color SurfaceAlt = Color.FromArgb(242, 245, 249);
        public static readonly Color Border = Color.FromArgb(208, 215, 225);

        public static readonly Color TextPrimary = Color.FromArgb(33, 41, 54);
        public static readonly Color TextSecondary = Color.FromArgb(100, 112, 128);

        public static readonly Color Primary = Color.FromArgb(45, 98, 196);
        public static readonly Color PrimaryHover = Color.FromArgb(38, 84, 170);
        public static readonly Color ButtonFill = Color.FromArgb(232, 237, 244);
        public static readonly Color ButtonHover = Color.FromArgb(218, 226, 236);
        public static readonly Color ButtonDisabledText = Color.FromArgb(150, 160, 174);

        public static readonly Color Success = Color.FromArgb(36, 150, 90);
        public static readonly Color Warning = Color.FromArgb(214, 138, 20);
        public static readonly Color Danger = Color.FromArgb(206, 56, 66);
        public static readonly Color DangerHover = Color.FromArgb(178, 44, 54);

        /// <summary>
        /// 일반(보조) 버튼: 밝은 바탕 + 진한 글씨.
        /// </summary>
        public static void StyleSecondaryButton(Button button)
        {
            StyleButton(button, ButtonFill, ButtonHover, TextPrimary);
        }

        /// <summary>
        /// 주 동작 버튼: 파란 바탕 + 흰 글씨.
        /// </summary>
        public static void StylePrimaryButton(Button button)
        {
            StyleButton(button, Primary, PrimaryHover, Color.White);
        }

        public static void StyleDangerButton(Button button)
        {
            StyleButton(button, Danger, DangerHover, Color.White);
        }

        public static void StyleButton(Button button, Color back, Color hover, Color text)
        {
            if (button == null)
            {
                return;
            }

            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = hover;
            button.FlatAppearance.MouseDownBackColor = hover;
            button.BackColor = back;
            button.ForeColor = text;
            button.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// 검은 FixedSingle 테두리를 연한 회색 1px 테두리로 바꾼다.
        /// </summary>
        public static void ApplySoftBorder(Control control)
        {
            if (control == null)
            {
                return;
            }

            if (control is Panel panel)
            {
                if (panel.BorderStyle != BorderStyle.FixedSingle)
                {
                    return;
                }
                panel.BorderStyle = BorderStyle.None;
            }
            else if (control is Label label)
            {
                if (label.BorderStyle != BorderStyle.FixedSingle)
                {
                    return;
                }
                label.BorderStyle = BorderStyle.None;
            }
            else
            {
                return;
            }

            control.Paint += (s, e) =>
            {
                using (var pen = new Pen(Border))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, control.ClientSize.Width - 1, control.ClientSize.Height - 1);
                }
            };
            control.Resize += (s, e) => control.Invalidate();
            control.Invalidate();
        }

        /// <summary>
        /// 선택 상태가 있는 탭/네비게이션 버튼.
        /// </summary>
        public static void ApplyToggleButton(Button button, bool selected)
        {
            if (selected)
            {
                StylePrimaryButton(button);
            }
            else
            {
                StyleSecondaryButton(button);
            }
        }
    }
}
