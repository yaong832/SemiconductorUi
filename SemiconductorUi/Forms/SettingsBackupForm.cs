using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.Helpers;
using SemiconductorUi.Services;

namespace SemiconductorUi.Forms
{
    /// <summary>
    /// 설정 백업·복원 창: 티칭 위치, 레시피, 알람 임계값, 사용자, 프로그램 설정 파일을 한 번에 보관하고 되돌립니다.
    /// 복원은 바로 하지 않고 다음에 프로그램을 켤 때 합니다 (지금 쓰고 있는 값과 파일이 어긋나지 않게).
    /// 모달이 아니라서 떠 있는 동안에도 메인 화면의 긴급 정지를 그대로 누를 수 있습니다.
    /// </summary>
    internal sealed class SettingsBackupForm : Form
    {
        private static readonly Color NoticeBack = Color.FromArgb(255, 246, 222);

        private readonly Func<bool> isLoggedIn;
        private readonly Func<string> currentUser;
        private readonly Func<bool> isAdmin;
        private readonly Action<string, string> log;

        private readonly TextBox textMemo;
        private readonly Label labelStatus;
        private readonly Panel panelPending;
        private readonly Label labelPending;
        private readonly ListView listBackups;
        private readonly FlowLayoutPanel panelChecks;
        private readonly List<CheckBox> componentChecks = new List<CheckBox>();
        private readonly Button buttonRestore;
        private readonly Panel panelConfirm;
        private readonly Label labelConfirm;
        private readonly Button buttonConfirm;

        private List<SettingsBackupInfo> backups = new List<SettingsBackupInfo>();
        private SettingsBackupInfo confirmBackup;
        private List<string> confirmKeys;

        public SettingsBackupForm(Func<bool> isLoggedIn, Func<string> currentUser, Func<bool> isAdmin, Action<string, string> log)
        {
            this.isLoggedIn = isLoggedIn ?? (() => false);
            this.currentUser = currentUser ?? (() => "");
            this.isAdmin = isAdmin ?? (() => false);
            this.log = log;

            Text = "설정 백업 · 복원";
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = UiTheme.Surface;
            Font = new Font("Segoe UI", 9F);
            KeyPreview = true;
            ClientSize = new Size(1040, 640);
            MinimumSize = new Size(860, 480);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(12),
                BackColor = UiTheme.Surface
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));

