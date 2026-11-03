using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Controls
{
    /// <summary>상세 창 한 번에 보여 줄 내용 (챔버 또는 FOUP)</summary>
    internal sealed class UnitDetailContent
    {
        public string Title = "";
        public string Status = "";
        public LampTone Tone = LampTone.Off;
        public bool Live;
        public readonly List<KeyValuePair<string, string>> Info = new List<KeyValuePair<string, string>>();
        public readonly List<KeyValuePair<string, bool>> Chips = new List<KeyValuePair<string, bool>>();
        /// <summary>환경 표: 이름, PV, SV, 단위. 없으면 null</summary>
        public string[][] Env;
        /// <summary>FOUP 슬롯: 위(슬롯 5)부터 아래(슬롯 1) 순서. 글자와 웨이퍼 있음 여부. 없으면 null</summary>
        public List<KeyValuePair<string, bool>> Slots;
        /// <summary>진행률(0~100). 없으면 null</summary>
        public int? Progress;
        public string ProgressText = "";
    }

    /// <summary>
    /// 장비 그림에서 챔버나 FOUP를 눌렀을 때 그 옆에 뜨는 작은 상세 창.
    /// 메인 화면 위에 놓인 일반 컨트롤이라 화면을 잠그지 않습니다(긴급 정지, 일시 정지 등은 그대로 누를 수 있음).
    /// 내용은 화면 갱신 때마다 SetContent로 바뀌고, 오른쪽 위 X로 닫습니다.
    /// </summary>
    internal sealed class UnitDetailPopup : Control
    {
        private const int BaseWidth = 520;
        private const int HeaderHeight = 38;
        private const int Pad = 14;
        private const int RowHeight = 21;
        private const int SlotHeight = 24;

        private UnitDetailContent content = new UnitDetailContent();
        private float scale = 1f;
        private Font titleFont, textFont, boldFont, smallFont, numFont;
        private Rectangle closeRect;
        private bool closeHover;
        private bool pressedOnClose;
        private bool dragging;
        private Point dragGrip;

        /// <summary>X를 눌러 닫을 때</summary>
        public event EventHandler CloseRequested;

        /// <summary>
        /// 머리줄을 잡고 끄는 중일 때. 값은 부모 기준으로 창 왼쪽 위가 가고 싶은 자리이며,
        /// 실제 자리(제어 패널을 가리지 않도록 제한)는 받는 쪽이 정합니다.
        /// </summary>
        public event EventHandler<Point> DragMoving;

        /// <summary>지금 보여 주는 대상 (예: "PMA", "FOUP A")</summary>
        public string UnitKey { get; set; }

        public UnitDetailPopup()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = UiTheme.Surface;
            TabStop = false;
            CreateFonts();
            Size = new Size(BaseWidth, 200);
        }

        /// <summary>글자와 칸 크기 배율 (캔버스 배율을 따름)</summary>
        public float DetailScale
        {
            get => scale;
            set
            {
                float v = Math.Max(0.85f, Math.Min(1.6f, value));
                if (Math.Abs(v - scale) < 0.01f) return;
                scale = v;
                CreateFonts();
                UpdateHeight();
                Invalidate();
            }
        }

        public void SetContent(UnitDetailContent value)
        {
            content = value ?? new UnitDetailContent();
            UpdateHeight();
            Invalidate();
        }

        private int S(int v) => (int)Math.Round(v * scale);

        private void CreateFonts()
        {
            titleFont?.Dispose(); textFont?.Dispose(); boldFont?.Dispose(); smallFont?.Dispose(); numFont?.Dispose();
            titleFont = new Font("Segoe UI", 11.5f * scale, FontStyle.Bold);
            textFont = new Font("Segoe UI", 9f * scale, FontStyle.Regular);
            boldFont = new Font("Segoe UI", 9f * scale, FontStyle.Bold);
            smallFont = new Font("Segoe UI", 8f * scale, FontStyle.Bold);
            numFont = new Font("Consolas", 9f * scale, FontStyle.Regular);
        }

        private int LeftHeight()
        {
            int h = content.Info.Count * S(RowHeight);
            if (content.Chips.Count > 0) h += S(8) + S(24);
            return h;
        }

        private int RightHeight()
        {
            if (content.Env != null) return S(RowHeight) + content.Env.Length * S(RowHeight);
            if (content.Slots != null) return content.Slots.Count * (S(SlotHeight) + S(4));
            return 0;
        }

        private void UpdateHeight()
        {
            int w = S(BaseWidth);
            int h = S(HeaderHeight) + S(Pad) + Math.Max(LeftHeight(), RightHeight()) + S(Pad);
            if (content.Progress.HasValue) h += S(RowHeight) + S(10) + S(Pad) / 2;
            if (Width != w || Height != h) Size = new Size(w, h);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = new Rectangle(0, 0, Width - 1, Height - 1);

            using (var back = new SolidBrush(UiTheme.Surface)) g.FillRectangle(back, ClientRectangle);

            // 머리줄: 램프, 이름, 상태 칩, 실시간, 닫기
            var header = new Rectangle(0, 0, Width, S(HeaderHeight));
            using (var hb = new SolidBrush(UiTheme.SurfaceAlt)) g.FillRectangle(hb, header);
            using (var pen = new Pen(UiTheme.Border)) g.DrawLine(pen, 0, header.Bottom, Width, header.Bottom);

            int x = S(Pad);
            float lamp = S(12);
            LampButtonPainter.DrawLamp(g, new RectangleF(x, (header.Height - lamp) / 2f, lamp, lamp), content.Tone);
            x += (int)lamp + S(8);

            var titleSize = TextRenderer.MeasureText(g, content.Title, titleFont, Size.Empty, TextFormatFlags.NoPadding);
            TextRenderer.DrawText(g, content.Title, titleFont, new Rectangle(x, 0, titleSize.Width + 2, header.Height), UiTheme.TextPrimary,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            x += titleSize.Width + S(10);

            if (!string.IsNullOrEmpty(content.Status))
            {
                ToneColors(content.Tone, out var pillBack, out var pillFore);
                var ps = TextRenderer.MeasureText(g, content.Status, smallFont, Size.Empty, TextFormatFlags.NoPadding);
                var pill = new Rectangle(x, (header.Height - S(20)) / 2, ps.Width + S(18), S(20));
                using (var path = ShellDraw.RoundedRect(pill, pill.Height / 2))
                using (var pb = new SolidBrush(pillBack)) g.FillPath(pb, path);
                TextRenderer.DrawText(g, content.Status, smallFont, pill, pillFore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                x = pill.Right + S(10);
            }

            if (content.Live)
            {
                float dot = S(7);
                using (var db = new SolidBrush(UiTheme.Success)) g.FillEllipse(db, x, (header.Height - dot) / 2f, dot, dot);
                TextRenderer.DrawText(g, "실시간", smallFont, new Rectangle(x + (int)dot + S(4), 0, S(60), header.Height), UiTheme.Success,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }

            int cs = S(24);
            closeRect = new Rectangle(Width - S(Pad) - cs + S(4), (header.Height - cs) / 2, cs, cs);
            using (var path = ShellDraw.RoundedRect(closeRect, S(5)))
            {
                using (var cb = new SolidBrush(closeHover ? UiTheme.ButtonHover : UiTheme.Surface)) g.FillPath(cb, path);
                using (var cp = new Pen(UiTheme.Border)) g.DrawPath(cp, path);
            }
            using (var xp = new Pen(UiTheme.TextPrimary, Math.Max(1.5f, 1.6f * scale)))
            {
                int m = S(8);
                g.DrawLine(xp, closeRect.Left + m, closeRect.Top + m, closeRect.Right - m, closeRect.Bottom - m);
                g.DrawLine(xp, closeRect.Right - m, closeRect.Top + m, closeRect.Left + m, closeRect.Bottom - m);
            }

            // 본문: 왼쪽 정보, 오른쪽 환경 표 또는 슬롯
            int top = header.Bottom + S(Pad);
            int inner = Width - S(Pad) * 2;
            int leftW = (int)(inner * 0.47f);
            int rightX = S(Pad) + leftW + S(Pad);
            int rightW = Width - S(Pad) - rightX;

            int y = top;
            int labelW = S(78);
            foreach (var kv in content.Info)
            {
                TextRenderer.DrawText(g, kv.Key, textFont, new Rectangle(S(Pad), y, labelW, S(RowHeight)), UiTheme.TextSecondary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                TextRenderer.DrawText(g, kv.Value ?? "-", boldFont, new Rectangle(S(Pad) + labelW, y, leftW - labelW, S(RowHeight)), UiTheme.TextPrimary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
                y += S(RowHeight);
            }

            if (content.Chips.Count > 0)
            {
                y += S(8);
                int cx = S(Pad);
                foreach (var chip in content.Chips)
                {
                    var size = TextRenderer.MeasureText(g, chip.Key, smallFont, Size.Empty, TextFormatFlags.NoPadding);
                    var r = new Rectangle(cx, y, size.Width + S(18), S(22));
                    Color cb = chip.Value ? Color.FromArgb(226, 243, 234) : UiTheme.SurfaceAlt;
                    Color cf = chip.Value ? UiTheme.Success : UiTheme.TextSecondary;
                    Color border = chip.Value ? Color.FromArgb(191, 227, 207) : UiTheme.Border;
                    using (var path = ShellDraw.RoundedRect(r, r.Height / 2))
                    {
                        using (var b = new SolidBrush(cb)) g.FillPath(b, path);
                        using (var p = new Pen(border)) g.DrawPath(p, path);
                    }
                    TextRenderer.DrawText(g, chip.Key, smallFont, r, cf,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                    cx = r.Right + S(6);
                }
            }

            if (content.Env != null)
            {
                DrawEnvTable(g, rightX, top, rightW);
            }
            else if (content.Slots != null)
            {
                DrawSlots(g, rightX, top, rightW);
            }

            if (content.Progress.HasValue)
            {
                int py = top + Math.Max(LeftHeight(), RightHeight()) + S(Pad) / 2;
                TextRenderer.DrawText(g, content.ProgressText ?? "", textFont, new Rectangle(S(Pad), py, inner, S(RowHeight)), UiTheme.TextPrimary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
                var track = new Rectangle(S(Pad), py + S(RowHeight) + S(2), inner, S(8));
                using (var path = ShellDraw.RoundedRect(track, track.Height / 2))
                {
                    using (var tb = new SolidBrush(UiTheme.SurfaceAlt)) g.FillPath(tb, path);
                    using (var tp = new Pen(UiTheme.Border)) g.DrawPath(tp, path);
                }
                int pct = Math.Max(0, Math.Min(100, content.Progress.Value));
                if (pct > 0)
                {
                    var fill = new Rectangle(track.X, track.Y, Math.Max(track.Height, track.Width * pct / 100), track.Height);
                    using (var path = ShellDraw.RoundedRect(fill, fill.Height / 2))
                    using (var fb = new SolidBrush(UiTheme.Primary)) g.FillPath(fb, path);
                }
            }

            using (var pen = new Pen(UiTheme.Primary, 2f))
            {
                g.DrawRectangle(pen, new Rectangle(1, 1, Width - 2, Height - 2));
            }
        }

        private void DrawEnvTable(Graphics g, int x, int y, int width)
        {
            int nameW = (int)(width * 0.22f);
            int unitW = (int)(width * 0.18f);
            int valW = (width - nameW - unitW) / 2;
            int rh = S(RowHeight);
            var right = TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;
            var left = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;

            TextRenderer.DrawText(g, "현재값 PV", smallFont, new Rectangle(x + nameW, y, valW - S(6), rh), Color.FromArgb(28, 122, 73), right);
            TextRenderer.DrawText(g, "설정값 SV", smallFont, new Rectangle(x + nameW + valW, y, valW - S(6), rh), UiTheme.Primary, right);
            using (var pen = new Pen(UiTheme.Border)) g.DrawLine(pen, x, y + rh - 1, x + width, y + rh - 1);
            y += rh;

            foreach (var row in content.Env)
            {
                if (row == null || row.Length < 4) continue;
                bool hasPv = row[1] != "-";
                TextRenderer.DrawText(g, row[0], textFont, new Rectangle(x, y, nameW, rh), UiTheme.TextPrimary, left);
                TextRenderer.DrawText(g, row[1], hasPv ? boldFont : numFont, new Rectangle(x + nameW, y, valW - S(6), rh),
                    hasPv ? Color.FromArgb(28, 122, 73) : UiTheme.ButtonDisabledText, right);
                TextRenderer.DrawText(g, row[2], numFont, new Rectangle(x + nameW + valW, y, valW - S(6), rh),
                    row[2] != "-" ? UiTheme.Primary : UiTheme.ButtonDisabledText, right);
                TextRenderer.DrawText(g, row[3], textFont, new Rectangle(x + nameW + valW * 2 + S(4), y, unitW, rh), UiTheme.TextSecondary, left);
                using (var pen = new Pen(UiTheme.SurfaceAlt)) g.DrawLine(pen, x, y + rh - 1, x + width, y + rh - 1);
                y += rh;
            }
        }

        private void DrawSlots(Graphics g, int x, int y, int width)
        {
            foreach (var slot in content.Slots)
            {
                var r = new Rectangle(x, y, width, S(SlotHeight));
                Color back = slot.Value ? Color.FromArgb(232, 239, 250) : UiTheme.SurfaceAlt;
                Color border = slot.Value ? Color.FromArgb(160, 186, 230) : UiTheme.Border;
                using (var path = ShellDraw.RoundedRect(r, S(4)))
                {
                    using (var b = new SolidBrush(back)) g.FillPath(b, path);
                    using (var p = new Pen(border)) g.DrawPath(p, path);
                }
                float lamp = S(9);
                LampButtonPainter.DrawLamp(g, new RectangleF(r.X + S(8), r.Y + (r.Height - lamp) / 2f, lamp, lamp), slot.Value ? LampTone.Blue : LampTone.Off);
                TextRenderer.DrawText(g, slot.Key, slot.Value ? boldFont : textFont,
                    new Rectangle(r.X + S(24), r.Y, r.Width - S(30), r.Height), slot.Value ? UiTheme.Primary : UiTheme.TextSecondary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
                y += S(SlotHeight) + S(4);
            }
        }

        private static void ToneColors(LampTone tone, out Color back, out Color fore)
        {
            switch (tone)
            {
                case LampTone.Green:
                    back = Color.FromArgb(226, 243, 234); fore = UiTheme.Success; break;
                case LampTone.Amber:
                    back = Color.FromArgb(251, 240, 220); fore = Color.FromArgb(154, 98, 8); break;
                case LampTone.Red:
                    back = Color.FromArgb(250, 228, 230); fore = UiTheme.Danger; break;
                case LampTone.Blue:
                    back = Color.FromArgb(227, 236, 251); fore = UiTheme.Primary; break;
                default:
                    back = UiTheme.ButtonFill; fore = UiTheme.TextSecondary; break;
            }
        }

        // 머리줄 중 닫기 단추를 뺀 곳을 잡고 끌 수 있습니다.
        private bool IsDragArea(Point p) => p.Y >= 0 && p.Y < S(HeaderHeight) && !closeRect.Contains(p);

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            pressedOnClose = closeRect.Contains(e.Location);
            if (IsDragArea(e.Location))
            {
                dragging = true;
                dragGrip = e.Location;
                Capture = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (dragging)
            {
                dragging = false;
                Capture = false;
            }
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
            dragging = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (dragging && Parent != null)
            {
                var cursor = Parent.PointToClient(Control.MousePosition);
                DragMoving?.Invoke(this, new Point(cursor.X - dragGrip.X, cursor.Y - dragGrip.Y));
                return;
            }

            bool over = closeRect.Contains(e.Location);
            Cursor = over ? Cursors.Hand : IsDragArea(e.Location) ? Cursors.SizeAll : Cursors.Default;
            if (over != closeHover)
            {
                closeHover = over;
                Invalidate(closeRect);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (closeHover)
            {
                closeHover = false;
                Invalidate(closeRect);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            // 끌다가 닫기 단추 위에서 놓은 경우는 닫지 않습니다 (눌렀을 때도 단추 위였어야 함).
            if (e.Button == MouseButtons.Left && pressedOnClose && closeRect.Contains(e.Location))
            {
                CloseRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                titleFont?.Dispose(); textFont?.Dispose(); boldFont?.Dispose(); smallFont?.Dispose(); numFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
