using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.Controls;
using SemiconductorUi.Helpers;

namespace SemiconductorUi
{
    using ProcessState = ViewModels.MainFormViewModel.ProcessState;

    /// <summary>
    /// 메인 화면 틀 (SEMI E95 배치): 맨 위 타이틀 줄(장비 이름·화면 이름·상태·로그인·연결·시계·3색 램프),
    /// 본문 위 작은 탭(모니터링/검증/이송), 맨 아래 메뉴 줄(운전~보고서, 현재 상태, 시스템).
    /// 배치(어떤 컨트롤이 어디에 있는지)는 모두 Form1.Designer.cs에 있고, 이 파일은 그리기·글자 연결과
    /// 창 크기에 따른 크기 조정만 합니다.
    /// 기존 상단 표(panelHeader)는 Form1.Designer.cs의 숨긴 보관 패널(panelShellParking)에 있습니다.
    /// 코드가 계속 바꾸는 상태 글자(labelEthercatStatus, labelServoStatus, labelHeaderEventMessage 등)를
    /// 상단 바가 읽어 보여 주므로 지우지 않습니다.
    /// 메뉴·탭 버튼은 Form1.Designer.cs에서 ShellNavButton으로 선언되어 디자이너에서도 실행 화면과 같은 모양으로 보입니다.
    /// 디자이너의 "디자이너 미리보기" 값(그림, 선택 표시, 칩 글자 등)은 디자이너 그림에만 쓰이고, 실행 중 모양은 이 파일이 정합니다.
    /// </summary>
    public partial class Form1
    {
        private const int ShellBottomBarHeight = 64;
        private const int ShellBottomBarLargeHeight = 72;
        // 창이 이 폭 이상이면(2560x1440 최대화 등) 상단 바 글자와 버튼을 한 단계 키웁니다.
        private const int ShellLargeMinWidth = 2200;

        private ToolTip shellToolTip;
        private readonly System.Collections.Generic.List<Tuple<Button, int>> shellBarButtons = new System.Collections.Generic.List<Tuple<Button, int>>();

        private void BuildMainShell()
        {
            if (IsDesignEnvironment()) return;
            if (tableLayoutRoot == null || shellTopBar == null || tableShellContent == null || tableShellBottomBar == null) return;

            SuspendLayout();
            tableLayoutRoot.SuspendLayout();

            // 상단 바(shellTopBar), 가운데(탭 줄 + 공정 화면), 하단 메뉴 줄은 Form1.Designer.cs에 배치되어 있습니다.
            // 여기서는 그리기와 글자 연결만 합니다.
            shellToolTip = new ToolTip();
            if (components != null) components.Add(shellToolTip);

            InitShellTopBar();
            AttachAlarmHistoryChip();

            InitShellSubTabs();
            InitShellBottomBar();

            // FOUP 표 아래 "교환 상태" 한 줄은 하단 메뉴 줄 가운데(현재 상태)로 옮겨 보여 줍니다.
            // 글자는 기존 코드가 계속 바꾸므로 라벨은 숨겨서 그대로 둡니다.
            if (labelFoupSummaryInfo != null && tableProcessMetrics != null && tableProcessMetrics.RowStyles.Count > 1)
            {
                labelFoupSummaryInfo.Visible = false;
                tableProcessMetrics.RowStyles[1].Height = 0;
            }

            // "메인 공정 모니터링" 제목은 모니터링 탭과 겹치므로 숨깁니다.
            if (labelMainProcessTitle != null && tableLayoutMainProcess != null && tableLayoutMainProcess.RowStyles.Count > 0)
            {
                labelMainProcessTitle.Visible = false;
                tableLayoutMainProcess.RowStyles[0].Height = 0;
            }

            HookShellRefresh();

            tableLayoutRoot.ResumeLayout(true);
            ResumeLayout(true);

            Resize += (s, e) => ApplyShellSize();
            ApplyShellSize();
        }

