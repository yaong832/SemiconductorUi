using System;
using IEG3268_Dll;
using SemiconductorUi.Controls;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// 챔버 하드웨어 제어 관련 Helper 클래스
    /// </summary>
    public static class ChamberHardwareHelper
    {
        /// <summary>
        /// 챔버 도어 제어를 위한 I/O 인덱스 정보
        /// </summary>
        public struct ChamberDoorIoIndices
        {
            public int CloseOutputIndex;
            public int OpenOutputIndex;
        }

        /// <summary>
        /// Region에 따른 챔버 도어 I/O 인덱스 가져오기
        /// </summary>
        /// <param name="region">EquipmentRegion</param>
        /// <returns>도어 I/O 인덱스 (Region이 챔버가 아니면 null)</returns>
        public static ChamberDoorIoIndices? GetChamberDoorIoIndices(EquipmentRegion region)
        {
            switch (region)
            {
                case EquipmentRegion.ChamberA:
                    return new ChamberDoorIoIndices
                    {
                        CloseOutputIndex = 4,
                        OpenOutputIndex = 5
                    };
                case EquipmentRegion.ChamberB:
                    return new ChamberDoorIoIndices
                    {
                        CloseOutputIndex = 7,
                        OpenOutputIndex = 8
                    };
                case EquipmentRegion.ChamberC:
                    return new ChamberDoorIoIndices
                    {
                        CloseOutputIndex = 10,
                        OpenOutputIndex = 11
                    };
                default:
                    return null;
            }
        }

        /// <summary>
        /// 챔버 도어 제어 (EtherCAT)
        /// </summary>
        /// <param name="ethercatDevice">EtherCAT 장치</param>
        /// <param name="region">EquipmentRegion</param>
        /// <param name="open">열림 여부</param>
        /// <returns>성공 여부</returns>
        public static bool ControlChamberDoor(IEG3268 ethercatDevice, EquipmentRegion region, bool open)
        {
            if (ethercatDevice == null)
            {
                return false;
            }

            var ioIndices = GetChamberDoorIoIndices(region);
            if (!ioIndices.HasValue)
            {
                return false;
            }

            try
            {
                var indices = ioIndices.Value;
                
                // EtherTest 기준 실제 동작 (I/O 테이블 명칭과 반대):
                // "상승 SOL" ON = 도어 닫힘, "하강 SOL" ON = 도어 열림
                if (open)
                {
                    ethercatDevice.Digital_Output(indices.CloseOutputIndex, false);  // 닫기 OFF
                    ethercatDevice.Digital_Output(indices.OpenOutputIndex, true);    // 열기 ON
                }
                else
                {
                    ethercatDevice.Digital_Output(indices.OpenOutputIndex, false);   // 열기 OFF
                    ethercatDevice.Digital_Output(indices.CloseOutputIndex, true);   // 닫기 ON
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 챔버 램프 제어를 위한 I/O 인덱스 가져오기
        /// </summary>
        /// <param name="region">EquipmentRegion</param>
        /// <returns>램프 I/O 인덱스 (Region이 챔버가 아니면 -1)</returns>
        public static int GetChamberLampIoIndex(EquipmentRegion region)
        {
            switch (region)
            {
                case EquipmentRegion.ChamberA:
                    return 3;
                case EquipmentRegion.ChamberB:
                    return 6;
                case EquipmentRegion.ChamberC:
                    return 9;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// 챔버 램프 제어 (EtherCAT)
        /// </summary>
        /// <param name="ethercatDevice">EtherCAT 장치</param>
        /// <param name="region">EquipmentRegion</param>
        /// <param name="on">켜기 여부</param>
        /// <returns>성공 여부</returns>
        public static bool ControlChamberLamp(IEG3268 ethercatDevice, EquipmentRegion region, bool on)
        {
            if (ethercatDevice == null)
            {
                return false;
            }

            int outputIndex = GetChamberLampIoIndex(region);
            if (outputIndex < 0)
            {
                return false;
            }

            try
            {
                ethercatDevice.Digital_Output(outputIndex, on);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 도어 열림 센서 입력 인덱스 가져오기
        /// </summary>
        /// <param name="region">EquipmentRegion</param>
        /// <returns>센서 입력 인덱스 (Region이 챔버가 아니면 -1)</returns>
        public static int GetDoorOpenSensorInputIndex(EquipmentRegion region)
        {
            switch (region)
            {
                case EquipmentRegion.ChamberA:
                    return 5;  // Chamber A 도어 열림 센서
                case EquipmentRegion.ChamberB:
                    return 8;  // Chamber B 도어 열림 센서
                case EquipmentRegion.ChamberC:
                    return 11; // Chamber C 도어 열림 센서
                default:
                    return -1;
            }
        }

        /// <summary>
        /// 도어 닫힘 센서 입력 인덱스 가져오기
        /// </summary>
        /// <param name="region">EquipmentRegion</param>
        /// <returns>센서 입력 인덱스 (Region이 챔버가 아니면 -1)</returns>
        public static int GetDoorClosedSensorInputIndex(EquipmentRegion region)
        {
            switch (region)
            {
                case EquipmentRegion.ChamberA:
                    return 4;  // Chamber A 도어 닫힘 센서
                case EquipmentRegion.ChamberB:
                    return 7;  // Chamber B 도어 닫힘 센서
                case EquipmentRegion.ChamberC:
                    return 10; // Chamber C 도어 닫힘 센서
                default:
                    return -1;
            }
        }

        /// <summary>
        /// 도어 열림 센서 상태 확인 (EtherCAT)
        /// </summary>
        /// <param name="ethercatDevice">EtherCAT 장치</param>
        /// <param name="region">EquipmentRegion</param>
        /// <returns>열림 상태 (오류 시 true 반환)</returns>
        public static bool CheckDoorSensorOpen(IEG3268 ethercatDevice, EquipmentRegion region)
        {
            if (ethercatDevice == null)
            {
                return true; // 오류 시 완료로 처리
            }

            int inputIndex = GetDoorOpenSensorInputIndex(region);
            if (inputIndex < 0)
            {
                return true; // FOUP은 도어 없음
            }

            try
            {
                return ethercatDevice.Digital_Input(inputIndex);
            }
            catch
            {
                return true; // 오류 시 완료로 처리
            }
        }

        /// <summary>
        /// 도어 닫힘 센서 상태 확인 (EtherCAT)
        /// </summary>
        /// <param name="ethercatDevice">EtherCAT 장치</param>
        /// <param name="region">EquipmentRegion</param>
        /// <returns>닫힘 상태 (오류 시 true 반환)</returns>
        public static bool CheckDoorSensorClosed(IEG3268 ethercatDevice, EquipmentRegion region)
        {
            if (ethercatDevice == null)
            {
                return true; // 오류 시 완료로 처리
            }

            int inputIndex = GetDoorClosedSensorInputIndex(region);
            if (inputIndex < 0)
            {
                return true; // FOUP은 도어 없음
            }

            try
            {
                return ethercatDevice.Digital_Input(inputIndex);
            }
            catch
            {
                return true; // 오류 시 완료로 처리
            }
        }
    }
}

