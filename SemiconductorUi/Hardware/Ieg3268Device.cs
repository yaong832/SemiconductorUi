using System;
using System.Threading;
using IEG3268_Dll;

namespace SemiconductorUi.Hardware
{
    /// <summary>
    /// IEG3268 DLL을 감싸는 장치 창구.
    ///
    /// DLL은 출력 버퍼(WriteData) 하나에 두 축의 명령과 디지털 출력을 모두 담고
    /// 호출할 때마다 버퍼 전체를 보냅니다. 여러 스레드가 동시에 부르면 명령이 섞이므로
    /// 모든 호출을 하나의 락으로 직렬화합니다.
    ///
    /// 버퍼 위치는 DLL을 분석해 확인한 값입니다(보고서 8장).
    /// - 축1: 제어워드 WriteData[0], 상태워드 ReadData[0..1]
    /// - 축2: 제어워드 WriteData[23], 상태워드 ReadData[20..21]
    /// 제어워드/상태워드 비트는 CiA402 표준을 따릅니다.
    /// </summary>
    public sealed class Ieg3268Device : IEthercatDevice
    {
        internal const int Axis1ControlWordIndex = 0;
        internal const int Axis2ControlWordIndex = 23;
        internal const int Axis1StatusWordIndex = 0;
        internal const int Axis2StatusWordIndex = 20;

        internal const byte ControlWordDisableVoltage = 0x00;
        internal const byte ControlWordQuickStop = 0x02;
        internal const byte ControlWordFaultReset = 0x80;
        internal const byte StatusWordFaultBit = 0x08;

        private const int FaultResetStepDelayMs = 100;

        private readonly IEG3268 _dll;
        private readonly ICifxChannel _channel;
        private readonly object _sync = new object();
        private bool _isConnected;
        private bool _readTimerRegistered;

        public Ieg3268Device(IEG3268 dll)
            : this(dll, new CifxNativeChannel())
        {
        }

