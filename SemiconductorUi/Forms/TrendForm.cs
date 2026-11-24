using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Forms
{
	/// <summary>
	/// 추이(Trend) 창: Chamber A/B/C의 온도·압력 PV/SV를 시간 그래프로 보여준다.
	/// 값은 메인 화면 PM 상태 카드에 표시되는 값과 같다.
	/// (PV는 그 챔버가 공정 중일 때만, SV는 레시피가 적용된 경우에만 있음 → 없으면 선이 끊긴다)
	/// 데이터는 Form1이 공정 중 1초마다 기록하고, 이 창은 1초마다 다시 그린다.
	/// </summary>
	public class TrendForm : Form
	{
		private static readonly string[] Units = { "PMA", "PMB", "PMC" };

		private static readonly Color PvColor = Color.FromArgb(45, 98, 196);
		private static readonly Color SvColor = Color.FromArgb(214, 138, 20);

		private ComboBox comboRange;
		private Label labelStatus;
		private Chart chart;
		private Timer refreshTimer;

		/// <summary>
		/// 기록된 샘플을 돌려준다. PV/SV가 없는 구간은 double.NaN.
		/// </summary>
		public Func<List<(DateTime t, string unit, double pvTemp, double svTemp, double pvPress, double svPress)>> ProvideSamples;

		public TrendForm()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Text = "추이 - 챔버 온도·압력";
			this.StartPosition = FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.Sizable;
			this.MaximizeBox = true;
			this.MinimizeBox = false;
			this.ShowInTaskbar = false;
			this.BackColor = UiTheme.Surface;
			this.Font = new Font("Segoe UI", 10F);
			this.ClientSize = new Size(1280, 820);
			this.MinimumSize = new Size(900, 600);

			var topBar = new FlowLayoutPanel
			{
				Dock = DockStyle.Top,
				Height = 44,
				Padding = new Padding(12, 8, 12, 0),
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false,
				BackColor = UiTheme.Surface
			};

			var labelRange = new Label
			{
				Text = "표시 구간",
				AutoSize = true,
				ForeColor = UiTheme.TextPrimary,
				Margin = new Padding(0, 6, 6, 0)
			};

			comboRange = new ComboBox
			{
				DropDownStyle = ComboBoxStyle.DropDownList,
				Width = 110,
				Margin = new Padding(0, 2, 16, 0)
			};
			comboRange.Items.AddRange(new object[] { "최근 5분", "최근 10분", "최근 30분", "최근 1시간", "전체" });
			comboRange.SelectedIndex = 1;
			comboRange.SelectedIndexChanged += (s, e) => RefreshChart();

			labelStatus = new Label
			{
				AutoSize = true,
				ForeColor = UiTheme.TextSecondary,
				Margin = new Padding(0, 6, 0, 0),
				Text = ""
			};

			topBar.Controls.Add(labelRange);
			topBar.Controls.Add(comboRange);
			topBar.Controls.Add(labelStatus);

			chart = new Chart
			{
				Dock = DockStyle.Fill,
				BackColor = UiTheme.Surface
			};

			// 3행(Chamber A/B/C) × 2열(온도, 압력)
			for (int row = 0; row < Units.Length; row++)
			{
				string unit = Units[row];
				AddArea(unit, "T", row, 0, "0.0");
				AddArea(unit, "P", row, 1, "G4");
			}

			var legend = new Legend("legend")
			{
				Docking = Docking.Top,
				Alignment = StringAlignment.Center,
				BackColor = UiTheme.Surface,
				ForeColor = UiTheme.TextPrimary,
				LegendStyle = LegendStyle.Row
			};
			// 그래프 칸을 직접 배치하므로 범례도 맨 위 5% 자리에 직접 둔다
			legend.Position = new ElementPosition(0f, 0f, 100f, 5f);
			legend.CustomItems.Add(new LegendItem("PV (현재값)", PvColor, ""));
			legend.CustomItems.Add(new LegendItem("SV (레시피 설정값)", SvColor, ""));
			chart.Legends.Add(legend);

			refreshTimer = new Timer { Interval = 1000 };
			refreshTimer.Tick += refreshTimer_Tick;

			this.Controls.Add(chart);
			this.Controls.Add(topBar);

			this.FormClosing += TrendForm_FormClosing;
		}

		private static string AreaName(string unit, string kind) => unit + "_" + kind;

		private void AddArea(string unit, string kind, int row, int col, string labelFormat)
		{
			string name = AreaName(unit, kind);
			var area = new ChartArea(name)
			{
				BackColor = UiTheme.SurfaceAlt
			};
			// 위쪽 범례(약 5%)를 뺀 나머지를 3×2로 나눈다
			const float top = 5f;
			float h = (100f - top) / Units.Length;
			area.Position = new ElementPosition(col * 50f, top + row * h, 50f, h);
			area.AxisX.LabelStyle.Format = "HH:mm:ss";
			area.AxisX.LabelStyle.ForeColor = UiTheme.TextSecondary;
			area.AxisY.LabelStyle.ForeColor = UiTheme.TextSecondary;
			area.AxisY.LabelStyle.Format = labelFormat;
			area.AxisX.MajorGrid.LineColor = UiTheme.Border;
			area.AxisY.MajorGrid.LineColor = UiTheme.Border;
			area.AxisX.LineColor = UiTheme.Border;
			area.AxisY.LineColor = UiTheme.Border;
			area.AxisY.IsStartedFromZero = false;
			chart.ChartAreas.Add(area);

			var title = new Title(name)
			{
				Name = name,
				DockedToChartArea = name,
				// 그래프 칸 위치를 직접 정했으므로 제목은 칸 안쪽 위에 둔다 (칸 밖이면 위 칸과 겹칠 수 있음)
				IsDockedInsideChartArea = true,
				Docking = Docking.Top,
				Alignment = ContentAlignment.MiddleLeft,
				ForeColor = UiTheme.TextPrimary,
				Font = new Font("Segoe UI", 10F, FontStyle.Bold),
				Text = TitleText(unit, kind, double.NaN, double.NaN)
			};
			chart.Titles.Add(title);

			chart.Series.Add(CreateSeries(name + "_PV", name, PvColor, ChartDashStyle.Solid));
			chart.Series.Add(CreateSeries(name + "_SV", name, SvColor, ChartDashStyle.Dash));
		}

		private static Series CreateSeries(string name, string area, Color color, ChartDashStyle dash)
		{
			var s = new Series(name)
			{
				ChartArea = area,
				ChartType = SeriesChartType.Line,
				Color = color,
				BorderWidth = 2,
				BorderDashStyle = dash,
				XValueType = ChartValueType.DateTime,
				IsVisibleInLegend = false
			};
			// 값이 없는 구간(공정 중 아님 / 레시피 미적용)은 선을 끊는다
			s.EmptyPointStyle.Color = Color.Transparent;
			s.EmptyPointStyle.BorderWidth = 0;
			return s;
		}

		private static string TitleText(string unit, string kind, double pv, double sv)
		{
			string label = EquipmentRegionHelper.FormatUnitLabel(unit);
			bool isTemp = kind == "T";
			string what = isTemp ? "온도 (°C)" : "압력 (Torr)";
			string pvText = double.IsNaN(pv) ? "-" : (isTemp ? pv.ToString("0.0") : FormatPressure(pv));
			string svText = double.IsNaN(sv) ? "-" : (isTemp ? sv.ToString("0.0") : FormatPressure(sv));
			return $"{label} {what}   PV {pvText} / SV {svText}";
		}

		private static string FormatPressure(double v)
		{
			return v < 1.0 ? v.ToString("0.####") : v.ToString("0.##");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (IsDesignEnvironment()) return;
			// 창을 열면 바로 그리고, 닫을 때까지 1초마다 갱신
			RefreshChart();
			refreshTimer.Start();
		}

		private void TrendForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if (refreshTimer != null)
				{
					refreshTimer.Stop();
					refreshTimer.Tick -= refreshTimer_Tick;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"TrendForm FormClosing 오류: {ex.Message}");
			}
		}

		private void refreshTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				if (this.IsDisposed || this.Disposing)
				{
					refreshTimer?.Stop();
					return;
				}
				RefreshChart();
			}
			catch (ObjectDisposedException)
			{
				refreshTimer?.Stop();
			}
			catch (Exception ex)
			{
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

		private TimeSpan? SelectedRange()
		{
			switch (comboRange?.SelectedIndex ?? 1)
			{
				case 0: return TimeSpan.FromMinutes(5);
				case 1: return TimeSpan.FromMinutes(10);
				case 2: return TimeSpan.FromMinutes(30);
				case 3: return TimeSpan.FromHours(1);
				default: return null;
			}
		}

		/// <summary>
		/// 기록된 샘플로 6개 그래프를 다시 그린다.
		/// </summary>
		public void RefreshChart()
		{
			if (chart == null || chart.IsDisposed)
			{
				return;
			}

			var samples = ProvideSamples?.Invoke() ?? new List<(DateTime, string, double, double, double, double)>();
			DateTime now = DateTime.Now;
			TimeSpan? range = SelectedRange();
			DateTime from = range.HasValue ? now - range.Value : (samples.Count > 0 ? samples[0].t : now.AddMinutes(-10));

			foreach (var s in chart.Series)
			{
				s.Points.Clear();
			}

			var lastPvT = new Dictionary<string, double>();
			var lastSvT = new Dictionary<string, double>();
			var lastPvP = new Dictionary<string, double>();
			var lastSvP = new Dictionary<string, double>();

			foreach (var s in samples)
			{
				if (s.t < from)
				{
					continue;
				}
				if (Array.IndexOf(Units, s.unit) < 0)
				{
					continue;
				}
				double x = s.t.ToOADate();
				AddPoint(AreaName(s.unit, "T") + "_PV", x, s.pvTemp);
				AddPoint(AreaName(s.unit, "T") + "_SV", x, s.svTemp);
				AddPoint(AreaName(s.unit, "P") + "_PV", x, s.pvPress);
				AddPoint(AreaName(s.unit, "P") + "_SV", x, s.svPress);
				lastPvT[s.unit] = s.pvTemp;
				lastSvT[s.unit] = s.svTemp;
				lastPvP[s.unit] = s.pvPress;
				lastSvP[s.unit] = s.svPress;
			}

			double minX = from.ToOADate();
			double maxX = now.ToOADate();
			foreach (var unit in Units)
			{
				foreach (var kind in new[] { "T", "P" })
				{
					string name = AreaName(unit, kind);
					var area = chart.ChartAreas[name];
					area.AxisX.Minimum = minX;
					area.AxisX.Maximum = maxX;
					area.RecalculateAxesScale();

					var lastPv = kind == "T" ? lastPvT : lastPvP;
					var lastSv = kind == "T" ? lastSvT : lastSvP;
					chart.Titles[name].Text = TitleText(unit, kind,
						lastPv.TryGetValue(unit, out var pv) ? pv : double.NaN,
						lastSv.TryGetValue(unit, out var sv) ? sv : double.NaN);
				}
			}

			if (samples.Count == 0)
			{
				labelStatus.Text = "아직 기록이 없습니다. 공정을 시작하면 1초마다 기록됩니다.";
			}
			else
			{
				labelStatus.Text = $"기록 {samples[0].t:HH:mm:ss} ~ {samples[samples.Count - 1].t:HH:mm:ss} (1초 간격) · PV는 공정 중인 챔버만, SV는 레시피 적용 시에만 표시";
			}
		}

		private void AddPoint(string seriesName, double x, double y)
		{
			var series = chart.Series[seriesName];
			if (double.IsNaN(y) || double.IsInfinity(y))
			{
				int i = series.Points.AddXY(x, 0);
				series.Points[i].IsEmpty = true;
			}
			else
			{
				series.Points.AddXY(x, y);
			}
		}
	}
}
