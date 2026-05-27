using System;
using System.Windows.Forms;

namespace SemiconductorUi.Forms
{
    public partial class EquipmentDetailForm : Form
    {
        public EquipmentDetailForm()
        {
            InitializeComponent();
        }

        public void SetDetail(string unitName, string status, string description)
        {
            labelTitle.Text = unitName;
            labelStatusValue.Text = status;
            textDescription.Text = description;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