        public Ieg3268Device(IEG3268 dll, ICifxChannel channel)
        {
            _dll = dll ?? throw new ArgumentNullException(nameof(dll));
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        #region 연결 / 데이터 수신

        public bool CIFX_50RE_Connect()
        {
            lock (_sync)
            {
                _isConnected = _dll.CIFX_50RE_Connect();
                return _isConnected;
            }
        }

        public void CIFX_50RE_Disconnect()
        {
            lock (_sync)
            {
                _dll.CIFX_50RE_Disconnect();
                _isConnected = false;
            }
        }

        public void ReadData_Send_Start(int interval)
        {
            lock (_sync)
            {
                _dll.ReadData_Send_Start(interval);
            }
        }

        public void ReadData_Timer_Start()
        {
            lock (_sync)
            {
                // DLL은 호출할 때마다 수신 이벤트를 하나씩 더 붙이므로 한 번만 등록합니다.
                if (_readTimerRegistered)
                {
                    return;
                }

                _dll.ReadData_Timer_Start();
                _readTimerRegistered = true;
            }
        }

        #endregion

        #region 디지털 I/O

        public bool Digital_Input(int numbering)
        {
            lock (_sync)
            {
                return _dll.Digital_Input(numbering);
            }
        }

        public void Digital_Output(int numbering, bool value)
        {
            lock (_sync)
            {
                _dll.Digital_Output(numbering, value);
            }
        }

        #endregion

        #region 축 1

        public void Axis1_ON()
        {
            lock (_sync)
            {
                _dll.Axis1_ON();
            }
        }

        public void Axis1_OFF()
        {
            lock (_sync)
            {
                _dll.Axis1_OFF();
            }
        }

        public bool Axis1_Status(string status)
        {
            lock (_sync)
            {
                return _dll.Axis1_Status(status);
            }
        }

        public string Axis1_is_PosData()
        {
            lock (_sync)
            {
                return _dll.Axis1_is_PosData();
            }
        }

        public void Axis1_UD_Config_Update(long acceleration, long deceleration, long maxVelocity, long velocity)
        {
            lock (_sync)
            {
                _dll.Axis1_UD_Config_Update(acceleration, deceleration, maxVelocity, velocity);
            }
        }

        public void Axis1_UD_POS_Update(long position)
        {
            lock (_sync)
            {
                _dll.Axis1_UD_POS_Update(position);
            }
        }

        public void Axis1_UD_Move_Send()
        {
            lock (_sync)
            {
                _dll.Axis1_UD_Move_Send();
            }
        }

        public void Axis1_UD_Homming()
        {
            lock (_sync)
            {
                _dll.Axis1_UD_Homming();
            }
        }

        #endregion

        #region 축 2

        public void Axis2_ON()
        {
            lock (_sync)
            {
                _dll.Axis2_ON();
            }
        }

        public void Axis2_OFF()
        {
            lock (_sync)
            {
                _dll.Axis2_OFF();
            }
        }

        public bool Axis2_Status(string status)
        {
            lock (_sync)
            {
                return _dll.Axis2_Status(status);
            }
        }

        public string Axis2_is_PosData()
        {
            lock (_sync)
            {
                return _dll.Axis2_is_PosData();
            }
        }

        public void Axis2_LR_Config_Update(long acceleration, long deceleration, long maxVelocity, long velocity)
        {
            lock (_sync)
            {
                _dll.Axis2_LR_Config_Update(acceleration, deceleration, maxVelocity, velocity);
            }
        }

        public void Axis2_LR_POS_Update(long position)
        {
            lock (_sync)
            {
                _dll.Axis2_LR_POS_Update(position);
            }
        }

        public void Axis2_LR_Move_Send()
        {
            lock (_sync)
            {
                _dll.Axis2_LR_Move_Send();
            }
        }

        public void Axis2_LR_Homming()
        {
            lock (_sync)
            {
                _dll.Axis2_LR_Homming();
            }
        }

        #endregion

        #region DLL에 없는 추가 기능

        public bool IsAxisFault(int axis)
        {
            // ReadData는 DLL 수신 타이머가 통째로 교체하므로 참조를 한 번만 잡아서 읽습니다.
            byte[] readData = _dll.ReadData;
            int index = GetStatusWordIndex(axis);

            if (readData == null || readData.Length <= index)
            {
                return false;
            }

            return (readData[index] & StatusWordFaultBit) != 0;
        }

        public void QuickStopAll()
        {
            lock (_sync)
            {
                byte[] writeData = _dll.WriteData;
                writeData[Axis1ControlWordIndex] = ControlWordQuickStop;
                writeData[Axis2ControlWordIndex] = ControlWordQuickStop;
                _channel.Write(writeData);
            }
        }

        public void ResetAxisFault(int axis)
        {
            int index = GetControlWordIndex(axis);

            lock (_sync)
            {
                // 알람 리셋은 제어워드 7번 비트의 상승 에지로 동작합니다.
                byte[] writeData = _dll.WriteData;
                writeData[index] = ControlWordDisableVoltage;
                _channel.Write(writeData);
                Thread.Sleep(FaultResetStepDelayMs);

                writeData[index] = ControlWordFaultReset;
                _channel.Write(writeData);
                Thread.Sleep(FaultResetStepDelayMs);

                writeData[index] = ControlWordDisableVoltage;
                _channel.Write(writeData);
            }
        }

        public EthercatCommunicationStatus CheckCommunication()
        {
            lock (_sync)
            {
                if (!_isConnected)
                {
                    return EthercatCommunicationStatus.NotConnected();
                }

                uint busState;
                uint busStateError = _channel.GetBusState(out busState);
                uint ioReadError = _channel.ReadIo(new byte[CifxConstants.IoReadLength]);

                return new EthercatCommunicationStatus(true, busStateError, busState, ioReadError);
            }
        }

        #endregion

        private static int GetControlWordIndex(int axis)
        {
            switch (axis)
            {
                case 1: return Axis1ControlWordIndex;
                case 2: return Axis2ControlWordIndex;
                default: throw new ArgumentOutOfRangeException(nameof(axis), axis, "축 번호는 1 또는 2입니다.");
            }
        }

        private static int GetStatusWordIndex(int axis)
        {
            switch (axis)
            {
                case 1: return Axis1StatusWordIndex;
                case 2: return Axis2StatusWordIndex;
                default: throw new ArgumentOutOfRangeException(nameof(axis), axis, "축 번호는 1 또는 2입니다.");
            }
        }
    }
}
