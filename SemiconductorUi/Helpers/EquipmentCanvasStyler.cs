using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// 메인 화면 장비 시뮬레이터(캔버스)의 공통 색상과 카드 스타일.
    /// 기존 패널(도어/램프/웨이퍼)은 상태 보관용으로 그대로 두고, 카드 모양은 Paint에서 직접 그린다.
    /// </summary>
    public static class EquipmentCanvasStyler
    {
        public static readonly Color CanvasBack = Color.FromArgb(236, 239, 244);
        public static readonly Color CorridorFill = Color.FromArgb(222, 227, 235);
        public static readonly Color CorridorEdge = Color.FromArgb(200, 207, 218);
        public static readonly Color TransferChamberFill = Color.FromArgb(210, 217, 228);
        public static readonly Color TransferChamberEdge = Color.FromArgb(160, 170, 186);
        public static readonly Color CardFill = Color.White;
        public static readonly Color CardBorder = Color.FromArgb(184, 192, 205);
        public static readonly Color CardTitle = Color.FromArgb(38, 46, 60);
        public static readonly Color CardSubText = Color.FromArgb(104, 114, 130);
        public static readonly Color WellFill = Color.FromArgb(228, 233, 240);
        public static readonly Color WellEdge = Color.FromArgb(170, 179, 193);
        public static readonly Color ValveClosed = Color.FromArgb(72, 84, 102);
        public static readonly Color LampOff = Color.FromArgb(60, 60, 60);

        public const int ChamberWidth = 176;
        public const int ChamberHeight = 100;
        public const int FoupWidth = 176;
        public const int FoupHeight = 96;
        public const int CardRadius = 10;
        public const int HeaderHeight = 28;
        public const int WellSize = 56;

        /// <summary>
        /// 챔버 패널을 카드 형태로 바꾼다. valveSide는 TM을 향한 변(슬릿 밸브 위치)이다.
        /// </summary>
        public static void ApplyChamberCard(Panel chamber, Label title, Panel door, Panel wafer, Panel lamp, AnchorStyles valveSide)
        {
            if (chamber == null)
            {
                return;
            }

            chamber.SuspendLayout();
            chamber.BorderStyle = BorderStyle.None;
            chamber.BackColor = CanvasBack;
            chamber.Size = new Size(ChamberWidth, ChamberHeight);
            chamber.Cursor = Cursors.Hand;
            EnableDoubleBuffer(chamber);

            var well = GetWellBounds(chamber.ClientRectangle);

            // 도어 패널은 숨기되 크기(열림 애니메이션 높이)와 색상은 상태 값으로 계속 사용한다.
            if (door != null)
            {
                door.Visible = false;
                door.Bounds = well;
                door.SizeChanged += (s, e) => chamber.Invalidate();
                door.BackColorChanged += (s, e) => chamber.Invalidate();
            }

            if (lamp != null)
            {
                lamp.Visible = false;
                lamp.BackColorChanged += (s, e) => chamber.Invalidate();
            }

            if (title != null)
            {
                title.Visible = false;
                title.TextChanged += (s, e) => chamber.Invalidate();
            }

            if (wafer != null)
            {
                wafer.Location = new Point(
                    well.X + (well.Width - wafer.Width) / 2,
                    well.Y + (well.Height - wafer.Height) / 2);
                wafer.BringToFront();
            }

            int closedHeight = well.Height;
            chamber.Paint += (s, e) => PaintChamberCard(e.Graphics, chamber.ClientRectangle, title?.Text ?? chamber.Name,
                lamp?.BackColor ?? LampOff, GetValveOpenRatio(door, closedHeight), valveSide);
            chamber.ResumeLayout(false);
            chamber.Invalidate();
        }

        /// <summary>
        /// 웨이퍼가 놓이는 원형 스테이지 영역 (카드 본문 중앙).
        /// </summary>
        public static Rectangle GetWellBounds(Rectangle card)
        {
            int bodyTop = card.Y + HeaderHeight;
            int bodyHeight = card.Height - HeaderHeight;
            int size = Math.Min(WellSize, Math.Max(16, bodyHeight - 10));
            return new Rectangle(
                card.X + (card.Width - size) / 2,
                bodyTop + (bodyHeight - size) / 2 - 2,
                size,
                size);
        }

        /// <summary>
        /// 카드 내부에서 웨이퍼 스테이지 중심 (TM 블레이드 목표점).
        /// </summary>
        public static PointF GetWellCenter(Control chamber)
        {
            var well = GetWellBounds(new Rectangle(Point.Empty, chamber.Size));
            return new PointF(chamber.Left + well.X + well.Width / 2f, chamber.Top + well.Y + well.Height / 2f);
        }

        /// <summary>
        /// FOUP 패널(테두리 포함 컨테이너)을 캔버스와 어울리게 정리한다.
        /// </summary>
        public static void ApplyFoupHost(Panel foupPanel)
        {
            if (foupPanel == null)
            {
                return;
            }

            foupPanel.BorderStyle = BorderStyle.None;
            foupPanel.BackColor = CanvasBack;
            foupPanel.Padding = new Padding(0);
            foupPanel.Size = new Size(FoupWidth, FoupHeight);
            foupPanel.Cursor = Cursors.Hand;
            foreach (Control child in foupPanel.Controls)
            {
                child.Cursor = Cursors.Hand;
            }
        }

        /// <summary>
        /// 장비 상태 램프 범례를 캔버스 모서리용 가로형 칩으로 바꾼다.
        /// </summary>
        public static void ApplyMainLampStyle(Panel host, Label title, Panel red, Panel yellow, Panel green, Label redLabel, Label yellowLabel, Label greenLabel)
        {
            if (host == null)
            {
                return;
            }

            host.SuspendLayout();
            host.BorderStyle = BorderStyle.None;
            host.BackColor = CanvasBack;
            host.MinimumSize = Size.Empty;
            host.Size = new Size(262, 34);
            host.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            EnableDoubleBuffer(host);

            foreach (Control c in new Control[] { title, red, yellow, green, redLabel, yellowLabel, greenLabel })
            {
                if (c != null)
                {
                    c.Visible = false;
                }
            }

            foreach (var lamp in new[] { red, yellow, green })
            {
                if (lamp != null)
                {
                    lamp.BackColorChanged += (s, e) => host.Invalidate();
                }
            }

            host.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, host.Width - 1, host.Height - 1);
                using (var path = CreateRoundedRectangle(rect, rect.Height / 2f))
                using (var fill = new SolidBrush(CardFill))
                using (var pen = new Pen(CardBorder))
                {
                    g.FillPath(fill, path);
                    g.DrawPath(pen, path);
                }

                using (var titleFont = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                using (var itemFont = new Font("Segoe UI", 8.5F))
                {
                    TextRenderer.DrawText(g, "상태 램프", titleFont, new Rectangle(12, 0, 64, host.Height), CardSubText,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

                    int x = 78;
                    x = DrawLampItem(g, itemFont, x, host.Height, red?.BackColor ?? LampOff, "오류");
                    x = DrawLampItem(g, itemFont, x, host.Height, yellow?.BackColor ?? LampOff, "대기");
                    DrawLampItem(g, itemFont, x, host.Height, green?.BackColor ?? LampOff, "진행");
                }
            };
            host.ResumeLayout(false);
            host.Invalidate();
        }

        private static int DrawLampItem(Graphics g, Font font, int x, int height, Color color, string text)
        {
            const int dot = 12;
            var dotRect = new Rectangle(x, (height - dot) / 2, dot, dot);
            DrawLampDot(g, dotRect, color);
            var textSize = TextRenderer.MeasureText(g, text, font, Size.Empty, TextFormatFlags.NoPadding);
            TextRenderer.DrawText(g, text, font, new Rectangle(x + dot + 6, 0, textSize.Width + 4, height), CardTitle,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            return x + dot + 6 + textSize.Width + 14;
        }

        public static void DrawLampDot(Graphics g, Rectangle rect, Color color)
        {
            bool off = IsLampOff(color);
            var fill = off ? Color.FromArgb(214, 219, 227) : color;
            using (var brush = new SolidBrush(fill))
            using (var pen = new Pen(off ? Color.FromArgb(176, 184, 197) : ControlPaint.Dark(color, 0.1f), 1f))
            {
                g.FillEllipse(brush, rect);
                g.DrawEllipse(pen, rect);
            }

            if (!off)
            {
                using (var glow = new Pen(Color.FromArgb(70, color), 3f))
                {
                    var halo = Rectangle.Inflate(rect, 2, 2);
                    g.DrawEllipse(glow, halo);
                }
            }
        }

        private static bool IsLampOff(Color color)
        {
            // SetLamp의 꺼짐 색상(60,60,60)과 도어 램프의 밝은 회색(230,230,235)을 꺼짐으로 본다.
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));
            return max - min < 12;
        }

        private static float GetValveOpenRatio(Panel door, int closedHeight)
        {
            if (door == null || closedHeight <= 0)
            {
                return 0f;
            }

            // DoorAnimationHelper는 열릴 때 높이를 ClosedHeight/4 (최소 6)까지 줄인다.
            float openHeight = Math.Max(6, closedHeight / 4);
            if (closedHeight <= openHeight)
            {
                return 0f;
            }

            float ratio = (closedHeight - door.Height) / (closedHeight - openHeight);
            return Math.Max(0f, Math.Min(1f, ratio));
        }

        private static void PaintChamberCard(Graphics g, Rectangle client, string title, Color lampColor, float valveOpen, AnchorStyles valveSide)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var card = new Rectangle(client.X + 1, client.Y + 1, client.Width - 3, client.Height - 3);

            using (var path = CreateRoundedRectangle(card, CardRadius))
            using (var fill = new SolidBrush(CardFill))
            using (var pen = new Pen(CardBorder, 1f))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }

            using (var divider = new Pen(Color.FromArgb(232, 236, 242)))
            {
                g.DrawLine(divider, card.Left + 10, card.Top + HeaderHeight, card.Right - 10, card.Top + HeaderHeight);
            }

            using (var titleFont = new Font("Segoe UI", 10F, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, title, titleFont, new Rectangle(card.Left + 12, card.Top, card.Width - 44, HeaderHeight),
                    CardTitle, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }

            DrawLampDot(g, new Rectangle(card.Right - 24, card.Top + (HeaderHeight - 12) / 2, 12, 12), lampColor);

            var well = GetWellBounds(client);
            using (var wellFill = new SolidBrush(WellFill))
            using (var wellPen = new Pen(WellEdge, 1.2f))
            using (var ringPen = new Pen(Color.FromArgb(205, 212, 223), 1f) { DashStyle = DashStyle.Dot })
            {
                g.FillEllipse(wellFill, well);
                g.DrawEllipse(wellPen, well);
                var inner = Rectangle.Inflate(well, -6, -6);
                g.DrawEllipse(ringPen, inner);
            }

            DrawSlitValve(g, card, valveOpen, valveSide);
        }

        private static void DrawSlitValve(Graphics g, Rectangle card, float openRatio, AnchorStyles side)
        {
            const int thickness = 6;
            const int length = 50;
            bool horizontal = side == AnchorStyles.Top || side == AnchorStyles.Bottom;

            RectangleF slot;
            switch (side)
            {
                case AnchorStyles.Bottom:
                    slot = new RectangleF(card.Left + (card.Width - length) / 2f, card.Bottom - thickness - 3, length, thickness);
                    break;
                case AnchorStyles.Top:
                    slot = new RectangleF(card.Left + (card.Width - length) / 2f, card.Top + 3, length, thickness);
                    break;
                case AnchorStyles.Left:
                    slot = new RectangleF(card.Left + 3, card.Top + HeaderHeight + (card.Height - HeaderHeight - length) / 2f, thickness, length);
                    break;
                default:
                    slot = new RectangleF(card.Right - thickness - 3, card.Top + HeaderHeight + (card.Height - HeaderHeight - length) / 2f, thickness, length);
                    break;
            }

            using (var track = new SolidBrush(Color.FromArgb(226, 231, 238)))
            {
                g.FillRectangle(track, slot);
            }

            // 열림 비율만큼 두 짝이 양쪽으로 벌어진다.
            float gap = (horizontal ? slot.Width : slot.Height) * openRatio;
            using (var brush = new SolidBrush(ValveClosed))
            {
                if (horizontal)
                {
                    float half = (slot.Width - gap) / 2f;
                    if (half > 0.5f)
                    {
                        g.FillRectangle(brush, slot.X, slot.Y, half, slot.Height);
                        g.FillRectangle(brush, slot.Right - half, slot.Y, half, slot.Height);
                    }
                }
                else
                {
                    float half = (slot.Height - gap) / 2f;
                    if (half > 0.5f)
                    {
                        g.FillRectangle(brush, slot.X, slot.Y, slot.Width, half);
                        g.FillRectangle(brush, slot.X, slot.Bottom - half, slot.Width, half);
                    }
                }
            }
        }

        public static GraphicsPath CreateRoundedRectangle(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            float diameter = Math.Max(1f, Math.Min(radius * 2f, Math.Min(rect.Width, rect.Height)));
            var arc = new RectangleF(rect.Location, new SizeF(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void EnableDoubleBuffer(Control control)
        {
            var property = typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            property?.SetValue(control, true, null);
        }
    }
}
