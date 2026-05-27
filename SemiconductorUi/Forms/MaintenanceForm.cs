using System;
using System.Drawing;
using System.Windows.Forms;

namespace SemiconductorUi.Forms
{
	public class MaintenanceForm : Form
	{
		private Button buttonLeakTest;
		private Button buttonPumpPurge;
		private Button buttonSensorCalib;
		private Button buttonDoorCycle;
		private Button buttonClose;
		private Label labelStatus;

		public Action OnLeakTest;
		public Action OnPumpPurge;
		public Action OnSensorCalibration;
		public Action OnDoorCycleTest;

		public MaintenanceForm()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Text = "Maintenance - 점검/조치";
			this.StartPosition = FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.BackColor = Color.FromArgb(250, 250, 255);
			this.ClientSize = new Size(620, 300);

			var actions = new FlowLayoutPanel();
			actions.Location = new Point(12, 12);
			actions.Size = new Size(580, 44);
			actions.FlowDirection = FlowDirection.LeftToRight;
			actions.WrapContents = false;

			buttonLeakTest = CreateActionButton("Leak Test", new Size(100, 34), buttonLeakTest_Click);
			buttonPumpPurge = CreateActionButton("Pump Purge", new Size(100, 34), buttonPumpPurge_Click);
			buttonSensorCalib = CreateActionButton("Sensor Calib", new Size(110, 34), buttonSensorCalib_Click);
			buttonDoorCycle = CreateActionButton("Door Cycle", new Size(100, 34), buttonDoorCycle_Click);

			actions.Controls.Add(buttonLeakTest);
			actions.Controls.Add(buttonPumpPurge);
			actions.Controls.Add(buttonSensorCalib);
			actions.Controls.Add(buttonDoorCycle);

			labelStatus = new Label();
			labelStatus.Text = "Ready";
			labelStatus.ForeColor = Color.FromArgb(40, 40, 40);
			labelStatus.Location = new Point(16, 70);
			labelStatus.Size = new Size(580, 24);
			labelStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

			buttonClose = new Button();
			buttonClose.Text = "Close";
			buttonClose.Width = 80;
			buttonClose.Height = 28;
			buttonClose.Location = new Point(520, 240);
			buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonClose.BackColor = Color.FromArgb(100, 120, 130);
			buttonClose.FlatStyle = FlatStyle.Flat;
			buttonClose.FlatAppearance.BorderSize = 0;
			buttonClose.ForeColor = Color.White;
			buttonClose.Click += buttonClose_Click;

			this.Controls.Add(actions);
			this.Controls.Add(labelStatus);
			this.Controls.Add(buttonClose);
		}

		private Button CreateActionButton(string text, Size size, EventHandler handler)
		{
			var b = new Button();
			b.Text = text;
			b.Size = size;
			b.BackColor = Color.FromArgb(100, 120, 130);
			b.FlatStyle = FlatStyle.Flat;
			b.FlatAppearance.BorderSize = 0;
			b.ForeColor = Color.White;
			b.Click += handler;
			return b;
		}

		private void SetStatus(string text)
		{
			labelStatus.Text = text;
		}

		private void buttonLeakTest_Click(object sender, EventArgs e)
		{
			OnLeakTest?.Invoke();
			SetStatus("Leak Test 실행");
		}

		private void buttonPumpPurge_Click(object sender, EventArgs e)
		{
			OnPumpPurge?.Invoke();
			SetStatus("Pump Purge 실행");
		}

		private void buttonSensorCalib_Click(object sender, EventArgs e)
		{
			OnSensorCalibration?.Invoke();
			SetStatus("Sensor Calibration 실행");
		}

		private void buttonDoorCycle_Click(object sender, EventArgs e)
		{
			OnDoorCycleTest?.Invoke();
			SetStatus("Door Cycle Test 실행");
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}

