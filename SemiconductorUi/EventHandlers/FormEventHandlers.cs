using System;
using System.Windows.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.EventHandlers
{
    /// <summary>
    /// 폼 이벤트 핸들러
    /// </summary>
    public class FormEventHandlers : Form1EventHandlersBase
    {
        /// <summary>
        /// FormEventHandlers 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public FormEventHandlers(Form1 form) : base(form) { }

        /// <summary>
        /// Form1 폼 종료 이벤트 핸들러
        /// 모든 타이머를 정지하고 하드웨어 연결을 해제합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">FormClosing 이벤트 인자</param>
        public void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 사용자가 창을 닫을 때(X 버튼, Alt+F4) 공정 중이면 닫지 않습니다.
            // 닫으면 확인 없이 진공 OFF → 실린더 후진 → 서보 OFF가 실행되기 때문입니다.
            // Windows 종료·작업 관리자 등 다른 이유로 닫힐 때는 막지 않습니다.
            if (e.CloseReason == CloseReason.UserClosing && form.IsProcessActiveOrStarting())
            {
                e.Cancel = true;
                form.AddLogMessage($"공정 진행 중 프로그램 종료 시도 차단 (사용자: {form.CurrentUser})", "WARN");
                MessageBox.Show(
                    form,
                    "공정이 진행 중(일시정지 포함)이거나 서보 ON·원점복귀 중에는 프로그램을 닫을 수 없습니다.\n\n" +
                    "닫으면 진공 OFF → 실린더 후진 → 서보 OFF가 바로 실행됩니다.\n" +
                    "공정이 끝나거나 '공정 초기화' 후에 닫아 주세요. 급히 멈춰야 하면 긴급 정지를 누르세요.",
                    "프로그램 종료 제한",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 시뮬레이션 서비스 정리 (IDisposable 구현)
                if (form.SimulationService is IDisposable simulationDisposable)
                {
                    try
                    {
                        simulationDisposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"SimulationService Dispose 오류: {ex.Message}");
                    }
                }

                // 헤더 클럭 타이머 정지
                if (form.HeaderClockTimer != null)
                {
                    form.HeaderClockTimer.Stop();
                    // 타이머 이벤트는 Form1에서 직접 관리
                }

                // UI 전용 고속 타이머 정지
                if (form.HardwareUiUpdateTimer != null)
                {
                    form.HardwareUiUpdateTimer.Stop();
                    // 타이머 이벤트는 Form1에서 직접 관리
                }

                // 시뮬레이션 상태 정리
                if (form.SimulationRunning)
                {
                    form.SimulationService?.Abort();
                }

                // DoorAnimationHelper의 모든 타이머 정리
                Form1.DoorAnimationHelper.CleanupAllTimers();

                // TM 하드웨어 컨트롤러 정리
                if (form.TmHardwareController != null)
                {
                    try
                    {
                        form.TmHardwareController.Shutdown();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"TM 하드웨어 컨트롤러 종료 오류: {ex.Message}");
                    }
                }

                // HardwareManager 정리 (IDisposable 구현)
                if (form.HardwareManager is IDisposable hardwareDisposable)
                {
                    try
                    {
                        // HardwareManager.Dispose()가 DisconnectEthercat()과 ShutdownTmHardwareController()를 호출함
                        hardwareDisposable.Dispose();
                        form.IsServoOn = false;
                        
                        if (form.labelEthercatStatus != null)
                        {
                            form.labelEthercatStatus.Text = "EtherCAT: Disconnected";
                            form.labelEthercatStatus.ForeColor = Helpers.UiTheme.TextPrimary;
                            form.labelEthercatStatus.BackColor = Helpers.UiTheme.SurfaceAlt;
                        }
                        
                        form.UpdateServoStatusLabel();
                        
                        if (form.HardwareUiUpdateTimer != null && form.HardwareUiUpdateTimer.Enabled)
                        {
                            form.HardwareUiUpdateTimer.Stop();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"HardwareManager Dispose 오류: {ex.Message}");
                    }
                }
                else if (form.EthercatConnected && form.EtherCAT_M != null)
                {
                    // IDisposable이 아닌 경우 기존 방식으로 처리 (하위 호환성)
                    try
                    {
                        form.HardwareManager?.DisconnectEthercat();
                        form.IsServoOn = false;
                        
                        if (form.labelEthercatStatus != null)
                        {
                            form.labelEthercatStatus.Text = "EtherCAT: Disconnected";
                            form.labelEthercatStatus.ForeColor = Helpers.UiTheme.TextPrimary;
                            form.labelEthercatStatus.BackColor = Helpers.UiTheme.SurfaceAlt;
                        }
                        
                        form.UpdateServoStatusLabel();
                        
                        if (form.HardwareUiUpdateTimer != null && form.HardwareUiUpdateTimer.Enabled)
                        {
                            form.HardwareUiUpdateTimer.Stop();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"EtherCAT 연결 해제 오류: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // 오류가 발생해도 폼 종료는 계속 진행
                System.Diagnostics.Debug.WriteLine($"FormClosing 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 장비 캔버스 패널 리사이즈 이벤트 핸들러
        /// 중앙 장비 레이아웃을 다시 계산합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void PanelEquipmentCanvas_Resize(object sender, EventArgs e)
        {
            form.LayoutCentralEquipment();
        }
    }
}

