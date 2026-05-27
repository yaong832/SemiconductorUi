using System;
using System.Windows.Forms;

namespace SemiconductorUi.Forms
{
    public partial class PmDetailForm : Form
    {
        private TextBox[,] envCells; // [row, col] -> 0:PV 1:SV
        private readonly string[] envNames = { "NF3", "O2", "CF4", "Press.", "RF Power", "Temp." };
        private readonly string[] envUnits = { "sccm", "sccm", "sccm", "Torr", "W", "°C" };

        public PmDetailForm()
        {
            InitializeComponent();
            BuildEnvironmentTable();
        }

        public void SetDetail(
            string unitName,
            string status,
            string recipeName,
            string stepName,
            string timeValue,
            string stepTimeValue,
            string messageValue,
            string stepMessage,
            int progress)
        {
            labelUnitTitle.Text = unitName;
            labelStatusValue.Text = status;
            labelRecipeValue.Text = recipeName;
            labelStepValue.Text = stepName;
            labelTimeValue.Text = timeValue;
            labelStepTimeValue.Text = stepTimeValue;
            labelMessageValue.Text = messageValue;
            labelStepMessage.Text = stepMessage;
            progressDetail.Value = Math.Max(0, Math.Min(100, progress));
        }

        private void BuildEnvironmentTable()
        {
            envCells = new TextBox[envNames.Length, 2];

            // Header
            var hName = new Label { Text = "", ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold) };
            var hPv = new Label { Text = "PV", ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold) };
            var hSv = new Label { Text = "SV", ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold) };
            var hUnit = new Label { Text = "", ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold) };
            tableLayoutEnv.Controls.Add(hName, 0, 0);
            tableLayoutEnv.Controls.Add(hPv, 1, 0);
            tableLayoutEnv.Controls.Add(hSv, 2, 0);
            tableLayoutEnv.Controls.Add(hUnit, 3, 0);

            for (int i = 0; i < envNames.Length; i++)
            {
                var name = new Label { Text = envNames[i], ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Font = new System.Drawing.Font("Segoe UI", 9F) };
                var pv = new TextBox { ReadOnly = true, Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, BackColor = System.Drawing.Color.FromArgb(62, 65, 73), ForeColor = System.Drawing.Color.White };
                var sv = new TextBox { ReadOnly = true, Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, BackColor = System.Drawing.Color.FromArgb(62, 65, 73), ForeColor = System.Drawing.Color.White };
                var unit = new Label { Text = envUnits[i], ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Font = new System.Drawing.Font("Segoe UI", 9F) };

                tableLayoutEnv.Controls.Add(name, 0, i + 1);
                tableLayoutEnv.Controls.Add(pv, 1, i + 1);
                tableLayoutEnv.Controls.Add(sv, 2, i + 1);
                tableLayoutEnv.Controls.Add(unit, 3, i + 1);

                envCells[i, 0] = pv;
                envCells[i, 1] = sv;
            }
        }

        public void SetEnvironment(
            double pvNF3, double svNF3,
            double pvO2, double svO2,
            double pvCF4, double svCF4,
            double pvPressTorr, double svPressTorr,
            double pvRfW, double svRfW,
            double pvTempC, double svTempC)
        {
            double[] pvs = { pvNF3, pvO2, pvCF4, pvPressTorr, pvRfW, pvTempC };
            double[] svs = { svNF3, svO2, svCF4, svPressTorr, svRfW, svTempC };
            for (int i = 0; i < envNames.Length; i++)
            {
                envCells[i, 0].Text = pvs[i].ToString("0.##");
                envCells[i, 1].Text = svs[i].ToString("0.##");
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

