using System;
using IEG3268_Dll;
using SemiconductorUi.Controllers;

namespace SemiconductorUi.Services
{
    /// <summary>
    /// 하드웨어 초기화, 종료, 연결 상태 관리를 담당하는 클래스
    /// Phase 2.3: 하드웨어 제어 분리
    /// </summary>
    public class HardwareManager : IDisposable
    {
        #region Fields

        private IEG3268 _ethercat;
        private TmHardwareController _tmHardwareController;
        private bool _isEthercatConnected;
        private bool _isTmHardwareInitialized;
        private Action<string, string> _logCallback;
        private bool _disposed = false;

        #endregion

        #region Properties

        /// <summary>
        /// EtherCAT 연결 상태
        /// </summary>
        public bool IsEthercatConnected => _isEthercatConnected;

        /// <summary>
        /// EtherCAT 인스턴스 (읽기 전용)
        /// </summary>
        public IEG3268 EtherCAT => _ethercat;

        /// <summary>
        /// TM 하드웨어 컨트롤러 (읽기 전용)
        /// </summary>
        public TmHardwareController TmHardwareController => _tmHardwareController;

        /// <summary>
        /// TM 하드웨어 초기화 완료 여부
        /// </summary>
        public bool IsTmHardwareInitialized => _isTmHardwareInitialized;

        #endregion

        #region Constructor

        /// <summary>
        /// HardwareManager 생성자
        /// </summary>
        /// <param name="ethercat">EtherCAT 인스턴스</param>
        /// <param name="logCallback">로깅 콜백 함수</param>
        public HardwareManager(IEG3268 ethercat, Action<string, string> logCallback)
        {
            _ethercat = ethercat ?? throw new ArgumentNullException(nameof(ethercat));
            _logCallback = logCallback ?? throw new ArgumentNullException(nameof(logCallback));
            _isEthercatConnected = false;
            _isTmHardwareInitialized = false;
        }

        #endregion

        #region EtherCAT Connection Management

