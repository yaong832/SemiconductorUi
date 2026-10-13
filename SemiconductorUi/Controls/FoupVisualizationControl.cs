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

            var bounds = new Rectangle(1, 1, ClientSize.Width - 3, ClientSize.Height - 3);
            if (bounds.Width <= 20 || bounds.Height <= 20)
            {
                return;
            }

            using (var bodyPath = CreateRoundedRectangle(bounds, EquipmentCanvasStyler.CardRadius))
            using (var bodyBrush = new SolidBrush(EquipmentCanvasStyler.CardFill))
            using (var borderPen = new Pen(EquipmentCanvasStyler.CardBorder, 1f))
            {
                g.FillPath(bodyBrush, bodyPath);
                g.DrawPath(borderPen, bodyPath);
            }

            DrawTitle(g, bounds);
            DrawLamp(g, bounds);
            DrawSlots(g, bounds);
            DrawStatus(g, bounds);
        }

        private void DrawTitle(Graphics g, Rectangle bounds)
        {
            var titleRect = new Rectangle(bounds.X + 12, bounds.Y, bounds.Width - 44, EquipmentCanvasStyler.HeaderHeight);
            using (var titleFont = new Font(Font.FontFamily, 10F, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, title, titleFont, titleRect, EquipmentCanvasStyler.CardTitle,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }

            using (var divider = new Pen(Color.FromArgb(232, 236, 242)))
            {
                int y = bounds.Y + EquipmentCanvasStyler.HeaderHeight;
                g.DrawLine(divider, bounds.Left + 10, y, bounds.Right - 10, y);
            }
        }

        private void DrawLamp(Graphics g, Rectangle bounds)
        {
            // 도어 닫힘(장착/잠금) = 녹색, 열림 = 꺼짐
            var lampRect = new Rectangle(bounds.Right - 24, bounds.Y + (EquipmentCanvasStyler.HeaderHeight - 12) / 2, 12, 12);
            EquipmentCanvasStyler.DrawLampDot(g, lampRect, doorClosed ? Color.FromArgb(64, 170, 100) : EquipmentCanvasStyler.LampOff);
        }

        /// <summary>
        /// 카세트 슬롯. 아래가 1번 슬롯이며, 웨이퍼는 1번부터 채워진다.
        /// </summary>
        private void DrawSlots(Graphics g, Rectangle bounds)
        {
            int slotCount = Math.Max(1, capacity);
            var rack = new RectangleF(bounds.X + 18, bounds.Y + EquipmentCanvasStyler.HeaderHeight + 7, bounds.Width - 36, bounds.Height - EquipmentCanvasStyler.HeaderHeight - 32);
            if (rack.Height < 10)
            {
                return;
            }

            float pitch = rack.Height / slotCount;
            float barHeight = Math.Max(2f, Math.Min(6f, pitch * 0.55f));
            int filled = Math.Max(0, Math.Min(waferCount, slotCount));

            using (var emptyBrush = new SolidBrush(Color.FromArgb(233, 237, 243)))
            using (var waferBrush = new SolidBrush(waferColor))
            using (var waferPen = new Pen(ControlPaint.Dark(waferColor, 0.15f), 1f))
            {
                for (int slot = 1; slot <= slotCount; slot++)
                {
                    float y = rack.Bottom - slot * pitch + (pitch - barHeight) / 2f;
                    var bar = new RectangleF(rack.X, y, rack.Width, barHeight);
                    if (slot <= filled)
                    {
                        g.FillRectangle(waferBrush, bar);
                        g.DrawRectangle(waferPen, bar.X, bar.Y, bar.Width, bar.Height);
                    }
                    else
                    {
                        g.FillRectangle(emptyBrush, bar);
                    }
                }
            }

            using (var railPen = new Pen(Color.FromArgb(196, 204, 216), 2f))
            {
                g.DrawLine(railPen, rack.X - 5, rack.Y, rack.X - 5, rack.Bottom);
                g.DrawLine(railPen, rack.Right + 5, rack.Y, rack.Right + 5, rack.Bottom);
            }
        }

        private void DrawStatus(Graphics g, Rectangle bounds)
        {
            var statusRect = new Rectangle(bounds.X + 10, bounds.Bottom - 24, bounds.Width - 20, 20);
            var display = string.IsNullOrWhiteSpace(statusText) ? "-" : statusText;
            // 상태 문구에 이미 매수가 들어 있으면 중복해서 붙이지 않는다.
            var combined = display.Contains("장") ? display : $"{display} · {Math.Max(0, waferCount)}장";

            TextRenderer.DrawText(g, combined, Font, statusRect, EquipmentCanvasStyler.CardSubText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
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

