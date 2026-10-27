using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.Helpers;

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
        private Button btnTeaching;
        private Panel panelButtons;

        // 레이아웃 크기 (창은 크기 고정)
        private const int PageLeft = 20;
        private const int PlateWidth = 374;
        private const int PlateGap = 12;
        private const int RowHeight = 36;

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
            this.Text = "설정";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(800, 520);
            this.BackColor = UiTheme.Surface;
            this.ForeColor = UiTheme.TextPrimary;
            this.Font = new Font("Segoe UI", 10F);

            // 메인 패널
            this.panelMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Surface,
                AutoScroll = true
            };
            this.Controls.Add(panelMain);

            var title = new Label
            {
                Text = "알람 기준",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(PageLeft, 16)
            };
            var subtitle = new Label
            {
                Text = "현재값(PV)이 설정값(SV)에서 이만큼 벗어나면 경고 또는 알람을 띄웁니다.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = UiTheme.TextSecondary,
                AutoSize = true,
                Location = new Point(PageLeft, 44)
            };
            panelMain.Controls.Add(title);
            panelMain.Controls.Add(subtitle);

            // 윗줄: 온도 | RF, 아랫줄: 압력 | 가스
            const string ratioHint = "비율은 소수로 입력합니다 (0.20 = SV의 20%).";

            this.grpTemp = CreateGroupBox("온도", 0, 76, 2, null);
            CreateNumericControl(grpTemp, "경고 차이 (°C)", out numTempWarn, 0, 50, 2, 0, 0);
            CreateNumericControl(grpTemp, "알람 차이 (°C)", out numTempAlarm, 0, 50, 5, 0, 1);

            this.grpRf = CreateGroupBox("RF", 1, 76, 2, ratioHint);
            CreateNumericControl(grpRf, "경고 비율 (SV 대비)", out numRfWarnRatio, 0, 1, 0.10m, 3, 0);
            CreateNumericControl(grpRf, "알람 비율 (SV 대비)", out numRfAlarmRatio, 0, 1, 0.20m, 3, 1);

            grpTemp.Height = grpRf.Height;
            int secondRowTop = 76 + grpRf.Height + PlateGap;

            this.grpPress = CreateGroupBox("압력", 0, secondRowTop, 4, ratioHint);
            CreateNumericControl(grpPress, "경고 비율 (SV 대비)", out numPressWarnRatio, 0, 2, 0.2m, 3, 0);
            CreateNumericControl(grpPress, "알람 비율 (SV 대비)", out numPressAlarmRatio, 0, 2, 0.5m, 3, 1);
            CreateNumericControl(grpPress, "경고 절대값 (Torr)", out numPressWarnAbs, 0, 100, 3, 0, 2);
            CreateNumericControl(grpPress, "알람 절대값 (Torr)", out numPressAlarmAbs, 0, 100, 10, 0, 3);

            this.grpGas = CreateGroupBox("가스", 1, secondRowTop, 4, null);
            CreateNumericControl(grpGas, "경고 절대값 (sccm)", out numGasWarn, 0, 100, 5, 1, 0);
            CreateNumericControl(grpGas, "알람 절대값 (sccm)", out numGasAlarm, 0, 100, 10, 1, 1);
            CreateNumericControl(grpGas, "누설 경고 (sccm)", out numGasLeakWarn, 0, 10, 1, 1, 2);
            CreateNumericControl(grpGas, "누설 알람 (sccm)", out numGasLeakAlarm, 0, 10, 3, 1, 3);
            grpGas.Height = grpPress.Height;

            // 버튼 패널: 왼쪽 티칭 위치, 오른쪽 기본값 복원 / 취소 / 저장
            this.panelButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 68,
                BackColor = UiTheme.Surface
            };
            this.panelButtons.Paint += (s, e) =>
            {
                using (var pen = new Pen(UiTheme.Border))
                {
                    e.Graphics.DrawLine(pen, 0, 0, panelButtons.Width, 0);
                }
            };
            this.Controls.Add(panelButtons);

            this.btnSave = CreateButton("저장", 120, DialogResult.None);
            this.btnCancel = CreateButton("취소", 100, DialogResult.Cancel);
            this.btnReset = CreateButton("기본값 복원", 130, DialogResult.None);
            this.btnTeaching = CreateButton("TM 티칭 위치", 140, DialogResult.None);

            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            btnReset.Click += btnReset_Click;
            btnTeaching.Click += btnTeaching_Click;

            LampButtonPainter.AttachPlain(btnSave, primary: true);
            LampButtonPainter.AttachPlain(btnCancel);
            LampButtonPainter.AttachPlain(btnReset);
            LampButtonPainter.AttachPlain(btnTeaching);

            panelButtons.Controls.Add(btnSave);
            panelButtons.Controls.Add(btnCancel);
            panelButtons.Controls.Add(btnReset);
            panelButtons.Controls.Add(btnTeaching);
            panelButtons.Layout += (s, e) =>
            {
                int right = PageLeft + PlateWidth * 2 + PlateGap;
                btnSave.Location = new Point(right - btnSave.Width, 12);
                btnCancel.Location = new Point(btnSave.Left - 10 - btnCancel.Width, 12);
                btnReset.Location = new Point(btnCancel.Left - 10 - btnReset.Width, 12);
                btnTeaching.Location = new Point(PageLeft, 12);
            };
        }

        /// <summary>항목 묶음 판. column 0/1 = 왼쪽/오른쪽 열, rows = 입력 줄 수, hint = 아래 안내 문구.</summary>
        private GroupBox CreateGroupBox(string title, int column, int top, int rows, string hint)
        {
            int height = 38 + rows * RowHeight + (hint != null ? 26 : 8);
            var grp = new GroupBox
            {
                Text = title,
                ForeColor = UiTheme.TextPrimary,
                Padding = new Padding(12, 3, 12, 3),
                Location = new Point(PageLeft + column * (PlateWidth + PlateGap), top),
                Size = new Size(PlateWidth, height)
            };
            LampButtonPainter.AttachPlate(grp);

            if (hint != null)
            {
                grp.Controls.Add(new Label
                {
                    Text = hint,
                    Font = new Font("Segoe UI", 8.5F),
                    ForeColor = UiTheme.TextSecondary,
                    BackColor = LampButtonPainter.PlateBack,
                    AutoSize = true,
                    Location = new Point(14, height - 26)
                });
            }

            panelMain.Controls.Add(grp);
            return grp;
        }

        private void CreateNumericControl(GroupBox parent, string labelText, out NumericUpDown num, decimal min, decimal max, decimal defaultValue, int decimals, int rowIndex)
        {
            int y = 34 + rowIndex * RowHeight;
            var lbl = new Label
            {
                Text = labelText,
                ForeColor = UiTheme.TextSecondary,
                BackColor = LampButtonPainter.PlateBack,
                Font = new Font("Segoe UI", 9F),
                AutoSize = true,
                Location = new Point(14, y + 5)
            };

            num = new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                Value = defaultValue,
                DecimalPlaces = decimals,
                Width = 120,
                BackColor = UiTheme.Surface,
                ForeColor = UiTheme.TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(PlateWidth - 14 - 120, y)
            };

            parent.Controls.Add(lbl);
            parent.Controls.Add(num);
        }

        private Button CreateButton(string text, int width, DialogResult dialogResult)
        {
            return new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(width, 44),
                DialogResult = dialogResult
            };
        }
    }
}
