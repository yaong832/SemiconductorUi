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
    /// 기존 버튼(로그인, EtherCAT 연결, 서보 ON/OFF, 메뉴, 탭)은 같은 객체를 자리만 옮기므로
    /// 클릭 핸들러, 권한에 따른 Enabled, 이름으로 찾는 코드는 그대로 동작합니다.
    /// 기존 상단 표와 하단 메뉴 줄은 숨겨서 보관하고, 코드가 계속 바꾸는 상태 글자는 상단 바가 읽어 보여 줍니다.
    /// </summary>
    public partial class Form1
    {
        private const int ShellBottomBarHeight = 64;
        private const int ShellBottomBarLargeHeight = 72;
        private const int ShellSubTabHeight = 46;
        // 창이 이 폭 이상이면(2560x1440 최대화 등) 상단 바 글자와 버튼을 한 단계 키웁니다.
        private const int ShellLargeMinWidth = 2200;

        private ShellTopBar shellTopBar;
        private ShellChip shellChipEthercat;
        private ShellChip shellChipServo;
        private ShellChip shellChipAlarm;
        private ShellClock shellClock;
        private ShellUserCard shellUserCard;
        private ShellTowerLamp shellTowerLamp;
        private ShellStatusLine shellStatusLine;
        private Control shellBottomBar;
        private ToolTip shellToolTip;
        private readonly System.Collections.Generic.List<Tuple<Button, int>> shellBarButtons = new System.Collections.Generic.List<Tuple<Button, int>>();

        private void BuildMainShell()
        {
            if (IsDesignEnvironment()) return;
            if (tableLayoutRoot == null || panelHeader == null || panelAlarmArea == null || tableLayoutProcessArea == null) return;

            SuspendLayout();
            tableLayoutRoot.SuspendLayout();

            // 기존 상단 영역과 하단 메뉴 줄은 화면에서 빼고 숨긴 채 보관합니다.
            tableLayoutRoot.Controls.Remove(panelHeader);
            tableLayoutRoot.Controls.Remove(panelAlarmArea);
            tableLayoutRoot.Controls.Remove(tableLayoutProcessArea);
            var parking = new Panel { Name = "panelShellParking", Visible = false };
            parking.Controls.Add(panelHeader);
            parking.Controls.Add(panelAlarmArea);
            Controls.Add(parking);

            shellToolTip = new ToolTip();
            if (components != null) components.Add(shellToolTip);

            shellTopBar = BuildShellTopBar();

            var content = new TableLayoutPanel
            {
                Name = "tableShellContent",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = new Padding(10, 0, 6, 6),
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UiTheme.AppBack
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, ShellSubTabHeight));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            content.Controls.Add(BuildShellSubTabs(), 0, 0);
            content.Controls.Add(tableLayoutProcessArea, 0, 1);

            shellBottomBar = BuildShellBottomBar();

            tableLayoutRoot.RowStyles.Clear();
            tableLayoutRoot.RowCount = 3;
            tableLayoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, ShellTopBar.BarHeight));
            tableLayoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, ShellBottomBarHeight));
            tableLayoutRoot.Margin = Padding.Empty;
            tableLayoutRoot.Padding = Padding.Empty;
            tableLayoutRoot.Controls.Add(shellTopBar, 0, 0);
            tableLayoutRoot.Controls.Add(content, 0, 1);
            tableLayoutRoot.Controls.Add(shellBottomBar, 0, 2);

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

        private ShellTopBar BuildShellTopBar()
        {
            var bar = new ShellTopBar
            {
                Name = "shellTopBar",
                StateProvider = GetShellStatePill,
                SubtitleProvider = GetShellViewTitle,
                PillSlotTexts = ShellStateTexts
            };

            // 상태 표시 옆: 로그인 사용자와 로그인/로그아웃/사용자 관리 (SEMI E95: 타이틀 줄에 로그인 버튼)
            shellUserCard = new ShellUserCard
            {
                Name = "shellUserCard",
                Horizontal = true,
                Size = new Size(92, 36),
                UserProvider = () => CurrentUser,
                RoleProvider = () => CurrentRole,
                LoggedInProvider = () => IsLoggedIn
            };
            bar.AddLeft(shellUserCard, 0);
            bar.AddLeft(AddShellBarButton(buttonLogin, "로그인", 64), 6);
            bar.AddLeft(AddShellBarButton(buttonLogout, "로그아웃", 76), 4);
            bar.AddLeft(AddShellBarButton(buttonUserManagement, "사용자 관리", 92), 4);

            shellChipEthercat = new ShellChip
            {
                Name = "shellChipEthercat",
                TextProvider = () => ShellEthercatText(labelEthercatStatus?.Text),
                DotProvider = () => labelEthercatStatus?.ForeColor ?? UiTheme.TextSecondary
            };
            bar.AddRight(shellChipEthercat, 0);
            bar.AddRight(AddShellBarButton(buttonEthercatConnect, "연결", 52), 6);
            bar.AddRight(AddShellBarButton(buttonEthercatDisconnect, "해제", 52), 4);
            bar.AddDivider();

            shellChipServo = new ShellChip
            {
                Name = "shellChipServo",
                MaxChipWidth = 200,
                TextProvider = () => ShellServoText(labelServoStatus?.Text),
                DotProvider = () => labelServoStatus?.ForeColor ?? UiTheme.TextSecondary
            };
            bar.AddRight(shellChipServo, 12);
            bar.AddRight(AddShellBarButton(buttonServoOn, "서보 ON", 68), 6);
            bar.AddRight(AddShellBarButton(buttonServoOff, "서보 OFF", 72), 4);
            bar.AddDivider();

            shellChipAlarm = new ShellChip
            {
                Name = "shellChipAlarm",
                MaxChipWidth = 220,
                TextProvider = ShellAlarmText,
                DotProvider = ShellAlarmDot
            };
            bar.AddRight(shellChipAlarm, 12);

            shellClock = new ShellClock { Name = "shellClock" };
            bar.AddRight(shellClock, 14);

            // 오른쪽 끝 3색 램프: 장비 그림의 램프 표시와 같은 색을 읽습니다.
            shellTowerLamp = new ShellTowerLamp
            {
                Name = "shellTowerLamp",
                RedProvider = () => panelMainLampRed?.BackColor ?? EquipmentCanvasStyler.LampOff,
                YellowProvider = () => panelMainLampYellow?.BackColor ?? EquipmentCanvasStyler.LampOff,
                GreenProvider = () => panelMainLampGreen?.BackColor ?? EquipmentCanvasStyler.LampOff
            };
            foreach (var lamp in new Control[] { panelMainLampRed, panelMainLampYellow, panelMainLampGreen })
            {
                if (lamp != null) lamp.BackColorChanged += (s, e) => shellTowerLamp?.Invalidate();
            }
            bar.AddRight(shellTowerLamp, 8);

            return bar;
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
        /// 기존 메뉴 버튼 객체를 그대로 옮겨 쓰므로 클릭 동작은 같습니다.
        /// </summary>
        private Control BuildShellBottomBar()
        {
            var bar = new TableLayoutPanel
            {
                Name = "tableShellBottomBar",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = new Padding(16, 8, 16, 8),
                ColumnCount = 3,
                RowCount = 1,
                BackColor = UiTheme.Surface
            };
            bar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            bar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            bar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            bar.Paint += (s, e) =>
            {
                using (var pen = new Pen(UiTheme.Border))
                {
                    e.Graphics.DrawLine(pen, 0, 0, bar.Width, 0);
                }
            };

            var menu = new FlowLayoutPanel
            {
                Name = "flowShellNavMenu",
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = UiTheme.Surface
            };
            AddShellNavItem(menu, buttonNavOperate, "운전", ShellNavIcon.Operate);
            AddShellNavItem(menu, buttonNavRecipe, "레시피", ShellNavIcon.Recipe);
            AddShellNavItem(menu, buttonNavMaintenance, "유지보수", ShellNavIcon.Maintenance);
            AddShellNavItem(menu, buttonNavConfig, "설정", ShellNavIcon.Config);
            AddShellNavItem(menu, buttonNavTrend, "추이", ShellNavIcon.Trend);
            AddShellNavItem(menu, buttonNavReport, "보고서", ShellNavIcon.Report);

            shellStatusLine = new ShellStatusLine
            {
                Name = "shellStatusLine",
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 0, 10, 0),
                TextProvider = () => labelFoupSummaryInfo?.Text ?? ""
            };
            if (labelFoupSummaryInfo != null)
            {
                labelFoupSummaryInfo.TextChanged += (s, e) => shellStatusLine?.Invalidate();
            }

            var system = new FlowLayoutPanel
            {
                Name = "flowShellNavSystem",
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = UiTheme.Surface
            };
            AddShellNavItem(system, buttonNavSystem, "시스템", ShellNavIcon.System);
            if (buttonNavSystem != null) buttonNavSystem.Margin = Padding.Empty;

            bar.Controls.Add(menu, 0, 0);
            bar.Controls.Add(shellStatusLine, 1, 0);
            bar.Controls.Add(system, 2, 0);
            bar.SizeChanged += (s, e) => FitShellNavItems(bar);
            return bar;
        }

        private static readonly string[] ShellStateTexts = { "비상정지", "공정 진행 중", "일시 정지", "경고 · 정지", "대기" };

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

        private static void AddShellNavItem(FlowLayoutPanel stack, Button button, string text, ShellNavIcon icon)
        {
            if (button == null) return;
            ShellNavPainter.StyleNavBarItem(button, text, icon, new Size(128, 48));
            button.Margin = new Padding(0, 0, 8, 0);
            stack.Controls.Add(button);
        }

        private Control BuildShellSubTabs()
        {
            var bar = new Panel
            {
                Name = "panelShellSubTabs",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                BackColor = UiTheme.AppBack
            };

            var segment = new FlowLayoutPanel
            {
                Name = "flowShellSubTabs",
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(3),
                BackColor = UiTheme.Surface,
                Location = new Point(3, 8)
            };
            segment.Paint += (s, e) =>
            {
                using (var pen = new Pen(UiTheme.Border))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, segment.Width - 1, segment.Height - 1);
                }
            };

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
                segment.Controls.Add(pair.Item1);
            }

            bar.Controls.Add(segment);
            return bar;
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
            shellToolTip?.SetToolTip(shellChipAlarm, labelHeaderEventMessage?.Text ?? "");
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
