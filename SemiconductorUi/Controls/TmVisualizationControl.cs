using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // 투명 배경인 경우 부모 컨트롤의 배경을 그리기
            if (BackColor == Color.Transparent)
            {
                // 부모 컨트롤의 배경색 사용
                if (Parent != null)
                {
                    using (var brush = new SolidBrush(Parent.BackColor))
                    {
                        g.FillRectangle(brush, ClientRectangle);
                    }
                }
            }
            else
            {
                g.Clear(BackColor);
            }

            var center = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);
            // TM 본체 크기는 고정 (컨트롤 크기와 무관하게 일정한 크기 유지)
            // 컨트롤이 크게 설정되어도 TM 본체는 적절한 크기로 표시
            var radius = 40f; // TM 본체 반지름 고정 (80x80 크기)
            var bodyRect = new RectangleF(center.X - radius, center.Y - radius, radius * 2f, radius * 2f);

            // 블레이드를 먼저 그리기 (TM 본체 뒤에 표시되도록)
            DrawBlade(g, center, radius);

            // 웨이퍼를 블레이드 위에 그리기 (carryingWafer가 true일 때)
            if (carryingWafer)
            {
                DrawWaferOnBlade(g, center, radius);
            }

            // TM 본체 (원형) - 나중에 그리기 (블레이드 위에 표시되도록)
            // 밝은 색상으로 변경
            using (var brush = new LinearGradientBrush(bodyRect,
                       Color.FromArgb(120, 180, 255),  // 더 밝은 파란색
                       Color.FromArgb(60, 120, 220),   // 더 밝은 파란색
                       LinearGradientMode.Vertical))
            using (var outline = new Pen(Color.FromArgb(255, 255, 255), 2.5f))  // 더 밝은 흰색 테두리
            {
                g.FillEllipse(brush, bodyRect);
                g.DrawEllipse(outline, bodyRect);
            }
        }

        private void DrawBlade(Graphics g, PointF center, float radius)
        {
            var angleRad = currentAngleRad;
            var minSide = Math.Min(ClientSize.Width, ClientSize.Height);

            // 블레이드 길이를 확대/축소하는 방식으로 변경
            // extensionFactor에 따라 길이가 변함
            // TM 본체 크기는 고정이므로, 블레이드 길이도 적절한 고정값 사용
            var baseLength = radius * 1.2f;  // 기본 길이 (TM 본체 근처, 48px)
            var maxLength = 180f;  // 최대 길이 (전진 시, 고정값으로 적절한 길이 유지)
            
            // extensionFactor 0.4~1.6을 0~1로 정규화
            var norm = (currentExtensionFactor - 0.4f) / (1.6f - 0.4f);
            norm = Math.Max(0f, Math.Min(1f, norm));
            
            // 전진할수록 길이가 길어짐
            var bladeLength = baseLength + norm * (maxLength - baseLength);
            var bladeWidth = 35f + norm * 10f;  // 전진할수록 약간 더 두껍게

            // 블레이드 시작 위치 (TM 중심에서 시작)
            var bladeStart = center;

            // 블레이드 방향 벡터
            var dx = (float)Math.Cos(angleRad);
            var dy = (float)Math.Sin(angleRad);

            // 블레이드 끝 위치
            var bladeEnd = new PointF(
                bladeStart.X + bladeLength * dx,
                bladeStart.Y + bladeLength * dy);

            // 블레이드 폭 방향(수직 벡터)
            var px = -dy;
            var py = dx;

            var halfWidth = bladeWidth / 2f;

            // 직사각형 블레이드의 네 꼭짓점 계산 (TM 중심에서 시작)
            var p1 = new PointF(
                bladeEnd.X + px * halfWidth,
                bladeEnd.Y + py * halfWidth);
            var p2 = new PointF(
                bladeEnd.X - px * halfWidth,
                bladeEnd.Y - py * halfWidth);
            var p3 = new PointF(
                bladeStart.X - px * halfWidth,
                bladeStart.Y - py * halfWidth);
            var p4 = new PointF(
                bladeStart.X + px * halfWidth,
                bladeStart.Y + py * halfWidth);

            using (var path = new GraphicsPath())
            {
                path.AddPolygon(new[] { p1, p2, p3, p4 });

                // 블레이드 그라디언트 (전진 방향으로)
                var bladeRect = new RectangleF(
                    Math.Min(bladeStart.X, bladeEnd.X) - halfWidth,
                    Math.Min(bladeStart.Y, bladeEnd.Y) - halfWidth,
                    bladeLength + bladeWidth,
                    bladeLength + bladeWidth);
                using (var bladeBrush = new LinearGradientBrush(
                           bladeRect,
                           Color.FromArgb(255, 255, 255),  // 밝은 흰색
                           Color.FromArgb(200, 200, 220),  // 밝은 회색
                           LinearGradientMode.ForwardDiagonal))
                using (var outlinePen = new Pen(Color.FromArgb(150, 150, 170), 2.5f))  // 더 밝은 테두리
                {
                    g.FillPath(bladeBrush, path);
                    g.DrawPath(outlinePen, path);
                }
            }
        }

        private void DrawWaferOnBlade(Graphics g, PointF center, float radius)
        {
            var angleRad = currentAngleRad;
            var minSide = Math.Min(ClientSize.Width, ClientSize.Height);

            // 블레이드 길이 계산 (DrawBlade와 동일한 로직)
            var baseLength = radius * 1.2f;
            var maxLength = 180f;
            var norm = (currentExtensionFactor - 0.4f) / (1.6f - 0.4f);
            norm = Math.Max(0f, Math.Min(1f, norm));
            var bladeLength = baseLength + norm * (maxLength - baseLength);

            // 블레이드 방향 벡터
            var dx = (float)Math.Cos(angleRad);
            var dy = (float)Math.Sin(angleRad);

            // 웨이퍼 위치: 블레이드 끝부분 (TM 중심에서 블레이드 길이만큼 떨어진 위치)
            var waferCenter = new PointF(
                center.X + bladeLength * dx,
                center.Y + bladeLength * dy);

            // 웨이퍼 크기 (Chamber에서 사용하는 크기와 유사하게)
            float waferRadius = 20f; // 웨이퍼 반지름
            var waferRect = new RectangleF(
                waferCenter.X - waferRadius,
                waferCenter.Y - waferRadius,
                waferRadius * 2f,
                waferRadius * 2f);

            // Chamber에서 사용하는 웨이퍼 스타일로 그리기
            // Color.FromArgb(200, 220, 255)와 유사한 색상 사용
            using (var waferBrush = new SolidBrush(waferColor))
            using (var waferPen = new Pen(Color.FromArgb(180, 80, 80, 80), 1.2f))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
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

        private static float GetBladeAngle(EquipmentRegion region, bool isSimulation = true)
        {
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

        private void DrawLabel(Graphics g, PointF center)
        {
            var textRect = new RectangleF(center.X - 40, center.Y - 18, 80, 36);
            TextRenderer.DrawText(
                g,
                "TM",
                new Font(Font.FontFamily, 12f, FontStyle.Bold),
                Rectangle.Round(textRect),
                ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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

