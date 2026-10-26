using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SemiconductorUi.Controllers;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Controls
{
    /// <summary>
    /// TM 기준 화면에 그릴 값. 위에서 본 화면(TmVisualizationControl)에 없는 높이와 층을 보여 주기 위한 것이다.
    /// </summary>
    public sealed class TmSideViewState
    {
        /// <summary>TM이 향한 모듈. 모듈이 아니면 블레이드만 그린다.</summary>
        public EquipmentRegion Target { get; set; } = EquipmentRegion.TM;

        /// <summary>블레이드 높이 (Axis1). 하드웨어 모드는 실제 위치, 시뮬레이션은 추정값.</summary>
        public long Height { get; set; }

        /// <summary>실린더 전진 정도 0(후진) ~ 1(전진).</summary>
        public float Extension { get; set; }

        public bool VacuumOn { get; set; }
        public bool WaferOnBlade { get; set; }
        public Color WaferColor { get; set; } = Color.FromArgb(250, 232, 168);

        /// <summary>FOUP 1~5층에 웨이퍼가 있는지 (인덱스 0 = 1층).</summary>
        public bool[] FoupLevels { get; set; } = new bool[TmHeightEstimator.FoupLevels];

        /// <summary>챔버 스테이지에 웨이퍼가 있는지.</summary>
        public bool ChamberHasWafer { get; set; }

        /// <summary>true면 실제 Axis1 값, false면 시뮬레이션 추정값.</summary>
        public bool IsHardware { get; set; }
    }

    /// <summary>
    /// TM이 향한 모듈을 TM 쪽에서 본 화면. 정면(모듈 앞모습)과 옆면(단면) 두 가지를 머리글 버튼으로 바꾼다.
    /// 높이 눈금은 모든 모듈이 같은 Axis1 값을 쓰므로 챔버와 FOUP 높이를 그대로 비교할 수 있다.
    /// </summary>
    public class TmSideViewControl : Control
    {
        public enum ViewMode
        {
            Front,
            Cut
        }

        private const int HeaderHeight = 30;
        private const int FooterHeight = 22;
        private const int AxisWidth = 26;
        private const int Pad = 10;
        private const int ToggleWidth = 38;
        private const int ToggleHeight = 20;
        private const int BladeThickness = 5;
        private const int WaferThickness = 5;

        private static readonly Color Grid = Color.FromArgb(222, 227, 235);
        private static readonly Color AxisText = Color.FromArgb(140, 150, 164);
        private static readonly Color ShelfColor = Color.FromArgb(170, 179, 193);
        private static readonly Color BladeFill = Color.FromArgb(238, 241, 245);
        private static readonly Color BladeEdge = Color.FromArgb(142, 152, 167);
        private static readonly Color PadOff = Color.FromArgb(154, 164, 178);
        private static readonly Color PadOn = Color.FromArgb(59, 209, 126);
        private static readonly Color CombColor = Color.FromArgb(90, 100, 116);
        private static readonly Color ChamberInside = Color.FromArgb(29, 37, 50);
        private static readonly Color SteelLight = Color.FromArgb(238, 241, 245);
        private static readonly Color SteelDark = Color.FromArgb(190, 198, 209);
        private static readonly Color SteelEdge = Color.FromArgb(142, 152, 167);
        private static readonly Color SmokeFill = Color.FromArgb(70, 95, 106, 122);
        private static readonly Color SmokeEdge = Color.FromArgb(60, 69, 84);
        private static readonly Color CarriageColor = Color.FromArgb(91, 106, 128);
        private static readonly Color MastFill = Color.FromArgb(214, 220, 229);
        private static readonly Color MastEdge = Color.FromArgb(174, 184, 198);
        private static readonly Color PedestalColor = Color.FromArgb(214, 220, 229);
        private static readonly Color WaferEdge = Color.FromArgb(181, 154, 69);
        private static readonly Color ToggleBack = Color.FromArgb(242, 245, 249);
        private static readonly Color ToggleOn = Color.FromArgb(232, 239, 250);

        private TmHardwareController.TmPositionSet positions = new TmHardwareController.TmPositionSet();
        private TmSideViewState state = new TmSideViewState();
        private ViewMode mode = ViewMode.Front;

        public TmSideViewControl()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = EquipmentCanvasStyler.CanvasBack;
        }

        public ViewMode Mode
        {
            get => mode;
            set
            {
                if (mode == value) return;
                mode = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 모듈 높이(선반, 스테이지)를 그릴 티칭 값. 하드웨어 모드는 TmHardwareController.Positions를 넘긴다.
        /// </summary>
        public void SetPositions(TmHardwareController.TmPositionSet teaching)
        {
            positions = teaching ?? new TmHardwareController.TmPositionSet();
            Invalidate();
        }

        public void UpdateState(TmSideViewState newState)
        {
            state = newState ?? new TmSideViewState();
            Invalidate();
        }

        #region 머리글 / 전환 버튼

        private Rectangle CardRect => new Rectangle(0, 0, Math.Max(1, ClientSize.Width - 1), Math.Max(1, ClientSize.Height - 1));

        private Rectangle FrontToggleRect => new Rectangle(CardRect.Right - Pad - ToggleWidth * 2 - 2, (HeaderHeight - ToggleHeight) / 2 + 1, ToggleWidth, ToggleHeight);

        private Rectangle CutToggleRect => new Rectangle(CardRect.Right - Pad - ToggleWidth, (HeaderHeight - ToggleHeight) / 2 + 1, ToggleWidth, ToggleHeight);

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (FrontToggleRect.Contains(e.Location))
            {
                Mode = ViewMode.Front;
            }
            else if (CutToggleRect.Contains(e.Location))
            {
                Mode = ViewMode.Cut;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool overToggle = FrontToggleRect.Contains(e.Location) || CutToggleRect.Contains(e.Location);
            Cursor = overToggle ? Cursors.Hand : Cursors.Default;
        }

        private void DrawHeader(Graphics g)
        {
            string title = (mode == ViewMode.Front ? "TM 정면" : "TM 옆면") + " · " + GetTargetName(state.Target);
            using (var font = new Font("Segoe UI", 9F, FontStyle.Bold))
            {
                var titleRect = new Rectangle(Pad, 0, FrontToggleRect.Left - Pad - 4, HeaderHeight);
                TextRenderer.DrawText(g, title, font, titleRect, EquipmentCanvasStyler.CardTitle,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }

            DrawToggle(g, FrontToggleRect, "정면", mode == ViewMode.Front);
            DrawToggle(g, CutToggleRect, "옆면", mode == ViewMode.Cut);

            using (var divider = new Pen(Color.FromArgb(232, 236, 242)))
            {
                g.DrawLine(divider, Pad, HeaderHeight, CardRect.Right - Pad, HeaderHeight);
            }
        }

        private static void DrawToggle(Graphics g, Rectangle rect, string text, bool on)
        {
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(rect, 5))
            using (var fill = new SolidBrush(on ? ToggleOn : ToggleBack))
            {
                g.FillPath(fill, path);
            }

            using (var font = new Font("Segoe UI", 8F, on ? FontStyle.Bold : FontStyle.Regular))
            {
                TextRenderer.DrawText(g, text, font, rect, on ? UiTheme.Primary : UiTheme.TextSecondary,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? EquipmentCanvasStyler.CanvasBack);

            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(CardRect, EquipmentCanvasStyler.CardRadius))
            using (var fill = new SolidBrush(EquipmentCanvasStyler.CardFill))
            using (var pen = new Pen(EquipmentCanvasStyler.CardBorder))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }

            DrawHeader(g);

            var body = new Rectangle(Pad, HeaderHeight + 12, CardRect.Width - Pad * 2, CardRect.Height - HeaderHeight - FooterHeight - 34);
            if (body.Width < 60 || body.Height < 60)
            {
                return;
            }

            float maxHeight = GetMaxHeight();
            DrawHeightAxis(g, body, maxHeight);

            var drawArea = Rectangle.FromLTRB(body.Left + AxisWidth, body.Top, body.Right, body.Bottom);
            if (mode == ViewMode.Front)
            {
                DrawFront(g, drawArea, body, maxHeight);
            }
            else
            {
                DrawCut(g, drawArea, body, maxHeight);
            }

            DrawFooter(g);
        }

        #region 높이 눈금

        /// <summary>
        /// 가장 높은 티칭 높이에 여유를 더한 값. 모든 모듈이 같은 눈금을 쓴다.
        /// </summary>
        private float GetMaxHeight()
        {
            long max = Math.Max(positions.ChamberA_RaiseY, Math.Max(positions.ChamberB_RaiseY, positions.ChamberC_RaiseY));
            foreach (var value in positions.FoupA_RaiseY) max = Math.Max(max, value);
            foreach (var value in positions.FoupB_RaiseY) max = Math.Max(max, value);
            max = Math.Max(max, state.Height);
            return Math.Max(1f, max * 1.08f);
        }

        private static float MapY(Rectangle body, float maxHeight, long height)
        {
            float clamped = Math.Max(0f, Math.Min(maxHeight, height));
            return body.Bottom - clamped / maxHeight * body.Height;
        }

        private static void DrawHeightAxis(Graphics g, Rectangle body, float maxHeight)
        {
            using (var grid = new Pen(Grid) { DashStyle = DashStyle.Dash })
            using (var font = new Font("Segoe UI", 7F))
            {
                for (long h = 0; h <= maxHeight; h += 1000000)
                {
                    float y = MapY(body, maxHeight, h);
                    g.DrawLine(grid, body.Left + AxisWidth - 2, y, body.Right, y);
                    string label = h == 0 ? "0" : (h / 1000000) + "M";
                    TextRenderer.DrawText(g, label, font, new Rectangle(body.Left, (int)y - 7, AxisWidth - 6, 14), AxisText,
                        TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                }
            }
        }

        #endregion

        #region 정면

        private void DrawFront(Graphics g, Rectangle area, Rectangle body, float maxHeight)
        {
            var frame = Rectangle.FromLTRB(area.Left + 4, area.Top, area.Right - 4, area.Bottom);
            float bladeY = MapY(body, maxHeight, state.Height);

            if (TmHeightEstimator.IsFoup(state.Target))
            {
                float top = MapY(body, maxHeight, GetFoupTopHeight()) - 10;
                var box = RectangleF.FromLTRB(frame.Left, top, frame.Right, body.Bottom + 2);
                FillFoupShell(g, box);
                using (var shelf = new SolidBrush(CombColor))
                using (var font = new Font("Segoe UI", 7F))
                {
                    for (int level = 1; level <= TmHeightEstimator.FoupLevels; level++)
                    {
                        float y = MapY(body, maxHeight, TmHeightEstimator.GetLandHeight(state.Target, positions, level));
                        g.FillRectangle(shelf, box.Left + 6, y, 12, 3);
                        g.FillRectangle(shelf, box.Right - 18, y, 12, 3);
                        TextRenderer.DrawText(g, level.ToString(), font, new Rectangle((int)box.Left + 6, (int)y - 13, 12, 12), AxisText,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
                        if (state.FoupLevels != null && level - 1 < state.FoupLevels.Length && state.FoupLevels[level - 1])
                        {
                            DrawFlatWafer(g, box.Left + 16, box.Right - 16, y, state.WaferColor);
                        }
                    }
                }
            }
            else if (TmHeightEstimator.IsModule(state.Target))
            {
                long land = TmHeightEstimator.GetLandHeight(state.Target, positions, 1);
                long raise = TmHeightEstimator.GetRaiseHeight(state.Target, positions, 1);
                long margin = Math.Max(150000, (raise - land) / 2);
                float bodyTop = MapY(body, maxHeight, raise + margin * 2);
                float bodyBottom = MapY(body, maxHeight, Math.Max(0, land - margin * 2));
                var box = RectangleF.FromLTRB(frame.Left, bodyTop, frame.Right, bodyBottom);
                FillSteelBody(g, box);

                // 슬릿 입구
                var slit = RectangleF.FromLTRB(box.Left + 14, MapY(body, maxHeight, raise + margin), box.Right - 14, MapY(body, maxHeight, Math.Max(0, land - margin)));
                using (var slitPath = EquipmentCanvasStyler.CreateRoundedRectangle(slit, 5))
                using (var slitFill = new LinearGradientBrush(RectangleF.Inflate(slit, 0, 1), ChamberInside, Color.FromArgb(58, 70, 92), LinearGradientMode.Vertical))
                using (var slitPen = new Pen(Color.FromArgb(110, 121, 135)))
                {
                    g.FillPath(slitFill, slitPath);
                    g.DrawPath(slitPen, slitPath);
                }

                // 스테이지와 받침대
                float stageY = MapY(body, maxHeight, land);
                float mid = (box.Left + box.Right) / 2f;
                using (var stage = new SolidBrush(ShelfColor))
                using (var pedestal = new SolidBrush(PedestalColor))
                {
                    g.FillRectangle(stage, mid - slit.Width * 0.35f, stageY, slit.Width * 0.7f, 4);
                    g.FillRectangle(pedestal, mid - 6, box.Bottom, 12, Math.Max(0, body.Bottom + 2 - box.Bottom));
                }

                if (state.ChamberHasWafer)
                {
                    DrawFlatWafer(g, mid - slit.Width * 0.32f, mid + slit.Width * 0.32f, stageY, state.WaferColor);
                }
            }
            else
            {
                DrawCenteredNote(g, frame, "TM이 모듈을 향하고 있지 않습니다");
            }

            // 블레이드: 지금 높이에 가로 막대. 전진이면 파란색, 아래 캐리지는 흐리게(안쪽으로 들어감).
            bool extended = state.Extension >= 0.5f;
            float bladeMid = (frame.Left + frame.Right) / 2f;
            float bladeHalf = frame.Width * 0.3f;
            using (var guide = new Pen(Color.FromArgb(120, UiTheme.Primary)) { DashStyle = DashStyle.Dash })
            {
                g.DrawLine(guide, area.Left - 2, bladeY, frame.Right, bladeY);
            }
            if (state.WaferOnBlade)
            {
                DrawFlatWafer(g, bladeMid - bladeHalf - 6, bladeMid + bladeHalf + 6, bladeY, state.WaferColor);
            }
            using (var carriage = new SolidBrush(Color.FromArgb(extended ? 90 : 255, CarriageColor)))
            {
                g.FillRectangle(carriage, bladeMid - 12, bladeY + BladeThickness, 24, 12);
            }
            DrawBlade(g, new RectangleF(bladeMid - bladeHalf, bladeY, bladeHalf * 2f, BladeThickness), extended,
                new[] { bladeMid - bladeHalf * 0.7f, bladeMid + bladeHalf * 0.7f });
            if (state.VacuumOn)
            {
                DrawVacuumDot(g, new PointF(bladeMid + 22, bladeY + BladeThickness + 6));
            }
        }

        #endregion

        #region 옆면

        private void DrawCut(Graphics g, Rectangle area, Rectangle body, float maxHeight)
        {
            float mastX = area.Left + 14;
            float moduleLeft = area.Left + area.Width * 0.46f;
            float moduleRight = area.Right - 2;
            float moduleMid = (moduleLeft + moduleRight) / 2f;
            float bladeY = MapY(body, maxHeight, state.Height);

            // TM 기둥 (상하 축)
            using (var mast = new SolidBrush(MastFill))
            using (var mastPen = new Pen(MastEdge))
            using (var baseBrush = new SolidBrush(Color.FromArgb(43, 53, 70)))
            {
                g.FillRectangle(mast, mastX - 5, body.Top, 10, body.Height);
                g.DrawRectangle(mastPen, mastX - 5, body.Top, 10, body.Height);
                g.FillRectangle(baseBrush, mastX - 16, body.Bottom, 32, 7);
            }

            if (TmHeightEstimator.IsFoup(state.Target))
            {
                float top = MapY(body, maxHeight, GetFoupTopHeight()) - 10;
                var box = RectangleF.FromLTRB(moduleLeft, top, moduleRight, body.Bottom + 2);
                FillFoupShell(g, box, true);
                using (var shelf = new SolidBrush(CombColor))
                {
                    for (int level = 1; level <= TmHeightEstimator.FoupLevels; level++)
                    {
                        float y = MapY(body, maxHeight, TmHeightEstimator.GetLandHeight(state.Target, positions, level));
                        g.FillRectangle(shelf, box.Right - box.Width * 0.35f, y, box.Width * 0.35f - 4, 3);
                        if (state.FoupLevels != null && level - 1 < state.FoupLevels.Length && state.FoupLevels[level - 1])
                        {
                            DrawFlatWafer(g, moduleMid - box.Width * 0.3f, moduleMid + box.Width * 0.3f, y, state.WaferColor);
                        }
                    }
                }
            }
            else if (TmHeightEstimator.IsModule(state.Target))
            {
                long land = TmHeightEstimator.GetLandHeight(state.Target, positions, 1);
                long raise = TmHeightEstimator.GetRaiseHeight(state.Target, positions, 1);
                long margin = Math.Max(150000, (raise - land) / 2);
                float bodyTop = MapY(body, maxHeight, raise + margin * 2);
                float bodyBottom = MapY(body, maxHeight, Math.Max(0, land - margin * 2));
                float slitTop = MapY(body, maxHeight, raise + margin);
                float slitBottom = MapY(body, maxHeight, Math.Max(0, land - margin));

                // TM 쪽 벽만 슬릿 높이에서 뚫린 단면
                using (var fill = new SolidBrush(ChamberInside))
                using (var pen = new Pen(EquipmentCanvasStyler.CardBorder, 1.5f))
                {
                    g.FillRectangle(fill, moduleLeft, bodyTop, moduleRight - moduleLeft, bodyBottom - bodyTop);
                    g.DrawLine(pen, moduleLeft, bodyTop, moduleRight, bodyTop);
                    g.DrawLine(pen, moduleRight, bodyTop, moduleRight, bodyBottom);
                    g.DrawLine(pen, moduleRight, bodyBottom, moduleLeft, bodyBottom);
                    g.DrawLine(pen, moduleLeft, bodyTop, moduleLeft, slitTop);
                    g.DrawLine(pen, moduleLeft, slitBottom, moduleLeft, bodyBottom);
                }
                using (var valve = new SolidBrush(EquipmentCanvasStyler.ValveClosed))
                {
                    g.FillRectangle(valve, moduleLeft - 2, slitTop - 8, 4, 8);
                    g.FillRectangle(valve, moduleLeft - 2, slitBottom, 4, 8);
                }

                float stageY = MapY(body, maxHeight, land);
                using (var stage = new SolidBrush(ShelfColor))
                using (var pedestal = new SolidBrush(PedestalColor))
                {
                    g.FillRectangle(stage, moduleMid - (moduleRight - moduleLeft) * 0.3f, stageY, (moduleRight - moduleLeft) * 0.6f, 4);
                    g.FillRectangle(pedestal, moduleMid - 5, stageY + 4, 10, Math.Max(0, bodyBottom - stageY - 4));
                }
                if (state.ChamberHasWafer)
                {
                    DrawFlatWafer(g, moduleMid - (moduleRight - moduleLeft) * 0.28f, moduleMid + (moduleRight - moduleLeft) * 0.28f, stageY, state.WaferColor);
                }
            }
            else
            {
                DrawCenteredNote(g, Rectangle.Round(RectangleF.FromLTRB(moduleLeft, area.Top, moduleRight, area.Bottom)), "모듈 없음");
            }

            // 실린더 팔과 블레이드. 후진이면 기둥 가까이, 전진이면 모듈 스테이지 가운데까지.
            bool extended = state.Extension >= 0.5f;
            float bladeLength = Math.Max(30f, (moduleRight - moduleLeft) * 0.55f);
            float tipRetracted = mastX + 14 + bladeLength;
            float tipExtended = moduleMid + bladeLength / 2f;
            float tip = tipRetracted + (tipExtended - tipRetracted) * Math.Max(0f, Math.Min(1f, state.Extension));

            using (var carriage = new SolidBrush(CarriageColor))
            {
                g.FillRectangle(carriage, mastX - 11, bladeY - 2, 22, 16);
                g.FillRectangle(carriage, mastX + 11, bladeY + BladeThickness, Math.Max(2f, tip - bladeLength - mastX - 11), 5);
            }
            DrawBlade(g, new RectangleF(tip - bladeLength, bladeY, bladeLength, BladeThickness), extended,
                new[] { tip - bladeLength + 8f, tip - 8f });
            if (state.WaferOnBlade)
            {
                DrawFlatWafer(g, tip - bladeLength, tip, bladeY, state.WaferColor);
            }
            if (state.VacuumOn)
            {
                DrawVacuumDot(g, new PointF(mastX, bladeY + 6));
            }
        }

        #endregion

        #region 공통

        private long GetFoupTopHeight()
        {
            return TmHeightEstimator.GetRaiseHeight(state.Target, positions, TmHeightEstimator.FoupLevels);
        }

        private void DrawFooter(Graphics g)
        {
            string cylinder = state.Extension >= 0.5f ? "전진" : "후진";
            string vacuum = state.VacuumOn ? "진공 ON" : "진공 OFF";
            string height = state.IsHardware ? state.Height.ToString("N0") : "약 " + state.Height.ToString("N0");
            string text = "높이 " + height + " · " + cylinder + " · " + vacuum;
            var rect = new Rectangle(Pad, CardRect.Bottom - FooterHeight, CardRect.Width - Pad * 2, FooterHeight - 4);
            using (var font = new Font("Segoe UI", 8F))
            {
                TextRenderer.DrawText(g, text, font, rect, EquipmentCanvasStyler.CardSubText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
        }

        /// <summary>
        /// 챔버 몸체: 스테인리스 느낌의 가로 그라데이션과 모서리 볼트.
        /// </summary>
        private static void FillSteelBody(Graphics g, RectangleF rect)
        {
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(rect, 6))
            using (var fill = new LinearGradientBrush(RectangleF.Inflate(rect, 1, 0), SteelDark, SteelLight, LinearGradientMode.Horizontal))
            using (var pen = new Pen(SteelEdge))
            {
                fill.SetBlendTriangularShape(0.35f);
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }
            using (var bolt = new SolidBrush(Color.FromArgb(232, 236, 241)))
            using (var boltPen = new Pen(SteelEdge, 0.7f))
            {
                foreach (var p in new[] { new PointF(rect.Left + 7, rect.Top + 7), new PointF(rect.Right - 7, rect.Top + 7), new PointF(rect.Left + 7, rect.Bottom - 7), new PointF(rect.Right - 7, rect.Bottom - 7) })
                {
                    g.FillEllipse(bolt, p.X - 2.2f, p.Y - 2.2f, 4.4f, 4.4f);
                    g.DrawEllipse(boltPen, p.X - 2.2f, p.Y - 2.2f, 4.4f, 4.4f);
                }
            }
        }

        /// <summary>
        /// FOUP 통: 반투명 회색 몸통과 위쪽 플랜지. FOUP에는 문이 없어서,
        /// 옆에서 자른 그림(openLeft)은 TM 쪽(왼쪽) 벽을 그리지 않고 열어 둔다.
        /// </summary>
        private static void FillFoupShell(Graphics g, RectangleF rect, bool openLeft = false)
        {
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(rect, 8))
            using (var fill = new SolidBrush(SmokeFill))
            {
                g.FillPath(fill, path);
            }
            using (var pen = new Pen(SmokeEdge, 1.4f))
            {
                if (openLeft)
                {
                    const float r = 8f;
                    using (var outline = new GraphicsPath())
                    {
                        outline.AddLine(rect.Left, rect.Top, rect.Right - r, rect.Top);
                        outline.AddArc(rect.Right - r * 2f, rect.Top, r * 2f, r * 2f, 270f, 90f);
                        outline.AddLine(rect.Right, rect.Top + r, rect.Right, rect.Bottom - r);
                        outline.AddArc(rect.Right - r * 2f, rect.Bottom - r * 2f, r * 2f, r * 2f, 0f, 90f);
                        outline.AddLine(rect.Right - r, rect.Bottom, rect.Left, rect.Bottom);
                        g.DrawPath(pen, outline);
                    }
                }
                else
                {
                    using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(rect, 8))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            using (var flange = new SolidBrush(SmokeEdge))
            {
                float w = Math.Min(48f, rect.Width * 0.3f);
                g.FillRectangle(flange, rect.Left + (rect.Width - w) / 2f, rect.Top - 6f, w, 6f);
            }
        }

        /// <summary>
        /// 블레이드(포크) 단면: 밝은 판, 전진이면 파란 테두리, 진공 ON이면 패드가 초록.
        /// </summary>
        private void DrawBlade(Graphics g, RectangleF rect, bool extended, float[] padCenters)
        {
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(rect, 2))
            using (var fill = new SolidBrush(BladeFill))
            using (var pen = new Pen(extended ? UiTheme.Primary : BladeEdge, extended ? 1.6f : 1f))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }
            using (var pad = new SolidBrush(state.VacuumOn ? PadOn : PadOff))
            {
                foreach (var x in padCenters)
                {
                    g.FillRectangle(pad, x - 4f, rect.Top - 2f, 8f, 2.5f);
                }
            }
        }

        /// <summary>
        /// 옆에서 본 웨이퍼 (얇은 막대). 아랫면이 bottomY에 놓인다.
        /// </summary>
        private static void DrawFlatWafer(Graphics g, float left, float right, float bottomY, Color color)
        {
            var rect = RectangleF.FromLTRB(left, bottomY - WaferThickness, right, bottomY);
            using (var path = EquipmentCanvasStyler.CreateRoundedRectangle(rect, WaferThickness / 2f))
            using (var fill = new SolidBrush(color))
            using (var pen = new Pen(WaferEdge, 0.8f))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }
        }

        private static void DrawVacuumDot(Graphics g, PointF center)
        {
            const float r = 3.5f;
            using (var fill = new SolidBrush(UiTheme.Success))
            using (var edge = new Pen(Color.White, 1.2f))
            {
                g.FillEllipse(fill, center.X - r, center.Y - r, r * 2, r * 2);
                g.DrawEllipse(edge, center.X - r, center.Y - r, r * 2, r * 2);
            }
        }

        private static void DrawCenteredNote(Graphics g, Rectangle rect, string text)
        {
            using (var font = new Font("Segoe UI", 8F))
            {
                TextRenderer.DrawText(g, text, font, rect, EquipmentCanvasStyler.CardSubText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
            }
        }

        private static string GetTargetName(EquipmentRegion region)
        {
            switch (region)
            {
                case EquipmentRegion.ChamberA: return "Chamber A";
                case EquipmentRegion.ChamberB: return "Chamber B";
                case EquipmentRegion.ChamberC: return "Chamber C";
                case EquipmentRegion.FoupA: return "FOUP A";
                case EquipmentRegion.FoupB: return "FOUP B";
                default: return "원점";
            }
        }

        #endregion
    }
}
