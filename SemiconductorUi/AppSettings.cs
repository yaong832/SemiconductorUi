using System;
using System.Configuration;
using System.Drawing;

namespace SemiconductorUi
{
    /// <summary>
    /// 애플리케이션 설정 관리 클래스
    /// App.config에서 설정값을 읽어옴
    /// </summary>
    public static class AppSettings
    {
        #region 기본 설정

        /// <summary>
        /// 기본 사용자명
        /// </summary>
        public static string DefaultUsername => GetSetting("DefaultUsername", "admin");

        /// <summary>
        /// 기본 비밀번호 (사용 안 함 - 보안상 제거 권장)
        /// </summary>
        [Obsolete("보안상 하드코딩된 기본 비밀번호 사용을 권장하지 않습니다.")]
        public static string DefaultPassword => GetSetting("DefaultPassword", "");

        /// <summary>
        /// 데모 모드 활성화 여부
        /// </summary>
        public static bool DemoModeEnabled => GetSetting("DemoModeEnabled", false);

        /// <summary>
        /// 데모 모드 자동 시작 여부
        /// </summary>
        public static bool DemoModeAutoStart => GetSetting("DemoModeAutoStart", false);

        #endregion

        #region UI 설정

        /// <summary>
        /// 최대 로그 항목 수
        /// </summary>
        public static int MaxLogEntries => GetSetting("MaxLogEntries", 200);

        /// <summary>
        /// FOUP 최대 용량
        /// </summary>
        public static int MaxFoupCapacity => GetSetting("MaxFoupCapacity", 5);

        #endregion

        #region UI 색상 설정

        /// <summary>
        /// FOUP 패널 로딩 색상 (연한 녹색)
        /// </summary>
        public static Color FoupPanelLoadingColor => GetColorSetting("FoupPanelLoadingColor", 180, 220, 180);

        /// <summary>
        /// FOUP 패널 언로딩 색상 (연한 파란색)
        /// </summary>
        public static Color FoupPanelUnloadingColor => GetColorSetting("FoupPanelUnloadingColor", 180, 200, 230);

        /// <summary>
        /// FOUP 패널 경고 색상 (연한 주황색)
        /// </summary>
        public static Color FoupPanelAlertColor => GetColorSetting("FoupPanelAlertColor", 255, 220, 180);

        /// <summary>
        /// 다이얼로그 배경 색상 (매우 밝은 회색)
        /// </summary>
        public static Color DialogBackgroundColor => GetColorSetting("DialogBackgroundColor", 250, 250, 255);

        /// <summary>
        /// 텍스트 색상 (어두운 회색)
        /// </summary>
        public static Color TextColor => GetColorSetting("TextColor", 40, 40, 40);

        /// <summary>
        /// 성공 색상 (녹색)
        /// </summary>
        public static Color SuccessColor => GetColorSetting("SuccessColor", 76, 175, 80);

        /// <summary>
        /// 패널 배경 색상 (밝은 회색)
        /// </summary>
        public static Color PanelBackgroundColor => GetColorSetting("PanelBackgroundColor", 220, 220, 225);

        /// <summary>
        /// 기본 배경 색상 (밝은 회색)
        /// </summary>
        public static Color DefaultBackgroundColor => GetColorSetting("DefaultBackgroundColor", 245, 245, 250);

        /// <summary>
        /// 구분선 색상 (밝은 회색)
        /// </summary>
        public static Color SeparatorLineColor => GetColorSetting("SeparatorLineColor", 200, 200, 210);

        /// <summary>
        /// 웨이퍼 브러시 색상 (MediumBlue 투명도 180)
        /// </summary>
        public static Color WaferBrushColor => GetColorSetting("WaferBrushColor", 180, 0, 0, 205); // Alpha=180, MediumBlue RGB (0,0,205)

        /// <summary>
        /// 웨이퍼 펜 색상 (MediumBlue 투명도 150)
        /// </summary>
        public static Color WaferPenColor => GetColorSetting("WaferPenColor", 150, 0, 0, 205); // Alpha=150, MediumBlue RGB (0,0,205)

        /// <summary>
        /// 테두리 색상 (밝은 회색)
        /// </summary>
        public static Color BorderColor => GetColorSetting("BorderColor", 220, 220, 230);

        /// <summary>
        /// 공정 초반 웨이퍼 색상 (파란색)
        /// </summary>
        public static Color WaferColorEarlyProcess => GetColorSetting("WaferColorEarlyProcess", 210, 230, 255);

