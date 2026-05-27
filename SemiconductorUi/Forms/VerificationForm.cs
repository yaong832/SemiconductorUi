using System;
using System.Drawing;
using System.Windows.Forms;

namespace SemiconductorUi.Forms
{
	public class VerificationForm : Form
	{
		private ListView listChecks;
		private Button buttonClose;

		public VerificationForm()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Text = "Verification - 인터락 점검";
			this.StartPosition = FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.BackColor = Color.FromArgb(250, 250, 255);
			this.ClientSize = new Size(760, 460);

			listChecks = new ListView();
			listChecks.Dock = DockStyle.Fill;
			listChecks.View = View.Details;
			listChecks.FullRowSelect = true;
			listChecks.GridLines = true;
			listChecks.Columns.Add("체크 항목 (무엇을 점검하는가)", 520);
			listChecks.Columns.Add("결과(OK/FAIL)", 180, HorizontalAlignment.Center);
			listChecks.ForeColor = Color.FromArgb(40, 40, 40);
			listChecks.BackColor = Color.FromArgb(240, 240, 245);

			var bottom = new FlowLayoutPanel();
			bottom.Dock = DockStyle.Bottom;
			bottom.Height = 52;
			bottom.Padding = new Padding(0, 8, 12, 8);
			bottom.FlowDirection = FlowDirection.RightToLeft;
			bottom.WrapContents = false;

			buttonClose = new Button();
			buttonClose.Text = "Close";
			buttonClose.Width = 80;
			buttonClose.Height = 28;
			buttonClose.Margin = new Padding(8, 8, 0, 8);
			buttonClose.BackColor = Color.FromArgb(100, 120, 130);
			buttonClose.FlatStyle = FlatStyle.Flat;
			buttonClose.FlatAppearance.BorderSize = 0;
			buttonClose.ForeColor = Color.White;

			bottom.Controls.Add(buttonClose);

			this.Padding = new Padding(12, 12, 12, 12);
			this.Controls.Add(listChecks);
			this.Controls.Add(bottom);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (IsDesignEnvironment()) return;
			buttonClose.Click -= buttonClose_Click;
			buttonClose.Click += buttonClose_Click;
		}

		public void SetResults((string name, bool ok)[] checks, string summaryText)
		{
			listChecks.BeginUpdate();
			listChecks.Items.Clear();
			foreach (var c in checks)
			{
				var item = new ListViewItem(c.name);
				item.SubItems.Add(c.ok ? "OK" : "FAIL");
				item.ForeColor = c.ok ? Color.FromArgb(40, 40, 40) : Color.OrangeRed;
				listChecks.Items.Add(item);
			}
			listChecks.EndUpdate();

			this.Text = string.IsNullOrWhiteSpace(summaryText)
				? "Verification - 인터락 점검"
				: $"Verification - {summaryText}";
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

