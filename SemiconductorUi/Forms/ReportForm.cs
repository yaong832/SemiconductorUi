using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SemiconductorUi.Forms
{
	public class ReportForm : Form
	{
		private ComboBox comboLevel;
		private ListView listLog;
		private Button buttonExport;
		private Button buttonClose;

		public Func<List<string>> ProvideLogEntries;
		public Func<string> OnExportCsv;

		public ReportForm()
		{
			InitializeComponent();
			if (IsDesignEnvironment())
			{
				InitializeDesignPreview();
			}
		}

		private void InitializeComponent()
		{
			this.Text = "Report - 로그/내보내기";
			this.StartPosition = FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.BackColor = Color.FromArgb(250, 250, 255);
			this.ClientSize = new Size(900, 580);

			comboLevel = new ComboBox();
			comboLevel.DropDownStyle = ComboBoxStyle.DropDownList;
			comboLevel.Items.AddRange(new object[] { "ALL", "INFO", "WARN", "ALARM", "ERROR", "CRITICAL" });
			comboLevel.SelectedIndex = 0;
			comboLevel.Location = new Point(16, 12);
			comboLevel.BackColor = Color.FromArgb(240, 240, 245);
			comboLevel.ForeColor = Color.FromArgb(40, 40, 40);

			var top = new FlowLayoutPanel();
			top.Location = new Point(16, 12);
			top.Size = new Size(860, 28);
			top.WrapContents = false;
			top.FlowDirection = FlowDirection.LeftToRight;

			listLog = new ListView();
			listLog.Location = new Point(16, 44);
			listLog.Size = new Size(860, 480);
			listLog.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			listLog.View = View.Details;
			listLog.FullRowSelect = true;
			listLog.GridLines = true;
			listLog.ForeColor = Color.FromArgb(40, 40, 40);
			listLog.BackColor = Color.FromArgb(240, 240, 245);
			listLog.Columns.Add("시간", 120);
			listLog.Columns.Add("레벨", 120);
			listLog.Columns.Add("메시지", 600);

			buttonExport = new Button();
			buttonExport.Text = "Export CSV";
			buttonExport.Width = 100;
			buttonExport.Height = 28;
			buttonExport.Location = new Point(672, 536);
			buttonExport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonExport.BackColor = Color.FromArgb(100, 120, 130);
			buttonExport.FlatStyle = FlatStyle.Flat;
			buttonExport.FlatAppearance.BorderSize = 0;
			buttonExport.ForeColor = Color.White;
			buttonExport.Click += buttonExport_Click;

			buttonClose = new Button();
			buttonClose.Text = "Close";
			buttonClose.Width = 80;
			buttonClose.Height = 28;
			buttonClose.Location = new Point(780, 536);
			buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonClose.BackColor = Color.FromArgb(100, 120, 130);
			buttonClose.FlatStyle = FlatStyle.Flat;
			buttonClose.FlatAppearance.BorderSize = 0;
			buttonClose.ForeColor = Color.White;

			top.Controls.Add(comboLevel);

			this.Padding = new Padding(12, 12, 12, 12);
			this.Controls.Add(top);
			this.Controls.Add(listLog);
			this.Controls.Add(buttonExport);
			this.Controls.Add(buttonClose);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (IsDesignEnvironment()) return;
			// 디자이너 경고 방지: 이벤트 연결은 InitializeComponent 바깥에서 수행
			comboLevel.SelectedIndexChanged -= comboLevel_SelectedIndexChanged;
			comboLevel.SelectedIndexChanged += comboLevel_SelectedIndexChanged;
			buttonClose.Click -= buttonClose_Click;
			buttonClose.Click += buttonClose_Click;
			// 폼 로드 시 로그 데이터 표시
			RefreshLog();
		}

		private void comboLevel_SelectedIndexChanged(object sender, EventArgs e)
		{
			RefreshLog();
		}

		private void buttonExport_Click(object sender, EventArgs e)
		{
			var path = OnExportCsv?.Invoke();
			if (!string.IsNullOrEmpty(path))
			{
				// 여러 파일이 저장된 경우와 단일 파일인 경우를 구분하여 표시
				if (path.Contains("\n(") && path.Contains("개 파일"))
				{
					MessageBox.Show(this, $"CSV 저장 완료:\n{path}", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show(this, $"CSV 저장 완료:\n{path}", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private bool IsDesignEnvironment()
		{
			try
			{
				if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return true;
				if (this.Site != null && this.Site.DesignMode) return true;
			}
			catch { }
			return false;
		}

		private void InitializeDesignPreview()
		{
			this.Text = "Report (Design Preview)";
			this.StartPosition = FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.BackColor = Color.FromArgb(34, 36, 43);
			this.ClientSize = new Size(900, 580);

			var label = new Label();
			label.Text = "Report 폼 (디자인 미리보기)";
			label.ForeColor = Color.FromArgb(40, 40, 40);
			label.Location = new Point(16, 12);
			label.AutoSize = true;

			var preview = new ListView();
			preview.Location = new Point(16, 44);
			preview.Size = new Size(860, 480);
			preview.View = View.Details;
			preview.GridLines = true;
			preview.ForeColor = Color.FromArgb(40, 40, 40);
			preview.BackColor = Color.FromArgb(240, 240, 245);
			preview.Columns.Add("시간", 120);
			preview.Columns.Add("레벨", 120);
			preview.Columns.Add("메시지", 600);
			preview.Items.Add(new ListViewItem(new[] { "12:00:00", "INFO", "디자인 미리보기 샘플" }));

			this.Controls.Add(label);
			this.Controls.Add(preview);
		}

		public void RefreshLog()
		{
			var entries = ProvideLogEntries?.Invoke() ?? new List<string>();
			var level = comboLevel.SelectedItem?.ToString() ?? "ALL";

			listLog.BeginUpdate();
			listLog.Items.Clear();
			foreach (var e in entries)
			{
				// [HH:mm:ss] [LEVEL] message
				var time = e.Length >= 10 ? e.Substring(1, 8) : "";
				
				// 첫 번째 ] 이후의 [ 위치를 찾아서 LEVEL 추출
				var firstBracketEnd = e.IndexOf(']', 0);
				if (firstBracketEnd < 0) continue;
				
				var levelBracketStart = e.IndexOf('[', firstBracketEnd + 1);
				if (levelBracketStart < 0) continue;
				
				var levelBracketEnd = e.IndexOf(']', levelBracketStart + 1);
				if (levelBracketEnd < 0) continue;
				
				// [와 ] 사이의 텍스트가 LEVEL
				var lvl = e.Substring(levelBracketStart + 1, levelBracketEnd - levelBracketStart - 1);
				var msgStart = levelBracketEnd + 2;
				var msg = msgStart < e.Length ? e.Substring(msgStart).TrimStart() : "";

				if (level != "ALL" && !string.Equals(level, lvl, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				var item = new ListViewItem(time);
				item.SubItems.Add(lvl);
				item.SubItems.Add(msg);
				listLog.Items.Add(item);
			}
			listLog.EndUpdate();
		}
	}
}

