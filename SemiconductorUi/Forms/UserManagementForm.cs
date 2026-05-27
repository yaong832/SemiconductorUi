using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.Models;
using SemiconductorUi.Repositories;
using SemiconductorUi;

namespace SemiconductorUi.Forms
{
	public class UserManagementForm : Form
	{
		private ListView listUsers;
		private TextBox textBoxUsername;
		private TextBox textBoxPassword;
		private ComboBox comboBoxRole;
		private Button buttonAdd;
		private Button buttonUpdate;
		private Button buttonDelete;
		private Button buttonClose;
		private Label labelUsername;
		private Label labelPassword;
		private Label labelRole;
		private List<User> users;

		public UserManagementForm()
		{
			InitializeComponent();
			LoadUsers();
		}

		private void InitializeComponent()
		{
			this.Text = "사용자 관리";
			this.StartPosition = FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.BackColor = Color.FromArgb(250, 250, 255);
			this.ClientSize = new Size(700, 500);

			// 사용자 목록
			listUsers = new ListView();
			listUsers.Location = new Point(16, 16);
			listUsers.Size = new Size(660, 280);
			listUsers.View = View.Details;
			listUsers.FullRowSelect = true;
			listUsers.GridLines = true;
			listUsers.ForeColor = Color.FromArgb(40, 40, 40);
			listUsers.BackColor = Color.FromArgb(240, 240, 245);
			listUsers.Columns.Add("사용자명", 200);
			listUsers.Columns.Add("역할", 150);
			listUsers.Columns.Add("생성일", 250);
			listUsers.SelectedIndexChanged += ListUsers_SelectedIndexChanged;

			// 입력 필드
			labelUsername = new Label();
			labelUsername.Text = "사용자명:";
			labelUsername.ForeColor = Color.FromArgb(40, 40, 40);
			labelUsername.Location = new Point(16, 310);
			labelUsername.Size = new Size(80, 20);

			textBoxUsername = new TextBox();
			textBoxUsername.Location = new Point(100, 308);
			textBoxUsername.Size = new Size(200, 23);
			textBoxUsername.BackColor = Color.FromArgb(240, 240, 245);
			textBoxUsername.ForeColor = Color.FromArgb(40, 40, 40);

			labelPassword = new Label();
			labelPassword.Text = "비밀번호:";
			labelPassword.ForeColor = Color.FromArgb(40, 40, 40);
			labelPassword.Location = new Point(320, 310);
			labelPassword.Size = new Size(80, 20);

			textBoxPassword = new TextBox();
			textBoxPassword.Location = new Point(404, 308);
			textBoxPassword.Size = new Size(200, 23);
			textBoxPassword.UseSystemPasswordChar = true;
			textBoxPassword.BackColor = Color.FromArgb(240, 240, 245);
			textBoxPassword.ForeColor = Color.FromArgb(40, 40, 40);
			
			// 비밀번호 안내 레이블
			var labelPasswordHint = new Label();
			labelPasswordHint.Text = "(수정 시: 비밀번호를 변경하려면 새 비밀번호 입력)";
			labelPasswordHint.ForeColor = Color.FromArgb(120, 120, 120);
			labelPasswordHint.Location = new Point(320, 335);
			labelPasswordHint.Size = new Size(300, 15);
			labelPasswordHint.Font = new Font("Segoe UI", 8F);

			labelRole = new Label();
			labelRole.Text = "역할:";
			labelRole.ForeColor = Color.FromArgb(40, 40, 40);
			labelRole.Location = new Point(16, 345);
			labelRole.Size = new Size(80, 20);

			comboBoxRole = new ComboBox();
			comboBoxRole.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxRole.Items.AddRange(new object[] { "관리자", "작업자" });
			comboBoxRole.SelectedIndex = 1; // 기본값: 작업자
			comboBoxRole.Location = new Point(100, 343);
			comboBoxRole.Size = new Size(200, 23);
			comboBoxRole.BackColor = Color.FromArgb(240, 240, 245);
			comboBoxRole.ForeColor = Color.FromArgb(40, 40, 40);

			// 버튼
			var buttonPanel = new FlowLayoutPanel();
			buttonPanel.Location = new Point(16, 380);
			buttonPanel.Size = new Size(660, 40);
			buttonPanel.FlowDirection = FlowDirection.LeftToRight;
			buttonPanel.WrapContents = false;

			buttonAdd = CreateButton("추가", Color.FromArgb(100, 120, 130), ButtonAdd_Click);
			buttonUpdate = CreateButton("수정", Color.FromArgb(100, 120, 130), ButtonUpdate_Click);
			buttonDelete = CreateButton("삭제", Color.FromArgb(100, 120, 130), ButtonDelete_Click);
			buttonClose = CreateButton("닫기", Color.FromArgb(100, 120, 130), ButtonClose_Click);

			buttonPanel.Controls.Add(buttonAdd);
			buttonPanel.Controls.Add(buttonUpdate);
			buttonPanel.Controls.Add(buttonDelete);
			buttonPanel.Controls.Add(buttonClose);

			this.Controls.Add(listUsers);
			this.Controls.Add(labelUsername);
			this.Controls.Add(textBoxUsername);
			this.Controls.Add(labelPassword);
			this.Controls.Add(textBoxPassword);
			this.Controls.Add(labelPasswordHint);
			this.Controls.Add(labelRole);
			this.Controls.Add(comboBoxRole);
			this.Controls.Add(buttonPanel);
		}

