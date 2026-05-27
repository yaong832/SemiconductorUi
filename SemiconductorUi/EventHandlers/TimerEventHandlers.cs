using System;
using System.Drawing;
using SemiconductorUi.ViewModels;

namespace SemiconductorUi.EventHandlers
{
    /// <summary>
    /// 타이머 관련 이벤트 핸들러
    /// </summary>
    public class TimerEventHandlers : Form1EventHandlersBase
    {
        /// <summary>
        /// TimerEventHandlers 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public TimerEventHandlers(Form1 form) : base(form) { }

        /// <summary>
        /// 하드웨어 UI 업데이트 타이머 틱 이벤트 핸들러
        /// 하드웨어 모드에서 실제 하드웨어 상태를 읽어서 UI를 업데이트합니다. (50ms 주기)
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void HardwareUiUpdateTimer_Tick(object sender, EventArgs e)
        {
            // 하드웨어 모드이고 EtherCAT 연결되어 있을 때만 UI 업데이트
            if (form.IsTmHardwareModeAvailable() && form.EthercatConnected)
            {
                try
                {
                    // 실제 하드웨어 상태를 읽어서 UI 업데이트
                    form.UpdateTmVisualizationFromHardware();
                    
                    // 주기적으로 서보 상태 동기화 (5초마다)
                    if (form._hardwareSyncCounter % 100 == 0) // 50ms * 100 = 5초
                    {
                        form.UpdateServoStatusLabel();
                    }
                    form._hardwareSyncCounter++;
                }
                catch (Exception ex)
                {
                    // 오류 발생 시 로그 기록 (로그 스팸 방지를 위해 첫 오류만 기록)
                    // 연속 오류는 Debug.WriteLine만 사용
                    if (!form._lastHardwareUiUpdateErrorLogged)
                    {
                        form.AddLogMessage($"UI 업데이트 타이머 오류: {ex.Message}", "WARN");
                        form._lastHardwareUiUpdateErrorLogged = true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"UI 업데이트 타이머 오류 (반복): {ex.Message}");
                    }
                }
            }
            else
            {
                // 하드웨어 모드가 아니거나 연결 해제된 경우, 에러 로그 플래그 리셋
                form._lastHardwareUiUpdateErrorLogged = false;
                form._hardwareSyncCounter = 0;
            }
        }

        /// <summary>
        /// 헤더 시계 타이머 틱 이벤트 핸들러
        /// 헤더의 현재 시각을 업데이트합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void HeaderClockTimer_Tick(object sender, EventArgs e)
        {
            form.UpdateHeaderClock();
        }

        /// <summary>
        /// 시뮬레이션 타이머 틱 이벤트 핸들러
        /// 하드웨어 모드와 시뮬레이션 모드를 구분하여 공정 로직을 처리합니다.
        /// 하드웨어 모드: 실제 시간 기반 공정 진행, 시뮬레이션 모드: 틱 기반 공정 진행
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void SimulationTimer_Tick(object sender, EventArgs e)
        {
            // EtherCAT 연결 시 서보 상태 라벨 항상 업데이트
            if (form.EthercatConnected)
            {
                form.UpdateServoStatusLabel();
            }
            
            if (!form.SimulationRunning || form.SimulationPaused)
            {
                // 하드웨어 모드: 일시정지 중에도 실제 위치 표시
                if (form.IsTmHardwareModeAvailable())
                {
                    form.UpdateTmVisualizationFromHardware();
                }
                return;
            }

            // 하드웨어 모드와 시뮬레이션 모드 분리
            if (form.IsTmHardwareModeAvailable())
            {
                // ========== 하드웨어 모드 ==========
                // 핵심 변경: Phase는 로직 제어용으로만 사용하고, UI는 항상 실제 하드웨어 상태 사용
                // 이렇게 하면 UI가 실제 하드웨어 움직임과 완벽히 동기화됨
                
                // 경과 시간 증가 (UI 표시용)
                form.SimulationController?.IncrementElapsedTime();
                
                // 하드웨어 모드: 실제 장비가 공정을 진행하지만, UI 추적을 위해 시간 기반으로 공정 시간 감소
                form.DecrementChamberTime(form.ChamberAState);
                form.DecrementChamberTime(form.ChamberBState);
                form.DecrementChamberTime(form.ChamberCState);
                
                // 최적화: 스케줄링을 ProcessTm() 전에 수행
                // → 다음 작업을 먼저 큐에 넣고, ProcessTm()에서 바로 시작
                bool tmBusyInHardwareMode = form.TmHardwareActionPending;
                if (!tmBusyInHardwareMode)
                {
                    // 우선순위: 완료 웨이퍼 빼기 > A→B/C 이송 > 새 웨이퍼 투입
                    form.TryScheduleTransferToFoupB(form.ChamberBState);
                    form.TryScheduleTransferToFoupB(form.ChamberCState);
                    form.TryScheduleTransferFromChamberA();
                    form.TryScheduleLoadFromFoupA();
                }
                
                // TM 동작 처리 (스케줄링된 작업 바로 시작)
                // Phase는 로직 제어용으로만 사용 (공정 진행, 다음 단계 결정 등)
                form.ProcessTm();
                
                // UI 업데이트는 별도의 고속 타이머(HardwareUiUpdateTimer, 50ms)에서 처리됨
                // SimulationTimer는 로직 처리만 담당하고, UI는 HardwareUiUpdateTimer가 담당
                
                // 공정 완료 확인
                if ((form.foupManager?.GetFoupBCount() ?? 0) - form.FoupBCompletedBaseline >= form.GetActiveTarget()
                    && form.ChamberAState.CurrentWafer == null
                    && form.ChamberBState.CurrentWafer == null
                    && form.ChamberCState.CurrentWafer == null
                    && (form.TransferController?.QueueCount ?? 0) == 0
                    && form.CurrentTransfer == null)
                {
                    if (form.IsTmHardwareModeAvailable())
                    {
                        form.AddLogMessage("공정 종료 - TM 원점복귀 수행", "INFO");
                        form.PerformShutdownHoming();
                    }
                    
                    form.SimulationController?.Stop();
                    
                    // 하드웨어 모드와 시뮬레이션 모드 구분하여 로그 메시지 표시
                    if (form.IsTmHardwareModeAvailable())
                    {
                        form.AddLogMessage("하드웨어 모드 공정 완료", "INFO");
                        form.SetProcessState(MainFormViewModel.ProcessState.Idle, "하드웨어 모드 공정이 완료되었습니다.");
                    }
                    else
                    {
                        form.AddLogMessage("시뮬레이션 완료", "INFO");
                        form.SetProcessState(MainFormViewModel.ProcessState.Idle, "시뮬레이션이 완료되었습니다.");
                    }
                    
                    form.UpdateSimulationUi();
                }
            }
            else
            {
                // ========== 시뮬레이션 모드 ==========
                form.mainLampBlinkState = !form.mainLampBlinkState;
                form.UpdateMainLampColors(form.mainLampBlinkState ? Color.LimeGreen : (Color?)null, false, false);
                form.AdvanceSimulation();
                
                // 시뮬레이션 모드: Phase 기반으로 TM 시각화 업데이트
                form.UpdateTmVisualization();
            }
            
            form.CaptureTrendSampleTick();
            form.UpdateSimulationUi();
            
            // 환경 정보 표 실시간 업데이트 (항상 표시 중)
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

