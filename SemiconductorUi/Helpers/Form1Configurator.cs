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

            // TM 컨트롤 위치 및 크기 설정 (캔버스 중앙, 블레이드가 잘리지 않도록 충분히 크게)
            // TM 본체 크기는 고정 (radius = 40f, 지름 = 80px)
            const int tmBodySize = 80; // TM 본체 실제 크기 (챔버 배치에 사용)
            int tmControlSize = 200; // TM 컨트롤 크기 (기본값)
            
            if (form.tmVisualizationControl != null)
            {
                // 블레이드가 잘리지 않도록 캔버스보다 크게 설정
                tmControlSize = Math.Max(canvasSize.Width, canvasSize.Height) + 300; // 캔버스보다 300px 더 크게
                form.tmVisualizationControl.Size = new Size(tmControlSize, tmControlSize);
                
                // TM 컨트롤을 중앙에 배치 (컨트롤 크기가 캔버스보다 크므로 음수 위치 가능)
                var tmPosition = new Point(
                    (canvasSize.Width - tmControlSize) / 2,
                    (canvasSize.Height - tmControlSize) / 2);
                form.tmVisualizationControl.Location = tmPosition;
                
                // Z-order: TM을 뒤로 보내서 FOUP/Chamber가 위에 표시되도록
                form.tmVisualizationControl.SendToBack();
            }

            // TM 중심 위치
            var tmCenter = new PointF(
                canvasSize.Width / 2f,
                canvasSize.Height / 2f);

            // Chamber 간격 조정 (TM 본체 크기 기준으로 적절한 간격)
            int chamberGap = 120; // 챔버와 TM 본체 사이 간격 (충분한 간격)

            if (form.panelChamberB != null)
            {
                var topPos = new Point(
                    (int)(tmCenter.X - form.panelChamberB.Width / 2f),
                    (int)(tmCenter.Y - tmBodySize / 2f - form.panelChamberB.Height - chamberGap));
                form.panelChamberB.Location = ClampToCanvas(topPos, form.panelChamberB);
            }

            if (form.panelChamberA != null)
            {
                var leftPos = new Point(
                    (int)(tmCenter.X - tmBodySize / 2f - form.panelChamberA.Width - chamberGap),
                    (int)(tmCenter.Y - form.panelChamberA.Height / 2f));
                form.panelChamberA.Location = ClampToCanvas(leftPos, form.panelChamberA);
            }

            if (form.panelChamberC != null)
            {
                var rightPos = new Point(
                    (int)(tmCenter.X + tmBodySize / 2f + chamberGap),
                    (int)(tmCenter.Y - form.panelChamberC.Height / 2f));
                form.panelChamberC.Location = ClampToCanvas(rightPos, form.panelChamberC);
            }

            // FOUP 반경 조정 (TM 본체 크기 기준)
            var radiusBase = tmBodySize * 3.5f; // TM 본체 크기의 3.5배 (더 멀리 배치)
            var radiusLimit = Math.Min(canvasSize.Width, canvasSize.Height) / 2.0f;
            var foupRadius = Math.Min(radiusBase, radiusLimit);

            PositionFoupAtAngle(form.panelFoupA, tmCenter, foupRadius, 225f);
            PositionFoupAtAngle(form.panelFoupB, tmCenter, foupRadius, 315f);
            
            // 캔버스 업데이트 (TM 그리기)
            form.panelEquipmentCanvas.Invalidate();
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

        private void PositionFoupAtAngle(Control foupPanel, PointF center, float radius, float angleDegrees)
        {
            if (foupPanel == null || form.panelEquipmentCanvas == null)
            {
                return;
            }

            double radians = angleDegrees * Math.PI / 180.0;
            var offset = new PointF(
                (float)(Math.Cos(radians) * radius),
                (float)(-Math.Sin(radians) * radius));

            var desiredCenter = new PointF(center.X + offset.X, center.Y + offset.Y);
            var desiredLocation = new Point(
                (int)Math.Round(desiredCenter.X - foupPanel.Width / 2f),
                (int)Math.Round(desiredCenter.Y - foupPanel.Height / 2f));

            foupPanel.Location = ClampToCanvas(desiredLocation, foupPanel);
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

