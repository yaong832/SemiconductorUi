using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SemiconductorUi.Forms
{
	public class TrendForm : Form
	{
		private ListView listTrend;
		private Button buttonStart;
		private Button buttonStop;
		private Button buttonRefresh;
		private ComboBox comboUnit;
		private Chart chartTemp;
		private Chart chartPress;
		private Timer refreshTimer;

		public Action OnStartCapture;
		public Action OnStopCapture;
		public Func<List<(DateTime t, string unit, double pvTemp, double svTemp, double pvPress, double svPress)>> ProvideSamples;

		public TrendForm()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Text = "Trend - 환경 트렌드";
			this.StartPosition = FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.BackColor = Color.FromArgb(250, 250, 255);
			this.ClientSize = new Size(1100, 720);

			comboUnit = new ComboBox();
			comboUnit.DropDownStyle = ComboBoxStyle.DropDownList;
			comboUnit.Items.AddRange(new object[] { "ALL", "PMA", "PMB", "PMC" });
			comboUnit.SelectedIndex = 0;
			comboUnit.Location = new Point(16, 12);
			comboUnit.Width = 120;
			comboUnit.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			comboUnit.BackColor = Color.FromArgb(240, 240, 245);
			comboUnit.ForeColor = Color.FromArgb(40, 40, 40);

			listTrend = new ListView();
			listTrend.Location = new Point(16, 44);
			listTrend.Width = 1060;
			listTrend.Height = 240;
			listTrend.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			listTrend.View = View.Details;
			listTrend.FullRowSelect = true;
			listTrend.GridLines = true;
			listTrend.ForeColor = Color.FromArgb(40, 40, 40);
			listTrend.BackColor = Color.FromArgb(240, 240, 245);
			listTrend.Columns.Add("시간", 140);
			listTrend.Columns.Add("유닛", 80);
			listTrend.Columns.Add("PV 온도(°C)", 140);
			listTrend.Columns.Add("SV 온도(°C)", 140);
			listTrend.Columns.Add("PV 압력(Torr)", 180);
			listTrend.Columns.Add("SV 압력(Torr)", 180);

			chartTemp = new Chart();
			chartTemp.Location = new Point(16, 296);
			chartTemp.Size = new Size(1060, 160);
			chartTemp.BackColor = Color.FromArgb(240, 240, 245);
			var areaT = new ChartArea("areaT");
			areaT.BackColor = Color.FromArgb(240, 240, 245);
			areaT.AxisX.MajorGrid.LineColor = Color.FromArgb(200, 200, 210);
			areaT.AxisY.MajorGrid.LineColor = Color.FromArgb(200, 200, 210);
			areaT.AxisX.LabelStyle.ForeColor = Color.FromArgb(40, 40, 40);
			areaT.AxisY.LabelStyle.ForeColor = Color.FromArgb(40, 40, 40);
			chartTemp.ChartAreas.Add(areaT);
			var legendT = new Legend();
			legendT.BackColor = Color.FromArgb(240, 240, 245);
			legendT.ForeColor = Color.FromArgb(40, 40, 40);
			chartTemp.Legends.Add(legendT);
			var sPvT = new Series("PV Temp");
			sPvT.ChartType = SeriesChartType.Line;
			sPvT.Color = Color.DeepSkyBlue;
			sPvT.BorderWidth = 2;
			sPvT.XValueType = ChartValueType.DateTime;
			sPvT.ChartArea = "areaT";
			var sSvT = new Series("SV Temp");
			sSvT.ChartType = SeriesChartType.Line;
			sSvT.Color = Color.LightCoral;
			sSvT.BorderWidth = 2;
			sSvT.XValueType = ChartValueType.DateTime;
			sSvT.ChartArea = "areaT";
			chartTemp.Series.Add(sPvT);
			chartTemp.Series.Add(sSvT);
			var titleT = new Title("Temperature (°C)");
			titleT.ForeColor = Color.FromArgb(40, 40, 40);
			chartTemp.Titles.Add(titleT);
			chartTemp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

			chartPress = new Chart();
			chartPress.Location = new Point(16, 480);
			chartPress.Size = new Size(1060, 160);
			chartPress.BackColor = Color.FromArgb(240, 240, 245);
			var areaP = new ChartArea("areaP");
			areaP.BackColor = Color.FromArgb(240, 240, 245);
			areaP.AxisX.MajorGrid.LineColor = Color.FromArgb(200, 200, 210);
			areaP.AxisY.MajorGrid.LineColor = Color.FromArgb(200, 200, 210);
			areaP.AxisX.LabelStyle.ForeColor = Color.FromArgb(40, 40, 40);
			areaP.AxisY.LabelStyle.ForeColor = Color.FromArgb(40, 40, 40);
			chartPress.ChartAreas.Add(areaP);
			var legendP = new Legend();
			legendP.BackColor = Color.FromArgb(240, 240, 245);
			legendP.ForeColor = Color.FromArgb(40, 40, 40);
			chartPress.Legends.Add(legendP);
			var sPvP = new Series("PV Press");
			sPvP.ChartType = SeriesChartType.Line;
			sPvP.Color = Color.DeepSkyBlue;
			sPvP.BorderWidth = 2;
			sPvP.XValueType = ChartValueType.DateTime;
			sPvP.ChartArea = "areaP";
			var sSvP = new Series("SV Press");
			sSvP.ChartType = SeriesChartType.Line;
			sSvP.Color = Color.LightCoral;
			sSvP.BorderWidth = 2;
			sSvP.XValueType = ChartValueType.DateTime;
			sSvP.ChartArea = "areaP";
			chartPress.Series.Add(sPvP);
			chartPress.Series.Add(sSvP);
			var titleP = new Title("Pressure (Torr)");
			titleP.ForeColor = Color.FromArgb(40, 40, 40);
			chartPress.Titles.Add(titleP);
			chartPress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

			buttonStart = new Button();
			buttonStart.Text = "Start";
			buttonStart.Width = 80;
			buttonStart.Height = 28;
			buttonStart.Location = new Point(160, 12);
			buttonStart.BackColor = Color.FromArgb(100, 120, 130);
			buttonStart.FlatStyle = FlatStyle.Flat;
			buttonStart.FlatAppearance.BorderSize = 0;
			buttonStart.ForeColor = Color.White;
			buttonStart.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			buttonStart.Click += buttonStart_Click;

			buttonStop = new Button();
			buttonStop.Text = "Stop";
			buttonStop.Width = 80;
			buttonStop.Height = 28;
			buttonStop.Location = new Point(246, 12);
			buttonStop.BackColor = Color.FromArgb(100, 120, 130);
			buttonStop.FlatStyle = FlatStyle.Flat;
			buttonStop.FlatAppearance.BorderSize = 0;
			buttonStop.ForeColor = Color.White;
			buttonStop.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			buttonStop.Click += buttonStop_Click;

			buttonRefresh = new Button();
			buttonRefresh.Text = "Refresh";
			buttonRefresh.Width = 90;
			buttonRefresh.Height = 28;
			buttonRefresh.Location = new Point(332, 12);
			buttonRefresh.BackColor = Color.FromArgb(100, 120, 130);
			buttonRefresh.FlatStyle = FlatStyle.Flat;
			buttonRefresh.FlatAppearance.BorderSize = 0;
			buttonRefresh.ForeColor = Color.White;
			buttonRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			buttonRefresh.Click += buttonRefresh_Click;

			refreshTimer = new Timer();
			refreshTimer.Interval = 1000;
			refreshTimer.Tick += refreshTimer_Tick;

			this.Controls.Add(comboUnit);
			this.Controls.Add(listTrend);
			this.Controls.Add(chartTemp);
			this.Controls.Add(chartPress);
			this.Controls.Add(buttonStart);
			this.Controls.Add(buttonStop);
			this.Controls.Add(buttonRefresh);

			this.FormClosing += TrendForm_FormClosing;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (IsDesignEnvironment()) return;
		}

		private void TrendForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				// 타이머 정지 및 정리
				if (refreshTimer != null)
				{
					refreshTimer.Stop();
					refreshTimer.Tick -= refreshTimer_Tick;
				}

				// 캡처 중지
				OnStopCapture?.Invoke();
			}
			catch (Exception ex)
			{
				// 오류가 발생해도 폼 종료는 계속 진행
				System.Diagnostics.Debug.WriteLine($"TrendForm FormClosing 오류: {ex.Message}");
			}
		}

		private void buttonStart_Click(object sender, EventArgs e)
		{
			OnStartCapture?.Invoke();
			StartAutoRefresh();
		}

		private void buttonStop_Click(object sender, EventArgs e)
		{
			OnStopCapture?.Invoke();
			StopAutoRefresh();
		}

		private void buttonRefresh_Click(object sender, EventArgs e)
		{
			RefreshSamples();
			PlotSamples();
		}

		private void refreshTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				// 폼이 닫히거나 disposed된 경우 처리 중단
				if (this.IsDisposed || this.Disposing)
				{
					refreshTimer?.Stop();
					return;
				}

				RefreshSamples();
				PlotSamples();
			}
			catch (ObjectDisposedException)
			{
				// 컨트롤이 이미 disposed된 경우 타이머 정지
				refreshTimer?.Stop();
			}
			catch (Exception ex)
			{
				// 기타 오류 발생 시에도 타이머는 계속 실행
				System.Diagnostics.Debug.WriteLine($"refreshTimer_Tick 오류: {ex.Message}");
			}
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

		public void RefreshSamples()
		{
			try
			{
				if (listTrend == null || listTrend.IsDisposed)
				{
					return;
				}

				var samples = ProvideSamples?.Invoke() ?? new List<(DateTime, string, double, double, double, double)>();
				listTrend.BeginUpdate();
				listTrend.Items.Clear();
				// 최근 200개만 표시
				int start = Math.Max(0, samples.Count - 200);
				for (int i = start; i < samples.Count; i++)
				{
					var s = samples[i];
					var item = new ListViewItem(s.t.ToString("HH:mm:ss"));
					item.SubItems.Add(s.unit);
					item.SubItems.Add(s.pvTemp.ToString("0.0"));
					item.SubItems.Add(s.svTemp.ToString("0.0"));
					item.SubItems.Add(s.pvPress.ToString("0.###"));
					item.SubItems.Add(s.svPress.ToString("0.###"));
					listTrend.Items.Add(item);
				}
				listTrend.EndUpdate();
			}
			catch (ObjectDisposedException)
			{
				// 컨트롤이 이미 disposed된 경우 무시
			}
		}

		private void PlotSamples()
		{
			try
			{
				if (chartTemp == null || chartTemp.IsDisposed || chartPress == null || chartPress.IsDisposed)
				{
					return;
				}

				var samples = ProvideSamples?.Invoke() ?? new List<(DateTime, string, double, double, double, double)>();
				string unitFilter = comboUnit?.SelectedItem?.ToString() ?? "ALL";

				if (chartTemp.Series.Count < 2 || chartPress.Series.Count < 2)
				{
					return;
				}

				var sPvT = chartTemp.Series[0];
				var sSvT = chartTemp.Series[1];
				var sPvP = chartPress.Series[0];
				var sSvP = chartPress.Series[1];
				sPvT.Points.Clear();
				sSvT.Points.Clear();
				sPvP.Points.Clear();
				sSvP.Points.Clear();

				int start = Math.Max(0, samples.Count - 400);
				for (int i = start; i < samples.Count; i++)
				{
					var s = samples[i];
					if (unitFilter != "ALL" && !string.Equals(unitFilter, s.unit, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					double x = s.t.ToOADate();
					sPvT.Points.AddXY(x, s.pvTemp);
					sSvT.Points.AddXY(x, s.svTemp);
					sPvP.Points.AddXY(x, s.pvPress);
					sSvP.Points.AddXY(x, s.svPress);
				}

				if (chartTemp.ChartAreas.Count > 0)
				{
					chartTemp.ChartAreas[0].RecalculateAxesScale();
				}
				if (chartPress.ChartAreas.Count > 0)
				{
					chartPress.ChartAreas[0].RecalculateAxesScale();
				}
			}
			catch (ObjectDisposedException)
			{
				// 컨트롤이 이미 disposed된 경우 무시
			}
		}

		private void StartAutoRefresh()
		{
			refreshTimer.Start();
		}

		private void StopAutoRefresh()
		{
			refreshTimer.Stop();
		}
	}
}

