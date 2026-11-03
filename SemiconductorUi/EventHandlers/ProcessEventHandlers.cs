using System;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.Hardware;
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

            if (IsEmergencyStopped())
            {
                MessageBox.Show(
                    "비상정지 중입니다.\n\n알람 리셋으로 비상정지를 해제하고, 서보 ON과 원점복귀를 한 뒤 시작하세요.",
                    "비상정지 중", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        /// 긴급 정지 버튼 클릭 이벤트 핸들러
        /// 공정 여부, 로그인 여부와 관계없이 두 축 서보를 끄고 장비 명령을 막습니다.
        /// 진공은 유지합니다. 알람 리셋으로 해제합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonStop_Click(object sender, EventArgs e)
        {
            form.ExecuteEmergencyStop("메인 화면 긴급 정지 버튼");

            MessageBox.Show(
                "비상정지가 실행되었습니다.\n\n" +
                "• 서보: OFF (두 축)\n" +
                "• 진공: 유지 (웨이퍼를 잡고 있으면 그대로)\n" +
                "• 실린더, 도어: 현재 상태 유지\n" +
                "• 장비 명령: 해제 전까지 차단\n\n" +
                "다시 움직이려면:\n" +
                "1. 장비 상태 확인\n" +
                "2. 알람 리셋으로 비상정지 해제\n" +
                "3. 서보 ON → 원점복귀",
                "비상정지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            bool emergencyStopCleared = false;
            if (IsEmergencyStopped())
            {
                var faultedAxes = form.GetFaultedServoAxes();
                string faultNotice = faultedAxes.Count > 0
                    ? $"드라이브 알람이 남아 있어 알람 리셋 신호도 함께 보냅니다:\n{form.DescribeServoAxes(faultedAxes)}\n\n"
                    : "";

                var confirm = MessageBox.Show(
                    "비상정지를 해제하시겠습니까?\n\n" +
                    faultNotice +
                    "해제하기 전에 확인하세요:\n" +
                    "• 장비 근처에 사람이 없는지\n" +
                    "• 웨이퍼, 블레이드, 실린더 위치\n\n" +
                    "해제해도 서보는 꺼진 채로 남습니다.\n" +
                    "해제한 뒤 서보 ON → 원점복귀를 다시 하세요.",
                    "비상정지 해제",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                form.EtherCAT_M.ClearEmergencyStop();
                emergencyStopCleared = true;
                form.AddLogMessage("비상정지 해제 (서보 OFF 유지, 서보 ON과 원점복귀 필요)", "WARN");
                form.ResetServoFaultsAfterEmergencyStopCleared();
            }

            // 헤더에 알람이 표시되어 있는지 확인
            if (!form.HasHeaderAlarm())
            {
                if (!emergencyStopCleared)
                {
                    MessageBox.Show("해제할 알람이 없습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                form.UpdateServoStatusLabel();
                form.UpdateProcessControlButtons();
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
            form.UpdateServoStatusLabel();
            form.UpdateProcessControlButtons();
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

            if (IsEmergencyStopped())
            {
                MessageBox.Show(
                    "비상정지 중에는 공정 리셋을 할 수 없습니다.\n공정 리셋은 진공 OFF, 실린더 후진, 원점복귀를 함께 수행합니다.\n\n먼저 알람 리셋으로 비상정지를 해제하세요.",
                    "비상정지 중", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // 소프트웨어 상태는 즉시 리셋 (UI 블로킹 방지)
            form.InitializeSimulationState();
            form.SetProcessState(MainFormViewModel.ProcessState.Idle, "공정을 초기화했습니다.");
            form.UpdateSimulationUi();
            form.UpdateTmAnimationIdleTarget();
            form.UpdateServoStatusLabel();
            form.AddLogMessage("사용자가 공정을 수동 리셋했습니다.", "INFO");
            
            // 하드웨어 초기화(대기 포함)는 백그라운드에서 수행
            if (form.EthercatConnected && form.EtherCAT_M != null)
            {
                form.AddLogMessage("공정 리셋: 하드웨어 초기화 시작 (백그라운드)", "INFO");
                var ethercat = form.EtherCAT_M;
                var servoOn = form.IsServoOn;
                HardwareOperation.Run(ethercat, () =>
                {
                    try
                    {
                        ethercat.Digital_Output(14, false);
                        ethercat.Digital_Output(15, false);
                        form.AddLogMessage("공정 리셋: 진공 OFF", "INFO");

                        bool cylinderRetracted = false;
                        try { cylinderRetracted = ethercat.Digital_Input(12); }
                        catch (Exception ex) { form.AddLogMessage($"실린더 상태 확인 오류: {ex.Message}", "WARN"); }

                        if (!cylinderRetracted)
                        {
                            form.AddLogMessage("공정 리셋: 실린더 후진 시도", "INFO");
                            ethercat.Digital_Output(12, false);
                            ethercat.Digital_Output(13, true);
                            var timeout = DateTime.Now.AddSeconds(5);
                            while (DateTime.Now < timeout)
                            {
                                ethercat.ThrowIfEmergencyStopped();
                                try
                                {
                                    if (ethercat.Digital_Input(12)) { cylinderRetracted = true; break; }
                                }
                                catch { }
                                System.Threading.Thread.Sleep(100);
                            }

                            if (!cylinderRetracted)
                            {
                                form.AddLogMessage("공정 리셋: 실린더 후진 타임아웃 - 수동 확인 필요", "WARN");
                                form.SafeBeginInvoke(() => MessageBox.Show(
                                    "실린더 후진이 완료되지 않았습니다.\n수동으로 실린더 상태를 확인해주세요.",
                                    "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                            }
                            else
                            {
                                form.AddLogMessage("공정 리셋: 실린더 후진 완료", "INFO");
                            }
                        }

                        if (servoOn && cylinderRetracted)
                        {
                            // 상하 원점복귀가 끝난 것을 확인한 뒤에 좌우를 보냅니다. 실린더는 두 단계 모두 후진이어야 합니다.
                            form.AddLogMessage("공정 리셋: 원점복귀 시작", "INFO");
                            string homingFailReason;
                            bool emergencyStopped = false;
                            try
                            {
                                homingFailReason = new ResetHomingSequence(ethercat).Run(msg => form.AddLogMessage(msg, "INFO"));
                            }
                            catch (EmergencyStopActiveException stopEx)
                            {
                                emergencyStopped = true;
                                homingFailReason = stopEx.Message;
                            }
                            catch (Exception homingEx)
                            {
                                homingFailReason = $"원점복귀 오류: {homingEx.Message}";
                            }

                            if (emergencyStopped)
                            {
                                // 비상정지 안내는 비상정지 쪽에서 이미 띄웁니다.
                                form.AddLogMessage($"공정 리셋 원점복귀 중단: {homingFailReason}", "WARN");
                                form.SafeBeginInvoke(() => form.TmHardwareInitialized = false);
                            }
                            else if (homingFailReason == null)
                            {
                                form.SafeBeginInvoke(() =>
                                {
                                    form.TmHardwareInitialized = true;
                                    form.TmHardwareController?.MarkHomedAfterSuccessfulHoming();
                                });
                            }
                            else
                            {
                                form.AddLogMessage($"공정 리셋 원점복귀 실패: {homingFailReason}", "ERROR");
                                form.SafeBeginInvoke(() =>
                                {
                                    form.TmHardwareInitialized = false;
                                    MessageBox.Show(
                                        $"공정 초기화 중 원점복귀를 끝내지 못했습니다.\n\n{homingFailReason}\n\n장비 상태를 확인한 뒤 원점복귀를 다시 해 주세요.",
                                        "원점복귀 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                });
                            }
                        }
                        else if (!servoOn)
                        {
                            form.SafeBeginInvoke(() =>
                            {
                                form.AddLogMessage("공정 리셋: 서보 OFF 상태 - 서보 ON 후 원점복귀 필요", "WARN");
                                form.TmHardwareInitialized = false;
                            });
                        }

                        form.TmHardwareActionPending = false;
                        form.TmSettleWaiting = false;

                        form.SafeBeginInvoke(() =>
                        {
                            if (form.TmHardwareController != null)
                            {
                                try
                                {
                                    form.TmHardwareController.UpdateCurrentPositions();
                                    long currentX = form.TmHardwareController.CurrentAxis2Position;
                                    long currentY = form.TmHardwareController.CurrentAxis1Position;
                                    var hardwareRegion = EquipmentRegionHelper.DetermineRegionFromPosition(currentX, currentY, form.TmHardwareController.Positions);
                                    form.TmVisualTarget = hardwareRegion;
                                    form.TmCurrentPosition = hardwareRegion;
                                    form.UpdateTmVisualizationFromHardware();
                                    form.AddLogMessage($"공정 리셋: TM 위치 업데이트 완료 - {EquipmentRegionHelper.FormatRegionLabel(hardwareRegion)}", "INFO");
                                }
                                catch (Exception ex)
                                {
                                    form.AddLogMessage($"공정 리셋: TM 위치 읽기 오류: {ex.Message}", "WARN");
                                }
                            }
                            form.UpdateServoStatusLabel();
                        });
                    }
                    catch (Exception ex)
                    {
                        form.AddLogMessage($"공정 리셋 하드웨어 오류: {ex.Message}", "ERROR");
                    }
                });
            }
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

        private bool IsEmergencyStopped()
        {
            return form.EtherCAT_M != null && form.EtherCAT_M.IsEmergencyStopped;
        }
    }
}