        /// <summary>
        /// 공정 후반 웨이퍼 색상 (보라색)
        /// </summary>
        public static Color WaferColorLateProcess => GetColorSetting("WaferColorLateProcess", 210, 190, 255);

        /// <summary>
        /// 램프 빨간색 (밝은 상태)
        /// </summary>
        public static Color LampRedBright => GetColorSetting("LampRedBright", 210, 60, 50);

        /// <summary>
        /// 램프 황색 (밝은 상태)
        /// </summary>
        public static Color LampYellowBright => GetColorSetting("LampYellowBright", 230, 190, 40);

        /// <summary>
        /// 램프 녹색 (밝은 상태)
        /// </summary>
        public static Color LampGreenBright => GetColorSetting("LampGreenBright", 55, 180, 90);

        /// <summary>
        /// 램프 빨간색 (어두운 상태)
        /// </summary>
        public static Color LampRedDark => GetColorSetting("LampRedDark", 50, 15, 12);

        /// <summary>
        /// 램프 황색 (어두운 상태)
        /// </summary>
        public static Color LampYellowDark => GetColorSetting("LampYellowDark", 60, 50, 15);

        /// <summary>
        /// 램프 녹색 (어두운 상태)
        /// </summary>
        public static Color LampGreenDark => GetColorSetting("LampGreenDark", 20, 60, 30);

        #endregion

        #region 타이머 설정

        /// <summary>
        /// 시뮬레이션 모드 타이머 주기 (밀리초)
        /// </summary>
        public static int SimulationTickMilliseconds => GetSetting("SimulationTickMilliseconds", 1000);

        /// <summary>
        /// 하드웨어 모드 타이머 주기 (밀리초)
        /// </summary>
        public static int HardwareModeTickMilliseconds => GetSetting("HardwareModeTickMilliseconds", 150);

        /// <summary>
        /// 챔버 공정 시간 처리 속도 (초/틱)
        /// </summary>
        public static double ChamberSecondsPerTick => GetSetting("ChamberSecondsPerTick", 1.0);

        #endregion

        #region TM (Transfer Module) 설정

        /// <summary>
        /// TM 픽업 지속 시간 (틱)
        /// </summary>
        public static int TmPickupDurationTicks => GetSetting("TmPickupDurationTicks", 3);

        /// <summary>
        /// TM 픽업 후퇴 시간 (틱)
        /// </summary>
        public static int TmPickupRetractTicks => GetSetting("TmPickupRetractTicks", 2);

        /// <summary>
        /// TM 드롭오프 지속 시간 (틱)
        /// </summary>
        public static int TmDropoffDurationTicks => GetSetting("TmDropoffDurationTicks", 3);

        /// <summary>
        /// TM 드롭오프 후퇴 시간 (틱)
        /// </summary>
        public static int TmDropoffRetractTicks => GetSetting("TmDropoffRetractTicks", 2);

        /// <summary>
        /// TM 단거리 이동 시간 (틱)
        /// </summary>
        public static int TmMoveShortTicks => GetSetting("TmMoveShortTicks", 3);

        /// <summary>
        /// TM 중거리 이동 시간 (틱)
        /// </summary>
        public static int TmMoveMediumTicks => GetSetting("TmMoveMediumTicks", 5);

        /// <summary>
        /// TM 장거리 이동 시간 (틱)
        /// </summary>
        public static int TmMoveLongTicks => GetSetting("TmMoveLongTicks", 8);

        /// <summary>
        /// TM 도어 동작 시간 (틱)
        /// </summary>
        public static int TmDoorActionTicks => GetSetting("TmDoorActionTicks", 2);

        /// <summary>
        /// 도어 열림 대기 틱 수
        /// </summary>
        public static int DoorOpenWaitTicks => GetSetting("DoorOpenWaitTicks", 4);

        /// <summary>
        /// 도어 닫힘 대기 틱 수
        /// </summary>
        public static int DoorCloseWaitTicks => GetSetting("DoorCloseWaitTicks", 4);

        #endregion

        #region 서보모터 파라미터 설정

        /// <summary>
        /// 서보모터 기본 속도
        /// </summary>
        public static long ServoDefaultVelocity => GetSetting("ServoDefaultVelocity", 1000000L);

        /// <summary>
        /// 서보모터 최대 속도
        /// </summary>
        public static long ServoDefaultMaxVelocity => GetSetting("ServoDefaultMaxVelocity", 1000000L);

