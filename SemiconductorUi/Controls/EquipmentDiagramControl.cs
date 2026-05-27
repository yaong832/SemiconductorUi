using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemiconductorUi.Controls
{
    public class EquipmentDiagramControl : Control
    {
        private readonly Dictionary<EquipmentRegion, GraphicsPath> hitRegions = new Dictionary<EquipmentRegion, GraphicsPath>();

        private string tmStatus = "Idle";
        private EquipmentRegion tmArmTarget = EquipmentRegion.FoupA;
        private bool tmCarryingWafer;
        private string processStatus = "-";
        private string pressureStatus = "-";
        private string temperatureStatus = "-";
        private string doorStatus = "-";
        private string foupAStatus = "-";
        private string foupBStatus = "-";
        
        // 램프 상태 (상단=적색, 중단=황색, 하단=녹색)
        private bool lampRedActive = false;
        private bool lampYellowActive = false;
        private bool lampGreenActive = false;

        public EquipmentDiagramControl()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.FromArgb(38, 41, 48);
        }
        
        /// <summary>
        /// 메인 램프 상태 업데이트 (상단=적색, 중단=황색, 하단=녹색)
        /// </summary>
        public void UpdateMainLampState(bool redActive, bool yellowActive, bool greenActive)
        {
            lampRedActive = redActive;
            lampYellowActive = yellowActive;
            lampGreenActive = greenActive;
            Invalidate();
        }

        public void UpdateSnapshot(string tm, string process, string pressure, string temperature, string door, string foupA, string foupB)
        {
            tmStatus = tm ?? tmStatus;
            processStatus = process ?? processStatus;
            pressureStatus = pressure ?? pressureStatus;
            temperatureStatus = temperature ?? temperatureStatus;
            doorStatus = door ?? doorStatus;
            foupAStatus = foupA ?? foupAStatus;
            foupBStatus = foupB ?? foupBStatus;
            Invalidate();
        }

        public void UpdateTmRoute(EquipmentRegion target, bool carryingWafer)
        {
            tmArmTarget = target;
            tmCarryingWafer = carryingWafer;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearHitRegions();
            }
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);

            ClearHitRegions();

            var bounds = ClientRectangle;
            bounds.Inflate(-25, -25);

            var boxWidth = Math.Min(bounds.Width, bounds.Height) / 3.2f;
            var boxHeight = boxWidth * 0.75f;
            var centerX = bounds.Left + bounds.Width / 2f;

            var chamberBRect = new RectangleF(centerX - boxWidth / 2f, bounds.Top + 20, boxWidth, boxHeight);
            var chamberARect = new RectangleF(bounds.Left + 40, bounds.Top + bounds.Height * 0.48f, boxWidth, boxHeight);
            var chamberCRect = new RectangleF(bounds.Right - boxWidth - 40, bounds.Top + bounds.Height * 0.48f, boxWidth, boxHeight);

            var foupWidth = boxWidth * 0.9f;
            var foupHeight = boxHeight * 0.55f;
            var foupY = bounds.Bottom - foupHeight - 15;
            var foupAX = centerX - foupWidth - 70;
            var foupBX = centerX + 70 - foupWidth / 10f;

            DrawChamberBlock(g, chamberARect, "CHAMBER A", processStatus, EquipmentRegion.ChamberA);
            DrawChamberBlock(g, chamberBRect, "CHAMBER B", pressureStatus, EquipmentRegion.ChamberB);
            DrawChamberBlock(g, chamberCRect, "CHAMBER C", temperatureStatus, EquipmentRegion.ChamberC);

            var foupARect = new RectangleF(foupAX, foupY, foupWidth, foupHeight);
            var foupBRect = new RectangleF(foupBX, foupY, foupWidth, foupHeight);
            DrawFoupBlock(g, foupARect, "FOUP A", foupAStatus, EquipmentRegion.FoupA);
            DrawFoupBlock(g, foupBRect, "FOUP B", foupBStatus, EquipmentRegion.FoupB);

            DrawTmModule(g, bounds, chamberARect, chamberBRect, chamberCRect, foupARect, foupBRect);
            DrawTowerLamp(g, new PointF(bounds.Right - 25, bounds.Top + 20));
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            foreach (var pair in hitRegions)
            {
                if (pair.Value.IsVisible(e.Location))
                {
                    OnRegionClicked(pair.Key);
                    return;
                }
            }
        }

        private void DrawChamberBlock(Graphics g, RectangleF rect, string title, string status, EquipmentRegion region)
        {
            using (var path = CreateRoundedRectangle(rect, 16f))
            using (var brush = new LinearGradientBrush(rect, Color.FromArgb(242, 242, 242), Color.FromArgb(205, 205, 205), LinearGradientMode.Vertical))
            using (var pen = new Pen(Color.FromArgb(90, Color.Black), 2))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
                AddHitRegion(region, path);
            }

            var titleRect = new RectangleF(rect.X, rect.Y + 6, rect.Width, 24);
            TextRenderer.DrawText(g, title, new Font(Font.FontFamily, 11f, FontStyle.Bold), Rectangle.Round(titleRect),
                Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            var statusRect = new RectangleF(rect.X + 10, rect.Bottom - 34, rect.Width - 20, 28);
            TextRenderer.DrawText(g, status, Font, Rectangle.Round(statusRect), Color.FromArgb(30, 30, 30),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
        }

        private void DrawFoupBlock(Graphics g, RectangleF rect, string title, string status, EquipmentRegion region)
        {
            using (var path = CreateRoundedRectangle(rect, 14f))
            using (var brush = new LinearGradientBrush(rect, Color.FromArgb(120, 120, 200), Color.FromArgb(70, 70, 130), LinearGradientMode.Vertical))
            using (var pen = new Pen(Color.FromArgb(130, Color.White), 2))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
                AddHitRegion(region, path);
            }

            var titleRect = new RectangleF(rect.X, rect.Y + 4, rect.Width, 20);
            TextRenderer.DrawText(g, title, new Font(Font.FontFamily, 10f, FontStyle.Bold), Rectangle.Round(titleRect),
                Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            var statusRect = new RectangleF(rect.X + 8, rect.Y + 28, rect.Width - 16, rect.Height - 34);
            TextRenderer.DrawText(g, status, Font, Rectangle.Round(statusRect), Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
        }

        private void DrawTmModule(Graphics g, Rectangle bounds, RectangleF chamberA, RectangleF chamberB, RectangleF chamberC, RectangleF foupA, RectangleF foupB)
        {
            var center = new PointF(bounds.Left + bounds.Width / 2f, bounds.Top + bounds.Height * 0.45f);
            float baseRadius = Math.Min(bounds.Width, bounds.Height) / 7f;
            var baseRect = new RectangleF(center.X - baseRadius / 2f, center.Y - baseRadius / 2f, baseRadius, baseRadius);

            using (var brush = new LinearGradientBrush(baseRect, Color.FromArgb(70, 110, 220), Color.FromArgb(35, 60, 140), LinearGradientMode.Vertical))
            using (var pen = new Pen(Color.FromArgb(230, Color.White), 3))
            {
                g.FillEllipse(brush, baseRect);
                g.DrawEllipse(pen, baseRect);
            }

            AddHitRegion(EquipmentRegion.TM, CreateEllipsePath(baseRect));

            using (var armPen = new Pen(tmCarryingWafer ? Color.Gold : Color.FromArgb(220, 220, 220), tmCarryingWafer ? 8 : 6)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            })
            {
                var target = GetTargetPoint(tmArmTarget, chamberA, chamberB, chamberC, foupA, foupB, center);
                g.DrawLine(armPen, center, target);

                if (tmCarryingWafer)
                {
                    var waferRect = new RectangleF(target.X - 7, target.Y - 7, 14, 14);
                    using (var waferBrush = new SolidBrush(Color.FromArgb(255, 240, 150)))
                    using (var waferPen = new Pen(Color.Goldenrod, 1.5f))
                    {
                        g.FillEllipse(waferBrush, waferRect);
                        g.DrawEllipse(waferPen, waferRect);
                    }
                }
            }

            TextRenderer.DrawText(g, "TM", new Font(Font.FontFamily, 11f, FontStyle.Bold),
                Rectangle.Round(baseRect), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            var statusRect = new RectangleF(baseRect.X, baseRect.Bottom + 6, baseRect.Width, 32);
            TextRenderer.DrawText(g, tmStatus, Font, Rectangle.Round(statusRect), Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
        }

        private PointF GetTargetPoint(EquipmentRegion target, RectangleF chamberA, RectangleF chamberB, RectangleF chamberC, RectangleF foupA, RectangleF foupB, PointF fallback)
        {
            switch (target)
            {
                case EquipmentRegion.ChamberA:
                    return new PointF(chamberA.Left + chamberA.Width * 0.15f, chamberA.Top + chamberA.Height / 2f);
                case EquipmentRegion.ChamberB:
                    return new PointF(chamberB.Left + chamberB.Width / 2f, chamberB.Bottom - chamberB.Height * 0.2f);
                case EquipmentRegion.ChamberC:
                    return new PointF(chamberC.Right - chamberC.Width * 0.15f, chamberC.Top + chamberC.Height / 2f);
                case EquipmentRegion.FoupA:
                    return new PointF(foupA.Left + foupA.Width / 2f, foupA.Top);
                case EquipmentRegion.FoupB:
                    return new PointF(foupB.Left + foupB.Width / 2f, foupB.Top);
                default:
                    return new PointF((foupA.X + foupB.Right) / 2f, Math.Min(foupA.Y, foupB.Y) + 5);
            }
        }

        private void DrawTowerLamp(Graphics g, PointF origin)
        {
            float width = 18f;
            float height = 100f;
            var rect = new RectangleF(origin.X, origin.Y, width, height);

            using (var bodyBrush = new SolidBrush(Color.FromArgb(60, 60, 70)))
            using (var bodyPen = new Pen(Color.FromArgb(120, Color.Black), 2))
            {
                g.FillRectangle(bodyBrush, rect);
                g.DrawRectangle(bodyPen, rect.X, rect.Y, rect.Width, rect.Height);
            }

            float segmentHeight = height / 3f;
            // 램프 색상 순서: 상단=적색, 중단=황색, 하단=녹색
            // 그리기 순서: i=0(상단) → 적색, i=1(중단) → 황색, i=2(하단) → 녹색
            // 밝은 색상 (켜져있을 때) - 배열 인덱스: [0]=적색, [1]=황색, [2]=녹색
            var brightColors = new[]
            {
                AppSettings.LampRedBright,     // red [0] (상단용)
                AppSettings.LampYellowBright,   // yellow [1] (중단용)
                AppSettings.LampGreenBright     // green [2] (하단용)
            };
            // 어두운 색상 (꺼져있을 때)
            var darkColors = new[]
            {
                AppSettings.LampRedDark,      // red (어두운)
                AppSettings.LampYellowDark,     // yellow (어두운)
                AppSettings.LampGreenDark      // green (어두운)
            };

            // 상단부터 하단 순서로 그리기
            for (int i = 0; i < 3; i++)
            {
                // i=0: 상단, i=1: 중단, i=2: 하단
                var segment = new RectangleF(rect.X + 2, rect.Y + 2 + i * segmentHeight, width - 4, segmentHeight - 4);
                // 색상 인덱스: i=0(상단) → 0(적색), i=1(중단) → 1(황색), i=2(하단) → 2(녹색)
                int colorIndex = i; // 0→0(적색), 1→1(황색), 2→2(녹색)
                // 램프 상태: i=0(상단) → lampRedActive, i=1(중단) → lampYellowActive, i=2(하단) → lampGreenActive
                bool isActive = i == 0 ? lampRedActive : (i == 1 ? lampYellowActive : lampGreenActive);
                // 램프가 켜져있으면 밝은 색상, 꺼져있으면 어두운 색상
                var color = isActive ? brightColors[colorIndex] : darkColors[colorIndex];
                using (var brush = new SolidBrush(color))
                using (var pen = new Pen(Color.FromArgb(180, Color.Black)))
                {
                    g.FillRectangle(brush, segment);
                    g.DrawRectangle(pen, segment.X, segment.Y, segment.Width, segment.Height);
                }
            }
        }

        private GraphicsPath CreateDiamond(RectangleF rect)
        {
            var path = new GraphicsPath();
            var centerX = rect.X + rect.Width / 2f;
            var centerY = rect.Y + rect.Height / 2f;
            path.AddPolygon(new[]
            {
                new PointF(centerX, rect.Y),
                new PointF(rect.Right, centerY),
                new PointF(centerX, rect.Bottom),
                new PointF(rect.X, centerY)
            });
            return path;
        }

        private GraphicsPath CreateRoundedRectangle(RectangleF rect, float radius)
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

        private GraphicsPath CreateEllipsePath(RectangleF rect)
        {
            var path = new GraphicsPath();
            path.AddEllipse(rect);
            return path;
        }

        private void AddHitRegion(EquipmentRegion region, GraphicsPath path)
        {
            if (region == EquipmentRegion.None)
            {
                path.Dispose();
                return;
            }

            var clone = (GraphicsPath)path.Clone();
            hitRegions[region] = clone;
        }

        private void ClearHitRegions()
        {
            foreach (var path in hitRegions.Values)
            {
                path.Dispose();
            }
            hitRegions.Clear();
        }

        protected virtual void OnRegionClicked(EquipmentRegion region)
        {
            if (region == EquipmentRegion.None)
            {
                return;
            }

            RegionClicked?.Invoke(this, new EquipmentRegionClickedEventArgs(region));
        }

        public event EventHandler<EquipmentRegionClickedEventArgs> RegionClicked;
    }

    public enum EquipmentRegion
    {
        None,
        TM,
        ChamberA,
        ChamberB,
        ChamberC,
        FoupA,
        FoupB
    }

    public sealed class EquipmentRegionClickedEventArgs : EventArgs
    {
        public EquipmentRegionClickedEventArgs(EquipmentRegion region)
        {
            Region = region;
        }

        public EquipmentRegion Region { get; }
    }
}

