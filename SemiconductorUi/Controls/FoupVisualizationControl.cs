using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Controls
{
    public class FoupVisualizationControl : Control
    {
        private string title = "FOUP";
        private string statusText = "대기";
        private int waferCount;
        private int capacity = 25;
        private bool doorClosed = true;
        private Color bodyStart = Color.FromArgb(92, 104, 148);
        private Color bodyEnd = Color.FromArgb(54, 61, 96);
        private Color waferColor = Color.FromArgb(120, 170, 235);
        private float facingDegrees = -45f;
        private bool mounted = true;

        // 위에서 본 FOUP: 로드포트 판 위의 반투명 통. 문(앞판)은 TM을 향한다.
        private const float PortHalf = 32f;
        private static readonly Color PortLight = Color.FromArgb(160, 170, 184);
        private static readonly Color PortDark = Color.FromArgb(124, 136, 152);
        private static readonly Color PortEdge = Color.FromArgb(104, 115, 130);
        private static readonly Color PinFill = Color.FromArgb(211, 217, 225);
        private static readonly Color ShellFill = Color.FromArgb(108, 78, 88, 104);
        private static readonly Color ShellEdge = Color.FromArgb(60, 69, 84);
        private static readonly Color ShellDark = Color.FromArgb(60, 69, 84);
        private static readonly Color FlangeFill = Color.FromArgb(90, 100, 116);
        private static readonly Color FlangeTop = Color.FromArgb(124, 135, 152);
        private static readonly Color DoorClosedFill = Color.FromArgb(89, 99, 111);
        private static readonly Color DoorOpenFill = Color.FromArgb(59, 209, 126);
        private static readonly Color SiliconFill = Color.FromArgb(206, 213, 225);
        private static readonly Color SiliconRim = Color.FromArgb(226, 181, 60);
        private static readonly Color PlateFill = Color.FromArgb(42, 50, 64);
        private static readonly Color PlateText = Color.FromArgb(233, 237, 243);
        private static readonly Color AlertText = Color.FromArgb(196, 72, 60);

        public FoupVisualizationControl()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            ForeColor = EquipmentCanvasStyler.CardTitle;
            BackColor = EquipmentCanvasStyler.CanvasBack;
        }

        [Category("FOUP"), DefaultValue("FOUP")]
        public string Title
        {
            get => title;
            set
            {
                if (title == value)
                {
                    return;
                }
                title = value;
                Invalidate();
            }
        }

        [Category("FOUP"), DefaultValue("대기")]
        public string StatusText
        {
            get => statusText;
            set
            {
                if (statusText == value)
                {
                    return;
                }
                statusText = value;
                Invalidate();
            }
        }

        [Category("FOUP"), DefaultValue(0)]
        public int WaferCount
        {
            get => waferCount;
            set
            {
                value = Math.Max(0, value);
                if (waferCount == value)
                {
                    return;
                }
                waferCount = value;
                Invalidate();
            }
        }

        [Category("FOUP"), DefaultValue(25)]
        public int Capacity
        {
            get => capacity;
            set
            {
                value = Math.Max(1, value);
                if (capacity == value)
                {
                    return;
                }
                capacity = value;
                Invalidate();
            }
        }

        [Category("FOUP"), DefaultValue(true)]
        public bool DoorClosed
        {
            get => doorClosed;
            set
            {
                if (doorClosed == value)
                {
                    return;
                }
                doorClosed = value;
                Invalidate();
            }
        }

        [Category("FOUP")]
        public Color BodyColorStart
        {
            get => bodyStart;
            set
            {
                if (bodyStart == value)
                {
                    return;
                }
                bodyStart = value;
                Invalidate();
            }
        }

        [Category("FOUP")]
        public Color BodyColorEnd
        {
            get => bodyEnd;
            set
            {
                if (bodyEnd == value)
                {
                    return;
                }
                bodyEnd = value;
                Invalidate();
            }
        }

        [Category("FOUP")]
        public Color WaferFillColor
        {
            get => waferColor;
            set
            {
                if (waferColor == value)
                {
                    return;
                }
                waferColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// FOUP 문이 향하는 방향(화면 좌표, 도). 배치할 때 TM 중심 방향으로 정한다.
        /// </summary>
        [Category("FOUP"), DefaultValue(-45f)]
        public float FacingDegrees
        {
            get => facingDegrees;
            set
            {
                if (Math.Abs(facingDegrees - value) < 0.01f)
                {
                    return;
                }
                facingDegrees = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 로드포트에 FOUP이 올라가 있는지. false면 빈 로드포트만 그린다.
        /// </summary>
        [Category("FOUP"), DefaultValue(true)]
        public bool Mounted
        {
            get => mounted;
            set
            {
                if (mounted == value)
                {
                    return;
                }
                mounted = value;
                Invalidate();
            }
        }

        public void UpdateState(string status, int wafers, bool doorClosed)
        {
            StatusText = status ?? statusText;
            WaferCount = wafers;
            DoorClosed = doorClosed;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? BackColor);

            if (ClientSize.Width <= 40 || ClientSize.Height <= 40)
            {
                return;
            }

            var center = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);
            var state = g.Save();
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(facingDegrees);
            DrawLoadPort(g);
            if (mounted)
            {
                DrawPod(g);
            }
            else
            {
                DrawEmptyOutline(g);
            }
            g.Restore(state);

            DrawLabels(g);
        }

        /// <summary>
        /// 로드포트 판과 키네마틱 핀 3개. 이 아래의 좌표는 FOUP 중심 기준이고 +x가 TM 쪽(문)이다.
        /// </summary>
        private static void DrawLoadPort(Graphics g)
        {
            var plate = new RectangleF(-PortHalf, -PortHalf, PortHalf * 2f, PortHalf * 2f);
            using (var shadowPath = EquipmentCanvasStyler.CreateRoundedRectangle(new RectangleF(plate.X + 1.5f, plate.Y + 2.5f, plate.Width, plate.Height), 7f))
            using (var shadow = new SolidBrush(Color.FromArgb(40, 28, 36, 51)))
            {
                g.FillPath(shadow, shadowPath);
            }
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(plate, 7f))
            using (var fill = new LinearGradientBrush(plate, PortLight, PortDark, LinearGradientMode.ForwardDiagonal))
            using (var pen = new Pen(PortEdge, 1f))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }

            using (var pin = new SolidBrush(PinFill))
            using (var pinEdge = new Pen(Color.FromArgb(94, 106, 122), 0.8f))
            {
                foreach (var p in new[] { new PointF(18f, -19f), new PointF(18f, 19f), new PointF(-21f, 0f) })
                {
                    var r = new RectangleF(p.X - 2.4f, p.Y - 2.4f, 4.8f, 4.8f);
                    g.FillEllipse(pin, r);
                    g.DrawEllipse(pinEdge, r);
                }
            }
        }

        /// <summary>
        /// FOUP 통: 문 쪽은 평평하고 뒤쪽은 둥글다. 반투명이라 맨 위 웨이퍼가 비친다.
        /// </summary>
        private void DrawPod(Graphics g)
        {
            const float back = -26f, front = 23f, half = 26f, round = 14f;

            if (waferCount > 0)
            {
                var wafer = new RectangleF(-22f, -21f, 42f, 42f);
                using (var fill = new SolidBrush(SiliconFill))
                using (var rim = new Pen(SiliconRim, 1.6f))
                {
                    g.FillEllipse(fill, wafer);
                    g.DrawEllipse(rim, wafer);
                }
            }

            using (var shell = new GraphicsPath())
            {
                shell.AddLine(front, -half, back + round, -half);
                shell.AddArc(back, -half, round * 2f, round * 2f, 270f, -90f);
                shell.AddLine(back, -half + round, back, half - round);
                shell.AddArc(back, half - round * 2f, round * 2f, round * 2f, 180f, -90f);
                shell.AddLine(back + round, half, front, half);
                shell.CloseFigure();
                using (var fill = new SolidBrush(ShellFill))
                using (var pen = new Pen(ShellEdge, 1.2f))
                {
                    g.FillPath(fill, shell);
                    g.DrawPath(pen, shell);
                }
            }

            // 문(앞판): 닫힘이면 어둡고, 열리면 초록
            var door = new RectangleF(front, -half, 5f, half * 2f);
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(door, 1.5f))
            using (var fill = new SolidBrush(doorClosed ? DoorClosedFill : DoorOpenFill))
            using (var pen = new Pen(ShellEdge, 0.8f))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }

            // 옆 손잡이와 위쪽 로봇 플랜지
            using (var dark = new SolidBrush(ShellDark))
            {
                g.FillRectangle(dark, -9f, -half - 4f, 20f, 4f);
                g.FillRectangle(dark, -9f, half, 20f, 4f);
            }
            var flange = new RectangleF(-9f, -9f, 18f, 18f);
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(flange, 4f))
            using (var fill = new SolidBrush(FlangeFill))
            using (var pen = new Pen(Color.FromArgb(46, 54, 68), 0.8f))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(RectangleF.Inflate(flange, -4f, -4f), 2f))
            using (var fill = new SolidBrush(FlangeTop))
            {
                g.FillPath(fill, path);
            }
        }

        private static void DrawEmptyOutline(Graphics g)
        {
            using (var pen = new Pen(Color.FromArgb(225, 230, 237), 1.2f) { DashStyle = DashStyle.Dash })
            {
                g.DrawRectangle(pen, -26f, -26f, 49f, 52f);
            }
        }

        /// <summary>
        /// 이름판과 매수. 통이 TM 쪽을 향하므로, 글자는 그 반대쪽(바깥쪽) 위 모서리에 모아 둔다.
        /// </summary>
        private void DrawLabels(Graphics g)
        {
            bool tmOnRight = Math.Cos(facingDegrees * Math.PI / 180.0) >= 0;
            const int margin = 3;

            using (var plateFont = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            using (var countFont = new Font("Segoe UI", 9F, FontStyle.Bold))
            {
                var flags = TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                var textSize = TextRenderer.MeasureText(g, title ?? string.Empty, plateFont, Size.Empty, TextFormatFlags.NoPadding);
                int plateWidth = textSize.Width + 14;
                const int plateHeight = 19;
                int x = tmOnRight ? margin : ClientSize.Width - margin - plateWidth;
                var plate = new Rectangle(x, margin, plateWidth, plateHeight);
                using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(plate, 4))
                using (var fill = new SolidBrush(PlateFill))
                {
                    g.FillPath(fill, path);
                }
                TextRenderer.DrawText(g, title ?? string.Empty, plateFont, plate, PlateText, flags);

                string count = mounted ? $"{Math.Max(0, waferCount)}/{capacity}" : "미장착";
                var countSize = TextRenderer.MeasureText(g, count, countFont, Size.Empty, TextFormatFlags.NoPadding);
                // 매수는 이름판 바로 아래에 둔다
                int countX = tmOnRight ? plate.Left + 2 : plate.Right - 2 - countSize.Width;
                var countRect = new Rectangle(countX, plate.Bottom + 4, countSize.Width + 2, countSize.Height + 2);
                TextRenderer.DrawText(g, count, countFont, countRect, mounted ? EquipmentCanvasStyler.CardTitle : AlertText,
                    TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle rect, float radius)
        {
            var path = new GraphicsPath();
            float diameter = radius * 2f;
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
    }
}

