using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// 메인 화면 장비 시뮬레이터(캔버스)의 공통 색상과 카드 스타일.
    /// 기존 패널(도어/램프/웨이퍼)은 상태 보관용으로 그대로 두고, 챔버 모양은 Paint에서 직접 그린다.
    /// </summary>
    public static class EquipmentCanvasStyler
    {
        public static readonly Color CanvasBack = Color.FromArgb(236, 239, 244);
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
        /// 장비 그림을 얼마나 키워 그릴지. 1이면 원래 크기입니다.
        /// 캔버스 배치(Form1Configurator.LayoutCentralEquipment)가 화면 크기에 맞춰 정하고,
        /// 챔버 카드와 램프 범례는 원래 크기 기준으로 그린 뒤 이 배율로 확대합니다.
        /// </summary>
        public static float DrawScale { get; set; } = 1f;

        /// <summary>확대 전 기준의 컨트롤 크기.</summary>
        public static Size LogicalSize(Size pixels)
        {
            float k = DrawScale;
            return k == 1f ? pixels : new Size((int)Math.Round(pixels.Width / k), (int)Math.Round(pixels.Height / k));
        }

        private static void BeginScaled(Graphics g)
        {
            float k = DrawScale;
            if (k != 1f)
            {
                g.ScaleTransform(k, k);
            }
        }

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
            chamber.Paint += (s, e) =>
            {
                BeginScaled(e.Graphics);
                PaintChamberCard(e.Graphics, new Rectangle(Point.Empty, LogicalSize(chamber.ClientSize)), title?.Text ?? chamber.Name,
                    lamp?.BackColor ?? LampOff, GetValveOpenRatio(door, closedHeight), valveSide);
            };
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
            float k = DrawScale;
            var well = GetWellBounds(new Rectangle(Point.Empty, LogicalSize(chamber.Size)));
            return new PointF(chamber.Left + (well.X + well.Width / 2f) * k, chamber.Top + (well.Y + well.Height / 2f) * k);
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
                BeginScaled(g);
                var size = LogicalSize(host.Size);
                var rect = new Rectangle(0, 0, size.Width - 1, size.Height - 1);
                using (var path = CreateRoundedRectangle(rect, rect.Height / 2f))
                using (var fill = new SolidBrush(CardFill))
                using (var pen = new Pen(CardBorder))
                {
                    g.FillPath(fill, path);
                    g.DrawPath(pen, path);
                }

                var redColor = red?.BackColor ?? LampOff;
                var yellowColor = yellow?.BackColor ?? LampOff;
                var greenColor = green?.BackColor ?? LampOff;
                DrawSignalTower(g, new Rectangle(16, 3, 12, size.Height - 6), redColor, yellowColor, greenColor);

                using (var titleFont = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                using (var itemFont = new Font("Segoe UI", 8.5F))
                {
                    CanvasText.Draw(g, "상태 램프", titleFont, new Rectangle(36, 0, 60, size.Height), CardSubText,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

                    int x = 96;
                    x = DrawLampItem(g, itemFont, x, size.Height, redColor, "오류");
                    x = DrawLampItem(g, itemFont, x, size.Height, yellowColor, "대기");
                    DrawLampItem(g, itemFont, x, size.Height, greenColor, "진행");
                }
            };
            host.ResumeLayout(false);
            host.Invalidate();
        }

        /// <summary>
        /// 장비 위 시그널 타워(빨강·노랑·초록 3단)를 작게 그린다. 켜진 단만 밝다.
        /// </summary>
        private static void DrawSignalTower(Graphics g, Rectangle area, Color red, Color yellow, Color green)
        {
            const int poleHeight = 4;
            float segment = (area.Height - poleHeight - 2) / 3f;
            var colors = new[]
            {
                new { Lit = red, Off = Color.FromArgb(232, 196, 196) },
                new { Lit = yellow, Off = Color.FromArgb(236, 222, 190) },
                new { Lit = green, Off = Color.FromArgb(196, 226, 206) }
            };
            for (int i = 0; i < colors.Length; i++)
            {
                bool on = !IsLampOff(colors[i].Lit);
                var rect = new RectangleF(area.X, area.Y + i * segment, area.Width, segment - 1f);
                using (var path = CreateRoundedRectangle(rect, 2f))
                using (var fill = new SolidBrush(on ? colors[i].Lit : colors[i].Off))
                using (var pen = new Pen(Color.FromArgb(140, 151, 166), 0.8f))
                {
                    g.FillPath(fill, path);
                    g.DrawPath(pen, path);
                }
            }
            using (var pole = new SolidBrush(Color.FromArgb(94, 106, 122)))
            {
                g.FillRectangle(pole, area.X + area.Width / 2f - 1.5f, area.Bottom - poleHeight - 2, 3f, poleHeight);
                g.FillRectangle(pole, area.X - 1, area.Bottom - 2, area.Width + 2, 2);
            }
        }

        private static int DrawLampItem(Graphics g, Font font, int x, int height, Color color, string text)
        {
            const int dot = 12;
            var dotRect = new Rectangle(x, (height - dot) / 2, dot, dot);
            DrawLampDot(g, dotRect, color);
            var textSize = CanvasText.Measure(g, text, font, Size.Empty, TextFormatFlags.NoPadding);
            CanvasText.Draw(g, text, font, new Rectangle(x + dot + 6, 0, textSize.Width + 4, height), CardTitle,
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

        // 챔버를 실제 장비처럼: 금속 몸체, 볼트로 고정한 둥근 뚜껑, 가운데 뷰포트, TM 쪽 슬릿 밸브.
        private static readonly Color BodyLight = Color.FromArgb(246, 248, 250);
        private static readonly Color BodyDark = Color.FromArgb(205, 212, 221);
        private static readonly Color BodyEdge = Color.FromArgb(150, 160, 174);
        private static readonly Color LidCenter = Color.FromArgb(252, 253, 254);
        private static readonly Color LidEdge = Color.FromArgb(178, 187, 199);
        private static readonly Color LidRim = Color.FromArgb(142, 152, 167);
        private static readonly Color BoltFill = Color.FromArgb(232, 236, 241);
        private static readonly Color BoltEdge = Color.FromArgb(140, 151, 166);
        private static readonly Color GlassLight = Color.FromArgb(78, 94, 120);
        private static readonly Color GlassDark = Color.FromArgb(26, 35, 50);
        private static readonly Color PlateFill = Color.FromArgb(42, 50, 64);
        private static readonly Color PlateText = Color.FromArgb(233, 237, 243);
        private static readonly Color ValveBody = Color.FromArgb(140, 151, 166);
        private static readonly Color ValveOpenGlow = Color.FromArgb(59, 209, 126);

        private const int ChamberLidBolts = 12;

        private static void PaintChamberCard(Graphics g, Rectangle client, string title, Color lampColor, float valveOpen, AnchorStyles valveSide)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var card = new Rectangle(client.X + 1, client.Y + 1, client.Width - 4, client.Height - 4);

            // 바닥 그림자
            using (var shadowPath = CreateRoundedRectangle(new RectangleF(card.X + 1.5f, card.Y + 2.5f, card.Width, card.Height), CardRadius))
            using (var shadow = new SolidBrush(Color.FromArgb(38, 28, 36, 51)))
            {
                g.FillPath(shadow, shadowPath);
            }

            // 금속 몸체
            using (var path = CreateRoundedRectangle(card, CardRadius))
            using (var fill = new LinearGradientBrush(card, BodyLight, BodyDark, LinearGradientMode.ForwardDiagonal))
            using (var pen = new Pen(BodyEdge, 1f))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }

            // 모서리 고정 볼트
            const int boltInset = 8;
            foreach (var corner in new[]
            {
                new PointF(card.Right - boltInset, card.Top + HeaderHeight + 4),
                new PointF(card.Left + boltInset, card.Bottom - boltInset),
                new PointF(card.Right - boltInset, card.Bottom - boltInset),
                new PointF(card.Left + boltInset, card.Top + HeaderHeight + 4)
            })
            {
                DrawBolt(g, corner, 2.6f);
            }

            DrawNameplate(g, card, title);
            DrawLampDot(g, new Rectangle(card.Right - 24, card.Top + (HeaderHeight - 12) / 2, 12, 12), lampColor);

            DrawChamberLid(g, GetWellBounds(client));
            DrawSlitValve(g, card, valveOpen, valveSide);
        }

        /// <summary>
        /// 장비 이름판: 어두운 판에 흰 글자.
        /// </summary>
        private static void DrawNameplate(Graphics g, Rectangle card, string title)
        {
            using (var font = new Font("Segoe UI", 9F, FontStyle.Bold))
            {
                var flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;
                int maxWidth = card.Width - 52;
                var textSize = CanvasText.Measure(g, title ?? string.Empty, font, new Size(maxWidth, HeaderHeight), flags);
                var plate = new Rectangle(card.Left + 8, card.Top + 5, Math.Min(maxWidth, textSize.Width) + 16, HeaderHeight - 9);
                using (var path = CreateRoundedRectangle(plate, 4))
                using (var fill = new SolidBrush(PlateFill))
                {
                    g.FillPath(fill, path);
                }
                CanvasText.Draw(g, title ?? string.Empty, font, Rectangle.Inflate(plate, -8, 0), PlateText, flags);
            }
        }

        /// <summary>
        /// 둥근 뚜껑(볼트 링)과 가운데 뷰포트. 웨이퍼 패널은 뷰포트 위에 놓여 안의 웨이퍼처럼 보인다.
        /// </summary>
        private static void DrawChamberLid(Graphics g, Rectangle well)
        {
            var lid = Rectangle.Inflate(well, 4, 4);
            using (var lidPath = new GraphicsPath())
            {
                lidPath.AddEllipse(lid);
                using (var brush = new PathGradientBrush(lidPath)
                {
                    CenterColor = LidCenter,
                    SurroundColors = new[] { LidEdge },
                    CenterPoint = new PointF(lid.Left + lid.Width * 0.36f, lid.Top + lid.Height * 0.32f)
                })
                {
                    g.FillPath(brush, lidPath);
                }
            }
            using (var rim = new Pen(LidRim, 1.2f))
            {
                g.DrawEllipse(rim, lid);
            }

            // 볼트 링
            float cx = lid.Left + lid.Width / 2f, cy = lid.Top + lid.Height / 2f;
            float boltRadius = lid.Width / 2f - 4f;
            for (int i = 0; i < ChamberLidBolts; i++)
            {
                double angle = (i + 0.5) * 2 * Math.PI / ChamberLidBolts;
                DrawBolt(g, new PointF(cx + boltRadius * (float)Math.Cos(angle), cy + boltRadius * (float)Math.Sin(angle)), 1.7f);
            }

            // 뷰포트: 밝은 테두리 + 어두운 유리 + 반사광
            var frame = Rectangle.Inflate(well, -3, -3);
            using (var frameFill = new SolidBrush(Color.FromArgb(211, 217, 225)))
            using (var framePen = new Pen(LidRim, 1f))
            {
                g.FillEllipse(frameFill, frame);
                g.DrawEllipse(framePen, frame);
            }

            var glass = Rectangle.Inflate(frame, -3, -3);
            using (var glassPath = new GraphicsPath())
            {
                glassPath.AddEllipse(glass);
                using (var brush = new PathGradientBrush(glassPath)
                {
                    CenterColor = GlassLight,
                    SurroundColors = new[] { GlassDark },
                    CenterPoint = new PointF(glass.Left + glass.Width * 0.35f, glass.Top + glass.Height * 0.3f)
                })
                {
                    g.FillPath(brush, glassPath);
                }
            }
            using (var shine = new Pen(Color.FromArgb(90, 255, 255, 255), 1.6f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                var arc = Rectangle.Inflate(glass, -5, -5);
                if (arc.Width > 4 && arc.Height > 4)
                {
                    g.DrawArc(shine, arc, 200, 60);
                }
            }
        }

        private static void DrawBolt(Graphics g, PointF center, float radius)
        {
            var rect = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            using (var fill = new SolidBrush(BoltFill))
            using (var pen = new Pen(BoltEdge, 0.8f))
            {
                g.FillEllipse(fill, rect);
                g.DrawEllipse(pen, rect);
            }
        }

        /// <summary>
        /// TM 쪽 변의 슬릿 밸브: 밸브 몸체 안의 틈이 닫히면 어둡고, 열린 만큼 가운데부터 초록으로 벌어진다.
        /// </summary>
        private static void DrawSlitValve(Graphics g, Rectangle card, float openRatio, AnchorStyles side)
        {
            const int bodyThickness = 11;
            const int bodyLength = 58;
            const int slotThickness = 4;
            const int slotLength = 46;
            bool horizontal = side == AnchorStyles.Top || side == AnchorStyles.Bottom;
            float bodyTop = card.Top + HeaderHeight + (card.Height - HeaderHeight - bodyLength) / 2f;

            RectangleF body;
            switch (side)
            {
                case AnchorStyles.Bottom:
                    body = new RectangleF(card.Left + (card.Width - bodyLength) / 2f, card.Bottom - bodyThickness, bodyLength, bodyThickness);
                    break;
                case AnchorStyles.Top:
                    body = new RectangleF(card.Left + (card.Width - bodyLength) / 2f, card.Top, bodyLength, bodyThickness);
                    break;
                case AnchorStyles.Left:
                    body = new RectangleF(card.Left, bodyTop, bodyThickness, bodyLength);
                    break;
                default:
                    body = new RectangleF(card.Right - bodyThickness, bodyTop, bodyThickness, bodyLength);
                    break;
            }

            using (var path = CreateRoundedRectangle(body, 3))
            using (var fill = new SolidBrush(ValveBody))
            using (var pen = new Pen(Color.FromArgb(110, 121, 135), 0.8f))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }

            var slot = horizontal
                ? new RectangleF(body.Left + (bodyLength - slotLength) / 2f, body.Top + (bodyThickness - slotThickness) / 2f, slotLength, slotThickness)
                : new RectangleF(body.Left + (bodyThickness - slotThickness) / 2f, body.Top + (bodyLength - slotLength) / 2f, slotThickness, slotLength);

            // 열린 만큼 초록 틈, 나머지는 닫힌 게이트
            using (var gate = new SolidBrush(ValveClosed))
            {
                g.FillRectangle(gate, slot);
            }

            float gap = (horizontal ? slot.Width : slot.Height) * Math.Max(0f, Math.Min(1f, openRatio));
            if (gap > 0.5f)
            {
                var open = horizontal
                    ? new RectangleF(slot.X + (slot.Width - gap) / 2f, slot.Y, gap, slot.Height)
                    : new RectangleF(slot.X, slot.Y + (slot.Height - gap) / 2f, slot.Width, gap);
                using (var glow = new SolidBrush(Color.FromArgb(80, ValveOpenGlow)))
                using (var lit = new SolidBrush(ValveOpenGlow))
                {
                    g.FillRectangle(glow, RectangleF.Inflate(open, 2f, 2f));
                    g.FillRectangle(lit, open);
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
