using System;
using System.Windows.Forms;

namespace SemiconductorUi.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public string EnteredUsername => textBoxUsername.Text.Trim();
        public string EnteredPassword => textBoxPassword.Text;

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EnteredUsername) || string.IsNullOrWhiteSpace(EnteredPassword))
            {
                labelError.Text = "아이디와 비밀번호를 입력하세요.";
                labelError.Visible = true;
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