        /// <summary>창 폭에 따라 상단 바를 보통 크기 또는 큰 크기로 맞춥니다.</summary>
        private void ApplyShellSize()
        {
            if (shellTopBar == null || WindowState == FormWindowState.Minimized) return;

            bool large = ClientSize.Width >= ShellLargeMinWidth;
            if (shellTopBar.Large == large) return;

            shellTopBar.SuspendLayout();
            foreach (var chip in new[] { shellChipEthercat, shellChipServo, shellChipAlarm })
            {
                if (chip != null) chip.Large = large;
            }
            if (shellClock != null) shellClock.Large = large;
            foreach (var pair in shellBarButtons)
            {
                pair.Item1.Size = new Size(pair.Item2 + (large ? 12 : 0), large ? 32 : 28);
                pair.Item1.Font = large ? ShellBarButtonLargeFont : ShellBarButtonFont;
            }
            if (shellUserCard != null) shellUserCard.Size = large ? new Size(100, 40) : new Size(92, 36);
            shellTopBar.Large = large;
            shellTopBar.ResumeLayout(true);

            if (tableLayoutRoot != null && tableLayoutRoot.RowStyles.Count > 2)
            {
                tableLayoutRoot.RowStyles[2].Height = large ? ShellBottomBarLargeHeight : ShellBottomBarHeight;
            }
        }

        private static readonly Font ShellBarButtonFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        private static readonly Font ShellBarButtonLargeFont = new Font("Segoe UI", 10F, FontStyle.Bold);

        private Button AddShellBarButton(Button button, string text, int width)
        {
            var styled = StyleShellBarButton(button, text, width);
            if (styled != null)
            {
                styled.Font = ShellBarButtonFont;
                shellBarButtons.Add(Tuple.Create(styled, width));
            }
            return styled;
        }

        /// <summary>
        /// 맨 위 타이틀 줄. 배치(shellTopBar 안의 flowShellTopLeft·flowShellTopRight와 칩·버튼·시계·램프)는
        /// Form1.Designer.cs에 있고, 여기서는 그릴 글자·색을 읽어 올 곳과 버튼 모양만 연결합니다.
        /// </summary>
        private void InitShellTopBar()
        {
            shellTopBar.StateProvider = GetShellStatePill;
            shellTopBar.SubtitleProvider = GetShellViewTitle;
            shellTopBar.PillSlotTexts = ShellStateTexts;

            // 상태 표시 옆: 로그인 사용자와 로그인/로그아웃/사용자 관리 (SEMI E95: 타이틀 줄에 로그인 버튼)
            if (shellUserCard != null)
            {
                shellUserCard.UserProvider = () => CurrentUser;
                shellUserCard.RoleProvider = () => CurrentRole;
                shellUserCard.LoggedInProvider = () => IsLoggedIn;
            }
            AddShellBarButton(buttonLogin, "로그인", 64);
            AddShellBarButton(buttonLogout, "로그아웃", 76);
            AddShellBarButton(buttonUserManagement, "사용자 관리", 92);

            if (shellChipEthercat != null)
            {
                shellChipEthercat.TextProvider = () => ShellEthercatText(labelEthercatStatus?.Text);
                shellChipEthercat.DotProvider = () => labelEthercatStatus?.ForeColor ?? UiTheme.TextSecondary;
            }
            AddShellBarButton(buttonEthercatConnect, "연결", 52);
            AddShellBarButton(buttonEthercatDisconnect, "해제", 52);

            if (shellChipServo != null)
            {
                shellChipServo.TextProvider = () => ShellServoText(labelServoStatus?.Text);
                shellChipServo.DotProvider = () => labelServoStatus?.ForeColor ?? UiTheme.TextSecondary;
            }
            AddShellBarButton(buttonServoOn, "서보 ON", 68);
            AddShellBarButton(buttonServoOff, "서보 OFF", 72);

            if (shellChipAlarm != null)
            {
                shellChipAlarm.TextProvider = ShellAlarmText;
                shellChipAlarm.DotProvider = ShellAlarmDot;
            }

            // 오른쪽 끝 3색 램프: 장비 그림의 램프 표시와 같은 색을 읽습니다.
            if (shellTowerLamp != null)
            {
                shellTowerLamp.RedProvider = () => panelMainLampRed?.BackColor ?? EquipmentCanvasStyler.LampOff;
                shellTowerLamp.YellowProvider = () => panelMainLampYellow?.BackColor ?? EquipmentCanvasStyler.LampOff;
                shellTowerLamp.GreenProvider = () => panelMainLampGreen?.BackColor ?? EquipmentCanvasStyler.LampOff;
            }
            foreach (var lamp in new Control[] { panelMainLampRed, panelMainLampYellow, panelMainLampGreen })
            {
                if (lamp != null) lamp.BackColorChanged += (s, e) => shellTowerLamp?.Invalidate();
            }
        }

