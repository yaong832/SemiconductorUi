namespace SemiconductorUi.Forms
{
    partial class PmDetailForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.labelUnitTitle = new System.Windows.Forms.Label();
            this.labelStatusValue = new System.Windows.Forms.Label();
            this.tableLayoutDetail = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutEnv = new System.Windows.Forms.TableLayoutPanel();
            this.labelRecipeValue = new System.Windows.Forms.Label();
            this.labelRecipeCaption = new System.Windows.Forms.Label();
            this.labelStepCaption = new System.Windows.Forms.Label();
            this.labelStepValue = new System.Windows.Forms.Label();
            this.labelTimeCaption = new System.Windows.Forms.Label();
            this.labelTimeValue = new System.Windows.Forms.Label();
            this.labelStepTimeCaption = new System.Windows.Forms.Label();
            this.labelStepTimeValue = new System.Windows.Forms.Label();
            this.labelMessageCaption = new System.Windows.Forms.Label();
            this.labelMessageValue = new System.Windows.Forms.Label();
            this.labelStepMessage = new System.Windows.Forms.Label();
            this.progressDetail = new System.Windows.Forms.ProgressBar();
            this.buttonClose = new System.Windows.Forms.Button();
            this.tableLayoutDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelUnitTitle
            // 
            this.labelUnitTitle.AutoSize = true;
            this.labelUnitTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelUnitTitle.ForeColor = System.Drawing.Color.White;
            this.labelUnitTitle.Location = new System.Drawing.Point(12, 9);
            this.labelUnitTitle.Name = "labelUnitTitle";
            this.labelUnitTitle.Size = new System.Drawing.Size(57, 25);
            this.labelUnitTitle.TabIndex = 0;
            this.labelUnitTitle.Text = "PM1";
            // 
            // labelStatusValue
            // 
            this.labelStatusValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatusValue.AutoSize = true;
            this.labelStatusValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatusValue.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.labelStatusValue.Location = new System.Drawing.Point(263, 15);
            this.labelStatusValue.Name = "labelStatusValue";
            this.labelStatusValue.Size = new System.Drawing.Size(76, 19);
            this.labelStatusValue.TabIndex = 1;
            this.labelStatusValue.Text = "CtcInUse";
            // 
            // tableLayoutDetail
            // 
            this.tableLayoutDetail.ColumnCount = 2;
            this.tableLayoutDetail.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutDetail.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutDetail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(54)))));
            this.tableLayoutDetail.Controls.Add(this.labelRecipeValue, 1, 0);
            this.tableLayoutDetail.Controls.Add(this.labelRecipeCaption, 0, 0);
            this.tableLayoutDetail.Controls.Add(this.labelStepCaption, 0, 1);
            this.tableLayoutDetail.Controls.Add(this.labelStepValue, 1, 1);
            this.tableLayoutDetail.Controls.Add(this.labelTimeCaption, 0, 2);
            this.tableLayoutDetail.Controls.Add(this.labelTimeValue, 1, 2);
            this.tableLayoutDetail.Controls.Add(this.labelStepTimeCaption, 0, 3);
            this.tableLayoutDetail.Controls.Add(this.labelStepTimeValue, 1, 3);
            this.tableLayoutDetail.Controls.Add(this.labelMessageCaption, 0, 4);
            this.tableLayoutDetail.Controls.Add(this.labelMessageValue, 1, 4);
            this.tableLayoutDetail.Location = new System.Drawing.Point(17, 46);
            this.tableLayoutDetail.Name = "tableLayoutDetail";
            this.tableLayoutDetail.RowCount = 5;
            this.tableLayoutDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutDetail.Size = new System.Drawing.Size(322, 130);
            this.tableLayoutDetail.TabIndex = 2;
            // 
            // tableLayoutEnv
            // 
            this.tableLayoutEnv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutEnv.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(54)))));
            this.tableLayoutEnv.ColumnCount = 4;
            this.tableLayoutEnv.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutEnv.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutEnv.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutEnv.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutEnv.Location = new System.Drawing.Point(357, 46);
            this.tableLayoutEnv.Name = "tableLayoutEnv";
            this.tableLayoutEnv.RowCount = 7;
            this.tableLayoutEnv.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutEnv.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutEnv.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutEnv.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutEnv.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutEnv.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutEnv.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutEnv.Size = new System.Drawing.Size(310, 180);
            this.tableLayoutEnv.TabIndex = 6;
            // 
            // labelRecipeValue
            // 
            this.labelRecipeValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRecipeValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelRecipeValue.ForeColor = System.Drawing.Color.White;
            this.labelRecipeValue.Location = new System.Drawing.Point(123, 0);
            this.labelRecipeValue.Name = "labelRecipeValue";
            this.labelRecipeValue.Size = new System.Drawing.Size(196, 26);
            this.labelRecipeValue.TabIndex = 1;
            this.labelRecipeValue.Text = "Time_150";
            this.labelRecipeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelRecipeCaption
            // 
            this.labelRecipeCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRecipeCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelRecipeCaption.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelRecipeCaption.Location = new System.Drawing.Point(3, 0);
            this.labelRecipeCaption.Name = "labelRecipeCaption";
            this.labelRecipeCaption.Size = new System.Drawing.Size(114, 26);
            this.labelRecipeCaption.TabIndex = 0;
            this.labelRecipeCaption.Text = "Name";
            this.labelRecipeCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStepCaption
            // 
            this.labelStepCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStepCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelStepCaption.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelStepCaption.Location = new System.Drawing.Point(3, 26);
            this.labelStepCaption.Name = "labelStepCaption";
            this.labelStepCaption.Size = new System.Drawing.Size(114, 26);
            this.labelStepCaption.TabIndex = 2;
            this.labelStepCaption.Text = "Step";
            this.labelStepCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStepValue
            // 
            this.labelStepValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStepValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelStepValue.ForeColor = System.Drawing.Color.White;
            this.labelStepValue.Location = new System.Drawing.Point(123, 26);
            this.labelStepValue.Name = "labelStepValue";
            this.labelStepValue.Size = new System.Drawing.Size(196, 26);
            this.labelStepValue.TabIndex = 3;
            this.labelStepValue.Text = "Step 1";
            this.labelStepValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTimeCaption
            // 
            this.labelTimeCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTimeCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelTimeCaption.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelTimeCaption.Location = new System.Drawing.Point(3, 52);
            this.labelTimeCaption.Name = "labelTimeCaption";
            this.labelTimeCaption.Size = new System.Drawing.Size(114, 26);
            this.labelTimeCaption.TabIndex = 4;
            this.labelTimeCaption.Text = "Time(sec)";
            this.labelTimeCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTimeValue
            // 
            this.labelTimeValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTimeValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelTimeValue.ForeColor = System.Drawing.Color.White;
            this.labelTimeValue.Location = new System.Drawing.Point(123, 52);
            this.labelTimeValue.Name = "labelTimeValue";
            this.labelTimeValue.Size = new System.Drawing.Size(196, 26);
            this.labelTimeValue.TabIndex = 5;
            this.labelTimeValue.Text = "0 / 0";
            this.labelTimeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStepTimeCaption
            // 
            this.labelStepTimeCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStepTimeCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelStepTimeCaption.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelStepTimeCaption.Location = new System.Drawing.Point(3, 78);
            this.labelStepTimeCaption.Name = "labelStepTimeCaption";
            this.labelStepTimeCaption.Size = new System.Drawing.Size(114, 26);
            this.labelStepTimeCaption.TabIndex = 6;
            this.labelStepTimeCaption.Text = "Step Time";
            this.labelStepTimeCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStepTimeValue
            // 
            this.labelStepTimeValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStepTimeValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelStepTimeValue.ForeColor = System.Drawing.Color.White;
            this.labelStepTimeValue.Location = new System.Drawing.Point(123, 78);
            this.labelStepTimeValue.Name = "labelStepTimeValue";
            this.labelStepTimeValue.Size = new System.Drawing.Size(196, 26);
            this.labelStepTimeValue.TabIndex = 7;
            this.labelStepTimeValue.Text = "0 / 0";
            this.labelStepTimeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelMessageCaption
            // 
            this.labelMessageCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMessageCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelMessageCaption.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelMessageCaption.Location = new System.Drawing.Point(3, 104);
            this.labelMessageCaption.Name = "labelMessageCaption";
            this.labelMessageCaption.Size = new System.Drawing.Size(114, 26);
            this.labelMessageCaption.TabIndex = 8;
            this.labelMessageCaption.Text = "Message";
            this.labelMessageCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelMessageValue
            // 
            this.labelMessageValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMessageValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelMessageValue.ForeColor = System.Drawing.Color.White;
            this.labelMessageValue.Location = new System.Drawing.Point(123, 104);
            this.labelMessageValue.Name = "labelMessageValue";
            this.labelMessageValue.Size = new System.Drawing.Size(196, 26);
            this.labelMessageValue.TabIndex = 9;
            this.labelMessageValue.Text = "0 / 0";
            this.labelMessageValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStepMessage
            // 
            this.labelStepMessage.AutoSize = true;
            this.labelStepMessage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelStepMessage.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelStepMessage.Location = new System.Drawing.Point(14, 187);
            this.labelStepMessage.Name = "labelStepMessage";
            this.labelStepMessage.Size = new System.Drawing.Size(116, 15);
            this.labelStepMessage.TabIndex = 3;
            this.labelStepMessage.Text = "[ ] Step Information";
            // 
            // progressDetail
            // 
            this.progressDetail.Location = new System.Drawing.Point(17, 207);
            this.progressDetail.Name = "progressDetail";
            this.progressDetail.Size = new System.Drawing.Size(322, 12);
            this.progressDetail.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressDetail.TabIndex = 4;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.buttonClose.FlatAppearance.BorderSize = 0;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonClose.ForeColor = System.Drawing.Color.White;
            this.buttonClose.Location = new System.Drawing.Point(264, 229);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 27);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // PmDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(681, 268);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.progressDetail);
            this.Controls.Add(this.labelStepMessage);
            this.Controls.Add(this.tableLayoutEnv);
            this.Controls.Add(this.tableLayoutDetail);
            this.Controls.Add(this.labelStatusValue);
            this.Controls.Add(this.labelUnitTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PmDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PM Detail";
            this.tableLayoutDetail.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUnitTitle;
        private System.Windows.Forms.Label labelStatusValue;
        private System.Windows.Forms.TableLayoutPanel tableLayoutDetail;
        private System.Windows.Forms.Label labelRecipeValue;
        private System.Windows.Forms.Label labelRecipeCaption;
        private System.Windows.Forms.Label labelStepCaption;
        private System.Windows.Forms.Label labelStepValue;
        private System.Windows.Forms.Label labelTimeCaption;
        private System.Windows.Forms.Label labelTimeValue;
        private System.Windows.Forms.Label labelStepTimeCaption;
        private System.Windows.Forms.Label labelStepTimeValue;
        private System.Windows.Forms.Label labelMessageCaption;
        private System.Windows.Forms.Label labelMessageValue;
        private System.Windows.Forms.Label labelStepMessage;
        private System.Windows.Forms.ProgressBar progressDetail;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutEnv;
    }
}

