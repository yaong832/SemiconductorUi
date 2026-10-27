using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Controls
{
    /// <summary>
    /// 상단 바에 크게 보여 줄 장비 상태 한 가지(글자와 색).
    /// </summary>
    public struct ShellStatePill
    {
        public string Text;
        public Color Dot;
        public Color Fore;
        public Color Back;

        public ShellStatePill(string text, Color dot, Color fore, Color back)
        {
            Text = text;
            Dot = dot;
            Fore = fore;
            Back = back;
        }
    }

    /// <summary>
    /// 메인 화면 맨 위 한 줄: 장비 이름, 큰 상태 표시, 오른쪽에 연결·서보·알람·시계.
    /// 상태 표시는 그릴 때마다 StateProvider에서 읽으므로 화면 표시만 하고 장비에 명령을 보내지 않습니다.
    /// </summary>
    public sealed class ShellTopBar : Panel
    {
        public const int BarHeight = 60;
        private const int BrandWidth = 236;
        private const int CompactBrandWidth = 70;

        private static readonly Font TitleFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        private static readonly Font SubFont = new Font("Segoe UI", 8F);
        private static readonly Font PillFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        private static readonly Font LargeTitleFont = new Font("Segoe UI", 12.5F, FontStyle.Bold);
        private static readonly Font LargeSubFont = new Font("Segoe UI", 9F);
        private static readonly Font LargePillFont = new Font("Segoe UI", 12.5F, FontStyle.Bold);

        private bool large;

        private readonly FlowLayoutPanel rightArea;

        public ShellTopBar()
        {
            DoubleBuffered = true;
            BackColor = UiTheme.Surface;
            Dock = DockStyle.Fill;
            Margin = Padding.Empty;
            // 아래 1px은 구분선 자리로 비워 둡니다.
            Padding = new Padding(0, 0, 0, 1);

            rightArea = new FlowLayoutPanel
            {
                Name = "flowShellTopRight",
                Dock = DockStyle.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = UiTheme.Surface,
                Padding = new Padding(0, 0, 16, 0),
                Margin = Padding.Empty
            };
            Controls.Add(rightArea);
            // 오른쪽 줄 너비가 바뀌면 장비 이름을 줄일지 다시 정합니다.
            rightArea.LocationChanged += (s, e) => Invalidate();
        }

        /// <summary>오른쪽 칩·버튼이 들어가는 줄.</summary>
        public FlowLayoutPanel RightArea => rightArea;

        public Func<ShellStatePill> StateProvider { get; set; }

        /// <summary>
        /// 넓은 화면용 큰 글자. 오른쪽 줄 컨트롤 크기는 바꾼 쪽에서 정하고, 여기서는 세로 가운데만 다시 맞춥니다.
        /// </summary>
        public bool Large
        {
            get => large;
            set
            {
                if (large == value) return;
                large = value;
                foreach (Control control in rightArea.Controls)
                {
                    // 세로 구분선
                    if (control.Width == 1) control.Height = large ? 32 : 28;
                }
                RecenterRightArea();
                Invalidate();
            }
        }

        /// <summary>오른쪽 줄 컨트롤의 높이가 바뀐 뒤 세로 가운데를 다시 맞춥니다.</summary>
        public void RecenterRightArea()
        {
            foreach (Control control in rightArea.Controls)
            {
                int top = Math.Max(0, (BarHeight - 1 - control.Height) / 2);
                control.Margin = new Padding(control.Margin.Left, top, 0, 0);
            }
        }

        /// <summary>
        /// 오른쪽 줄에 넣을 컨트롤의 위아래 여백을 맞춰 세로 가운데에 오게 합니다.
        /// </summary>
        public void AddRight(Control control, int leftGap)
        {
            int top = Math.Max(0, (BarHeight - 1 - control.Height) / 2);
            control.Margin = new Padding(leftGap, top, 0, 0);
            rightArea.Controls.Add(control);
        }

        /// <summary>오른쪽 줄 사이에 넣는 얇은 세로 구분선.</summary>
        public void AddDivider()
        {
            var divider = new Panel
            {
                Size = new Size(1, 28),
                BackColor = UiTheme.Border
            };
            AddRight(divider, 12);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 큰 상태 표시의 크기를 먼저 재서, 창이 좁아 오른쪽 줄과 겹칠 것 같으면 장비 이름 글자를 빼고 로고만 둡니다.
            ShellStatePill? state = StateProvider?.Invoke();
            var titleFont = large ? LargeTitleFont : TitleFont;
            var subFont = large ? LargeSubFont : SubFont;
            var pillFont = large ? LargePillFont : PillFont;
            int pillHeight = large ? 38 : 34;
            int pillWidth = 0;
            Size pillTextSize = Size.Empty;
            if (state.HasValue)
            {
                pillTextSize = TextRenderer.MeasureText(g, state.Value.Text, pillFont, Size.Empty, TextFormatFlags.NoPadding);
                pillWidth = pillTextSize.Width + 46;
            }
            int fullBrandWidth = large ? BrandWidth + 24 : BrandWidth;
            int brandWidth = fullBrandWidth;
            if (fullBrandWidth + 18 + pillWidth + 12 > rightArea.Left)
            {
                brandWidth = CompactBrandWidth;
            }

            // 장비 이름
            var logo = new Rectangle(20, (BarHeight - 30) / 2, 30, 30);
            using (var path = ShellDraw.RoundedRect(logo, 7))
            using (var brush = new SolidBrush(UiTheme.Primary))
            {
                g.FillPath(brush, path);
            }
            using (var pen = new Pen(Color.White, 1.8f))
            {
                g.DrawEllipse(pen, logo.X + 8, logo.Y + 8, 14, 14);
            }
            using (var pen = new Pen(Color.FromArgb(150, Color.White), 1.4f))
            {
                g.DrawLine(pen, logo.X + 11, logo.Y + 15, logo.Right - 11, logo.Y + 15);
                g.DrawLine(pen, logo.X + 15, logo.Y + 11, logo.X + 15, logo.Bottom - 11);
            }
            if (brandWidth == fullBrandWidth)
            {
                TextRenderer.DrawText(g, "웨이퍼 이송 장비", titleFont, new Point(logo.Right + 10, large ? 9 : 12), UiTheme.TextPrimary, TextFormatFlags.NoPadding);
                TextRenderer.DrawText(g, "SemiconductorUi", subFont, new Point(logo.Right + 11, large ? 34 : 33), UiTheme.TextSecondary, TextFormatFlags.NoPadding);
            }

            using (var pen = new Pen(UiTheme.Border))
            {
                g.DrawLine(pen, brandWidth, 16, brandWidth, BarHeight - 17);
                g.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            }

            // 큰 상태 표시
            if (state.HasValue)
            {
                var pill = new Rectangle(brandWidth + 18, (BarHeight - pillHeight) / 2, pillWidth, pillHeight);
                using (var path = ShellDraw.RoundedRect(pill, pillHeight / 2))
                using (var brush = new SolidBrush(state.Value.Back))
                {
                    g.FillPath(brush, path);
                }
                using (var brush = new SolidBrush(state.Value.Dot))
                {
                    g.FillEllipse(brush, pill.X + 15, pill.Y + (pill.Height - 10) / 2, 10, 10);
                }
                TextRenderer.DrawText(g, state.Value.Text, pillFont,
                    new Point(pill.X + 32, pill.Y + (pill.Height - pillTextSize.Height) / 2),
                    state.Value.Fore, TextFormatFlags.NoPadding);
            }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            Invalidate();
        }
    }

    /// <summary>
    /// 상단 바의 작은 상태 표시(점 + 짧은 글자). 글자와 점 색은 그릴 때마다 읽어 옵니다.
    /// </summary>
    public sealed class ShellChip : Control
    {
        private const int ChipHeight = 28;
        private const int LargeChipHeight = 32;
        private static readonly Font ChipFont = new Font("Segoe UI", 9F);
        private static readonly Font LargeChipFont = new Font("Segoe UI", 10F);

        private bool large;

        public ShellChip()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = UiTheme.Surface;
            Size = new Size(120, ChipHeight);
            MaxChipWidth = 260;
        }

        public Func<string> TextProvider { get; set; }
        public Func<Color> DotProvider { get; set; }
        public int MaxChipWidth { get; set; }

        /// <summary>넓은 화면용 큰 글자와 높이.</summary>
        public bool Large
        {
            get => large;
            set
            {
                if (large == value) return;
                large = value;
                Height = large ? LargeChipHeight : ChipHeight;
                RefreshChip();
            }
        }

        private Font CurrentFont => large ? LargeChipFont : ChipFont;

        /// <summary>글자에 맞춰 너비를 다시 잡고 다시 그립니다.</summary>
        public void RefreshChip()
        {
            string text = TextProvider?.Invoke() ?? "";
            int width = TextRenderer.MeasureText(text, CurrentFont, Size.Empty, TextFormatFlags.NoPadding).Width + 30;
            int maxWidth = large ? MaxChipWidth + MaxChipWidth / 8 : MaxChipWidth;
            width = Math.Min(maxWidth, Math.Max(60, width));
            if (Width != width)
            {
                Width = width;
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Parent?.BackColor ?? UiTheme.Surface);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color dot = DotProvider?.Invoke() ?? UiTheme.TextSecondary;
            ShellDraw.ToneFor(dot, out var back, out var fore, out var dotColor);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = ShellDraw.RoundedRect(rect, Height / 2))
            using (var brush = new SolidBrush(back))
            {
                g.FillPath(brush, path);
            }
            using (var brush = new SolidBrush(dotColor))
            {
                g.FillEllipse(brush, 11, (Height - 8) / 2, 8, 8);
            }
            var textRect = new Rectangle(24, 0, Width - 32, Height);
            TextRenderer.DrawText(g, TextProvider?.Invoke() ?? "", CurrentFont, textRect, fore,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
        }
    }

    /// <summary>
    /// 상단 바 오른쪽 끝 시계. 1초 시계 타이머가 기존 시각 글자를 바꿀 때 함께 다시 그립니다.
    /// </summary>
    public sealed class ShellClock : Control
    {
        private static readonly Font TimeFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        private static readonly Font DateFont = new Font("Segoe UI", 8F);
        private static readonly Font LargeTimeFont = new Font("Segoe UI", 13.5F, FontStyle.Bold);
        private static readonly Font LargeDateFont = new Font("Segoe UI", 9F);

        private bool large;

        public ShellClock()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = UiTheme.Surface;
            Size = new Size(104, 40);
        }

        /// <summary>넓은 화면용 큰 글자.</summary>
        public bool Large
        {
            get => large;
            set
            {
                if (large == value) return;
                large = value;
                Size = large ? new Size(120, 46) : new Size(104, 40);
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            var now = DateTime.Now;
            using (var pen = new Pen(UiTheme.Border))
            {
                g.DrawLine(pen, 0, 6, 0, Height - 7);
            }
            TextRenderer.DrawText(g, now.ToString("HH:mm:ss"), large ? LargeTimeFont : TimeFont, new Point(14, 1), UiTheme.TextPrimary, TextFormatFlags.NoPadding);
            TextRenderer.DrawText(g, now.ToString("yyyy-MM-dd"), large ? LargeDateFont : DateFont, new Point(15, large ? 27 : 23), UiTheme.TextSecondary, TextFormatFlags.NoPadding);
        }
    }

    /// <summary>
    /// 왼쪽 메뉴 아래쪽의 로그인 사용자 표시(동그란 첫 글자 + 이름 + 권한).
    /// </summary>
    public sealed class ShellUserCard : Control
    {
        private static readonly Font InitialFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        private static readonly Font NameFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        private static readonly Font RoleFont = new Font("Segoe UI", 8F);

        public ShellUserCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = UiTheme.Surface;
            Size = new Size(80, 66);
        }

        public Func<string> UserProvider { get; set; }
        public Func<string> RoleProvider { get; set; }
        public Func<bool> LoggedInProvider { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            bool loggedIn = LoggedInProvider?.Invoke() ?? false;
            string user = UserProvider?.Invoke() ?? "Guest";
            string role = loggedIn ? (RoleProvider?.Invoke() ?? "") : "로그인 필요";

            var avatar = new Rectangle((Width - 28) / 2, 2, 28, 28);
            using (var brush = new SolidBrush(loggedIn ? Color.FromArgb(232, 239, 250) : UiTheme.SurfaceAlt))
            {
                g.FillEllipse(brush, avatar);
            }
            using (var pen = new Pen(loggedIn ? UiTheme.Primary : UiTheme.Border))
            {
                g.DrawEllipse(pen, avatar);
            }
            string initial = string.IsNullOrEmpty(user) ? "?" : user.Substring(0, 1).ToUpperInvariant();
            TextRenderer.DrawText(g, initial, InitialFont, avatar, loggedIn ? UiTheme.Primary : UiTheme.TextSecondary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            var nameRect = new Rectangle(0, 33, Width, 16);
            TextRenderer.DrawText(g, user, NameFont, nameRect, UiTheme.TextPrimary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
            var roleRect = new Rectangle(0, 50, Width, 14);
            TextRenderer.DrawText(g, role, RoleFont, roleRect, UiTheme.TextSecondary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
        }
    }

    internal static class ShellDraw
    {
        public static GraphicsPath RoundedRect(Rectangle rect, int radius)
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

        /// <summary>
        /// 점 색 하나로 칩의 바탕·글자 색을 고릅니다. 빨강은 실제 오류일 때만 나옵니다.
        /// </summary>
        public static void ToneFor(Color dot, out Color back, out Color fore, out Color dotColor)
        {
            if (Same(dot, UiTheme.Success))
            {
                back = Color.FromArgb(228, 243, 234);
                fore = Color.FromArgb(28, 122, 73);
                dotColor = UiTheme.Success;
            }
            else if (Same(dot, UiTheme.Danger))
            {
                back = Color.FromArgb(251, 233, 234);
                fore = Color.FromArgb(168, 38, 48);
                dotColor = UiTheme.Danger;
            }
            else if (Same(dot, UiTheme.Warning))
            {
                back = Color.FromArgb(252, 241, 222);
                fore = Color.FromArgb(150, 92, 8);
                dotColor = UiTheme.Warning;
            }
            else if (Same(dot, UiTheme.Primary))
            {
                back = Color.FromArgb(232, 239, 250);
                fore = UiTheme.Primary;
                dotColor = UiTheme.Primary;
            }
            else
            {
                back = UiTheme.SurfaceAlt;
                fore = UiTheme.TextSecondary;
                dotColor = Color.FromArgb(140, 150, 164);
            }
        }

        private static bool Same(Color a, Color b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B;
        }
    }
}