        private Button StyleShellBarButton(Button button, string text, int width)
        {
            if (button == null) return null;
            button.Text = text;
            button.Size = new Size(width, 28);
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            UiTheme.StyleButton(button, UiTheme.Surface, UiTheme.SurfaceAlt, UiTheme.Primary);
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = UiTheme.Border;
            return button;
        }

        /// <summary>
        /// 맨 아래 메뉴 줄 (SEMI E95 내비게이션 패널): 왼쪽에 화면 메뉴, 가운데 현재 상태 한 줄, 오른쪽 끝에 시스템.
        /// 배치(tableShellBottomBar, flowShellNavMenu, shellStatusLine, flowShellNavSystem)는 Form1.Designer.cs에 있고,
        /// 여기서는 메뉴 버튼 그리기와 이벤트만 연결합니다.
        /// </summary>
        private void InitShellBottomBar()
        {
            var bar = tableShellBottomBar;
            bar.Paint += TableShellBottomBar_Paint;

            StyleShellNavItem(buttonNavOperate, "운전", ShellNavIcon.Operate);
            StyleShellNavItem(buttonNavRecipe, "레시피", ShellNavIcon.Recipe);
            StyleShellNavItem(buttonNavMaintenance, "유지보수", ShellNavIcon.Maintenance);
            StyleShellNavItem(buttonNavConfig, "설정", ShellNavIcon.Config);
            StyleShellNavItem(buttonNavTrend, "추이", ShellNavIcon.Trend);
            StyleShellNavItem(buttonNavReport, "보고서", ShellNavIcon.Report);

            if (shellStatusLine != null)
            {
                shellStatusLine.TextProvider = () => labelFoupSummaryInfo?.Text ?? "";
            }
            if (labelFoupSummaryInfo != null)
            {
                labelFoupSummaryInfo.TextChanged += (s, e) => shellStatusLine?.Invalidate();
            }

            StyleShellNavItem(buttonNavSystem, "시스템", ShellNavIcon.System);
            if (buttonNavSystem != null) buttonNavSystem.Margin = Padding.Empty;

            bar.SizeChanged += (s, e) => FitShellNavItems(bar);
            FitShellNavItems(bar);
        }

        private void TableShellBottomBar_Paint(object sender, PaintEventArgs e)
        {
            var bar = (Control)sender;
            using (var pen = new Pen(UiTheme.Border))
            {
                e.Graphics.DrawLine(pen, 0, 0, bar.Width, 0);
            }
        }

        private static readonly string[] ShellStateTexts = { "비상정지", "공정 진행 중", "일시 정지", "경고 · 정지", "원점복귀 중", "대기" };

        /// <summary>메뉴 버튼 높이를 하단 줄 높이에 맞춥니다.</summary>
        private void FitShellNavItems(Control bar)
        {
            int height = Math.Max(36, bar.Height - bar.Padding.Vertical);
            foreach (var button in new[] { buttonNavOperate, buttonNavRecipe, buttonNavMaintenance, buttonNavConfig, buttonNavTrend, buttonNavReport, buttonNavSystem })
            {
                if (button != null && button.Height != height)
                {
                    button.Height = height;
                }
            }
        }

