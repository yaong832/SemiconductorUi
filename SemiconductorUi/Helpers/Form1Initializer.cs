using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.ViewModels;
using SemiconductorUi.Controllers;
using SemiconductorUi.Services;
using SemiconductorUi.Controls;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// Form1의 UI 초기화 메서드들을 담당하는 헬퍼 클래스
    /// </summary>
    public class Form1Initializer
    {
        private readonly Form1 form;

        /// <summary>
        /// Form1Initializer 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="form">초기화할 Form1 인스턴스</param>
        public Form1Initializer(Form1 form)
        {
            this.form = form ?? throw new ArgumentNullException(nameof(form));
        }

        /// <summary>
        /// 커스텀 컨트롤들을 초기화합니다.
        /// </summary>
        public void InitializeCustomControls()
        {
            if (form.panelEquipmentCanvas == null) return;

            form.panelEquipmentCanvas.BackColor = EquipmentCanvasStyler.CanvasBack;

            // DoubleBuffered 설정 (리플렉션 사용)
            var doubleBufferedProperty = typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (doubleBufferedProperty != null)
            {
                doubleBufferedProperty.SetValue(form.panelEquipmentCanvas, true, null);
            }

            // 캔버스 위의 구성 요소는 Form1.Designer.cs에서 panelEquipmentCanvas 안에 놓여 있다 (앞에서 뒤 순서):
            // tmSideViewControl(TM 기준 화면), panelMainLamp(상태 램프), panelChamberA/B/C, panelFoupA/B
            // (안에 foupVisualizationControlA/B), tmVisualizationControl(TM 그림, 맨 뒤).
            // 위치와 크기는 LayoutCentralEquipment가 캔버스 크기에 맞춰 다시 정한다.
            form.LayoutCentralEquipment();

            // FOUP 칸 수는 설정 파일(MaxFoupCapacity) 값을 따른다.
            foreach (var foup in new[] { form.foupVisualizationControlA, form.foupVisualizationControlB })
            {
                if (foup != null)
                {
                    foup.Capacity = AppSettings.MaxFoupCapacity;
                }
            }

            ApplyEquipmentCanvasStyle();
            ApplyControlPanelTheme();
        }

        /// <summary>
        /// 오른쪽 제어 패널 버튼과 그룹 박스를 밝은 톤으로 맞춥니다.
        /// 공정 시작은 주 동작(파랑), 긴급 정지는 위험(빨강), 나머지는 보조 버튼입니다.
        /// </summary>
        private void ApplyControlPanelTheme()
        {
            UiTheme.StylePrimaryButton(form.buttonStart);
            UiTheme.StyleDangerButton(form.buttonStop);
            foreach (var button in new[]
            {
                form.buttonPause, form.buttonResetAlarm, form.buttonResetProcess, form.buttonEquipmentControl,
                form.buttonToggleFoupMount, form.buttonWaferLoading, form.buttonWaferUnloading, form.buttonApplyRecipe
            })
            {
                UiTheme.StyleSecondaryButton(button);
            }

            foreach (var button in new[]
            {
                form.buttonNavOperate, form.buttonNavRecipe, form.buttonNavMaintenance, form.buttonNavConfig,
                form.buttonNavTrend, form.buttonNavReport, form.buttonNavSystem
            })
            {
                UiTheme.StyleSecondaryButton(button);
            }
            form.UpdateTabButtonStates("Main");

            foreach (var groupBox in new[] { form.groupBoxControlButtons, form.groupBoxFoupReady, form.groupBoxRecipe })
            {
                if (groupBox != null)
                {
                    groupBox.ForeColor = UiTheme.TextSecondary;
                }
            }

            foreach (Control control in new Control[]
            {
                form.flowHeaderLogin, form.labelLoginStatus, form.labelEthercatStatus, form.labelServoStatus,
                form.panelHeaderStatusSummary, form.flowHeaderStatus, form.flowAlarmIndicator, form.labelHeaderCurrentTime,
                form.panelMainProcess, form.panelEquipment, form.panelPmStatus,
                form.panelSummaryPMA, form.panelSummaryPMB, form.panelSummaryPMC,
                form.panelFoupStatusA, form.panelFoupStatusB, form.panelControlPanel
            })
            {
                UiTheme.ApplySoftBorder(control);
            }

            if (form.labelControlTitle != null)
            {
                form.labelControlTitle.ForeColor = UiTheme.TextPrimary;
            }
        }

        /// <summary>
        /// 시뮬레이터 캔버스의 챔버/FOUP/상태 램프를 카드 스타일로 정리합니다.
        /// 슬릿 밸브는 각 챔버에서 TM을 향한 변에 표시됩니다.
        /// </summary>
        private void ApplyEquipmentCanvasStyle()
        {
            EquipmentCanvasStyler.ApplyChamberCard(form.panelChamberA, form.labelChamberA, form.panelDoorChamberA, form.panelWaferChamberA, form.panelLampChamberA, AnchorStyles.Right);
            EquipmentCanvasStyler.ApplyChamberCard(form.panelChamberB, form.labelChamberB, form.panelDoorChamberB, form.panelWaferChamberB, form.panelLampChamberB, AnchorStyles.Bottom);
            EquipmentCanvasStyler.ApplyChamberCard(form.panelChamberC, form.labelChamberC, form.panelDoorChamberC, form.panelWaferChamberC, form.panelLampChamberC, AnchorStyles.Left);
            EquipmentCanvasStyler.ApplyFoupHost(form.panelFoupA);
            EquipmentCanvasStyler.ApplyFoupHost(form.panelFoupB);
            EquipmentCanvasStyler.ApplyMainLampStyle(form.panelMainLamp, form.labelMainLamp,
                form.panelMainLampRed, form.panelMainLampYellow, form.panelMainLampGreen,
                form.labelMainLampRed, form.labelMainLampYellow, form.labelMainLampGreen);
        }

        /// <summary>
        /// FOUP 장착 버튼들을 초기화합니다.
        /// </summary>
        public void InitializeFoupMountButtons()
        {
            if (form.IsDesignEnvironment()) return;
            if (form.flowLayoutFoupReadyButtons == null || form.buttonMountFoupA == null || form.buttonMountFoupB == null) return;

            // 겹침 방지: 세로 스택, 랩 금지, 스크롤 허용
            form.flowLayoutFoupReadyButtons.FlowDirection = FlowDirection.TopDown;
            form.flowLayoutFoupReadyButtons.WrapContents = false;
            form.flowLayoutFoupReadyButtons.AutoScroll = true;

            // FOUP A/B 버튼(buttonMountFoupA, buttonMountFoupB)은 Form1.Designer.cs에서 맨 위에 배치되어 있고,
            // 여기서는 모양과 클릭 처리만 연결합니다. 기존 토글(buttonToggleFoupMount)은 숨김 처리됩니다.
            foreach (var button in new[] { form.buttonMountFoupA, form.buttonMountFoupB })
            {
                button.Width = form.flowLayoutFoupReadyButtons.ClientSize.Width;
                button.Height = 40;
                button.Margin = new Padding(0, 0, 0, 10);
                button.FlatStyle = FlatStyle.Flat;
                UiTheme.StyleSecondaryButton(button);
                button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            }
            form.buttonMountFoupA.Click += ButtonMountFoupA_Click;
            form.buttonMountFoupB.Click += ButtonMountFoupB_Click;

            // 패널 리사이즈 시 버튼 폭 동기화
            form.flowLayoutFoupReadyButtons.Resize += (s, e) =>
            {
                var w = form.flowLayoutFoupReadyButtons.ClientSize.Width;
                foreach (Control c in form.flowLayoutFoupReadyButtons.Controls)
                {
                    if (c is Button b)
                    {
                        b.Width = w;
                    }
                }
            };
            form.UpdateFoupPreparationButtons();
        }

        /// <summary>FOUP A 장착/분리 버튼 (buttonMountFoupA).</summary>
        private void ButtonMountFoupA_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn()) return;
            form.IsFoupAMounted = !form.IsFoupAMounted;
            form.IsFoupMounted = form.IsFoupAMounted && form.IsFoupBMounted;
            form.AddLogMessage($"FOUP A {(form.IsFoupAMounted ? "장착" : "분리")}", "INFO");
            form.UpdateFoupPreparationButtons();
            form.UpdateSimulationUi();
        }

        /// <summary>FOUP B 장착/분리 버튼 (buttonMountFoupB).</summary>
        private void ButtonMountFoupB_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn()) return;
            form.IsFoupBMounted = !form.IsFoupBMounted;
            form.IsFoupMounted = form.IsFoupAMounted && form.IsFoupBMounted;
            form.AddLogMessage($"FOUP B {(form.IsFoupBMounted ? "장착" : "분리")}", "INFO");
            form.UpdateFoupPreparationButtons();
            form.UpdateSimulationUi();
        }

        /// <summary>
        /// 웨이퍼 오버레이를 초기화합니다.
        /// </summary>
        public void InitializeWaferOverlays()
        {
            RelocateWaferPanel(form.panelDoorChamberA, form.panelWaferChamberA);
            RelocateWaferPanel(form.panelDoorChamberB, form.panelWaferChamberB);
            RelocateWaferPanel(form.panelDoorChamberC, form.panelWaferChamberC);
        }

        private void RelocateWaferPanel(Panel doorPanel, Panel waferPanel)
        {
            if (doorPanel == null || waferPanel == null)
            {
                return;
            }

            var parent = doorPanel.Parent;
            if (parent == null)
            {
                return;
            }

            var location = new Point(
                doorPanel.Left + (doorPanel.Width - waferPanel.Width) / 2,
                doorPanel.Top + (doorPanel.Height - waferPanel.Height) / 2);

            waferPanel.Parent = parent;
            waferPanel.Location = location;
            waferPanel.BringToFront();
        }

        /// <summary>
        /// FOUP 트랙 패널들을 초기화합니다.
        /// </summary>
        /// <summary>
        /// FOUP 카드 왼쪽의 슬롯 5칸을 세로로 배치합니다. 1층이 맨 아래입니다.
        /// 칸 안에 슬롯 번호와 웨이퍼 유무 글자를 넣으므로 줄 높이의 70%를 씁니다.
        /// </summary>
        private static void LayoutFoupTrackPanels(TableLayoutPanel tableLayout, Panel[] trackPanels)
        {
            if (tableLayout.Width <= 0 || tableLayout.Height <= 0 || trackPanels == null) return;

            float rowHeight = tableLayout.Height / 5f;
            float slotHeight = Math.Max(12f, rowHeight * 0.7f);
            float horizontalMargin = tableLayout.Width * 0.05f;
            float slotWidth = tableLayout.Width - horizontalMargin * 2;
            float verticalMargin = (rowHeight - slotHeight) / 2f;

            for (int i = 0; i < 5 && i < trackPanels.Length; i++)
            {
                int rowIndex = 4 - i; // 1층(i=0)이 맨 아래 줄
                var trackPanel = trackPanels[i];
                if (trackPanel == null) continue;

                trackPanel.Dock = DockStyle.None;
                trackPanel.Location = new Point((int)horizontalMargin, (int)(rowIndex * rowHeight + verticalMargin));
                trackPanel.Size = new Size((int)slotWidth, (int)slotHeight);
            }
        }

        public void InitializeFoupTrackPanels()
        {
            // 슬롯 칸은 Form1.Designer.cs에 있다: tableFoupASlots / tableFoupBSlots 안의 panelFoupASlot1~5, panelFoupBSlot1~5.
            // 배열 인덱스와 층 번호: foupATrackPanels[0]=1층(맨 아래, panelFoupASlot1) ... [4]=5층(맨 위, panelFoupASlot5).
            WireFoupSlotPanels(form.tableFoupASlots, form.foupATrackPanels,
                new[] { form.panelFoupASlot1, form.panelFoupASlot2, form.panelFoupASlot3, form.panelFoupASlot4, form.panelFoupASlot5 },
                (panel, g, layer) => form.DrawSingleWaferTrack(panel, g, layer, form.FoupAQueue));
            WireFoupSlotPanels(form.tableFoupBSlots, form.foupBTrackPanels,
                new[] { form.panelFoupBSlot1, form.panelFoupBSlot2, form.panelFoupBSlot3, form.panelFoupBSlot4, form.panelFoupBSlot5 },
                (panel, g, layer) => form.DrawSingleWaferTrack(panel, g, layer, null, form.FoupBCompleted));
        }

        /// <summary>
        /// 슬롯 칸 5개에 그리기와 배치를 연결합니다. 칸 안의 웨이퍼 그림은 DrawSingleWaferTrack이 그립니다.
        /// </summary>
        private static void WireFoupSlotPanels(TableLayoutPanel tableLayout, Panel[] trackPanels, Panel[] slots, Action<Panel, Graphics, int> paint)
        {
            if (tableLayout == null || trackPanels == null || slots == null) return;

            for (int i = 0; i < 5 && i < slots.Length && i < trackPanels.Length; i++)
            {
                var trackPanel = slots[i];
                if (trackPanel == null) continue;

                int layer = i + 1; // 1~5층
                // 갱신 때 지우고 다시 그리는 과정이 보이지 않도록 이중 버퍼로 그립니다.
                EquipmentCanvasStyler.EnableDoubleBuffer(trackPanel);
                trackPanel.Paint += (s, e) => paint(trackPanel, e.Graphics, layer);
                trackPanel.Resize += (s, e) => trackPanel.Invalidate();
                trackPanels[i] = trackPanel;
            }

            // 칸 높이의 70%만 쓰도록 배치 (Layout 때마다 다시 계산)
            tableLayout.Layout += (s, e) => LayoutFoupTrackPanels(tableLayout, trackPanels);
            tableLayout.PerformLayout();
        }

        /// <summary>
        /// PM 환경 테이블들을 초기화합니다.
        /// </summary>
        public void InitializePmEnvironmentTables()
        {
            form.CreatePmEnvironmentTable("PMA");
            form.CreatePmEnvironmentTable("PMB");
            form.CreatePmEnvironmentTable("PMC");
        }

        /// <summary>
        /// PM 상태 패널들을 초기화합니다.
        /// </summary>
        public void InitializePmStatusPanels()
        {
            // pmStatusPanelA/B/C는 Form1.Designer.cs에서 panelSummaryPMA/B/C를 채우고 있다.
            // ConfigureStatusPanels가 카드 안쪽 여백을 주므로 여기서 없애 그림이 카드 전체를 쓰게 하고,
            // 같은 카드에 코드가 만든 환경 값 표(tableEnvPMA 등, 값 보관용)가 있으므로 상태 패널을 맨 앞으로 둔다.
            foreach (var panel in new[] { form.pmStatusPanelA, form.pmStatusPanelB, form.pmStatusPanelC })
            {
                if (panel == null) continue;
                if (panel.Parent != null)
                {
                    panel.Parent.Padding = new Padding(0);
                }
                panel.BringToFront();
            }
        }

        /// <summary>
        /// 환경 텔레메트리를 초기화합니다.
        /// </summary>
        public void InitializeEnvironmentTelemetry()
        {
            // 공정 시작 시에만 호출: PV 값을 SV 주변에서 시작하도록 초기화
            // (이후 UpdateLiveEnvironment에서 실제 값으로 업데이트됨)
            foreach (var kvp in form.ChamberEnvSpecs)
            {
                // 공정 시작 시 PV 값을 SV 주변에서 시작하도록 초기화
                form.ChamberEnvLive[kvp.Key] = new ChamberController.ChamberEnvironmentLive
                {
                    TemperatureC = kvp.Value.TargetTemperatureC,
                    PressureTorr = kvp.Value.TargetPressureTorr,
                    HumidityPercent = kvp.Value.TargetHumidityPercent
                };
            }
        }

        /// <summary>
        /// 시뮬레이션 상태를 초기화합니다.
        /// </summary>
        public void InitializeSimulationState()
        {
            form.SimulationService?.Reset();
            form.activeBatchTargetCount = 0;
            // 웨이퍼 로딩 상태가 아닐 때만 큐를 초기화 (로딩 상태에서는 이미 추가된 웨이퍼 유지)
            if (form.WaferLoadState != MainFormViewModel.WaferLoadStateType.Loading)
            {
                form.foupManager?.Reset();
            }
            // FoupManager는 생성자에서 이미 초기화되므로 null 체크 불필요
            form.chamberCompletedCountA = 0;
            form.chamberCompletedCountB = 0;
            form.chamberCompletedCountC = 0;
            
            // ChamberController 초기화
            // 서비스 초기화 (이미 생성되어 있으면 재사용)
            if (form.ChamberService == null)
            {
                form.ChamberService = new ChamberController(Form1.ChamberProcessFlow);
                ServiceContainer.Register<IChamberService>(form.ChamberService);
            }
            if (form.TransferService == null)
            {
                form.TransferService = new TransferController();
                ServiceContainer.Register<ITransferService>(form.TransferService);
            }
            if (form.SimulationService == null)
            {
                form.SimulationService = new SimulationController(form.timerEventHandlers.SimulationTimer_Tick);
                ServiceContainer.Register<ISimulationService>(form.SimulationService);
            }
            if (form.LoggerService == null)
            {
                form.LoggerService = new LoggerService();
                ServiceContainer.Register<ILoggerService>(form.LoggerService);
            }
            
            form.ChamberService.ResetChamberStates();
            form.TmVisualTarget = EquipmentRegion.TM;
            form.TmCarryingVisual = false;
            
            // TransferController 초기화
            form.TransferService.ClearQueue();
            form.TmCurrentPosition = EquipmentRegion.TM;
            form.TmBladeExtensionFactor = 0.55f;
            form.UpdateTmAnimationIdleTarget();
            form.ApplyConfiguredFoupCountsFromState();
            form.ResetDoorStates();
            // 초기화 시에는 PV 값을 설정하지 않음 (공정 시작 시에만 초기화)
            // InitializeEnvironmentTelemetry()는 공정 시작 시에만 호출됨
        }
    }
}

