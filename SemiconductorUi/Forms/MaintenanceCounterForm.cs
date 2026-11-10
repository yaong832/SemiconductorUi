using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.Helpers;
using SemiconductorUi.Services;

namespace SemiconductorUi.Forms
{
    /// <summary>
    /// 유지보수 카운터 창: 실린더·진공·배기·도어·축의 동작 횟수, 점검 주기, 점검 이력을 보여 줍니다.
    /// 메인 화면을 잠그지 않으므로(모달 아님), 떠 있는 동안에도 긴급 정지를 그대로 누를 수 있습니다.
    /// 확인 질문도 창 안에서 묻습니다 (메시지 상자를 띄우지 않음).
    /// 점검 완료는 로그인한 사용자, 점검 주기 변경은 관리자만 할 수 있습니다.
    /// </summary>
    internal sealed class MaintenanceCounterForm : Form
    {
        private const int RefreshIntervalMs = 1000;
        private const int HistoryRows = 200;
        private static readonly Color DueRowBack = Color.FromArgb(253, 236, 237);
        private static readonly Color SoonRowBack = Color.FromArgb(254, 245, 228);

        private readonly MaintenanceCounterService service;
        private readonly Func<bool> isLoggedIn;
        private readonly Func<string> currentUser;
        private readonly Func<bool> isAdmin;

        private readonly BufferedListView listCounters;
        private readonly BufferedListView listHistory;
        private readonly Label labelSelected;
        private readonly NumericUpDown numericLimit;
        private readonly Button buttonSaveLimit;
        private readonly TextBox textNote;
        private readonly Button buttonServiced;
        private readonly Panel panelConfirm;
        private readonly Label labelConfirm;
        private readonly Button buttonConfirm;
        private readonly Button buttonCancel;
        private readonly Label labelMessage;
        private readonly Timer refreshTimer;

        private string pendingServiceKey;
        private int tickCount;

        public MaintenanceCounterForm(MaintenanceCounterService service, Func<bool> isLoggedIn, Func<string> currentUser, Func<bool> isAdmin)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.isLoggedIn = isLoggedIn ?? (() => false);
            this.currentUser = currentUser ?? (() => "");
            this.isAdmin = isAdmin ?? (() => false);

            Text = "유지보수 카운터";
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = UiTheme.Surface;
            Font = new Font("Segoe UI", 9F);
            KeyPreview = true;
            ClientSize = new Size(1040, 640);
            MinimumSize = new Size(880, 560);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(12),
                BackColor = UiTheme.Surface
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 52F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 48F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            var labelTitle = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "동작 횟수 · 실제 장비로 보낸 명령만 셉니다 (시뮬레이션은 세지 않음)"
            };
            layout.Controls.Add(labelTitle, 0, 0);