        /// <summary>장비 이름 아래 작은 글자: 선택된 메뉴와 탭 이름(예: "운전 · 모니터링").</summary>
        private string GetShellViewTitle()
        {
            string nav = null;
            foreach (var button in new[] { buttonNavOperate, buttonNavRecipe, buttonNavMaintenance, buttonNavConfig, buttonNavTrend, buttonNavReport, buttonNavSystem })
            {
                if (ShellNavPainter.IsSelected(button)) { nav = button.Text; break; }
            }
            string tab = null;
            foreach (var button in new[] { buttonTabMain, buttonTabVerification, buttonTabTransfer })
            {
                if (ShellNavPainter.IsSelected(button)) { tab = button.Text; break; }
            }
            if (nav == null) return tab;
            return tab == null || nav != "운전" ? nav : nav + " · " + tab;
        }

        private static void StyleShellNavItem(Button button, string text, ShellNavIcon icon)
        {
            if (button == null) return;
            ShellNavPainter.StyleNavBarItem(button, text, icon, new Size(128, 48));
            button.Margin = new Padding(0, 0, 8, 0);
        }

        /// <summary>
        /// 모니터링·검증·이송 탭 줄. 배치(panelShellSubTabs, flowShellSubTabs)는 Form1.Designer.cs에 있고,
        /// 여기서는 탭 그리기와 테두리만 연결합니다.
        /// </summary>
        private void InitShellSubTabs()
        {
            if (flowShellSubTabs == null) return;
            flowShellSubTabs.Paint += FlowShellSubTabs_Paint;

            var tabSize = new Size(88, 28);
            foreach (var pair in new[]
            {
                Tuple.Create(buttonTabMain, "모니터링"),
                Tuple.Create(buttonTabVerification, "검증"),
                Tuple.Create(buttonTabTransfer, "이송")
            })
            {
                if (pair.Item1 == null) continue;
                ShellNavPainter.StyleSubTab(pair.Item1, pair.Item2, tabSize);
                pair.Item1.Margin = new Padding(0, 0, 2, 0);
            }
        }

