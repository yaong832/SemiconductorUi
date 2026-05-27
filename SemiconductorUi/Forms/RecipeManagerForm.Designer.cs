using System;
using System.Windows.Forms;
using System.Drawing;

namespace SemiconductorUi.Forms
{
	partial class RecipeManagerForm
	{
		private System.ComponentModel.IContainer components = null;
		private SplitContainer splitContainer1;
		private ListBox listRecipes;
		private Button btnNew;
		private Button btnSave;
		private Button btnDelete;
		private Button btnClose;

		private TextBox txtName;
		private NumericUpDown numWafer;
		private NumericUpDown numDurA;
		private NumericUpDown numDurB;
		private NumericUpDown numDurC;
		private CheckBox chkSecond;

		private NumericUpDown pmaT, pmaP, pmaH;
		private NumericUpDown pmbT, pmbP, pmbH;
		private NumericUpDown pmcT, pmcP, pmcH;

		private NumericUpDown gasNF3, gasO2, gasCF4, gasRF;

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
			this.Text = "Recipe Manager";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ClientSize = new System.Drawing.Size(1180, 740);
			this.MinimumSize = new System.Drawing.Size(1100, 700);
			this.BackColor = System.Drawing.Color.FromArgb(250, 250, 255);

			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.listRecipes = new System.Windows.Forms.ListBox();
			var pnlLeftBottom = new System.Windows.Forms.FlowLayoutPanel();
			this.btnNew = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();

			var pnlRight = new System.Windows.Forms.Panel();
			var flRight = new System.Windows.Forms.FlowLayoutPanel();
			var lblName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			var lblWafer = new System.Windows.Forms.Label();
			this.numWafer = new System.Windows.Forms.NumericUpDown();
			var lblA = new System.Windows.Forms.Label();
			this.numDurA = new System.Windows.Forms.NumericUpDown();
			var lblB = new System.Windows.Forms.Label();
			this.numDurB = new System.Windows.Forms.NumericUpDown();
			var lblC = new System.Windows.Forms.Label();
			this.numDurC = new System.Windows.Forms.NumericUpDown();
			this.chkSecond = new System.Windows.Forms.CheckBox();

			var grpPMA = new System.Windows.Forms.GroupBox();
			var lblPmaT = new System.Windows.Forms.Label();
			var lblPmaP = new System.Windows.Forms.Label();
			var lblPmaH = new System.Windows.Forms.Label();
			this.pmaT = new System.Windows.Forms.NumericUpDown();
			this.pmaP = new System.Windows.Forms.NumericUpDown();
			this.pmaH = new System.Windows.Forms.NumericUpDown();

			var grpPMB = new System.Windows.Forms.GroupBox();
			var lblPmbT = new System.Windows.Forms.Label();
			var lblPmbP = new System.Windows.Forms.Label();
			var lblPmbH = new System.Windows.Forms.Label();
			this.pmbT = new System.Windows.Forms.NumericUpDown();
			this.pmbP = new System.Windows.Forms.NumericUpDown();
			this.pmbH = new System.Windows.Forms.NumericUpDown();

			var grpPMC = new System.Windows.Forms.GroupBox();
			var lblPmcT = new System.Windows.Forms.Label();
			var lblPmcP = new System.Windows.Forms.Label();
			var lblPmcH = new System.Windows.Forms.Label();
			this.pmcT = new System.Windows.Forms.NumericUpDown();
			this.pmcP = new System.Windows.Forms.NumericUpDown();
			this.pmcH = new System.Windows.Forms.NumericUpDown();

			var grpGas = new System.Windows.Forms.GroupBox();
			var lblNF3 = new System.Windows.Forms.Label();
			var lblO2 = new System.Windows.Forms.Label();
			var lblCF4 = new System.Windows.Forms.Label();
			var lblRF = new System.Windows.Forms.Label();
			this.gasNF3 = new System.Windows.Forms.NumericUpDown();
			this.gasO2 = new System.Windows.Forms.NumericUpDown();
			this.gasCF4 = new System.Windows.Forms.NumericUpDown();
			this.gasRF = new System.Windows.Forms.NumericUpDown();

			var grpGasPMB = new System.Windows.Forms.GroupBox();
			var lblB_NF3 = new System.Windows.Forms.Label();
			var lblB_O2 = new System.Windows.Forms.Label();
			var lblB_CF4 = new System.Windows.Forms.Label();
			var lblB_RF = new System.Windows.Forms.Label();
			var gasB_NF3 = new System.Windows.Forms.NumericUpDown();
			var gasB_O2 = new System.Windows.Forms.NumericUpDown();
			var gasB_CF4 = new System.Windows.Forms.NumericUpDown();
			var gasB_RF = new System.Windows.Forms.NumericUpDown();

