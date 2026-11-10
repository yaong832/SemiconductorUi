using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.Helpers;
using SemiconductorUi.Services;

namespace SemiconductorUi.Forms
{
    /// <summary>
    /// 변경 이력 창: 누가 언제 티칭 위치, 서보 속도, 레시피, 알람 임계값, 사용자를 바꿨는지 보여 줍니다.
    /// 읽기 전용이고 모달이 아니라서, 떠 있는 동안에도 메인 화면의 긴급 정지를 그대로 누를 수 있습니다.
    /// </summary>
    internal sealed class ChangeHistoryForm : Form
    {
        private const int MaxRows = 2000;
        private const string AllCategories = "전체";

        private static readonly string[] PeriodNames = { "오늘", "최근 7일", "최근 30일", "최근 1년" };
        private static readonly int[] PeriodDays = { 1, 7, 30, 365 };

        private readonly ComboBox comboPeriod;
        private readonly ComboBox comboCategory;
        private readonly ListView listRows;
        private readonly Label labelCount;

        public ChangeHistoryForm()
        {
            Text = "변경 이력";
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = UiTheme.Surface;
            Font = new Font("Segoe UI", 9F);
            KeyPreview = true;
            ClientSize = new Size(1120, 620);
            MinimumSize = new Size(820, 420);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12),
                BackColor = UiTheme.Surface
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            var top = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0)
            };
            var labelPeriod = new Label { AutoSize = false, Size = new Size(40, 30), TextAlign = ContentAlignment.MiddleLeft, Text = "기간" };
            comboPeriod = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 110, Margin = new Padding(0, 4, 16, 0) };
            comboPeriod.Items.AddRange(PeriodNames.Cast<object>().ToArray());
            comboPeriod.SelectedIndex = 2;
            var labelCategory = new Label { AutoSize = false, Size = new Size(40, 30), TextAlign = ContentAlignment.MiddleLeft, Text = "구분" };
            comboCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 130, Margin = new Padding(0, 4, 16, 0) };
            comboCategory.Items.Add(AllCategories);
            comboCategory.Items.AddRange(ChangeHistory.Categories.Cast<object>().ToArray());
            comboCategory.SelectedIndex = 0;
            var buttonRefresh = new Button { Text = "새로고침", Size = new Size(90, 30), Margin = new Padding(0, 1, 6, 0) };
            UiTheme.StyleSecondaryButton(buttonRefresh);
            var buttonFolder = new Button { Text = "폴더 열기", Size = new Size(90, 30), Margin = new Padding(0, 1, 16, 0) };
            UiTheme.StyleSecondaryButton(buttonFolder);
            labelCount = new Label { AutoSize = false, Size = new Size(320, 30), TextAlign = ContentAlignment.MiddleLeft, ForeColor = UiTheme.TextSecondary };
            top.Controls.AddRange(new Control[] { labelPeriod, comboPeriod, labelCategory, comboCategory, buttonRefresh, buttonFolder, labelCount });
            layout.Controls.Add(top, 0, 0);

            listRows = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true,
                Margin = new Padding(0, 4, 0, 4)
            };
            listRows.Columns.Add("시각", 140);
            listRows.Columns.Add("사용자", 110);
            listRows.Columns.Add("구분", 90);
            listRows.Columns.Add("대상", 240);
            listRows.Columns.Add("항목", 200);
            listRows.Columns.Add("이전 값", 140, HorizontalAlignment.Right);
            listRows.Columns.Add("바뀐 값", 140, HorizontalAlignment.Right);
            layout.Controls.Add(listRows, 0, 1);

            var labelNote = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UiTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "티칭 위치·서보 속도·레시피·알람 임계값·사용자를 저장할 때 바뀐 값만 남깁니다. 비밀번호 값은 남기지 않습니다.\n" +
                       "파일은 지우지 않습니다 (문서\\SemiconductorUi_Logs\\Changes\\Change_날짜.csv)."
            };
            layout.Controls.Add(labelNote, 0, 2);
            Controls.Add(layout);

            comboPeriod.SelectedIndexChanged += (s, e) => RefreshRows();
            comboCategory.SelectedIndexChanged += (s, e) => RefreshRows();
            buttonRefresh.Click += (s, e) => RefreshRows();
            buttonFolder.Click += (s, e) => OpenFolder();
            VisibleChanged += (s, e) =>
            {
                if (Visible)
                {
                    RefreshRows();
                }
            };
        }

        public void RefreshRows()
        {
            int days = PeriodDays[Math.Max(0, comboPeriod.SelectedIndex)];
            string category = comboCategory.SelectedItem as string;
            var today = DateTime.Today;
            var rows = ChangeHistory.ReadRange(today.AddDays(-(days - 1)), today, MaxRows);
            if (!string.IsNullOrEmpty(category) && category != AllCategories)
            {
                rows = rows.Where(r => r.Category == category).ToList();
            }

            listRows.BeginUpdate();
            try
            {
                listRows.Items.Clear();
                foreach (var r in rows)
                {
                    var item = new ListViewItem(r.Time.ToString("yyyy-MM-dd HH:mm:ss"));
                    item.SubItems.Add(string.IsNullOrEmpty(r.Role) ? r.User : $"{r.User} ({r.Role})");
                    item.SubItems.Add(r.Category);
                    item.SubItems.Add(r.Target);
                    item.SubItems.Add(r.Item);
                    item.SubItems.Add(r.Before);
                    item.SubItems.Add(r.After);
                    listRows.Items.Add(item);
                }
            }
            finally
            {
                listRows.EndUpdate();
            }

            labelCount.Text = rows.Count >= MaxRows ? $"{rows.Count}건 (최근 {MaxRows}건까지)" : $"{rows.Count}건";
        }

        private void OpenFolder()
        {
            try
            {
                System.IO.Directory.CreateDirectory(ChangeHistory.Folder);
                System.Diagnostics.Process.Start("explorer.exe", "\"" + ChangeHistory.Folder + "\"");
            }
            catch (Exception ex)
            {
                labelCount.Text = $"폴더를 열 수 없습니다: {ex.Message}";
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
            // X를 눌러도 숨기기만 합니다. 프로그램이 끝날 때는 그대로 닫힙니다.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                return;
            }

            base.OnFormClosing(e);
        }
    }
}