        private void FlowShellSubTabs_Paint(object sender, PaintEventArgs e)
        {
            var segment = (Control)sender;
            using (var pen = new Pen(UiTheme.Border))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, segment.Width - 1, segment.Height - 1);
            }
        }

        /// <summary>
        /// 기존 상태 글자가 바뀔 때마다 상단 바와 사용자 표시를 다시 그립니다.
        /// </summary>
        private void HookShellRefresh()
        {
            if (labelEthercatStatus != null)
            {
                labelEthercatStatus.TextChanged += (s, e) => shellChipEthercat?.RefreshChip();
                labelEthercatStatus.ForeColorChanged += (s, e) => shellChipEthercat?.RefreshChip();
            }
            if (labelServoStatus != null)
            {
                labelServoStatus.TextChanged += (s, e) => shellChipServo?.RefreshChip();
                labelServoStatus.ForeColorChanged += (s, e) => shellChipServo?.RefreshChip();
            }
            if (labelHeaderEventMessage != null)
            {
                labelHeaderEventMessage.TextChanged += (s, e) => RefreshShellAlarmChip();
            }
            if (labelHeaderEventLevel != null)
            {
                labelHeaderEventLevel.TextChanged += (s, e) => RefreshShellAlarmChip();
            }
            if (labelLoginStatus != null)
            {
                labelLoginStatus.TextChanged += (s, e) => shellUserCard?.Invalidate();
            }
            if (labelHeaderCurrentTime != null)
            {
                // 1초 시계 타이머가 이 글자를 바꿉니다. 그때 시계와 큰 상태 표시(비상정지 포함)를 다시 그립니다.
                labelHeaderCurrentTime.TextChanged += (s, e) =>
                {
                    shellClock?.Invalidate();
                    shellTopBar?.Invalidate();
                };
            }

            Action repaintTitle = () => shellTopBar?.Invalidate();
            ShellNavPainter.SelectionChanged += repaintTitle;
            FormClosed += (s, e) => ShellNavPainter.SelectionChanged -= repaintTitle;

            shellChipEthercat?.RefreshChip();
            shellChipServo?.RefreshChip();
            RefreshShellAlarmChip();
        }

        private void RefreshShellAlarmChip()
        {
            if (shellChipAlarm == null) return;
            shellChipAlarm.RefreshChip();
            shellToolTip?.SetToolTip(shellChipAlarm, (labelHeaderEventMessage?.Text ?? "") + "\n(누르면 현재 알람과 알람 이력을 봅니다)");
        }

        /// <summary>공정 상태가 바뀌면 상단 바의 큰 상태 표시를 다시 그립니다.</summary>
        internal void RefreshShellState()
        {
            shellTopBar?.Invalidate();
            shellUserCard?.Invalidate();
            RefreshLampButtons();
        }

        private ShellStatePill GetShellStatePill()
        {
            if (EtherCAT_M != null && EtherCAT_M.IsEmergencyStopped)
            {
                return new ShellStatePill("비상정지", UiTheme.Danger, Color.FromArgb(168, 38, 48), Color.FromArgb(251, 233, 234));
            }

            switch (CurrentProcessState)
            {
                case ProcessState.Running:
                    return new ShellStatePill("공정 진행 중", UiTheme.Primary, UiTheme.Primary, Color.FromArgb(232, 239, 250));
                case ProcessState.Paused:
                    return new ShellStatePill("일시 정지", UiTheme.Warning, Color.FromArgb(150, 92, 8), Color.FromArgb(252, 241, 222));
                case ProcessState.Error:
                    return new ShellStatePill("경고 · 정지", UiTheme.Danger, Color.FromArgb(168, 38, 48), Color.FromArgb(251, 233, 234));
                default:
                    if (IsBackgroundHomingInProgress())
                    {
                        // 공정은 대기지만 TM이 원점복귀로 움직이는 중 (시작·리셋 등이 잠겨 있음)
                        return new ShellStatePill("원점복귀 중", UiTheme.Warning, Color.FromArgb(150, 92, 8), Color.FromArgb(252, 241, 222));
                    }
                    return new ShellStatePill("대기", Color.FromArgb(140, 150, 164), Color.FromArgb(58, 69, 85), Color.FromArgb(238, 241, 245));
            }
        }

        private static string ShellEthercatText(string raw)
        {
            switch ((raw ?? "").Trim())
            {
                case "EtherCAT: Connected": return "EtherCAT 연결됨";
                case "EtherCAT: Disconnected": return "EtherCAT 끊김";
                case "EtherCAT: Connection Failed": return "EtherCAT 연결 실패";
                case "EtherCAT: Error": return "EtherCAT 오류";
                default: return string.IsNullOrWhiteSpace(raw) ? "EtherCAT" : raw.Trim();
            }
        }

        private static string ShellServoText(string raw)
        {
            var text = (raw ?? "").Trim();
            if (text.Length == 0) return "서보";
            if (text == "서보: - | Home: -") return "서보 미연결";
            return text.Replace("서보: ", "서보 ").Replace(" | Home: ", " · 원점 ").Replace(" | ", " · ");
        }

        private string ShellAlarmText()
        {
            var message = labelHeaderEventMessage?.Text;
            return string.IsNullOrWhiteSpace(message) ? "알람 없음" : message.Trim();
        }

        private Color ShellAlarmDot()
        {
            var message = (labelHeaderEventMessage?.Text ?? "").Trim();
            if (message.Length == 0 || message == "알람 없음" || message == "메시지 없음")
            {
                return UiTheme.Success;
            }

            switch ((labelHeaderEventLevel?.Text ?? "").Trim().ToUpperInvariant())
            {
                case "ALARM":
                case "ERROR":
                case "CRITICAL":
                    return UiTheme.Danger;
                case "WARN":
                case "WARNING":
                    return UiTheme.Warning;
                default:
                    return UiTheme.TextSecondary;
            }
        }
    }
}
