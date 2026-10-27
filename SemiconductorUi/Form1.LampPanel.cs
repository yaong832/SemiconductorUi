using System.Collections.Generic;
using System.Windows.Forms;
using SemiconductorUi.Helpers;
using SemiconductorUi.ViewModels;

namespace SemiconductorUi
{
    /// <summary>
    /// 오른쪽 제어 패널을 "장비 조작반" 모양으로 그립니다 (램프 버튼 + 상태 칩, 밝은 색).
    /// 버튼 객체, 클릭 처리, Enabled 판단은 그대로이고, 그리기만 바꿉니다.
    /// 램프는 화면에 이미 있는 상태(공정 상태, 알람 표시, FOUP 장착, 웨이퍼 로드 상태)만 읽습니다.
    /// </summary>
    public partial class Form1
    {
        private readonly List<Button> lampButtons = new List<Button>();

        private void ApplyLampPanelStyle()
        {
            if (IsDesignEnvironment()) return;
            if (panelControlPanel == null) return;

            foreach (var group in new[] { groupBoxControlButtons, groupBoxFoupReady, groupBoxRecipe })
            {
                if (group == null) continue;
                group.Paint -= paintEventHandlers.GroupBox_Paint;
                LampButtonPainter.AttachPlate(group);
            }
            foreach (var flow in new Control[] { flowLayoutControlButtons, flowLayoutFoupReadyButtons })
            {
                if (flow != null) flow.BackColor = LampButtonPainter.PlateBack;
            }
            if (labelRecipe != null) labelRecipe.BackColor = LampButtonPainter.PlateBack;

            AttachLamp(buttonStart, StartLamp, primary: true);
            AttachLamp(buttonPause, PauseLamp);
            LampButtonPainter.AttachEmergency(buttonStop);
            AttachLamp(buttonResetAlarm, AlarmResetLamp);
            AttachLamp(buttonResetProcess, () => new LampState(LampTone.Off, null, null));
            AttachLamp(buttonEquipmentControl, () => new LampState(LampTone.Off, null, null));

            AttachLamp(buttonMountFoupA, () => MountLamp("FOUP A", IsFoupAMounted));
            AttachLamp(buttonMountFoupB, () => MountLamp("FOUP B", IsFoupBMounted));
            AttachLamp(buttonWaferLoading, () => LoadLamp("웨이퍼 로딩", MainFormViewModel.WaferLoadStateType.Loading, "로딩 중"));
            AttachLamp(buttonWaferUnloading, () => LoadLamp("웨이퍼 언로딩", MainFormViewModel.WaferLoadStateType.Unloading, "언로딩 중"));
            AttachLamp(buttonApplyRecipe, () => new LampState(LampTone.Off, null, null));

            // 공정 상태와 알람 표시는 다른 곳에서 바뀌므로, 1초 시계가 바뀔 때 램프를 다시 그립니다.
            if (labelHeaderCurrentTime != null)
            {
                labelHeaderCurrentTime.TextChanged += (s, e) => RefreshLampButtons();
            }
        }

        private void AttachLamp(Button button, System.Func<LampState> state, bool primary = false)
        {
            if (button == null) return;
            LampButtonPainter.Attach(button, state, primary);
            lampButtons.Add(button);
        }

        /// <summary>램프 버튼을 다시 그립니다 (공정 상태가 바뀔 때도 부릅니다).</summary>
        internal void RefreshLampButtons()
        {
            foreach (var button in lampButtons)
            {
                button.Invalidate();
            }
        }

        private LampState StartLamp()
        {
            // 진행 중이거나 알람 상태에서는 "시작 가능"으로 보이지 않게 대기 상태일 때만 켭니다.
            bool canStart = buttonStart != null && buttonStart.Enabled
                && CurrentProcessState == MainFormViewModel.ProcessState.Idle;
            return canStart
                ? new LampState(LampTone.Green, null, "시작 가능")
                : new LampState(LampTone.Off, null, null);
        }

        private LampState PauseLamp()
        {
            switch (CurrentProcessState)
            {
                case MainFormViewModel.ProcessState.Running:
                    return new LampState(LampTone.Amber, null, "진행 중");
                case MainFormViewModel.ProcessState.Paused:
                    return new LampState(LampTone.Amber, null, "정지됨");
                default:
                    return new LampState(LampTone.Off, null, null);
            }
        }

        private LampState AlarmResetLamp()
        {
            bool alarm = CurrentProcessState == MainFormViewModel.ProcessState.Error
                || SameColor(ShellAlarmDot(), UiTheme.Danger);
            return alarm
                ? new LampState(LampTone.Red, null, "알람")
                : new LampState(LampTone.Off, null, null);
        }

        private static LampState MountLamp(string name, bool mounted)
        {
            return mounted
                ? new LampState(LampTone.Green, name, "장착됨")
                : new LampState(LampTone.Off, name, "미장착");
        }

        private LampState LoadLamp(string name, MainFormViewModel.WaferLoadStateType active, string activeText)
        {
            return WaferLoadState == active
                ? new LampState(LampTone.Blue, name, activeText)
                : new LampState(LampTone.Off, name, "대기");
        }

        private static bool SameColor(System.Drawing.Color a, System.Drawing.Color b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B;
        }
    }
}
