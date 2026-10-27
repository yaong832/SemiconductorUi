using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.Controls;
using SemiconductorUi.Helpers;

namespace SemiconductorUi
{
    using ProcessState = ViewModels.MainFormViewModel.ProcessState;

    /// <summary>
    /// 메인 화면 틀: 맨 위 상단 바 한 줄 + 왼쪽 세로 메뉴 + 본문 위 작은 탭(모니터링/검증/이송).
    /// 기존 버튼(로그인, EtherCAT 연결, 서보 ON/OFF, 메뉴, 탭)은 같은 객체를 자리만 옮기므로
    /// 클릭 핸들러, 권한에 따른 Enabled, 이름으로 찾는 코드는 그대로 동작합니다.
    /// 기존 상단 표와 하단 메뉴 줄은 숨겨서 보관하고, 코드가 계속 바꾸는 상태 글자는 상단 바가 읽어 보여 줍니다.
    /// </summary>
    public partial class Form1
    {
        private const int ShellNavWidth = 100;
        private const int ShellSubTabHeight = 46;
        // 창이 이 폭 이상이면(2560x1440 최대화 등) 상단 바 글자와 버튼을 한 단계 키웁니다.
        private const int ShellLargeMinWidth = 2200;

        private ShellTopBar shellTopBar;
        private ShellChip shellChipEthercat;
        private ShellChip shellChipServo;
        private ShellChip shellChipAlarm;
        private ShellClock shellClock;
        private ShellUserCard shellUserCard;
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

            var body = new TableLayoutPanel
            {
                Name = "tableShellBody",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UiTheme.AppBack
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, ShellNavWidth));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var content = new TableLayoutPanel
            {
                Name = "tableShellContent",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = new Padding(6, 0, 4, 4),
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UiTheme.AppBack
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, ShellSubTabHeight));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            content.Controls.Add(BuildShellSubTabs(), 0, 0);
            content.Controls.Add(tableLayoutProcessArea, 0, 1);

            body.Controls.Add(BuildShellNav(), 0, 0);
            body.Controls.Add(content, 1, 0);

            tableLayoutRoot.RowStyles.Clear();
            tableLayoutRoot.RowCount = 2;
            tableLayoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, ShellTopBar.BarHeight));
            tableLayoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutRoot.Margin = Padding.Empty;
            tableLayoutRoot.Padding = Padding.Empty;
            tableLayoutRoot.Controls.Add(shellTopBar, 0, 0);
            tableLayoutRoot.Controls.Add(body, 0, 1);

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
            shellTopBar.Large = large;
            shellTopBar.ResumeLayout(true);
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
            var bar = new ShellTopBar { Name = "shellTopBar", StateProvider = GetShellStatePill };

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

        private Control BuildShellNav()
        {
            var nav = new Panel
            {
                Name = "panelShellNav",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = new Padding(10, 10, 10, 10),
                BackColor = UiTheme.Surface
            };
            nav.Paint += (s, e) =>
            {
                using (var pen = new Pen(UiTheme.Border))
                {
                    e.Graphics.DrawLine(pen, nav.Width - 1, 0, nav.Width - 1, nav.Height);
                }
            };

            var itemSize = new Size(ShellNavWidth - 20, 58);
            var top = NewShellNavStack("flowShellNavTop");
            top.Dock = DockStyle.Top;
            AddShellNavItem(top, buttonNavOperate, "운전", ShellNavIcon.Operate, itemSize);
            AddShellNavItem(top, buttonNavRecipe, "레시피", ShellNavIcon.Recipe, itemSize);
            AddShellNavItem(top, buttonNavMaintenance, "유지보수", ShellNavIcon.Maintenance, itemSize);
            AddShellNavItem(top, buttonNavConfig, "설정", ShellNavIcon.Config, itemSize);
            AddShellNavItem(top, buttonNavTrend, "추이", ShellNavIcon.Trend, itemSize);
            AddShellNavItem(top, buttonNavReport, "보고서", ShellNavIcon.Report, itemSize);

            var bottom = NewShellNavStack("flowShellNavBottom");
            bottom.Dock = DockStyle.Bottom;
            AddShellNavItem(bottom, buttonNavSystem, "시스템", ShellNavIcon.System, itemSize);
            bottom.Controls.Add(new Panel { Size = new Size(itemSize.Width, 1), BackColor = UiTheme.Border, Margin = new Padding(0, 4, 0, 10) });

            shellUserCard = new ShellUserCard
            {
                Name = "shellUserCard",
                Size = new Size(itemSize.Width, 66),
                Margin = new Padding(0, 0, 0, 6),
                UserProvider = () => CurrentUser,
                RoleProvider = () => CurrentRole,
                LoggedInProvider = () => IsLoggedIn
            };
            bottom.Controls.Add(shellUserCard);
            foreach (var pair in new[]
            {
                Tuple.Create(buttonLogin, "로그인"),
                Tuple.Create(buttonLogout, "로그아웃"),
                Tuple.Create(buttonUserManagement, "사용자 관리")
            })
            {
                var button = StyleShellBarButton(pair.Item1, pair.Item2, itemSize.Width);
                if (button == null) continue;
                button.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
                button.Margin = new Padding(0, 0, 0, 4);
                bottom.Controls.Add(button);
            }

            nav.Controls.Add(top);
            nav.Controls.Add(bottom);
            return nav;
        }

        private static FlowLayoutPanel NewShellNavStack(string name)
        {
            return new FlowLayoutPanel
            {
                Name = name,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = UiTheme.Surface
            };
        }

        private static void AddShellNavItem(FlowLayoutPanel stack, Button button, string text, ShellNavIcon icon, Size size)
        {
            if (button == null) return;
            ShellNavPainter.StyleNavItem(button, text, icon, size);
            button.Margin = new Padding(0, 0, 0, 4);
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