        /// <summary>
        /// 서보모터 감속도
        /// </summary>
        public static long ServoDefaultDeceleration => GetSetting("ServoDefaultDeceleration", 100000000L);

        /// <summary>
        /// 서보모터 가속도
        /// </summary>
        public static long ServoDefaultAcceleration => GetSetting("ServoDefaultAcceleration", 1000000L);

        /// <summary>
        /// 서보모터 원점복귀 속도
        /// </summary>
        public static long ServoHomingVelocity => GetSetting("ServoHomingVelocity", 300000L);

        /// <summary>
        /// 서보모터 원점복귀 최대 속도
        /// </summary>
        public static long ServoHomingMaxVelocity => GetSetting("ServoHomingMaxVelocity", 500000L);

        /// <summary>
        /// 서보 XY 동시 이동 타임아웃 (밀리초)
        /// </summary>
        public static int ServoXyMoveTimeoutMs => GetSetting("ServoXyMoveTimeoutMs", 5000);

        /// <summary>
        /// 서보 이동 최소 대기 시간 (밀리초)
        /// </summary>
        public static int ServoMoveMinWaitMs => GetSetting("ServoMoveMinWaitMs", 500);

        /// <summary>
        /// 서보 Axis1 최소 대기 시간 (밀리초)
        /// </summary>
        public static int ServoAxis1MinWaitMs => GetSetting("ServoAxis1MinWaitMs", 300);

        #endregion

        #region 하드웨어 타임아웃 설정

        /// <summary>
        /// TM 하드웨어 동작 타임아웃 (밀리초)
        /// </summary>
        public static int TmHardwareActionTimeoutMs => GetSetting("TmHardwareActionTimeoutMs", 30000);

        /// <summary>
        /// 실린더 동작 타임아웃 (밀리초)
        /// </summary>
        public static int CylinderActionTimeoutMs => GetSetting("CylinderActionTimeoutMs", 5000);

        /// <summary>
        /// 서보 이동 타임아웃 (밀리초)
        /// </summary>
        public static int ServoMoveTimeoutMs => GetSetting("ServoMoveTimeoutMs", 3000);

        /// <summary>
        /// 서보 안정화 지연 시간 (밀리초)
        /// </summary>
        public static int ServoSettleDelayMs => GetSetting("ServoSettleDelayMs", 50);

        /// <summary>
        /// 실린더 안정화 지연 시간 (밀리초)
        /// </summary>
        public static int CylinderSettleDelayMs => GetSetting("CylinderSettleDelayMs", 50);

        /// <summary>
        /// 진공 ON 안정화 지연 시간 (밀리초)
        /// </summary>
        public static int VacuumOnSettleDelayMs => GetSetting("VacuumOnSettleDelayMs", 100);

        /// <summary>
        /// 진공 OFF 안정화 지연 시간 (밀리초)
        /// </summary>
        public static int VacuumOffSettleDelayMs => GetSetting("VacuumOffSettleDelayMs", 100);

        /// <summary>
        /// 도어 안정화 지연 시간 (밀리초)
        /// </summary>
        public static int DoorSettleDelayMs => GetSetting("DoorSettleDelayMs", 30);

        /// <summary>
        /// 하드웨어 폴링 주기 (밀리초)
        /// 서보 이동, 실린더 동작 등의 상태 확인 주기
        /// </summary>
        public static int HardwarePollingIntervalMs => GetSetting("HardwarePollingIntervalMs", 30);

        /// <summary>
        /// 서보 초기화 지연 시간 (밀리초)
        /// 서보모터 OFF 후 ON 전 대기 시간
        /// </summary>
        public static int ServoInitDelayMs => GetSetting("ServoInitDelayMs", 100);

        /// <summary>
        /// 서보모터 ON 후 대기 시간 (밀리초)
        /// 서보모터 ON 명령 후 안정화 대기 시간
        /// </summary>
        public static int ServoOnDelayMs => GetSetting("ServoOnDelayMs", 200);

        /// <summary>
        /// 원점복귀 폴링 주기 (밀리초)
        /// 원점복귀 상태 확인 주기
        /// </summary>
        public static int HomingPollingIntervalMs => GetSetting("HomingPollingIntervalMs", 100);

        #endregion

        #region 알람 임계값 기본 설정

        /// <summary>
        /// 온도 경고 임계값 (°C)
        /// </summary>
        public static double AlarmTempWarn => GetSetting("AlarmTempWarn", 2.0);

        /// <summary>
        /// 온도 알람 임계값 (°C)
        /// </summary>
        public static double AlarmTempAlarm => GetSetting("AlarmTempAlarm", 5.0);

