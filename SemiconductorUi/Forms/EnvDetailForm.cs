using System;
using System.Windows.Forms;

namespace SemiconductorUi.Forms
{
    public partial class EnvDetailForm : Form
    {
        private Label[,] valueLabels; // [row, col] -> 0:PV 1:SV
        private NumericUpDown[] svEditors;
        private readonly string[] envNames = { "NF3", "O2", "CF4", "Press.", "RF Power", "Temp." };
        private readonly string[] envUnits = { "sccm", "sccm", "sccm", "Torr", "W", "°C" };
        private Func<double[]> pvProvider;
        public Action<double, double, double, double, double, double> OnApplySetpoints;

        public EnvDetailForm()
        {
            InitializeComponent();
            BuildEnvironmentTable();
        }

        private void BuildEnvironmentTable()
        {
            valueLabels = new Label[envNames.Length, 2];

            var hName = new Label { Text = "", ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold) };
            var hPv = new Label { Text = "PV", ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold) };
            var hSv = new Label { Text = "SV", ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold) };
            var hUnit = new Label { Text = "", ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold) };
            tableEnv.Controls.Add(hName, 0, 0);
            tableEnv.Controls.Add(hPv, 1, 0);
            tableEnv.Controls.Add(hSv, 2, 0);
            tableEnv.Controls.Add(hUnit, 3, 0);

            svEditors = new NumericUpDown[envNames.Length];
            for (int i = 0; i < envNames.Length; i++)
            {
                var name = new Label { Text = envNames[i], ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Font = new System.Drawing.Font("Segoe UI", 10F) };
                var pv = new Label { Dock = DockStyle.Fill, ForeColor = System.Drawing.Color.White, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold) };
                var sv = new NumericUpDown { Dock = DockStyle.Fill, ForeColor = System.Drawing.Color.White, TextAlign = HorizontalAlignment.Center, BackColor = System.Drawing.Color.FromArgb(62,65,73), BorderStyle = BorderStyle.FixedSingle };
                sv.DecimalPlaces = (i == 3) ? 3 : 1; // Press more precision
                sv.Maximum = 100000;
                sv.Minimum = 0;
                var unit = new Label { Text = envUnits[i], ForeColor = System.Drawing.Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Font = new System.Drawing.Font("Segoe UI", 10F) };

                tableEnv.Controls.Add(name, 0, i + 1);
                tableEnv.Controls.Add(pv, 1, i + 1);
                tableEnv.Controls.Add(sv, 2, i + 1);
                tableEnv.Controls.Add(unit, 3, i + 1);

                valueLabels[i, 0] = pv;
                valueLabels[i, 1] = null; // SV는 에디터가 담당
                svEditors[i] = sv;
            }
        }

        public void SetUnit(string unitKey)
        {
            labelUnit.Text = $"{unitKey} 환경 상세";
        }

        public void SetSetpoints(double svNF3, double svO2, double svCF4, double svPressTorr, double svRfW, double svTempC)
        {
            double[] svs = { svNF3, svO2, svCF4, svPressTorr, svRfW, svTempC };
            for (int i = 0; i < envNames.Length; i++)
            {
                svEditors[i].Value = (decimal)svs[i];
            }
        }

        public void UpdateProcessValues(double pvNF3, double pvO2, double pvCF4, double pvPressTorr, double pvRfW, double pvTempC)
        {
            double[] pvs = { pvNF3, pvO2, pvCF4, pvPressTorr, pvRfW, pvTempC };
            for (int i = 0; i < envNames.Length; i++)
            {
                valueLabels[i, 0].Text = pvs[i].ToString("0.##");
            }
        }

        public void StartLiveUpdates(Func<double[]> provider)
        {
            pvProvider = provider;
            liveTimer.Start();
        }

        private void liveTimer_Tick(object sender, EventArgs e)
        {
            if (pvProvider == null) return;
            var pvs = pvProvider.Invoke();
            if (pvs == null || pvs.Length < 6) return;
            for (int i = 0; i < envNames.Length; i++)
            {
                valueLabels[i, 0].Text = pvs[i].ToString("0.##");
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            liveTimer.Stop();
            Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            OnApplySetpoints?.Invoke(
                (double)svEditors[0].Value,
                (double)svEditors[1].Value,
                (double)svEditors[2].Value,
                (double)svEditors[3].Value,
                (double)svEditors[4].Value,
                (double)svEditors[5].Value
            );
        }
    }
}

