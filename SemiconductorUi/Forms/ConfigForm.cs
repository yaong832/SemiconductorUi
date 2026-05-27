using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using SemiconductorUi.Models;
using SemiconductorUi.Repositories;
using AppSettings = SemiconductorUi.AppSettings;

namespace SemiconductorUi.Forms
{
    public partial class ConfigForm : Form
    {
        private EnvThresholdSnapshot currentThresholds;
        public Action<EnvThresholdSnapshot> OnSaved;

        public ConfigForm(EnvThresholdSnapshot initialThresholds)
        {
            InitializeComponent();
            currentThresholds = initialThresholds ?? CreateDefaultThresholds();
            LoadThresholdsToUI();
        }

        private EnvThresholdSnapshot CreateDefaultThresholds()
        {
            // AppSettings에서 기본값 읽어옴
            return new EnvThresholdSnapshot
            {
                TempWarn = AppSettings.AlarmTempWarn,
                TempAlarm = AppSettings.AlarmTempAlarm,
                PressWarnRatio = AppSettings.AlarmPressWarnRatio,
                PressAlarmRatio = AppSettings.AlarmPressAlarmRatio,
                PressWarnAbs = AppSettings.AlarmPressWarnAbs,
                PressAlarmAbs = AppSettings.AlarmPressAlarmAbs,
                RfWarnRatio = AppSettings.AlarmRfWarnRatio,
                RfAlarmRatio = AppSettings.AlarmRfAlarmRatio,
                GasWarn = AppSettings.AlarmGasWarn,
                GasAlarm = AppSettings.AlarmGasAlarm,
                GasLeakWarn = AppSettings.AlarmGasLeakWarn,
                GasLeakAlarm = AppSettings.AlarmGasLeakAlarm
            };
        }

        private void LoadThresholdsToUI()
        {
            numTempWarn.Value = (decimal)currentThresholds.TempWarn;
            numTempAlarm.Value = (decimal)currentThresholds.TempAlarm;
            numPressWarnRatio.Value = (decimal)currentThresholds.PressWarnRatio;
            numPressAlarmRatio.Value = (decimal)currentThresholds.PressAlarmRatio;
            numPressWarnAbs.Value = (decimal)currentThresholds.PressWarnAbs;
            numPressAlarmAbs.Value = (decimal)currentThresholds.PressAlarmAbs;
            numRfWarnRatio.Value = (decimal)currentThresholds.RfWarnRatio;
            numRfAlarmRatio.Value = (decimal)currentThresholds.RfAlarmRatio;
            numGasWarn.Value = (decimal)currentThresholds.GasWarn;
            numGasAlarm.Value = (decimal)currentThresholds.GasAlarm;
            numGasLeakWarn.Value = (decimal)currentThresholds.GasLeakWarn;
            numGasLeakAlarm.Value = (decimal)currentThresholds.GasLeakAlarm;
        }

        private EnvThresholdSnapshot GetThresholdsFromUI()
        {
            return new EnvThresholdSnapshot
            {
                TempWarn = (double)numTempWarn.Value,
                TempAlarm = (double)numTempAlarm.Value,
                PressWarnRatio = (double)numPressWarnRatio.Value,
                PressAlarmRatio = (double)numPressAlarmRatio.Value,
                PressWarnAbs = (double)numPressWarnAbs.Value,
                PressAlarmAbs = (double)numPressAlarmAbs.Value,
                RfWarnRatio = (double)numRfWarnRatio.Value,
                RfAlarmRatio = (double)numRfAlarmRatio.Value,
                GasWarn = (double)numGasWarn.Value,
                GasAlarm = (double)numGasAlarm.Value,
                GasLeakWarn = (double)numGasLeakWarn.Value,
                GasLeakAlarm = (double)numGasLeakAlarm.Value
            };
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var thresholds = GetThresholdsFromUI();
                ConfigRepository.Save(thresholds);
                
                // 저장 후 파일에서 다시 로드하여 확인
                var savedThresholds = ConfigRepository.Load();
                if (savedThresholds != null && 
                    Math.Abs(savedThresholds.TempWarn - thresholds.TempWarn) < 0.001 &&
                    Math.Abs(savedThresholds.TempAlarm - thresholds.TempAlarm) < 0.001)
                {
                    // 저장 성공 (주요 값 비교)
                    OnSaved?.Invoke(thresholds);
                    MessageBox.Show(this, "설정이 저장되었습니다.", "저장 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(this, "설정 저장에 실패했습니다. 파일 저장에 문제가 있을 수 있습니다.", "저장 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"설정 저장 중 오류가 발생했습니다:\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this, "모든 설정을 기본값으로 복원하시겠습니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                currentThresholds = CreateDefaultThresholds();
                LoadThresholdsToUI();
            }
        }
    }
}