        /// <summary>
        /// 압력 경고 비율 (SV 대비)
        /// </summary>
        public static double AlarmPressWarnRatio => GetSetting("AlarmPressWarnRatio", 0.2);

        /// <summary>
        /// 압력 알람 비율 (SV 대비)
        /// </summary>
        public static double AlarmPressAlarmRatio => GetSetting("AlarmPressAlarmRatio", 0.5);

        /// <summary>
        /// 압력 경고 절대값 (Torr)
        /// </summary>
        public static double AlarmPressWarnAbs => GetSetting("AlarmPressWarnAbs", 3.0);

        /// <summary>
        /// 압력 알람 절대값 (Torr)
        /// </summary>
        public static double AlarmPressAlarmAbs => GetSetting("AlarmPressAlarmAbs", 10.0);

        /// <summary>
        /// RF 경고 비율 (SV 대비)
        /// </summary>
        public static double AlarmRfWarnRatio => GetSetting("AlarmRfWarnRatio", 0.10);

        /// <summary>
        /// RF 알람 비율 (SV 대비)
        /// </summary>
        public static double AlarmRfAlarmRatio => GetSetting("AlarmRfAlarmRatio", 0.20);

        /// <summary>
        /// 가스 경고 절대값 (sccm)
        /// </summary>
        public static double AlarmGasWarn => GetSetting("AlarmGasWarn", 5.0);

        /// <summary>
        /// 가스 알람 절대값 (sccm)
        /// </summary>
        public static double AlarmGasAlarm => GetSetting("AlarmGasAlarm", 10.0);

        /// <summary>
        /// 가스 누설 경고 (sccm)
        /// </summary>
        public static double AlarmGasLeakWarn => GetSetting("AlarmGasLeakWarn", 1.0);

        /// <summary>
        /// 가스 누설 알람 (sccm)
        /// </summary>
        public static double AlarmGasLeakAlarm => GetSetting("AlarmGasLeakAlarm", 3.0);

        #endregion

        #region 설정 읽기 헬퍼 메서드

        /// <summary>
        /// 문자열 설정값 읽기
        /// </summary>
        private static string GetSetting(string key, string defaultValue)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 정수 설정값 읽기
        /// </summary>
        private static int GetSetting(string key, int defaultValue)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                if (int.TryParse(value, out int result))
                {
                    return result;
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 부울 설정값 읽기
        /// </summary>
        private static bool GetSetting(string key, bool defaultValue)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                if (bool.TryParse(value, out bool result))
                {
                    return result;
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Long 정수 설정값 읽기
        /// </summary>
        private static long GetSetting(string key, long defaultValue)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                if (long.TryParse(value, out long result))
                {
                    return result;
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 실수 설정값 읽기
        /// </summary>
        private static double GetSetting(string key, double defaultValue)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                if (double.TryParse(value, out double result))
                {
                    return result;
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 색상 설정값 읽기 (RGB 형식: "R,G,B" 또는 "A,R,G,B")
        /// </summary>
        private static Color GetColorSetting(string key, int defaultR, int defaultG, int defaultB, int? defaultA = null)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var parts = value.Split(',');
                    if (parts.Length == 3)
                    {
                        // RGB 형식
                        if (int.TryParse(parts[0].Trim(), out int r) &&
                            int.TryParse(parts[1].Trim(), out int g) &&
                            int.TryParse(parts[2].Trim(), out int b))
                        {
                            return Color.FromArgb(255, r, g, b);
                        }
                    }
                    else if (parts.Length == 4)
                    {
                        // ARGB 형식
                        if (int.TryParse(parts[0].Trim(), out int a) &&
                            int.TryParse(parts[1].Trim(), out int r) &&
                            int.TryParse(parts[2].Trim(), out int g) &&
                            int.TryParse(parts[3].Trim(), out int b))
                        {
                            return Color.FromArgb(a, r, g, b);
                        }
                    }
                }
                
                // 기본값 반환
                if (defaultA.HasValue)
                {
                    return Color.FromArgb(defaultA.Value, defaultR, defaultG, defaultB);
                }
                return Color.FromArgb(255, defaultR, defaultG, defaultB);
            }
            catch
            {
                // 기본값 반환
                if (defaultA.HasValue)
                {
                    return Color.FromArgb(defaultA.Value, defaultR, defaultG, defaultB);
                }
                return Color.FromArgb(255, defaultR, defaultG, defaultB);
            }
        }

        #endregion
    }
}

