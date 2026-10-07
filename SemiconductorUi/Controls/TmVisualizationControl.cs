using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Controls
{
    public class TmVisualizationControl : Control
    {
        private EquipmentRegion targetRegion = EquipmentRegion.TM;
        private bool carryingWafer;
        private Color waferColor = Color.FromArgb(250, 232, 168);
        private float currentExtensionFactor = 0.65f;
        private float targetExtensionFactor = 0.65f;
        private float currentAngleRad = (float)(Math.PI / 2);
        private float targetAngleRad = (float)(Math.PI / 2);
        private readonly Timer animationTimer;
        private bool isSimulationMode = true; // 기본값: 시뮬레이션 모드
        private readonly Dictionary<EquipmentRegion, PointF> anchors = new Dictionary<EquipmentRegion, PointF>();
        private PointF hubCenter;
        private bool hasLayout;

        private const float HubRadius = 26f;
        private const float TransferChamberRadius = 64f;
        private const float CorridorWidth = 46f;
        private const float RetractedLength = 42f;
        private const float RetractedFactor = 0.7f;
        private const float ExtendedFactor = 1.3f;

        public TmVisualizationControl()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            
            // 투명 배경 지원 활성화
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            
            BackColor = Color.Transparent; // 투명 배경
            ForeColor = Color.White;
            animationTimer = new Timer { Interval = 16 };
            animationTimer.Tick += AnimationTimer_Tick;
        }

        public void UpdateTmState(EquipmentRegion target, bool carrying, float extensionFactor, Color? waferDisplayColor = null, bool isSimulation = true)
        {
            targetRegion = target;
            carryingWafer = carrying;
            isSimulationMode = isSimulation;
            if (waferDisplayColor.HasValue)
            {
                waferColor = waferDisplayColor.Value;
            }
            targetExtensionFactor = Math.Max(0.4f, Math.Min(1.6f, extensionFactor));
            targetAngleRad = GetBladeAngle(targetRegion, isSimulation);
            if (!animationTimer.Enabled)
            {
                animationTimer.Start();
            }
            Invalidate();
        }

        /// <summary>
        /// 캔버스 배치 결과를 받아 TM 중심과 각 모듈의 블레이드 목표점을 갱신한다.
        /// 좌표는 이 컨트롤의 클라이언트 좌표 기준이다.
        /// </summary>
        public void SetLayout(PointF center, IDictionary<EquipmentRegion, PointF> moduleAnchors)
        {
            hubCenter = center;
            anchors.Clear();
            if (moduleAnchors != null)
            {
                foreach (var pair in moduleAnchors)
                {
                    anchors[pair.Key] = pair.Value;
                }
            }

            hasLayout = true;
            targetAngleRad = GetBladeAngle(targetRegion, isSimulationMode);
            currentAngleRad = targetAngleRad;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? EquipmentCanvasStyler.CanvasBack);

            var center = hasLayout ? hubCenter : new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);

            DrawCorridors(g, center);
            DrawTransferChamber(g, center);
            DrawBlade(g, center);
            if (carryingWafer)
            {
                DrawWaferOnBlade(g, center);
            }
            DrawHub(g, center);
        }

        private void DrawCorridors(Graphics g, PointF center)
        {
            if (anchors.Count == 0)
            {
                return;
            }

            using (var edgePen = new Pen(EquipmentCanvasStyler.CorridorEdge, CorridorWidth + 2f) { StartCap = LineCap.Flat, EndCap = LineCap.Flat })
            using (var fillPen = new Pen(EquipmentCanvasStyler.CorridorFill, CorridorWidth) { StartCap = LineCap.Flat, EndCap = LineCap.Flat })
            {
                foreach (var anchor in anchors.Values)
                {
                    g.DrawLine(edgePen, center, anchor);
                }
                foreach (var anchor in anchors.Values)
                {
                    g.DrawLine(fillPen, center, anchor);
                }
            }
        }

        private static void DrawTransferChamber(Graphics g, PointF center)
        {
            // 팔각형 이송 챔버
            var points = new PointF[8];
            for (int i = 0; i < 8; i++)
            {
                double angle = Math.PI / 8 + i * Math.PI / 4;
                points[i] = new PointF(
                    center.X + TransferChamberRadius * (float)Math.Cos(angle),
                    center.Y + TransferChamberRadius * (float)Math.Sin(angle));
            }

            using (var fill = new SolidBrush(EquipmentCanvasStyler.TransferChamberFill))
            using (var pen = new Pen(EquipmentCanvasStyler.TransferChamberEdge, 1.5f))
            {
                g.FillPolygon(fill, points);
                g.DrawPolygon(pen, points);
            }
        }

        private void DrawHub(Graphics g, PointF center)
        {
            var hubRect = new RectangleF(center.X - HubRadius, center.Y - HubRadius, HubRadius * 2f, HubRadius * 2f);
            using (var brush = new LinearGradientBrush(hubRect, Color.FromArgb(92, 108, 132), Color.FromArgb(52, 63, 80), LinearGradientMode.Vertical))
            using (var outline = new Pen(Color.FromArgb(235, 239, 245), 2f))
            {
                g.FillEllipse(brush, hubRect);
                g.DrawEllipse(outline, hubRect);
            }

            using (var font = new Font("Segoe UI", 9F, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, "TM", font, Rectangle.Round(hubRect), Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }

        /// <summary>
        /// extensionFactor(0.55 대기 ~ 1.3 최대 전진)를 블레이드 길이로 바꾼다.
        /// 최대 전진 시 블레이드 끝(웨이퍼 중심)이 목표 모듈의 스테이지 중심에 닿는다.
        /// </summary>
        private float GetBladeLength()
        {
            float reach = RetractedLength + 120f;
            if (anchors.TryGetValue(targetRegion, out var anchor))
            {
                var center = hubCenter;
                reach = (float)Math.Sqrt((anchor.X - center.X) * (anchor.X - center.X) + (anchor.Y - center.Y) * (anchor.Y - center.Y));
            }

            var norm = (currentExtensionFactor - RetractedFactor) / (ExtendedFactor - RetractedFactor);
            norm = Math.Max(0f, Math.Min(1f, norm));
            return RetractedLength + norm * Math.Max(0f, reach - RetractedLength);
        }

        private void DrawBlade(Graphics g, PointF center)
        {
            var bladeLength = GetBladeLength();
            var dx = (float)Math.Cos(currentAngleRad);
            var dy = (float)Math.Sin(currentAngleRad);
            var px = -dy;
            var py = dx;

            // 팔(arm)과 끝단(end effector)
            var armEnd = new PointF(center.X + (bladeLength - 18f) * dx, center.Y + (bladeLength - 18f) * dy);
            using (var armPen = new Pen(Color.FromArgb(150, 160, 176), 12f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                g.DrawLine(armPen, center, armEnd);
            }

            const float halfWidth = 13f;
            const float forkLength = 34f;
            var tip = new PointF(center.X + (bladeLength + 12f) * dx, center.Y + (bladeLength + 12f) * dy);
            var root = new PointF(tip.X - forkLength * dx, tip.Y - forkLength * dy);
            var poly = new[]
            {
                new PointF(root.X + px * halfWidth, root.Y + py * halfWidth),
                new PointF(tip.X + px * halfWidth, tip.Y + py * halfWidth),
                new PointF(tip.X - px * halfWidth, tip.Y - py * halfWidth),
                new PointF(root.X - px * halfWidth, root.Y - py * halfWidth)
            };

            using (var fill = new SolidBrush(Color.FromArgb(244, 246, 250)))
            using (var pen = new Pen(Color.FromArgb(120, 131, 148), 1.5f))
            {
                g.FillPolygon(fill, poly);
                g.DrawPolygon(pen, poly);
            }
        }

        private void DrawWaferOnBlade(Graphics g, PointF center)
        {
            var bladeLength = GetBladeLength();
            var waferCenter = new PointF(
                center.X + bladeLength * (float)Math.Cos(currentAngleRad),
                center.Y + bladeLength * (float)Math.Sin(currentAngleRad));

            const float waferRadius = 22f;
            var waferRect = new RectangleF(waferCenter.X - waferRadius, waferCenter.Y - waferRadius, waferRadius * 2f, waferRadius * 2f);
            using (var waferBrush = new SolidBrush(waferColor))
            using (var waferPen = new Pen(Color.FromArgb(160, 60, 70, 85), 1.2f))
            {
                g.FillEllipse(waferBrush, waferRect);
                g.DrawEllipse(waferPen, waferRect);
            }
        }

        private void DrawMovementRangeLimit(Graphics g, PointF center, float radius)
        {
            // 제한 범위를 시각적으로 표시하기 위한 반지름
            float limitRadius = radius * 2.5f; // TM 본체보다 충분히 큰 반지름
            
            if (isSimulationMode)
            {
                // 시뮬레이션 모드: 원점(FOUP A 쪽) 기준 제한만 표시
                // 원점이 FOUP A 쪽에 있으므로, 아래 방향(90도)은 갈 수 없는 위치
                // 대기 상태에서 TM이 아래로 향해 있는데, 그 위치는 실제로 갈 수 없음
                
                // 원점 제한선: 아래 방향(90도) - 갈 수 없는 위치
                float originLimitAngleRad = (float)(Math.PI / 2); // 90도 (아래)
                
                using (var limitLinePen = new Pen(Color.FromArgb(200, 255, 100, 100), 3f)) // 반투명 빨간색 선
                {
                    float originEndX = center.X + limitRadius * (float)Math.Cos(originLimitAngleRad);
                    float originEndY = center.Y + limitRadius * (float)Math.Sin(originLimitAngleRad);
                    g.DrawLine(limitLinePen, center, new PointF(originEndX, originEndY));
                    
                    // 원점 제한 영역 표시 (아래 방향 반원)
                    using (var limitPen = new Pen(Color.FromArgb(100, 255, 100, 100), 2f)) // 반투명 빨간색
                    {
                        var limitRect = new RectangleF(
                            center.X - limitRadius,
                            center.Y - limitRadius,
                            limitRadius * 2f,
                            limitRadius * 2f);
                        
                        // 아래 방향 반원 그리기 (90도 중심, 좌우 45도씩)
                        float startAngle = 45f; // 좌하단
                        float sweepAngle = 90f; // 45도에서 135도까지 (아래 방향)
                        
                        g.DrawArc(limitPen, limitRect, startAngle, sweepAngle);
                    }
                }
            }
            else
            {
                // 하드웨어 모드: FOUP A/B 제한선 표시
                // FOUP A: 135도 (3π/4, 좌하단) - 좌로 이동 불가
                // FOUP B: 45도 (π/4, 우하단) - 우로 이동 불가
                
                float foupAAngleRad = (float)(3 * Math.PI / 4); // 135도
                float foupBAngleRad = (float)(Math.PI / 4); // 45도
                
                // 제한선 그리기 (FOUP A와 FOUP B 방향의 경계선)
                using (var limitLinePen = new Pen(Color.FromArgb(180, 255, 200, 0), 2.5f)) // 반투명 주황색 선
                {
                    // FOUP A 방향 제한선 (135도) - 이 방향보다 왼쪽으로는 갈 수 없음
                    float foupAEndX = center.X + limitRadius * (float)Math.Cos(foupAAngleRad);
                    float foupAEndY = center.Y + limitRadius * (float)Math.Sin(foupAAngleRad);
                    g.DrawLine(limitLinePen, center, new PointF(foupAEndX, foupAEndY));
                    
                    // FOUP B 방향 제한선 (45도) - 이 방향보다 오른쪽으로는 갈 수 없음
                    float foupBEndX = center.X + limitRadius * (float)Math.Cos(foupBAngleRad);
                    float foupBEndY = center.Y + limitRadius * (float)Math.Sin(foupBAngleRad);
                    g.DrawLine(limitLinePen, center, new PointF(foupBEndX, foupBEndY));
                }
                
                // 제한된 범위를 반투명 호(arc)로 표시 (45도 ~ 135도)
                using (var limitPen = new Pen(Color.FromArgb(80, 255, 200, 0), 2f)) // 반투명 주황색
                {
                    var limitRect = new RectangleF(
                        center.X - limitRadius,
                        center.Y - limitRadius,
                        limitRadius * 2f,
                        limitRadius * 2f);
                    
                    // WinForms의 DrawArc는 0도가 오른쪽, 시계방향
                    // 45도 ~ 135도 범위를 그리기
                    float startAngle = 45f; // FOUP B 방향
                    float sweepAngle = 90f; // 45도에서 135도까지 90도
                    
                    g.DrawArc(limitPen, limitRect, startAngle, sweepAngle);
                }
            }
        }

        private float GetBladeAngle(EquipmentRegion region, bool isSimulation = true)
        {
            // 배치가 정해졌으면 실제 모듈 위치를 향한다.
            if (hasLayout && anchors.TryGetValue(region, out var anchor))
            {
                return (float)Math.Atan2(anchor.Y - hubCenter.Y, anchor.X - hubCenter.X);
            }

            switch (region)
            {
                case EquipmentRegion.ChamberA:
                    return (float)Math.PI;            // 180° - left (9 o'clock)
                case EquipmentRegion.ChamberB:
                    return (float)(-Math.PI / 2);     // -90° - up (12 o'clock)
                case EquipmentRegion.ChamberC:
                    return 0f;                         // right (3 o'clock)
                case EquipmentRegion.FoupA:
                    return (float)(3 * Math.PI / 4);   // 135° → 7 o'clock (screen coords)
                case EquipmentRegion.FoupB:
                    return (float)(Math.PI / 4);       // 45° → 5 o'clock
                case EquipmentRegion.TM:
                    // 시뮬레이션 모드: 원점(FOUP A 쪽) 기준으로 아래 방향(90도)은 갈 수 없는 위치
                    // 대기 상태에서는 FOUP A 방향(135도)으로 설정하여 아래 방향을 피함
                    if (isSimulation)
                    {
                        return (float)(3 * Math.PI / 4); // 135° - FOUP A 방향 (아래 방향 90도는 갈 수 없음)
                    }
                    else
                    {
                        return (float)(Math.PI / 2);     // 하드웨어 모드: 기본 아래 방향
                    }
                default:
                    // 시뮬레이션 모드: 기본값도 아래 방향(90도) 대신 FOUP A 방향 사용
                    if (isSimulation)
                    {
                        return (float)(3 * Math.PI / 4); // 135° - FOUP A 방향
                    }
                    return (float)(Math.PI / 2);       // 하드웨어 모드: down by default
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            var diff = targetExtensionFactor - currentExtensionFactor;
            var angleDiff = NormalizeAngle(targetAngleRad - currentAngleRad);
            bool stillAnimating = false;

            if (Math.Abs(diff) >= 0.005f)
            {
                currentExtensionFactor += diff * 0.4f;
                stillAnimating = true;
            }
            else
            {
                currentExtensionFactor = targetExtensionFactor;
            }

            if (Math.Abs(angleDiff) >= 0.005f)
            {
                currentAngleRad = NormalizeAngle(currentAngleRad + angleDiff * 0.2f);
                stillAnimating = true;
            }
            else
            {
                currentAngleRad = targetAngleRad;
            }

            if (!stillAnimating)
            {
                animationTimer.Stop();
            }

            Invalidate();
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > Math.PI) angle -= (float)(2 * Math.PI);
            while (angle < -Math.PI) angle += (float)(2 * Math.PI);
            return angle;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (animationTimer != null)
                {
                    animationTimer.Stop();
                    animationTimer.Tick -= AnimationTimer_Tick;
                    animationTimer.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}