			var grpGasPMC = new System.Windows.Forms.GroupBox();
			var lblC_NF3 = new System.Windows.Forms.Label();
			var lblC_O2 = new System.Windows.Forms.Label();
			var lblC_CF4 = new System.Windows.Forms.Label();
			var lblC_RF = new System.Windows.Forms.Label();
			var gasC_NF3 = new System.Windows.Forms.NumericUpDown();
			var gasC_O2 = new System.Windows.Forms.NumericUpDown();
			var gasC_CF4 = new System.Windows.Forms.NumericUpDown();
			var gasC_RF = new System.Windows.Forms.NumericUpDown();

			// splitContainer
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.BackColor = this.BackColor;
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Vertical;
			// 최소값 완화(초기 폭 0 시에도 예외 방지)
			this.splitContainer1.Panel1MinSize = 0;
			this.splitContainer1.Panel2MinSize = 0;
			// 초기 분할 거리 0으로 설정(Load 이후 보정)
			this.splitContainer1.SplitterDistance = 0;

			// left list
			this.listRecipes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listRecipes.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.listRecipes.BackColor = System.Drawing.Color.FromArgb(240, 240, 245);
			this.listRecipes.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
			this.listRecipes.SelectedIndexChanged += new System.EventHandler(this.listRecipes_SelectedIndexChanged);

			// left bottom panel
			pnlLeftBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			pnlLeftBottom.Height = 48;
			pnlLeftBottom.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
			pnlLeftBottom.Padding = new System.Windows.Forms.Padding(8);

			this.btnNew.Text = "신규";
			this.btnNew.Width = 80; this.btnNew.Height = 28;
			this.btnNew.BackColor = System.Drawing.Color.FromArgb(33, 150, 243);
			this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnNew.FlatAppearance.BorderSize = 0;
			this.btnNew.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
			this.btnNew.BackColor = System.Drawing.Color.FromArgb(100, 120, 130);
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

			this.btnSave.Text = "저장/수정";
			this.btnSave.Width = 90; this.btnSave.Height = 28;
			this.btnSave.BackColor = System.Drawing.Color.FromArgb(46, 125, 50);
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnSave.FlatAppearance.BorderSize = 0;
			this.btnSave.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
			this.btnSave.BackColor = System.Drawing.Color.FromArgb(100, 120, 130);
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

			this.btnDelete.Text = "삭제";
			this.btnDelete.Width = 80; this.btnDelete.Height = 28;
			this.btnDelete.BackColor = System.Drawing.Color.FromArgb(198, 40, 40);
			this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDelete.FlatAppearance.BorderSize = 0;
			this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
			this.btnDelete.BackColor = System.Drawing.Color.FromArgb(100, 120, 130);
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

			this.btnClose.Text = "닫기";
			this.btnClose.Width = 80; this.btnClose.Height = 28;
			this.btnClose.BackColor = System.Drawing.Color.FromArgb(120, 144, 156);
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnClose.FlatAppearance.BorderSize = 0;
			this.btnClose.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
			this.btnClose.BackColor = System.Drawing.Color.FromArgb(100, 120, 130);
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

			pnlLeftBottom.Controls.Add(this.btnNew);
			pnlLeftBottom.Controls.Add(this.btnSave);
			pnlLeftBottom.Controls.Add(this.btnDelete);
			pnlLeftBottom.Controls.Add(this.btnClose);

			this.splitContainer1.Panel1.Controls.Add(this.listRecipes);
			this.splitContainer1.Panel1.Controls.Add(pnlLeftBottom);

			// right panel
			pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
			pnlRight.BackColor = this.BackColor;
			pnlRight.Padding = new System.Windows.Forms.Padding(0);

			// right flow panel (auto layout)
			flRight.Dock = System.Windows.Forms.DockStyle.Fill;
			flRight.BackColor = this.BackColor;
			flRight.AutoScroll = true;
			flRight.AutoScrollMargin = new System.Drawing.Size(12, 12);
			flRight.WrapContents = true;
			flRight.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
			flRight.Padding = new System.Windows.Forms.Padding(12);
			flRight.Margin = new System.Windows.Forms.Padding(0);

			int x1 = 20; int y = 20; int wLbl = 140; int gap = 32;

