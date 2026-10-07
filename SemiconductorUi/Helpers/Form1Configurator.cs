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
        /// 중앙 장비 레이아웃을 구성합니다.
        /// </summary>
        public void LayoutCentralEquipment()
        {
            if (form.panelEquipmentCanvas == null)
            {
                return;
            }

            var canvasSize = form.panelEquipmentCanvas.ClientSize;
            if (canvasSize.Width <= 0 || canvasSize.Height <= 0)
            {
                return;
            }

            // 배치: Chamber B는 위, A/C는 좌우, FOUP A/B는 아래 양쪽. TM은 그 사이 중앙.
            const int margin = 14;
            const int tmClearance = 70; // TM 중심에서 모듈 가장자리까지 최소 거리 (팔각형 이송 챔버 + 여유)
            int chamberW = EquipmentCanvasStyler.ChamberWidth;
            int chamberH = EquipmentCanvasStyler.ChamberHeight;
            int foupW = EquipmentCanvasStyler.FoupWidth;
            int foupH = EquipmentCanvasStyler.FoupHeight;

            float cx = canvasSize.Width / 2f;
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

            PlaceCentered(form.panelChamberB, new PointF(cx, topY + chamberH / 2f));
            PlaceCentered(form.panelChamberA, new PointF(cx - sideDistance, cy));
            PlaceCentered(form.panelChamberC, new PointF(cx + sideDistance, cy));
            PlaceCentered(form.panelFoupA, new PointF(cx - foupOffset, foupY + foupH / 2f));
            PlaceCentered(form.panelFoupB, new PointF(cx + foupOffset, foupY + foupH / 2f));

            if (form.panelMainLamp != null)
            {
                form.panelMainLamp.Location = new Point(margin, margin);
                form.panelMainLamp.BringToFront();
            }

            if (form.tmVisualizationControl != null)
            {
                form.tmVisualizationControl.Location = Point.Empty;
                form.tmVisualizationControl.Size = canvasSize;
                form.tmVisualizationControl.SendToBack();

                var anchors = new System.Collections.Generic.Dictionary<EquipmentRegion, PointF>();
                AddWellAnchor(anchors, EquipmentRegion.ChamberA, form.panelChamberA);
                AddWellAnchor(anchors, EquipmentRegion.ChamberB, form.panelChamberB);
                AddWellAnchor(anchors, EquipmentRegion.ChamberC, form.panelChamberC);
                AddCenterAnchor(anchors, EquipmentRegion.FoupA, form.panelFoupA);
                AddCenterAnchor(anchors, EquipmentRegion.FoupB, form.panelFoupB);
                form.tmVisualizationControl.SetLayout(new PointF(cx, cy), anchors);
            }

            form.panelEquipmentCanvas.Invalidate();
        }

        private void PlaceCentered(Control control, PointF center)
        {
            if (control == null)
            {
                return;
            }

            var desired = new Point(
                (int)Math.Round(center.X - control.Width / 2f),
                (int)Math.Round(center.Y - control.Height / 2f));
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

