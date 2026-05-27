using System;
using System.Windows.Forms;
using AppSettings = SemiconductorUi.AppSettings;

namespace SemiconductorUi.EventHandlers
{
    /// <summary>
    /// 하드웨어 연결/제어 관련 이벤트 핸들러
    /// </summary>
    public class HardwareEventHandlers : Form1EventHandlersBase
    {
        /// <summary>
        /// HardwareEventHandlers 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public HardwareEventHandlers(Form1 form) : base(form) { }

        /// <summary>
        /// EtherCAT 연결 버튼 클릭 이벤트 핸들러
        /// HardwareManager를 통해 EtherCAT 연결을 시도하고 TM 하드웨어 컨트롤러를 초기화합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonEthercatConnect_Click(object sender, EventArgs e)
        {
            // 연결 버튼은 "연결"만 수행 (토글 금지)
            if (form.EthercatConnected)
            {
                MessageBox.Show("이미 EtherCAT에 연결되어 있습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // HardwareManager를 통해 EtherCAT 연결
                        bool connected = form.HardwareManager?.ConnectEthercat() ?? false;
                
                if (connected)
                {
                    if (form.labelEthercatStatus != null)
                    {
                        form.labelEthercatStatus.Text = "EtherCAT: Connected";
                        form.labelEthercatStatus.ForeColor = System.Drawing.Color.FromArgb(76, 175, 80);
                        form.labelEthercatStatus.BackColor = System.Drawing.Color.FromArgb(170, 170, 180);
                    }
                    
                    // EtherCAT 연결 시 서보 상태를 명시적으로 OFF로 초기화
                    // (실제 하드웨어 상태는 SyncEquipmentStateToUI에서 확인)
                    form.IsServoOn = false;
                    form.TmHardwareInitialized = false;
                    
                    // 연결 성공 시 실제 장비 상태를 읽어서 UI에 반영
                    try
                    {
                        form.SyncEquipmentStateToUI();
                    }
                    catch (Exception initEx)
                    {
                        form.AddLogMessage($"장비 상태 동기화 오류: {initEx.Message}", "WARN");
                    }

                    // TM 하드웨어 컨트롤러 초기화
                    try
                    {
                        form.InitializeTmHardwareController();
                    }
                    catch (Exception tmEx)
                    {
                        form.AddLogMessage($"TM 하드웨어 컨트롤러 초기화 오류: {tmEx.Message}", "WARN");
                    }

                    // 서보 상태 라벨 업데이트
                    form.UpdateServoStatusLabel();
                    
                    // EtherCAT 연결 시 하드웨어 모니터링을 위해 타이머 시작
                    // SimulationController는 ISimulationService를 통해 접근
                    if (form.SimulationService != null)
                    {
                        form.SimulationService.SetInterval(AppSettings.HardwareModeTickMilliseconds);
                        if (!form.SimulationService.IsTimerRunning)
                        {
                            form.SimulationService.StartTimer();
                        }
                    }
                    
                    // UI 전용 고속 타이머 시작 (50ms 주기로 하드웨어 상태 확인 및 UI 업데이트)
                    if (form.HardwareUiUpdateTimer != null && !form.HardwareUiUpdateTimer.Enabled)
                    {
                        form.HardwareUiUpdateTimer.Start();
                    }
                }
                else
                {
                    if (form.labelEthercatStatus != null)
                    {
                        form.labelEthercatStatus.Text = "EtherCAT: Connection Failed";
                        form.labelEthercatStatus.ForeColor = System.Drawing.Color.FromArgb(244, 67, 54);
                        form.labelEthercatStatus.BackColor = System.Drawing.Color.FromArgb(170, 170, 180);
                    }
                    MessageBox.Show("EtherCAT 연결에 실패했습니다.\n시뮬레이션 모드로 동작합니다.", "연결 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                if (form.labelEthercatStatus != null)
                {
                    form.labelEthercatStatus.Text = "EtherCAT: Error";
                    form.labelEthercatStatus.ForeColor = System.Drawing.Color.FromArgb(244, 67, 54);
                    form.labelEthercatStatus.BackColor = System.Drawing.Color.FromArgb(170, 170, 180);
                }
                form.AddLogMessage($"EtherCAT 연결 오류: {ex.Message}", "ERROR");
                MessageBox.Show($"EtherCAT 연결 중 오류가 발생했습니다: {ex.Message}\n시뮬레이션 모드로 동작합니다.", "연결 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// EtherCAT 연결 해제 버튼 클릭 이벤트 핸들러
        /// HardwareManager를 통해 EtherCAT 연결을 해제하고 관련 타이머를 중지합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonEthercatDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                // HardwareManager를 통해 EtherCAT 연결 해제 (TM 하드웨어 컨트롤러 종료 포함)
                        form.HardwareManager?.DisconnectEthercat();
                
                        form.IsServoOn = false;  // 서보 상태 초기화
                
                if (form.labelEthercatStatus != null)
                {
                    form.labelEthercatStatus.Text = "EtherCAT: Disconnected";
                    form.labelEthercatStatus.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
                    form.labelEthercatStatus.BackColor = System.Drawing.Color.FromArgb(170, 170, 180);
                }
                
                // 서보 상태 라벨 업데이트
                form.UpdateServoStatusLabel();
                
                // 시뮬레이션이 실행 중이 아니면 하드웨어 모니터링 타이머 중지
                        if (!form.SimulationRunning && (form.SimulationService?.IsTimerRunning ?? false))
                        {
                            form.SimulationService?.StopTimer();
                        }
                        
                        // UI 전용 고속 타이머 중지
                        if (form.HardwareUiUpdateTimer != null && form.HardwareUiUpdateTimer.Enabled)
                        {
                            form.HardwareUiUpdateTimer.Stop();
                        }
                
                form.AddLogMessage("EtherCAT 연결 해제됨", "WARN");
            }
            catch (Exception ex)
            {
                form.AddLogMessage($"EtherCAT 연결 해제 오류: {ex.Message}", "ERROR");
            }
        }

        /// <summary>
        /// 서보 ON 버튼 클릭 이벤트 핸들러
        /// 서보모터를 ON하고 파라미터를 설정합니다. 서보 상태를 확인하여 ON 여부를 검증합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonServoOn_Click(object sender, EventArgs e)
        {
            if (!form.EthercatConnected)
            {
                MessageBox.Show("먼저 EtherCAT을 연결해주세요.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 서보 파라미터 설정 (AppSettings에서 읽어옴)
                Int64 velocity = AppSettings.ServoDefaultVelocity;
                Int64 maxVelocity = AppSettings.ServoDefaultMaxVelocity;
                Int64 deceleration = AppSettings.ServoDefaultDeceleration;
                Int64 acceleration = AppSettings.ServoDefaultAcceleration;

                // 서보 OFF 후 파라미터 설정
                form.EtherCAT_M.Axis1_OFF();
                form.EtherCAT_M.Axis2_OFF();
                System.Threading.Thread.Sleep(100);

                form.EtherCAT_M.Axis1_UD_Config_Update(velocity, maxVelocity, deceleration, acceleration);
                form.EtherCAT_M.Axis2_LR_Config_Update(velocity, maxVelocity, deceleration, acceleration);

                // 서보 ON
                form.EtherCAT_M.Axis1_ON();
                form.EtherCAT_M.Axis2_ON();
                
                // 서보 ON 완료 대기 (최대 2초)
                System.Threading.Thread.Sleep(500); // 초기 안정화 대기
                bool servoOnConfirmed = false;
                var servoCheckTimeout = DateTime.Now.AddSeconds(2);
                while (DateTime.Now < servoCheckTimeout)
                {
                    try
                    {
                        // 서보 ON 상태 확인 (위치 데이터가 있으면 서보 ON으로 간주)
                        string axis1Pos = form.EtherCAT_M.Axis1_is_PosData();
                        string axis2Pos = form.EtherCAT_M.Axis2_is_PosData();
                        bool hasPosition = !string.IsNullOrEmpty(axis1Pos) && axis1Pos != "-" &&
                                          !string.IsNullOrEmpty(axis2Pos) && axis2Pos != "-";
                        
                        if (hasPosition)
                        {
                            servoOnConfirmed = true;
                            break;
                        }
                    }
                    catch (Exception checkEx)
                    {
                        // 상태 확인 오류는 무시하고 계속 시도
                        System.Diagnostics.Debug.WriteLine($"서보 ON 상태 확인 오류: {checkEx.Message}");
                    }
                    System.Threading.Thread.Sleep(100);
                }
                
                if (servoOnConfirmed)
                {
                    form.IsServoOn = true;  // 서보 ON 상태 플래그 설정
                    form.AddLogMessage("서보모터 ON - 파라미터 설정 완료", "INFO");
                }
                else
                {
                    form.IsServoOn = false;
                    form.AddLogMessage("서보모터 ON 명령 전송 완료, 하지만 상태 확인 실패 - 수동 확인 필요", "WARN");
                    MessageBox.Show(
                        "서보 ON 명령을 전송했지만 상태 확인에 실패했습니다.\n\n" +
                        "확인 사항:\n" +
                        "1. 서보 모터 전원 확인\n" +
                        "2. EtherCAT 연결 상태 확인\n" +
                        "3. 서보 모터 상태 수동 확인",
                        "서보 ON 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                
                form.UpdateServoStatusLabel();
            }
            catch (Exception ex)
            {
                form.AddLogMessage($"서보 ON 오류: {ex.Message}", "ERROR");
                MessageBox.Show($"서보 ON 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 서보 OFF 버튼 클릭 이벤트 핸들러
        /// 서보모터를 OFF하고 원점복귀 상태를 초기화합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonServoOff_Click(object sender, EventArgs e)
        {
            if (!form.EthercatConnected)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                form.EtherCAT_M.Axis1_OFF();
                form.EtherCAT_M.Axis2_OFF();

                        form.IsServoOn = false;  // 서보 OFF 상태 플래그 설정
                form.TmHardwareInitialized = false;  // 원점복귀 상태도 초기화
                form.AddLogMessage("서보모터 OFF", "INFO");
                form.UpdateServoStatusLabel();
            }
            catch (Exception ex)
            {
                form.AddLogMessage($"서보 OFF 오류: {ex.Message}", "ERROR");
                MessageBox.Show($"서보 OFF 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 원점복귀 버튼 클릭 이벤트 핸들러
        /// 실린더가 후진 상태인지 확인한 후 원점복귀를 수행합니다. 비동기로 실행되어 UI 블로킹을 방지합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonServoHome_Click(object sender, EventArgs e)
        {
            if (!form.EthercatConnected)
            {
                MessageBox.Show("먼저 EtherCAT을 연결해주세요.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 실린더 상태 확인 - 좌우 이동 전 필수
                bool cylinderRetracted = form.EtherCAT_M.Digital_Input(12); // 후진 센서
                if (!cylinderRetracted)
                {
                    MessageBox.Show("웨이퍼 이송 실린더가 전진되어 있습니다.\n실린더 전진 상태에서는 좌우 이동이 불가합니다.\n먼저 실린더를 후진해주세요.", "안전 경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 비동기로 원점복귀 실행 (UI 블로킹 방지)
                System.Threading.Tasks.Task.Run(() => form.PerformHomingSequence());
            }
            catch (Exception ex)
            {
                form.AddLogMessage($"원점복귀 오류: {ex.Message}", "ERROR");
                MessageBox.Show($"원점복귀 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