            // 1줄: 지금 백업
            var top = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0)
            };
            var buttonBackup = new Button { Text = "지금 백업", Size = new Size(100, 32), Margin = new Padding(0, 1, 12, 0) };
            UiTheme.StylePrimaryButton(buttonBackup);
            var labelMemo = new Label { AutoSize = false, Size = new Size(40, 32), TextAlign = ContentAlignment.MiddleLeft, Text = "메모" };
            textMemo = new TextBox { Width = 260, MaxLength = 100, Margin = new Padding(0, 6, 16, 0) };
            var buttonRefresh = new Button { Text = "새로고침", Size = new Size(90, 32), Margin = new Padding(0, 1, 6, 0) };
            UiTheme.StyleSecondaryButton(buttonRefresh);
            var buttonFolder = new Button { Text = "폴더 열기", Size = new Size(90, 32), Margin = new Padding(0, 1, 16, 0) };
            UiTheme.StyleSecondaryButton(buttonFolder);
            labelStatus = new Label { AutoSize = false, Size = new Size(420, 32), TextAlign = ContentAlignment.MiddleLeft, ForeColor = UiTheme.TextSecondary };
            top.Controls.AddRange(new Control[] { buttonBackup, labelMemo, textMemo, buttonRefresh, buttonFolder, labelStatus });
            layout.Controls.Add(top, 0, 0);

            // 2줄: 복원 예약 (있을 때만)
            panelPending = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 40,
                BackColor = NoticeBack,
                Padding = new Padding(8, 4, 8, 4),
                Margin = new Padding(0, 4, 0, 0),
                Visible = false
            };
            labelPending = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = UiTheme.TextPrimary };
            var buttonCancelPending = new Button { Text = "예약 취소", Dock = DockStyle.Right, Width = 96 };
            UiTheme.StyleSecondaryButton(buttonCancelPending);
            panelPending.Controls.Add(labelPending);
            panelPending.Controls.Add(buttonCancelPending);
            layout.Controls.Add(panelPending, 0, 1);

            // 3줄: 백업 목록
            listBackups = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                HideSelection = false,
                GridLines = true,
                Margin = new Padding(0, 6, 0, 6)
            };
            listBackups.Columns.Add("시각", 150);
            listBackups.Columns.Add("종류", 70);
            listBackups.Columns.Add("사용자", 110);
            listBackups.Columns.Add("메모", 300);
            listBackups.Columns.Add("들어 있는 항목", 380);
            layout.Controls.Add(listBackups, 0, 2);

            // 4줄: 되돌릴 항목 고르기 (확인 문구가 같은 자리에 뜸)
            var restoreRow = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            var restoreBox = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.SurfaceAlt, Padding = new Padding(8, 6, 8, 6) };
            var labelRestore = new Label
            {
                Dock = DockStyle.Top,
                Height = 24,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "고른 백업에서 되돌릴 항목 (다음에 프로그램을 켤 때 적용)"
            };
            panelChecks = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.Transparent
            };
            foreach (var c in SettingsBackup.Components)
            {
                var check = new CheckBox
                {
                    Text = c.Name,
                    Tag = c.Key,
                    AutoSize = true,
                    Margin = new Padding(0, 6, 18, 0),
                    Enabled = false
                };
                check.CheckedChanged += (s, e) => UpdateRestoreButton();
                componentChecks.Add(check);
                panelChecks.Controls.Add(check);
            }
            buttonRestore = new Button { Text = "다음 시작 때 복원", Size = new Size(150, 34), Margin = new Padding(0, 20, 0, 0), Enabled = false };
            UiTheme.StylePrimaryButton(buttonRestore);
            var restoreButtonHolder = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 160, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = Color.Transparent };
            restoreButtonHolder.Controls.Add(buttonRestore);
            restoreBox.Controls.Add(panelChecks);
            restoreBox.Controls.Add(labelRestore);
            restoreBox.Controls.Add(restoreButtonHolder);
            restoreRow.Controls.Add(restoreBox);

            panelConfirm = new Panel { Dock = DockStyle.Fill, BackColor = NoticeBack, Visible = false, Padding = new Padding(8, 4, 8, 4) };
            labelConfirm = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = UiTheme.TextPrimary };
            var confirmButtons = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 190, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = Color.Transparent };
            var buttonCancel = new Button { Text = "취소", Size = new Size(80, 30), Margin = new Padding(4, 26, 0, 0) };
            UiTheme.StyleSecondaryButton(buttonCancel);
            buttonConfirm = new Button { Text = "예약", Size = new Size(80, 30), Margin = new Padding(4, 26, 0, 0) };
            UiTheme.StylePrimaryButton(buttonConfirm);
            confirmButtons.Controls.Add(buttonCancel);
            confirmButtons.Controls.Add(buttonConfirm);
            panelConfirm.Controls.Add(labelConfirm);
            panelConfirm.Controls.Add(confirmButtons);
            restoreRow.Controls.Add(panelConfirm);
            layout.Controls.Add(restoreRow, 0, 3);

            // 5줄: 안내
            var labelNote = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UiTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "자동 백업은 프로그램을 켤 때 하루 한 번 하고 최근 " + AppSettings.AutoBackupKeepCount + "개만 남깁니다. 수동 백업과 복원 전 백업은 지우지 않습니다.\n" +
                       "복원하면 그 전에 지금 설정을 \"복원전\" 백업으로 먼저 남깁니다. 백업 폴더: " + SettingsBackup.BackupRoot
            };
            layout.Controls.Add(labelNote, 0, 4);
            Controls.Add(layout);

            buttonBackup.Click += (s, e) => BackupNow();
            buttonRefresh.Click += (s, e) => RefreshAll();
            buttonFolder.Click += (s, e) => OpenFolder();
            buttonCancelPending.Click += (s, e) => CancelPending();
            listBackups.SelectedIndexChanged += (s, e) => OnSelectionChanged();
            buttonRestore.Click += (s, e) => AskRestore();
            buttonConfirm.Click += (s, e) => ConfirmRestore();
            buttonCancel.Click += (s, e) => CloseConfirm("취소했습니다.");
            VisibleChanged += (s, e) =>
            {
                if (Visible)
                {
                    CloseConfirm(null);
                    RefreshAll();
                }
            };
        }

        public void RefreshAll()
        {
            string selected = SelectedBackup?.Folder;
            try
            {
                backups = SettingsBackup.ListBackups();
            }
            catch (Exception ex)
            {
                backups = new List<SettingsBackupInfo>();
                ShowStatus($"백업 목록을 읽지 못했습니다: {ex.Message}", UiTheme.Danger);
            }

            listBackups.BeginUpdate();
            try
            {
                listBackups.Items.Clear();
                foreach (var b in backups)
                {
                    var item = new ListViewItem(b.Time.ToString("yyyy-MM-dd HH:mm:ss")) { Tag = b };
                    item.SubItems.Add(b.Kind);
                    item.SubItems.Add(b.User);
                    item.SubItems.Add(b.Note);
                    item.SubItems.Add(string.Join(", ", b.Components.Select(SettingsBackup.ComponentName)));
                    listBackups.Items.Add(item);
                    if (b.Folder == selected)
                    {
                        item.Selected = true;
                    }
                }
            }
            finally
            {
                listBackups.EndUpdate();
            }

            RefreshPending();
            OnSelectionChanged();
        }

        private SettingsBackupInfo SelectedBackup =>
            listBackups.SelectedItems.Count > 0 ? listBackups.SelectedItems[0].Tag as SettingsBackupInfo : null;

        private void RefreshPending()
        {
            PendingRestoreInfo pending = null;
            try
            {
                pending = SettingsBackup.GetPendingRestore();
            }
            catch
            {
                // 읽지 못하면 예약 없음으로 보여 줍니다. 켤 때 다시 확인합니다.
            }

            if (pending == null)
            {
                panelPending.Visible = false;
                return;
            }

            labelPending.Text =
                $"복원 예약됨: 백업 {Path.GetFileName(pending.BackupFolder)}의 {string.Join(", ", pending.Components.Select(SettingsBackup.ComponentName))} " +
                $"(요청 {pending.User}, {pending.RequestedAt:MM-dd HH:mm}). 프로그램을 끄고 다시 켜면 적용됩니다.";
            panelPending.Visible = true;
        }

        private void OnSelectionChanged()
        {
            var b = SelectedBackup;
            foreach (var check in componentChecks)
            {
                bool has = b != null && b.Components.Contains((string)check.Tag);
                check.Enabled = has;
                if (!has)
                {
                    check.Checked = false;
                }
            }

            UpdateRestoreButton();
        }

        private void UpdateRestoreButton()
        {
            buttonRestore.Enabled = SelectedBackup != null && componentChecks.Any(c => c.Enabled && c.Checked);
        }

        private void BackupNow()
        {
            CloseConfirm(null);
            if (!isLoggedIn())
            {
                ShowStatus("백업은 로그인한 뒤에 할 수 있습니다.", UiTheme.Danger);
                return;
            }

            try
            {
                var info = SettingsBackup.CreateBackup(SettingsBackup.KindManual, currentUser(), textMemo.Text.Trim(), DateTime.Now);
                log?.Invoke($"설정 수동 백업: {info.Folder} ({info.Components.Count}개 파일, 사용자 {info.User})", "INFO");
                textMemo.Clear();
                RefreshAll();
                SelectBackup(info.Folder);
                ShowStatus($"백업했습니다: {info.Name}", UiTheme.Success);
            }
            catch (Exception ex)
            {
                ShowStatus($"백업하지 못했습니다: {ex.Message}", UiTheme.Danger);
                log?.Invoke($"설정 수동 백업 실패: {ex.Message}", "WARN");
            }
        }

        private void SelectBackup(string folder)
        {
            foreach (ListViewItem item in listBackups.Items)
            {
                if ((item.Tag as SettingsBackupInfo)?.Folder == folder)
                {
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();
                    return;
                }
            }
        }

        private void AskRestore()
        {
            var b = SelectedBackup;
            if (b == null)
            {
                return;
            }

            if (!isAdmin())
            {
                ShowStatus("복원은 관리자로 로그인한 뒤에 할 수 있습니다.", UiTheme.Danger);
                return;
            }

            var keys = componentChecks.Where(c => c.Enabled && c.Checked).Select(c => (string)c.Tag).ToList();
            if (keys.Count == 0)
            {
                return;
            }

            confirmBackup = b;
            confirmKeys = keys;
            string text = $"백업 {b.Time:yyyy-MM-dd HH:mm:ss} ({b.Kind})의 {string.Join(", ", keys.Select(SettingsBackup.ComponentName))}을(를) " +
                          "다음에 프로그램을 켤 때 되돌릴까요? 지금 설정은 그때 \"복원전\" 백업으로 먼저 남깁니다.";
            if (keys.Contains(SettingsBackup.KeyTeaching))
            {
                text += "\n티칭 위치를 되돌린 뒤에는 장비를 움직이기 전에 티칭 값을 확인해 주세요.";
            }
            if (keys.Contains(SettingsBackup.KeyUsers))
            {
                text += "\n사용자를 되돌리면 비밀번호도 그 백업 때로 돌아갑니다.";
            }

            labelConfirm.Text = text;
            panelConfirm.Visible = true;
            panelConfirm.BringToFront();
            buttonConfirm.Focus();
        }

        private void ConfirmRestore()
        {
            var b = confirmBackup;
            var keys = confirmKeys;
            CloseConfirm(null);
            if (b == null || keys == null)
            {
                return;
            }

            if (!isAdmin())
            {
                ShowStatus("복원은 관리자로 로그인한 뒤에 할 수 있습니다.", UiTheme.Danger);
                return;
            }

            try
            {
                string user = currentUser();
                bool replaced = SettingsBackup.GetPendingRestore() != null;
                SettingsBackup.ScheduleRestore(b, keys, user, DateTime.Now);
                log?.Invoke($"설정 복원 예약: {string.Join(", ", keys.Select(SettingsBackup.ComponentName))} ← 백업 {b.Name} (사용자 {user}). 다음 시작 때 적용", "WARN");
                foreach (var check in componentChecks)
                {
                    check.Checked = false;
                }
                RefreshPending();
                ShowStatus(replaced
                    ? "전에 있던 예약 대신 이 복원을 예약했습니다. 프로그램을 끄고 다시 켜면 적용됩니다."
                    : "복원을 예약했습니다. 프로그램을 끄고 다시 켜면 적용됩니다.", UiTheme.Success);
            }
            catch (Exception ex)
            {
                ShowStatus($"복원을 예약하지 못했습니다: {ex.Message}", UiTheme.Danger);
            }
        }

        private void CancelPending()
        {
            CloseConfirm(null);
            if (!isAdmin())
            {
                ShowStatus("예약 취소는 관리자로 로그인한 뒤에 할 수 있습니다.", UiTheme.Danger);
                return;
            }

            try
            {
                if (SettingsBackup.CancelPendingRestore())
                {
                    log?.Invoke($"설정 복원 예약 취소 (사용자 {currentUser()})", "INFO");
                    ShowStatus("복원 예약을 취소했습니다.", UiTheme.Success);
                }
                RefreshPending();
            }
            catch (Exception ex)
            {
                ShowStatus($"예약을 취소하지 못했습니다: {ex.Message}", UiTheme.Danger);
            }
        }

        private void CloseConfirm(string message)
        {
            confirmBackup = null;
            confirmKeys = null;
            panelConfirm.Visible = false;
            if (message != null)
            {
                ShowStatus(message, UiTheme.TextSecondary);
            }
        }

        private void ShowStatus(string text, Color color)
        {
            labelStatus.ForeColor = color;
            labelStatus.Text = text;
        }

        private void OpenFolder()
        {
            try
            {
                string folder = SelectedBackup?.Folder ?? SettingsBackup.BackupRoot;
                Directory.CreateDirectory(folder);
                System.Diagnostics.Process.Start("explorer.exe", "\"" + folder + "\"");
            }
            catch (Exception ex)
            {
                ShowStatus($"폴더를 열 수 없습니다: {ex.Message}", UiTheme.Danger);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                if (panelConfirm.Visible)
                {
                    CloseConfirm("취소했습니다.");
                }
                else
                {
                    Hide();
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // X를 눌러도 숨기기만 합니다. 프로그램이 끝날 때는 그대로 닫힙니다.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                CloseConfirm(null);
                Hide();
                return;
            }

            base.OnFormClosing(e);
        }
    }
}