		private Button CreateButton(string text, Color backColor, EventHandler clickHandler)
		{
			var button = new Button();
			button.Text = text;
			button.Size = new Size(100, 32);
			button.BackColor = backColor;
			button.FlatStyle = FlatStyle.Flat;
			button.FlatAppearance.BorderSize = 0;
			button.ForeColor = Color.White;
			button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			button.Margin = new Padding(5, 5, 5, 5);
			button.Click += clickHandler;
			return button;
		}

		private void LoadUsers()
		{
			users = UserRepository.LoadAll();
			listUsers.Items.Clear();
			foreach (var user in users)
			{
				var item = new ListViewItem(user.Username);
				item.SubItems.Add(user.Role);
				item.SubItems.Add(user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
				item.Tag = user;
				listUsers.Items.Add(item);
			}
			ClearInputs();
		}

		private void ListUsers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listUsers.SelectedItems.Count > 0)
			{
				var user = (User)listUsers.SelectedItems[0].Tag;
				textBoxUsername.Text = user.Username;
				// 비밀번호는 해시된 값이므로 표시하지 않음 (보안상 원래 비밀번호를 복원할 수 없음)
				// 수정 시 비밀번호를 변경하려면 새 비밀번호를 입력해야 함
				textBoxPassword.Text = "";
				comboBoxRole.SelectedItem = user.Role;
				textBoxUsername.Enabled = false; // 수정 시 사용자명 변경 불가
			}
			else
			{
				ClearInputs();
			}
		}

		private void ClearInputs()
		{
			textBoxUsername.Text = "";
			textBoxPassword.Text = "";
			comboBoxRole.SelectedIndex = 1;
			textBoxUsername.Enabled = true;
		}

		private void ButtonAdd_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(textBoxUsername.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text))
			{
				MessageBox.Show("사용자명과 비밀번호를 입력하세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (users.Any(u => u.Username.Equals(textBoxUsername.Text, StringComparison.OrdinalIgnoreCase)))
			{
				MessageBox.Show("이미 존재하는 사용자명입니다.", "중복 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

				var newUser = new User
				{
					Username = textBoxUsername.Text.Trim(),
				Password = textBoxPassword.Text, // BeforeFinal 로직: 평문 비밀번호
					Role = comboBoxRole.SelectedItem?.ToString() ?? "작업자",
					CreatedAt = DateTime.Now
				};

				users.Add(newUser);
					UserRepository.SaveAll(users);
						LoadUsers();
						MessageBox.Show("사용자가 추가되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void ButtonUpdate_Click(object sender, EventArgs e)
		{
			if (listUsers.SelectedItems.Count == 0)
			{
				MessageBox.Show("수정할 사용자를 선택하세요.", "선택 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(textBoxPassword.Text))
			{
				MessageBox.Show("비밀번호를 입력하세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

				var selectedUser = (User)listUsers.SelectedItems[0].Tag;
				var user = users.FirstOrDefault(u => u.Username == selectedUser.Username);
				if (user != null)
				{
				user.Password = textBoxPassword.Text; // BeforeFinal 로직: 평문 비밀번호
					user.Role = comboBoxRole.SelectedItem?.ToString() ?? "작업자";
					UserRepository.SaveAll(users);
						LoadUsers();
						MessageBox.Show("사용자 정보가 수정되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void ButtonDelete_Click(object sender, EventArgs e)
		{
			if (listUsers.SelectedItems.Count == 0)
			{
				MessageBox.Show("삭제할 사용자를 선택하세요.", "선택 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var selectedUser = (User)listUsers.SelectedItems[0].Tag;
			
			if (selectedUser.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))
			{
				MessageBox.Show("관리자 계정은 삭제할 수 없습니다.", "삭제 불가", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var result = MessageBox.Show($"사용자 '{selectedUser.Username}'를 삭제하시겠습니까?", "삭제 확인", 
				MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			
			if (result == DialogResult.Yes)
			{
				users.RemoveAll(u => u.Username == selectedUser.Username);
				UserRepository.SaveAll(users);
				LoadUsers();
				MessageBox.Show("사용자가 삭제되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void ButtonClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}

