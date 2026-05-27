using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SemiconductorUi.Controls;
using SemiconductorUi.ViewModels;
using SemiconductorUi.Controllers;
using AppSettings = SemiconductorUi.AppSettings;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// Form1의 UI 업데이트 로직을 담당하는 헬퍼 클래스
    /// Form1.cs의 복잡도를 줄이기 위해 UI 업데이트 메서드들을 분리
    /// </summary>
    public class Form1UiUpdater
    {
        private readonly Form1 form;

        /// <summary>
        /// Form1UiUpdater 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        /// <exception cref="ArgumentNullException">form이 null인 경우</exception>
        public Form1UiUpdater(Form1 form)
        {
            this.form = form ?? throw new ArgumentNullException(nameof(form));
        }

        #region 간단한 UI 업데이트 메서드

        /// <summary>
        /// 헤더 시계 업데이트
        /// </summary>
        public void UpdateHeaderClock()
        {
            if (form.labelHeaderCurrentTime == null)
            {
                return;
            }

            form.labelHeaderCurrentTime.Text = $"현재 시각: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        /// <summary>
        /// 서보 상태 라벨 업데이트
        /// 실제 하드웨어 HOME_D 센서를 확인하여 원점복귀 상태 표시
        /// </summary>
        public void UpdateServoStatusLabel()
        {
            if (form.labelServoStatus == null) return;

            try
            {
                if (!form.EthercatConnected)
                {
                    form.labelServoStatus.Text = "서보: - | Home: -";
                    form.labelServoStatus.ForeColor = Color.Gainsboro;
                    return;
                }

                // 서보 상태 확인: IsServoOn 플래그를 우선 사용 (사용자가 명시적으로 서보 ON을 눌렀을 때만 true)
                // 실제 하드웨어 상태는 보조적으로만 확인 (위치 데이터만으로는 부정확할 수 있음)
                bool isServoOn = form.IsServoOn;  // UI 상태 플래그 우선 사용
                bool axis1Homed = false;
                bool axis2Homed = false;
                bool isHomed = false;
                
                // IsServoOn이 true일 때만 실제 하드웨어 상태 확인
                if (isServoOn && form.EtherCAT_M != null)
                {
                    try
                    {
                        // 실제 하드웨어에서 위치 데이터 확인 (서보가 실제로 ON인지 검증)
                        string axis1Pos = form.EtherCAT_M.Axis1_is_PosData();
                        string axis2Pos = form.EtherCAT_M.Axis2_is_PosData();
                        bool hasPosition = !string.IsNullOrEmpty(axis1Pos) && axis1Pos != "-" &&
                                          !string.IsNullOrEmpty(axis2Pos) && axis2Pos != "-";
                        
                        // 위치 데이터가 없으면 서보가 실제로 OFF 상태일 수 있음
                        if (!hasPosition)
                        {
                            // 하드웨어는 OFF인데 UI 상태가 ON인 경우 동기화
                            isServoOn = false;
                            form.IsServoOn = false;
                            form.TmHardwareInitialized = false;
                            form.AddLogMessage("서보 상태 확인: 하드웨어는 OFF 상태입니다", "WARN");
                        }
                        else
                        {
                            // 서보 ON 상태이면 HOME_D 센서 확인
                            axis1Homed = form.EtherCAT_M.Axis1_Status("HOME_D");
                            axis2Homed = form.EtherCAT_M.Axis2_Status("HOME_D");
                            isHomed = axis1Homed && axis2Homed;
                        }
                    }
                    catch (Exception ex)
                    {
                        // 하드웨어 읽기 실패 시 UI 상태 플래그 유지
                        form.AddLogMessage($"서보 상태 확인 오류: {ex.Message}", "WARN");
                    }
                }
                else if (!isServoOn && form.EtherCAT_M != null)
                {
                    // IsServoOn이 false일 때는 하드웨어 확인 없이 OFF로 표시
                    // (위치 데이터가 읽혀도 서보 ON 버튼을 누르지 않았으면 OFF로 표시)
                }
                
                // 상태 표시
                string statusText;
                if (isServoOn && isHomed)
                {
                    statusText = "서보: ON | Home: 완료";
                    form.labelServoStatus.ForeColor = Color.LimeGreen;
                }
                else if (isServoOn)
                {
                    // 원점복귀 미완료 상태 상세 표시
                    if (!axis1Homed && !axis2Homed)
                    {
                        statusText = "서보: ON | Home: 미완료 (Axis1,2)";
                    }
                    else if (!axis1Homed)
                    {
                        statusText = "서보: ON | Home: 미완료 (Axis1)";
                    }
                    else if (!axis2Homed)
                    {
                        statusText = "서보: ON | Home: 미완료 (Axis2)";
                    }
                    else
                    {
                        statusText = "서보: ON | Home: 미완료";
                    }
                    form.labelServoStatus.ForeColor = Color.Orange;
                }
                else
                {
                    statusText = "서보: OFF | Home: -";
                    form.labelServoStatus.ForeColor = Color.Gray;
                }
                
                form.labelServoStatus.Text = statusText;
            }
            catch (Exception ex)
            {
                form.labelServoStatus.Text = "서보: 오류";
                form.labelServoStatus.ForeColor = Color.Red;
                form.AddLogMessage($"서보 상태 확인 오류: {ex.Message}", "WARN");
            }
        }

        /// <summary>
        /// 탭 버튼 상태 업데이트
        /// </summary>
        /// <param name="selectedTab">선택된 탭 이름</param>
        public void UpdateTabButtonStates(string selectedTab)
        {
            // 기본 색상 (비선택)
            Color inactiveColor = Color.FromArgb(100, 120, 130);
            // 선택된 색상 (더 밝게)
            Color activeColor = Color.FromArgb(130, 150, 160);

            if (form.buttonTabMain != null)
            {
                form.buttonTabMain.BackColor = selectedTab == "Main" ? activeColor : inactiveColor;
            }
            if (form.buttonTabVerification != null)
            {
                form.buttonTabVerification.BackColor = selectedTab == "Verification" ? activeColor : inactiveColor;
            }
            if (form.buttonTabTransfer != null)
            {
                form.buttonTabTransfer.BackColor = selectedTab == "Transfer" ? activeColor : inactiveColor;
            }
        }

        /// <summary>
        /// 하단 네비게이션 버튼 상태 업데이트 (선택된 버튼은 더 밝게)
        /// </summary>
        /// <param name="selectedNav">선택된 네비게이션 이름</param>
        public void UpdateNavButtonStates(string selectedNav)
        {
            // 기본 색상 (비선택)
            Color inactiveColor = Color.FromArgb(100, 120, 130);
            // 선택된 색상 (더 밝게)
            Color activeColor = Color.FromArgb(130, 150, 160);

            if (form.buttonNavOperate != null)
            {
                form.buttonNavOperate.BackColor = selectedNav == "Operate" ? activeColor : inactiveColor;
            }
            if (form.buttonNavRecipe != null)
            {
                form.buttonNavRecipe.BackColor = selectedNav == "Recipe" ? activeColor : inactiveColor;
            }
            if (form.buttonNavMaintenance != null)
            {
                form.buttonNavMaintenance.BackColor = selectedNav == "Maintenance" ? activeColor : inactiveColor;
            }
            if (form.buttonNavConfig != null)
            {
                form.buttonNavConfig.BackColor = selectedNav == "Config" ? activeColor : inactiveColor;
            }
            if (form.buttonNavTrend != null)
            {
                form.buttonNavTrend.BackColor = selectedNav == "Trend" ? activeColor : inactiveColor;
            }
            if (form.buttonNavReport != null)
            {
                form.buttonNavReport.BackColor = selectedNav == "Report" ? activeColor : inactiveColor;
            }
            if (form.buttonNavSystem != null)
            {
                form.buttonNavSystem.BackColor = selectedNav == "System" ? activeColor : inactiveColor;
            }
        }

        /// <summary>
        /// 네비게이션 버튼 상태 업데이트 (로그인 상태에 따라 활성화/비활성화)
        /// </summary>
        public void UpdateNavigationButtons()
        {
            bool enabled = form.IsLoggedIn;
            
            // 상단/하단 네비게이션 버튼들 비활성화 (동적으로 찾기)
            var navButtonNames = new[]
            {
                "buttonNavVerification",
                "buttonNavTransfer",
                "buttonNavOperate",
                "buttonNavRecipe",
                "buttonNavMaintenance",
                "buttonNavConfig",
                "buttonNavTrend",
                "buttonNavReport",
                "buttonNavSystem"
            };

            foreach (var buttonName in navButtonNames)
            {
                var found = form.Controls.Find(buttonName, true);
                if (found != null && found.Length > 0)
                {
                    var button = found[0] as Button;
                    if (button != null)
                    {
                        button.Enabled = enabled;
                    }
                }
            }

            // 상단 탭 버튼들도 비활성화 (buttonTabVerification, buttonTabTransfer)
            var tabButtonNames = new[]
            {
                "buttonTabVerification",
                "buttonTabTransfer"
            };

            foreach (var buttonName in tabButtonNames)
            {
                var found = form.Controls.Find(buttonName, true);
                if (found != null && found.Length > 0)
                {
                    var button = found[0] as Button;
                    if (button != null)
                    {
                        button.Enabled = enabled;
                    }
                }
            }

            // 장비 제어 버튼도 로그인 상태에 따라 활성화
            if (form.buttonEquipmentControl != null)
            {
                form.buttonEquipmentControl.Enabled = enabled;
            }
        }

        #endregion

        #region 중간 복잡도 UI 업데이트 메서드

        /// <summary>
        /// FOUP 레벨 채우기 업데이트 (트랙 패널 무효화)
        /// </summary>
        /// <param name="trackPanel">트랙 패널</param>
        /// <param name="fillPanel">채우기 패널 (사용 안 함)</param>
        /// <param name="waferCount">웨이퍼 개수</param>
        public void UpdateFoupLevelFill(Panel trackPanel, Panel fillPanel, int waferCount)
        {
            // 개별 슬롯 표시로 변경되었으므로 Invalidate만 호출
            if (trackPanel != null)
            {
                trackPanel.Invalidate();
            }
        }

        /// <summary>
        /// 헤더 카드 상태 업데이트
        /// </summary>
        /// <param name="tm">TM 상태 텍스트</param>
        /// <param name="pma">PMA 상태 텍스트</param>
        /// <param name="pmb">PMB 상태 텍스트</param>
        /// <param name="pmc">PMC 상태 텍스트</param>
        public void UpdateHeaderCardStatuses(string tm, string pma, string pmb, string pmc)
        {
            if (form.labelHeaderCardTMStatus != null) form.labelHeaderCardTMStatus.Text = tm;
            if (form.labelHeaderCardPMAStatus != null) form.labelHeaderCardPMAStatus.Text = pma;
            if (form.labelHeaderCardPMBStatus != null) form.labelHeaderCardPMBStatus.Text = pmb;
            if (form.labelHeaderCardPMCStatus != null) form.labelHeaderCardPMCStatus.Text = pmc;
            if (form.labelHeaderTMStatus != null) form.labelHeaderTMStatus.Text = tm;
            if (form.labelHeaderPM1Status != null) form.labelHeaderPM1Status.Text = pma;
            if (form.labelHeaderPM2Status != null) form.labelHeaderPM2Status.Text = pmb;
            if (form.labelHeaderPM3Status != null) form.labelHeaderPM3Status.Text = pmc;
            
            // PM 상태에 따른 색상 표시 (녹색: 정상 작동, 황색: 경고, 적색: 치명적 오류)
            form.UpdateHeaderCardPmStatusColor(pma, pmb, pmc);
        }

        /// <summary>
        /// FOUP 상태 카드 업데이트
        /// </summary>
        public void UpdateFoupStatusCards(
            string foupAStatus, int foupACount, string foupAPath, string foupAPPID, string foupALotId, string foupAMid, string foupALock,
            string foupBStatus, int foupBCount, string foupBPath, string foupBPPID, string foupBLotId, string foupBMid, string foupBLock,
            string exchangeState, string queueInfo)
        {
            form.currentFoupACount = Math.Max(0, Math.Min(AppSettings.MaxFoupCapacity, foupACount));
            form.currentFoupBCount = Math.Max(0, Math.Min(AppSettings.MaxFoupCapacity, foupBCount));

            if (form.labelFoupAStatusHeadline != null) form.labelFoupAStatusHeadline.Text = foupAStatus;
            if (form.labelFoupAPathValue != null) form.labelFoupAPathValue.Text = foupAPath;
            if (form.labelFoupAPPIDValue != null) form.labelFoupAPPIDValue.Text = foupAPPID;
            if (form.labelFoupALotIdValue != null) form.labelFoupALotIdValue.Text = foupALotId;
            if (form.labelFoupAMidValue != null) form.labelFoupAMidValue.Text = foupAMid;
            if (form.labelFoupALockValue != null) form.labelFoupALockValue.Text = foupALock;

            if (form.labelFoupBStatusHeadline != null) form.labelFoupBStatusHeadline.Text = foupBStatus;
            if (form.labelFoupBPathValue != null) form.labelFoupBPathValue.Text = foupBPath;
            if (form.labelFoupBPPIDValue != null) form.labelFoupBPPIDValue.Text = foupBPPID;
            if (form.labelFoupBLotIdValue != null) form.labelFoupBLotIdValue.Text = foupBLotId;
            if (form.labelFoupBMidValue != null) form.labelFoupBMidValue.Text = foupBMid;
            if (form.labelFoupBLockValue != null) form.labelFoupBLockValue.Text = foupBLock;

            form.CurrentFoupExchangeState = $"교환 상태: {exchangeState}";
            form.CurrentFoupQueueInfo = queueInfo;
            if (form.labelFoupSummaryInfo != null) form.labelFoupSummaryInfo.Text = $"{form.CurrentFoupExchangeState} | {form.CurrentFoupQueueInfo}";

            if (form.labelSummaryFoupAStatus != null) form.labelSummaryFoupAStatus.Text = $"상태: {foupAStatus}";
            if (form.labelSummaryFoupBStatus != null) form.labelSummaryFoupBStatus.Text = $"상태: {foupBStatus}";

            form.currentFoupAStatusText = foupAStatus;
            form.currentFoupBStatusText = foupBStatus;

            // 개별 트랙 패널 업데이트
            if (form.foupATrackPanels != null)
            {
                foreach (var panel in form.foupATrackPanels)
                {
                    panel?.Invalidate();
                }
            }
            if (form.foupBTrackPanels != null)
            {
                foreach (var panel in form.foupBTrackPanels)
                {
                    panel?.Invalidate();
                }
            }

            var foupADoorClosed = !form.IsRegionDoorOpen(EquipmentRegion.FoupA);
            var foupBDoorClosed = !form.IsRegionDoorOpen(EquipmentRegion.FoupB);

            form.foupVisualizationControlA?.UpdateState(foupAStatus, form.currentFoupACount, foupADoorClosed);
            form.foupVisualizationControlB?.UpdateState(foupBStatus, form.currentFoupBCount, foupBDoorClosed);
        }

        /// <summary>
        /// FOUP 준비 버튼 상태 업데이트
        /// </summary>
        public void UpdateFoupPreparationButtons()
        {
            if (!form.IsLoggedIn)
            {
                // 로그인하지 않았을 때도 버튼 텍스트는 설정
                if (form.buttonMountFoupA != null)
                {
                    form.buttonMountFoupA.Enabled = false;
                    ApplyPreparationButtonStyle(
                        form.buttonMountFoupA,
                        form.IsFoupAMounted,
                        "FOUP A 장착 완료",
                        "FOUP A 미장착");
                }
                if (form.buttonMountFoupB != null)
                {
                    form.buttonMountFoupB.Enabled = false;
                    ApplyPreparationButtonStyle(
                        form.buttonMountFoupB,
                        form.IsFoupBMounted,
                        "FOUP B 장착 완료",
                        "FOUP B 미장착");
                }
                if (form.buttonToggleFoupMount != null)
                {
                    form.buttonToggleFoupMount.Enabled = false;
                    form.buttonToggleFoupMount.Visible = false; // 로그인하지 않았을 때도 숨김
                }
                if (form.buttonWaferLoading != null) form.buttonWaferLoading.Enabled = false;
                if (form.buttonWaferUnloading != null) form.buttonWaferUnloading.Enabled = false;
                return;
            }

            // 단일 토글 버튼은 숨김
            if (form.buttonToggleFoupMount != null) form.buttonToggleFoupMount.Visible = false;

            // 신규 A/B 장착 버튼 스타일 갱신
            ApplyPreparationButtonStyle(
                form.buttonMountFoupA,
                form.IsFoupAMounted,
                "FOUP A 장착 완료",
                "FOUP A 미장착");

            ApplyPreparationButtonStyle(
                form.buttonMountFoupB,
                form.IsFoupBMounted,
                "FOUP B 장착 완료",
                "FOUP B 미장착");

            ApplyPreparationButtonStyle(
                form.buttonWaferLoading,
                form.WaferLoadState == MainFormViewModel.WaferLoadStateType.Loading,
                "웨이퍼 로딩 중",
                "웨이퍼 로딩 대기");

            ApplyPreparationButtonStyle(
                form.buttonWaferUnloading,
                form.WaferLoadState == MainFormViewModel.WaferLoadStateType.Unloading,
                "웨이퍼 언로딩 중",
                "웨이퍼 언로딩 대기");

            var baseEnabled = (form.groupBoxFoupReady?.Enabled ?? true);
            // FOUP A 로딩은 공정 중이라도 TM가 FOUP A 픽업 대기/진행 중이 아니면 허용
            bool canLoadA = (!form.SimulationRunning) || (form.SimulationRunning && !form.HasPendingPickupFromFoupA());
            bool canUnloadB = !form.SimulationRunning; // 언로딩은 공정 중엔 잠금 유지

            if (form.buttonToggleFoupMount != null) form.buttonToggleFoupMount.Enabled = false;
            if (form.buttonMountFoupA != null) form.buttonMountFoupA.Enabled = baseEnabled && !form.SimulationRunning;
            if (form.buttonMountFoupB != null) form.buttonMountFoupB.Enabled = baseEnabled && !form.SimulationRunning;
            if (form.buttonWaferLoading != null) form.buttonWaferLoading.Enabled = baseEnabled && canLoadA;
            if (form.buttonWaferUnloading != null) form.buttonWaferUnloading.Enabled = baseEnabled && canUnloadB;
        }

        /// <summary>
        /// 준비 버튼 스타일 적용 (헬퍼 메서드)
        /// </summary>
        private static void ApplyPreparationButtonStyle(Button button, bool active, string activeText, string inactiveText)
        {
            if (button == null)
            {
                return;
            }

            button.Text = active ? activeText : inactiveText;
            button.BackColor = active ? Color.FromArgb(100, 120, 130) : Color.FromArgb(100, 120, 130);
            button.ForeColor = Color.White;
        }

        /// <summary>
        /// PM 요약 정보 업데이트
        /// </summary>
        /// <param name="data">PM 상세 데이터</param>
        public void UpdatePmSummary(Form1.PmDetailData data)
        {
            // 새 PmStatusPanel 사용 (깜빡임 방지)
            var statusPanel = form.GetPmStatusPanel(data.UnitKey);
            if (statusPanel != null)
            {
                var envInfo = form.BuildChamberEnvInfo(data.UnitKey);
                var timeInfo = $"{data.StepTimeCurrent}/{data.StepTimeTotal}s · 웨이퍼 {data.StepIndex}/{Math.Max(1, data.StepCount)}";
                var waferInfo = string.IsNullOrWhiteSpace(data.ActiveWaferText) ? "웨이퍼 정보 없음" : data.ActiveWaferText;
                var stepMsg = string.IsNullOrWhiteSpace(data.StatusText)
                    ? data.StepMessage
                    : $"{data.StepMessage}    {data.StatusText}";
                var isProcessing = data.StepTimeCurrent > 0 && data.StepTimeTotal > 0;

                // 알람 상태 확인 및 설정
                bool hasAlarm = form.ChamberAlarmStatus.ContainsKey(data.UnitKey) 
                    && form.ChamberAlarmStatus[data.UnitKey];
                statusPanel.HasAlarm = hasAlarm;

                // 모든 값을 한 번에 업데이트 (Invalidate 한 번만 호출)
                statusPanel.UpdateAllValues(
                    data.StatusText,
                    data.RecipeName,
                    data.StepName,
                    timeInfo,
                    waferInfo,
                    "", // 환경 정보는 별도 표에서 표시
                    stepMsg,
                    data.StepProgress,
                    isProcessing
                );
            }

            form.PmDetails[data.UnitKey] = data;

            // 환경 정보 표 실시간 업데이트
            if (form.PmEnvTables.ContainsKey(data.UnitKey) && form.PmEnvTables[data.UnitKey] != null && form.PmEnvTables[data.UnitKey].Visible)
            {
                form.UpdatePmEnvironmentTable(data.UnitKey);
            }

            // PmStatusPanel 환경 정보 업데이트
            UpdatePmStatusPanelEnvironment(data.UnitKey);
        }

        /// <summary>
        /// PmStatusPanel에 환경 정보 업데이트
        /// </summary>
        /// <param name="unitKey">유닛 키</param>
        public void UpdatePmStatusPanelEnvironment(string unitKey)
        {
            var statusPanel = form.GetPmStatusPanel(unitKey);
            if (statusPanel == null) return;

            // SV: 레시피가 적용된 경우에만 표시
            double svTemp = 0, svPress = 0;
            if (form.RecipeApplied && form.ChamberEnvSpecs.TryGetValue(unitKey, out var spec))
            {
                svTemp = spec.TargetTemperatureC;
                svPress = spec.TargetPressureTorr;
            }

            // 가스/전력 SV: 레시피가 적용된 경우에만 표시
            var gasRf = (form.RecipeApplied && form.UnitGasRfSv.ContainsKey(unitKey)) ? form.UnitGasRfSv[unitKey] : (0, 0, 0, 0);
            double svNF3 = gasRf.NF3;
            double svO2 = gasRf.O2;
            double svCF4 = gasRf.CF4;
            double svRf = gasRf.RF;

            // PV: 해당 챔버에서 공정 진행 중일 때만 표시
            double pvTemp = 0, pvPress = 0, pvNF3 = 0, pvO2 = 0, pvCF4 = 0, pvRf = 0;
            
            var cs = form.GetChamberStateForUnit(unitKey);
            // IsChamberProcessing과 동일한 조건 사용 (도어 상태 포함)
            bool isProcessing = form.SimulationRunning && cs != null && form.IsChamberProcessing(cs);
            
            if (isProcessing)
            {
                var live = form.ChamberEnvLive.ContainsKey(unitKey) ? form.ChamberEnvLive[unitKey] : null;
                pvTemp = live != null ? live.TemperatureC : svTemp;
                pvPress = live != null ? live.PressureTorr : svPress;
                
                // 가스 PV: 각 가스별로 SV 근처 랜덤값 생성 (실시간 변동)
                // SV가 0보다 크면 ±5% 범위에서 변동
                pvNF3 = form.GenerateGasPV(svNF3);
                pvO2 = form.GenerateGasPV(svO2);
                pvCF4 = form.GenerateGasPV(svCF4);
                
                // RF PV: SV 근처 ±5% 변동
                pvRf = svRf > 0 ? svRf * (0.95 + form.envRandom.NextDouble() * 0.1) : 0;
            }

            statusPanel.UpdateEnvironmentValues(
                pvTemp, svTemp,
                pvPress, svPress,
                pvNF3, svNF3,
                pvO2, svO2,
                pvCF4, svCF4,
                pvRf, svRf
            );
        }

        #endregion

        #region 복잡한 UI 업데이트 메서드

        /// <summary>
        /// 시뮬레이션 UI 업데이트 (전체 상태 동기화)
        /// </summary>
        public void UpdateSimulationUi()
        {
            string tmStatus;
            if (!form.SimulationRunning)
            {
                tmStatus = form.CurrentProcessState == MainFormViewModel.ProcessState.Error ? "Error" : "대기";
            }
            else if (form.SimulationPaused)
            {
                tmStatus = "Paused";
            }
            else
            {
                var pickupLabel = EquipmentRegionHelper.FormatRegionLabel(form.CurrentTransfer != null ? form.CurrentTransfer.Pickup : form.TmVisualTarget);
                var dropoffLabel = EquipmentRegionHelper.FormatRegionLabel(form.CurrentTransfer != null ? form.CurrentTransfer.Dropoff : form.TmVisualTarget);

                switch (form.TmPhase)
                {
                    case TransferController.TmPhase.MoveToPickup:
                        tmStatus = $"이동(픽업) → {pickupLabel}";
                        break;
                    case TransferController.TmPhase.WaitDoorPickupOpen:
                        tmStatus = $"{pickupLabel} 문 열림 대기";
                        break;
                    case TransferController.TmPhase.PickupExtend:
                        tmStatus = $"Arm 전진(픽업) → {pickupLabel}";
                        break;
                    case TransferController.TmPhase.PickupRetract:
                        tmStatus = $"Arm 후퇴(픽업) → {pickupLabel}";
                        break;
                    case TransferController.TmPhase.WaitDoorPickupClose:
                        tmStatus = $"{pickupLabel} 문 닫힘 대기";
                        break;
                    case TransferController.TmPhase.MoveToDropoff:
                        tmStatus = $"이동(반입) → {dropoffLabel}";
                        break;
                    case TransferController.TmPhase.WaitDoorDropoffOpen:
                        tmStatus = $"{dropoffLabel} 문 열림 대기";
                        break;
                    case TransferController.TmPhase.DropoffExtend:
                        tmStatus = $"Arm 전진(반입) → {dropoffLabel}";
                        break;
                    case TransferController.TmPhase.DropoffRetract:
                        tmStatus = $"Arm 후퇴(반입) → {dropoffLabel}";
                        break;
                    case TransferController.TmPhase.WaitDoorDropoffClose:
                        tmStatus = $"{dropoffLabel} 문 닫힘 대기";
                        break;
                    default:
                        tmStatus = (form.TransferController?.QueueCount ?? 0) > 0 ? "Transfer 준비" : "대기";
                        break;
                }
            }

            form.GetFoupDisplayCounts(out var displayFoupACount, out var displayFoupBCount);

            var pmaDetail = form.BuildPmDetail(form.ChamberAState, form.chamberCompletedCountA, AppSettings.MaxFoupCapacity);
            var pmbDetail = form.BuildPmDetail(form.ChamberBState, form.chamberCompletedCountB, AppSettings.MaxFoupCapacity);
            var pmcDetail = form.BuildPmDetail(form.ChamberCState, form.chamberCompletedCountC, AppSettings.MaxFoupCapacity);

            UpdatePmSummary(pmaDetail);
            UpdatePmSummary(pmbDetail);
            UpdatePmSummary(pmcDetail);

            var foupAStatePrefix = !form.IsFoupAMounted
                ? "FOUP A 미장착"
                : form.WaferLoadState == MainFormViewModel.WaferLoadStateType.Loading
                    ? "로딩 준비"
                    : form.WaferLoadState == MainFormViewModel.WaferLoadStateType.Unloading
                        ? "언로딩 대기"
                        : (form.SimulationRunning ? "공정 진행" : "대기");
            var foupAStatus = $"{foupAStatePrefix} · 웨이퍼 {displayFoupACount}장";

            var foupBStatePrefix = form.WaferLoadState == MainFormViewModel.WaferLoadStateType.Unloading
                ? "언로딩 진행"
                : (displayFoupBCount > 0 ? "완료 적재" : "대기");
            var foupBStatus = $"{foupBStatePrefix} · 완료 {displayFoupBCount}/{AppSettings.MaxFoupCapacity}";

            var exchangeState = form.SimulationRunning
                ? (form.SimulationPaused ? "이송 대기 (Pause)" : "TM 이송 중")
                : form.GetWaferLoadStateDisplay();
            var queueInfo = $"웨이퍼 상태: {form.GetWaferLoadStateDisplay()} · 완료 {displayFoupBCount}/{Math.Max(1, form.GetActiveTarget())}";

            var foupALock = !form.IsFoupAMounted
                ? "미장착"
                : (form.SimulationRunning && !form.SimulationPaused ? "Locked" : "Released");
            var foupBLock = (displayFoupBCount > 0 || form.WaferLoadState == MainFormViewModel.WaferLoadStateType.Unloading)
                ? "Locked"
                : "Released";

            UpdateFoupStatusCards(
                foupAStatus, displayFoupACount, "CM1", form.LastSelectedRecipe, "LOT-A01", "MID-A", foupALock,
                foupBStatus, displayFoupBCount, "CM2", form.LastSelectedRecipe, "LOT-B01", "MID-B", foupBLock,
                exchangeState, queueInfo);

            form.RefreshAllDoorVisuals();
            UpdateChamberWaferIndicators();

            form.labelSummaryTMStatus.Text = $"상태: {tmStatus}";
            form.labelSummaryFoupAStatus.Text = $"상태: {foupAStatus}";
            form.labelSummaryFoupBStatus.Text = $"상태: {foupBStatus}";
            form.labelHeaderFoupAStatus.Text = foupAStatus;
            form.labelHeaderFoupBStatus.Text = foupBStatus;
            
            // 버튼 상태 업데이트 (일시정지/재개 후 상태 동기화)
            UpdateProcessControlButtons();

            var closedDoors = 0;
            if (form.ChamberAState.CurrentWafer != null) closedDoors++;
            if (form.ChamberBState.CurrentWafer != null) closedDoors++;
            if (form.ChamberCState.CurrentWafer != null) closedDoors++;
            form.StatusDoorText = closedDoors == 0 ? "문 상태: 모두 개방" : $"문 상태: {closedDoors}개 닫힘";

            var processDetail = $"{form.StatusProcessText} | 완료 {form.foupManager?.GetFoupBCount() ?? 0}/{AppSettings.MaxFoupCapacity}";
            form.StatusProcessDetail = processDetail;
            form.ApplyStatusTextsToLabels();

            var doorClosedA = !form.IsRegionDoorOpen(EquipmentRegion.ChamberA);
            var doorClosedB = !form.IsRegionDoorOpen(EquipmentRegion.ChamberB);
            var doorClosedC = !form.IsRegionDoorOpen(EquipmentRegion.ChamberC);

            UpdateDoorIndicator(form.panelLampChamberA, null, form.panelDoorChamberA, doorClosedA, EquipmentRegion.ChamberA);
            UpdateDoorIndicator(form.panelLampChamberB, null, form.panelDoorChamberB, doorClosedB, EquipmentRegion.ChamberB);
            UpdateDoorIndicator(form.panelLampChamberC, null, form.panelDoorChamberC, doorClosedC, EquipmentRegion.ChamberC);

            // Chamber 램프를 공정 상태에 따라 업데이트 (도어 상태가 아닌 공정 상태 반영)
            UpdateChamberProcessLamps();

            UpdateHeaderCardStatuses(
                tmStatus,
                pmaDetail.StatusText,
                pmbDetail.StatusText,
                pmcDetail.StatusText);
            
            // 상단 헤더의 PM1, PM2, PM3 상태 색상 업데이트
            form.UpdateHeaderPmStatusColor(pmaDetail.StatusText, pmbDetail.StatusText, pmcDetail.StatusText);

            form.currentFoupAStatusText = foupAStatus;
            form.currentFoupBStatusText = foupBStatus;

            UpdateCentralFoupVisualState();
            UpdateFoupPreparationButtons();
            
            // 캔버스에 직접 그리므로 항상 업데이트
            if (form.panelEquipmentCanvas != null)
            {
                form.panelEquipmentCanvas.Invalidate();
            }
        }

        /// <summary>
        /// TM 시각화 업데이트
        /// </summary>
        public void UpdateTmVisualization()
        {
            // TM 컨트롤 업데이트 (별도 컨트롤이므로 직접 업데이트)
            // 웨이퍼가 블레이드에 있는지 확인
            bool waferOnBlade = form.IsWaferOnBlade();
            var waferColor = waferOnBlade
                ? form.GetWaferColorForRegion(form.TmVisualTarget)
                : (Color?)null;
            form.tmVisualizationControl?.UpdateTmState(form.TmVisualTarget, waferOnBlade, form.TmBladeExtensionFactor, waferColor);
        }

        /// <summary>
        /// 하드웨어 모드에서 TM 시각화 업데이트
        /// </summary>
        public void UpdateTmVisualizationFromHardware()
        {
            if (form.TmHardwareController == null || !form.EthercatConnected) return;

            try
            {
                // 현재 서보 위치 읽기
                form.TmHardwareController.UpdateCurrentPositions();
                long currentX = form.TmHardwareController.CurrentAxis2Position;
                long currentY = form.TmHardwareController.CurrentAxis1Position;

                // 서보 위치를 Region으로 변환 (참고용)
                var hardwareRegion = EquipmentRegionHelper.DetermineRegionFromPosition(currentX, currentY, form.TmHardwareController.Positions);

                // 실린더 상태 확인 (웨이퍼 보유 여부 추정)
                bool carrying = form.TmHardwareController.IsVacuumOn;

                // 상태 변수 업데이트
                form.TmCarryingVisual = carrying;
                
                // ========== 핵심 변경: Phase 무시하고 항상 실제 하드웨어 상태 사용 ==========
                // 하드웨어 모드에서는 Phase와 관계없이 실제 센서 값만 사용하여 UI 업데이트
                // 이렇게 하면 UI가 실제 하드웨어 움직임과 완벽히 동기화됨
                
                // 1. 서보 위치: 항상 실제 하드웨어 위치 사용
                EquipmentRegion targetRegion = hardwareRegion;
                form.TmVisualTarget = hardwareRegion;
                
                // 2. 실린더 상태: 항상 실제 하드웨어 상태 확인
                bool cylinderExtended = form.CheckTmCylinderExtended();
                bool cylinderRetracted = form.CheckTmCylinderRetracted();
                
                // 실린더 상태에 따라 extensionFactor 설정
                if (cylinderExtended && !cylinderRetracted)
                {
                    // 전진 상태
                    form.TmBladeExtensionFactor = 1.3f;
                }
                else if (cylinderRetracted && !cylinderExtended)
                {
                    // 후진 상태
                    form.TmBladeExtensionFactor = 0.55f;
                }
                else
                {
                    // 이동 중이거나 불명확한 상태: 기본값 사용
                    form.TmBladeExtensionFactor = 0.7f; // 중간값
                }
                
                // 3. 웨이퍼 보유 상태: 하드웨어 진공 상태와 로직 상태를 모두 확인
                // 하드웨어 진공 상태가 ON이거나, 로직상 웨이퍼가 블레이드에 있어야 하는 경우
                bool waferOnBlade = carrying || form.IsWaferOnBlade(); // 하드웨어 상태 또는 로직 상태
                var waferColor = waferOnBlade ? form.GetWaferColorForRegion(targetRegion) : (Color?)null;
                
                // 4. UI 업데이트 (실제 하드웨어 상태만 반영)
                form.tmVisualizationControl?.UpdateTmState(targetRegion, waferOnBlade, form.TmBladeExtensionFactor, waferColor, false);
            }
            catch (Exception ex)
            {
                // 오류 시 무시 (로그 스팸 방지)
                System.Diagnostics.Debug.WriteLine($"TM 시각화 업데이트 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// PM 환경 테이블 업데이트
        /// </summary>
        /// <param name="unitKey">유닛 키</param>
        public void UpdatePmEnvironmentTable(string unitKey)
        {
            if (!form.PmEnvTables.TryGetValue(unitKey, out var envTable) || envTable == null)
            {
                return;
            }

            // SV: 레시피가 적용된 경우에만 표시
            double svTemp = 0, svPress = 0;
            if (form.RecipeApplied && form.ChamberEnvSpecs.TryGetValue(unitKey, out var spec))
            {
                svTemp = spec.TargetTemperatureC;
                svPress = spec.TargetPressureTorr;
            }

            // 가스/전력 SV: 레시피가 적용된 경우에만 표시
            var gasRf = (form.RecipeApplied && form.UnitGasRfSv.ContainsKey(unitKey)) ? form.UnitGasRfSv[unitKey] : (0, 0, 0, 0);
            double svNF3 = gasRf.NF3;
            double svO2 = gasRf.O2;
            double svCF4 = gasRf.CF4;
            double svRf = gasRf.RF;

            // PV: 해당 챔버에 웨이퍼가 있고 공정이 실제 진행 중일 때만 표시
            // 조건: 웨이퍼 있음 + 공정 시간 설정됨 + 시간이 흐르고 있음 (프로그레스 바와 동기화)
            double pvTemp, pvPress, pvNF3, pvO2, pvCF4, pvRf;
            
            // 해당 챔버의 상태 확인
            var cs = form.GetChamberStateForUnit(unitKey);
            // 공정이 실제로 진행 중인지 확인:
            // 1. 시뮬레이션 실행 중
            // 2. 챔버에 웨이퍼가 있음
            // 3. TotalSeconds > 0 (공정 시간이 설정됨 = StartChamberProcessing 호출됨)
            // 4. RemainingSeconds > 0 (아직 공정 중)
            // 5. StatusText가 공정 진행 상태 ("처리 중", "2차 노광 중", "Processing", "공정 중")
            bool isChamberProcessing = form.SimulationRunning && cs != null && cs.CurrentWafer != null && 
                cs.TotalSeconds > 0 && cs.RemainingSeconds > 0 &&
                (cs.StatusText.Contains("처리 중") || cs.StatusText.Contains("2차 노광 중") || 
                 cs.StatusText.Contains("Processing") || cs.StatusText.Contains("공정 중"));
            
            if (isChamberProcessing)
            {
                // 해당 챔버에서 공정 진행 중: 실제 PV 값 생성
                var live = form.UpdateLiveEnvironment(unitKey, cs);
                pvTemp = live != null ? live.TemperatureC : svTemp;
                pvPress = live != null ? live.PressureTorr : svPress;

                // 가스/전력 PV: 알람이 발생하지 않도록 안전하게 랜덤 값 생성
                pvNF3 = form.GenerateSafeRandomValue(svNF3, form.AlarmThresholds.GasWarnAbsSccm, form.AlarmThresholds.GasAlarmAbsSccm);
                pvO2 = form.GenerateSafeRandomValue(svO2, form.AlarmThresholds.GasWarnAbsSccm, form.AlarmThresholds.GasAlarmAbsSccm);
                pvCF4 = form.GenerateSafeRandomValue(svCF4, form.AlarmThresholds.GasWarnAbsSccm, form.AlarmThresholds.GasAlarmAbsSccm);
                // RF: 1% 확률로 알람 발생
                if (svRf > 0)
                {
                    bool shouldTriggerAlarm = form.envRandom.NextDouble() < 0.01;
                    double rfWarnThreshold = svRf * form.AlarmThresholds.RfWarnRatio;
                    double rfAlarmThreshold = svRf * form.AlarmThresholds.RfAlarmRatio;
                    double rfSafeRange;
                    if (shouldTriggerAlarm)
                    {
                        // 알람 발생: 경고 임계값의 80-100% 범위
                        rfSafeRange = rfWarnThreshold * (0.8 + form.envRandom.NextDouble() * 0.2);
                    }
                    else
                    {
                        // 안전 범위: 경고 임계값의 20% 이내
                        rfSafeRange = Math.Min(rfWarnThreshold * 0.2, rfAlarmThreshold * 0.15);
                    }
                    double rfDeviation = (form.envRandom.NextDouble() - 0.5) * rfSafeRange * 2;
                    pvRf = Math.Max(0, svRf + rfDeviation);
                }
                else
                {
                    pvRf = 0;
                }

                // 경고/알람 판정 (해당 챔버 공정 실행 중일 때만)
                form.EvaluateEnvironmentAlarms(
                    unitKey,
                    svNF3, svO2, svCF4, svPress, svRf, svTemp,
                    pvNF3, pvO2, pvCF4, pvPress, pvRf, pvTemp
                );
            }
            else
            {
                // 해당 챔버에서 공정 미실행: PV 값을 0으로 설정 (표시 시 "-"로 변환)
                pvTemp = 0;
                pvPress = 0;
                pvNF3 = 0;
                pvO2 = 0;
                pvCF4 = 0;
                pvRf = 0;
            }

            // 표 업데이트
            var values = new[]
            {
                (pvNF3, svNF3),
                (pvO2, svO2),
                (pvCF4, svCF4),
                (pvPress, svPress),
                (pvRf, svRf),
                (pvTemp, svTemp)
            };

            for (int i = 0; i < values.Length && i < 6; i++)
            {
                int row = i + 1;
                // TableLayoutPanel의 컨트롤 접근: GetControlFromPosition 사용
                var pvLabel = envTable.GetControlFromPosition(1, row) as Label;
                var svLabel = envTable.GetControlFromPosition(2, row) as Label;
                
                // PV 값 표시: 해당 챔버 공정 실행 중일 때만 실제 값 표시, 그 외에는 "-" 표시
                if (pvLabel != null)
                {
                    if (isChamberProcessing && values[i].Item1 != 0)
                    {
                        pvLabel.Text = values[i].Item1.ToString("0.##");
                    }
                    else
                    {
                        pvLabel.Text = "-";
                    }
                }
                
                // SV 값 표시: 레시피가 적용되지 않았거나 값이 0이면 "-" 표시
                if (svLabel != null)
                {
                    if (!form.RecipeApplied || values[i].Item2 == 0)
                    {
                        svLabel.Text = "-";
                    }
                    else
                    {
                        svLabel.Text = values[i].Item2.ToString("0.##");
                    }
                }
            }
        }

        /// <summary>
        /// 공정 제어 버튼 상태 업데이트
        /// </summary>
        public void UpdateProcessControlButtons()
        {
            if (!form.IsLoggedIn)
            {
                form.buttonStart.Enabled = false;
                form.buttonPause.Enabled = false;
                form.buttonStop.Enabled = false;
                form.buttonResetAlarm.Enabled = false;
                form.buttonResetProcess.Enabled = false;
                form.buttonApplyRecipe.Enabled = false;
                if (form.buttonEquipmentControl != null)
                {
                    form.buttonEquipmentControl.Enabled = false;
                }
                return;
            }

            form.buttonStart.Enabled = !form.SimulationRunning || form.SimulationPaused || form.CurrentProcessState == MainFormViewModel.ProcessState.Error;
            form.buttonPause.Enabled = form.SimulationRunning && !form.SimulationPaused;
            form.buttonStop.Enabled = form.SimulationRunning;
            
            // 알람 리셋 버튼: 헤더에 알람이 표시되면 활성화
            bool hasHeaderAlarm = form.HasHeaderAlarm();
            form.buttonResetAlarm.Enabled = hasHeaderAlarm;
            
            form.buttonResetProcess.Enabled = true;
            form.buttonApplyRecipe.Enabled = true;
            if (form.buttonEquipmentControl != null)
            {
                form.buttonEquipmentControl.Enabled = true;
            }
        }

        /// <summary>
        /// 메인 램프 색상 업데이트 (UI + 장비 제어)
        /// </summary>
        /// <param name="greenActive">녹색 램프 활성화 여부</param>
        /// <param name="yellowActive">황색 램프 활성화 여부</param>
        /// <param name="redActive">적색 램프 활성화 여부</param>
        public void UpdateMainLampColors(Color? greenActive, bool yellowActive, bool redActive)
        {
            form.SetLamp(form.panelMainLampGreen, greenActive.HasValue, greenActive ?? Color.ForestGreen);
            form.SetLamp(form.panelMainLampYellow, yellowActive, Color.Goldenrod);
            form.SetLamp(form.panelMainLampRed, redActive, Color.Firebrick);

            // 실제 장비 3색 램프 제어 (EtherCAT 연결 시)
            if (form.EthercatConnected)
            {
                try
                {
                    // 적색 (Digital_Output 0)
                    form.EtherCAT_M.Digital_Output(0, redActive);
                    // 황색 (Digital_Output 1)
                    form.EtherCAT_M.Digital_Output(1, yellowActive);
                    // 녹색 (Digital_Output 2)
                    form.EtherCAT_M.Digital_Output(2, greenActive.HasValue);
                }
                catch (Exception ex)
                {
                    form.AddLogMessage($"3색 램프 제어 오류: {ex.Message}", "ERROR");
                }
            }
        }

        /// <summary>
        /// 헤더 이벤트 메시지 업데이트
        /// </summary>
        /// <param name="level">알람 레벨</param>
        /// <param name="message">메시지</param>
        public void UpdateHeaderEventMessage(string level, string message)
        {
            if (form.labelHeaderEventTitle == null || form.labelHeaderEventMessage == null)
            {
                return;
            }

            var normalizedLevel = string.IsNullOrWhiteSpace(level)
                ? "ALARM"
                : level.Trim().ToUpperInvariant();

            form.labelHeaderEventTitle.Text = "최근 알람";
            form.labelHeaderEventMessage.Text = message;

            var badgeColor = Color.FromArgb(96, 125, 139);
            switch (normalizedLevel)
            {
                case "WARN":
                case "WARNING":
                    badgeColor = Color.FromArgb(255, 167, 38);
                    break;
                case "ERROR":
                case "ALARM":
                    badgeColor = Color.FromArgb(244, 67, 54);
                    break;
                case "INFO":
                    badgeColor = Color.FromArgb(33, 150, 243);
                    break;
            }

            if (form.panelHeaderMessageAccent != null)
            {
                form.panelHeaderMessageAccent.BackColor = badgeColor;
            }
        }

        /// <summary>
        /// 중앙 FOUP 시각적 상태 업데이트
        /// </summary>
        public void UpdateCentralFoupVisualState()
        {
            form.GetFoupDisplayCounts(out var displayFoupACount, out var displayFoupBCount);

            var foupAVisual = form.GetFoupAPanelVisualState(displayFoupACount);
            form.ApplyFoupPanelVisual(form.panelFoupStatusA, form.labelFoupInfoATitle, foupAVisual.Descriptor, foupAVisual.Accent);
            form.ApplyFoupPanelVisual(form.panelSummaryFoupA, form.labelSummaryFoupATitle, foupAVisual.Descriptor, foupAVisual.Accent);

            var foupBVisual = form.GetFoupBPanelVisualState(displayFoupBCount);
            form.ApplyFoupPanelVisual(form.panelFoupStatusB, form.labelFoupInfoBTitle, foupBVisual.Descriptor, foupBVisual.Accent);
            form.ApplyFoupPanelVisual(form.panelSummaryFoupB, form.labelSummaryFoupBTitle, foupBVisual.Descriptor, foupBVisual.Accent);
        }

        /// <summary>
        /// 챔버 웨이퍼 인디케이터 업데이트
        /// </summary>
        public void UpdateChamberWaferIndicators()
        {
            UpdateChamberWaferIndicator(form.ChamberAState, form.panelWaferChamberA);
            UpdateChamberWaferIndicator(form.ChamberBState, form.panelWaferChamberB);
            UpdateChamberWaferIndicator(form.ChamberCState, form.panelWaferChamberC);
        }

        /// <summary>
        /// 챔버 웨이퍼 인디케이터 업데이트 (단일 챔버)
        /// </summary>
        /// <param name="chamber">챔버 상태</param>
        /// <param name="waferPanel">웨이퍼 패널</param>
        public void UpdateChamberWaferIndicator(ChamberController.ChamberState chamber, Panel waferPanel)
        {
            if (waferPanel == null)
            {
                return;
            }

            bool hasWafer =
                chamber?.CurrentWafer != null
                || form.IsWaferEnRouteToChamber(chamber)
                || form.IsWaferAwaitingPickup(chamber);
            if (!hasWafer)
            {
                waferPanel.Visible = false;
                waferPanel.Tag = null;
                return;
            }

            waferPanel.Visible = true;
            var color = chamber?.CurrentWafer != null
                ? form.GetWaferColorForState(chamber)
                : Color.FromArgb(240, 230, 80); // 삽입/회수 중(문 열림) 웨이퍼
            waferPanel.Tag = color;
            waferPanel.BringToFront();
            form.UpdateWaferPanelRegion(waferPanel);
            waferPanel.Invalidate();
        }

        /// <summary>
        /// 도어 인디케이터 업데이트
        /// </summary>
        /// <param name="lampPanel">램프 패널</param>
        /// <param name="doorLabel">도어 라벨</param>
        /// <param name="doorPanel">도어 패널</param>
        /// <param name="doorClosed">도어 닫힘 여부</param>
        /// <param name="region">장비 영역</param>
        public void UpdateDoorIndicator(Panel lampPanel, Label doorLabel, Panel doorPanel, bool doorClosed, EquipmentRegion? region = null)
        {
            UpdateDoorLamp(lampPanel, doorClosed, region);
            if (doorLabel != null)
            {
                doorLabel.Text = doorClosed ? "Door: 닫힘" : "Door: 열림";
            }

            if (doorPanel != null)
            {
                doorPanel.BackColor = doorClosed
                    ? Color.FromArgb(200, 230, 210)  // 밝은 녹색 계열
                    : Color.FromArgb(245, 245, 250);  // 밝은 회색
            }
        }

        /// <summary>
        /// 챔버 공정 램프 업데이트
        /// 공정 중: 켜짐, 공정 완료(웨이퍼 있음): 깜빡임, 웨이퍼 없음: 꺼짐
        /// </summary>
        public void UpdateChamberProcessLamps()
        {
            // 각 챔버의 상태 확인
            bool chamberAProcessing = form.IsChamberProcessing(form.ChamberAState);
            bool chamberBProcessing = form.IsChamberProcessing(form.ChamberBState);
            bool chamberCProcessing = form.IsChamberProcessing(form.ChamberCState);
            
            bool chamberACompleted = form.IsChamberCompleted(form.ChamberAState);
            bool chamberBCompleted = form.IsChamberCompleted(form.ChamberBState);
            bool chamberCCompleted = form.IsChamberCompleted(form.ChamberCState);

            // UI 램프 색상 및 깜빡임 업데이트
            UpdateChamberLampUI(form.panelLampChamberA, chamberAProcessing, chamberACompleted);
            UpdateChamberLampUI(form.panelLampChamberB, chamberBProcessing, chamberBCompleted);
            UpdateChamberLampUI(form.panelLampChamberC, chamberCProcessing, chamberCCompleted);

            // 실제 장비 램프 제어 (EtherCAT 연결 시)
            if (form.EthercatConnected && !form.isSyncingEquipmentState)
            {
                // Chamber A 램프 제어
                UpdateHardwareChamberLamp(EquipmentRegion.ChamberA, chamberAProcessing, chamberACompleted);
                
                // Chamber B 램프 제어
                UpdateHardwareChamberLamp(EquipmentRegion.ChamberB, chamberBProcessing, chamberBCompleted);
                
                // Chamber C 램프 제어
                UpdateHardwareChamberLamp(EquipmentRegion.ChamberC, chamberCProcessing, chamberCCompleted);
            }
        }

        /// <summary>
        /// 도어 램프 업데이트
        /// </summary>
        /// <param name="lampPanel">램프 패널</param>
        /// <param name="doorClosed">도어 닫힘 여부</param>
        /// <param name="region">장비 영역</param>
        public void UpdateDoorLamp(Panel lampPanel, bool doorClosed, EquipmentRegion? region = null)
        {
            // 도어 램프는 더 이상 도어 상태가 아닌 공정 상태를 표시
            // 이 함수는 도어 상태 표시용으로만 사용되며, 실제 램프 제어는 UpdateChamberProcessLamps()에서 처리
            // 램프 패널의 색상은 UpdateChamberProcessLamps()에서 업데이트하므로 여기서는 건드리지 않음
        }

        // 챔버 램프 깜빡임 상태 추적 (UI 깜빡임 주기 제어용)
        private readonly System.Collections.Generic.Dictionary<Panel, bool> _chamberLampBlinkStates = 
            new System.Collections.Generic.Dictionary<Panel, bool>();
        private readonly System.Collections.Generic.Dictionary<Panel, int> _chamberLampBlinkCounters = 
            new System.Collections.Generic.Dictionary<Panel, int>();
        
        // 하드웨어 램프 깜빡임 상태 추적 (실제 장비 제어용)
        private readonly System.Collections.Generic.Dictionary<EquipmentRegion, bool> _hardwareLampBlinkStates = 
            new System.Collections.Generic.Dictionary<EquipmentRegion, bool>();
        private readonly System.Collections.Generic.Dictionary<EquipmentRegion, int> _hardwareLampBlinkCounters = 
            new System.Collections.Generic.Dictionary<EquipmentRegion, int>();
        
        private const int CHAMBER_LAMP_BLINK_INTERVAL = 10; // 10틱마다 깜빡임 (시뮬레이션 타이머 기준)

        /// <summary>
        /// 챔버 램프 UI 업데이트
        /// </summary>
        /// <param name="lampPanel">램프 패널</param>
        /// <param name="isProcessing">공정 진행 중 여부</param>
        /// <param name="isCompleted">공정 완료 상태 (웨이퍼 있음)</param>
        public void UpdateChamberLampUI(Panel lampPanel, bool isProcessing, bool isCompleted)
        {
            if (lampPanel == null) return;
            
            if (isProcessing)
            {
                // 공정 중: 녹색 켜짐
                lampPanel.BackColor = Color.ForestGreen;
                // 깜빡임 상태 초기화
                if (_chamberLampBlinkStates.ContainsKey(lampPanel))
                {
                    _chamberLampBlinkStates[lampPanel] = false;
                }
                if (_chamberLampBlinkCounters.ContainsKey(lampPanel))
                {
                    _chamberLampBlinkCounters[lampPanel] = 0;
                }
            }
            else if (isCompleted)
            {
                // 공정 완료 (웨이퍼 있음): 깜빡임
                // 각 챔버별로 독립적인 깜빡임 카운터 사용
                if (!_chamberLampBlinkCounters.ContainsKey(lampPanel))
                {
                    _chamberLampBlinkCounters[lampPanel] = 0;
                    _chamberLampBlinkStates[lampPanel] = false;
                }
                
                _chamberLampBlinkCounters[lampPanel]++;
                if (_chamberLampBlinkCounters[lampPanel] >= CHAMBER_LAMP_BLINK_INTERVAL)
                {
                    _chamberLampBlinkCounters[lampPanel] = 0;
                    // 깜빡임 상태 토글
                    _chamberLampBlinkStates[lampPanel] = !_chamberLampBlinkStates[lampPanel];
                }
                
                // 깜빡임 상태에 따라 색상 변경
                bool isBlinkOn = _chamberLampBlinkStates[lampPanel];
                lampPanel.BackColor = isBlinkOn ? Color.ForestGreen : Color.FromArgb(230, 230, 235);
            }
            else
            {
                // 웨이퍼 없음: 꺼짐 (밝은 회색)
                lampPanel.BackColor = Color.FromArgb(230, 230, 235);
                // 깜빡임 상태 초기화
                if (_chamberLampBlinkStates.ContainsKey(lampPanel))
                {
                    _chamberLampBlinkStates[lampPanel] = false;
                }
                if (_chamberLampBlinkCounters.ContainsKey(lampPanel))
                {
                    _chamberLampBlinkCounters[lampPanel] = 0;
                }
            }
        }

        /// <summary>
        /// 하드웨어 챔버 램프 제어 (공정 중: 켜짐, 공정 완료: 깜빡임, 웨이퍼 없음: 꺼짐)
        /// </summary>
        /// <param name="region">챔버 영역</param>
        /// <param name="isProcessing">공정 진행 중 여부</param>
        /// <param name="isCompleted">공정 완료 상태 (웨이퍼 있음)</param>
        private void UpdateHardwareChamberLamp(EquipmentRegion region, bool isProcessing, bool isCompleted)
        {
            if (isProcessing)
            {
                // 공정 중: 램프 켜짐
                form.SetChamberLamp(region, true);
                // 깜빡임 상태 초기화
                if (_hardwareLampBlinkStates.ContainsKey(region))
                {
                    _hardwareLampBlinkStates[region] = false;
                }
                if (_hardwareLampBlinkCounters.ContainsKey(region))
                {
                    _hardwareLampBlinkCounters[region] = 0;
                }
            }
            else if (isCompleted)
            {
                // 공정 완료 (웨이퍼 있음): 깜빡임
                // 각 챔버별로 독립적인 깜빡임 카운터 사용
                if (!_hardwareLampBlinkCounters.ContainsKey(region))
                {
                    _hardwareLampBlinkCounters[region] = 0;
                    _hardwareLampBlinkStates[region] = false;
                }
                
                _hardwareLampBlinkCounters[region]++;
                if (_hardwareLampBlinkCounters[region] >= CHAMBER_LAMP_BLINK_INTERVAL)
                {
                    _hardwareLampBlinkCounters[region] = 0;
                    // 깜빡임 상태 토글
                    _hardwareLampBlinkStates[region] = !_hardwareLampBlinkStates[region];
                }
                
                // 깜빡임 상태에 따라 하드웨어 램프 제어
                bool isBlinkOn = _hardwareLampBlinkStates[region];
                form.SetChamberLamp(region, isBlinkOn);
            }
            else
            {
                // 웨이퍼 없음: 램프 꺼짐
                form.SetChamberLamp(region, false);
                // 깜빡임 상태 초기화
                if (_hardwareLampBlinkStates.ContainsKey(region))
                {
                    _hardwareLampBlinkStates[region] = false;
                }
                if (_hardwareLampBlinkCounters.ContainsKey(region))
                {
                    _hardwareLampBlinkCounters[region] = 0;
                }
            }
        }

        /// <summary>
        /// TM 애니메이션 Idle 타겟 업데이트
        /// </summary>
        public void UpdateTmAnimationIdleTarget()
        {
            if (form.TmPhase != TransferController.TmPhase.Idle || form.CurrentTransfer != null)
            {
                return;
            }

            var idleTarget = EquipmentRegion.TM;

            if (form.TmVisualTarget != idleTarget || form.TmCarryingVisual)
            {
                form.TmVisualTarget = idleTarget;
                form.TmCarryingVisual = false;
                form.TmBladeExtensionFactor = 0.55f;
                
                // 하드웨어 모드: 실제 하드웨어 위치로 업데이트
                // 시뮬레이션 모드: Phase 기반 값으로 업데이트
                if (form.IsTmHardwareModeAvailable())
                {
                    UpdateTmVisualizationFromHardware();
                }
                else
                {
                    UpdateTmVisualization();
                }
            }
        }

        /// <summary>
        /// 메인 램프 색상 업데이트 (UI만)
        /// </summary>
        /// <param name="greenActive">녹색 램프 활성화 여부</param>
        /// <param name="yellowActive">황색 램프 활성화 여부</param>
        /// <param name="redActive">적색 램프 활성화 여부</param>
        public void UpdateMainLampColorsUIOnly(Color? greenActive, bool yellowActive, bool redActive)
        {
            form.SetLamp(form.panelMainLampGreen, greenActive.HasValue, greenActive ?? Color.ForestGreen);
            form.SetLamp(form.panelMainLampYellow, yellowActive, Color.Goldenrod);
            form.SetLamp(form.panelMainLampRed, redActive, Color.Firebrick);
        }

        /// <summary>
        /// 도어 인디케이터 업데이트 (UI만)
        /// </summary>
        /// <param name="region">장비 영역</param>
        /// <param name="doorClosed">도어 닫힘 여부</param>
        public void UpdateDoorIndicatorUIOnly(EquipmentRegion region, bool doorClosed)
        {
            var visuals = form.GetDoorVisualTargets(region);
            if (visuals.DoorPanel == null && visuals.LampPanel == null)
            {
                return;
            }

            // 램프 UI만 업데이트 (장비 제어 없음)
            if (visuals.LampPanel != null)
            {
                visuals.LampPanel.BackColor = doorClosed ? Color.ForestGreen : Color.FromArgb(230, 230, 235);  // 밝은 회색
            }

            // 도어 패널 UI만 업데이트 (애니메이션 없이 즉시)
            if (visuals.DoorPanel != null)
            {
                visuals.DoorPanel.BackColor = doorClosed
                    ? Color.FromArgb(200, 230, 210)  // 밝은 녹색 계열
                    : Color.FromArgb(245, 245, 250);  // 밝은 회색
                
                // DoorAnimationHelper를 사용하여 즉시 상태 적용 (애니메이션 없음)
                Form1.DoorAnimationHelper.ApplyImmediateState(visuals.DoorPanel, open: !doorClosed);
            }
        }

        /// <summary>
        /// TM FOUP 슬롯 업데이트 (지역별)
        /// </summary>
        /// <param name="region">장비 영역</param>
        public void UpdateTmFoupSlotForRegion(EquipmentRegion region)
        {
            if (form.TmHardwareController == null) return;

            if (region == EquipmentRegion.FoupA)
            {
                // FOUP A: 1층부터 빼가므로
                // 현재 층 = configuredFoupALoadCount - foupARemainingInventoryCount + 1
                // 예: 5개 로딩, 5개 남음 → 1층 (slot index 0)
                // 예: 5개 로딩, 4개 남음 → 2층 (slot index 1)
                int slotIndex = form.ConfiguredFoupALoadCount - form.FoupARemainingInventoryCount;
                slotIndex = Math.Max(0, Math.Min(4, slotIndex)); // 0~4 범위로 제한
                form.TmHardwareController.CurrentFoupASlot = slotIndex + 1; // 1~5로 변환
                form.AddLogMessage($"FOUP A 현재 층: {form.TmHardwareController.CurrentFoupASlot}층", "INFO");
            }
            else if (region == EquipmentRegion.FoupB)
            {
                // FOUP B: 완료된 웨이퍼를 1층부터 채워넣음
                // 현재 층 = FoupBCompleted.Count + 1
                int slotIndex = form.foupManager?.GetFoupBCount() ?? 0;
                slotIndex = Math.Max(0, Math.Min(4, slotIndex)); // 0~4 범위로 제한
                form.TmHardwareController.CurrentFoupBSlot = slotIndex + 1;
                form.AddLogMessage($"FOUP B 현재 층: {form.TmHardwareController.CurrentFoupBSlot}층", "INFO");
            }
        }

        /// <summary>
        /// 웨이퍼 패널 영역 업데이트
        /// </summary>
        /// <param name="panel">웨이퍼 패널</param>
        public void UpdateWaferPanelRegion(Panel panel)
        {
            if (panel == null)
            {
                return;
            }

            var rect = panel.ClientRectangle;
            rect.Inflate(-3, -3);
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddEllipse(rect);
                panel.Region = new Region(path);
            }
        }

        #endregion
    }
}

