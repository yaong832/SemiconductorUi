using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
        private Color waferColor = Color.FromArgb(108, 196, 255);

        public FoupVisualizationControl()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ForeColor = Color.White;
            BackColor = Color.FromArgb(40, 43, 52);
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
            g.Clear(BackColor);

            var bounds = ClientRectangle;
            bounds.Inflate(-2, -2);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            using (var bodyPath = CreateRoundedRectangle(bounds, 18f))
            using (var bodyBrush = new LinearGradientBrush(bounds, bodyStart, bodyEnd, LinearGradientMode.Vertical))
            using (var borderPen = new Pen(Color.FromArgb(150, Color.Black), 1.5f))
            {
                g.FillPath(bodyBrush, bodyPath);
                g.DrawPath(borderPen, bodyPath);
            }

            DrawTitle(g, bounds);
            DrawLamp(g, bounds);
            DrawDoor(g, bounds);
            DrawStatus(g, bounds);
        }

        private void DrawTitle(Graphics g, Rectangle bounds)
        {
            var titleRect = new Rectangle(bounds.X + 16, bounds.Y + 10, bounds.Width - 80, 24);
            using (var titleFont = new Font(Font.FontFamily, Font.Size + 2f, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, title, titleFont, Rectangle.Round(titleRect), Color.White,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        private void DrawLamp(Graphics g, Rectangle bounds)
        {
            var lampRect = new RectangleF(bounds.Right - 34, bounds.Y + 10, 18, 18);
            var lampColor = doorClosed ? Color.FromArgb(80, 210, 125) : Color.FromArgb(150, 70, 70);

            using (var lampBrush = new SolidBrush(lampColor))
            using (var lampPen = new Pen(Color.FromArgb(200, Color.Black), 1.2f))
            {
                g.FillEllipse(lampBrush, lampRect);
                g.DrawEllipse(lampPen, lampRect);
            }

            using (var glareBrush = new SolidBrush(Color.FromArgb(120, Color.White)))
            {
                var glareRect = new RectangleF(lampRect.X + 3, lampRect.Y + 3, lampRect.Width / 2.5f, lampRect.Height / 2.5f);
                g.FillEllipse(glareBrush, glareRect);
            }
        }

        private void DrawDoor(Graphics g, Rectangle bounds)
        {
            var doorRect = new RectangleF(bounds.X + 18, bounds.Y + 44, bounds.Width - 36, bounds.Height - 96);
            if (doorRect.Height < 40 || doorRect.Width < 40)
            {
                return;
            }

            using (var doorBrush = new LinearGradientBrush(doorRect, Color.FromArgb(54, 60, 84), Color.FromArgb(28, 32, 48), LinearGradientMode.Vertical))
            using (var doorPen = new Pen(Color.FromArgb(180, Color.Black), 1f))
            {
                g.FillRectangle(doorBrush, doorRect);
                g.DrawRectangle(doorPen, doorRect.X, doorRect.Y, doorRect.Width, doorRect.Height);
            }

            var innerRect = RectangleF.Inflate(doorRect, -8, -10);
            
            // 실제 웨이퍼 슬롯 개수 (capacity와 동일)
            int slotCount = capacity;
            float slotHeight = innerRect.Height / slotCount;
            
            // 슬롯 구분선 그리기
            using (var slotPen = new Pen(Color.FromArgb(70, Color.White), 1f))
            {
                for (int i = 1; i < slotCount; i++)
                {
                    float y = innerRect.Top + i * slotHeight;
                    g.DrawLine(slotPen, innerRect.Left, y, innerRect.Right, y);
                }
            }
            
            // 개별 웨이퍼 슬롯 표시 (1층부터 아래로, 1층이 맨 아래)
            // Queue에서 1층부터 빼가므로, waferCount가 줄어들면 1층부터 사라지도록 표시
            // 1층이 사라지면 2층이 아래로 내려오는 것처럼 보이도록 함
            using (var waferBrush = new SolidBrush(Color.FromArgb(180, waferColor)))
            using (var waferPen = new Pen(Color.FromArgb(150, waferColor), 1f))
            {
                int actualWaferCount = Math.Min(waferCount, slotCount);
                // 각 웨이퍼를 고정된 층 위치에 개별 칸으로 표시
                // 1층이 사라지면 나머지 웨이퍼는 각자의 층 위치를 유지 (아래로 내려오지 않음)
                // waferCount=5일 때: 1층(24), 2층(23), 3층(22), 4층(21), 5층(20)
                // waferCount=4일 때: 1층 사라짐, 2층(23, 그 자리 유지), 3층(22), 4층(21), 5층(20)
                // 즉, 각 웨이퍼는 항상 자신의 층 위치에 고정되어 있음
                // Queue에서 1층부터 빼가므로, waferCount가 줄어들면 아래쪽 층부터 사라짐
                // 하지만 나머지 웨이퍼는 각자의 층 위치를 유지
                // 따라서 각 층을 개별적으로 확인하여 웨이퍼가 있는지 표시
                for (int layer = 1; layer <= slotCount; layer++)
                {
                    // 각 층을 확인: 1층부터 위로 올라가며
                    // 해당 층에 웨이퍼가 있는지 확인 (waferCount와 비교)
                    // 예: waferCount=5일 때 1~5층에 웨이퍼 있음, waferCount=4일 때 2~5층에 웨이퍼 있음
                    // 즉, layer가 (slotCount - actualWaferCount + 1) 이상이면 웨이퍼가 있음
                    // 또는 더 간단하게: layer > (slotCount - actualWaferCount)이면 웨이퍼가 있음
                    int slotIndex = slotCount - layer; // 1층 = slotCount - 1, 2층 = slotCount - 2, ...
                    
                    // 해당 층에 웨이퍼가 있는지 확인
                    // Queue에서 1층부터 빼가므로, waferCount가 줄어들면 아래쪽 층부터 사라짐
                    // 예: 원래 5개(1,2,3,4,5층), 현재 4개 → 1층 사라짐, 2,3,4,5층은 그 자리 유지
                    // 즉, 아래쪽부터 actualWaferCount개만큼 웨이퍼가 있음
                    // slotCount=25, actualWaferCount=5일 때: 1~5층에 웨이퍼 있음
                    // slotCount=25, actualWaferCount=4일 때: 2~5층에 웨이퍼 있음 (1층 사라짐)
                    // slotCount=25, actualWaferCount=3일 때: 3~5층에 웨이퍼 있음 (1,2층 사라짐)
                    // 따라서 layer가 (slotCount - actualWaferCount + 1) 이상이면 웨이퍼가 있음
                    // 1층부터 actualWaferCount개만큼 웨이퍼가 있음
                    // slotCount=25, actualWaferCount=3일 때: 1~3층에 웨이퍼 있음
                    // slotCount=25, actualWaferCount=5일 때: 1~5층에 웨이퍼 있음
                    bool hasWafer = layer <= actualWaferCount;
                    
                    if (!hasWafer)
                    {
                        continue; // 해당 층에 웨이퍼가 없으면 표시하지 않음
                    }
                    
                    // 해당 층에 웨이퍼가 있으면 개별 칸으로 표시 (실제 웨이퍼처럼 좌우로 길고 상하로 얇게)
                    float slotY = innerRect.Top + slotIndex * slotHeight;
                    // 웨이퍼 높이를 줄이고 좌우 여백을 줄여서 실제 웨이퍼처럼 표시
                    float waferHeight = slotHeight * 0.3f; // 높이를 30%로 줄임
                    float waferY = slotY + (slotHeight - waferHeight) / 2f; // 수직 중앙 정렬
                    float horizontalMargin = innerRect.Width * 0.05f; // 좌우 여백을 5%로 줄임
                    var slotRect = new RectangleF(
                        innerRect.X + horizontalMargin,
                        waferY,
                        innerRect.Width - horizontalMargin * 2,
                        waferHeight
                    );
                    
                    // 웨이퍼 슬롯 채우기 (개별 칸)
                    g.FillRectangle(waferBrush, slotRect);
                    g.DrawRectangle(waferPen, slotRect.X, slotRect.Y, slotRect.Width, slotRect.Height);
                }
            }

            using (var framePen = new Pen(Color.FromArgb(120, Color.White), 1f))
            {
                g.DrawRectangle(framePen, innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);
            }
        }

        private void DrawStatus(Graphics g, Rectangle bounds)
        {
            var statusRect = new Rectangle(bounds.X + 14, bounds.Bottom - 38, bounds.Width - 28, 26);
            var display = string.IsNullOrWhiteSpace(statusText) ? "-" : statusText;
            var waferDisplay = $"{Math.Max(0, waferCount)}장";
            var combined = $"{display} · {waferDisplay}";

            TextRenderer.DrawText(g, combined, Font, Rectangle.Round(statusRect), ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
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

