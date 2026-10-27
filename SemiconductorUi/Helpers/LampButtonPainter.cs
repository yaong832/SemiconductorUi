using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemiconductorUi.Helpers
{
    /// <summary>램프와 상태 칩의 색.</summary>
    internal enum LampTone
    {
        Off,
        Green,
        Amber,
        Red,
        Blue
    }

    /// <summary>램프 버튼 한 개가 지금 보여 줄 것: 램프 색, 버튼 글자, 오른쪽 상태 칩.</summary>
    internal struct LampState
    {
        public LampTone Tone;
        public string Label;
        public string Chip;

        public LampState(LampTone tone, string label, string chip)
        {
            Tone = tone;
            Label = label;
            Chip = chip;
        }
    }

    /// <summary>
    /// 오른쪽 제어 패널의 "램프 버튼"을 그립니다.
    /// 기존 Button 객체에 Paint만 덧붙이므로 클릭 처리, Enabled, Text는 그대로입니다.
    /// 램프가 켜진 버튼은 지금 누를 수 있거나 켜져 있는 상태입니다.
    /// </summary>
    internal static class LampButtonPainter
    {
        public static readonly Color PlateBack = UiTheme.SurfaceAlt;
        private static readonly Color FaceEdge = Color.FromArgb(196, 205, 217);
        private static readonly Color FaceDisabled = Color.FromArgb(247, 249, 251);
        private static readonly Color BorderDisabled = Color.FromArgb(225, 230, 237);
        private static readonly Color LampOff = Color.FromArgb(200, 207, 217);
        private static readonly Color PrimaryEdge = Color.FromArgb(31, 70, 140);
        private static readonly Color PrimaryDisabled = Color.FromArgb(201, 214, 238);
        private static readonly Color DangerRing = Color.FromArgb(168, 38, 48);
        private static readonly Color DangerEdge = Color.FromArgb(142, 31, 39);

        private const int EdgeDepth = 3;
        private const int PressDepth = 2;

        private static readonly Font ChipFont = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        private static readonly Font PlateTitleFont = new Font("Segoe UI", 9F, FontStyle.Bold);

        private static readonly HashSet<Control> hovered = new HashSet<Control>();
        private static readonly HashSet<Control> pressed = new HashSet<Control>();
        // 키보드(Tab)로 포커스가 온 버튼만 포커스 테두리를 그립니다.
        private static readonly HashSet<Control> keyboardFocus = new HashSet<Control>();

        /// <summary>램프 버튼으로 그립니다. primary면 파란 바탕(공정 시작)입니다.</summary>
        public static void Attach(Button button, Func<LampState> state, bool primary = false)
        {
            if (button == null) return;
            HookPressState(button);
            button.Paint += (s, e) => PaintLampButton(button, e.Graphics, state, primary, true);
        }

        /// <summary>램프 없이 같은 모양의 버튼으로 그립니다 (창 아래쪽 저장/닫기 같은 버튼). 글자는 가운데입니다.</summary>
        public static void AttachPlain(Button button, bool primary = false)
        {
            if (button == null) return;
            HookPressState(button);
            button.Paint += (s, e) => PaintLampButton(button, e.Graphics, null, primary, false);
        }

        /// <summary>긴급 정지: 빨간 바탕에 진한 테두리, 가운데 글자.</summary>
        public static void AttachEmergency(Button button)
        {
            if (button == null) return;
            HookPressState(button);
            button.Paint += (s, e) => PaintEmergency(button, e.Graphics);
        }

        /// <summary>그룹 상자(또는 패널)를 연회색 판(plate)으로 그리고, 왼쪽 위에 Text를 제목으로 씁니다.</summary>
        public static void AttachPlate(Control group)
        {
            if (group == null) return;
            group.BackColor = PlateBack;
            group.Paint += (s, e) => PaintPlate(group, e.Graphics);
        }

        public static Color ToneColor(LampTone tone)
        {
            switch (tone)
            {
                case LampTone.Green: return UiTheme.Success;
                case LampTone.Amber: return UiTheme.Warning;
                case LampTone.Red: return UiTheme.Danger;
                case LampTone.Blue: return UiTheme.Primary;
                default: return LampOff;
            }
        }

        /// <summary>상태 칩 바탕과 글자 색 (A 시안의 칩 색).</summary>
        public static void ChipColors(LampTone tone, out Color back, out Color fore)
        {
            switch (tone)
            {
                case LampTone.Green: back = Color.FromArgb(228, 243, 234); fore = Color.FromArgb(28, 122, 73); break;
                case LampTone.Amber: back = Color.FromArgb(252, 241, 222); fore = Color.FromArgb(150, 92, 8); break;
                case LampTone.Red: back = Color.FromArgb(251, 233, 234); fore = Color.FromArgb(168, 38, 48); break;
                case LampTone.Blue: back = Color.FromArgb(232, 239, 250); fore = UiTheme.Primary; break;
                default: back = Color.FromArgb(238, 241, 245); fore = Color.FromArgb(58, 69, 85); break;
            }
        }

        /// <summary>둥근 램프 하나. 켜져 있으면 둘레에 은은한 빛을 그립니다.</summary>
        public static void DrawLamp(Graphics g, RectangleF rect, LampTone tone)
        {
            var color = ToneColor(tone);
            if (tone != LampTone.Off)
            {
                for (int i = 3; i >= 1; i--)
                {
                    var glow = RectangleF.Inflate(rect, i * 1.5f, i * 1.5f);
                    using (var path = Rounded(glow, Math.Min(glow.Width, glow.Height) / 2f))
                    using (var brush = new SolidBrush(Color.FromArgb(28, color)))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            using (var path = Rounded(rect, Math.Min(rect.Width, rect.Height) / 2f))
            using (var brush = new SolidBrush(color))
            {
                g.FillPath(brush, path);
            }
            if (tone == LampTone.Off)
            {
                using (var pen = new Pen(Color.FromArgb(70, 0, 0, 0), 1f))
                using (var path = Rounded(rect, Math.Min(rect.Width, rect.Height) / 2f))
                {
                    g.DrawPath(pen, path);
                }
            }
            else
            {
                // 위쪽 반사광
                var shine = new RectangleF(rect.X + rect.Width * 0.25f, rect.Y + 2, rect.Width * 0.5f, Math.Max(2f, rect.Height * 0.25f));
                using (var path = Rounded(shine, shine.Width / 2f))
                using (var brush = new SolidBrush(Color.FromArgb(110, Color.White)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        /// <summary>오른쪽 끝 상태 칩을 그리고 칩의 왼쪽 X를 돌려줍니다.</summary>
        public static int DrawChip(Graphics g, string text, LampTone tone, int right, int centerY, bool onPrimary = false)
        {
            if (string.IsNullOrEmpty(text)) return right;

            ChipColors(tone, out var back, out var fore);
            if (onPrimary) back = Color.White;
            var size = TextRenderer.MeasureText(g, text, ChipFont, Size.Empty, TextFormatFlags.NoPadding);
            int height = 22;
            var rect = new Rectangle(right - size.Width - 20, centerY - height / 2, size.Width + 20, height);
            using (var path = Rounded(rect, height / 2f))
            using (var brush = new SolidBrush(back))
            {
                g.FillPath(brush, path);
            }
            TextRenderer.DrawText(g, text, ChipFont, rect, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
            return rect.Left;
        }

        private static void HookPressState(Button button)
        {
            button.MouseEnter += (s, e) => { hovered.Add(button); button.Invalidate(); };
            button.MouseLeave += (s, e) => { hovered.Remove(button); pressed.Remove(button); button.Invalidate(); };
            button.MouseDown += (s, e) => { keyboardFocus.Remove(button); if (e.Button == MouseButtons.Left) { pressed.Add(button); button.Invalidate(); } };
            button.KeyUp += (s, e) => { if (keyboardFocus.Add(button)) button.Invalidate(); };
            button.LostFocus += (s, e) => { if (keyboardFocus.Remove(button)) button.Invalidate(); };
            button.MouseUp += (s, e) => { pressed.Remove(button); button.Invalidate(); };
            button.Disposed += (s, e) => { hovered.Remove(button); pressed.Remove(button); keyboardFocus.Remove(button); };
        }

        /// <summary>눌린 정도만큼 내려간 버튼 앞면 영역과 아래 그림자 영역.</summary>
        private static void FaceRects(Button button, out Rectangle face, out Rectangle edge, out bool isPressed)
        {
            isPressed = button.Enabled && pressed.Contains(button);
            int offset = isPressed ? PressDepth : 0;
            face = new Rectangle(0, offset, button.Width - 1, button.Height - 1 - EdgeDepth);
            edge = new Rectangle(0, EdgeDepth, button.Width - 1, button.Height - 1 - EdgeDepth);
        }

        private static void PaintLampButton(Button button, Graphics g, Func<LampState> stateProvider, bool primary, bool showLamp)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(button.Parent?.BackColor ?? UiTheme.Surface);

            var state = stateProvider?.Invoke() ?? new LampState(LampTone.Off, null, null);
            bool enabled = button.Enabled;
            bool hover = enabled && hovered.Contains(button);
            FaceRects(button, out var face, out var edge, out _);

            Color faceColor, borderColor, edgeColor, textColor;
            if (primary)
            {
                faceColor = !enabled ? PrimaryDisabled : hover ? UiTheme.PrimaryHover : UiTheme.Primary;
                borderColor = faceColor;
                edgeColor = enabled ? PrimaryEdge : PrimaryDisabled;
                textColor = Color.White;
            }
            else
            {
                faceColor = !enabled ? FaceDisabled : hover ? UiTheme.SurfaceAlt : UiTheme.Surface;
                borderColor = enabled ? UiTheme.Border : BorderDisabled;
                edgeColor = enabled ? FaceEdge : BorderDisabled;
                textColor = enabled ? UiTheme.TextPrimary : UiTheme.ButtonDisabledText;
            }

            const int radius = 7;
            if (enabled)
            {
                using (var path = Rounded(edge, radius))
                using (var brush = new SolidBrush(edgeColor))
                {
                    g.FillPath(brush, path);
                }
            }
            using (var path = Rounded(face, radius))
            {
                using (var brush = new SolidBrush(faceColor))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(borderColor))
                {
                    g.DrawPath(pen, path);
                }
            }

            if (!showLamp)
            {
                TextRenderer.DrawText(g, button.Text, button.Font, face, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
                DrawFocus(button, g, face);
                return;
            }

            // 왼쪽 세로 램프
            int lampHeight = Math.Max(12, Math.Min(28, face.Height - 18));
            var lamp = new RectangleF(face.X + 13, face.Y + (face.Height - lampHeight) / 2f, 8, lampHeight);
            DrawLamp(g, lamp, enabled ? state.Tone : LampTone.Off);

            int centerY = face.Y + face.Height / 2;
            int textRight = DrawChip(g, state.Chip, state.Tone, face.Right - 10, centerY, primary) - 8;
            int textLeft = (int)lamp.Right + 14;
            var textRect = new Rectangle(textLeft, face.Y, Math.Max(10, textRight - textLeft), face.Height);
            TextRenderer.DrawText(g, state.Label ?? button.Text, button.Font, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);

            DrawFocus(button, g, face);
        }

        private static void PaintEmergency(Button button, Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(button.Parent?.BackColor ?? UiTheme.Surface);

            bool hover = button.Enabled && hovered.Contains(button);
            FaceRects(button, out var face, out var edge, out _);

            const int radius = 8;
            using (var path = Rounded(edge, radius))
            using (var brush = new SolidBrush(DangerEdge))
            {
                g.FillPath(brush, path);
            }
            using (var path = Rounded(face, radius))
            using (var brush = new SolidBrush(hover ? UiTheme.DangerHover : UiTheme.Danger))
            {
                g.FillPath(brush, path);
            }
            var ring = Rectangle.Inflate(face, -3, -3);
            using (var path = Rounded(ring, radius - 2))
            using (var pen = new Pen(DangerRing, 3f))
            {
                g.DrawPath(pen, path);
            }

            TextRenderer.DrawText(g, button.Text, button.Font, face, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);

            DrawFocus(button, g, face);
        }

        private static void PaintPlate(Control group, Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(group.Parent?.BackColor ?? UiTheme.Surface);

            var rect = new Rectangle(0, 0, group.Width - 1, group.Height - 1);
            using (var path = Rounded(rect, 8))
            {
                using (var brush = new SolidBrush(PlateBack))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(UiTheme.Border))
                {
                    g.DrawPath(pen, path);
                }
            }
            TextRenderer.DrawText(g, group.Text, PlateTitleFont, new Point(group.Padding.Left + 2, 7), UiTheme.TextSecondary, TextFormatFlags.NoPadding);
        }

        private static void DrawFocus(Button button, Graphics g, Rectangle face)
        {
            if (!button.Focused || !keyboardFocus.Contains(button)) return;
            var focus = Rectangle.Inflate(face, -4, -4);
            ControlPaint.DrawFocusRectangle(g, focus);
        }

        private static GraphicsPath Rounded(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            float d = Math.Min(radius * 2f, Math.Min(rect.Width, rect.Height));
            if (d <= 0.5f)
            {
                path.AddRectangle(rect);
                return path;
            }
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
