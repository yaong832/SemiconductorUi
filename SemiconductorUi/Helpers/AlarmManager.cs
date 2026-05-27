using System;
using System.Collections.Generic;
using System.Linq;
using SemiconductorUi.ViewModels;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// 알람 관리 로직을 담당하는 헬퍼 클래스
    /// 환경 알람 평가, 알람 상태 관리, 알람 리셋 처리
    /// </summary>
    public class AlarmManager
    {
        private readonly Form1 _form;

        /// <summary>
        /// 현재 알람 임계값 (Form1의 EnvAlarmThresholds 구조 사용)
        /// </summary>
        public Form1.EnvAlarmThresholds Thresholds { get; private set; }

        /// <summary>
        /// AlarmManager 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public AlarmManager(Form1 form)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            LoadDefaultThresholds();
        }

        /// <summary>
        /// 기본 알람 임계값 로드
        /// </summary>
        private void LoadDefaultThresholds()
        {
            Thresholds = Form1.EnvAlarmThresholds.CreateDefault();
        }

        /// <summary>
        /// 알람 임계값 설정
        /// </summary>
        /// <param name="thresholds">알람 임계값</param>
        public void SetThresholds(Form1.EnvAlarmThresholds thresholds)
        {
            Thresholds = thresholds;
        }

        /// <summary>
        /// 환경 알람 평가
        /// </summary>
        /// <param name="unitKey">유닛 키 (PMA, PMB, PMC)</param>
        /// <param name="svNF3">NF3 설정값</param>
        /// <param name="svO2">O2 설정값</param>
        /// <param name="svCF4">CF4 설정값</param>
        /// <param name="svPress">압력 설정값</param>
        /// <param name="svRf">RF 설정값</param>
        /// <param name="svTemp">온도 설정값</param>
        /// <param name="pvNF3">NF3 현재값</param>
        /// <param name="pvO2">O2 현재값</param>
        /// <param name="pvCF4">CF4 현재값</param>
        /// <param name="pvPress">압력 현재값</param>
        /// <param name="pvRf">RF 현재값</param>
        /// <param name="pvTemp">온도 현재값</param>
        /// <returns>알람 발생 여부</returns>
        public bool EvaluateEnvironmentAlarms(
            string unitKey,
            double svNF3, double svO2, double svCF4, double svPress, double svRf, double svTemp,
            double pvNF3, double pvO2, double pvCF4, double pvPress, double pvRf, double pvTemp)
        {
            bool hasAlarmInChamber = false;

            // 온도 평가
            var tempDiff = Math.Abs(pvTemp - svTemp);
            if (tempDiff > Thresholds.TempAlarmDiffC)
            {
                _form.AddLogMessage($"{unitKey} 온도 이탈(알람): PV={pvTemp:0.0}°C, SV={svTemp:0.0}°C (Δ={tempDiff:0.0}°C)", "ALARM");
                hasAlarmInChamber = true;
            }
            else if (tempDiff > Thresholds.TempWarnDiffC)
            {
                _form.AddLogMessage($"{unitKey} 온도 이탈(주의): PV={pvTemp:0.0}°C, SV={svTemp:0.0}°C (Δ={tempDiff:0.0}°C)", "WARN");
            }

            // 압력 평가: 고압/저압에 따라 비율 또는 절대값 기준 선택
            double pressDiff = Math.Abs(pvPress - svPress);
            if (svPress > 0)
            {
                bool isHighPressure = svPress > 10.0; // 10 Torr 이상은 고압으로 간주
                bool isAlarm = false;
                bool isWarn = false;

                if (isHighPressure)
                {
                    // 고압: 절대값 기준만 사용 (비율 기준은 너무 큼)
                    isAlarm = pressDiff > Thresholds.PressAlarmAbsTorr;
                    isWarn = pressDiff > Thresholds.PressWarnAbsTorr && !isAlarm;
                }
                else
                {
                    // 저압: 비율과 절대값 모두 확인 (둘 중 하나라도 초과하면 알람)
                    double pressRatio = pressDiff / svPress;
                    isAlarm = pressRatio > Thresholds.PressAlarmRatio || pressDiff > Thresholds.PressAlarmAbsTorr;
                    isWarn = (pressRatio > Thresholds.PressWarnRatio || pressDiff > Thresholds.PressWarnAbsTorr) && !isAlarm;
                }

                if (isAlarm)
                {
                    _form.AddLogMessage($"{unitKey} 압력 이탈(알람): PV={pvPress:0.###} Torr, SV={svPress:0.###} Torr (Δ={pressDiff:0.###} Torr)", "ALARM");
                    hasAlarmInChamber = true;
                }
                else if (isWarn)
                {
                    _form.AddLogMessage($"{unitKey} 압력 이탈(주의): PV={pvPress:0.###} Torr, SV={svPress:0.###} Torr (Δ={pressDiff:0.###} Torr)", "WARN");
                }
            }

            // RF 평가
            if (svRf > 0)
            {
                double rfRatio = Math.Abs(pvRf - svRf) / svRf;
                if (rfRatio > Thresholds.RfAlarmRatio)
                {
                    _form.AddLogMessage($"{unitKey} RF 이탈(알람): PV={pvRf:0} W, SV={svRf:0} W", "ALARM");
                    hasAlarmInChamber = true;
                }
                else if (rfRatio > Thresholds.RfWarnRatio)
                {
                    _form.AddLogMessage($"{unitKey} RF 이탈(주의): PV={pvRf:0} W, SV={svRf:0} W", "WARN");
                }
            }

            // 가스 평가 (각각)
            if (EvaluateGas(unitKey, "NF3", svNF3, pvNF3))
            {
                hasAlarmInChamber = true;
            }
            
            if (EvaluateGas(unitKey, "O2", svO2, pvO2))
            {
                hasAlarmInChamber = true;
            }
            
            if (EvaluateGas(unitKey, "CF4", svCF4, pvCF4))
            {
                hasAlarmInChamber = true;
            }

            return hasAlarmInChamber;
        }

        /// <summary>
        /// 가스 알람 평가
        /// </summary>
        /// <param name="unitKey">유닛 키</param>
        /// <param name="gasName">가스 이름</param>
        /// <param name="sv">설정값</param>
        /// <param name="pv">현재값</param>
        /// <returns>알람 발생 여부</returns>
        private bool EvaluateGas(string unitKey, string gasName, double sv, double pv)
        {
            bool localHasAlarm = false;
            
            if (sv > 0)
            {
                double diff = Math.Abs(pv - sv);
                if (diff > Thresholds.GasAlarmAbsSccm)
                {
                    _form.AddLogMessage($"{unitKey} {gasName} 유량 이탈(알람): PV={pv:0.0} sccm, SV={sv:0.0} sccm", "ALARM");
                    localHasAlarm = true;
                }
                else if (diff > Thresholds.GasWarnAbsSccm)
                {
                    _form.AddLogMessage($"{unitKey} {gasName} 유량 이탈(주의): PV={pv:0.0} sccm, SV={sv:0.0} sccm", "WARN");
                }
            }
            else
            {
                if (pv > Thresholds.GasLeakAlarmSccm)
                {
                    _form.AddLogMessage($"{unitKey} {gasName} 누설 의심(알람): PV={pv:0.0} sccm (SV=0)", "ALARM");
                    localHasAlarm = true;
                }
                else if (pv > Thresholds.GasLeakWarnSccm)
                {
                    _form.AddLogMessage($"{unitKey} {gasName} 누설 의심(주의): PV={pv:0.0} sccm (SV=0)", "WARN");
                }
            }
            return localHasAlarm;
        }

        /// <summary>
        /// 알람 상태 리셋
        /// </summary>
        public void ResetAllAlarms()
        {
            // 모든 알람 상태 리셋
            _form.HasAlarm = false;
            foreach (var key in _form.ChamberAlarmStatus.Keys.ToList())
            {
                _form.ChamberAlarmStatus[key] = false;
            }

            // verification 알람도 리셋됨을 표시
            _form.VerificationAlarmDismissed = true;

            _form.AddLogMessage("알람이 리셋되었습니다.", "INFO");
        }

        /// <summary>
        /// 특정 챔버의 알람 상태 설정
        /// </summary>
        /// <param name="unitKey">유닛 키</param>
        /// <param name="hasAlarm">알람 발생 여부</param>
        public void SetChamberAlarm(string unitKey, bool hasAlarm)
        {
            if (string.IsNullOrEmpty(unitKey))
            {
                return;
            }

            _form.ViewModel?.SetChamberAlarmStatus(unitKey, hasAlarm);
        }
    }
}