            listCounters = new BufferedListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                HideSelection = false,
                GridLines = true,
                Margin = new Padding(0, 2, 0, 4)
            };
            listCounters.Columns.Add("항목", 150);
            listCounters.Columns.Add("점검 후 횟수", 105, HorizontalAlignment.Right);
            listCounters.Columns.Add("점검 주기", 105, HorizontalAlignment.Right);
            listCounters.Columns.Add("상태", 135);
            listCounters.Columns.Add("누적 횟수", 105, HorizontalAlignment.Right);
            listCounters.Columns.Add("마지막 점검", 175);
            listCounters.Columns.Add("세는 기준", 205);
            listCounters.SelectedIndexChanged += (s, e) => OnSelectionChanged();
            layout.Controls.Add(listCounters, 0, 1);

            // 선택한 항목: 점검 주기, 메모, 점검 완료
            var editRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0)
            };
            labelSelected = new Label
            {
                AutoSize = false,
                Size = new Size(170, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                Text = "항목을 고르세요"
            };
            var labelLimit = new Label { AutoSize = false, Size = new Size(64, 30), TextAlign = ContentAlignment.MiddleRight, Text = "점검 주기" };
            numericLimit = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 1000000000,
                Increment = 1000,
                ThousandsSeparator = true,
                Size = new Size(120, 26),
                Margin = new Padding(4, 5, 2, 0),
                TextAlign = HorizontalAlignment.Right
            };
            var labelLimitUnit = new Label { AutoSize = false, Size = new Size(84, 30), TextAlign = ContentAlignment.MiddleLeft, Text = "회 (0 = 없음)", ForeColor = UiTheme.TextSecondary };
            buttonSaveLimit = new Button { Text = "주기 저장", Size = new Size(84, 30), Margin = new Padding(2, 1, 16, 0) };
            UiTheme.StyleSecondaryButton(buttonSaveLimit);
            buttonSaveLimit.Click += (s, e) => SaveLimit();

            var labelNote = new Label { AutoSize = false, Size = new Size(36, 30), TextAlign = ContentAlignment.MiddleRight, Text = "메모" };
            textNote = new TextBox { Size = new Size(190, 26), Margin = new Padding(4, 5, 6, 0), MaxLength = 200 };
            buttonServiced = new Button { Text = "점검 완료", Size = new Size(92, 30), Margin = new Padding(2, 1, 0, 0) };
            UiTheme.StylePrimaryButton(buttonServiced);
            buttonServiced.Click += (s, e) => AskServiced();

            editRow.Controls.AddRange(new Control[] { labelSelected, labelLimit, numericLimit, labelLimitUnit, buttonSaveLimit, labelNote, textNote, buttonServiced });
            layout.Controls.Add(editRow, 0, 2);

            // 확인 질문 (창 안) 또는 결과 메시지
            var messageRow = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            labelMessage = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = UiTheme.TextSecondary };
            panelConfirm = new Panel { Dock = DockStyle.Fill, BackColor = SoonRowBack, Visible = false, Padding = new Padding(8, 4, 8, 4) };
            labelConfirm = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = UiTheme.TextPrimary };
            var confirmButtons = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 190, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = Color.Transparent };
            buttonCancel = new Button { Text = "취소", Size = new Size(80, 28), Margin = new Padding(4, 0, 0, 0) };
            UiTheme.StyleSecondaryButton(buttonCancel);
            buttonCancel.Click += (s, e) => CancelConfirm("취소했습니다.");
            buttonConfirm = new Button { Text = "확인", Size = new Size(80, 28), Margin = new Padding(4, 0, 0, 0) };
            UiTheme.StylePrimaryButton(buttonConfirm);
            buttonConfirm.Click += (s, e) => ConfirmServiced();
            confirmButtons.Controls.Add(buttonCancel);
            confirmButtons.Controls.Add(buttonConfirm);
            panelConfirm.Controls.Add(labelConfirm);
            panelConfirm.Controls.Add(confirmButtons);
            messageRow.Controls.Add(panelConfirm);
            messageRow.Controls.Add(labelMessage);
            layout.Controls.Add(messageRow, 0, 3);

            var historyHeader = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 4, 0, 0) };
            var labelHistory = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = $"점검 이력 (최근 {HistoryRows}건)"
            };
            var buttonFolder = new Button { Text = "폴더 열기", Dock = DockStyle.Right, Width = 90 };
            UiTheme.StyleSecondaryButton(buttonFolder);
            buttonFolder.Click += (s, e) => OpenFolder();
            historyHeader.Controls.Add(labelHistory);
            historyHeader.Controls.Add(buttonFolder);
            layout.Controls.Add(historyHeader, 0, 4);

            listHistory = new BufferedListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true,
                Margin = new Padding(0, 2, 0, 4)
            };
            listHistory.Columns.Add("시각", 140);
            listHistory.Columns.Add("구분", 80);
            listHistory.Columns.Add("항목", 140);
            listHistory.Columns.Add("횟수", 100, HorizontalAlignment.Right);
            listHistory.Columns.Add("주기", 100, HorizontalAlignment.Right);
            listHistory.Columns.Add("사용자", 90);
            listHistory.Columns.Add("메모", 320);
            layout.Controls.Add(listHistory, 0, 5);

            var labelFoot = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UiTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "점검 주기에 도달하면 경고 로그만 남기고 장비는 멈추지 않습니다. 점검 완료는 로그인한 사용자, 점검 주기 변경은 관리자만 할 수 있습니다.\n" +
                       "점검 완료는 '점검 후 횟수'만 0으로 되돌리고 누적 횟수는 그대로 둡니다."
            };
            layout.Controls.Add(labelFoot, 0, 6);

            Controls.Add(layout);

            refreshTimer = new Timer { Interval = RefreshIntervalMs };
            refreshTimer.Tick += (s, e) =>
            {
                tickCount++;
                RefreshCounters();
                if (tickCount % 5 == 0)
                {
                    RefreshHistory();
                }
            };
            VisibleChanged += (s, e) =>
            {
                if (Visible)
                {
                    RefreshAll();
                    refreshTimer.Start();
                }
                else
                {
                    refreshTimer.Stop();
                    CancelConfirm(null);
                }
            };

            RefreshAll();
            UpdateEditState();
        }

        public void RefreshAll()
        {
            RefreshCounters();
            RefreshHistory();
        }

        private void RefreshCounters()
        {
            var counters = service.GetCounters();
            listCounters.BeginUpdate();
            try
            {
                while (listCounters.Items.Count > counters.Count)
                {
                    listCounters.Items.RemoveAt(listCounters.Items.Count - 1);
                }

                for (int i = 0; i < counters.Count; i++)
                {
                    var c = counters[i];
                    string[] texts =
                    {
                        c.Name,
                        c.SinceService.ToString("N0"),
                        MaintenanceCounterService.FormatLimit(c.Limit),
                        StatusText(c),
                        c.Total.ToString("N0"),
                        c.LastServiceTime.HasValue
                            ? $"{c.LastServiceTime.Value:yyyy-MM-dd HH:mm}{(string.IsNullOrEmpty(c.LastServiceUser) ? "" : " · " + c.LastServiceUser)}"
                            : "기록 없음",
                        c.Source
                    };

                    ListViewItem item;
                    if (i < listCounters.Items.Count)
                    {
                        item = listCounters.Items[i];
                    }
                    else
                    {
                        item = new ListViewItem(texts[0]) { UseItemStyleForSubItems = false };
                        for (int k = 1; k < texts.Length; k++)
                        {
                            item.SubItems.Add(texts[k]);
                        }
                        listCounters.Items.Add(item);
                    }

                    item.Tag = c.Key;
                    for (int k = 0; k < texts.Length; k++)
                    {
                        if (item.SubItems[k].Text != texts[k])
                        {
                            item.SubItems[k].Text = texts[k];
                        }
                    }

                    var status = c.Status;
                    Color back = status == MaintenanceCounterStatus.Due ? DueRowBack
                        : status == MaintenanceCounterStatus.Soon ? SoonRowBack
                        : UiTheme.Surface;
                    Color statusFore = status == MaintenanceCounterStatus.Due ? UiTheme.Danger
                        : status == MaintenanceCounterStatus.Soon ? UiTheme.Warning
                        : status == MaintenanceCounterStatus.Normal ? UiTheme.Success
                        : UiTheme.TextSecondary;
                    for (int k = 0; k < item.SubItems.Count; k++)
                    {
                        var sub = item.SubItems[k];
                        if (sub.BackColor != back)
                        {
                            sub.BackColor = back;
                        }

                        Color fore = k == 3 ? statusFore : UiTheme.TextPrimary;
                        if (sub.ForeColor != fore)
                        {
                            sub.ForeColor = fore;
                        }
                    }
                }
            }
            finally
            {
                listCounters.EndUpdate();
            }
        }

        internal static string StatusText(MaintenanceCounterState c)
        {
            int percent = (int)Math.Floor(c.Ratio * 100);
            switch (c.Status)
            {
                case MaintenanceCounterStatus.Due:
                    return $"점검 필요 · {percent}%";
                case MaintenanceCounterStatus.Soon:
                    return $"곧 점검 · {percent}%";
                case MaintenanceCounterStatus.Normal:
                    return $"정상 · {percent}%";
                default:
                    return "주기 없음";
            }
        }

        private void RefreshHistory()
        {
            var rows = service.ReadHistory(HistoryRows);
            listHistory.BeginUpdate();
            try
            {
                listHistory.Items.Clear();
                foreach (var r in rows)
                {
                    var item = new ListViewItem(r.Time.ToString("yyyy-MM-dd HH:mm:ss"));
                    item.SubItems.Add(r.Event);
                    item.SubItems.Add(r.Item);
                    item.SubItems.Add(FormatNumber(r.Count));
                    item.SubItems.Add(long.TryParse(r.Limit, out long limit) ? MaintenanceCounterService.FormatLimit(limit) : r.Limit);
                    item.SubItems.Add(r.User);
                    item.SubItems.Add(r.Note);
                    if (r.Event == MaintenanceCounterService.EventDue)
                    {
                        item.ForeColor = UiTheme.Danger;
                    }
                    listHistory.Items.Add(item);
                }
            }
            finally
            {
                listHistory.EndUpdate();
            }
        }

        private static string FormatNumber(string text)
        {
            return long.TryParse(text, out long v) ? v.ToString("N0") : text;
        }

        private MaintenanceCounterState SelectedCounter()
        {
            if (listCounters.SelectedItems.Count == 0)
            {
                return null;
            }

            string key = listCounters.SelectedItems[0].Tag as string;
            return service.GetCounters().FirstOrDefault(c => c.Key == key);
        }

        private void OnSelectionChanged()
        {
            CancelConfirm(null);
            var c = SelectedCounter();
            if (c != null)
            {
                numericLimit.Value = Math.Min(numericLimit.Maximum, c.Limit);
                textNote.Text = "";
            }
            UpdateEditState();
        }

        private void UpdateEditState()
        {
            var c = SelectedCounter();
            bool selected = c != null;
            labelSelected.Text = selected ? c.Name : "항목을 고르세요";
            numericLimit.Enabled = selected;
            buttonSaveLimit.Enabled = selected;
            textNote.Enabled = selected;
            buttonServiced.Enabled = selected;
        }

        private void SaveLimit()
        {
            var c = SelectedCounter();
            if (c == null)
            {
                return;
            }

            if (!isLoggedIn() || !isAdmin())
            {
                ShowMessage("점검 주기 변경은 관리자로 로그인한 뒤에 할 수 있습니다.", UiTheme.Danger);
                return;
            }

            long limit = (long)numericLimit.Value;
            if (limit == c.Limit)
            {
                ShowMessage($"{c.Name}: 점검 주기가 이미 {MaintenanceCounterService.FormatLimit(limit)}입니다.", UiTheme.TextSecondary);
                return;
            }

            service.SetLimit(c.Key, limit, currentUser(), DateTime.Now);
            ShowMessage($"{c.Name}: 점검 주기를 {MaintenanceCounterService.FormatLimit(c.Limit)} → {MaintenanceCounterService.FormatLimit(limit)}로 바꿨습니다.", UiTheme.Success);
            RefreshAll();
        }

        private void AskServiced()
        {
            var c = SelectedCounter();
            if (c == null)
            {
                return;
            }

            if (!isLoggedIn())
            {
                ShowMessage("점검 완료는 로그인한 뒤에 할 수 있습니다.", UiTheme.Danger);
                return;
            }

            pendingServiceKey = c.Key;
            labelConfirm.Text = $"{c.Name}: 점검 후 횟수 {c.SinceService:N0}회를 0으로 되돌리고 이력에 남길까요? (누적 {c.Total:N0}회는 그대로)";
            panelConfirm.Visible = true;
            panelConfirm.BringToFront();
            buttonConfirm.Focus();
        }

        private void ConfirmServiced()
        {
            string key = pendingServiceKey;
            var c = service.GetCounters().FirstOrDefault(x => x.Key == key);
            CancelConfirm(null);
            if (c == null)
            {
                return;
            }

            if (!isLoggedIn())
            {
                ShowMessage("점검 완료는 로그인한 뒤에 할 수 있습니다.", UiTheme.Danger);
                return;
            }

            service.MarkServiced(key, currentUser(), textNote.Text.Trim(), DateTime.Now);
            textNote.Text = "";
            ShowMessage($"{c.Name}: 점검 완료로 기록했습니다 ({c.SinceService:N0}회 → 0).", UiTheme.Success);
            RefreshAll();
        }

        private void CancelConfirm(string message)
        {
            pendingServiceKey = null;
            panelConfirm.Visible = false;
            if (message != null)
            {
                ShowMessage(message, UiTheme.TextSecondary);
            }
        }

        private void ShowMessage(string text, Color color)
        {
            labelMessage.Text = text;
            labelMessage.ForeColor = color;
        }

        private void OpenFolder()
        {
            try
            {
                System.IO.Directory.CreateDirectory(service.Folder);
                System.Diagnostics.Process.Start("explorer.exe", "\"" + service.Folder + "\"");
            }
            catch (Exception ex)
            {
                ShowMessage($"폴더를 열 수 없습니다: {ex.Message}", UiTheme.Danger);
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
                    CancelConfirm("취소했습니다.");
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
                Hide();
                return;
            }

            refreshTimer.Stop();
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                refreshTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>갱신할 때 깜박이지 않게 이중 버퍼를 켠 목록</summary>
        private sealed class BufferedListView : ListView
        {
            public BufferedListView()
            {
                DoubleBuffered = true;
            }
        }
    }
}
