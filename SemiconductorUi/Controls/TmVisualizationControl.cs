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

        private readonly List<RectangleF> moduleBounds = new List<RectangleF>();
        private float rangeRadius = 60f;

        // 로봇 몸체(원)와 팔, U자 포크 크기
        private const float BodyRadius = 26f;
        private const float BodyCoreRadius = 20f;
        private const float ArmThickness = 10f;
        private const float WaferRadius = 22f;
        private const float RetractedLength = BodyRadius + WaferRadius + 4f;
        private const float RetractedFactor = 0.7f;
        private const float ExtendedFactor = 1.3f;

        // 회전 범위 고리
        private const float RangeTrackWidth = 6f;
        private const float MinRangeRadius = BodyRadius + 14f;
        private const float MaxRangeRadius = 96f;
        private const float RangeGapToModule = 10f;

        private static readonly Color ArmColor = Color.FromArgb(91, 106, 128);
        private static readonly Color ForkColor = Color.FromArgb(140, 153, 172);
        private static readonly Color ForkExtendedColor = UiTheme.Primary;
        private static readonly Color BodyRing = Color.White;
        private static readonly Color BodyRingEdge = Color.FromArgb(184, 192, 205);
        private static readonly Color BodyCore = Color.FromArgb(43, 53, 70);
        private static readonly Color RangeTrack = Color.FromArgb(213, 220, 230);
        private static readonly Color RangeTick = Color.FromArgb(154, 166, 184);
        private static readonly Color WaferEdge = Color.FromArgb(181, 154, 69);

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
            SetLayout(center, moduleAnchors, null);
        }

        /// <summary>
        /// 모듈 카드 영역도 함께 받아, 회전 범위 고리가 카드에 겹치지 않을 만큼만 크게 그린다.
        /// </summary>
        public void SetLayout(PointF center, IDictionary<EquipmentRegion, PointF> moduleAnchors, IEnumerable<RectangleF> modules)
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

            moduleBounds.Clear();
            if (modules != null)
            {
                moduleBounds.AddRange(modules);
            }
            rangeRadius = CalculateRangeRadius(center);

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

            DrawRotationRange(g, center);
            DrawArm(g, center);
            if (carryingWafer)
            {
                DrawWaferOnBlade(g, center);
            }
            DrawBody(g, center);
            DrawCurrentAngleMarker(g, center);
        }

        /// <summary>
        /// 가장 가까운 모듈 카드까지의 거리에서 여유를 뺀 값. 카드 정보가 없으면 기본값.
        /// </summary>
        private float CalculateRangeRadius(PointF center)
        {
            if (moduleBounds.Count == 0)
            {
                return 60f;
            }

            float nearest = float.MaxValue;
            foreach (var rect in moduleBounds)
            {
                float dx = Math.Max(Math.Max(rect.Left - center.X, 0f), center.X - rect.Right);
                float dy = Math.Max(Math.Max(rect.Top - center.Y, 0f), center.Y - rect.Bottom);
                nearest = Math.Min(nearest, (float)Math.Sqrt(dx * dx + dy * dy));
            }

            return Math.Max(MinRangeRadius, Math.Min(MaxRangeRadius, nearest - RangeGapToModule));
        }

        /// <summary>
        /// TM이 돌 수 있는 범위를 고리로 그린다. FOUP A에서 시계 방향으로 챔버 A·B·C를 지나 FOUP B까지이고,
        /// 그 사이 아래쪽(FOUP B → FOUP A)은 TM이 지나가지 않는 구간이라 비워 둔다.
        /// 고리 위 눈금은 각 모듈 방향이다.
        /// </summary>
        private void DrawRotationRange(Graphics g, PointF center)
        {
            if (anchors.Count == 0)
            {
                return;
            }

            var ring = new RectangleF(center.X - rangeRadius, center.Y - rangeRadius, rangeRadius * 2f, rangeRadius * 2f);
            if (anchors.TryGetValue(EquipmentRegion.FoupA, out var foupA) && anchors.TryGetValue(EquipmentRegion.FoupB, out var foupB))
            {
                float start = ToDegrees(AngleTo(center, foupA));
                float end = ToDegrees(AngleTo(center, foupB));
                float sweep = end - start;
                while (sweep <= 0f) sweep += 360f;

                using (var track = new Pen(RangeTrack, RangeTrackWidth) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    g.DrawArc(track, ring, start, sweep);
                }
            }
            else
            {
                using (var track = new Pen(RangeTrack, RangeTrackWidth))
                {
                    g.DrawEllipse(track, ring);
                }
            }

            using (var tick = new Pen(RangeTick, 2f))
            {
                foreach (var anchor in anchors.Values)
                {
                    float angle = AngleTo(center, anchor);
                    float cos = (float)Math.Cos(angle), sin = (float)Math.Sin(angle);
                    float inner = rangeRadius - RangeTrackWidth - 2f, outer = rangeRadius + RangeTrackWidth + 2f;
                    g.DrawLine(tick, center.X + inner * cos, center.Y + inner * sin, center.X + outer * cos, center.Y + outer * sin);
                }
            }
        }

        /// <summary>
        /// 지금 TM이 향한 각도를 고리 위 파란 점으로 표시한다.
        /// </summary>
        private void DrawCurrentAngleMarker(Graphics g, PointF center)
        {
            if (anchors.Count == 0)
            {
                return;
            }

            const float radius = 5.5f;
            var point = new PointF(
                center.X + rangeRadius * (float)Math.Cos(currentAngleRad),
                center.Y + rangeRadius * (float)Math.Sin(currentAngleRad));
            var rect = new RectangleF(point.X - radius, point.Y - radius, radius * 2f, radius * 2f);
            using (var fill = new SolidBrush(UiTheme.Primary))
            using (var edge = new Pen(Color.White, 2f))
            {
                g.FillEllipse(fill, rect);
                g.DrawEllipse(edge, rect);
            }
        }

        private void DrawBody(Graphics g, PointF center)
        {
            var ringRect = new RectangleF(center.X - BodyRadius, center.Y - BodyRadius, BodyRadius * 2f, BodyRadius * 2f);
            var coreRect = new RectangleF(center.X - BodyCoreRadius, center.Y - BodyCoreRadius, BodyCoreRadius * 2f, BodyCoreRadius * 2f);
            using (var ringFill = new SolidBrush(BodyRing))
            using (var ringEdge = new Pen(BodyRingEdge, 1.5f))
            using (var core = new SolidBrush(BodyCore))
            {
                g.FillEllipse(ringFill, ringRect);
                g.DrawEllipse(ringEdge, ringRect);
                g.FillEllipse(core, coreRect);
            }

            using (var font = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, "TM", font, Rectangle.Round(coreRect), Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }

            // 웨이퍼를 들고 있으면(하드웨어 모드에서는 진공 ON) 몸체 오른쪽 위에 초록 점
            if (carryingWafer)
            {
                const float dot = 5f;
                float offset = BodyRadius * 0.72f;
                var dotRect = new RectangleF(center.X + offset - dot, center.Y - offset - dot, dot * 2f, dot * 2f);
                using (var fill = new SolidBrush(UiTheme.Success))
                using (var edge = new Pen(Color.White, 1.5f))
                {
                    g.FillEllipse(fill, dotRect);
                    g.DrawEllipse(edge, dotRect);
                }
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

        /// <summary>
        /// 실린더 팔과 U자 포크. 팔은 몸체 중심에서 포크 뿌리까지, 포크 두 갈래는 웨이퍼 아래로 들어간다.
        /// 실린더가 전진 쪽에 있으면 포크를 파란색으로 칠한다.
        /// </summary>
        private void DrawArm(Graphics g, PointF center)
        {
            var bladeLength = GetBladeLength();
            var state = g.Save();
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(ToDegrees(currentAngleRad));

            float forkRoot = bladeLength - WaferRadius - 8f;
            using (var armBrush = new SolidBrush(ArmColor))
            using (var armPath = EquipmentCanvasStyler.CreateRoundedRectangle(
                new RectangleF(0f, -ArmThickness / 2f, Math.Max(ArmThickness, forkRoot + 4f), ArmThickness), ArmThickness / 2f))
            {
                g.FillPath(armBrush, armPath);
            }

            bool extended = currentExtensionFactor >= (RetractedFactor + ExtendedFactor) / 2f;
            using (var forkBrush = new SolidBrush(extended ? ForkExtendedColor : ForkColor))
            using (var forkPath = CreateForkPath(forkRoot, bladeLength + WaferRadius - 2f))
            {
                g.FillPath(forkBrush, forkPath);
            }

            g.Restore(state);
        }

        /// <summary>
        /// x축 방향으로 놓인 U자 포크. root에서 시작해 tip에서 끝나며, 가운데가 비어 있다.
        /// </summary>
        private static GraphicsPath CreateForkPath(float root, float tip)
        {
            const float halfWidth = 14f;
            const float prong = 8f;
            const float corner = 6f;
            float slotStart = root + 14f;

            var path = new GraphicsPath();
            path.AddLine(root, -halfWidth, tip - corner, -halfWidth);
            path.AddArc(tip - corner * 2f, -halfWidth, corner * 2f, corner * 2f, 270f, 90f);
            path.AddLine(tip, -halfWidth + corner, tip, -halfWidth + prong);
            path.AddLine(tip, -halfWidth + prong, slotStart, -halfWidth + prong);
            path.AddLine(slotStart, halfWidth - prong, tip, halfWidth - prong);
            path.AddLine(tip, halfWidth - prong, tip, halfWidth - corner);
            path.AddArc(tip - corner * 2f, halfWidth - corner * 2f, corner * 2f, corner * 2f, 0f, 90f);
            path.AddLine(tip - corner, halfWidth, root, halfWidth);
            path.CloseFigure();
            return path;
        }

        private void DrawWaferOnBlade(Graphics g, PointF center)
        {
            var bladeLength = GetBladeLength();
            var waferCenter = new PointF(
                center.X + bladeLength * (float)Math.Cos(currentAngleRad),
                center.Y + bladeLength * (float)Math.Sin(currentAngleRad));

            var waferRect = new RectangleF(waferCenter.X - WaferRadius, waferCenter.Y - WaferRadius, WaferRadius * 2f, WaferRadius * 2f);
            using (var waferBrush = new SolidBrush(waferColor))
            using (var waferPen = new Pen(WaferEdge, 1.2f))
            {
                g.FillEllipse(waferBrush, waferRect);
                g.DrawEllipse(waferPen, waferRect);
            }
        }

        private static float AngleTo(PointF from, PointF to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        private static float ToDegrees(float radians)
        {
            return radians * 180f / (float)Math.PI;
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

