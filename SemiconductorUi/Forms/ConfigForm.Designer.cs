using System;
using System.Drawing;
using System.Windows.Forms;

namespace SemiconductorUi.Forms
{
    partial class ConfigForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelMain;
        private GroupBox grpTemp;
        private GroupBox grpPress;
        private GroupBox grpRf;
        private GroupBox grpGas;
        private NumericUpDown numTempWarn;
        private NumericUpDown numTempAlarm;
        private NumericUpDown numPressWarnRatio;
        private NumericUpDown numPressAlarmRatio;
        private NumericUpDown numPressWarnAbs;
        private NumericUpDown numPressAlarmAbs;
        private NumericUpDown numRfWarnRatio;
        private NumericUpDown numRfAlarmRatio;
        private NumericUpDown numGasWarn;
        private NumericUpDown numGasAlarm;
        private NumericUpDown numGasLeakWarn;
        private NumericUpDown numGasLeakAlarm;
        private Button btnSave;
        private Button btnCancel;
        private Button btnReset;
        private FlowLayoutPanel panelButtons;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "설정 (Configuration)";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(800, 650);
            this.MinimumSize = new Size(800, 650);
            this.BackColor = Color.FromArgb(250, 250, 255);

            // 메인 패널
            this.panelMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true
            };
            this.Controls.Add(panelMain);

            // 온도 그룹
            this.grpTemp = CreateGroupBox("온도 알람 임계값", 0);
            CreateNumericControl(grpTemp, "경고 차이 (°C):", out numTempWarn, 0, 50, 2, 0, 0);
            CreateNumericControl(grpTemp, "알람 차이 (°C):", out numTempAlarm, 0, 50, 5, 0, 1);

            // 압력 그룹
            this.grpPress = CreateGroupBox("압력 알람 임계값", 1);
            CreateNumericControl(grpPress, "경고 비율 (SV 대비 %):", out numPressWarnRatio, 0, 2, 0.2m, 3, 0);
            CreateNumericControl(grpPress, "알람 비율 (SV 대비 %):", out numPressAlarmRatio, 0, 2, 0.5m, 3, 1);
            CreateNumericControl(grpPress, "경고 절대값 (Torr):", out numPressWarnAbs, 0, 100, 3, 0, 2);
            CreateNumericControl(grpPress, "알람 절대값 (Torr):", out numPressAlarmAbs, 0, 100, 10, 0, 3);

            // RF 그룹
            this.grpRf = CreateGroupBox("RF 알람 임계값", 2);
            CreateNumericControl(grpRf, "경고 비율 (SV 대비 %):", out numRfWarnRatio, 0, 1, 0.10m, 3, 0);
            CreateNumericControl(grpRf, "알람 비율 (SV 대비 %):", out numRfAlarmRatio, 0, 1, 0.20m, 3, 1);

            // 가스 그룹
            this.grpGas = CreateGroupBox("가스 알람 임계값", 3);
            CreateNumericControl(grpGas, "경고 절대값 (sccm):", out numGasWarn, 0, 100, 5, 1, 0);
            CreateNumericControl(grpGas, "알람 절대값 (sccm):", out numGasAlarm, 0, 100, 10, 1, 1);
            CreateNumericControl(grpGas, "누설 경고 (sccm):", out numGasLeakWarn, 0, 10, 1, 1, 2);
            CreateNumericControl(grpGas, "누설 알람 (sccm):", out numGasLeakAlarm, 0, 10, 3, 1, 3);

            // 버튼 패널
            this.panelButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(15),
                BackColor = Color.FromArgb(240, 240, 245)
            };
            this.Controls.Add(panelButtons);

            this.btnSave = CreateButton("저장", Color.FromArgb(72, 115, 90), DialogResult.None);
            this.btnCancel = CreateButton("취소", Color.FromArgb(97, 97, 97), DialogResult.Cancel);
            this.btnReset = CreateButton("기본값 복원", Color.FromArgb(128, 74, 74), DialogResult.None);

            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            btnReset.Click += btnReset_Click;

            panelButtons.Controls.Add(btnSave);
            panelButtons.Controls.Add(btnCancel);
            panelButtons.Controls.Add(btnReset);
        }

        private GroupBox CreateGroupBox(string title, int index)
        {
            var grp = new GroupBox
            {
                Text = title,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                BackColor = Color.FromArgb(240, 240, 245),
                Padding = new Padding(15),
                Margin = new Padding(0, 0, 0, 15),
                AutoSize = false,
                Width = 750,
                Height = 150,
                Location = new Point(10, index * 160 + 10)
            };

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                Padding = new Padding(10)
            };
            grp.Controls.Add(flow);

            panelMain.Controls.Add(grp);
            return grp;
        }

        private void CreateNumericControl(GroupBox parent, string labelText, out NumericUpDown num, decimal min, decimal max, decimal defaultValue, int decimals, int rowIndex)
        {
            var flow = parent.Controls[0] as FlowLayoutPanel;
            if (flow == null) flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = true, AutoSize = true };

            var lbl = new Label
            {
                Text = labelText,
                ForeColor = Color.FromArgb(40, 40, 40),
                Font = new Font("Segoe UI", 9F),
                Width = 180,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Height = 28,
                Padding = new Padding(0, 0, 5, 0)
            };

            num = new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                Value = defaultValue,
                DecimalPlaces = decimals,
                Width = 130,
                Height = 28,
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(240, 240, 245),
                ForeColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Right
            };

            var pnl = new Panel
            {
                Width = 340,
                Height = 35,
                Margin = new Padding(8, 5, 8, 5)
            };
            pnl.Controls.Add(lbl);
            lbl.Location = new Point(0, 3);
            pnl.Controls.Add(num);
            num.Location = new Point(205, 0);

            if (parent.Controls.Count == 0)
            {
                var flowPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = true,
                    AutoSize = true,
                    Padding = new Padding(10)
                };
                parent.Controls.Add(flowPanel);
            }

            (parent.Controls[0] as FlowLayoutPanel)?.Controls.Add(pnl);
        }

        private Button CreateButton(string text, Color backColor, DialogResult dialogResult)
        {
            return new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = Color.FromArgb(40, 40, 40),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Width = 110,
                Height = 38,
                Margin = new Padding(8, 5, 8, 5),
                DialogResult = dialogResult,
                TextAlign = ContentAlignment.MiddleCenter
            };
        }
    }
}

