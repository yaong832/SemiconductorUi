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
    /// 현재 알람과 날짜별 알람 이력을 보여 주는 창입니다.
    /// 메인 화면을 잠그지 않으므로(모달 아님), 떠 있는 동안에도 긴급 정지와 알람 리셋을 그대로 누를 수 있습니다.
    /// 보기 전용이며 장비에는 아무 명령도 보내지 않습니다. 알람 해제는 메인 화면의 알람 리셋으로 합니다.
    /// </summary>
    internal sealed class AlarmHistoryForm : Form
    {
        private static readonly Font BodyFont = new Font("Segoe UI", 9F);
        private static readonly Font TitleFont = new Font("Segoe UI", 10F, FontStyle.Bold);

        private readonly AlarmHistoryService service;
        private readonly Label labelActiveTitle;
        private readonly ListView listActive;
        private readonly ListView listHistory;
        private readonly DateTimePicker pickerDate;
        private readonly Label labelHistoryInfo;

        public AlarmHistoryForm(AlarmHistoryService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));

            Text = "알람";
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = UiTheme.Surface;
            Font = BodyFont;
            KeyPreview = true;
            ClientSize = new Size(960, 600);
            MinimumSize = new Size(640, 420);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(12),
                BackColor = UiTheme.Surface
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));

            labelActiveTitle = new Label
            {
                Dock = DockStyle.Fill,
                Font = TitleFont,
                ForeColor = UiTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "현재 알람"
            };

            listActive = CreateList();
            listActive.Columns.Add("코드", 70);
            listActive.Columns.Add("유닛", 100);
            listActive.Columns.Add("알람", 230);
            listActive.Columns.Add("상세", 360);
            listActive.Columns.Add("발생 시각", 150);

            var historyBar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 6, 0, 0),
                BackColor = UiTheme.Surface
            };
            var labelHistoryTitle = new Label
            {
                AutoSize = true,
                Font = TitleFont,
                ForeColor = UiTheme.TextPrimary,
                Text = "알람 이력",
                Margin = new Padding(0, 6, 12, 0)
            };
            pickerDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                Width = 120,
                Value = DateTime.Today,
                Margin = new Padding(0, 2, 6, 0)
            };
            pickerDate.ValueChanged += (s, e) => RefreshHistory();
            var buttonToday = CreateButton("오늘", 60, (s, e) => pickerDate.Value = DateTime.Today);
            var buttonRefresh = CreateButton("새로고침", 80, (s, e) => RefreshAll());
            var buttonFolder = CreateButton("폴더 열기", 84, (s, e) => OpenFolder());
            labelHistoryInfo = new Label
            {
                AutoSize = true,
                ForeColor = UiTheme.TextSecondary,
                Margin = new Padding(8, 7, 0, 0)
            };
            historyBar.Controls.Add(labelHistoryTitle);
            historyBar.Controls.Add(pickerDate);
            historyBar.Controls.Add(buttonToday);
            historyBar.Controls.Add(buttonRefresh);
            historyBar.Controls.Add(buttonFolder);
            historyBar.Controls.Add(labelHistoryInfo);

            listHistory = CreateList();
            listHistory.Columns.Add("시각", 100);
            listHistory.Columns.Add("구분", 50);
            listHistory.Columns.Add("코드", 60);
            listHistory.Columns.Add("유닛", 90);
            listHistory.Columns.Add("알람", 200);
            listHistory.Columns.Add("상세", 300);
            listHistory.Columns.Add("해제한 사용자", 90);

            var labelNote = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UiTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "알람 해제는 메인 화면의 [알람 리셋]으로 합니다. 이 창은 보기 전용입니다."
            };

            layout.Controls.Add(labelActiveTitle, 0, 0);
            layout.Controls.Add(listActive, 0, 1);
            layout.Controls.Add(historyBar, 0, 2);
            layout.Controls.Add(listHistory, 0, 3);
            layout.Controls.Add(labelNote, 0, 4);
            Controls.Add(layout);

            service.Changed += OnServiceChanged;
        }

        private static ListView CreateList()
        {
            return new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false,
                MultiSelect = false,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                BackColor = UiTheme.Surface,
                ForeColor = UiTheme.TextPrimary
            };
        }

        private static Button CreateButton(string text, int width, EventHandler onClick)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(width, 26),
                Margin = new Padding(0, 1, 6, 0)
            };
            UiTheme.StyleSecondaryButton(button);
            button.Click += onClick;
            return button;
        }

        /// <summary>
        /// 현재 알람과 이력을 다시 읽어 표시합니다.
        /// </summary>
        public void RefreshAll()
        {
            RefreshActive();
            RefreshHistory();
        }

        private void RefreshActive()
        {
            var active = service.GetActive();
            labelActiveTitle.Text = active.Count == 0 ? "현재 알람: 없음" : $"현재 알람: {active.Count}건";
            labelActiveTitle.ForeColor = active.Count == 0 ? UiTheme.TextPrimary : UiTheme.Danger;

            listActive.BeginUpdate();
            try
            {
                listActive.Items.Clear();
                // 최신 알람이 위로 오게 합니다.
                foreach (var record in active.OrderByDescending(a => a.SetTime))
                {
                    var item = new ListViewItem(record.Code.ToString());
                    item.SubItems.Add(record.Unit);
                    item.SubItems.Add(record.Name);
                    item.SubItems.Add(record.Detail);
                    item.SubItems.Add(record.SetTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    item.ForeColor = UiTheme.Danger;
                    listActive.Items.Add(item);
                }
            }
            finally
            {
                listActive.EndUpdate();
            }
        }

        private void RefreshHistory()
        {
            var date = pickerDate.Value.Date;
            List<AlarmHistoryRow> rows = service.ReadDay(date);

            listHistory.BeginUpdate();
            try
            {
                listHistory.Items.Clear();
                // 파일은 시간순이므로 거꾸로 넣어 최신이 위로 오게 합니다.
                for (int i = rows.Count - 1; i >= 0; i--)
                {
                    var row = rows[i];
                    var item = new ListViewItem(row.Time);
                    item.SubItems.Add(row.Event);
                    item.SubItems.Add(row.Code);
                    item.SubItems.Add(row.Unit);
                    item.SubItems.Add(row.Name);
                    item.SubItems.Add(row.Detail);
                    item.SubItems.Add(row.User);
                    if (row.Event == AlarmHistoryService.EventSet)
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

            int setCount = rows.Count(r => r.Event == AlarmHistoryService.EventSet);
            string info = $"발생 {setCount}건";
            if (service.Folder == null)
            {
                info += " · 이력 파일 기록이 꺼져 있습니다";
            }
            else if (service.IsFileFailing)
            {
                info += " · 이력 파일 쓰기 실패 중";
            }
            labelHistoryInfo.Text = info;
        }

        private void OpenFolder()
        {
            var folder = service.Folder;
            if (string.IsNullOrEmpty(folder))
            {
                MessageBox.Show(this, "알람 이력 파일 기록이 꺼져 있습니다.", "알람 이력", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Directory.CreateDirectory(folder);
                System.Diagnostics.Process.Start("explorer.exe", "\"" + folder + "\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"폴더를 열지 못했습니다.\n{folder}\n{ex.Message}", "알람 이력", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnServiceChanged()
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            try
            {
                BeginInvoke(new Action(() =>
                {
                    if (IsDisposed || !Visible)
                    {
                        return;
                    }
                    RefreshActive();
                    if (pickerDate.Value.Date == DateTime.Today)
                    {
                        RefreshHistory();
                    }
                }));
            }
            catch (InvalidOperationException)
            {
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                Hide();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 사용자가 X로 닫으면 숨기기만 하고 다음에 다시 씁니다.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                return;
            }

            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                service.Changed -= OnServiceChanged;
            }
            base.Dispose(disposing);
        }
    }
}
