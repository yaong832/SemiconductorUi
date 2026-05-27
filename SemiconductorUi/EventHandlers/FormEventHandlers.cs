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
                            form.labelEthercatStatus.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
                            form.labelEthercatStatus.BackColor = System.Drawing.Color.FromArgb(170, 170, 180);
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
                            form.labelEthercatStatus.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
                            form.labelEthercatStatus.BackColor = System.Drawing.Color.FromArgb(170, 170, 180);
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

