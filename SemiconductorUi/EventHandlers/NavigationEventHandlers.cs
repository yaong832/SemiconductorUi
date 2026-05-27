using System;
using System.Windows.Forms;
using SemiconductorUi.Forms;
using SemiconductorUi.Controls;

namespace SemiconductorUi.EventHandlers
{
    /// <summary>
    /// 네비게이션 관련 이벤트 핸들러
    /// </summary>
    public class NavigationEventHandlers : Form1EventHandlersBase
    {
        /// <summary>
        /// NavigationEventHandlers 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public NavigationEventHandlers(Form1 form) : base(form) { }

        /// <summary>
        /// 메인 화면 네비게이션 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonNavMain_Click(object sender, EventArgs e)
        {
            form.UpdateTabButtonStates("Main");
            form.NavigateToSection(Form1.AppSection.Main);
        }

        /// <summary>
        /// 검증 화면 네비게이션 버튼 클릭 이벤트 핸들러
        /// Verification 폼을 열고 인터락 상태를 확인합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonNavVerification_Click(object sender, EventArgs e)
        {
            form.UpdateTabButtonStates("Verification");
            form.NavigateToSection(Form1.AppSection.Verification);
            // Verification 폼 열기
            try
            {
                using (var vf = new VerificationForm())
                {
                    var checks = form.BuildVerificationChecklist();
                    string verReasons;
                    var ok = form.EvaluateInterlocks(out verReasons);
                    vf.SetResults(checks, ok ? "모든 인터락 통과" : $"인터락 미통과: {verReasons}");
                    vf.ShowDialog(form);
                }
            }
            catch (Exception ex)
            {
                form.AddLogMessage($"Verification 폼 오류: {ex.Message}", "ERROR");
            }
        }

        /// <summary>
        /// 이송 화면 네비게이션 버튼 클릭 이벤트 핸들러
        /// Transfer 폼을 열고 이송 큐 상태를 표시합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonNavTransfer_Click(object sender, EventArgs e)
        {
            form.UpdateTabButtonStates("Transfer");
            form.NavigateToSection(Form1.AppSection.Transfer);
            // Transfer 폼 열기
            try
            {
                using (var tfm = new TransferForm())
                {
                    tfm.OnAutoPlan = () => form.AutoPlanTransferIfNeeded();
                    tfm.OnClear = () => form.ClearTransferQueue();
                    tfm.ProvideQueueLines = () => form.GetTransferQueueLines();
                    tfm.RefreshQueue();
                    tfm.ShowDialog(form);
                }
            }
            catch (Exception ex)
            {
                form.AddLogMessage($"Transfer 폼 오류: {ex.Message}", "ERROR");
            }
        }

        /// <summary>
        /// 운전 화면 네비게이션 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonNavOperate_Click(object sender, EventArgs e)
        {
            form.UpdateNavButtonStates("Operate");
            form.NavigateToSection(Form1.AppSection.Operate);
        }

