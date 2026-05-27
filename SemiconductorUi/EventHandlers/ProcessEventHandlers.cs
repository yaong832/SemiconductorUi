using System;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.ViewModels;
using SemiconductorUi.Repositories;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.EventHandlers
{
    /// <summary>
    /// 공정 제어 관련 이벤트 핸들러
    /// </summary>
    public class ProcessEventHandlers : Form1EventHandlersBase
    {
        /// <summary>
        /// ProcessEventHandlers 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public ProcessEventHandlers(Form1 form) : base(form) { }

        /// <summary>
        /// 공정 시작 버튼 클릭 이벤트 핸들러
        /// 공정 시작 전 준비 상태를 확인하고 시뮬레이션을 시작하거나 재개합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonStart_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }

            if (form.SimulationRunning && !form.SimulationPaused)
            {
                MessageBox.Show("이미 공정이 진행 중입니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!form.SimulationRunning)
            {
                if (!form.IsFoupAMounted || !form.IsFoupBMounted)
                {
                    MessageBox.Show("FOUP A/B 장착 상태를 확인해 주세요.", "준비 미완료", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (form.WaferLoadState != MainFormViewModel.WaferLoadStateType.Loading)
                {
                    MessageBox.Show("웨이퍼를 '로딩' 상태로 전환한 뒤 공정을 시작할 수 있습니다.", "준비 미완료", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // EtherCAT 연결 확인 (경고만 표시, 시뮬레이션은 계속 진행)
                if (!form.EthercatConnected)
                {
                    var result = MessageBox.Show(
                        "EtherCAT가 연결되지 않았습니다.\n시뮬레이션 모드로 동작합니다.\n계속하시겠습니까?",
                        "EtherCAT 미연결",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            form.HasAlarm = false;
            if (form.SimulationRunning && form.SimulationPaused)
            {
                form.ResumeSimulation();
            }
            else
            {
                form.StartSimulation();
            }
        }

        /// <summary>
        /// 공정 일시정지 버튼 클릭 이벤트 핸들러
        /// 진행 중인 공정을 일시정지합니다. 하드웨어 모드에서는 새 명령만 중지하고 서보는 유지합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonPause_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }

            if (!form.SimulationRunning || form.SimulationPaused)
            {
                MessageBox.Show("진행 중인 공정이 없습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 하드웨어 모드: 새 명령만 중지 (서보 ON 유지)
            if (form.IsTmHardwareModeAvailable())
            {
                form.AddLogMessage("하드웨어 일시정지: 새 명령 중지 (서보 ON 유지)", "WARN");
            }

            form.PauseSimulation();
        }

        /// <summary>
        /// 공정 정지 버튼 클릭 이벤트 핸들러
        /// 진행 중인 공정을 긴급 정지합니다. 하드웨어 모드에서는 서보와 진공을 즉시 OFF합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonStop_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }

            if (!form.SimulationRunning)
            {
                MessageBox.Show("정지할 공정이 없습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 하드웨어 모드: 긴급 정지
            if (form.EthercatConnected && form.EtherCAT_M != null)
            {
                try
                {
                    // 1. 서보 즉시 OFF
                    form.EtherCAT_M.Axis1_OFF();
                    form.EtherCAT_M.Axis2_OFF();
                    form.IsServoOn = false;
                    
                    // 2. 진공 OFF (웨이퍼가 떨어질 수 있으므로 주의 메시지)
                    form.EtherCAT_M.Digital_Output(14, false); // 진공 OFF
                    form.EtherCAT_M.Digital_Output(15, false); // 배기 OFF
                    
                    form.AddLogMessage("긴급정지: 서보 OFF, 진공 OFF - 장비 상태를 확인하세요!", "ALARM");
                    
                    // 상태 초기화
                    form.TmHardwareInitialized = false;
                    form.TmHardwareActionPending = false;
                    form.TmSettleWaiting = false;
                }
                catch (Exception ex)
                {
                    form.AddLogMessage($"긴급정지 하드웨어 제어 오류: {ex.Message}", "ERROR");
                }
            }

            form.HasAlarm = true;
            form.AbortSimulation("긴급 정지를 실행했습니다. 장비 상태를 확인하세요.", true);
            
            // 긴급정지 후 안내 메시지
            MessageBox.Show(
                "긴급 정지가 실행되었습니다.\n\n" +
                "⚠️ 장비 상태를 확인하세요:\n" +
                "• 서보: OFF 상태\n" +
                "• 진공: OFF 상태\n" +
                "• 실린더: 현재 상태 유지\n\n" +
                "재시작하려면:\n" +
                "1. 장비 상태 확인\n" +
                "2. 실린더 후진 확인\n" +
                "3. 서보 ON → 원점복귀\n" +
                "4. 공정 리셋 후 재시작",
                "긴급 정지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 알람 리셋 버튼 클릭 이벤트 핸들러
        /// 모든 알람 상태를 리셋하고 이전 공정 상태로 복귀합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonResetAlarm_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }

            // 헤더에 알람이 표시되어 있는지 확인
            if (!form.HasHeaderAlarm())
            {
                MessageBox.Show("해제할 알람이 없습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 모든 알람 상태 리셋
            form.HasAlarm = false;
            foreach (var key in form.ChamberAlarmStatus.Keys.ToList())
            {
                form.ChamberAlarmStatus[key] = false;
            }
            
            // verification 알람도 리셋됨을 표시
            form.VerificationAlarmDismissed = true;

            // 알람 리셋: 저장된 이전 상태로 복귀 또는 적절한 상태로 설정
            MainFormViewModel.ProcessState targetState;
            string resetMessage;
            
            // 알람 발생 전 상태가 저장되어 있으면 해당 상태로 복귀
            var stateBeforeAlarm = form._stateBeforeAlarm;
            if (stateBeforeAlarm.HasValue)
            {
                var savedState = stateBeforeAlarm.Value;
                
                // 저장된 상태가 Running이지만 공정이 중단된 경우(AbortSimulation 호출됨)는 Idle로 설정
                // 긴급 정지 등으로 공정이 완전히 중단된 경우를 고려
                if (savedState == MainFormViewModel.ProcessState.Running && !form.SimulationRunning)
                {
                    // 공정이 중단되었으므로 Idle로 설정 (안전상 공정 자동 재개 방지)
                    targetState = MainFormViewModel.ProcessState.Idle;
                    resetMessage = "알람을 리셋했습니다. 공정이 중단되었으므로 대기 상태로 설정되었습니다.";
                }
                else
                {
                    // 저장된 상태로 복귀
                    targetState = savedState;
                    resetMessage = $"알람을 리셋했습니다. 이전 상태({targetState})로 복귀합니다.";
                }
                
                form._stateBeforeAlarm = null; // 복귀 후 저장된 상태 초기화
            }
            else
            {
                // 저장된 상태가 없으면 공정 상태에 따라 결정
                if (form.SimulationRunning)
                {
                    // 공정 중이면 Running 상태로 복귀
                    targetState = MainFormViewModel.ProcessState.Running;
                    resetMessage = "알람을 리셋했습니다. 공정을 계속합니다.";
                }
                else
                {
                    // 공정 중이 아니면 Idle 상태로 복귀
                    targetState = MainFormViewModel.ProcessState.Idle;
                    resetMessage = "알람을 리셋했습니다.";
                }
            }

            // 공정 중이 아닐 때만 시뮬레이션 상태 초기화
            if (!form.SimulationRunning)
            {
                // 시뮬레이션 상태 초기화 (에러 상태 해제)
                if (form.CurrentProcessState == MainFormViewModel.ProcessState.Error)
                {
                    form.InitializeSimulationState();
                }
            }

            // 프로세스 상태를 복귀 상태로 변경
            form.SetProcessState(targetState, resetMessage);
            
            // 로그에 알람 리셋 메시지 추가
            form.AddLogMessage("알람이 리셋되었습니다.", "INFO");
            
            // 헤더 최근 알람 영역 즉시 초기화 (verification 알람 포함)
            form.SetHeaderAlarmIdle();
            
            // UI 업데이트
            form.UpdateSimulationUi();
        }

        /// <summary>
        /// 공정 리셋 버튼 클릭 이벤트 핸들러
        /// 현재 공정을 완전히 초기화합니다. 하드웨어 모드에서는 실린더 후진 및 원점복귀를 수행합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonResetProcess_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }

            string confirmMessage = "현재 공정을 완전히 초기화하시겠습니까?\nTM 대기 상태, FOUP 표시, 챔버 상태 등이 리셋됩니다.";
            
            // 하드웨어 모드 추가 안내
            if (form.EthercatConnected)
            {
                confirmMessage += "\n\n[하드웨어 모드]\n• 실린더 후진 확인\n• 진공 OFF\n• 원점복귀 수행";
            }

            var confirm = MessageBox.Show(
                confirmMessage,
                "공정 리셋 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            form.SimulationController?.StopTimer();
            
            // 진행 중인 작업 취소 및 하드웨어 동작 중지
            if (form.TransferService != null && form.TransferService.CurrentTransfer != null)
            {
                form.AddLogMessage($"공정 리셋: 진행 중인 작업 취소 - {EquipmentRegionHelper.FormatRegionLabel(form.TransferService.CurrentTransfer.Pickup)} → {EquipmentRegionHelper.FormatRegionLabel(form.TransferService.CurrentTransfer.Dropoff)}", "WARN");
            }
            
            // 하드웨어 동작 플래그 리셋 (진행 중인 작업 중지)
            form.TmHardwareActionPending = false;
            form.TmSettleWaiting = false;
            
            // Transfer 큐 및 현재 작업 클리어
            form.TransferService?.ResetToIdle();
            form.TransferService?.ClearQueue();
            
            // 하드웨어 모드: 안전한 초기화 수행
            if (form.EthercatConnected && form.EtherCAT_M != null)
            {
                try
                {
                    form.AddLogMessage("공정 리셋: 하드웨어 초기화 시작", "INFO");
                    
                    // 1. 진공 OFF (웨이퍼 분리)
                    form.EtherCAT_M.Digital_Output(14, false); // 진공 OFF
                    form.EtherCAT_M.Digital_Output(15, false); // 배기 OFF
                    form.AddLogMessage("공정 리셋: 진공 OFF", "INFO");
                    
                    // 2. 실린더 상태 확인
                    bool cylinderRetracted = false;
                    try
                    {
                        cylinderRetracted = form.EtherCAT_M.Digital_Input(12); // 후진 센서
                    }
                    catch (Exception ex)
                    {
                        form.AddLogMessage($"실린더 상태 확인 오류: {ex.Message}", "WARN");
                    }
                    
                    // 3. 실린더 전진 상태면 후진 시도
                    if (!cylinderRetracted)
                    {
                        form.AddLogMessage("공정 리셋: 실린더 후진 시도", "INFO");
                        form.EtherCAT_M.Digital_Output(12, false); // 전진 OFF
                        form.EtherCAT_M.Digital_Output(13, true);  // 후진 ON
                        
                        // 후진 완료 대기 (최대 5초)
                        var timeout = DateTime.Now.AddSeconds(5);
                        while (DateTime.Now < timeout)
                        {
                            try
                            {
                                if (form.EtherCAT_M.Digital_Input(12))
                                {
                                    cylinderRetracted = true;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"실린더 후진 확인 오류: {ex.Message}");
                            }
                            System.Threading.Thread.Sleep(100);
                        }
                        
                        if (!cylinderRetracted)
                        {
                            form.AddLogMessage("공정 리셋: 실린더 후진 타임아웃 - 수동 확인 필요", "WARN");
                            MessageBox.Show(
                                "실린더 후진이 완료되지 않았습니다.\n수동으로 실린더 상태를 확인해주세요.",
                                "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            form.AddLogMessage("공정 리셋: 실린더 후진 완료", "INFO");
                        }
                    }
                    
                    // 4. 서보 ON 상태 확인 및 원점복귀 (BeforeFinal 로직: 명령만 전송, 완료 대기 없음)
                    if (form.IsServoOn && cylinderRetracted)
                    {
                        form.AddLogMessage("공정 리셋: 원점복귀 시작", "INFO");
                        
                        // BeforeFinal 로직: 원점복귀 명령만 전송 (완료 대기 없음)
                        try
                        {
                            form.EtherCAT_M.Axis1_UD_Homming();
                            System.Threading.Thread.Sleep(100);
                            form.EtherCAT_M.Axis2_LR_Homming();
                            
                            form.AddLogMessage("공정 리셋: 원점복귀 명령 전송 완료", "INFO");
                            form.TmHardwareInitialized = true;
                        }
                        catch (Exception homingEx)
                        {
                            form.AddLogMessage($"공정 리셋 원점복귀 오류: {homingEx.Message}", "ERROR");
                        }
                    }
                    else if (!form.IsServoOn)
                    {
                        form.AddLogMessage("공정 리셋: 서보 OFF 상태 - 서보 ON 후 원점복귀 필요", "WARN");
                        form.TmHardwareInitialized = false;
                    }
                    
                    // 상태 초기화 (이미 위에서 리셋했지만 안전을 위해 다시 설정)
                    form.TmHardwareActionPending = false;
                    form.TmSettleWaiting = false;
                    
                    // 하드웨어 모드: 실제 TM 위치를 읽어서 UI에 반영
                    if (form.TmHardwareController != null)
                    {
                        try
                        {
                            form.TmHardwareController.UpdateCurrentPositions();
                            long currentX = form.TmHardwareController.CurrentAxis2Position;
                            long currentY = form.TmHardwareController.CurrentAxis1Position;
                            
                            // 하드웨어 위치를 Region으로 변환
                            var hardwareRegion = EquipmentRegionHelper.DetermineRegionFromPosition(currentX, currentY, form.TmHardwareController.Positions);
                            
                            // TM 위치 업데이트
                            form.TmVisualTarget = hardwareRegion;
                            form.TmCurrentPosition = hardwareRegion;
                            
                            // TM 시각화 업데이트 (하드웨어 모드이므로 하드웨어 업데이트 메서드 사용)
                            form.UpdateTmVisualizationFromHardware();
                            
                            form.AddLogMessage($"공정 리셋: TM 위치 업데이트 완료 - {EquipmentRegionHelper.FormatRegionLabel(hardwareRegion)}", "INFO");
                        }
                        catch (Exception ex)
                        {
                            form.AddLogMessage($"공정 리셋: TM 위치 읽기 오류: {ex.Message}", "WARN");
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.AddLogMessage($"공정 리셋 하드웨어 오류: {ex.Message}", "ERROR");
                }
            }
            
            form.InitializeSimulationState();
            form.SetProcessState(MainFormViewModel.ProcessState.Idle, "공정을 초기화했습니다.");
            form.UpdateSimulationUi();
            form.UpdateTmAnimationIdleTarget();
            form.UpdateServoStatusLabel();
            form.AddLogMessage("사용자가 공정을 수동 리셋했습니다.", "INFO");
        }

        /// <summary>
        /// 레시피 적용 버튼 클릭 이벤트 핸들러
        /// 선택된 레시피를 로드하고 공정 파라미터를 적용합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonApplyRecipe_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }

            var recipeName = form.comboRecipeSelect?.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                MessageBox.Show(form, "적용할 레시피를 선택하세요.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            form.LastSelectedRecipe = recipeName;
            form.AddLogMessage($"레시피 '{recipeName}' 적용을 준비했습니다.", "INFO");
            var list = RecipeRepository.LoadAll();
            var snap = list.FirstOrDefault(r => string.Equals(r.Name, recipeName, StringComparison.Ordinal));
            if (snap == null)
            {
                MessageBox.Show(form, "레시피를 찾을 수 없습니다. 레시피 관리에서 다시 저장해 주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            form.ApplyRecipeFromSnapshot(snap);
            form.RecipeApplied = true; // 레시피 적용 플래그 설정
            // 레시피 적용 후 SV 값만 업데이트 (PV는 공정 시작 시에만 생성되므로 여기서는 SV만 표시)
            foreach (var kvp in form.PmEnvTables)
            {
                if (kvp.Value != null && kvp.Value.Visible)
                {
                    form.UpdatePmEnvironmentTable(kvp.Key);
                }
            }
        }
    }
}

