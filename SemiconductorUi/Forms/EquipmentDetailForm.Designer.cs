namespace SemiconductorUi.Forms
{
    partial class EquipmentDetailForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelStatusCaption = new System.Windows.Forms.Label();
            this.labelStatusValue = new System.Windows.Forms.Label();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(20, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(106, 25);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Unit Name";
            // 
            // labelStatusCaption
            // 
            this.labelStatusCaption.AutoSize = true;
            this.labelStatusCaption.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelStatusCaption.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelStatusCaption.Location = new System.Drawing.Point(22, 60);
            this.labelStatusCaption.Name = "labelStatusCaption";
            this.labelStatusCaption.Size = new System.Drawing.Size(41, 19);
            this.labelStatusCaption.TabIndex = 1;
            this.labelStatusCaption.Text = "상태:";
            // 
            // labelStatusValue
            // 
            this.labelStatusValue.AutoSize = true;
            this.labelStatusValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelStatusValue.ForeColor = System.Drawing.Color.White;
            this.labelStatusValue.Location = new System.Drawing.Point(69, 60);
            this.labelStatusValue.Name = "labelStatusValue";
            this.labelStatusValue.Size = new System.Drawing.Size(69, 19);
            this.labelStatusValue.TabIndex = 2;
            this.labelStatusValue.Text = "상태 정보";
            // 
            // textDescription
            // 
            this.textDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(54)))));
            this.textDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textDescription.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textDescription.ForeColor = System.Drawing.Color.Gainsboro;
            this.textDescription.Location = new System.Drawing.Point(24, 95);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.ReadOnly = true;
            this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDescription.Size = new System.Drawing.Size(392, 152);
            this.textDescription.TabIndex = 3;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.buttonClose.FlatAppearance.BorderSize = 0;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.buttonClose.ForeColor = System.Drawing.Color.White;
            this.buttonClose.Location = new System.Drawing.Point(328, 262);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(88, 32);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "닫기";
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // EquipmentDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(438, 311);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textDescription);
            this.Controls.Add(this.labelStatusValue);
            this.Controls.Add(this.labelStatusCaption);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EquipmentDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "상세 정보";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelStatusCaption;
        private System.Windows.Forms.Label labelStatusValue;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.Button buttonClose;
    }
}