			lblName.Text = "레시피 이름"; lblName.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblName.AutoSize = true; lblName.Location = new System.Drawing.Point(x1, y + 4);
			this.txtName = new System.Windows.Forms.TextBox(); this.txtName.Width = 260; this.txtName.Location = new System.Drawing.Point(x1 + wLbl + 8, y); this.txtName.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.txtName.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; y += gap;

			lblWafer.Text = "웨이퍼 수"; lblWafer.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblWafer.AutoSize = true; lblWafer.Location = new System.Drawing.Point(x1, y + 4);
			this.numWafer = new System.Windows.Forms.NumericUpDown(); this.numWafer.Minimum = 1; this.numWafer.Maximum = 50; this.numWafer.Width = 120; this.numWafer.Location = new System.Drawing.Point(x1 + wLbl + 8, y); this.numWafer.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.numWafer.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.numWafer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			y += gap;

			lblA.Text = "Duration A (s)"; lblA.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblA.AutoSize = true; lblA.Location = new System.Drawing.Point(x1, y + 4);
			this.numDurA = new System.Windows.Forms.NumericUpDown(); this.numDurA.Minimum = 1; this.numDurA.Maximum = 600; this.numDurA.Width = 120; this.numDurA.Location = new System.Drawing.Point(x1 + wLbl + 8, y); this.numDurA.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.numDurA.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.numDurA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; y += gap;

			lblB.Text = "Duration B (s)"; lblB.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblB.AutoSize = true; lblB.Location = new System.Drawing.Point(x1, y + 4);
			this.numDurB = new System.Windows.Forms.NumericUpDown(); this.numDurB.Minimum = 1; this.numDurB.Maximum = 600; this.numDurB.Width = 120; this.numDurB.Location = new System.Drawing.Point(x1 + wLbl + 8, y); this.numDurB.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.numDurB.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.numDurB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; y += gap;

			lblC.Text = "Duration C (s)"; lblC.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblC.AutoSize = true; lblC.Location = new System.Drawing.Point(x1, y + 4);
			this.numDurC = new System.Windows.Forms.NumericUpDown(); this.numDurC.Minimum = 1; this.numDurC.Maximum = 600; this.numDurC.Width = 120; this.numDurC.Location = new System.Drawing.Point(x1 + wLbl + 8, y); this.numDurC.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.numDurC.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.numDurC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; y += gap;

			this.chkSecond.Text = "2차 노광 사용"; this.chkSecond.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.chkSecond.AutoSize = true; this.chkSecond.Location = new System.Drawing.Point(x1, y); y += gap;

