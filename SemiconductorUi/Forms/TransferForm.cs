using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SemiconductorUi.Forms
{
	public class TransferForm : Form
	{
		private ListView listQueue;
		private Button buttonAutoPlan;
		private Button buttonClear;
		private Button buttonClose;
		public Action OnAutoPlan;
		public Action OnClear;
		public Func<List<string>> ProvideQueueLines;

		public TransferForm()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Text = "Transfer - 이송 플래너/큐";
			this.StartPosition = FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.BackColor = Color.FromArgb(250, 250, 255);
			this.ClientSize = new Size(900, 540);

			listQueue = new ListView();
			listQueue.Dock = DockStyle.Fill;
			listQueue.View = View.Details;
			listQueue.FullRowSelect = true;
			listQueue.GridLines = true;
			listQueue.ForeColor = Color.FromArgb(40, 40, 40);
			listQueue.BackColor = Color.FromArgb(240, 240, 245);
			listQueue.Columns.Add("대기 중 이송 작업 (출발 → 도착 · 웨이퍼 ID)", 840);

			var bottom = new FlowLayoutPanel();
			bottom.Dock = DockStyle.Bottom;
			bottom.Height = 52;
			((FlowLayoutPanel)bottom).FlowDirection = FlowDirection.RightToLeft;
			((FlowLayoutPanel)bottom).WrapContents = false;

			buttonAutoPlan = new Button();
			buttonAutoPlan.Text = "Auto Plan";
			buttonAutoPlan.Width = 100;
			buttonAutoPlan.Height = 28;
			buttonAutoPlan.Margin = new Padding(8, 12, 0, 12);
			buttonAutoPlan.BackColor = Color.FromArgb(100, 120, 130);
			buttonAutoPlan.FlatStyle = FlatStyle.Flat;
			buttonAutoPlan.FlatAppearance.BorderSize = 0;
			buttonAutoPlan.ForeColor = Color.White;

			buttonClear = new Button();
			buttonClear.Text = "Clear";
			buttonClear.Width = 80;
			buttonClear.Height = 28;
			buttonClear.Margin = new Padding(8, 12, 0, 12);
			buttonClear.BackColor = Color.FromArgb(100, 120, 130);
			buttonClear.FlatStyle = FlatStyle.Flat;
			buttonClear.FlatAppearance.BorderSize = 0;
			buttonClear.ForeColor = Color.White;

			buttonClose = new Button();
			buttonClose.Text = "Close";
			buttonClose.Width = 80;
			buttonClose.Height = 28;
			buttonClose.Margin = new Padding(8, 12, 16, 12);
			buttonClose.BackColor = Color.FromArgb(100, 120, 130);
			buttonClose.FlatStyle = FlatStyle.Flat;
			buttonClose.FlatAppearance.BorderSize = 0;
			buttonClose.ForeColor = Color.White;

			bottom.Controls.Add(buttonClose);
			bottom.Controls.Add(buttonClear);
			bottom.Controls.Add(buttonAutoPlan);

			this.Padding = new Padding(12, 12, 12, 12);
			this.Controls.Add(listQueue);
			this.Controls.Add(bottom);
		}

		public void RefreshQueue()
		{
			var lines = ProvideQueueLines?.Invoke() ?? new List<string>();
			listQueue.BeginUpdate();
			listQueue.Items.Clear();
			foreach (var line in lines)
			{
				listQueue.Items.Add(new ListViewItem(line));
			}
			listQueue.EndUpdate();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (IsDesignEnvironment()) return;
			buttonAutoPlan.Click -= buttonAutoPlan_Click;
			buttonAutoPlan.Click += buttonAutoPlan_Click;
			buttonClear.Click -= buttonClear_Click;
			buttonClear.Click += buttonClear_Click;
			buttonClose.Click -= buttonClose_Click;
			buttonClose.Click += buttonClose_Click;
		}

		private void buttonAutoPlan_Click(object sender, EventArgs e)
		{
			OnAutoPlan?.Invoke();
			RefreshQueue();
		}

		private void buttonClear_Click(object sender, EventArgs e)
		{
			OnClear?.Invoke();
			RefreshQueue();
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
	}
}

