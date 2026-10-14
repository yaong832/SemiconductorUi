namespace SemiconductorUi.Hardware
{
    /// <summary>
    /// EtherCAT 장치 접근 창구.
    /// 기존 IEG3268 DLL과 같은 이름의 메서드를 그대로 두어 호출부를 바꾸지 않고,
    /// DLL에 없는 기능(서보 알람, 감속 정지, 통신 상태 확인)을 추가로 제공합니다.
    /// 나중에 DLL을 자체 구현으로 교체할 때 이 인터페이스만 다시 구현하면 됩니다.
    /// </summary>
    public interface IEthercatDevice
    {
        #region 연결 / 데이터 수신

        bool CIFX_50RE_Connect();
        void CIFX_50RE_Disconnect();

        /// <summary>
        /// 입력 데이터 수신 주기 설정 및 시작 (ms)
        /// </summary>
        void ReadData_Send_Start(int interval);

        /// <summary>
        /// 입력 데이터 수신 타이머 등록. 여러 번 호출해도 한 번만 등록됩니다.
        /// </summary>
        void ReadData_Timer_Start();

        #endregion

        #region 디지털 I/O

        bool Digital_Input(int numbering);
        void Digital_Output(int numbering, bool value);

        /// <summary>
        /// 마지막으로 보낸 디지털 출력 값. DLL로는 실제 출력 단자를 다시 읽을 수 없어서
        /// 램프·도어처럼 출력으로만 제어하는 장치의 상태 표시는 이 값을 씁니다.
        /// Digital_Input과 번호 체계가 다르므로 섞어 쓰면 안 됩니다.
        /// </summary>
        bool GetDigitalOutput(int numbering);

        #endregion

        #region 축 1 (상하, UD)

        void Axis1_ON();
        void Axis1_OFF();
        bool Axis1_Status(string status);
        string Axis1_is_PosData();
        void Axis1_UD_Config_Update(long acceleration, long deceleration, long maxVelocity, long velocity);
        void Axis1_UD_POS_Update(long position);
        void Axis1_UD_Move_Send();
        void Axis1_UD_Homming();

        #endregion

        #region 축 2 (좌우, LR)

        void Axis2_ON();
        void Axis2_OFF();
        bool Axis2_Status(string status);
        string Axis2_is_PosData();
        void Axis2_LR_Config_Update(long acceleration, long deceleration, long maxVelocity, long velocity);
        void Axis2_LR_POS_Update(long position);
        void Axis2_LR_Move_Send();
        void Axis2_LR_Homming();

        #endregion

        #region DLL에 없는 추가 기능

        /// <summary>
        /// 서보 드라이브 알람(Fault) 상태. 상태워드 3번 비트를 읽습니다.
        /// </summary>
        /// <param name="axis">1 = 상하, 2 = 좌우</param>
        bool IsAxisFault(int axis);

        /// <summary>
        /// 두 축 모두 감속 정지(Quick Stop)합니다. 서보 OFF와 달리 드라이브가 감속하며 멈춥니다.
        /// 다시 움직이려면 AxisN_ON()으로 서보를 다시 켜야 합니다.
        /// </summary>
        void QuickStopAll();

        /// <summary>
        /// 서보 드라이브 알람 리셋. 리셋 후 AxisN_ON()으로 서보를 다시 켜야 합니다.
        /// </summary>
        /// <param name="axis">1 = 상하, 2 = 좌우</param>
        void ResetAxisFault(int axis);

        /// <summary>
        /// 드라이버 에러 코드를 포함한 실제 통신 상태를 확인합니다.
        /// </summary>
        EthercatCommunicationStatus CheckCommunication();

        #endregion
    }
}
