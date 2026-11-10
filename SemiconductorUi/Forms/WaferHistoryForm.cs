using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SemiconductorUi.Helpers;
using SemiconductorUi.Services;

namespace SemiconductorUi.Forms
{
    /// <summary>
    /// 웨이퍼 이력 창: 웨이퍼마다 어느 챔버에 언제 들어가 몇 초 공정했고 언제 FOUP B에 들어갔는지 보여 줍니다.
    /// 읽기 전용이고 모달이 아니라서, 떠 있는 동안에도 메인 화면의 긴급 정지를 그대로 누를 수 있습니다.
    /// 진행 중인 웨이퍼는 맨 위에 "진행 중"으로 보이고 1초마다 바뀐 것만 다시 그립니다.
    /// </summary>
    internal sealed class WaferHistoryForm : Form
    {
        private const int MaxRows = 2000;
        private const string AllResults = "전체";

        private static readonly string[] PeriodNames = { "오늘", "최근 7일", "최근 30일" };
        private static readonly int[] PeriodDays = { 1, 7, 30 };
        private static readonly Color RunningBack = Color.FromArgb(232, 241, 255);
        private static readonly Color AbortedBack = Color.FromArgb(255, 236, 236);

        private readonly WaferHistoryService history;
        private readonly ComboBox comboPeriod;
        private readonly ComboBox comboResult;
        private readonly ListView listRows;
        private readonly TextBox textDetail;
        private readonly Label labelCount;
        private readonly Timer refreshTimer;
        private int shownVersion = -1;

        public WaferHistoryForm(WaferHistoryService history)
        {
            this.history = history ?? throw new ArgumentNullException(nameof(history));

            Text = "웨이퍼 이력";
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = UiTheme.Surface;
            Font = new Font("Segoe UI", 9F);
            KeyPreview = true;
            ClientSize = new Size(1260, 660);
            MinimumSize = new Size(900, 480);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12),
                BackColor = UiTheme.Surface
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
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
            comboPeriod.SelectedIndex = 1;
            var labelResult = new Label { AutoSize = false, Size = new Size(40, 30), TextAlign = ContentAlignment.MiddleLeft, Text = "결과" };
            comboResult = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 100, Margin = new Padding(0, 4, 16, 0) };
            comboResult.Items.AddRange(new object[] { AllResults, WaferRunRecord.ResultDone, WaferRunRecord.ResultAborted });
            comboResult.SelectedIndex = 0;
            var buttonRefresh = new Button { Text = "새로고침", Size = new Size(90, 30), Margin = new Padding(0, 1, 6, 0) };
            UiTheme.StyleSecondaryButton(buttonRefresh);
            var buttonFolder = new Button { Text = "폴더 열기", Size = new Size(90, 30), Margin = new Padding(0, 1, 16, 0) };
            UiTheme.StyleSecondaryButton(buttonFolder);
            labelCount = new Label { AutoSize = false, Size = new Size(420, 30), TextAlign = ContentAlignment.MiddleLeft, ForeColor = UiTheme.TextSecondary };
            top.Controls.AddRange(new Control[] { labelPeriod, comboPeriod, labelResult, comboResult, buttonRefresh, buttonFolder, labelCount });
            layout.Controls.Add(top, 0, 0);

