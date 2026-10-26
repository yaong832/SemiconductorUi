using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace SemiconductorUi.Helpers
{
    /// <summary>왼쪽 메뉴 항목 위에 그리는 작은 그림.</summary>
    public enum ShellNavIcon
    {
        Operate,
        Recipe,
        Maintenance,
        Config,
        Trend,
        Report,
        System
    }

    /// <summary>
    /// 기존 메뉴/탭 버튼을 그대로 쓰면서 모양만 왼쪽 세로 메뉴와 작은 탭으로 그립니다.
    /// 버튼의 클릭 핸들러, Enabled, 이름은 건드리지 않습니다.
    /// </summary>
    public static class ShellNavPainter
    {
        private static readonly Font NavFont = new Font("Segoe UI", 9F);
        private static readonly Font NavFontBold = new Font("Segoe UI", 9F, FontStyle.Bold);
        private static readonly Font TabFont = new Font("Segoe UI", 9.5F);
        private static readonly Font TabFontBold = new Font("Segoe UI", 9.5F, FontStyle.Bold);

        private static readonly Color SelectedSoft = Color.FromArgb(232, 239, 250);

        private static readonly Dictionary<Button, ShellNavIcon> navItems = new Dictionary<Button, ShellNavIcon>();
        private static readonly HashSet<Button> subTabs = new HashSet<Button>();
        private static readonly HashSet<Button> selected = new HashSet<Button>();
        private static readonly HashSet<Button> hovered = new HashSet<Button>();

        public static bool IsShellStyled(Button button)
        {
            return button != null && (navItems.ContainsKey(button) || subTabs.Contains(button));
        }

        /// <summary>선택 표시만 바꿉니다(UiTheme.ApplyToggleButton에서 부릅니다).</summary>
        public static void SetSelected(Button button, bool isSelected)
        {
            if (button == null) return;
            bool changed = isSelected ? selected.Add(button) : selected.Remove(button);
            if (changed) button.Invalidate();
        }

        public static void StyleNavItem(Button button, string text, ShellNavIcon icon, Size size)
        {
            if (button == null) return;
            navItems[button] = icon;
            Prepare(button, text, size);
            button.Paint += PaintNavItem;
        }

        public static void StyleSubTab(Button button, string text, Size size)
        {
            if (button == null) return;
            subTabs.Add(button);
            Prepare(button, text, size);
            button.Paint += PaintSubTab;
        }

        private static void Prepare(Button button, string text, Size size)
        {
            button.Text = text;
            button.Size = size;
            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            EnableDoubleBuffer(button);
            button.MouseEnter += (s, e) => { if (hovered.Add(button)) button.Invalidate(); };
            button.MouseLeave += (s, e) => { if (hovered.Remove(button)) button.Invalidate(); };
            button.EnabledChanged += (s, e) => button.Invalidate();
        }

        private static void EnableDoubleBuffer(Control control)
        {
            try
            {
                var setStyle = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
                setStyle?.Invoke(control, new object[] { ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true });
            }
            catch
            {
                // 깜빡임만 조금 생길 뿐 동작에는 영향이 없습니다.
            }
        }

        private static void PaintNavItem(object sender, PaintEventArgs e)
        {
            var button = (Button)sender;
            var g = e.Graphics;
            var bounds = button.ClientRectangle;
            Color pageBack = button.Parent?.BackColor ?? UiTheme.Surface;
            g.Clear(pageBack);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            bool isSelected = selected.Contains(button);
            bool isHover = hovered.Contains(button) && button.Enabled;
            Color back = isSelected ? SelectedSoft : (isHover ? UiTheme.SurfaceAlt : pageBack);
            Color fore = !button.Enabled ? UiTheme.ButtonDisabledText
                : (isSelected ? UiTheme.Primary : UiTheme.TextSecondary);

            using (var path = RoundedRect(new Rectangle(0, 0, bounds.Width - 1, bounds.Height - 1), 8))
            using (var brush = new SolidBrush(back))
            {
                g.FillPath(brush, path);
            }

            var iconBox = new RectangleF((bounds.Width - 22) / 2f, 9, 22, 22);
            DrawIcon(g, navItems[button], iconBox, fore);

            var textRect = new Rectangle(0, 34, bounds.Width, bounds.Height - 36);
            TextRenderer.DrawText(g, button.Text, isSelected ? NavFontBold : NavFont, textRect, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
        }

        private static void PaintSubTab(object sender, PaintEventArgs e)
        {
            var button = (Button)sender;
            var g = e.Graphics;
            var bounds = button.ClientRectangle;
            Color pageBack = button.Parent?.BackColor ?? UiTheme.Surface;
            g.Clear(pageBack);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            bool isSelected = selected.Contains(button);
            bool isHover = hovered.Contains(button) && button.Enabled;
            Color back = isSelected ? UiTheme.Primary : (isHover ? UiTheme.SurfaceAlt : pageBack);
            Color fore = !button.Enabled ? UiTheme.ButtonDisabledText
                : (isSelected ? Color.White : UiTheme.TextSecondary);

            using (var path = RoundedRect(new Rectangle(0, 0, bounds.Width - 1, bounds.Height - 1), 6))
            using (var brush = new SolidBrush(back))
            {
                g.FillPath(brush, path);
            }
            TextRenderer.DrawText(g, button.Text, isSelected ? TabFontBold : TabFont, bounds, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
        }

        private static void DrawIcon(Graphics g, ShellNavIcon icon, RectangleF box, Color color)
        {
            float x = box.X;
            float y = box.Y;
            float s = box.Width / 24f;
            Func<float, float, PointF> p = (px, py) => new PointF(x + px * s, y + py * s);

            using (var pen = new Pen(color, 1.7f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
            using (var brush = new SolidBrush(color))
            {
                switch (icon)
                {
                    case ShellNavIcon.Operate:
                        g.DrawEllipse(pen, x + 3 * s, y + 3 * s, 18 * s, 18 * s);
                        g.FillPolygon(brush, new[] { p(10, 8.5f), p(10, 15.5f), p(15.5f, 12) });
                        break;
                    case ShellNavIcon.Recipe:
                        using (var path = RoundedRect(Rectangle.Round(new RectangleF(x + 5 * s, y + 3.5f * s, 14 * s, 17 * s)), (int)(2 * s)))
                        {
                            g.DrawPath(pen, path);
                        }
                        g.DrawLine(pen, p(9, 8.5f), p(15, 8.5f));
                        g.DrawLine(pen, p(9, 12), p(15, 12));
                        g.DrawLine(pen, p(9, 15.5f), p(13, 15.5f));
                        break;
                    case ShellNavIcon.Maintenance:
                        // 렌치: 둥근 머리 + 손잡이
                        g.DrawArc(pen, x + 11 * s, y + 3 * s, 10 * s, 10 * s, 120, 270);
                        g.DrawLine(pen, p(13, 11), p(5, 19));
                        g.DrawLine(pen, p(5, 19), p(7, 21));
                        g.DrawLine(pen, p(7, 21), p(15, 13));
                        break;
                    case ShellNavIcon.Config:
                        g.DrawLine(pen, p(5, 7), p(19, 7));
                        g.DrawLine(pen, p(5, 12), p(19, 12));
                        g.DrawLine(pen, p(5, 17), p(19, 17));
                        DrawKnob(g, pen, p(9, 7), 2 * s);
                        DrawKnob(g, pen, p(15, 12), 2 * s);
                        DrawKnob(g, pen, p(8, 17), 2 * s);
                        break;
                    case ShellNavIcon.Trend:
                        g.DrawLine(pen, p(4, 19), p(20, 19));
                        g.DrawLines(pen, new[] { p(5, 15), p(9, 10), p(13, 13), p(19, 6) });
                        break;
                    case ShellNavIcon.Report:
                        g.DrawLines(pen, new[] { p(14, 3.5f), p(7, 3.5f), p(7, 20.5f), p(18, 20.5f), p(18, 7.5f), p(14, 3.5f), p(14, 7.5f), p(18, 7.5f) });
                        g.DrawLine(pen, p(10, 13), p(15, 13));
                        g.DrawLine(pen, p(10, 16.5f), p(15, 16.5f));
                        break;
                    case ShellNavIcon.System:
                        g.DrawEllipse(pen, x + 3 * s, y + 3 * s, 18 * s, 18 * s);
                        g.DrawLine(pen, p(12, 11), p(12, 16.5f));
                        g.FillEllipse(brush, x + 11.1f * s, y + 6.9f * s, 1.8f * s, 1.8f * s);
                        break;
                }
            }
        }

        private static void DrawKnob(Graphics g, Pen pen, PointF center, float r)
        {
            using (var fill = new SolidBrush(UiTheme.Surface))
            {
                g.FillEllipse(fill, center.X - r, center.Y - r, r * 2, r * 2);
            }
            g.DrawEllipse(pen, center.X - r, center.Y - r, r * 2, r * 2);
        }

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            if (d <= 0)
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
