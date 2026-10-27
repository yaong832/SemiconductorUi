using System;
using System.Windows.Forms;
using System.Drawing;
using SemiconductorUi.Helpers;

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

		// 레이아웃 크기 (창은 크기 고정)
		private const int EditorLeft = 20;
		private const int CardTop = 176;
		private const int CardWidth = 246;
		private const int CardGap = 10;
		private const int RowLabelX = 14;
		private const int RowFieldX = 124;
		private const int RowFieldWidth = 106;

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
			this.Text = "레시피 관리";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ClientSize = new System.Drawing.Size(1180, 700);
			this.BackColor = UiTheme.Surface;
			this.ForeColor = UiTheme.TextPrimary;
			this.Font = new System.Drawing.Font("Segoe UI", 10F);

			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.listRecipes = new System.Windows.Forms.ListBox();
			this.btnNew = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.txtName = new System.Windows.Forms.TextBox();
			this.numWafer = NewNumber(1, 50);
			this.numDurA = NewNumber(1, 600);
			this.numDurB = NewNumber(1, 600);
			this.numDurC = NewNumber(1, 600);
			this.chkSecond = new System.Windows.Forms.CheckBox();
			this.pmaT = NewNumber(0, 400); this.pmaP = NewNumber(0, 10000); this.pmaH = NewNumber(0, 100);
			this.pmbT = NewNumber(0, 400); this.pmbP = NewNumber(0, 10000); this.pmbH = NewNumber(0, 100);
			this.pmcT = NewNumber(0, 400); this.pmcP = NewNumber(0, 10000); this.pmcH = NewNumber(0, 100);
			this.gasNF3 = NewNumber(0, 5000); this.gasO2 = NewNumber(0, 5000); this.gasCF4 = NewNumber(0, 5000); this.gasRF = NewNumber(0, 50000);
			// Chamber B/C 가스는 이름으로 찾습니다 (FindGasControlsFor)
			var gasB_NF3 = NewNumber(0, 5000, "gasB_NF3"); var gasB_O2 = NewNumber(0, 5000, "gasB_O2");
			var gasB_CF4 = NewNumber(0, 5000, "gasB_CF4"); var gasB_RF = NewNumber(0, 50000, "gasB_RF");
			var gasC_NF3 = NewNumber(0, 5000, "gasC_NF3"); var gasC_O2 = NewNumber(0, 5000, "gasC_O2");
			var gasC_CF4 = NewNumber(0, 5000, "gasC_CF4"); var gasC_RF = NewNumber(0, 50000, "gasC_RF");

			// splitContainer: 왼쪽 레시피 목록, 오른쪽 편집
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.BackColor = UiTheme.Border;
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Vertical;
			// 최소값 완화(초기 폭 0 시에도 예외 방지)
			this.splitContainer1.Panel1MinSize = 0;
			this.splitContainer1.Panel2MinSize = 0;
			// 초기 분할 거리 0으로 설정(Load 이후 보정)
			this.splitContainer1.SplitterDistance = 0;
			this.splitContainer1.Panel1.BackColor = UiTheme.SurfaceAlt;
			this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
			this.splitContainer1.Panel2.BackColor = UiTheme.Surface;

			// 왼쪽: 제목, 목록, 신규/삭제
			var lblListTitle = NewTitle("레시피 목록");
			lblListTitle.Dock = System.Windows.Forms.DockStyle.Top;
			lblListTitle.Height = 52;
			lblListTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

			var listHost = new System.Windows.Forms.Panel();
			listHost.Dock = System.Windows.Forms.DockStyle.Fill;
			listHost.BackColor = UiTheme.Border;
			listHost.Padding = new System.Windows.Forms.Padding(1);

			this.listRecipes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listRecipes.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listRecipes.BackColor = UiTheme.Surface;
			this.listRecipes.ForeColor = UiTheme.TextPrimary;
			this.listRecipes.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listRecipes.ItemHeight = 50;
			this.listRecipes.IntegralHeight = false;
			this.listRecipes.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listRecipes_DrawItem);
			this.listRecipes.SelectedIndexChanged += new System.EventHandler(this.listRecipes_SelectedIndexChanged);
			listHost.Controls.Add(this.listRecipes);

			var pnlLeftBottom = new System.Windows.Forms.TableLayoutPanel();
			pnlLeftBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			pnlLeftBottom.Height = 68;
			pnlLeftBottom.ColumnCount = 2;
			pnlLeftBottom.RowCount = 1;
			pnlLeftBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			pnlLeftBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			pnlLeftBottom.Padding = new System.Windows.Forms.Padding(0, 12, 0, 12);

			this.btnNew.Text = "신규";
			this.btnNew.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnNew.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

			this.btnDelete.Text = "삭제";
			this.btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnDelete.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

			pnlLeftBottom.Controls.Add(this.btnNew, 0, 0);
			pnlLeftBottom.Controls.Add(this.btnDelete, 1, 0);

			this.splitContainer1.Panel1.Controls.Add(listHost);
			this.splitContainer1.Panel1.Controls.Add(pnlLeftBottom);
			this.splitContainer1.Panel1.Controls.Add(lblListTitle);

			// 오른쪽: 편집 영역 + 아래 저장/닫기
			var pnlEditor = new System.Windows.Forms.Panel();
			pnlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			pnlEditor.BackColor = UiTheme.Surface;
			pnlEditor.AutoScroll = true;

			var lblEditorTitle = NewTitle("레시피 편집");
			lblEditorTitle.Location = new System.Drawing.Point(EditorLeft, 16);
			lblEditorTitle.AutoSize = true;
			pnlEditor.Controls.Add(lblEditorTitle);

			// 기본 정보
			var grpBasic = NewPlate("기본 정보", EditorLeft, 56, CardWidth * 3 + CardGap * 2, 104);
			var lblName = NewLabel("레시피 이름", 16, 38);
			this.txtName.Location = new System.Drawing.Point(116, 34);
			this.txtName.Width = 330;
			this.txtName.BackColor = UiTheme.Surface;
			this.txtName.ForeColor = UiTheme.TextPrimary;
			this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			var lblWafer = NewLabel("웨이퍼 수 (장)", 478, 38);
			this.numWafer.Location = new System.Drawing.Point(600, 34);
			this.numWafer.Width = 90;
			this.chkSecond.Text = "2차 노광 사용";
			this.chkSecond.AutoSize = true;
			this.chkSecond.ForeColor = UiTheme.TextPrimary;
			this.chkSecond.BackColor = LampButtonPainter.PlateBack;
			this.chkSecond.Location = new System.Drawing.Point(16, 70);
			grpBasic.Controls.Add(lblName);
			grpBasic.Controls.Add(this.txtName);
			grpBasic.Controls.Add(lblWafer);
			grpBasic.Controls.Add(this.numWafer);
			grpBasic.Controls.Add(this.chkSecond);
			pnlEditor.Controls.Add(grpBasic);

			// Chamber 카드: 공정 시간, 환경, 가스·RF (변수 이름은 저장 키 PMA/PMB/PMC를 따릅니다)
			pnlEditor.Controls.Add(NewChamberCard("Chamber A", 0, this.numDurA, this.pmaT, this.pmaP, this.pmaH, this.gasNF3, this.gasO2, this.gasCF4, this.gasRF));
			pnlEditor.Controls.Add(NewChamberCard("Chamber B", 1, this.numDurB, this.pmbT, this.pmbP, this.pmbH, gasB_NF3, gasB_O2, gasB_CF4, gasB_RF));
			pnlEditor.Controls.Add(NewChamberCard("Chamber C", 2, this.numDurC, this.pmcT, this.pmcP, this.pmcH, gasC_NF3, gasC_O2, gasC_CF4, gasC_RF));

			var pnlActions = new System.Windows.Forms.Panel();
			pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
			pnlActions.Height = 68;
			pnlActions.BackColor = UiTheme.Surface;
			pnlActions.Paint += (s, e) =>
			{
				using (var pen = new Pen(UiTheme.Border))
				{
					e.Graphics.DrawLine(pen, 0, 0, pnlActions.Width, 0);
				}
			};

			this.btnSave.Text = "저장";
			this.btnSave.Size = new System.Drawing.Size(140, 44);
			this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

			this.btnClose.Text = "닫기";
			this.btnClose.Size = new System.Drawing.Size(110, 44);
			this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

			pnlActions.Controls.Add(this.btnSave);
			pnlActions.Controls.Add(this.btnClose);
			pnlActions.Layout += (s, e) =>
			{
				int right = EditorLeft + CardWidth * 3 + CardGap * 2;
				this.btnSave.Location = new System.Drawing.Point(right - this.btnSave.Width, 12);
				this.btnClose.Location = new System.Drawing.Point(this.btnSave.Left - 10 - this.btnClose.Width, 12);
			};

			this.splitContainer1.Panel2.Controls.Add(pnlEditor);
			this.splitContainer1.Panel2.Controls.Add(pnlActions);

			foreach (var b in new[] { this.btnNew, this.btnDelete, this.btnClose })
			{
				b.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
				LampButtonPainter.AttachPlain(b);
			}
			this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			LampButtonPainter.AttachPlain(this.btnSave, primary: true);

			this.Controls.Add(this.splitContainer1);
		}

		/// <summary>Chamber 한 대의 설정 카드. 위에서부터 공정 시간, 환경, 가스·RF 순서입니다.</summary>
		private GroupBox NewChamberCard(string title, int column, NumericUpDown duration,
			NumericUpDown temp, NumericUpDown press, NumericUpDown hum,
			NumericUpDown nf3, NumericUpDown o2, NumericUpDown cf4, NumericUpDown rf)
		{
			var card = NewPlate(title, EditorLeft + column * (CardWidth + CardGap), CardTop, CardWidth, 0);
			int y = 34;
			AddCaption(card, "공정", ref y);
			AddRow(card, "공정 시간 (초)", duration, ref y);
			y += 6;
			AddCaption(card, "환경", ref y);
			AddRow(card, "온도 (°C)", temp, ref y);
			AddRow(card, "압력 (Torr)", press, ref y);
			AddRow(card, "습도 (%)", hum, ref y);
			y += 6;
			AddCaption(card, "가스 · RF", ref y);
			AddRow(card, "NF3 (sccm)", nf3, ref y);
			AddRow(card, "O2 (sccm)", o2, ref y);
			AddRow(card, "CF4 (sccm)", cf4, ref y);
			AddRow(card, "RF (W)", rf, ref y);
			card.Height = y + 10;
			return card;
		}

		private static void AddCaption(Control card, string text, ref int y)
		{
			var caption = new System.Windows.Forms.Label();
			caption.Text = text;
			caption.AutoSize = true;
			caption.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
			caption.ForeColor = UiTheme.Primary;
			caption.BackColor = LampButtonPainter.PlateBack;
			caption.Location = new System.Drawing.Point(RowLabelX, y);
			card.Controls.Add(caption);
			y += 22;
		}

		private static void AddRow(Control card, string label, NumericUpDown field, ref int y)
		{
			card.Controls.Add(NewLabel(label, RowLabelX, y + 4));
			field.Location = new System.Drawing.Point(RowFieldX, y);
			field.Width = RowFieldWidth;
			card.Controls.Add(field);
			y += 32;
		}

		private static NumericUpDown NewNumber(int min, int max, string name = null)
		{
			var n = new System.Windows.Forms.NumericUpDown();
			n.Minimum = min;
			n.Maximum = max;
			n.BackColor = UiTheme.Surface;
			n.ForeColor = UiTheme.TextPrimary;
			n.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			if (name != null) n.Name = name;
			return n;
		}

		private static Label NewLabel(string text, int x, int y)
		{
			var label = new System.Windows.Forms.Label();
			label.Text = text;
			label.AutoSize = true;
			label.ForeColor = UiTheme.TextSecondary;
			label.BackColor = LampButtonPainter.PlateBack;
			label.Font = new System.Drawing.Font("Segoe UI", 9F);
			label.Location = new System.Drawing.Point(x, y);
			return label;
		}

		private static Label NewTitle(string text)
		{
			var label = new System.Windows.Forms.Label();
			label.Text = text;
			label.ForeColor = UiTheme.TextPrimary;
			label.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
			return label;
		}

		private static GroupBox NewPlate(string title, int x, int y, int width, int height)
		{
			var plate = new System.Windows.Forms.GroupBox();
			plate.Text = title;
			plate.ForeColor = UiTheme.TextPrimary;
			plate.Location = new System.Drawing.Point(x, y);
			plate.Size = new System.Drawing.Size(width, height);
			plate.Padding = new System.Windows.Forms.Padding(12, 3, 12, 3);
			LampButtonPainter.AttachPlate(plate);
			return plate;
		}
	}
}