        /// <summary>
        /// 레시피 관리 화면 네비게이션 버튼 클릭 이벤트 핸들러
        /// 관리자 권한이 있는 경우 RecipeManagerForm을 엽니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonNavRecipe_Click(object sender, EventArgs e)
        {
            form.UpdateNavButtonStates("Recipe");
            
            if (!form.EnsureLoggedIn())
            {
                return;
            }

            if (form.CurrentRole != "관리자")
            {
                MessageBox.Show("레시피 직접 수정은 관리자만 접근할 수 있습니다.", "권한 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var f = new RecipeManagerForm())
                {
                    f.OnSavedAll = list => 
                    { 
                        try 
                        { 
                            form.ReloadRecipeCombo(list); 
                        } 
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"레시피 리로드 오류: {ex.Message}");
                        }
                    };
                    f.ShowDialog(form);
                    // 닫힌 뒤에도 한 번 더 로드 보정
                    form.ReloadRecipeCombo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(form, $"Recipe 관리 화면을 여는 중 문제가 발생했습니다.\r\n{ex.Message}\r\n\r\n{ex.GetType().FullName}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 유지보수 화면 네비게이션 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonNavMaint_Click(object sender, EventArgs e)
        {
            form.UpdateNavButtonStates("Maintenance");
            form.NavigateToSection(Form1.AppSection.Maintenance);
            // NavigateToSection에서 Maintenance 폼이 열림 (중복 방지)
        }

        /// <summary>
        /// 장비 제어 버튼 클릭 이벤트 핸들러
        /// EquipmentControlForm을 열고 하드웨어 제어 기능을 제공합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonEquipmentControl_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }

            try
            {
                using (var ecf = new EquipmentControlForm(form.EtherCAT_M, form.EthercatConnected))
                {
                    // 도어 상태 변경 시 UI 업데이트 콜백
                    ecf.OnDoorStateChanged = (region, isOpen) =>
                    {
                        if (form.InvokeRequired)
                        {
                            form.Invoke(new Action(() =>
                            {
                                form.ViewModel?.SetDoorState(region, isOpen);
                                form.ApplyDoorVisualsForRegion(region, animate: true);
                            }));
                        }
                        else
                        {
                            form.ViewModel?.SetDoorState(region, isOpen);
                            form.ApplyDoorVisualsForRegion(region, animate: true);
                        }
                    };
                    
                    // Chamber 램프 상태 변경 시 UI 업데이트 콜백
                    ecf.OnChamberLampStateChanged = (region, isOn) =>
                    {
                        if (form.InvokeRequired)
                        {
                            form.Invoke(new Action(() =>
                            {
                                form.SetChamberLampUIOnly(region, isOn);
                            }));
                        }
                        else
                        {
                            form.SetChamberLampUIOnly(region, isOn);
                        }
                    };
                    
                    // 3색 램프 상태 변경 시 UI 업데이트 콜백
                    ecf.OnMainLampStateChanged = (red, yellow, green) =>
                    {
                        if (form.InvokeRequired)
                        {
                            form.Invoke(new Action(() =>
                            {
                                form.UpdateMainLampColorsUIOnly(green ? System.Drawing.Color.ForestGreen : (System.Drawing.Color?)null, yellow, red);
                                // 중앙 UI (EquipmentDiagramControl) 램프 상태 업데이트
                                form.UpdateEquipmentDiagramLampState(red, yellow, green);
                            }));
                        }
                        else
                        {
                            form.UpdateMainLampColorsUIOnly(green ? System.Drawing.Color.ForestGreen : (System.Drawing.Color?)null, yellow, red);
                            // 중앙 UI (EquipmentDiagramControl) 램프 상태 업데이트
                            form.UpdateEquipmentDiagramLampState(red, yellow, green);
                        }
                    };
                    
                    // 서보 상태 변경 시 UI 업데이트 콜백
                    ecf.OnServoStateChanged = (servoIsOn) =>
                    {
                        if (form.InvokeRequired)
                        {
                            form.Invoke(new Action(() =>
                            {
                                form.IsServoOn = servoIsOn;  // 서보 상태 플래그 업데이트
                                if (!servoIsOn)
                                {
                                    form.TmHardwareInitialized = false;  // 서보 OFF 시 원점복귀 상태 초기화
                                }
                                form.UpdateServoStatusLabel();
                            }));
                        }
                        else
                        {
                            form.IsServoOn = servoIsOn;  // 서보 상태 플래그 업데이트
                            if (!servoIsOn)
                            {
                                form.TmHardwareInitialized = false;  // 서보 OFF 시 원점복귀 상태 초기화
                            }
                            form.UpdateServoStatusLabel();
                        }
                    };
                    
                    // 원점복귀 요청 시 UI 업데이트 콜백
                    ecf.OnHomingRequested = () =>
                    {
                        if (form.InvokeRequired)
                        {
                            form.Invoke(new Action(() =>
                            {
                                form.TmHardwareInitialized = true;
                                form.UpdateServoStatusLabel();
                            }));
                        }
                        else
                        {
                            form.TmHardwareInitialized = true;
                            form.UpdateServoStatusLabel();
                        }
                    };
                    
                    // 실린더 상태 변경 시 UI 업데이트 콜백 (필요 시 구현)
                    ecf.OnCylinderStateChanged = (isExtended) =>
                    {
                        // 실린더 상태는 현재 UI에 직접 표시되지 않으므로 로그만 기록
                        form.AddLogMessage($"실린더 상태 변경: {(isExtended ? "전진" : "후진")}", "INFO");
                    };
                    
                    // 진공 상태 변경 시 UI 업데이트 콜백 (필요 시 구현)
                    ecf.OnVacuumStateChanged = (isOn) =>
                    {
                        // 진공 상태는 현재 UI에 직접 표시되지 않으므로 로그만 기록
                        form.AddLogMessage($"진공 상태 변경: {(isOn ? "ON" : "OFF")}", "INFO");
                    };

                    ecf.ShowDialog(form);
                }
            }
            catch (Exception ex)
            {
                form.AddLogMessage($"Equipment Control 폼 오류: {ex.Message}", "ERROR");
                MessageBox.Show(form, $"Equipment Control 폼을 여는 중 오류가 발생했습니다.\r\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 설정 화면 네비게이션 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonNavConfig_Click(object sender, EventArgs e)
        {
            form.UpdateNavButtonStates("Config");
            form.NavigateToSection(Form1.AppSection.Config);
            // NavigateToSection에서 Config 폼이 열림 (권한 체크 및 콜백 포함, 중복 방지)
        }

        /// <summary>
        /// 트렌드 화면 네비게이션 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonNavTrend_Click(object sender, EventArgs e)
        {
            form.UpdateNavButtonStates("Trend");
            form.NavigateToSection(Form1.AppSection.Trend);
            // NavigateToSection에서 Trend 폼이 열림 (중복 방지)
        }

        /// <summary>
        /// 리포트 화면 네비게이션 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonNavReport_Click(object sender, EventArgs e)
        {
            form.UpdateNavButtonStates("Report");
            form.NavigateToSection(Form1.AppSection.Report);
            // NavigateToSection에서 Report 폼이 열림 (콜백 포함, 중복 방지)
        }

        /// <summary>
        /// 시스템 정보 화면 네비게이션 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonNavSysInfo_Click(object sender, EventArgs e)
        {
            form.UpdateNavButtonStates("System");
            form.NavigateToSection(Form1.AppSection.SysInfo);
            form.ShowSystemInfoSummary();
        }

        /// <summary>
        /// 메인 탭 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonTabMain_Click(object sender, EventArgs e)
        {
            form.UpdateTabButtonStates("Main");
            form.NavigateToSection(Form1.AppSection.Main);
        }

        /// <summary>
        /// 검증 탭 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonTabVerification_Click(object sender, EventArgs e)
        {
            form.UpdateTabButtonStates("Verification");
            form.NavigateToSection(Form1.AppSection.Verification);
        }

        /// <summary>
        /// 이송 탭 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonTabTransfer_Click(object sender, EventArgs e)
        {
            form.UpdateTabButtonStates("Transfer");
            form.NavigateToSection(Form1.AppSection.Transfer);
        }
    }
}