            listRows = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                HideSelection = false,
                GridLines = true,
                Margin = new Padding(0, 4, 0, 4)
            };
            listRows.Columns.Add("끝난 시각", 140);
            listRows.Columns.Add("작업", 115);
            listRows.Columns.Add("웨이퍼", 60, HorizontalAlignment.Center);
            listRows.Columns.Add("레시피", 130);
            listRows.Columns.Add("결과", 64, HorizontalAlignment.Center);
            listRows.Columns.Add("경로", 170);
            listRows.Columns.Add("A 공정", 92, HorizontalAlignment.Right);
            listRows.Columns.Add("B 공정", 92, HorizontalAlignment.Right);
            listRows.Columns.Add("C 공정", 92, HorizontalAlignment.Right);
            listRows.Columns.Add("총 시간", 76, HorizontalAlignment.Right);
            listRows.Columns.Add("비고", 200);
            layout.Controls.Add(listRows, 0, 1);

            textDetail = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = UiTheme.SurfaceAlt,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                Margin = new Padding(0, 2, 0, 2),
                Text = "웨이퍼를 고르면 챔버마다 투입·공정·반출 시각이 여기에 나옵니다."
            };
            layout.Controls.Add(textDetail, 0, 2);

            var labelNote = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UiTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "공정 시간은 실제 시간 / 레시피 시간(초)입니다. 알람 홀드로 멈춘 시간도 실제 시간에 들어갑니다. 긴급 정지·공정 리셋 때 진행 중이던 웨이퍼는 \"중단\"으로 남습니다.\n" +
                       "FOUP A에서 꺼내기 전인 웨이퍼는 남기지 않습니다. 파일(Wafer_날짜.csv)은 지우지 않으며 [폴더 열기]로 볼 수 있습니다."
            };
            layout.Controls.Add(labelNote, 0, 3);
            Controls.Add(layout);

            refreshTimer = new Timer { Interval = 1000 };
            refreshTimer.Tick += (s, e) =>
            {
                if (Visible && history.Version != shownVersion)
                {
                    RefreshRows();
                }
            };

            comboPeriod.SelectedIndexChanged += (s, e) => RefreshRows();
            comboResult.SelectedIndexChanged += (s, e) => RefreshRows();
            buttonRefresh.Click += (s, e) => RefreshRows();
            buttonFolder.Click += (s, e) => OpenFolder();
            listRows.SelectedIndexChanged += (s, e) => ShowDetail();
            VisibleChanged += (s, e) =>
            {
                if (Visible)
                {
                    RefreshRows();
                    refreshTimer.Start();
                }
                else
                {
                    refreshTimer.Stop();
                }
            };
        }

        public void RefreshRows()
        {
            shownVersion = history.Version;
            int days = PeriodDays[Math.Max(0, comboPeriod.SelectedIndex)];
            string result = comboResult.SelectedItem as string;
            var today = DateTime.Today;

            var rows = new List<WaferRunRecord>();
            if (result == null || result == AllResults)
            {
                rows.AddRange(history.GetCurrent());
            }
            var saved = history.ReadRange(today.AddDays(-(days - 1)), today, MaxRows);
            if (!string.IsNullOrEmpty(result) && result != AllResults)
            {
                saved = saved.Where(r => r.Result == result).ToList();
            }
            rows.AddRange(saved);

            string selectedKey = SelectedRecord != null ? Key(SelectedRecord) : null;
            listRows.BeginUpdate();
            try
            {
                listRows.Items.Clear();
                foreach (var r in rows)
                {
                    bool running = r.Result == WaferRunRecord.ResultRunning;
                    var item = new ListViewItem(running ? "진행 중" : Time(r.EndTime, true)) { Tag = r };
                    item.SubItems.Add(r.Run);
                    item.SubItems.Add("#" + r.WaferId);
                    item.SubItems.Add(r.Recipe);
                    item.SubItems.Add(r.Result);
                    item.SubItems.Add(r.Route);
                    foreach (var step in r.Steps)
                    {
                        item.SubItems.Add(StepCell(step, r.Result == WaferRunRecord.ResultAborted));
                    }
                    item.SubItems.Add(Duration(r.TotalSeconds));
                    item.SubItems.Add(r.Note);
                    if (running)
                    {
                        item.BackColor = RunningBack;
                    }
                    else if (r.Result == WaferRunRecord.ResultAborted)
                    {
                        item.BackColor = AbortedBack;
                    }
                    listRows.Items.Add(item);
                    if (selectedKey != null && Key(r) == selectedKey)
                    {
                        item.Selected = true;
                    }
                }
            }
            finally
            {
                listRows.EndUpdate();
            }

            int runningCount = rows.Count(r => r.Result == WaferRunRecord.ResultRunning);
            int savedCount = rows.Count - runningCount;
            labelCount.Text = (runningCount > 0 ? $"진행 중 {runningCount}장 · " : "") +
                              (saved.Count >= MaxRows ? $"기록 {savedCount}장 (최근 {MaxRows}장까지)" : $"기록 {savedCount}장");
            ShowDetail();
        }

        private WaferRunRecord SelectedRecord =>
            listRows.SelectedItems.Count > 0 ? listRows.SelectedItems[0].Tag as WaferRunRecord : null;

        private static string Key(WaferRunRecord r)
        {
            return r.Run + "#" + r.WaferId;
        }

        private void ShowDetail()
        {
            var r = SelectedRecord;
            if (r == null)
            {
                textDetail.Text = "웨이퍼를 고르면 챔버마다 투입·공정·반출 시각이 여기에 나옵니다.";
                return;
            }

            var sb = new StringBuilder();
            sb.Append($"작업 {r.Run} · 웨이퍼 #{r.WaferId} · {r.Recipe} · {r.Mode}");
            if (!string.IsNullOrEmpty(r.User))
            {
                sb.Append($" · 시작한 사용자 {r.User}");
            }
            sb.Append($" · 결과 {r.Result}");
            if (!string.IsNullOrEmpty(r.Note))
            {
                sb.Append($" ({r.Note})");
            }
            sb.AppendLine();
            sb.AppendLine($"FOUP A에서 꺼냄 {Time(r.FoupAOut, false)}");
            foreach (var s in r.Steps.Where(x => x.Visited))
            {
                string process = s.Start.HasValue
                    ? $"공정 {Time(s.Start, false)} ~ {(s.End.HasValue ? Time(s.End, false) : (r.Result == WaferRunRecord.ResultAborted ? "끝나지 않음" : "진행 중"))}" +
                      (s.ActualSeconds.HasValue ? $" ({s.ActualSeconds.Value.ToString("0.0", CultureInfo.InvariantCulture)}초, 레시피 {s.SetSeconds}초)" : "")
                    : "공정 시작 전";
                sb.AppendLine($"Chamber {s.Chamber}: 투입 {Time(s.In, false)} · {process} · 반출 {Time(s.Out, false)}");
            }
            if (r.FoupBIn.HasValue)
            {
                sb.AppendLine($"FOUP B에 넣음 {Time(r.FoupBIn, false)} · 총 {Duration(r.TotalSeconds)}");
            }
            textDetail.Text = sb.ToString().TrimEnd();
        }

        private static string StepCell(WaferStepRecord s, bool aborted)
        {
            if (!s.Visited)
            {
                return "";
            }

            if (s.ActualSeconds.HasValue)
            {
                return $"{s.ActualSeconds.Value.ToString("0", CultureInfo.InvariantCulture)} / {s.SetSeconds}초";
            }

            if (aborted)
            {
                return s.Start.HasValue ? "공정 중 중단" : "공정 전 중단";
            }

            return s.Start.HasValue ? "공정 중" : "대기";
        }

        private static string Time(DateTime? t, bool withDate)
        {
            if (!t.HasValue)
            {
                return "-";
            }

            return t.Value.ToString(withDate ? "yyyy-MM-dd HH:mm:ss" : "HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private static string Duration(double? seconds)
        {
            if (!seconds.HasValue)
            {
                return "";
            }

            var span = TimeSpan.FromSeconds(Math.Max(0, Math.Round(seconds.Value)));
            return span.TotalHours >= 1
                ? $"{(int)span.TotalHours}시간 {span.Minutes}분"
                : span.TotalMinutes >= 1 ? $"{span.Minutes}분 {span.Seconds}초" : $"{span.Seconds}초";
        }

        private void OpenFolder()
        {
            try
            {
                System.IO.Directory.CreateDirectory(history.Folder);
                System.Diagnostics.Process.Start("explorer.exe", "\"" + history.Folder + "\"");
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
    }
}
