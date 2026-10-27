using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.ViewModels;
using SemiconductorUi.Helpers;
using SemiconductorUi.Controls;
using AppSettings = SemiconductorUi.AppSettings;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// Form1의 UI 설정 및 구성 메서드들을 담당하는 헬퍼 클래스
    /// </summary>
    public class Form1Configurator
    {
        private readonly Form1 form;

        /// <summary>
        /// Form1Configurator 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="form">설정할 Form1 인스턴스</param>
        public Form1Configurator(Form1 form)
        {
            this.form = form ?? throw new ArgumentNullException(nameof(form));
        }

        /// <summary>
        /// 장비 그림을 원래 크기로 그리는 캔버스 크기. 캔버스가 이보다 크면 그만큼 그림을 키운다.
        /// 1920x1080 화면의 최대화 창에서 캔버스가 대략 이 크기다.
        /// </summary>
        private const float DesignCanvasWidth = 1000f;
        private const float DesignCanvasHeight = 560f;
        private const float MaxCanvasScale = 1.8f;

        // 챔버 카드 안 웨이퍼 표시의 원래 위치와 크기. 확대할 때 기준으로 쓴다.
        private readonly System.Collections.Generic.Dictionary<Control, Rectangle> designChildBounds =
            new System.Collections.Generic.Dictionary<Control, Rectangle>();

        /// <summary>
        /// 캔버스 크기에 맞춘 장비 그림 배율. 원래 크기보다 작게는 그리지 않는다.
        /// </summary>
        internal static float GetCanvasScale(Size canvasSize)
        {
            float k = Math.Min(canvasSize.Width / DesignCanvasWidth, canvasSize.Height / DesignCanvasHeight);
            k = Math.Max(1f, Math.Min(MaxCanvasScale, k));
            // 창 크기를 조금 바꿀 때마다 그림이 흔들리지 않게 0.05 단위로 맞춘다.
            return (float)Math.Floor(k * 20f) / 20f;
        }

        /// <summary>
        /// 중앙 장비 레이아웃을 구성합니다.
        /// 배치는 확대 전 좌표(캔버스 크기 / 배율)로 계산하고, 컨트롤 위치와 크기에 배율을 곱합니다.
        /// </summary>
        public void LayoutCentralEquipment()
        {
            if (form.panelEquipmentCanvas == null)
            {
                return;
            }

            var pixelSize = form.panelEquipmentCanvas.ClientSize;
            if (pixelSize.Width <= 0 || pixelSize.Height <= 0)
            {
                return;
            }

            float k = GetCanvasScale(pixelSize);
            EquipmentCanvasStyler.DrawScale = k;
            var canvasSize = new Size((int)(pixelSize.Width / k), (int)(pixelSize.Height / k));

            // 배치: Chamber B는 위, A/C는 좌우, FOUP A/B는 아래 양쪽. TM은 그 사이 중앙.
            const int margin = 14;
            const int tmClearance = 70; // TM 중심에서 모듈 가장자리까지 최소 거리 (로봇 몸체 + 회전 범위 고리 + 여유)
            int chamberW = EquipmentCanvasStyler.ChamberWidth;
            int chamberH = EquipmentCanvasStyler.ChamberHeight;
            int foupW = EquipmentCanvasStyler.FoupWidth;
            int foupH = EquipmentCanvasStyler.FoupHeight;

            // 오른쪽에는 TM 기준 화면(정면/옆면) 칸을 둔다. 캔버스가 좁으면 숨기고 전체 폭을 시뮬레이터에 쓴다.
            const int sideViewWidth = 262;
            const int sideViewGap = 12;
            const int minSideViewCanvasWidth = 880;
            bool showSideView = form.tmSideViewControl != null && canvasSize.Width >= minSideViewCanvasWidth;
            int clusterWidth = showSideView ? canvasSize.Width - sideViewWidth - sideViewGap - margin : canvasSize.Width;

            float cx = clusterWidth / 2f;
            int topY = margin;
            int foupY = Math.Max(topY + chamberH + 2 * tmClearance, canvasSize.Height - margin - foupH);
            float cy = (topY + chamberH + foupY) / 2f;

            // 좌우 챔버: 화면 폭이 허락하는 만큼 벌리되, 아래 FOUP과 겹치지 않게 한다.
            float sideDistance = Math.Max(tmClearance + chamberW / 2f, (cy - topY) * 1.25f);
            float maxSideDistance = cx - margin - chamberW / 2f;
            float foupOffset = Math.Max(foupW / 2f + 14f, Math.Min(sideDistance * 0.62f, foupW / 2f + 60f));
            bool sideOverlapsFoupRows = cy + chamberH / 2f + 10f > foupY;
            if (sideOverlapsFoupRows)
            {
                sideDistance = Math.Max(sideDistance, foupOffset + foupW / 2f + 16f + chamberW / 2f);
            }
            sideDistance = Math.Min(sideDistance, Math.Max(tmClearance + chamberW / 2f, maxSideDistance));

            var chamberSize = new Size(chamberW, chamberH);
            var foupSize = new Size(foupW, foupH);
            PlaceCentered(form.panelChamberB, new PointF(cx, topY + chamberH / 2f), chamberSize, k);
            PlaceCentered(form.panelChamberA, new PointF(cx - sideDistance, cy), chamberSize, k);
            PlaceCentered(form.panelChamberC, new PointF(cx + sideDistance, cy), chamberSize, k);
            PlaceCentered(form.panelFoupA, new PointF(cx - foupOffset, foupY + foupH / 2f), foupSize, k);
            PlaceCentered(form.panelFoupB, new PointF(cx + foupOffset, foupY + foupH / 2f), foupSize, k);
            foreach (var chamber in new Control[] { form.panelChamberA, form.panelChamberB, form.panelChamberC })
            {
                ScaleChildren(chamber, k);
                chamber?.Invalidate();
            }
            var tmCenter = new PointF(cx * k, cy * k);
            foreach (var foup in new[] { form.foupVisualizationControlA, form.foupVisualizationControlB })
            {
                if (foup != null)
                {
                    foup.DrawScale = k;
                }
            }
            FaceFoupToward(form.foupVisualizationControlA, form.panelFoupA, tmCenter);
            FaceFoupToward(form.foupVisualizationControlB, form.panelFoupB, tmCenter);

            int sideViewLeft = canvasSize.Width - margin - sideViewWidth;
            const int lampWidth = 262;
            const int lampHeight = 34;
            if (form.panelMainLamp != null)
            {
                // TM 기준 화면이 있으면 상태 램프 범례는 그 칸 위로 옮긴다 (왼쪽 위는 Chamber B와 가까워짐).
                var lampLocation = showSideView ? new Point(sideViewLeft, margin) : new Point(margin, margin);
                form.panelMainLamp.Bounds = ScaleRect(new Rectangle(lampLocation, new Size(lampWidth, lampHeight)), k);
                form.panelMainLamp.Invalidate();
                form.panelMainLamp.BringToFront();
            }

            if (form.tmSideViewControl != null)
            {
                form.tmSideViewControl.Visible = showSideView;
                if (showSideView)
                {
                    int sideTop = margin + lampHeight + 8;
                    form.tmSideViewControl.DrawScale = k;
                    form.tmSideViewControl.Bounds = ScaleRect(
                        new Rectangle(sideViewLeft, sideTop, sideViewWidth, Math.Max(120, canvasSize.Height - margin - sideTop)), k);
                    form.tmSideViewControl.BringToFront();
                }
            }

            if (form.tmVisualizationControl != null)
            {
                form.tmVisualizationControl.Location = Point.Empty;
                form.tmVisualizationControl.Size = pixelSize;
                form.tmVisualizationControl.DrawScale = k;
                form.tmVisualizationControl.SendToBack();

                var anchors = new System.Collections.Generic.Dictionary<EquipmentRegion, PointF>();
                AddWellAnchor(anchors, EquipmentRegion.ChamberA, form.panelChamberA);
                AddWellAnchor(anchors, EquipmentRegion.ChamberB, form.panelChamberB);
                AddWellAnchor(anchors, EquipmentRegion.ChamberC, form.panelChamberC);
                AddCenterAnchor(anchors, EquipmentRegion.FoupA, form.panelFoupA);
                AddCenterAnchor(anchors, EquipmentRegion.FoupB, form.panelFoupB);
                var modules = new System.Collections.Generic.List<RectangleF>();
                foreach (var panel in new Control[] { form.panelChamberA, form.panelChamberB, form.panelChamberC, form.panelFoupA, form.panelFoupB })
                {
                    if (panel != null)
                    {
                        modules.Add(panel.Bounds);
                    }
                }
                form.tmVisualizationControl.SetLayout(tmCenter, anchors, modules);
            }

            form.panelEquipmentCanvas.Invalidate();
        }

        private static Rectangle ScaleRect(Rectangle rect, float k)
        {
            return Rectangle.Round(new RectangleF(rect.X * k, rect.Y * k, rect.Width * k, rect.Height * k));
        }

        /// <summary>
        /// 챔버 카드 안의 웨이퍼 표시를 카드와 같은 배율로 옮기고 키운다.
        /// </summary>
        private void ScaleChildren(Control parent, float k)
        {
            if (parent == null)
            {
                return;
            }

            foreach (Control child in parent.Controls)
            {
                if (!designChildBounds.TryGetValue(child, out var design))
                {
                    design = child.Bounds;
                    designChildBounds[child] = design;
                }
                child.Bounds = ScaleRect(design, k);
            }
        }

        /// <summary>
        /// 위에서 본 FOUP의 문이 TM 중심을 향하도록 돌린다.
        /// </summary>
        private static void FaceFoupToward(FoupVisualizationControl foup, Control host, PointF tmCenter)
        {
            if (foup == null || host == null)
            {
                return;
            }

            float hx = host.Left + host.Width / 2f;
            float hy = host.Top + host.Height / 2f;
            foup.FacingDegrees = (float)(Math.Atan2(tmCenter.Y - hy, tmCenter.X - hx) * 180.0 / Math.PI);
        }

        /// <summary>
        /// 확대 전 좌표의 중심과 크기로 컨트롤을 놓는다. 위치와 크기에 배율을 곱한다.
        /// </summary>
        private void PlaceCentered(Control control, PointF center, Size logicalSize, float k)
        {
            if (control == null)
            {
                return;
            }

            control.Size = new Size((int)Math.Round(logicalSize.Width * k), (int)Math.Round(logicalSize.Height * k));
            var desired = new Point(
                (int)Math.Round(center.X * k - control.Width / 2f),
                (int)Math.Round(center.Y * k - control.Height / 2f));
            control.Location = ClampToCanvas(desired, control);
        }

        private static void AddWellAnchor(System.Collections.Generic.IDictionary<EquipmentRegion, PointF> anchors, EquipmentRegion region, Control chamber)
        {
            if (chamber != null)
            {
                anchors[region] = EquipmentCanvasStyler.GetWellCenter(chamber);
            }
        }

        private static void AddCenterAnchor(System.Collections.Generic.IDictionary<EquipmentRegion, PointF> anchors, EquipmentRegion region, Control control)
        {
            if (control != null)
            {
                anchors[region] = new PointF(control.Left + control.Width / 2f, control.Top + control.Height / 2f);
            }
        }

        private Point ClampToCanvas(Point desired, Control control)
        {
            if (form.panelEquipmentCanvas == null || control == null)
            {
                return desired;
            }

            int maxX = form.panelEquipmentCanvas.ClientSize.Width - control.Width;
            int maxY = form.panelEquipmentCanvas.ClientSize.Height - control.Height;

            return new Point(
                Math.Max(0, Math.Min(desired.X, maxX)),
                Math.Max(0, Math.Min(desired.Y, maxY)));
        }

        /// <summary>
        /// 상태 패널들을 구성합니다.
        /// </summary>
        public void ConfigureStatusPanels()
        {
            form.labelPmStatusTitle.Text = "장비 상태 상세";

            if (form.panelSummaryTM != null)
            {
                form.panelSummaryTM.Visible = false;
            }

            var summaryPanels = new[] { form.panelSummaryPMA, form.panelSummaryPMB, form.panelSummaryPMC };

            foreach (var panel in summaryPanels)
            {
                MoveSummaryPanel(panel);
            }

            form.tableLayoutPmStatus.Controls.Clear();
            form.tableLayoutPmStatus.RowStyles.Clear();
            form.tableLayoutPmStatus.RowCount = summaryPanels.Length;

            for (int i = 0; i < summaryPanels.Length; i++)
            {
                form.tableLayoutPmStatus.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / summaryPanels.Length));
                summaryPanels[i].Margin = i == summaryPanels.Length - 1 ? new Padding(0) : new Padding(0, 0, 0, 12);
                form.tableLayoutPmStatus.Controls.Add(summaryPanels[i], 0, i);
            }
        }

        private void MoveSummaryPanel(Panel panel)
        {
            if (panel.Parent != null)
            {
                panel.Parent.Controls.Remove(panel);
            }
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(12, 10, 12, 10);
        }

        /// <summary>
        /// 상태에서 구성된 FOUP 카운트를 적용합니다.
        /// </summary>
        public void ApplyConfiguredFoupCountsFromState()
        {
            switch (form.WaferLoadState)
            {
                case MainFormViewModel.WaferLoadStateType.Loading:
                    // 사용자 설정 로딩 수를 적용 (기본값 5장)
                    form.ConfiguredFoupALoadCount = Math.Min(form.UserWaferLoadCount, AppSettings.MaxFoupCapacity);
                    // 시뮬레이션 표시용 실제 잔량도 초기화
                    form.FoupARemainingInventoryCount = form.ConfiguredFoupALoadCount;
                    break;
                case MainFormViewModel.WaferLoadStateType.Unloading:
                case MainFormViewModel.WaferLoadStateType.None:
                default:
                    form.ConfiguredFoupALoadCount = 0;
                    form.FoupARemainingInventoryCount = 0;
                    break;
            }
        }

        /// <summary>
        /// FOUP 기본 시각적 요소를 캡처합니다.
        /// </summary>
        public void CaptureFoupBaseVisuals()
        {
            CacheLabelBaseText(form.labelFoupInfoATitle);
            CacheLabelBaseText(form.labelFoupInfoBTitle);
            CacheLabelBaseText(form.labelSummaryFoupATitle);
            CacheLabelBaseText(form.labelSummaryFoupBTitle);

            CachePanelBaseColor(form.panelFoupStatusA);
            CachePanelBaseColor(form.panelFoupStatusB);
            CachePanelBaseColor(form.panelSummaryFoupA);
            CachePanelBaseColor(form.panelSummaryFoupB);
        }

        private void CacheLabelBaseText(Label label)
        {
            if (label != null && label.Tag == null)
            {
                label.Tag = label.Text;
            }
        }

        private void CachePanelBaseColor(Panel panel)
        {
            if (panel != null && !form.originalPanelColors.ContainsKey(panel))
            {
                form.originalPanelColors[panel] = panel.BackColor;
            }
        }

        /// <summary>
        /// 도어 상태를 리셋합니다.
        /// </summary>
        public void ResetDoorStates()
        {
            form.ViewModel?.ResetDoorStates();
            RefreshAllDoorVisuals();
            if (form.uiUpdater != null)
            {
                form.uiUpdater.UpdateChamberWaferIndicators();
            }
        }

        /// <summary>
        /// 모든 도어 시각적 요소를 새로고침합니다.
        /// </summary>
        public void RefreshAllDoorVisuals()
        {
            form.ApplyDoorVisualsForRegion(EquipmentRegion.ChamberA, animate: false);
            form.ApplyDoorVisualsForRegion(EquipmentRegion.ChamberB, animate: false);
            form.ApplyDoorVisualsForRegion(EquipmentRegion.ChamberC, animate: false);
        }
    }
}

