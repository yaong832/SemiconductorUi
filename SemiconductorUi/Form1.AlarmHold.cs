using System;
using System.Windows.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi
{
    /// <summary>
    /// 환경 알람이 나면 공정을 붙잡아 둡니다 (알람 홀드).
    /// - 진행 중인 TM 이송은 끝까지 마칩니다. 중간에 멈추면 실린더가 전진한 채로 남을 수 있기 때문입니다.
    /// - 새 이송은 시작하지 않고, 챔버 공정 시간도 흐르지 않습니다.
    /// - 알람 리셋으로 알람을 풀면 일시정지 상태가 되고, 시작을 눌러야 이어 갑니다.
    /// 비상정지와 서보 알람은 이것과 별개로 기존 경로(서보 OFF)를 그대로 탑니다.
    /// </summary>
    public partial class Form1
    {
        /// <summary>
        /// 환경 알람으로 공정을 붙잡고 있는지. 알람 리셋 뒤에도 시작을 누를 때까지 true입니다.
        /// </summary>
        internal bool ProcessHeldByAlarm { get; private set; }

        /// <summary>
        /// 환경 알람이 났을 때 부릅니다. 공정 중이 아니면 아무것도 하지 않습니다.
        /// </summary>
        internal void HoldProcessForAlarm(string unitKey)
        {
            if (!SimulationRunning || ProcessHeldByAlarm)
            {
                return;
            }

            ProcessHeldByAlarm = true;
            string transferNote = CurrentTransfer != null
                ? "진행 중인 TM 이송은 끝까지 마칩니다."
                : "TM은 대기 위치에 있습니다.";
            AddLogMessage(
                $"{EquipmentRegionHelper.FormatUnitLabel(unitKey)} 환경 알람: 공정을 멈춥니다. 새 이송과 챔버 공정 시간을 멈추고, {transferNote} 알람 리셋 후 시작을 누르면 이어 갑니다.",
                "WARN");
        }

        /// <summary>
        /// 알람 리셋 뒤 시작을 눌렀을 때, 새 공정을 시작할 때, 공정을 중단하거나 초기화할 때 부릅니다.
        /// </summary>
        internal void ClearAlarmHold()
        {
            ProcessHeldByAlarm = false;
        }

        /// <summary>
        /// 시뮬레이션 모드 시험용: 챔버 표시 영역을 오른쪽 클릭하면 환경 알람을 한 번 넣을 수 있게 합니다.
        /// 가짜 환경값은 기준을 넘지 않으므로, 알람 → 알람 리셋 흐름을 확인하려면 이것이 필요합니다.
        /// EtherCAT에 연결되어 있으면 쓸 수 없습니다.
        /// </summary>
        private void AttachSimulationTestAlarmMenus()
        {
            AttachSimulationTestAlarmMenu(panelSummaryPMA, "PMA");
            AttachSimulationTestAlarmMenu(panelSummaryPMB, "PMB");
            AttachSimulationTestAlarmMenu(panelSummaryPMC, "PMC");
        }

        private void AttachSimulationTestAlarmMenu(Control panel, string unitKey)
        {
            if (panel == null)
            {
                return;
            }

            var menu = new ContextMenuStrip();
            var item = new ToolStripMenuItem();
            item.Click += (s, e) => InjectSimulationTestAlarm(unitKey);
            menu.Items.Add(item);
            menu.Opening += (s, e) =>
            {
                bool allowed = CanInjectSimulationTestAlarm(unitKey);
                item.Enabled = allowed;
                item.Text = allowed
                    ? $"시험 알람 넣기: {EquipmentRegionHelper.FormatUnitLabel(unitKey)} 온도 이탈 (시뮬레이션 전용)"
                    : "시험 알람 넣기 (EtherCAT 미연결, 이 챔버 공정 중일 때만)";
            };
            if (components != null)
            {
                components.Add(menu);
            }
            SetContextMenuRecursive(panel, menu);
        }

        private static void SetContextMenuRecursive(Control control, ContextMenuStrip menu)
        {
            control.ContextMenuStrip = menu;
            foreach (Control child in control.Controls)
            {
                SetContextMenuRecursive(child, menu);
            }
        }

        private bool CanInjectSimulationTestAlarm(string unitKey)
        {
            if (EthercatConnected || !SimulationRunning || SimulationPaused)
            {
                return false;
            }

            var chamber = GetChamberStateForUnit(unitKey);
            return chamber != null && chamber.CurrentWafer != null
                && chamber.RemainingSeconds > 0
                && ChamberStatusTexts.IsProcessing(chamber.StatusText);
        }

        /// <summary>
        /// 온도를 설정값 +100°C로 한 번 넣어 환경 알람 판정을 탑니다 (실제 값 생성 경로와 같은 판정).
        /// </summary>
        internal void InjectSimulationTestAlarm(string unitKey)
        {
            if (!CanInjectSimulationTestAlarm(unitKey))
            {
                return;
            }

            if (!ChamberEnvSpecs.TryGetValue(unitKey, out var spec))
            {
                return;
            }

            AddLogMessage($"시험 알람: {EquipmentRegionHelper.FormatUnitLabel(unitKey)} 온도를 기준 밖으로 한 번 넣었습니다 (시뮬레이션 전용)", "WARN");
            double svTemp = spec.TargetTemperatureC;
            double svPress = spec.TargetPressureTorr;
            EvaluateEnvironmentAlarms(
                unitKey,
                0, 0, 0, svPress, 0, svTemp,
                0, 0, 0, svPress, 0, svTemp + 100.0);
        }
    }
}