        /// <summary>
        /// EtherCAT 연결 시도
        /// </summary>
        /// <returns>연결 성공 여부</returns>
        public bool ConnectEthercat()
        {
            if (_isEthercatConnected)
            {
                Log("이미 EtherCAT에 연결되어 있습니다.", "INFO");
                return true;
            }

            try
            {
                bool connected = _ethercat.CIFX_50RE_Connect();

                if (connected)
                {
                    _isEthercatConnected = true;

                    // ReadData Timer 설정 및 시작
                    _ethercat.ReadData_Send_Start(300); // Timer interval 300ms
                    _ethercat.ReadData_Timer_Start();

                    Log("EtherCAT 연결 성공 - 실제 장비 제어 모드 활성화", "INFO");
                    return true;
                }
                else
                {
                    Log("EtherCAT 연결 실패", "ERROR");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log($"EtherCAT 연결 오류: {ex.Message}", "ERROR");
                return false;
            }
        }

        /// <summary>
        /// EtherCAT 연결 해제
        /// </summary>
        public void DisconnectEthercat()
        {
            if (!_isEthercatConnected)
            {
                return;
            }

            try
            {
                // TM 하드웨어 컨트롤러 종료
                ShutdownTmHardwareController();

                // ReadData Timer 중지 시도
                // ReadData_Timer_Stop() 메서드가 존재하지 않을 수 있으므로,
                // 리플렉션을 사용하여 가능한 모든 Stop 메서드를 시도합니다.
                TryStopReadDataTimer();

                // EtherCAT 연결 해제
                if (_ethercat != null)
                {
                    _ethercat.CIFX_50RE_Disconnect();
                }

                _isEthercatConnected = false;
                Log("EtherCAT 연결 해제 완료", "INFO");
            }
            catch (Exception ex)
            {
                Log($"EtherCAT 연결 해제 오류: {ex.Message}", "ERROR");
            }
        }

        #endregion

        #region TM Hardware Controller Management

        /// <summary>
        /// TM 하드웨어 컨트롤러 초기화
        /// </summary>
        /// <returns>초기화 성공 여부</returns>
        public bool InitializeTmHardwareController()
        {
            if (_tmHardwareController != null)
            {
                return true; // 이미 초기화됨
            }

            if (!_isEthercatConnected)
            {
                Log("EtherCAT가 연결되지 않아 TM 하드웨어 컨트롤러를 초기화할 수 없습니다.", "WARN");
                return false;
            }

            try
            {
                _tmHardwareController = new TmHardwareController(_ethercat, Log);

                // 위치 설정은 TmPositionSet에 기본값으로 티칭값이 설정되어 있음
                Log("TM 하드웨어 컨트롤러 생성 완료 (티칭값 적용됨)", "INFO");
                Log($"  Chamber A: X={_tmHardwareController.Positions.ChamberA_X}", "INFO");
                Log($"  Chamber B: X={_tmHardwareController.Positions.ChamberB_X}", "INFO");
                Log($"  Chamber C: X={_tmHardwareController.Positions.ChamberC_X}", "INFO");
                Log($"  FOUP A: X={_tmHardwareController.Positions.FoupA_X}", "INFO");
                Log($"  FOUP B: X={_tmHardwareController.Positions.FoupB_X}", "INFO");

                return true;
            }
            catch (Exception ex)
            {
                Log($"TM 하드웨어 컨트롤러 생성 오류: {ex.Message}", "ERROR");
                _tmHardwareController = null;
                return false;
            }
        }

        /// <summary>
        /// TM 하드웨어 초기화 및 원점복귀 수행
        /// </summary>
        /// <returns>초기화 및 원점복귀 성공 여부</returns>
        public bool InitializeAndHomeTmHardware()
        {
            if (_tmHardwareController == null)
            {
                Log("TM 하드웨어 컨트롤러가 없습니다", "ERROR");
                return false;
            }

            try
            {
                // 하드웨어 초기화
                var initResult = _tmHardwareController.Initialize();
                if (!initResult.Success)
                {
                    Log($"TM 초기화 실패: {initResult.ErrorMessage}", "ERROR");
                    return false;
                }

                // 원점복귀
                var homeResult = _tmHardwareController.PerformHoming();
                if (!homeResult.Success)
                {
                    Log($"TM 원점복귀 실패: {homeResult.ErrorMessage}", "ERROR");
                    return false;
                }

                _isTmHardwareInitialized = true;
                Log("TM 하드웨어 초기화 및 원점복귀 완료", "INFO");
                return true;
            }
            catch (Exception ex)
            {
                Log($"TM 하드웨어 초기화 오류: {ex.Message}", "ERROR");
                return false;
            }
        }

        /// <summary>
        /// TM 하드웨어 컨트롤러 종료
        /// </summary>
        public void ShutdownTmHardwareController()
        {
            if (_tmHardwareController == null)
            {
                return;
            }

            try
            {
                // 서보 OFF
                if (_isEthercatConnected && _ethercat != null)
                {
                    _ethercat.Axis1_OFF();
                    _ethercat.Axis2_OFF();
                }

                // 진공 OFF
                if (_isEthercatConnected && _ethercat != null)
                {
                    _ethercat.Digital_Output(14, false); // 진공 OFF
                    _ethercat.Digital_Output(15, false); // 배기 OFF
                }

                _tmHardwareController = null;
                _isTmHardwareInitialized = false;
                Log("TM 하드웨어 컨트롤러 종료 완료", "INFO");
            }
            catch (Exception ex)
            {
                Log($"TM 하드웨어 컨트롤러 종료 오류: {ex.Message}", "ERROR");
            }
        }

        /// <summary>
        /// TM 하드웨어 초기화 상태 리셋
        /// </summary>
        public void ResetTmHardwareInitialized()
        {
            _isTmHardwareInitialized = false;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 로그 기록
        /// </summary>
        private void Log(string message, string level)
        {
            _logCallback?.Invoke(message, level);
        }

        /// <summary>
        /// ReadData Timer 중지 시도 (리플렉션 사용)
        /// ReadData_Timer_Stop() 메서드가 존재하지 않을 수 있으므로,
        /// 여러 가능한 메서드명을 시도합니다.
        /// </summary>
        private void TryStopReadDataTimer()
        {
            if (_ethercat == null)
            {
                return;
            }

            // 가능한 타이머 중지 메서드명 목록
            string[] possibleMethodNames = new string[]
            {
                "ReadData_Timer_Stop",
                "ReadData_Send_Stop",
                "ReadData_Stop",
                "Timer_Stop",
                "StopReadData",
                "StopReadDataTimer"
            };

            bool timerStopped = false;

            foreach (var methodName in possibleMethodNames)
            {
                try
                {
                    var method = _ethercat.GetType().GetMethod(methodName, 
                        System.Reflection.BindingFlags.Public | 
                        System.Reflection.BindingFlags.Instance,
                        null, 
                        System.Type.EmptyTypes, 
                        null);

                    if (method != null && method.GetParameters().Length == 0)
                    {
                        method.Invoke(_ethercat, null);
                        Log($"ReadData Timer 중지 성공: {methodName}()", "INFO");
                        timerStopped = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // 메서드가 없거나 호출 실패 시 무시하고 다음 시도
                    System.Diagnostics.Debug.WriteLine($"Timer 중지 메서드 {methodName} 시도 실패: {ex.Message}");
                }
            }

            if (!timerStopped)
            {
                Log("ReadData Timer 중지 메서드를 찾을 수 없습니다. CIFX_50RE_Disconnect()가 타이머를 자동으로 중지할 수 있습니다.", "WARN");
            }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// 리소스 정리
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 리소스 정리 (protected)
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 해제할지 여부</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 관리되는 리소스 해제
                    DisconnectEthercat();
                    ShutdownTmHardwareController();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}

