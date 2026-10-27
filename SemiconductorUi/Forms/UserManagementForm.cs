using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.Helpers;
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
			this.BackColor = UiTheme.Surface;
			this.ForeColor = UiTheme.TextPrimary;
			this.Font = new Font("Segoe UI", 9.5F);
			this.ClientSize = new Size(700, 540);

			var labelTitle = new Label();
			labelTitle.Text = "사용자 관리";
			labelTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
			labelTitle.ForeColor = UiTheme.TextPrimary;
			labelTitle.AutoSize = true;
			labelTitle.Location = new Point(20, 16);

			// 사용자 목록
			listUsers = new ListView();
			listUsers.Location = new Point(20, 52);
			listUsers.Size = new Size(660, 250);
			listUsers.View = View.Details;
			listUsers.FullRowSelect = true;
			listUsers.GridLines = false;
			listUsers.HideSelection = false;
			listUsers.BorderStyle = BorderStyle.FixedSingle;
			listUsers.ForeColor = UiTheme.TextPrimary;
			listUsers.BackColor = UiTheme.Surface;
			listUsers.Columns.Add("사용자명", 200);
			listUsers.Columns.Add("역할", 150);
			listUsers.Columns.Add("생성일", 250);
			listUsers.SelectedIndexChanged += ListUsers_SelectedIndexChanged;

			// 입력 필드: 연회색 판 안에 둡니다
			var plate = new GroupBox();
			plate.Text = "사용자 정보";
			plate.Location = new Point(20, 316);
			plate.Size = new Size(660, 128);
			plate.Padding = new Padding(12, 3, 12, 3);
			LampButtonPainter.AttachPlate(plate);

			labelUsername = NewFieldLabel("사용자명", new Point(14, 40));

			textBoxUsername = new TextBox();
			textBoxUsername.Location = new Point(96, 36);
			textBoxUsername.Size = new Size(210, 25);
			textBoxUsername.BorderStyle = BorderStyle.FixedSingle;
			textBoxUsername.BackColor = UiTheme.Surface;
			textBoxUsername.ForeColor = UiTheme.TextPrimary;

			labelPassword = NewFieldLabel("비밀번호", new Point(336, 40));

			textBoxPassword = new TextBox();
			textBoxPassword.Location = new Point(418, 36);
			textBoxPassword.Size = new Size(226, 25);
			textBoxPassword.UseSystemPasswordChar = true;
			textBoxPassword.BorderStyle = BorderStyle.FixedSingle;
			textBoxPassword.BackColor = UiTheme.Surface;
			textBoxPassword.ForeColor = UiTheme.TextPrimary;
			
			// 비밀번호 안내 레이블
			var labelPasswordHint = new Label();
			labelPasswordHint.Text = "수정할 때는 비밀번호를 바꿀 때만 입력합니다.";
			labelPasswordHint.ForeColor = UiTheme.TextSecondary;
			labelPasswordHint.BackColor = LampButtonPainter.PlateBack;
			labelPasswordHint.Location = new Point(336, 82);
			labelPasswordHint.Size = new Size(310, 34);
			labelPasswordHint.Font = new Font("Segoe UI", 8.5F);

			labelRole = NewFieldLabel("역할", new Point(14, 82));

			comboBoxRole = new ComboBox();
			comboBoxRole.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxRole.Items.AddRange(new object[] { "관리자", "작업자" });
			comboBoxRole.SelectedIndex = 1; // 기본값: 작업자
			comboBoxRole.Location = new Point(96, 78);
			comboBoxRole.Size = new Size(210, 25);
			comboBoxRole.BackColor = UiTheme.Surface;
			comboBoxRole.ForeColor = UiTheme.TextPrimary;

			plate.Controls.Add(labelUsername);
			plate.Controls.Add(textBoxUsername);
			plate.Controls.Add(labelPassword);
			plate.Controls.Add(textBoxPassword);
			plate.Controls.Add(labelPasswordHint);
			plate.Controls.Add(labelRole);
			plate.Controls.Add(comboBoxRole);

			// 버튼: 왼쪽 추가/수정/삭제, 오른쪽 닫기
			var buttonBar = new Panel();
			buttonBar.Dock = DockStyle.Bottom;
			buttonBar.Height = 72;
			buttonBar.BackColor = UiTheme.Surface;
			buttonBar.Paint += (s, e) =>
			{
				using (var pen = new Pen(UiTheme.Border))
				{
					e.Graphics.DrawLine(pen, 0, 0, buttonBar.Width, 0);
				}
			};

			buttonAdd = CreateButton("추가", Color.FromArgb(100, 120, 130), ButtonAdd_Click);
			buttonUpdate = CreateButton("수정", Color.FromArgb(100, 120, 130), ButtonUpdate_Click);
			buttonDelete = CreateButton("삭제", Color.FromArgb(100, 120, 130), ButtonDelete_Click);
			buttonClose = CreateButton("닫기", Color.FromArgb(100, 120, 130), ButtonClose_Click);
			LampButtonPainter.AttachPlain(buttonAdd, primary: true);
			LampButtonPainter.AttachPlain(buttonUpdate);
			LampButtonPainter.AttachPlain(buttonDelete);
			LampButtonPainter.AttachPlain(buttonClose);

			buttonAdd.Location = new Point(20, 14);
			buttonUpdate.Location = new Point(buttonAdd.Right + 10, 14);
			buttonDelete.Location = new Point(buttonUpdate.Right + 10, 14);
			buttonClose.Location = new Point(680 - buttonClose.Width, 14);

			buttonBar.Controls.Add(buttonAdd);
			buttonBar.Controls.Add(buttonUpdate);
			buttonBar.Controls.Add(buttonDelete);
			buttonBar.Controls.Add(buttonClose);

			this.Controls.Add(labelTitle);
			this.Controls.Add(listUsers);
			this.Controls.Add(plate);
			this.Controls.Add(buttonBar);
		}

		private static Label NewFieldLabel(string text, Point location)
		{
			var label = new Label();
			label.Text = text;
			label.ForeColor = UiTheme.TextSecondary;
			label.BackColor = LampButtonPainter.PlateBack;
			label.AutoSize = true;
			label.Location = location;
			return label;
		}

		private Button CreateButton(string text, Color backColor, EventHandler clickHandler)
		{
			var button = new Button();
			button.Text = text;
			button.Size = new Size(110, 44);
			button.BackColor = backColor;
			button.FlatStyle = FlatStyle.Flat;
			button.FlatAppearance.BorderSize = 0;
			button.ForeColor = Color.White;
			button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
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
					Password = PasswordHelper.HashPassword(textBoxPassword.Text),
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
				user.Password = PasswordHelper.HashPassword(textBoxPassword.Text);
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

