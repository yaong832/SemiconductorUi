using System;
using System.Windows.Forms;
using SemiconductorUi.Forms;
using SemiconductorUi.Repositories;
using SemiconductorUi.ViewModels;

namespace SemiconductorUi.EventHandlers
{
    /// <summary>
    /// 로그인 관련 이벤트 핸들러
    /// </summary>
    public class LoginEventHandlers : Form1EventHandlersBase
    {
        /// <summary>
        /// LoginEventHandlers 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public LoginEventHandlers(Form1 form) : base(form) { }

        /// <summary>
        /// 로그인 버튼 클릭 이벤트 핸들러
        /// 로그인 폼을 표시하고 사용자 인증을 수행합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonLogin_Click(object sender, EventArgs e)
        {
            while (true)
            {
                using (var loginForm = new LoginForm())
                {
                    var dialogResult = loginForm.ShowDialog(form);
                    if (dialogResult != DialogResult.OK)
                    {
                        return;
                    }

                    var username = loginForm.EnteredUsername;
                    var password = loginForm.EnteredPassword;

                    if (form.IsCredentialValid(username, password))
                    {
                        var user = UserRepository.FindByUsername(username);
                        form.IsLoggedIn = true;
                        form.CurrentUser = username;
                        form.CurrentRole = user?.Role ?? "작업자";
                        form.ApplyLoginState();
                        return;
                    }

                    MessageBox.Show("아이디 또는 비밀번호가 올바르지 않습니다.", "로그인 실패",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// 사용자 관리 버튼 클릭 이벤트 핸들러
        /// 관리자 권한이 있는 경우 사용자 관리 폼을 엽니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonUserManagement_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }

            if (form.CurrentRole != "관리자")
            {
                MessageBox.Show("사용자 관리는 관리자만 접근할 수 있습니다.", "권한 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var userManagementForm = new UserManagementForm())
                {
                    userManagementForm.ShowDialog(form);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"사용자 관리 폼을 여는 중 오류가 발생했습니다.\r\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                form.AddLogMessage($"사용자 관리 폼 오류: {ex.Message}", "ERROR");
            }
        }

        /// <summary>
        /// 로그아웃 버튼 클릭 이벤트 핸들러
        /// 현재 사용자를 로그아웃하고 공정을 대기 상태로 전환합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonLogout_Click(object sender, EventArgs e)
        {
            if (!form.IsLoggedIn)
            {
                return;
            }

            // 공정이 도는 중(일시정지·알람 대기 포함)이나 서보 ON/원점복귀 중에는 로그아웃을 막습니다.
            // 로그아웃하면 일시정지·시작 버튼이 잠기는데 TM은 계속 움직이기 때문입니다.
            if (form.IsLogoutBlockedByProcess())
            {
                form.AddLogMessage($"공정 진행 중 로그아웃 시도 차단 (사용자: {form.CurrentUser})", "WARN");
                MessageBox.Show(
                    "공정이 진행 중(일시정지 포함)이거나 서보 ON·원점복귀 중에는 로그아웃할 수 없습니다.\n\n" +
                    "공정이 끝나거나 '공정 초기화' 후에 로그아웃해 주세요.\n" +
                    "급히 멈춰야 하면 긴급 정지를 누르세요.",
                    "로그아웃 제한",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            form.IsLoggedIn = false;
            form.CurrentUser = "Guest";
            form.CurrentRole = "없음";
            form.SetProcessState(MainFormViewModel.ProcessState.Idle, "로그아웃으로 인해 공정을 대기 상태로 전환했습니다.");
            form.ApplyLoginState();
        }
    }
}