			// PMA
			grpPMA.Text = "PMA"; grpPMA.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); grpPMA.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); grpPMA.Size = new System.Drawing.Size(360, 150);
			grpPMA.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
			lblPmaT.Text = "Temp(°C)"; lblPmaT.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblPmaT.AutoSize = true; lblPmaT.Location = new System.Drawing.Point(12, 24);
			this.pmaT.Minimum = 0; this.pmaT.Maximum = 400; this.pmaT.Location = new System.Drawing.Point(120, 22); this.pmaT.Width = 100; this.pmaT.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.pmaT.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.pmaT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			lblPmaP.Text = "Press(Torr)"; lblPmaP.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblPmaP.AutoSize = true; lblPmaP.Location = new System.Drawing.Point(12, 54);
			this.pmaP.Minimum = 0; this.pmaP.Maximum = 10000; this.pmaP.Location = new System.Drawing.Point(120, 52); this.pmaP.Width = 100; this.pmaP.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.pmaP.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.pmaP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			lblPmaH.Text = "Hum(%)"; lblPmaH.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblPmaH.AutoSize = true; lblPmaH.Location = new System.Drawing.Point(12, 84);
			this.pmaH.Minimum = 0; this.pmaH.Maximum = 100; this.pmaH.Location = new System.Drawing.Point(120, 82); this.pmaH.Width = 100; this.pmaH.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.pmaH.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.pmaH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			grpPMA.Controls.Add(lblPmaT); grpPMA.Controls.Add(this.pmaT); grpPMA.Controls.Add(lblPmaP); grpPMA.Controls.Add(this.pmaP); grpPMA.Controls.Add(lblPmaH); grpPMA.Controls.Add(this.pmaH);

			// PMB
			grpPMB.Text = "PMB"; grpPMB.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); grpPMB.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); grpPMB.Size = new System.Drawing.Size(360, 150);
			grpPMB.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
			lblPmbT.Text = "Temp(°C)"; lblPmbT.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblPmbT.AutoSize = true; lblPmbT.Location = new System.Drawing.Point(12, 24);
			this.pmbT.Minimum = 0; this.pmbT.Maximum = 400; this.pmbT.Location = new System.Drawing.Point(120, 22); this.pmbT.Width = 100; this.pmbT.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.pmbT.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.pmbT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			lblPmbP.Text = "Press(Torr)"; lblPmbP.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblPmbP.AutoSize = true; lblPmbP.Location = new System.Drawing.Point(12, 54);
			this.pmbP.Minimum = 0; this.pmbP.Maximum = 10000; this.pmbP.Location = new System.Drawing.Point(120, 52); this.pmbP.Width = 100; this.pmbP.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.pmbP.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.pmbP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			lblPmbH.Text = "Hum(%)"; lblPmbH.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblPmbH.AutoSize = true; lblPmbH.Location = new System.Drawing.Point(12, 84);
			this.pmbH.Minimum = 0; this.pmbH.Maximum = 100; this.pmbH.Location = new System.Drawing.Point(120, 82); this.pmbH.Width = 100; this.pmbH.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.pmbH.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.pmbH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			grpPMB.Controls.Add(lblPmbT); grpPMB.Controls.Add(this.pmbT); grpPMB.Controls.Add(lblPmbP); grpPMB.Controls.Add(this.pmbP); grpPMB.Controls.Add(lblPmbH); grpPMB.Controls.Add(this.pmbH);

			// PMC
			grpPMC.Text = "PMC"; grpPMC.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); grpPMC.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); grpPMC.Size = new System.Drawing.Size(360, 150);
			grpPMC.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
			lblPmcT.Text = "Temp(°C)"; lblPmcT.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblPmcT.AutoSize = true; lblPmcT.Location = new System.Drawing.Point(12, 24);
			this.pmcT.Minimum = 0; this.pmcT.Maximum = 400; this.pmcT.Location = new System.Drawing.Point(120, 22); this.pmcT.Width = 100; this.pmcT.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.pmcT.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.pmcT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			lblPmcP.Text = "Press(Torr)"; lblPmcP.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblPmcP.AutoSize = true; lblPmcP.Location = new System.Drawing.Point(12, 54);
			this.pmcP.Minimum = 0; this.pmcP.Maximum = 10000; this.pmcP.Location = new System.Drawing.Point(120, 52); this.pmcP.Width = 100; this.pmcP.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.pmcP.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.pmcP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			lblPmcH.Text = "Hum(%)"; lblPmcH.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblPmcH.AutoSize = true; lblPmcH.Location = new System.Drawing.Point(12, 84);
			this.pmcH.Minimum = 0; this.pmcH.Maximum = 100; this.pmcH.Location = new System.Drawing.Point(120, 82); this.pmcH.Width = 100; this.pmcH.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.pmcH.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.pmcH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			grpPMC.Controls.Add(lblPmcT); grpPMC.Controls.Add(this.pmcT); grpPMC.Controls.Add(lblPmcP); grpPMC.Controls.Add(this.pmcP); grpPMC.Controls.Add(lblPmcH); grpPMC.Controls.Add(this.pmcH);

			// Gas group PMA
			grpGas.Text = "PMA Gas/RF"; grpGas.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); grpGas.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); grpGas.Size = new System.Drawing.Size(360, 160);
			grpGas.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
			lblNF3.Text = "NF3"; lblNF3.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblNF3.AutoSize = true; lblNF3.Location = new System.Drawing.Point(12, 28);
			this.gasNF3.Minimum = 0; this.gasNF3.Maximum = 5000; this.gasNF3.Width = 120; this.gasNF3.Location = new System.Drawing.Point(60, 26); this.gasNF3.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.gasNF3.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.gasNF3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			lblO2.Text = "O2"; lblO2.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblO2.AutoSize = true; lblO2.Location = new System.Drawing.Point(200, 28);
			this.gasO2.Minimum = 0; this.gasO2.Maximum = 5000; this.gasO2.Width = 120; this.gasO2.Location = new System.Drawing.Point(230, 26); this.gasO2.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.gasO2.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.gasO2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			lblCF4.Text = "CF4"; lblCF4.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblCF4.AutoSize = true; lblCF4.Location = new System.Drawing.Point(12, 78);
			this.gasCF4.Minimum = 0; this.gasCF4.Maximum = 5000; this.gasCF4.Width = 120; this.gasCF4.Location = new System.Drawing.Point(60, 76); this.gasCF4.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.gasCF4.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.gasCF4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			lblRF.Text = "RF"; lblRF.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblRF.AutoSize = true; lblRF.Location = new System.Drawing.Point(200, 78);
			this.gasRF.Minimum = 0; this.gasRF.Maximum = 50000; this.gasRF.Width = 120; this.gasRF.Location = new System.Drawing.Point(230, 76); this.gasRF.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); this.gasRF.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); this.gasRF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			grpGas.Controls.Add(lblNF3); grpGas.Controls.Add(this.gasNF3); grpGas.Controls.Add(lblO2); grpGas.Controls.Add(this.gasO2); grpGas.Controls.Add(lblCF4); grpGas.Controls.Add(this.gasCF4); grpGas.Controls.Add(lblRF); grpGas.Controls.Add(this.gasRF);

			// Gas group PMB
			grpGasPMB.Text = "PMB Gas/RF"; grpGasPMB.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); grpGasPMB.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); grpGasPMB.Size = new System.Drawing.Size(360, 160);
			grpGasPMB.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
			lblB_NF3.Text = "NF3"; lblB_NF3.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblB_NF3.AutoSize = true; lblB_NF3.Location = new System.Drawing.Point(12, 28);
			gasB_NF3.Minimum = 0; gasB_NF3.Maximum = 5000; gasB_NF3.Width = 120; gasB_NF3.Location = new System.Drawing.Point(60, 26); gasB_NF3.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); gasB_NF3.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); gasB_NF3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; gasB_NF3.Name = "gasB_NF3";
			lblB_O2.Text = "O2"; lblB_O2.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblB_O2.AutoSize = true; lblB_O2.Location = new System.Drawing.Point(200, 28);
			gasB_O2.Minimum = 0; gasB_O2.Maximum = 5000; gasB_O2.Width = 120; gasB_O2.Location = new System.Drawing.Point(230, 26); gasB_O2.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); gasB_O2.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); gasB_O2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; gasB_O2.Name = "gasB_O2";
			lblB_CF4.Text = "CF4"; lblB_CF4.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblB_CF4.AutoSize = true; lblB_CF4.Location = new System.Drawing.Point(12, 78);
			gasB_CF4.Minimum = 0; gasB_CF4.Maximum = 5000; gasB_CF4.Width = 120; gasB_CF4.Location = new System.Drawing.Point(60, 76); gasB_CF4.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); gasB_CF4.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); gasB_CF4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; gasB_CF4.Name = "gasB_CF4";
			lblB_RF.Text = "RF"; lblB_RF.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblB_RF.AutoSize = true; lblB_RF.Location = new System.Drawing.Point(200, 78);
			gasB_RF.Minimum = 0; gasB_RF.Maximum = 50000; gasB_RF.Width = 120; gasB_RF.Location = new System.Drawing.Point(230, 76); gasB_RF.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); gasB_RF.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); gasB_RF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; gasB_RF.Name = "gasB_RF";
			grpGasPMB.Controls.Add(lblB_NF3); grpGasPMB.Controls.Add(gasB_NF3); grpGasPMB.Controls.Add(lblB_O2); grpGasPMB.Controls.Add(gasB_O2); grpGasPMB.Controls.Add(lblB_CF4); grpGasPMB.Controls.Add(gasB_CF4); grpGasPMB.Controls.Add(lblB_RF); grpGasPMB.Controls.Add(gasB_RF);

			// Gas group PMC
			grpGasPMC.Text = "PMC Gas/RF"; grpGasPMC.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); grpGasPMC.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); grpGasPMC.Size = new System.Drawing.Size(360, 160);
			grpGasPMC.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
			lblC_NF3.Text = "NF3"; lblC_NF3.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblC_NF3.AutoSize = true; lblC_NF3.Location = new System.Drawing.Point(12, 28);
			gasC_NF3.Minimum = 0; gasC_NF3.Maximum = 5000; gasC_NF3.Width = 120; gasC_NF3.Location = new System.Drawing.Point(60, 26); gasC_NF3.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); gasC_NF3.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); gasC_NF3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; gasC_NF3.Name = "gasC_NF3";
			lblC_O2.Text = "O2"; lblC_O2.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblC_O2.AutoSize = true; lblC_O2.Location = new System.Drawing.Point(200, 28);
			gasC_O2.Minimum = 0; gasC_O2.Maximum = 5000; gasC_O2.Width = 120; gasC_O2.Location = new System.Drawing.Point(230, 26); gasC_O2.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); gasC_O2.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); gasC_O2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; gasC_O2.Name = "gasC_O2";
			lblC_CF4.Text = "CF4"; lblC_CF4.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblC_CF4.AutoSize = true; lblC_CF4.Location = new System.Drawing.Point(12, 78);
			gasC_CF4.Minimum = 0; gasC_CF4.Maximum = 5000; gasC_CF4.Width = 120; gasC_CF4.Location = new System.Drawing.Point(60, 76); gasC_CF4.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); gasC_CF4.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); gasC_CF4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; gasC_CF4.Name = "gasC_CF4";
			lblC_RF.Text = "RF"; lblC_RF.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); lblC_RF.AutoSize = true; lblC_RF.Location = new System.Drawing.Point(200, 78);
			gasC_RF.Minimum = 0; gasC_RF.Maximum = 50000; gasC_RF.Width = 120; gasC_RF.Location = new System.Drawing.Point(230, 76); gasC_RF.BackColor = System.Drawing.Color.FromArgb(240, 240, 245); gasC_RF.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40); gasC_RF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; gasC_RF.Name = "gasC_RF";
			grpGasPMC.Controls.Add(lblC_NF3); grpGasPMC.Controls.Add(gasC_NF3); grpGasPMC.Controls.Add(lblC_O2); grpGasPMC.Controls.Add(gasC_O2); grpGasPMC.Controls.Add(lblC_CF4); grpGasPMC.Controls.Add(gasC_CF4); grpGasPMC.Controls.Add(lblC_RF); grpGasPMC.Controls.Add(gasC_RF);

			// header fields keep absolute at top (inside flow as a block panel)
			var headerPanel = new System.Windows.Forms.Panel();
			headerPanel.Width = 920;
			headerPanel.Height = 240; // 충분한 높이로 확대
			headerPanel.MinimumSize = new System.Drawing.Size(920, 200);
			headerPanel.AutoSize = false;
			headerPanel.Margin = new System.Windows.Forms.Padding(8, 4, 8, 8);
			headerPanel.BackColor = this.BackColor;
			headerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));

			this.txtName.Location = new System.Drawing.Point(x1 + wLbl + 8, 20);
			lblName.Location = new System.Drawing.Point(x1, 24);
			this.numWafer.Location = new System.Drawing.Point(x1 + wLbl + 8, 20 + gap);
			lblWafer.Location = new System.Drawing.Point(x1, 24 + gap);
			this.numDurA.Location = new System.Drawing.Point(x1 + wLbl + 8, 20 + gap * 2);
			lblA.Location = new System.Drawing.Point(x1, 24 + gap * 2);
			this.numDurB.Location = new System.Drawing.Point(x1 + wLbl + 8, 20 + gap * 3);
			lblB.Location = new System.Drawing.Point(x1, 24 + gap * 3);
			this.numDurC.Location = new System.Drawing.Point(x1 + wLbl + 8, 20 + gap * 4);
			lblC.Location = new System.Drawing.Point(x1, 24 + gap * 4);
			this.chkSecond.Location = new System.Drawing.Point(x1, 20 + gap * 5);

			headerPanel.Controls.Add(lblName);
			headerPanel.Controls.Add(this.txtName);
			headerPanel.Controls.Add(lblWafer);
			headerPanel.Controls.Add(this.numWafer);
			headerPanel.Controls.Add(lblA);
			headerPanel.Controls.Add(this.numDurA);
			headerPanel.Controls.Add(lblB);
			headerPanel.Controls.Add(this.numDurB);
			headerPanel.Controls.Add(lblC);
			headerPanel.Controls.Add(this.numDurC);
			headerPanel.Controls.Add(this.chkSecond);

			flRight.Controls.Add(headerPanel);
			flRight.Controls.Add(grpPMA);
			flRight.Controls.Add(grpPMB);
			flRight.Controls.Add(grpPMC);
			flRight.Controls.Add(grpGas);
			flRight.Controls.Add(grpGasPMB);
			flRight.Controls.Add(grpGasPMC);
			// 줄바꿈 강제: Gas/RF는 다음 줄로 배치
			flRight.SetFlowBreak(grpPMC, true);

			pnlRight.Controls.Add(flRight);

			this.splitContainer1.Panel2.Controls.Add(pnlRight);
			this.Controls.Add(this.splitContainer1);
		}
	}
}

