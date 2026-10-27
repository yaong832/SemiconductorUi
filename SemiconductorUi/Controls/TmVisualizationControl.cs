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

        // 로봇 크기: 고정 베이스(원), 회전 몸체, 실린더 빔, 진공 패드가 달린 U자 포크
        private const float BodyRadius = 26f;          // 회전 몸체가 차지하는 반경(블레이드 시작 길이 계산용)
        private const float BaseRadius = 24f;
        private const float BeamThickness = 12f;
        private const float WaferRadius = 22f;
        private const float RetractedLength = BodyRadius + WaferRadius + 4f;
        private const float RetractedFactor = 0.7f;
        private const float ExtendedFactor = 1.3f;

        // 로봇 구역(원판)과 회전 범위 홈
        private const float RangeTrackWidth = 4f;
        private const float MinRangeRadius = BodyRadius + 14f;
        private const float MaxRangeRadius = 96f;
        private const float RangeGapToModule = 10f;
        private const float WellMargin = 7f;

        private static readonly Color WellLight = Color.FromArgb(190, 198, 209);
        private static readonly Color WellDark = Color.FromArgb(166, 175, 188);
        private static readonly Color WellEdge = Color.FromArgb(142, 152, 167);
        private static readonly Color RangeGroove = Color.FromArgb(150, 118, 130, 148);
        private static readonly Color RangeTick = Color.FromArgb(84, 96, 114);
        private static readonly Color BaseLight = Color.FromArgb(234, 238, 243);
        private static readonly Color BaseDark = Color.FromArgb(170, 179, 192);
        private static readonly Color MetalEdge = Color.FromArgb(124, 135, 152);
        private static readonly Color BoltFill = Color.FromArgb(232, 236, 241);
        private static readonly Color BeamLight = Color.FromArgb(205, 212, 222);
        private static readonly Color BeamDark = Color.FromArgb(142, 153, 169);
        private static readonly Color TubeLight = Color.FromArgb(236, 239, 243);
        private static readonly Color TubeDark = Color.FromArgb(137, 148, 164);
        private static readonly Color RodFill = Color.FromArgb(221, 226, 232);
        private static readonly Color TurretLight = Color.FromArgb(75, 84, 101);
        private static readonly Color TurretDark = Color.FromArgb(38, 44, 55);
        private static readonly Color ForkFill = Color.FromArgb(238, 241, 245);
        private static readonly Color ForkEdge = Color.FromArgb(142, 152, 167);
        private static readonly Color PadOff = Color.FromArgb(154, 164, 178);
        private static readonly Color PadOn = Color.FromArgb(59, 209, 126);
        private static readonly Color HoseColor = Color.FromArgb(58, 67, 82);
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
        /// 그림을 얼마나 키워 그릴지(1이면 원래 크기). SetLayout보다 먼저 정해야 한다.
        /// 로봇 모양은 원래 크기 기준으로 그린 뒤 이 배율로 확대한다.
        /// </summary>
        [System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public float DrawScale { get; set; } = 1f;

        /// <summary>
        /// 모듈 카드 영역도 함께 받아, 회전 범위 고리가 카드에 겹치지 않을 만큼만 크게 그린다.
        /// </summary>
        public void SetLayout(PointF center, IDictionary<EquipmentRegion, PointF> moduleAnchors, IEnumerable<RectangleF> modules)
        {
            // 받은 좌표는 화면 픽셀이다. 그림은 확대 전 좌표로 그리므로 배율로 나눠 둔다.
            float k = DrawScale > 0f ? DrawScale : 1f;
            hubCenter = new PointF(center.X / k, center.Y / k);
            anchors.Clear();
            if (moduleAnchors != null)
            {
                foreach (var pair in moduleAnchors)
                {
                    anchors[pair.Key] = new PointF(pair.Value.X / k, pair.Value.Y / k);
                }
            }

            moduleBounds.Clear();
            if (modules != null)
            {
                foreach (var rect in modules)
                {
                    moduleBounds.Add(new RectangleF(rect.X / k, rect.Y / k, rect.Width / k, rect.Height / k));
                }
            }
            rangeRadius = CalculateRangeRadius(hubCenter);

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

            float k = DrawScale > 0f ? DrawScale : 1f;
            if (k != 1f)
            {
                g.ScaleTransform(k, k);
            }

            var center = hasLayout ? hubCenter : new PointF(ClientSize.Width / 2f / k, ClientSize.Height / 2f / k);

            DrawRobotWell(g, center);
            DrawRotationRange(g, center);
            DrawBase(g, center);
            DrawRobot(g, center);
            DrawCenterMarks(g, center);
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

                using (var track = new Pen(RangeGroove, RangeTrackWidth) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    g.DrawArc(track, ring, start, sweep);
                }
            }
            else
            {
                using (var track = new Pen(RangeGroove, RangeTrackWidth))
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

        /// <summary>
        /// 로봇이 도는 움푹한 원판. 회전 범위 홈 바깥으로 조금 더 크게, 모듈 카드에는 닿지 않게 그린다.
        /// </summary>
        private void DrawRobotWell(Graphics g, PointF center)
        {
            float radius = rangeRadius + WellMargin;
            var rect = new RectangleF(center.X - radius, center.Y - radius, radius * 2f, radius * 2f);
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using (var brush = new PathGradientBrush(path)
                {
                    CenterColor = WellLight,
                    SurroundColors = new[] { WellDark },
                    CenterPoint = new PointF(center.X, center.Y - radius * 0.1f)
                })
                {
                    g.FillPath(brush, path);
                }
            }
            using (var edge = new Pen(WellEdge, 1.4f))
            using (var inner = new Pen(Color.FromArgb(120, 255, 255, 255), 1f))
            {
                g.DrawEllipse(edge, rect);
                g.DrawEllipse(inner, RectangleF.Inflate(rect, -3f, -3f));
            }
        }

        /// <summary>
        /// 바닥에 고정된 둥근 베이스와 볼트 링.
        /// </summary>
        private static void DrawBase(Graphics g, PointF center)
        {
            var rect = new RectangleF(center.X - BaseRadius, center.Y - BaseRadius, BaseRadius * 2f, BaseRadius * 2f);
            using (var shadow = new SolidBrush(Color.FromArgb(45, 28, 36, 51)))
            {
                g.FillEllipse(shadow, rect.X + 1.5f, rect.Y + 2.5f, rect.Width, rect.Height);
            }
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using (var brush = new PathGradientBrush(path)
                {
                    CenterColor = BaseLight,
                    SurroundColors = new[] { BaseDark },
                    CenterPoint = new PointF(center.X - BaseRadius * 0.3f, center.Y - BaseRadius * 0.35f)
                })
                {
                    g.FillPath(brush, path);
                }
            }
            using (var edge = new Pen(MetalEdge, 1.2f))
            {
                g.DrawEllipse(edge, rect);
            }

            const int bolts = 10;
            float boltRadius = BaseRadius - 4f;
            using (var fill = new SolidBrush(BoltFill))
            using (var pen = new Pen(MetalEdge, 0.7f))
            {
                for (int i = 0; i < bolts; i++)
                {
                    double angle = (i + 0.5) * 2 * Math.PI / bolts;
                    var bolt = new RectangleF(center.X + boltRadius * (float)Math.Cos(angle) - 1.6f, center.Y + boltRadius * (float)Math.Sin(angle) - 1.6f, 3.2f, 3.2f);
                    g.FillEllipse(fill, bolt);
                    g.DrawEllipse(pen, bolt);
                }
            }
        }

        /// <summary>
        /// 회전하는 로봇: 몸체, 공압 실린더(배럴 + 로드), 빔, 진공 패드가 달린 U자 포크, 진공 호스.
        /// 실린더가 전진 쪽이면 포크 테두리를 파랗게, 웨이퍼를 들고 있으면(진공 ON) 패드를 초록으로 칠한다.
        /// </summary>
        private void DrawRobot(Graphics g, PointF center)
        {
            var bladeLength = GetBladeLength();
            float forkRoot = bladeLength - WaferRadius - 8f;
            float forkTip = bladeLength + WaferRadius - 2f;
            bool extended = currentExtensionFactor >= (RetractedFactor + ExtendedFactor) / 2f;

            var state = g.Save();
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(ToDegrees(currentAngleRad));

            // 빔 (몸체에서 포크 뿌리까지)
            var beam = new RectangleF(12f, -BeamThickness / 2f, Math.Max(10f, forkRoot + 6f - 12f), BeamThickness);
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(beam, 3f))
            using (var fill = new LinearGradientBrush(new RectangleF(beam.X, beam.Y - 0.5f, beam.Width, beam.Height + 1f), BeamLight, BeamDark, LinearGradientMode.Vertical))
            using (var pen = new Pen(MetalEdge, 0.8f))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }
            using (var groove = new Pen(MetalEdge, 1f))
            {
                g.DrawLine(groove, beam.Left + 4f, 0f, beam.Right - 4f, 0f);
            }

            // 포크 (전진이면 파란 테두리와 은은한 빛)
            using (var forkPath = CreateForkPath(forkRoot, forkTip))
            {
                if (extended)
                {
                    using (var glow = new Pen(Color.FromArgb(70, UiTheme.Primary), 5f) { LineJoin = LineJoin.Round })
                    {
                        g.DrawPath(glow, forkPath);
                    }
                }
                using (var fill = new SolidBrush(ForkFill))
                using (var pen = new Pen(extended ? UiTheme.Primary : ForkEdge, extended ? 1.6f : 1f))
                {
                    g.FillPath(fill, forkPath);
                    g.DrawPath(pen, forkPath);
                }
            }

            // 진공 패드 3개 (두 갈래 끝 + 뿌리)
            using (var pad = new SolidBrush(carryingWafer ? PadOn : PadOff))
            using (var padEdge = new Pen(Color.FromArgb(94, 106, 122), 0.6f))
            {
                foreach (var p in new[] { new PointF(forkTip - 7f, -10f), new PointF(forkTip - 7f, 10f), new PointF(forkRoot + 7f, 0f) })
                {
                    var r = new RectangleF(p.X - 2.6f, p.Y - 2.6f, 5.2f, 5.2f);
                    g.FillEllipse(pad, r);
                    g.DrawEllipse(padEdge, r);
                }
            }

            if (carryingWafer)
            {
                var wafer = new RectangleF(bladeLength - WaferRadius, -WaferRadius, WaferRadius * 2f, WaferRadius * 2f);
                using (var brush = new SolidBrush(waferColor))
                using (var pen = new Pen(WaferEdge, 1.2f))
                {
                    g.FillEllipse(brush, wafer);
                    g.DrawEllipse(pen, wafer);
                }
            }

            // 공압 실린더: 배럴은 몸체 옆에 고정, 로드는 포크 뿌리까지 나간다
            const float barrelTop = -20f, barrelHeight = 10f, barrelStart = 4f, barrelEnd = 40f;
            float rodEnd = Math.Max(barrelEnd + 3f, forkRoot - 2f);
            using (var rod = new SolidBrush(RodFill))
            using (var rodPen = new Pen(ForkEdge, 0.6f))
            {
                g.FillRectangle(rod, barrelEnd - 2f, barrelTop + 3f, rodEnd - barrelEnd + 2f, 4f);
                g.DrawRectangle(rodPen, barrelEnd - 2f, barrelTop + 3f, rodEnd - barrelEnd + 2f, 4f);
            }
            var barrel = new RectangleF(barrelStart, barrelTop, barrelEnd - barrelStart, barrelHeight);
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(barrel, 3f))
            using (var fill = new LinearGradientBrush(new RectangleF(barrel.X, barrel.Y - 0.5f, barrel.Width, barrel.Height + 1f), TubeLight, TubeDark, LinearGradientMode.Vertical))
            using (var pen = new Pen(MetalEdge, 0.8f))
            using (var cap = new SolidBrush(BaseDark))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
                g.FillRectangle(cap, barrel.Left, barrel.Top + 0.5f, 4f, barrel.Height - 1f);
                g.FillRectangle(cap, barrel.Right - 4f, barrel.Top + 0.5f, 4f, barrel.Height - 1f);
            }

            // 회전 몸체
            var turret = new RectangleF(-20f, -18f, 40f, 36f);
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(turret, 8f))
            using (var fill = new LinearGradientBrush(new RectangleF(turret.X, turret.Y - 0.5f, turret.Width, turret.Height + 1f), TurretLight, TurretDark, LinearGradientMode.Vertical))
            using (var pen = new Pen(Color.FromArgb(31, 37, 48), 1f))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }

            // 진공 호스 (몸체 → 포크 뿌리)
            using (var hose = new Pen(HoseColor, 2.2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                g.DrawBezier(hose, new PointF(10f, 14f), new PointF(26f, 20f), new PointF(forkRoot - 18f, 18f), new PointF(forkRoot + 2f, 9f));
            }

            g.Restore(state);
        }

        /// <summary>
        /// 몸체 위 "TM" 글자와 진공 표시(웨이퍼를 들고 있으면 초록 점). 글자는 회전하지 않는다.
        /// </summary>
        private void DrawCenterMarks(Graphics g, PointF center)
        {
            using (var font = new Font("Segoe UI", 8F, FontStyle.Bold))
            {
                var rect = new Rectangle((int)(center.X - 16), (int)(center.Y - 8), 32, 16);
                CanvasText.Draw(g, "TM", font, rect, Color.FromArgb(233, 237, 243),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }

            if (carryingWafer)
            {
                const float dot = 4f;
                var dotRect = new RectangleF(center.X + 13f - dot, center.Y - 13f - dot, dot * 2f, dot * 2f);
                using (var fill = new SolidBrush(PadOn))
                using (var edge = new Pen(Color.White, 1.2f))
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
        /// x축 방향으로 놓인 U자 포크. root에서 시작해 tip에서 끝나며, 가운데가 비어 있다.
        /// </summary>
        private static GraphicsPath CreateForkPath(float root, float tip)
        {
            const float halfWidth = 15f;
            const float prong = 9f;
            const float corner = 6f;
            float slotStart = root + 16f;

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

