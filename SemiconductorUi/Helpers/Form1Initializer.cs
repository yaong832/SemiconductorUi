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

            // DoubleBuffered 설정 (리플렉션 사용)
            var doubleBufferedProperty = typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (doubleBufferedProperty != null)
            {
                doubleBufferedProperty.SetValue(form.panelEquipmentCanvas, true, null);
            }

            // TM Visualization Control 초기화 (별도 컨트롤로 분리하여 부드러운 애니메이션)
            if (form.tmVisualizationControl == null)
            {
                form.tmVisualizationControl = new TmVisualizationControl();
                form.tmVisualizationControl.BackColor = Color.Transparent; // 투명 배경
                form.tmVisualizationControl.Location = new Point(0, 0);
                form.tmVisualizationControl.Name = "tmVisualizationControl";
                // 블레이드가 잘리지 않도록 캔버스보다 크게 설정 (충분한 여유 공간)
                var canvasSize = form.panelEquipmentCanvas.ClientSize;
                int tmControlSize = Math.Max(canvasSize.Width, canvasSize.Height) + 300; // 캔버스보다 300px 더 크게
                form.tmVisualizationControl.Size = new Size(tmControlSize, tmControlSize);
                form.tmVisualizationControl.TabIndex = 0;
                form.panelEquipmentCanvas.Controls.Add(form.tmVisualizationControl);
                form.tmVisualizationControl.SendToBack(); // FOUP/Chamber 뒤로 보내기 (가리지 않도록)
            }
            
            // 초기 레이아웃 설정
            form.LayoutCentralEquipment();

            // FOUP A Visualization Control 초기화
            if (form.foupVisualizationControlA == null && form.panelFoupA != null)
            {
                form.foupVisualizationControlA = new FoupVisualizationControl();
                form.foupVisualizationControlA.Dock = DockStyle.Fill;
                form.foupVisualizationControlA.Location = new Point(12, 12);
                form.foupVisualizationControlA.Name = "foupVisualizationControlA";
                form.foupVisualizationControlA.Size = new Size(176, 86);
                form.foupVisualizationControlA.TabIndex = 0;
                form.foupVisualizationControlA.Title = "FOUP A";
                form.panelFoupA.Controls.Add(form.foupVisualizationControlA);
            }

            // FOUP B Visualization Control 초기화
            if (form.foupVisualizationControlB == null && form.panelFoupB != null)
            {
                form.foupVisualizationControlB = new FoupVisualizationControl();
                form.foupVisualizationControlB.Dock = DockStyle.Fill;
                form.foupVisualizationControlB.Location = new Point(12, 12);
                form.foupVisualizationControlB.Name = "foupVisualizationControlB";
                form.foupVisualizationControlB.Size = new Size(176, 86);
                form.foupVisualizationControlB.TabIndex = 0;
                form.foupVisualizationControlB.Title = "FOUP B";
                form.panelFoupB.Controls.Add(form.foupVisualizationControlB);
            }

            // Chamber와 FOUP 패널들을 panelEquipmentCanvas에 추가
            if (form.panelChamberA != null && form.panelChamberA.Parent != form.panelEquipmentCanvas)
            {
                if (form.panelChamberA.Parent != null)
                {
                    form.panelChamberA.Parent.Controls.Remove(form.panelChamberA);
                }
                form.panelEquipmentCanvas.Controls.Add(form.panelChamberA);
            }

            if (form.panelChamberB != null && form.panelChamberB.Parent != form.panelEquipmentCanvas)
            {
                if (form.panelChamberB.Parent != null)
                {
                    form.panelChamberB.Parent.Controls.Remove(form.panelChamberB);
                }
                form.panelEquipmentCanvas.Controls.Add(form.panelChamberB);
            }

            if (form.panelChamberC != null && form.panelChamberC.Parent != form.panelEquipmentCanvas)
            {
                if (form.panelChamberC.Parent != null)
                {
                    form.panelChamberC.Parent.Controls.Remove(form.panelChamberC);
                }
                form.panelEquipmentCanvas.Controls.Add(form.panelChamberC);
            }

            if (form.panelFoupA != null && form.panelFoupA.Parent != form.panelEquipmentCanvas)
            {
                if (form.panelFoupA.Parent != null)
                {
                    form.panelFoupA.Parent.Controls.Remove(form.panelFoupA);
                }
                form.panelEquipmentCanvas.Controls.Add(form.panelFoupA);
            }

            if (form.panelFoupB != null && form.panelFoupB.Parent != form.panelEquipmentCanvas)
            {
                if (form.panelFoupB.Parent != null)
                {
                    form.panelFoupB.Parent.Controls.Remove(form.panelFoupB);
                }
                form.panelEquipmentCanvas.Controls.Add(form.panelFoupB);
            }

            // Main Lamp 패널을 panelEquipmentCanvas에 추가
            if (form.panelMainLamp != null && form.panelMainLamp.Parent != form.panelEquipmentCanvas)
            {
                if (form.panelMainLamp.Parent != null)
                {
                    form.panelMainLamp.Parent.Controls.Remove(form.panelMainLamp);
                }
                // Main Lamp는 우측 상단에 고정 위치
                form.panelMainLamp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                form.panelEquipmentCanvas.Controls.Add(form.panelMainLamp);
                form.panelMainLamp.BringToFront();
            }
        }

        /// <summary>
        /// FOUP 장착 버튼들을 초기화합니다.
        /// </summary>
        public void InitializeFoupMountButtons()
        {
            if (form.IsDesignEnvironment()) return;
            if (form.flowLayoutFoupReadyButtons == null) return;

            // 겹침 방지: 세로 스택, 랩 금지, 스크롤 허용
            form.flowLayoutFoupReadyButtons.FlowDirection = FlowDirection.TopDown;
            form.flowLayoutFoupReadyButtons.WrapContents = false;
            form.flowLayoutFoupReadyButtons.AutoScroll = true;

            form.buttonMountFoupA = new Button
            {
                Width = form.flowLayoutFoupReadyButtons.ClientSize.Width,
                Height = 40,
                Margin = new Padding(0, 0, 0, 10),
                FlatStyle = FlatStyle.Flat,
            };
            form.buttonMountFoupA.FlatAppearance.BorderSize = 0;
            form.buttonMountFoupA.BackColor = Color.FromArgb(200, 220, 240);  // 밝은 파란색 계열
            form.buttonMountFoupA.ForeColor = Color.FromArgb(40, 40, 40);  // 어두운 회색 텍스트
            form.buttonMountFoupA.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            form.buttonMountFoupA.Click += (s, e) =>
            {
                if (!form.EnsureLoggedIn()) return;
                form.IsFoupAMounted = !form.IsFoupAMounted;
                form.IsFoupMounted = form.IsFoupAMounted && form.IsFoupBMounted;
                form.AddLogMessage($"FOUP A {(form.IsFoupAMounted ? "장착" : "분리")}", "INFO");
                form.UpdateFoupPreparationButtons();
                form.UpdateSimulationUi();
            };

            form.buttonMountFoupB = new Button
            {
                Width = form.flowLayoutFoupReadyButtons.ClientSize.Width,
                Height = 40,
                Margin = new Padding(0, 0, 0, 10),
                FlatStyle = FlatStyle.Flat,
            };
            form.buttonMountFoupB.FlatAppearance.BorderSize = 0;
            form.buttonMountFoupB.BackColor = Color.FromArgb(200, 220, 240);  // 밝은 파란색 계열
            form.buttonMountFoupB.ForeColor = Color.FromArgb(40, 40, 40);  // 어두운 회색 텍스트
            form.buttonMountFoupB.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            form.buttonMountFoupB.Click += (s, e) =>
            {
                if (!form.EnsureLoggedIn()) return;
                form.IsFoupBMounted = !form.IsFoupBMounted;
                form.IsFoupMounted = form.IsFoupAMounted && form.IsFoupBMounted;
                form.AddLogMessage($"FOUP B {(form.IsFoupBMounted ? "장착" : "분리")}", "INFO");
                form.UpdateFoupPreparationButtons();
                form.UpdateSimulationUi();
            };

            // 상단에 A/B 버튼을 추가하고 기존 토글은 숨김 처리
            form.flowLayoutFoupReadyButtons.Controls.Add(form.buttonMountFoupA);
            form.flowLayoutFoupReadyButtons.Controls.Add(form.buttonMountFoupB);
            // A/B 버튼을 최상단으로 정렬
            form.flowLayoutFoupReadyButtons.Controls.SetChildIndex(form.buttonMountFoupB, 0);
            form.flowLayoutFoupReadyButtons.Controls.SetChildIndex(form.buttonMountFoupA, 0);

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
        public void InitializeFoupTrackPanels()
        {
            // FOUP A 트랙 패널 생성 (1~5층)
            if (form.panelFoupALevelContainer != null)
            {
                form.panelFoupALevelContainer.Controls.Clear(); // 기존 컨트롤 제거
                
                // TableLayoutPanel 생성하여 5개 패널을 세로로 배치
                var tableLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 5,
                    Padding = new Padding(0)
                };
                
                // 각 행의 높이를 동일하게 설정
                for (int i = 0; i < 5; i++)
                {
                    tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
                }
                
                // 1층이 맨 아래에 오도록 역순으로 추가 (5층부터 1층까지)
                // 배열 인덱스와 층 번호 매핑: foupATrackPanels[0]=1층, foupATrackPanels[1]=2층, ..., foupATrackPanels[4]=5층
                tableLayout.Layout += (s, e) =>
                {
                    if (tableLayout.Width > 0 && tableLayout.Height > 0)
                    {
                        float rowHeight = tableLayout.Height / 5f;
                        float waferHeight = rowHeight * 0.4f; // 웨이퍼 높이를 40%로 증가 (더 두껍게)
                        float horizontalMargin = tableLayout.Width * 0.05f;
                        float waferWidth = tableLayout.Width - horizontalMargin * 2;
                        float gapBetweenWafers = 2f; // 웨이퍼 사이 간격 (2px)
                        float baseVerticalMargin = (rowHeight - waferHeight) / 2f - 6f; // 더 아래로 이동
                        
                        for (int i = 0; i < 5; i++)
                        {
                            int rowIndex = 4 - i; // 역순 배치 (1층=4, 2층=3, ..., 5층=0)
                            var trackPanel = form.foupATrackPanels[i];
                            if (trackPanel != null)
                            {
                                trackPanel.Dock = DockStyle.None;
                                // 각 층 사이에 간격 추가 (위에서 아래로 갈수록 간격 누적)
                                float accumulatedGap = i * gapBetweenWafers; // i는 0~4, 위에서 아래로 갈수록 증가
                                float yPosition = rowIndex * rowHeight + baseVerticalMargin + accumulatedGap;
                                trackPanel.Location = new Point((int)horizontalMargin, (int)yPosition);
                                trackPanel.Size = new Size((int)waferWidth, (int)waferHeight);
                            }
                        }
                    }
                };
                
                for (int i = 0; i < 5; i++)
                {
                    int layer = i + 1; // 1~5층 (배열 인덱스 i에 해당하는 층 번호)
                    int rowIndex = 4 - i; // 역순 배치: 1층(row=4, 맨 아래), 2층(row=3), ..., 5층(row=0, 맨 위)
                    var trackPanel = new Panel
                    {
                        Dock = DockStyle.Fill,  // 초기에는 Fill, Layout 이벤트에서 조정됨
                        BackColor = Color.FromArgb(245, 245, 250),  // 밝은 회색 배경
                        BorderStyle = BorderStyle.None,  // 테두리는 Paint에서 그리므로 None
                        Margin = new Padding(0)
                    };
                    
                    // 해당 층의 웨이퍼만 표시하도록 Paint 이벤트 연결
                    // layer는 1~5층을 의미하며, 1층부터 순서대로 쌓임
                    int layerIndex = layer; // 클로저를 위한 로컬 변수
                    trackPanel.Paint += (s, e) => form.DrawSingleWaferTrack(trackPanel, e.Graphics, layerIndex, form.FoupAQueue);
                    trackPanel.Resize += (s, e) => trackPanel.Invalidate();
                    
                    // 배열에 저장: foupATrackPanels[0]=1층 패널, foupATrackPanels[1]=2층 패널, ...
                    form.foupATrackPanels[i] = trackPanel;
                    tableLayout.Controls.Add(trackPanel, 0, rowIndex);
                }
                
                form.panelFoupALevelContainer.Controls.Add(tableLayout);
            }
            
            // FOUP B 트랙 패널 생성 (1~5층)
            if (form.panelFoupBLevelContainer != null)
            {
                form.panelFoupBLevelContainer.Controls.Clear(); // 기존 컨트롤 제거
                
                // TableLayoutPanel 생성하여 5개 패널을 세로로 배치
                var tableLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 5,
                    Padding = new Padding(0)
                };
                
                // 각 행의 높이를 동일하게 설정
                for (int i = 0; i < 5; i++)
                {
                    tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
                }
                
                // 1층이 맨 아래에 오도록 역순으로 추가 (5층부터 1층까지)
                // 배열 인덱스와 층 번호 매핑: foupBTrackPanels[0]=1층, foupBTrackPanels[1]=2층, ..., foupBTrackPanels[4]=5층
                tableLayout.Layout += (s, e) =>
                {
                    if (tableLayout.Width > 0 && tableLayout.Height > 0)
                    {
                        float rowHeight = tableLayout.Height / 5f;
                        float waferHeight = rowHeight * 0.4f; // 웨이퍼 높이를 40%로 증가 (더 두껍게)
                        float horizontalMargin = tableLayout.Width * 0.05f;
                        float waferWidth = tableLayout.Width - horizontalMargin * 2;
                        float gapBetweenWafers = 2f; // 웨이퍼 사이 간격 (2px)
                        float verticalMargin = (rowHeight - waferHeight) / 2f - 2f; // 약간 아래로 이동
                        
                        for (int i = 0; i < 5; i++)
                        {
                            int rowIndex = 4 - i; // 역순 배치
                            var trackPanel = form.foupBTrackPanels[i];
                            if (trackPanel != null)
                            {
                                trackPanel.Dock = DockStyle.None;
                                // 각 층 사이에 간격 추가하고 약간 아래로 이동
                                float yPosition = rowIndex * rowHeight + verticalMargin + (4 - rowIndex) * gapBetweenWafers;
                                trackPanel.Location = new Point((int)horizontalMargin, (int)yPosition);
                                trackPanel.Size = new Size((int)waferWidth, (int)waferHeight);
                            }
                        }
                    }
                };
                
                for (int i = 0; i < 5; i++)
                {
                    int layer = i + 1; // 1~5층 (배열 인덱스 i에 해당하는 층 번호)
                    int rowIndex = 4 - i; // 역순 배치: 1층(row=4, 맨 아래), 2층(row=3), ..., 5층(row=0, 맨 위)
                    var trackPanel = new Panel
                    {
                        Dock = DockStyle.Fill,  // 초기에는 Fill, Layout 이벤트에서 조정됨
                        BackColor = Color.FromArgb(245, 245, 250),  // 밝은 회색 배경
                        BorderStyle = BorderStyle.None,  // 테두리는 Paint에서 그리므로 None
                        Margin = new Padding(0)
                    };
                    
                    // 해당 층의 웨이퍼만 표시하도록 Paint 이벤트 연결
                    // layer는 1~5층을 의미하며, 1층부터 순서대로 쌓임
                    int layerIndex = layer; // 클로저를 위한 로컬 변수
                    trackPanel.Paint += (s, e) => form.DrawSingleWaferTrack(trackPanel, e.Graphics, layerIndex, null, form.FoupBCompleted);
                    trackPanel.Resize += (s, e) => trackPanel.Invalidate();
                    
                    // 배열에 저장: foupBTrackPanels[0]=1층 패널, foupBTrackPanels[1]=2층 패널, ...
                    form.foupBTrackPanels[i] = trackPanel;
                    tableLayout.Controls.Add(trackPanel, 0, rowIndex);
                }
                
                form.panelFoupBLevelContainer.Controls.Add(tableLayout);
            }
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
            // PMA 패널
            form.pmStatusPanelA = form.CreatePmStatusPanel("PMA", form.panelSummaryPMA);
            
            // PMB 패널
            form.pmStatusPanelB = form.CreatePmStatusPanel("PMB", form.panelSummaryPMB);
            
            // PMC 패널
            form.pmStatusPanelC = form.CreatePmStatusPanel("PMC", form.panelSummaryPMC);
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

