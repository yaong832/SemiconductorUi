namespace SemiconductorUi
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutRoot = new System.Windows.Forms.TableLayoutPanel();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.tableLayoutHeader = new System.Windows.Forms.TableLayoutPanel();
            this.flowHeaderLogin = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLoginTopRow = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.buttonEthercatConnect = new System.Windows.Forms.Button();
            this.buttonEthercatDisconnect = new System.Windows.Forms.Button();
            this.buttonServoOn = new System.Windows.Forms.Button();
            this.buttonServoOff = new System.Windows.Forms.Button();
            this.flowLoginBottomRow = new System.Windows.Forms.FlowLayoutPanel();
            this.labelLoginStatus = new System.Windows.Forms.Label();
            this.labelEthercatStatus = new System.Windows.Forms.Label();
            this.labelServoStatus = new System.Windows.Forms.Label();
            this.panelHeaderStatusSummary = new System.Windows.Forms.Panel();
            this.labelHeaderStatusTitle = new System.Windows.Forms.Label();
            this.tableHeaderStatus = new System.Windows.Forms.TableLayoutPanel();
            this.labelHeaderLotTitle = new System.Windows.Forms.Label();
            this.labelHeaderFoupATitle = new System.Windows.Forms.Label();
            this.labelHeaderFoupBTitle = new System.Windows.Forms.Label();
            this.labelHeaderPM1Title = new System.Windows.Forms.Label();
            this.labelHeaderPM2Title = new System.Windows.Forms.Label();
            this.labelHeaderPM3Title = new System.Windows.Forms.Label();
            this.labelHeaderTMTitle = new System.Windows.Forms.Label();
            this.labelHeaderAlarmTitle = new System.Windows.Forms.Label();
            this.labelHeaderLotStatus = new System.Windows.Forms.Label();
            this.labelHeaderFoupAStatus = new System.Windows.Forms.Label();
            this.labelHeaderFoupBStatus = new System.Windows.Forms.Label();
            this.labelHeaderPM1Status = new System.Windows.Forms.Label();
            this.labelHeaderPM2Status = new System.Windows.Forms.Label();
            this.labelHeaderPM3Status = new System.Windows.Forms.Label();
            this.labelHeaderTMStatus = new System.Windows.Forms.Label();
            this.labelHeaderAlarmStatus = new System.Windows.Forms.Label();
            this.flowHeaderStatus = new System.Windows.Forms.FlowLayoutPanel();
            this.panelHeaderCardTM = new System.Windows.Forms.Panel();
            this.labelHeaderCardTMStatus = new System.Windows.Forms.Label();
            this.labelHeaderCardTMTitle = new System.Windows.Forms.Label();
            this.panelHeaderCardPMA = new System.Windows.Forms.Panel();
            this.labelHeaderCardPMAStatus = new System.Windows.Forms.Label();
            this.labelHeaderCardPMATitle = new System.Windows.Forms.Label();
            this.panelHeaderCardPMB = new System.Windows.Forms.Panel();
            this.labelHeaderCardPMBStatus = new System.Windows.Forms.Label();
            this.labelHeaderCardPMBTitle = new System.Windows.Forms.Label();
            this.panelHeaderCardPMC = new System.Windows.Forms.Panel();
            this.labelHeaderCardPMCStatus = new System.Windows.Forms.Label();
            this.labelHeaderCardPMCTitle = new System.Windows.Forms.Label();
            this.panelHeaderAlarm = new System.Windows.Forms.TableLayoutPanel();
            this.flowAlarmIndicator = new System.Windows.Forms.FlowLayoutPanel();
            this.panelHeaderMessageAccent = new System.Windows.Forms.Panel();
            this.tableHeaderMessageText = new System.Windows.Forms.TableLayoutPanel();
            this.labelHeaderEventTitle = new System.Windows.Forms.Label();
            this.labelHeaderEventLevel = new System.Windows.Forms.Label();
            this.labelHeaderEventMessage = new System.Windows.Forms.Label();
            this.flowHeaderTabs = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonTabMain = new System.Windows.Forms.Button();
            this.buttonTabVerification = new System.Windows.Forms.Button();
            this.buttonTabTransfer = new System.Windows.Forms.Button();
            this.flowHeaderTimeAndUser = new System.Windows.Forms.FlowLayoutPanel();
            this.labelHeaderCurrentTime = new System.Windows.Forms.Label();
            this.buttonUserManagement = new System.Windows.Forms.Button();
            this.tableLayoutProcessArea = new System.Windows.Forms.TableLayoutPanel();
            this.panelMainProcess = new System.Windows.Forms.Panel();
            this.tableLayoutMainProcess = new System.Windows.Forms.TableLayoutPanel();
            this.labelMainProcessTitle = new System.Windows.Forms.Label();
            this.tableLayoutMainContent = new System.Windows.Forms.TableLayoutPanel();
            this.panelEquipment = new System.Windows.Forms.Panel();
            this.tableLayoutEquipmentArea = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutEquipment = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutChamberCluster = new System.Windows.Forms.TableLayoutPanel();
            this.panelEquipmentCanvas = new System.Windows.Forms.Panel();
            this.panelPmStatus = new System.Windows.Forms.Panel();
            this.tableLayoutPmStatus = new System.Windows.Forms.TableLayoutPanel();
            this.panelSummaryPMA = new System.Windows.Forms.Panel();
            this.panelSummaryPMB = new System.Windows.Forms.Panel();
            this.panelSummaryPMC = new System.Windows.Forms.Panel();
            this.labelPmStatusTitle = new System.Windows.Forms.Label();
            this.tableProcessMetrics = new System.Windows.Forms.TableLayoutPanel();
            this.panelFoupStatusA = new System.Windows.Forms.Panel();
            this.tableFoupACard = new System.Windows.Forms.TableLayoutPanel();
            this.panelFoupALevelContainer = new System.Windows.Forms.Panel();
            this.panelFoupALevelTrack = new System.Windows.Forms.Panel();
            this.panelFoupALevelFill = new System.Windows.Forms.Panel();
            this.panelFoupADetail = new System.Windows.Forms.Panel();
            this.tableFoupAInfo = new System.Windows.Forms.TableLayoutPanel();
            this.labelFoupAFieldPath = new System.Windows.Forms.Label();
            this.labelFoupAPathValue = new System.Windows.Forms.Label();
            this.labelFoupAFieldPPID = new System.Windows.Forms.Label();
            this.labelFoupAPPIDValue = new System.Windows.Forms.Label();
            this.labelFoupAFieldLotId = new System.Windows.Forms.Label();
            this.labelFoupALotIdValue = new System.Windows.Forms.Label();
            this.labelFoupAFieldMid = new System.Windows.Forms.Label();
            this.labelFoupAMidValue = new System.Windows.Forms.Label();
            this.labelFoupAFieldLock = new System.Windows.Forms.Label();
            this.labelFoupALockValue = new System.Windows.Forms.Label();
            this.labelFoupAStatusHeadline = new System.Windows.Forms.Label();
            this.labelFoupInfoATitle = new System.Windows.Forms.Label();
            this.panelFoupStatusB = new System.Windows.Forms.Panel();
            this.tableFoupBCard = new System.Windows.Forms.TableLayoutPanel();
            this.panelFoupBLevelContainer = new System.Windows.Forms.Panel();
            this.panelFoupBLevelTrack = new System.Windows.Forms.Panel();
            this.panelFoupBLevelFill = new System.Windows.Forms.Panel();
            this.panelFoupBDetail = new System.Windows.Forms.Panel();
            this.tableFoupBInfo = new System.Windows.Forms.TableLayoutPanel();
            this.labelFoupBFieldPath = new System.Windows.Forms.Label();
            this.labelFoupBPathValue = new System.Windows.Forms.Label();
            this.labelFoupBFieldPPID = new System.Windows.Forms.Label();
            this.labelFoupBPPIDValue = new System.Windows.Forms.Label();
            this.labelFoupBFieldLotId = new System.Windows.Forms.Label();
            this.labelFoupBLotIdValue = new System.Windows.Forms.Label();
            this.labelFoupBFieldMid = new System.Windows.Forms.Label();
            this.labelFoupBMidValue = new System.Windows.Forms.Label();
            this.labelFoupBFieldLock = new System.Windows.Forms.Label();
            this.labelFoupBLockValue = new System.Windows.Forms.Label();
            this.labelFoupBStatusHeadline = new System.Windows.Forms.Label();
            this.labelFoupInfoBTitle = new System.Windows.Forms.Label();
            this.labelFoupSummaryInfo = new System.Windows.Forms.Label();
            this.panelControlPanel = new System.Windows.Forms.Panel();
            this.flowControlPanelStack = new System.Windows.Forms.FlowLayoutPanel();
            this.labelControlTitle = new System.Windows.Forms.Label();
            this.groupBoxControlButtons = new System.Windows.Forms.GroupBox();
            this.flowLayoutControlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonResetAlarm = new System.Windows.Forms.Button();
            this.buttonResetProcess = new System.Windows.Forms.Button();
            this.buttonEquipmentControl = new System.Windows.Forms.Button();
            this.groupBoxFoupReady = new System.Windows.Forms.GroupBox();
            this.flowLayoutFoupReadyButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonToggleFoupMount = new System.Windows.Forms.Button();
            this.buttonWaferLoading = new System.Windows.Forms.Button();
            this.buttonWaferUnloading = new System.Windows.Forms.Button();
            this.groupBoxRecipe = new System.Windows.Forms.GroupBox();
            this.buttonApplyRecipe = new System.Windows.Forms.Button();
            this.comboRecipeSelect = new System.Windows.Forms.ComboBox();
            this.labelRecipe = new System.Windows.Forms.Label();
            this.panelAlarmArea = new System.Windows.Forms.Panel();
            this.flowBottomNavigation = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonNavOperate = new System.Windows.Forms.Button();
            this.buttonNavRecipe = new System.Windows.Forms.Button();
            this.buttonNavMaintenance = new System.Windows.Forms.Button();
            this.buttonNavConfig = new System.Windows.Forms.Button();
            this.buttonNavTrend = new System.Windows.Forms.Button();
            this.buttonNavReport = new System.Windows.Forms.Button();
            this.buttonNavSystem = new System.Windows.Forms.Button();
            this.buttonServoHome = new System.Windows.Forms.Button();
            this.panelMainLamp = new System.Windows.Forms.Panel();
            this.labelMainLampRed = new System.Windows.Forms.Label();
            this.labelMainLampYellow = new System.Windows.Forms.Label();
            this.labelMainLampGreen = new System.Windows.Forms.Label();
            this.panelMainLampRed = new System.Windows.Forms.Panel();
            this.panelMainLampYellow = new System.Windows.Forms.Panel();
            this.panelMainLampGreen = new System.Windows.Forms.Panel();
            this.labelMainLamp = new System.Windows.Forms.Label();
            this.panelFoupB = new System.Windows.Forms.Panel();
            this.panelFoupA = new System.Windows.Forms.Panel();
            this.panelChamberC = new System.Windows.Forms.Panel();
            this.panelDoorChamberC = new System.Windows.Forms.Panel();
            this.panelWaferChamberC = new System.Windows.Forms.Panel();
            this.panelLampChamberC = new System.Windows.Forms.Panel();
            this.labelChamberC = new System.Windows.Forms.Label();
            this.panelChamberA = new System.Windows.Forms.Panel();
            this.panelDoorChamberA = new System.Windows.Forms.Panel();
            this.panelWaferChamberA = new System.Windows.Forms.Panel();
            this.panelLampChamberA = new System.Windows.Forms.Panel();
            this.labelChamberA = new System.Windows.Forms.Label();
            this.panelChamberB = new System.Windows.Forms.Panel();
            this.panelDoorChamberB = new System.Windows.Forms.Panel();
            this.panelWaferChamberB = new System.Windows.Forms.Panel();
            this.panelLampChamberB = new System.Windows.Forms.Panel();
            this.labelChamberB = new System.Windows.Forms.Label();
            this.flowProcessSummary = new System.Windows.Forms.FlowLayoutPanel();
            this.panelSummaryFoupA = new System.Windows.Forms.Panel();
            this.labelSummaryFoupAStatus = new System.Windows.Forms.Label();
            this.labelSummaryFoupATitle = new System.Windows.Forms.Label();
            this.panelSummaryFoupB = new System.Windows.Forms.Panel();
            this.labelSummaryFoupBStatus = new System.Windows.Forms.Label();
            this.labelSummaryFoupBTitle = new System.Windows.Forms.Label();
            this.panelSummaryProcess = new System.Windows.Forms.Panel();
            this.labelProcessValue = new System.Windows.Forms.Label();
            this.labelSummaryProcessTitle = new System.Windows.Forms.Label();
            this.panelSummaryPressure = new System.Windows.Forms.Panel();
            this.labelPressureValue = new System.Windows.Forms.Label();
            this.labelSummaryPressureTitle = new System.Windows.Forms.Label();
            this.panelSummaryTemperature = new System.Windows.Forms.Panel();
            this.labelTemperatureValue = new System.Windows.Forms.Label();
            this.labelSummaryTemperatureTitle = new System.Windows.Forms.Label();
            this.panelSummaryDoor = new System.Windows.Forms.Panel();
            this.labelDoorValue = new System.Windows.Forms.Label();
            this.labelSummaryDoorTitle = new System.Windows.Forms.Label();
            this.panelSummaryTM = new System.Windows.Forms.Panel();
            this.labelSummaryTMStatus = new System.Windows.Forms.Label();
            this.labelSummaryTMTitle = new System.Windows.Forms.Label();
            this.panelStatusDoorProcess = new System.Windows.Forms.Panel();
            this.panelStatusLampProcess = new System.Windows.Forms.Panel();
            this.panelStatusDoorPressure = new System.Windows.Forms.Panel();
            this.panelStatusLampPressure = new System.Windows.Forms.Panel();
            this.panelStatusDoorTemperature = new System.Windows.Forms.Panel();
            this.panelStatusLampTemperature = new System.Windows.Forms.Panel();
            this.panelStatusDoorOverall = new System.Windows.Forms.Panel();
            this.panelStatusLampDoor = new System.Windows.Forms.Panel();
            this.panelStatusMonitoring = new System.Windows.Forms.Panel();
            this.tableStatusIndicators = new System.Windows.Forms.TableLayoutPanel();
            this.labelStatusTitle = new System.Windows.Forms.Label();
            this.tableLayoutRoot.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.tableLayoutHeader.SuspendLayout();
            this.flowHeaderLogin.SuspendLayout();
            this.flowLoginTopRow.SuspendLayout();
            this.flowLoginBottomRow.SuspendLayout();
            this.panelHeaderStatusSummary.SuspendLayout();
            this.tableHeaderStatus.SuspendLayout();
            this.flowHeaderStatus.SuspendLayout();
            this.panelHeaderCardTM.SuspendLayout();
            this.panelHeaderCardPMA.SuspendLayout();
            this.panelHeaderCardPMB.SuspendLayout();
            this.panelHeaderCardPMC.SuspendLayout();
            this.panelHeaderAlarm.SuspendLayout();
            this.flowAlarmIndicator.SuspendLayout();
            this.tableHeaderMessageText.SuspendLayout();
            this.flowHeaderTabs.SuspendLayout();
            this.flowHeaderTimeAndUser.SuspendLayout();
            this.tableLayoutProcessArea.SuspendLayout();
            this.panelMainProcess.SuspendLayout();
            this.tableLayoutMainProcess.SuspendLayout();
            this.tableLayoutMainContent.SuspendLayout();
            this.panelEquipment.SuspendLayout();
            this.tableLayoutEquipmentArea.SuspendLayout();
            this.tableLayoutEquipment.SuspendLayout();
            this.tableLayoutChamberCluster.SuspendLayout();
            this.panelPmStatus.SuspendLayout();
            this.tableLayoutPmStatus.SuspendLayout();
            this.tableProcessMetrics.SuspendLayout();
            this.panelFoupStatusA.SuspendLayout();
            this.tableFoupACard.SuspendLayout();
            this.panelFoupALevelContainer.SuspendLayout();
            this.panelFoupALevelTrack.SuspendLayout();
            this.panelFoupADetail.SuspendLayout();
            this.tableFoupAInfo.SuspendLayout();
            this.panelFoupStatusB.SuspendLayout();
            this.tableFoupBCard.SuspendLayout();
            this.panelFoupBLevelContainer.SuspendLayout();
            this.panelFoupBLevelTrack.SuspendLayout();
            this.panelFoupBDetail.SuspendLayout();
            this.tableFoupBInfo.SuspendLayout();
            this.panelControlPanel.SuspendLayout();
            this.flowControlPanelStack.SuspendLayout();
            this.groupBoxControlButtons.SuspendLayout();
            this.flowLayoutControlButtons.SuspendLayout();
            this.groupBoxFoupReady.SuspendLayout();
            this.flowLayoutFoupReadyButtons.SuspendLayout();
            this.groupBoxRecipe.SuspendLayout();
            this.panelAlarmArea.SuspendLayout();
            this.flowBottomNavigation.SuspendLayout();
            this.panelMainLamp.SuspendLayout();
            this.panelChamberC.SuspendLayout();
            this.panelDoorChamberC.SuspendLayout();
            this.panelChamberA.SuspendLayout();
            this.panelDoorChamberA.SuspendLayout();
            this.panelChamberB.SuspendLayout();
            this.panelDoorChamberB.SuspendLayout();
            this.flowProcessSummary.SuspendLayout();
            this.panelSummaryFoupA.SuspendLayout();
            this.panelSummaryFoupB.SuspendLayout();
            this.panelSummaryProcess.SuspendLayout();
            this.panelSummaryPressure.SuspendLayout();
            this.panelSummaryTemperature.SuspendLayout();
            this.panelSummaryDoor.SuspendLayout();
            this.panelSummaryTM.SuspendLayout();
            this.panelStatusMonitoring.SuspendLayout();
            this.tableStatusIndicators.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutRoot
            // 
            this.tableLayoutRoot.ColumnCount = 1;
            this.tableLayoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutRoot.Controls.Add(this.panelHeader, 0, 0);
            this.tableLayoutRoot.Controls.Add(this.tableLayoutProcessArea, 0, 1);
            this.tableLayoutRoot.Controls.Add(this.panelAlarmArea, 0, 2);
            this.tableLayoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutRoot.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutRoot.Name = "tableLayoutRoot";
            this.tableLayoutRoot.RowCount = 3;
            this.tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutRoot.Size = new System.Drawing.Size(1701, 976);
            this.tableLayoutRoot.TabIndex = 0;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(200)))));
            this.panelHeader.Controls.Add(this.tableLayoutHeader);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHeader.Location = new System.Drawing.Point(3, 3);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.panelHeader.Size = new System.Drawing.Size(1695, 164);
            this.panelHeader.TabIndex = 0;
            // 
            // tableLayoutHeader
            // 
            this.tableLayoutHeader.ColumnCount = 3;
            this.tableLayoutHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.tableLayoutHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54F));
            this.tableLayoutHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tableLayoutHeader.Controls.Add(this.flowHeaderLogin, 0, 0);
            this.tableLayoutHeader.Controls.Add(this.panelHeaderStatusSummary, 1, 0);
            this.tableLayoutHeader.Controls.Add(this.panelHeaderAlarm, 2, 0);
            this.tableLayoutHeader.Controls.Add(this.flowHeaderTabs, 0, 1);
            this.tableLayoutHeader.Controls.Add(this.flowHeaderTimeAndUser, 2, 1);
            this.tableLayoutHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutHeader.Location = new System.Drawing.Point(20, 15);
            this.tableLayoutHeader.Name = "tableLayoutHeader";
            this.tableLayoutHeader.RowCount = 2;
            this.tableLayoutHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tableLayoutHeader.Size = new System.Drawing.Size(1655, 134);
            this.tableLayoutHeader.TabIndex = 0;
            // 
            // flowHeaderLogin
            // 
            this.flowHeaderLogin.AutoSize = true;
            this.flowHeaderLogin.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowHeaderLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(190)))));
            this.flowHeaderLogin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowHeaderLogin.Controls.Add(this.flowLoginTopRow);
            this.flowHeaderLogin.Controls.Add(this.flowLoginBottomRow);
            this.flowHeaderLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowHeaderLogin.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowHeaderLogin.Location = new System.Drawing.Point(0, 0);
            this.flowHeaderLogin.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.flowHeaderLogin.Name = "flowHeaderLogin";
            this.flowHeaderLogin.Padding = new System.Windows.Forms.Padding(16, 8, 16, 10);
            this.flowHeaderLogin.Size = new System.Drawing.Size(463, 88);
            this.flowHeaderLogin.TabIndex = 0;
            this.flowHeaderLogin.WrapContents = false;
            // 
            // flowLoginTopRow
            // 
            this.flowLoginTopRow.AutoSize = true;
            this.flowLoginTopRow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLoginTopRow.Controls.Add(this.buttonLogin);
            this.flowLoginTopRow.Controls.Add(this.buttonLogout);
            this.flowLoginTopRow.Controls.Add(this.buttonEthercatConnect);
            this.flowLoginTopRow.Controls.Add(this.buttonEthercatDisconnect);
            this.flowLoginTopRow.Controls.Add(this.buttonServoOn);
            this.flowLoginTopRow.Controls.Add(this.buttonServoOff);
            this.flowLoginTopRow.Location = new System.Drawing.Point(16, 10);
            this.flowLoginTopRow.Margin = new System.Windows.Forms.Padding(0, 2, 0, 6);
            this.flowLoginTopRow.Name = "flowLoginTopRow";
            this.flowLoginTopRow.Size = new System.Drawing.Size(642, 32);
            this.flowLoginTopRow.TabIndex = 6;
            this.flowLoginTopRow.WrapContents = false;
            // 
            // buttonLogin
            // 
            this.buttonLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.buttonLogin.FlatAppearance.BorderSize = 0;
            this.buttonLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLogin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonLogin.ForeColor = System.Drawing.Color.White;
            this.buttonLogin.Location = new System.Drawing.Point(0, 3);
            this.buttonLogin.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(103, 26);
            this.buttonLogin.TabIndex = 2;
            this.buttonLogin.Text = "로그인";
            this.buttonLogin.UseVisualStyleBackColor = false;
            // 
            // buttonLogout
            // 
            this.buttonLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.buttonLogout.Enabled = false;
            this.buttonLogout.FlatAppearance.BorderSize = 0;
            this.buttonLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.buttonLogout.Location = new System.Drawing.Point(113, 3);
            this.buttonLogout.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(103, 26);
            this.buttonLogout.TabIndex = 3;
            this.buttonLogout.Text = "로그아웃";
            this.buttonLogout.UseVisualStyleBackColor = false;
            // 
            // buttonEthercatConnect
            // 
            this.buttonEthercatConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.buttonEthercatConnect.FlatAppearance.BorderSize = 0;
            this.buttonEthercatConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEthercatConnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonEthercatConnect.ForeColor = System.Drawing.Color.White;
            this.buttonEthercatConnect.Location = new System.Drawing.Point(236, 3);
            this.buttonEthercatConnect.Margin = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.buttonEthercatConnect.Name = "buttonEthercatConnect";
            this.buttonEthercatConnect.Size = new System.Drawing.Size(108, 26);
            this.buttonEthercatConnect.TabIndex = 11;
            this.buttonEthercatConnect.Text = "EtherCAT 연결";
            this.buttonEthercatConnect.UseVisualStyleBackColor = false;
            // 
            // buttonEthercatDisconnect
            // 
            this.buttonEthercatDisconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.buttonEthercatDisconnect.FlatAppearance.BorderSize = 0;
            this.buttonEthercatDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEthercatDisconnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonEthercatDisconnect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.buttonEthercatDisconnect.Location = new System.Drawing.Point(354, 3);
            this.buttonEthercatDisconnect.Margin = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.buttonEthercatDisconnect.Name = "buttonEthercatDisconnect";
            this.buttonEthercatDisconnect.Size = new System.Drawing.Size(108, 26);
            this.buttonEthercatDisconnect.TabIndex = 12;
            this.buttonEthercatDisconnect.Text = "연결 해제";
            this.buttonEthercatDisconnect.UseVisualStyleBackColor = false;
            // 
            // buttonServoOn
            // 
            this.buttonServoOn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(133)))), ((int)(((byte)(244)))));
            this.buttonServoOn.FlatAppearance.BorderSize = 0;
            this.buttonServoOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonServoOn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonServoOn.ForeColor = System.Drawing.Color.White;
            this.buttonServoOn.Location = new System.Drawing.Point(472, 3);
            this.buttonServoOn.Margin = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.buttonServoOn.Name = "buttonServoOn";
            this.buttonServoOn.Size = new System.Drawing.Size(80, 26);
            this.buttonServoOn.TabIndex = 13;
            this.buttonServoOn.Text = "서보 ON";
            this.buttonServoOn.UseVisualStyleBackColor = false;
            // 
            // buttonServoOff
            // 
            this.buttonServoOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.buttonServoOff.FlatAppearance.BorderSize = 0;
            this.buttonServoOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonServoOff.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonServoOff.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.buttonServoOff.Location = new System.Drawing.Point(562, 3);
            this.buttonServoOff.Margin = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.buttonServoOff.Name = "buttonServoOff";
            this.buttonServoOff.Size = new System.Drawing.Size(80, 26);
            this.buttonServoOff.TabIndex = 14;
            this.buttonServoOff.Text = "서보 OFF";
            this.buttonServoOff.UseVisualStyleBackColor = false;
            // 
            // flowLoginBottomRow
            // 
            this.flowLoginBottomRow.AutoSize = true;
            this.flowLoginBottomRow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLoginBottomRow.Controls.Add(this.labelLoginStatus);
            this.flowLoginBottomRow.Controls.Add(this.labelEthercatStatus);
            this.flowLoginBottomRow.Controls.Add(this.labelServoStatus);
            this.flowLoginBottomRow.Location = new System.Drawing.Point(16, 48);
            this.flowLoginBottomRow.Margin = new System.Windows.Forms.Padding(0);
            this.flowLoginBottomRow.Name = "flowLoginBottomRow";
            this.flowLoginBottomRow.Size = new System.Drawing.Size(640, 29);
            this.flowLoginBottomRow.TabIndex = 7;
            this.flowLoginBottomRow.WrapContents = false;
            // 
            // labelLoginStatus
            // 
            this.labelLoginStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(180)))));
            this.labelLoginStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelLoginStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelLoginStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelLoginStatus.Location = new System.Drawing.Point(0, 3);
            this.labelLoginStatus.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelLoginStatus.Name = "labelLoginStatus";
            this.labelLoginStatus.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.labelLoginStatus.Size = new System.Drawing.Size(220, 26);
            this.labelLoginStatus.TabIndex = 5;
            this.labelLoginStatus.Text = "사용자: Guest / 권한: 없음";
            this.labelLoginStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelEthercatStatus
            // 
            this.labelEthercatStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(180)))));
            this.labelEthercatStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelEthercatStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelEthercatStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelEthercatStatus.Location = new System.Drawing.Point(230, 3);
            this.labelEthercatStatus.Margin = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.labelEthercatStatus.MaximumSize = new System.Drawing.Size(200, 26);
            this.labelEthercatStatus.MinimumSize = new System.Drawing.Size(200, 26);
            this.labelEthercatStatus.Name = "labelEthercatStatus";
            this.labelEthercatStatus.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.labelEthercatStatus.Size = new System.Drawing.Size(200, 26);
            this.labelEthercatStatus.TabIndex = 10;
            this.labelEthercatStatus.Text = "EtherCAT: Disconnected";
            this.labelEthercatStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelServoStatus
            // 
            this.labelServoStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(180)))));
            this.labelServoStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelServoStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelServoStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelServoStatus.Location = new System.Drawing.Point(440, 3);
            this.labelServoStatus.Margin = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.labelServoStatus.MaximumSize = new System.Drawing.Size(200, 26);
            this.labelServoStatus.MinimumSize = new System.Drawing.Size(200, 26);
            this.labelServoStatus.Name = "labelServoStatus";
            this.labelServoStatus.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.labelServoStatus.Size = new System.Drawing.Size(200, 26);
            this.labelServoStatus.TabIndex = 11;
            this.labelServoStatus.Text = "서보: OFF | Home: -";
            this.labelServoStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelHeaderStatusSummary
            // 
            this.panelHeaderStatusSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(190)))));
            this.panelHeaderStatusSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelHeaderStatusSummary.Controls.Add(this.labelHeaderStatusTitle);
            this.panelHeaderStatusSummary.Controls.Add(this.tableHeaderStatus);
            this.panelHeaderStatusSummary.Controls.Add(this.flowHeaderStatus);
            this.panelHeaderStatusSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHeaderStatusSummary.Location = new System.Drawing.Point(466, 3);
            this.panelHeaderStatusSummary.Name = "panelHeaderStatusSummary";
            this.panelHeaderStatusSummary.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.panelHeaderStatusSummary.Size = new System.Drawing.Size(887, 90);
            this.panelHeaderStatusSummary.TabIndex = 1;
            // 
            // labelHeaderStatusTitle
            // 
            this.labelHeaderStatusTitle.AutoSize = true;
            this.labelHeaderStatusTitle.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.labelHeaderStatusTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderStatusTitle.Location = new System.Drawing.Point(18, 8);
            this.labelHeaderStatusTitle.Margin = new System.Windows.Forms.Padding(0);
            this.labelHeaderStatusTitle.Name = "labelHeaderStatusTitle";
            this.labelHeaderStatusTitle.Size = new System.Drawing.Size(79, 13);
            this.labelHeaderStatusTitle.TabIndex = 2;
            this.labelHeaderStatusTitle.Text = "장비 운영 현황";
            this.labelHeaderStatusTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelHeaderStatusTitle.Visible = false;
            // 
            // tableHeaderStatus
            // 
            this.tableHeaderStatus.ColumnCount = 8;
            this.tableHeaderStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableHeaderStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableHeaderStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableHeaderStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableHeaderStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableHeaderStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableHeaderStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableHeaderStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableHeaderStatus.Controls.Add(this.labelHeaderLotTitle, 0, 0);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderFoupATitle, 1, 0);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderFoupBTitle, 2, 0);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderPM1Title, 3, 0);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderPM2Title, 4, 0);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderPM3Title, 5, 0);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderTMTitle, 6, 0);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderAlarmTitle, 7, 0);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderLotStatus, 0, 1);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderFoupAStatus, 1, 1);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderFoupBStatus, 2, 1);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderPM1Status, 3, 1);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderPM2Status, 4, 1);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderPM3Status, 5, 1);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderTMStatus, 6, 1);
            this.tableHeaderStatus.Controls.Add(this.labelHeaderAlarmStatus, 7, 1);
            this.tableHeaderStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableHeaderStatus.Location = new System.Drawing.Point(16, 8);
            this.tableHeaderStatus.Margin = new System.Windows.Forms.Padding(0);
            this.tableHeaderStatus.Name = "tableHeaderStatus";
            this.tableHeaderStatus.RowCount = 2;
            this.tableHeaderStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableHeaderStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableHeaderStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableHeaderStatus.Size = new System.Drawing.Size(853, 66);
            this.tableHeaderStatus.TabIndex = 1;
            // 
            // labelHeaderLotTitle
            // 
            this.labelHeaderLotTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(210)))));
            this.labelHeaderLotTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderLotTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelHeaderLotTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderLotTitle.Location = new System.Drawing.Point(2, 2);
            this.labelHeaderLotTitle.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderLotTitle.Name = "labelHeaderLotTitle";
            this.labelHeaderLotTitle.Size = new System.Drawing.Size(115, 32);
            this.labelHeaderLotTitle.TabIndex = 0;
            this.labelHeaderLotTitle.Text = "Lot";
            this.labelHeaderLotTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderFoupATitle
            // 
            this.labelHeaderFoupATitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.labelHeaderFoupATitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderFoupATitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderFoupATitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderFoupATitle.Location = new System.Drawing.Point(121, 2);
            this.labelHeaderFoupATitle.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderFoupATitle.Name = "labelHeaderFoupATitle";
            this.labelHeaderFoupATitle.Size = new System.Drawing.Size(123, 32);
            this.labelHeaderFoupATitle.TabIndex = 1;
            this.labelHeaderFoupATitle.Text = "FOUP A";
            this.labelHeaderFoupATitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderFoupBTitle
            // 
            this.labelHeaderFoupBTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.labelHeaderFoupBTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderFoupBTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderFoupBTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderFoupBTitle.Location = new System.Drawing.Point(248, 2);
            this.labelHeaderFoupBTitle.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderFoupBTitle.Name = "labelHeaderFoupBTitle";
            this.labelHeaderFoupBTitle.Size = new System.Drawing.Size(123, 32);
            this.labelHeaderFoupBTitle.TabIndex = 2;
            this.labelHeaderFoupBTitle.Text = "FOUP B";
            this.labelHeaderFoupBTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderPM1Title
            // 
            this.labelHeaderPM1Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(200)))));
            this.labelHeaderPM1Title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderPM1Title.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderPM1Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderPM1Title.Location = new System.Drawing.Point(375, 2);
            this.labelHeaderPM1Title.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderPM1Title.Name = "labelHeaderPM1Title";
            this.labelHeaderPM1Title.Size = new System.Drawing.Size(89, 32);
            this.labelHeaderPM1Title.TabIndex = 3;
            this.labelHeaderPM1Title.Text = "PM1";
            this.labelHeaderPM1Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderPM2Title
            // 
            this.labelHeaderPM2Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(200)))));
            this.labelHeaderPM2Title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderPM2Title.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderPM2Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderPM2Title.Location = new System.Drawing.Point(468, 2);
            this.labelHeaderPM2Title.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderPM2Title.Name = "labelHeaderPM2Title";
            this.labelHeaderPM2Title.Size = new System.Drawing.Size(89, 32);
            this.labelHeaderPM2Title.TabIndex = 4;
            this.labelHeaderPM2Title.Text = "PM2";
            this.labelHeaderPM2Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderPM3Title
            // 
            this.labelHeaderPM3Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(200)))));
            this.labelHeaderPM3Title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderPM3Title.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderPM3Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderPM3Title.Location = new System.Drawing.Point(561, 2);
            this.labelHeaderPM3Title.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderPM3Title.Name = "labelHeaderPM3Title";
            this.labelHeaderPM3Title.Size = new System.Drawing.Size(89, 32);
            this.labelHeaderPM3Title.TabIndex = 5;
            this.labelHeaderPM3Title.Text = "PM3";
            this.labelHeaderPM3Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderTMTitle
            // 
            this.labelHeaderTMTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(200)))));
            this.labelHeaderTMTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderTMTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderTMTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderTMTitle.Location = new System.Drawing.Point(654, 2);
            this.labelHeaderTMTitle.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderTMTitle.Name = "labelHeaderTMTitle";
            this.labelHeaderTMTitle.Size = new System.Drawing.Size(89, 32);
            this.labelHeaderTMTitle.TabIndex = 6;
            this.labelHeaderTMTitle.Text = "TM";
            this.labelHeaderTMTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderAlarmTitle
            // 
            this.labelHeaderAlarmTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.labelHeaderAlarmTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderAlarmTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderAlarmTitle.ForeColor = System.Drawing.Color.White;
            this.labelHeaderAlarmTitle.Location = new System.Drawing.Point(747, 2);
            this.labelHeaderAlarmTitle.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderAlarmTitle.Name = "labelHeaderAlarmTitle";
            this.labelHeaderAlarmTitle.Size = new System.Drawing.Size(104, 32);
            this.labelHeaderAlarmTitle.TabIndex = 6;
            this.labelHeaderAlarmTitle.Text = "ALARM";
            this.labelHeaderAlarmTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderLotStatus
            // 
            this.labelHeaderLotStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
            this.labelHeaderLotStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderLotStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelHeaderLotStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderLotStatus.Location = new System.Drawing.Point(2, 38);
            this.labelHeaderLotStatus.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderLotStatus.Name = "labelHeaderLotStatus";
            this.labelHeaderLotStatus.Size = new System.Drawing.Size(115, 26);
            this.labelHeaderLotStatus.TabIndex = 7;
            this.labelHeaderLotStatus.Text = "Auto Processing";
            this.labelHeaderLotStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderFoupAStatus
            // 
            this.labelHeaderFoupAStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(190)))), ((int)(((byte)(210)))));
            this.labelHeaderFoupAStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderFoupAStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.labelHeaderFoupAStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderFoupAStatus.Location = new System.Drawing.Point(121, 38);
            this.labelHeaderFoupAStatus.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderFoupAStatus.Name = "labelHeaderFoupAStatus";
            this.labelHeaderFoupAStatus.Size = new System.Drawing.Size(123, 26);
            this.labelHeaderFoupAStatus.TabIndex = 8;
            this.labelHeaderFoupAStatus.Text = "대기 웨이퍼: 0";
            this.labelHeaderFoupAStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderFoupBStatus
            // 
            this.labelHeaderFoupBStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(190)))), ((int)(((byte)(210)))));
            this.labelHeaderFoupBStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderFoupBStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.labelHeaderFoupBStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderFoupBStatus.Location = new System.Drawing.Point(248, 38);
            this.labelHeaderFoupBStatus.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderFoupBStatus.Name = "labelHeaderFoupBStatus";
            this.labelHeaderFoupBStatus.Size = new System.Drawing.Size(123, 26);
            this.labelHeaderFoupBStatus.TabIndex = 9;
            this.labelHeaderFoupBStatus.Text = "완료 웨이퍼: 0";
            this.labelHeaderFoupBStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderPM1Status
            // 
            this.labelHeaderPM1Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(180)))), ((int)(((byte)(200)))));
            this.labelHeaderPM1Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderPM1Status.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.labelHeaderPM1Status.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderPM1Status.Location = new System.Drawing.Point(375, 38);
            this.labelHeaderPM1Status.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderPM1Status.Name = "labelHeaderPM1Status";
            this.labelHeaderPM1Status.Size = new System.Drawing.Size(89, 26);
            this.labelHeaderPM1Status.TabIndex = 10;
            this.labelHeaderPM1Status.Text = "Auto Proc";
            this.labelHeaderPM1Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderPM2Status
            // 
            this.labelHeaderPM2Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(180)))), ((int)(((byte)(200)))));
            this.labelHeaderPM2Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderPM2Status.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.labelHeaderPM2Status.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderPM2Status.Location = new System.Drawing.Point(468, 38);
            this.labelHeaderPM2Status.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderPM2Status.Name = "labelHeaderPM2Status";
            this.labelHeaderPM2Status.Size = new System.Drawing.Size(89, 26);
            this.labelHeaderPM2Status.TabIndex = 11;
            this.labelHeaderPM2Status.Text = "Auto Proc";
            this.labelHeaderPM2Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderPM3Status
            // 
            this.labelHeaderPM3Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(180)))), ((int)(((byte)(200)))));
            this.labelHeaderPM3Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderPM3Status.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.labelHeaderPM3Status.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderPM3Status.Location = new System.Drawing.Point(561, 38);
            this.labelHeaderPM3Status.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderPM3Status.Name = "labelHeaderPM3Status";
            this.labelHeaderPM3Status.Size = new System.Drawing.Size(89, 26);
            this.labelHeaderPM3Status.TabIndex = 12;
            this.labelHeaderPM3Status.Text = "Standby";
            this.labelHeaderPM3Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderTMStatus
            // 
            this.labelHeaderTMStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(180)))), ((int)(((byte)(200)))));
            this.labelHeaderTMStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderTMStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.labelHeaderTMStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderTMStatus.Location = new System.Drawing.Point(654, 38);
            this.labelHeaderTMStatus.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderTMStatus.Name = "labelHeaderTMStatus";
            this.labelHeaderTMStatus.Size = new System.Drawing.Size(89, 26);
            this.labelHeaderTMStatus.TabIndex = 13;
            this.labelHeaderTMStatus.Text = "TM Ready";
            this.labelHeaderTMStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderAlarmStatus
            // 
            this.labelHeaderAlarmStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.labelHeaderAlarmStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderAlarmStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelHeaderAlarmStatus.ForeColor = System.Drawing.Color.White;
            this.labelHeaderAlarmStatus.Location = new System.Drawing.Point(747, 38);
            this.labelHeaderAlarmStatus.Margin = new System.Windows.Forms.Padding(2);
            this.labelHeaderAlarmStatus.Name = "labelHeaderAlarmStatus";
            this.labelHeaderAlarmStatus.Size = new System.Drawing.Size(104, 26);
            this.labelHeaderAlarmStatus.TabIndex = 14;
            this.labelHeaderAlarmStatus.Text = "Normal";
            this.labelHeaderAlarmStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowHeaderStatus
            // 
            this.flowHeaderStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(190)))));
            this.flowHeaderStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowHeaderStatus.Controls.Add(this.panelHeaderCardTM);
            this.flowHeaderStatus.Controls.Add(this.panelHeaderCardPMA);
            this.flowHeaderStatus.Controls.Add(this.panelHeaderCardPMB);
            this.flowHeaderStatus.Controls.Add(this.panelHeaderCardPMC);
            this.flowHeaderStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowHeaderStatus.Location = new System.Drawing.Point(16, 8);
            this.flowHeaderStatus.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.flowHeaderStatus.Name = "flowHeaderStatus";
            this.flowHeaderStatus.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.flowHeaderStatus.Size = new System.Drawing.Size(853, 72);
            this.flowHeaderStatus.TabIndex = 0;
            this.flowHeaderStatus.WrapContents = false;
            // 
            // panelHeaderCardTM
            // 
            this.panelHeaderCardTM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
            this.panelHeaderCardTM.Controls.Add(this.labelHeaderCardTMStatus);
            this.panelHeaderCardTM.Controls.Add(this.labelHeaderCardTMTitle);
            this.panelHeaderCardTM.Location = new System.Drawing.Point(8, 6);
            this.panelHeaderCardTM.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.panelHeaderCardTM.Name = "panelHeaderCardTM";
            this.panelHeaderCardTM.Padding = new System.Windows.Forms.Padding(8, 6, 8, 4);
            this.panelHeaderCardTM.Size = new System.Drawing.Size(120, 36);
            this.panelHeaderCardTM.TabIndex = 0;
            // 
            // labelHeaderCardTMStatus
            // 
            this.labelHeaderCardTMStatus.AutoSize = true;
            this.labelHeaderCardTMStatus.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.labelHeaderCardTMStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.labelHeaderCardTMStatus.Location = new System.Drawing.Point(56, 10);
            this.labelHeaderCardTMStatus.Name = "labelHeaderCardTMStatus";
            this.labelHeaderCardTMStatus.Size = new System.Drawing.Size(26, 13);
            this.labelHeaderCardTMStatus.TabIndex = 1;
            this.labelHeaderCardTMStatus.Text = "Idle";
            this.labelHeaderCardTMStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelHeaderCardTMTitle
            // 
            this.labelHeaderCardTMTitle.AutoSize = true;
            this.labelHeaderCardTMTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderCardTMTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.labelHeaderCardTMTitle.Location = new System.Drawing.Point(8, 9);
            this.labelHeaderCardTMTitle.Name = "labelHeaderCardTMTitle";
            this.labelHeaderCardTMTitle.Size = new System.Drawing.Size(25, 15);
            this.labelHeaderCardTMTitle.TabIndex = 0;
            this.labelHeaderCardTMTitle.Text = "TM";
            // 
            // panelHeaderCardPMA
            // 
            this.panelHeaderCardPMA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.panelHeaderCardPMA.Controls.Add(this.labelHeaderCardPMAStatus);
            this.panelHeaderCardPMA.Controls.Add(this.labelHeaderCardPMATitle);
            this.panelHeaderCardPMA.Location = new System.Drawing.Point(136, 6);
            this.panelHeaderCardPMA.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.panelHeaderCardPMA.Name = "panelHeaderCardPMA";
            this.panelHeaderCardPMA.Padding = new System.Windows.Forms.Padding(8, 6, 8, 4);
            this.panelHeaderCardPMA.Size = new System.Drawing.Size(120, 36);
            this.panelHeaderCardPMA.TabIndex = 1;
            // 
            // labelHeaderCardPMAStatus
            // 
            this.labelHeaderCardPMAStatus.AutoSize = true;
            this.labelHeaderCardPMAStatus.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.labelHeaderCardPMAStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.labelHeaderCardPMAStatus.Location = new System.Drawing.Point(56, 10);
            this.labelHeaderCardPMAStatus.Name = "labelHeaderCardPMAStatus";
            this.labelHeaderCardPMAStatus.Size = new System.Drawing.Size(49, 13);
            this.labelHeaderCardPMAStatus.TabIndex = 1;
            this.labelHeaderCardPMAStatus.Text = "Standby";
            this.labelHeaderCardPMAStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelHeaderCardPMATitle
            // 
            this.labelHeaderCardPMATitle.AutoSize = true;
            this.labelHeaderCardPMATitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderCardPMATitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.labelHeaderCardPMATitle.Location = new System.Drawing.Point(8, 9);
            this.labelHeaderCardPMATitle.Name = "labelHeaderCardPMATitle";
            this.labelHeaderCardPMATitle.Size = new System.Drawing.Size(33, 15);
            this.labelHeaderCardPMATitle.TabIndex = 0;
            this.labelHeaderCardPMATitle.Text = "PMA";
            // 
            // panelHeaderCardPMB
            // 
            this.panelHeaderCardPMB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.panelHeaderCardPMB.Controls.Add(this.labelHeaderCardPMBStatus);
            this.panelHeaderCardPMB.Controls.Add(this.labelHeaderCardPMBTitle);
            this.panelHeaderCardPMB.Location = new System.Drawing.Point(264, 6);
            this.panelHeaderCardPMB.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.panelHeaderCardPMB.Name = "panelHeaderCardPMB";
            this.panelHeaderCardPMB.Padding = new System.Windows.Forms.Padding(8, 6, 8, 4);
            this.panelHeaderCardPMB.Size = new System.Drawing.Size(120, 36);
            this.panelHeaderCardPMB.TabIndex = 2;
            // 
            // labelHeaderCardPMBStatus
            // 
            this.labelHeaderCardPMBStatus.AutoSize = true;
            this.labelHeaderCardPMBStatus.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.labelHeaderCardPMBStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.labelHeaderCardPMBStatus.Location = new System.Drawing.Point(56, 10);
            this.labelHeaderCardPMBStatus.Name = "labelHeaderCardPMBStatus";
            this.labelHeaderCardPMBStatus.Size = new System.Drawing.Size(62, 13);
            this.labelHeaderCardPMBStatus.TabIndex = 1;
            this.labelHeaderCardPMBStatus.Text = "Processing";
            this.labelHeaderCardPMBStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelHeaderCardPMBTitle
            // 
            this.labelHeaderCardPMBTitle.AutoSize = true;
            this.labelHeaderCardPMBTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderCardPMBTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.labelHeaderCardPMBTitle.Location = new System.Drawing.Point(8, 9);
            this.labelHeaderCardPMBTitle.Name = "labelHeaderCardPMBTitle";
            this.labelHeaderCardPMBTitle.Size = new System.Drawing.Size(33, 15);
            this.labelHeaderCardPMBTitle.TabIndex = 0;
            this.labelHeaderCardPMBTitle.Text = "PMB";
            // 
            // panelHeaderCardPMC
            // 
            this.panelHeaderCardPMC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.panelHeaderCardPMC.Controls.Add(this.labelHeaderCardPMCStatus);
            this.panelHeaderCardPMC.Controls.Add(this.labelHeaderCardPMCTitle);
            this.panelHeaderCardPMC.Location = new System.Drawing.Point(392, 6);
            this.panelHeaderCardPMC.Margin = new System.Windows.Forms.Padding(0);
            this.panelHeaderCardPMC.Name = "panelHeaderCardPMC";
            this.panelHeaderCardPMC.Padding = new System.Windows.Forms.Padding(8, 6, 8, 4);
            this.panelHeaderCardPMC.Size = new System.Drawing.Size(120, 36);
            this.panelHeaderCardPMC.TabIndex = 3;
            // 
            // labelHeaderCardPMCStatus
            // 
            this.labelHeaderCardPMCStatus.AutoSize = true;
            this.labelHeaderCardPMCStatus.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.labelHeaderCardPMCStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.labelHeaderCardPMCStatus.Location = new System.Drawing.Point(56, 10);
            this.labelHeaderCardPMCStatus.Name = "labelHeaderCardPMCStatus";
            this.labelHeaderCardPMCStatus.Size = new System.Drawing.Size(74, 13);
            this.labelHeaderCardPMCStatus.TabIndex = 1;
            this.labelHeaderCardPMCStatus.Text = "Maintenance";
            this.labelHeaderCardPMCStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelHeaderCardPMCTitle
            // 
            this.labelHeaderCardPMCTitle.AutoSize = true;
            this.labelHeaderCardPMCTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderCardPMCTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.labelHeaderCardPMCTitle.Location = new System.Drawing.Point(8, 9);
            this.labelHeaderCardPMCTitle.Name = "labelHeaderCardPMCTitle";
            this.labelHeaderCardPMCTitle.Size = new System.Drawing.Size(32, 15);
            this.labelHeaderCardPMCTitle.TabIndex = 0;
            this.labelHeaderCardPMCTitle.Text = "PMC";
            // 
            // panelHeaderAlarm
            // 
            this.panelHeaderAlarm.AutoSize = true;
            this.panelHeaderAlarm.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelHeaderAlarm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(200)))));
            this.panelHeaderAlarm.ColumnCount = 1;
            this.panelHeaderAlarm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelHeaderAlarm.Controls.Add(this.flowAlarmIndicator, 0, 0);
            this.panelHeaderAlarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHeaderAlarm.Location = new System.Drawing.Point(1356, 0);
            this.panelHeaderAlarm.Margin = new System.Windows.Forms.Padding(0);
            this.panelHeaderAlarm.Name = "panelHeaderAlarm";
            this.panelHeaderAlarm.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.panelHeaderAlarm.RowCount = 1;
            this.panelHeaderAlarm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelHeaderAlarm.Size = new System.Drawing.Size(299, 96);
            this.panelHeaderAlarm.TabIndex = 3;
            // 
            // flowAlarmIndicator
            // 
            this.flowAlarmIndicator.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowAlarmIndicator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(180)))));
            this.flowAlarmIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowAlarmIndicator.Controls.Add(this.panelHeaderMessageAccent);
            this.flowAlarmIndicator.Controls.Add(this.tableHeaderMessageText);
            this.flowAlarmIndicator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowAlarmIndicator.Location = new System.Drawing.Point(12, 8);
            this.flowAlarmIndicator.Margin = new System.Windows.Forms.Padding(0);
            this.flowAlarmIndicator.Name = "flowAlarmIndicator";
            this.flowAlarmIndicator.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.flowAlarmIndicator.Size = new System.Drawing.Size(275, 80);
            this.flowAlarmIndicator.TabIndex = 3;
            this.flowAlarmIndicator.WrapContents = false;
            // 
            // panelHeaderMessageAccent
            // 
            this.panelHeaderMessageAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(111)))), ((int)(((byte)(60)))));
            this.panelHeaderMessageAccent.Location = new System.Drawing.Point(10, 6);
            this.panelHeaderMessageAccent.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.panelHeaderMessageAccent.Name = "panelHeaderMessageAccent";
            this.panelHeaderMessageAccent.Size = new System.Drawing.Size(6, 34);
            this.panelHeaderMessageAccent.TabIndex = 4;
            // 
            // tableHeaderMessageText
            // 
            this.tableHeaderMessageText.AutoSize = true;
            this.tableHeaderMessageText.ColumnCount = 2;
            this.tableHeaderMessageText.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78F));
            this.tableHeaderMessageText.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tableHeaderMessageText.Controls.Add(this.labelHeaderEventTitle, 0, 0);
            this.tableHeaderMessageText.Controls.Add(this.labelHeaderEventLevel, 1, 0);
            this.tableHeaderMessageText.Controls.Add(this.labelHeaderEventMessage, 0, 1);
            this.tableHeaderMessageText.Location = new System.Drawing.Point(28, 6);
            this.tableHeaderMessageText.Margin = new System.Windows.Forms.Padding(0);
            this.tableHeaderMessageText.Name = "tableHeaderMessageText";
            this.tableHeaderMessageText.RowCount = 2;
            this.tableHeaderMessageText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableHeaderMessageText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableHeaderMessageText.Size = new System.Drawing.Size(65535, 56);
            this.tableHeaderMessageText.TabIndex = 5;
            // 
            // labelHeaderEventTitle
            // 
            this.labelHeaderEventTitle.AutoSize = true;
            this.labelHeaderEventTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderEventTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.labelHeaderEventTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderEventTitle.Location = new System.Drawing.Point(0, 0);
            this.labelHeaderEventTitle.Margin = new System.Windows.Forms.Padding(0, 0, 8, 4);
            this.labelHeaderEventTitle.Name = "labelHeaderEventTitle";
            this.labelHeaderEventTitle.Size = new System.Drawing.Size(51109, 21);
            this.labelHeaderEventTitle.TabIndex = 0;
            this.labelHeaderEventTitle.Text = "최근 메시지";
            this.labelHeaderEventTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelHeaderEventLevel
            // 
            this.labelHeaderEventLevel.AutoSize = true;
            this.labelHeaderEventLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.labelHeaderEventLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderEventLevel.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelHeaderEventLevel.ForeColor = System.Drawing.Color.White;
            this.labelHeaderEventLevel.Location = new System.Drawing.Point(32767, 0);
            this.labelHeaderEventLevel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.labelHeaderEventLevel.MinimumSize = new System.Drawing.Size(48, 0);
            this.labelHeaderEventLevel.Name = "labelHeaderEventLevel";
            this.labelHeaderEventLevel.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.labelHeaderEventLevel.Size = new System.Drawing.Size(14418, 21);
            this.labelHeaderEventLevel.TabIndex = 2;
            this.labelHeaderEventLevel.Text = "INFO";
            this.labelHeaderEventLevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHeaderEventMessage
            // 
            this.labelHeaderEventMessage.AutoEllipsis = true;
            this.tableHeaderMessageText.SetColumnSpan(this.labelHeaderEventMessage, 2);
            this.labelHeaderEventMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeaderEventMessage.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelHeaderEventMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderEventMessage.Location = new System.Drawing.Point(0, 25);
            this.labelHeaderEventMessage.Margin = new System.Windows.Forms.Padding(0);
            this.labelHeaderEventMessage.Name = "labelHeaderEventMessage";
            this.labelHeaderEventMessage.Size = new System.Drawing.Size(65535, 31);
            this.labelHeaderEventMessage.TabIndex = 1;
            this.labelHeaderEventMessage.Text = "메시지 없음";
            this.labelHeaderEventMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowHeaderTabs
            // 
            this.flowHeaderTabs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(190)))));
            this.tableLayoutHeader.SetColumnSpan(this.flowHeaderTabs, 2);
            this.flowHeaderTabs.Controls.Add(this.buttonTabMain);
            this.flowHeaderTabs.Controls.Add(this.buttonTabVerification);
            this.flowHeaderTabs.Controls.Add(this.buttonTabTransfer);
            this.flowHeaderTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowHeaderTabs.Location = new System.Drawing.Point(0, 96);
            this.flowHeaderTabs.Margin = new System.Windows.Forms.Padding(0);
            this.flowHeaderTabs.Name = "flowHeaderTabs";
            this.flowHeaderTabs.Padding = new System.Windows.Forms.Padding(12, 6, 0, 8);
            this.flowHeaderTabs.Size = new System.Drawing.Size(1356, 64);
            this.flowHeaderTabs.TabIndex = 6;
            this.flowHeaderTabs.WrapContents = false;
            // 
            // buttonTabMain
            // 
            this.buttonTabMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonTabMain.FlatAppearance.BorderSize = 0;
            this.buttonTabMain.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(140)))), ((int)(((byte)(150)))));
            this.buttonTabMain.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(140)))), ((int)(((byte)(150)))));
            this.buttonTabMain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTabMain.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonTabMain.ForeColor = System.Drawing.Color.White;
            this.buttonTabMain.Location = new System.Drawing.Point(12, 6);
            this.buttonTabMain.Margin = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.buttonTabMain.Name = "buttonTabMain";
            this.buttonTabMain.Size = new System.Drawing.Size(140, 34);
            this.buttonTabMain.TabIndex = 0;
            this.buttonTabMain.Text = "MAIN";
            this.buttonTabMain.UseVisualStyleBackColor = false;
            // 
            // buttonTabVerification
            // 
            this.buttonTabVerification.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonTabVerification.FlatAppearance.BorderSize = 0;
            this.buttonTabVerification.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(140)))), ((int)(((byte)(150)))));
            this.buttonTabVerification.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(140)))), ((int)(((byte)(150)))));
            this.buttonTabVerification.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTabVerification.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonTabVerification.ForeColor = System.Drawing.Color.White;
            this.buttonTabVerification.Location = new System.Drawing.Point(166, 6);
            this.buttonTabVerification.Margin = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.buttonTabVerification.Name = "buttonTabVerification";
            this.buttonTabVerification.Size = new System.Drawing.Size(160, 34);
            this.buttonTabVerification.TabIndex = 1;
            this.buttonTabVerification.Text = "VERIFICATION";
            this.buttonTabVerification.UseVisualStyleBackColor = false;
            // 
            // buttonTabTransfer
            // 
            this.buttonTabTransfer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonTabTransfer.FlatAppearance.BorderSize = 0;
            this.buttonTabTransfer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(140)))), ((int)(((byte)(150)))));
            this.buttonTabTransfer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(140)))), ((int)(((byte)(150)))));
            this.buttonTabTransfer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTabTransfer.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonTabTransfer.ForeColor = System.Drawing.Color.White;
            this.buttonTabTransfer.Location = new System.Drawing.Point(340, 6);
            this.buttonTabTransfer.Margin = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.buttonTabTransfer.Name = "buttonTabTransfer";
            this.buttonTabTransfer.Size = new System.Drawing.Size(140, 34);
            this.buttonTabTransfer.TabIndex = 2;
            this.buttonTabTransfer.Text = "TRANSFER";
            this.buttonTabTransfer.UseVisualStyleBackColor = false;
            // 
            // flowHeaderTimeAndUser
            // 
            this.flowHeaderTimeAndUser.AutoSize = true;
            this.flowHeaderTimeAndUser.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowHeaderTimeAndUser.Controls.Add(this.labelHeaderCurrentTime);
            this.flowHeaderTimeAndUser.Controls.Add(this.buttonUserManagement);
            this.flowHeaderTimeAndUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowHeaderTimeAndUser.Location = new System.Drawing.Point(1356, 96);
            this.flowHeaderTimeAndUser.Margin = new System.Windows.Forms.Padding(0);
            this.flowHeaderTimeAndUser.Name = "flowHeaderTimeAndUser";
            this.flowHeaderTimeAndUser.Padding = new System.Windows.Forms.Padding(12, 6, 16, 0);
            this.flowHeaderTimeAndUser.Size = new System.Drawing.Size(299, 64);
            this.flowHeaderTimeAndUser.TabIndex = 0;
            this.flowHeaderTimeAndUser.WrapContents = false;
            // 
            // labelHeaderCurrentTime
            // 
            this.labelHeaderCurrentTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(200)))));
            this.labelHeaderCurrentTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelHeaderCurrentTime.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelHeaderCurrentTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelHeaderCurrentTime.Location = new System.Drawing.Point(12, 6);
            this.labelHeaderCurrentTime.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.labelHeaderCurrentTime.MaximumSize = new System.Drawing.Size(220, 26);
            this.labelHeaderCurrentTime.MinimumSize = new System.Drawing.Size(220, 26);
            this.labelHeaderCurrentTime.Name = "labelHeaderCurrentTime";
            this.labelHeaderCurrentTime.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.labelHeaderCurrentTime.Size = new System.Drawing.Size(220, 26);
            this.labelHeaderCurrentTime.TabIndex = 4;
            this.labelHeaderCurrentTime.Text = "현재 시각: 2025-11-11 10:05:00";
            this.labelHeaderCurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonUserManagement
            // 
            this.buttonUserManagement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonUserManagement.FlatAppearance.BorderSize = 0;
            this.buttonUserManagement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUserManagement.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonUserManagement.ForeColor = System.Drawing.Color.White;
            this.buttonUserManagement.Location = new System.Drawing.Point(240, 6);
            this.buttonUserManagement.Margin = new System.Windows.Forms.Padding(0);
            this.buttonUserManagement.Name = "buttonUserManagement";
            this.buttonUserManagement.Size = new System.Drawing.Size(100, 26);
            this.buttonUserManagement.TabIndex = 5;
            this.buttonUserManagement.Text = "사용자 관리";
            this.buttonUserManagement.UseVisualStyleBackColor = false;
            // 
            // tableLayoutProcessArea
            // 
            this.tableLayoutProcessArea.ColumnCount = 2;
            this.tableLayoutProcessArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutProcessArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.tableLayoutProcessArea.Controls.Add(this.panelMainProcess, 0, 0);
            this.tableLayoutProcessArea.Controls.Add(this.panelControlPanel, 1, 0);
            this.tableLayoutProcessArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutProcessArea.Location = new System.Drawing.Point(3, 173);
            this.tableLayoutProcessArea.Name = "tableLayoutProcessArea";
            this.tableLayoutProcessArea.RowCount = 1;
            this.tableLayoutProcessArea.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutProcessArea.Size = new System.Drawing.Size(1695, 730);
            this.tableLayoutProcessArea.TabIndex = 1;
            // 
            // panelMainProcess
            // 
            this.panelMainProcess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
            this.panelMainProcess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMainProcess.Controls.Add(this.tableLayoutMainProcess);
            this.panelMainProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainProcess.Location = new System.Drawing.Point(3, 3);
            this.panelMainProcess.Name = "panelMainProcess";
            this.panelMainProcess.Padding = new System.Windows.Forms.Padding(20, 20, 20, 15);
            this.panelMainProcess.Size = new System.Drawing.Size(1369, 724);
            this.panelMainProcess.TabIndex = 1;
            // 
            // tableLayoutMainProcess
            // 
            this.tableLayoutMainProcess.ColumnCount = 1;
            this.tableLayoutMainProcess.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMainProcess.Controls.Add(this.labelMainProcessTitle, 0, 0);
            this.tableLayoutMainProcess.Controls.Add(this.tableLayoutMainContent, 0, 1);
            this.tableLayoutMainProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMainProcess.Location = new System.Drawing.Point(20, 20);
            this.tableLayoutMainProcess.Name = "tableLayoutMainProcess";
            this.tableLayoutMainProcess.RowCount = 2;
            this.tableLayoutMainProcess.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutMainProcess.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMainProcess.Size = new System.Drawing.Size(1327, 687);
            this.tableLayoutMainProcess.TabIndex = 0;
            // 
            // labelMainProcessTitle
            // 
            this.labelMainProcessTitle.AutoSize = true;
            this.labelMainProcessTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMainProcessTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelMainProcessTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelMainProcessTitle.Location = new System.Drawing.Point(3, 0);
            this.labelMainProcessTitle.Name = "labelMainProcessTitle";
            this.labelMainProcessTitle.Size = new System.Drawing.Size(1321, 32);
            this.labelMainProcessTitle.TabIndex = 0;
            this.labelMainProcessTitle.Text = "메인 공정 모니터링";
            this.labelMainProcessTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutMainContent
            // 
            this.tableLayoutMainContent.ColumnCount = 2;
            this.tableLayoutMainContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutMainContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutMainContent.Controls.Add(this.panelEquipment, 0, 0);
            this.tableLayoutMainContent.Controls.Add(this.panelPmStatus, 1, 0);
            this.tableLayoutMainContent.Controls.Add(this.tableProcessMetrics, 0, 1);
            this.tableLayoutMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMainContent.Location = new System.Drawing.Point(3, 35);
            this.tableLayoutMainContent.Name = "tableLayoutMainContent";
            this.tableLayoutMainContent.RowCount = 2;
            this.tableLayoutMainContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMainContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutMainContent.Size = new System.Drawing.Size(1321, 649);
            this.tableLayoutMainContent.TabIndex = 4;
            // 
            // panelEquipment
            // 
            this.panelEquipment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
            this.panelEquipment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEquipment.Controls.Add(this.tableLayoutEquipmentArea);
            this.panelEquipment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEquipment.Location = new System.Drawing.Point(3, 3);
            this.panelEquipment.Name = "panelEquipment";
            this.panelEquipment.Padding = new System.Windows.Forms.Padding(20);
            this.panelEquipment.Size = new System.Drawing.Size(984, 403);
            this.panelEquipment.TabIndex = 1;
            // 
            // tableLayoutEquipmentArea
            // 
            this.tableLayoutEquipmentArea.ColumnCount = 1;
            this.tableLayoutEquipmentArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutEquipmentArea.Controls.Add(this.tableLayoutEquipment, 0, 0);
            this.tableLayoutEquipmentArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEquipmentArea.Location = new System.Drawing.Point(20, 20);
            this.tableLayoutEquipmentArea.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutEquipmentArea.Name = "tableLayoutEquipmentArea";
            this.tableLayoutEquipmentArea.RowCount = 1;
            this.tableLayoutEquipmentArea.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutEquipmentArea.Size = new System.Drawing.Size(942, 361);
            this.tableLayoutEquipmentArea.TabIndex = 0;
            // 
            // tableLayoutEquipment
            // 
            this.tableLayoutEquipment.ColumnCount = 1;
            this.tableLayoutEquipment.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutEquipment.Controls.Add(this.tableLayoutChamberCluster, 0, 0);
            this.tableLayoutEquipment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutEquipment.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutEquipment.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutEquipment.Name = "tableLayoutEquipment";
            this.tableLayoutEquipment.RowCount = 1;
            this.tableLayoutEquipment.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutEquipment.Size = new System.Drawing.Size(942, 361);
            this.tableLayoutEquipment.TabIndex = 0;
            // 
            // tableLayoutChamberCluster
            // 
            this.tableLayoutChamberCluster.ColumnCount = 1;
            this.tableLayoutChamberCluster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutChamberCluster.Controls.Add(this.panelEquipmentCanvas, 0, 0);
            this.tableLayoutChamberCluster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutChamberCluster.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutChamberCluster.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutChamberCluster.Name = "tableLayoutChamberCluster";
            this.tableLayoutChamberCluster.RowCount = 1;
            this.tableLayoutChamberCluster.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutChamberCluster.Size = new System.Drawing.Size(942, 361);
            this.tableLayoutChamberCluster.TabIndex = 0;
            // 
            // panelEquipmentCanvas
            // 
            this.panelEquipmentCanvas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.panelEquipmentCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEquipmentCanvas.Location = new System.Drawing.Point(0, 0);
            this.panelEquipmentCanvas.Margin = new System.Windows.Forms.Padding(0);
            this.panelEquipmentCanvas.Name = "panelEquipmentCanvas";
            this.panelEquipmentCanvas.Size = new System.Drawing.Size(942, 361);
            this.panelEquipmentCanvas.TabIndex = 0;
            this.panelEquipmentCanvas.Resize += new System.EventHandler(this.panelEquipmentCanvas_Resize);
            // 
            // panelPmStatus
            // 
            this.panelPmStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.panelPmStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPmStatus.Controls.Add(this.tableLayoutPmStatus);
            this.panelPmStatus.Controls.Add(this.labelPmStatusTitle);
            this.panelPmStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPmStatus.Location = new System.Drawing.Point(993, 0);
            this.panelPmStatus.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.panelPmStatus.Name = "panelPmStatus";
            this.panelPmStatus.Padding = new System.Windows.Forms.Padding(12, 12, 12, 10);
            this.tableLayoutMainContent.SetRowSpan(this.panelPmStatus, 2);
            this.panelPmStatus.Size = new System.Drawing.Size(328, 649);
            this.panelPmStatus.TabIndex = 1;
            // 
            // tableLayoutPmStatus
            // 
            this.tableLayoutPmStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPmStatus.ColumnCount = 1;
            this.tableLayoutPmStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPmStatus.Controls.Add(this.panelSummaryPMA, 0, 0);
            this.tableLayoutPmStatus.Controls.Add(this.panelSummaryPMB, 0, 1);
            this.tableLayoutPmStatus.Controls.Add(this.panelSummaryPMC, 0, 2);
            this.tableLayoutPmStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPmStatus.Location = new System.Drawing.Point(12, 38);
            this.tableLayoutPmStatus.Name = "tableLayoutPmStatus";
            this.tableLayoutPmStatus.RowCount = 3;
            this.tableLayoutPmStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPmStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPmStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPmStatus.Size = new System.Drawing.Size(302, 599);
            this.tableLayoutPmStatus.TabIndex = 1;
            // 
            // panelSummaryPMA
            // 
            this.panelSummaryPMA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.panelSummaryPMA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummaryPMA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSummaryPMA.Location = new System.Drawing.Point(12, 0);
            this.panelSummaryPMA.Margin = new System.Windows.Forms.Padding(12, 0, 12, 6);
            this.panelSummaryPMA.MinimumSize = new System.Drawing.Size(2, 190);
            this.panelSummaryPMA.Name = "panelSummaryPMA";
            this.panelSummaryPMA.Padding = new System.Windows.Forms.Padding(12, 12, 12, 4);
            this.panelSummaryPMA.Size = new System.Drawing.Size(278, 193);
            this.panelSummaryPMA.TabIndex = 1;
            // 
            // panelSummaryPMB
            // 
            this.panelSummaryPMB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.panelSummaryPMB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummaryPMB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSummaryPMB.Location = new System.Drawing.Point(12, 199);
            this.panelSummaryPMB.Margin = new System.Windows.Forms.Padding(12, 0, 12, 6);
            this.panelSummaryPMB.MinimumSize = new System.Drawing.Size(2, 190);
            this.panelSummaryPMB.Name = "panelSummaryPMB";
            this.panelSummaryPMB.Padding = new System.Windows.Forms.Padding(12, 12, 12, 4);
            this.panelSummaryPMB.Size = new System.Drawing.Size(278, 193);
            this.panelSummaryPMB.TabIndex = 2;
            // 
            // panelSummaryPMC
            // 
            this.panelSummaryPMC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.panelSummaryPMC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummaryPMC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSummaryPMC.Location = new System.Drawing.Point(12, 398);
            this.panelSummaryPMC.Margin = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.panelSummaryPMC.MinimumSize = new System.Drawing.Size(2, 190);
            this.panelSummaryPMC.Name = "panelSummaryPMC";
            this.panelSummaryPMC.Padding = new System.Windows.Forms.Padding(12, 12, 12, 4);
            this.panelSummaryPMC.Size = new System.Drawing.Size(278, 201);
            this.panelSummaryPMC.TabIndex = 3;
            // 
            // labelPmStatusTitle
            // 
            this.labelPmStatusTitle.AutoSize = true;
            this.labelPmStatusTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPmStatusTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.labelPmStatusTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelPmStatusTitle.Location = new System.Drawing.Point(12, 12);
            this.labelPmStatusTitle.Name = "labelPmStatusTitle";
            this.labelPmStatusTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.labelPmStatusTitle.Size = new System.Drawing.Size(100, 26);
            this.labelPmStatusTitle.TabIndex = 0;
            this.labelPmStatusTitle.Text = "PM 진행 현황";
            // 
            // tableProcessMetrics
            // 
            this.tableProcessMetrics.ColumnCount = 2;
            this.tableProcessMetrics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableProcessMetrics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableProcessMetrics.Controls.Add(this.panelFoupStatusA, 0, 0);
            this.tableProcessMetrics.Controls.Add(this.panelFoupStatusB, 1, 0);
            this.tableProcessMetrics.Controls.Add(this.labelFoupSummaryInfo, 0, 1);
            this.tableProcessMetrics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableProcessMetrics.Location = new System.Drawing.Point(3, 412);
            this.tableProcessMetrics.Name = "tableProcessMetrics";
            this.tableProcessMetrics.RowCount = 2;
            this.tableProcessMetrics.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableProcessMetrics.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableProcessMetrics.Size = new System.Drawing.Size(984, 234);
            this.tableProcessMetrics.TabIndex = 4;
            // 
            // panelFoupStatusA
            // 
            this.panelFoupStatusA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.panelFoupStatusA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFoupStatusA.Controls.Add(this.tableFoupACard);
            this.panelFoupStatusA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFoupStatusA.Location = new System.Drawing.Point(0, 3);
            this.panelFoupStatusA.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.panelFoupStatusA.Name = "panelFoupStatusA";
            this.panelFoupStatusA.Padding = new System.Windows.Forms.Padding(10, 10, 10, 12);
            this.panelFoupStatusA.Size = new System.Drawing.Size(484, 196);
            this.panelFoupStatusA.TabIndex = 0;
            // 
            // tableFoupACard
            // 
            this.tableFoupACard.ColumnCount = 2;
            this.tableFoupACard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableFoupACard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableFoupACard.Controls.Add(this.panelFoupALevelContainer, 0, 0);
            this.tableFoupACard.Controls.Add(this.panelFoupADetail, 1, 0);
            this.tableFoupACard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableFoupACard.Location = new System.Drawing.Point(10, 10);
            this.tableFoupACard.Name = "tableFoupACard";
            this.tableFoupACard.RowCount = 1;
            this.tableFoupACard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableFoupACard.Size = new System.Drawing.Size(462, 172);
            this.tableFoupACard.TabIndex = 0;
            // 
            // panelFoupALevelContainer
            // 
            this.panelFoupALevelContainer.Controls.Add(this.panelFoupALevelTrack);
            this.panelFoupALevelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFoupALevelContainer.Location = new System.Drawing.Point(3, 3);
            this.panelFoupALevelContainer.Name = "panelFoupALevelContainer";
            this.panelFoupALevelContainer.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.panelFoupALevelContainer.Size = new System.Drawing.Size(144, 166);
            this.panelFoupALevelContainer.TabIndex = 0;
            // 
            // panelFoupALevelTrack
            // 
            this.panelFoupALevelTrack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.panelFoupALevelTrack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFoupALevelTrack.Controls.Add(this.panelFoupALevelFill);
            this.panelFoupALevelTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFoupALevelTrack.Location = new System.Drawing.Point(8, 5);
            this.panelFoupALevelTrack.Name = "panelFoupALevelTrack";
            this.panelFoupALevelTrack.Size = new System.Drawing.Size(128, 156);
            this.panelFoupALevelTrack.TabIndex = 0;
            // 
            // panelFoupALevelFill
            // 
            this.panelFoupALevelFill.BackColor = System.Drawing.Color.MediumBlue;
            this.panelFoupALevelFill.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFoupALevelFill.Location = new System.Drawing.Point(0, 154);
            this.panelFoupALevelFill.Name = "panelFoupALevelFill";
            this.panelFoupALevelFill.Size = new System.Drawing.Size(126, 0);
            this.panelFoupALevelFill.TabIndex = 0;
            // 
            // panelFoupADetail
            // 
            this.panelFoupADetail.Controls.Add(this.tableFoupAInfo);
            this.panelFoupADetail.Controls.Add(this.labelFoupAStatusHeadline);
            this.panelFoupADetail.Controls.Add(this.labelFoupInfoATitle);
            this.panelFoupADetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFoupADetail.Location = new System.Drawing.Point(153, 3);
            this.panelFoupADetail.Name = "panelFoupADetail";
            this.panelFoupADetail.Size = new System.Drawing.Size(306, 166);
            this.panelFoupADetail.TabIndex = 1;
            // 
            // tableFoupAInfo
            // 
            this.tableFoupAInfo.ColumnCount = 2;
            this.tableFoupAInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableFoupAInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableFoupAInfo.Controls.Add(this.labelFoupAFieldPath, 0, 0);
            this.tableFoupAInfo.Controls.Add(this.labelFoupAPathValue, 1, 0);
            this.tableFoupAInfo.Controls.Add(this.labelFoupAFieldPPID, 0, 1);
            this.tableFoupAInfo.Controls.Add(this.labelFoupAPPIDValue, 1, 1);
            this.tableFoupAInfo.Controls.Add(this.labelFoupAFieldLotId, 0, 2);
            this.tableFoupAInfo.Controls.Add(this.labelFoupALotIdValue, 1, 2);
            this.tableFoupAInfo.Controls.Add(this.labelFoupAFieldMid, 0, 3);
            this.tableFoupAInfo.Controls.Add(this.labelFoupAMidValue, 1, 3);
            this.tableFoupAInfo.Controls.Add(this.labelFoupAFieldLock, 0, 4);
            this.tableFoupAInfo.Controls.Add(this.labelFoupALockValue, 1, 4);
            this.tableFoupAInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableFoupAInfo.Location = new System.Drawing.Point(0, 48);
            this.tableFoupAInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tableFoupAInfo.Name = "tableFoupAInfo";
            this.tableFoupAInfo.RowCount = 5;
            this.tableFoupAInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableFoupAInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableFoupAInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableFoupAInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableFoupAInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableFoupAInfo.Size = new System.Drawing.Size(306, 118);
            this.tableFoupAInfo.TabIndex = 2;
            // 
            // labelFoupAFieldPath
            // 
            this.labelFoupAFieldPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupAFieldPath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelFoupAFieldPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupAFieldPath.Location = new System.Drawing.Point(0, 0);
            this.labelFoupAFieldPath.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupAFieldPath.Name = "labelFoupAFieldPath";
            this.labelFoupAFieldPath.Size = new System.Drawing.Size(70, 24);
            this.labelFoupAFieldPath.TabIndex = 0;
            this.labelFoupAFieldPath.Text = "Path";
            // 
            // labelFoupAPathValue
            // 
            this.labelFoupAPathValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupAPathValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFoupAPathValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupAPathValue.Location = new System.Drawing.Point(70, 0);
            this.labelFoupAPathValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupAPathValue.Name = "labelFoupAPathValue";
            this.labelFoupAPathValue.Size = new System.Drawing.Size(236, 24);
            this.labelFoupAPathValue.TabIndex = 1;
            this.labelFoupAPathValue.Text = "-";
            // 
            // labelFoupAFieldPPID
            // 
            this.labelFoupAFieldPPID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupAFieldPPID.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelFoupAFieldPPID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupAFieldPPID.Location = new System.Drawing.Point(0, 24);
            this.labelFoupAFieldPPID.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupAFieldPPID.Name = "labelFoupAFieldPPID";
            this.labelFoupAFieldPPID.Size = new System.Drawing.Size(70, 24);
            this.labelFoupAFieldPPID.TabIndex = 2;
            this.labelFoupAFieldPPID.Text = "PPID";
            // 
            // labelFoupAPPIDValue
            // 
            this.labelFoupAPPIDValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupAPPIDValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFoupAPPIDValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupAPPIDValue.Location = new System.Drawing.Point(70, 24);
            this.labelFoupAPPIDValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupAPPIDValue.Name = "labelFoupAPPIDValue";
            this.labelFoupAPPIDValue.Size = new System.Drawing.Size(236, 24);
            this.labelFoupAPPIDValue.TabIndex = 3;
            this.labelFoupAPPIDValue.Text = "-";
            // 
            // labelFoupAFieldLotId
            // 
            this.labelFoupAFieldLotId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupAFieldLotId.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelFoupAFieldLotId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupAFieldLotId.Location = new System.Drawing.Point(0, 48);
            this.labelFoupAFieldLotId.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupAFieldLotId.Name = "labelFoupAFieldLotId";
            this.labelFoupAFieldLotId.Size = new System.Drawing.Size(70, 24);
            this.labelFoupAFieldLotId.TabIndex = 4;
            this.labelFoupAFieldLotId.Text = "LOTID";
            // 
            // labelFoupALotIdValue
            // 
            this.labelFoupALotIdValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupALotIdValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFoupALotIdValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupALotIdValue.Location = new System.Drawing.Point(70, 48);
            this.labelFoupALotIdValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupALotIdValue.Name = "labelFoupALotIdValue";
            this.labelFoupALotIdValue.Size = new System.Drawing.Size(236, 24);
            this.labelFoupALotIdValue.TabIndex = 5;
            this.labelFoupALotIdValue.Text = "-";
            // 
            // labelFoupAFieldMid
            // 
            this.labelFoupAFieldMid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupAFieldMid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelFoupAFieldMid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupAFieldMid.Location = new System.Drawing.Point(0, 72);
            this.labelFoupAFieldMid.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupAFieldMid.Name = "labelFoupAFieldMid";
            this.labelFoupAFieldMid.Size = new System.Drawing.Size(70, 24);
            this.labelFoupAFieldMid.TabIndex = 6;
            this.labelFoupAFieldMid.Text = "MID";
            // 
            // labelFoupAMidValue
            // 
            this.labelFoupAMidValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupAMidValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFoupAMidValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupAMidValue.Location = new System.Drawing.Point(70, 72);
            this.labelFoupAMidValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupAMidValue.Name = "labelFoupAMidValue";
            this.labelFoupAMidValue.Size = new System.Drawing.Size(236, 24);
            this.labelFoupAMidValue.TabIndex = 7;
            this.labelFoupAMidValue.Text = "-";
            // 
            // labelFoupAFieldLock
            // 
            this.labelFoupAFieldLock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupAFieldLock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelFoupAFieldLock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupAFieldLock.Location = new System.Drawing.Point(0, 96);
            this.labelFoupAFieldLock.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupAFieldLock.Name = "labelFoupAFieldLock";
            this.labelFoupAFieldLock.Size = new System.Drawing.Size(70, 24);
            this.labelFoupAFieldLock.TabIndex = 8;
            this.labelFoupAFieldLock.Text = "Lock Status";
            // 
            // labelFoupALockValue
            // 
            this.labelFoupALockValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupALockValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFoupALockValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupALockValue.Location = new System.Drawing.Point(70, 96);
            this.labelFoupALockValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupALockValue.Name = "labelFoupALockValue";
            this.labelFoupALockValue.Size = new System.Drawing.Size(236, 24);
            this.labelFoupALockValue.TabIndex = 9;
            this.labelFoupALockValue.Text = "-";
            // 
            // labelFoupAStatusHeadline
            // 
            this.labelFoupAStatusHeadline.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelFoupAStatusHeadline.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelFoupAStatusHeadline.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupAStatusHeadline.Location = new System.Drawing.Point(0, 24);
            this.labelFoupAStatusHeadline.Name = "labelFoupAStatusHeadline";
            this.labelFoupAStatusHeadline.Size = new System.Drawing.Size(306, 24);
            this.labelFoupAStatusHeadline.TabIndex = 1;
            this.labelFoupAStatusHeadline.Text = "Waiting";
            this.labelFoupAStatusHeadline.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFoupInfoATitle
            // 
            this.labelFoupInfoATitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelFoupInfoATitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelFoupInfoATitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupInfoATitle.Location = new System.Drawing.Point(0, 0);
            this.labelFoupInfoATitle.Name = "labelFoupInfoATitle";
            this.labelFoupInfoATitle.Size = new System.Drawing.Size(306, 24);
            this.labelFoupInfoATitle.TabIndex = 0;
            this.labelFoupInfoATitle.Text = "FOUP A";
            this.labelFoupInfoATitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelFoupStatusB
            // 
            this.panelFoupStatusB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.panelFoupStatusB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFoupStatusB.Controls.Add(this.tableFoupBCard);
            this.panelFoupStatusB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFoupStatusB.Location = new System.Drawing.Point(492, 3);
            this.panelFoupStatusB.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.panelFoupStatusB.Name = "panelFoupStatusB";
            this.panelFoupStatusB.Padding = new System.Windows.Forms.Padding(10, 10, 10, 12);
            this.panelFoupStatusB.Size = new System.Drawing.Size(484, 196);
            this.panelFoupStatusB.TabIndex = 1;
            // 
            // tableFoupBCard
            // 
            this.tableFoupBCard.ColumnCount = 2;
            this.tableFoupBCard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableFoupBCard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableFoupBCard.Controls.Add(this.panelFoupBLevelContainer, 0, 0);
            this.tableFoupBCard.Controls.Add(this.panelFoupBDetail, 1, 0);
            this.tableFoupBCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableFoupBCard.Location = new System.Drawing.Point(10, 10);
            this.tableFoupBCard.Name = "tableFoupBCard";
            this.tableFoupBCard.RowCount = 1;
            this.tableFoupBCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableFoupBCard.Size = new System.Drawing.Size(462, 172);
            this.tableFoupBCard.TabIndex = 0;
            // 
            // panelFoupBLevelContainer
            // 
            this.panelFoupBLevelContainer.Controls.Add(this.panelFoupBLevelTrack);
            this.panelFoupBLevelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFoupBLevelContainer.Location = new System.Drawing.Point(3, 3);
            this.panelFoupBLevelContainer.Name = "panelFoupBLevelContainer";
            this.panelFoupBLevelContainer.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.panelFoupBLevelContainer.Size = new System.Drawing.Size(144, 166);
            this.panelFoupBLevelContainer.TabIndex = 0;
            // 
            // panelFoupBLevelTrack
            // 
            this.panelFoupBLevelTrack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.panelFoupBLevelTrack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFoupBLevelTrack.Controls.Add(this.panelFoupBLevelFill);
            this.panelFoupBLevelTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFoupBLevelTrack.Location = new System.Drawing.Point(8, 5);
            this.panelFoupBLevelTrack.Name = "panelFoupBLevelTrack";
            this.panelFoupBLevelTrack.Size = new System.Drawing.Size(128, 156);
            this.panelFoupBLevelTrack.TabIndex = 0;
            // 
            // panelFoupBLevelFill
            // 
            this.panelFoupBLevelFill.BackColor = System.Drawing.Color.MediumBlue;
            this.panelFoupBLevelFill.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFoupBLevelFill.Location = new System.Drawing.Point(0, 154);
            this.panelFoupBLevelFill.Name = "panelFoupBLevelFill";
            this.panelFoupBLevelFill.Size = new System.Drawing.Size(126, 0);
            this.panelFoupBLevelFill.TabIndex = 0;
            // 
            // panelFoupBDetail
            // 
            this.panelFoupBDetail.Controls.Add(this.tableFoupBInfo);
            this.panelFoupBDetail.Controls.Add(this.labelFoupBStatusHeadline);
            this.panelFoupBDetail.Controls.Add(this.labelFoupInfoBTitle);
            this.panelFoupBDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFoupBDetail.Location = new System.Drawing.Point(153, 3);
            this.panelFoupBDetail.Name = "panelFoupBDetail";
            this.panelFoupBDetail.Size = new System.Drawing.Size(306, 166);
            this.panelFoupBDetail.TabIndex = 1;
            // 
            // tableFoupBInfo
            // 
            this.tableFoupBInfo.ColumnCount = 2;
            this.tableFoupBInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableFoupBInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableFoupBInfo.Controls.Add(this.labelFoupBFieldPath, 0, 0);
            this.tableFoupBInfo.Controls.Add(this.labelFoupBPathValue, 1, 0);
            this.tableFoupBInfo.Controls.Add(this.labelFoupBFieldPPID, 0, 1);
            this.tableFoupBInfo.Controls.Add(this.labelFoupBPPIDValue, 1, 1);
            this.tableFoupBInfo.Controls.Add(this.labelFoupBFieldLotId, 0, 2);
            this.tableFoupBInfo.Controls.Add(this.labelFoupBLotIdValue, 1, 2);
            this.tableFoupBInfo.Controls.Add(this.labelFoupBFieldMid, 0, 3);
            this.tableFoupBInfo.Controls.Add(this.labelFoupBMidValue, 1, 3);
            this.tableFoupBInfo.Controls.Add(this.labelFoupBFieldLock, 0, 4);
            this.tableFoupBInfo.Controls.Add(this.labelFoupBLockValue, 1, 4);
            this.tableFoupBInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableFoupBInfo.Location = new System.Drawing.Point(0, 48);
            this.tableFoupBInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tableFoupBInfo.Name = "tableFoupBInfo";
            this.tableFoupBInfo.RowCount = 5;
            this.tableFoupBInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableFoupBInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableFoupBInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableFoupBInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableFoupBInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableFoupBInfo.Size = new System.Drawing.Size(306, 118);
            this.tableFoupBInfo.TabIndex = 2;
            // 
            // labelFoupBFieldPath
            // 
            this.labelFoupBFieldPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupBFieldPath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelFoupBFieldPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupBFieldPath.Location = new System.Drawing.Point(0, 0);
            this.labelFoupBFieldPath.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupBFieldPath.Name = "labelFoupBFieldPath";
            this.labelFoupBFieldPath.Size = new System.Drawing.Size(70, 24);
            this.labelFoupBFieldPath.TabIndex = 0;
            this.labelFoupBFieldPath.Text = "Path";
            // 
            // labelFoupBPathValue
            // 
            this.labelFoupBPathValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupBPathValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFoupBPathValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupBPathValue.Location = new System.Drawing.Point(70, 0);
            this.labelFoupBPathValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupBPathValue.Name = "labelFoupBPathValue";
            this.labelFoupBPathValue.Size = new System.Drawing.Size(236, 24);
            this.labelFoupBPathValue.TabIndex = 1;
            this.labelFoupBPathValue.Text = "-";
            // 
            // labelFoupBFieldPPID
            // 
            this.labelFoupBFieldPPID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupBFieldPPID.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelFoupBFieldPPID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupBFieldPPID.Location = new System.Drawing.Point(0, 24);
            this.labelFoupBFieldPPID.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupBFieldPPID.Name = "labelFoupBFieldPPID";
            this.labelFoupBFieldPPID.Size = new System.Drawing.Size(70, 24);
            this.labelFoupBFieldPPID.TabIndex = 2;
            this.labelFoupBFieldPPID.Text = "PPID";
            // 
            // labelFoupBPPIDValue
            // 
            this.labelFoupBPPIDValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupBPPIDValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFoupBPPIDValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupBPPIDValue.Location = new System.Drawing.Point(70, 24);
            this.labelFoupBPPIDValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupBPPIDValue.Name = "labelFoupBPPIDValue";
            this.labelFoupBPPIDValue.Size = new System.Drawing.Size(236, 24);
            this.labelFoupBPPIDValue.TabIndex = 3;
            this.labelFoupBPPIDValue.Text = "-";
            // 
            // labelFoupBFieldLotId
            // 
            this.labelFoupBFieldLotId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupBFieldLotId.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelFoupBFieldLotId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupBFieldLotId.Location = new System.Drawing.Point(0, 48);
            this.labelFoupBFieldLotId.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupBFieldLotId.Name = "labelFoupBFieldLotId";
            this.labelFoupBFieldLotId.Size = new System.Drawing.Size(70, 24);
            this.labelFoupBFieldLotId.TabIndex = 4;
            this.labelFoupBFieldLotId.Text = "LOTID";
            // 
            // labelFoupBLotIdValue
            // 
            this.labelFoupBLotIdValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupBLotIdValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFoupBLotIdValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupBLotIdValue.Location = new System.Drawing.Point(70, 48);
            this.labelFoupBLotIdValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupBLotIdValue.Name = "labelFoupBLotIdValue";
            this.labelFoupBLotIdValue.Size = new System.Drawing.Size(236, 24);
            this.labelFoupBLotIdValue.TabIndex = 5;
            this.labelFoupBLotIdValue.Text = "-";
            // 
            // labelFoupBFieldMid
            // 
            this.labelFoupBFieldMid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupBFieldMid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelFoupBFieldMid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupBFieldMid.Location = new System.Drawing.Point(0, 72);
            this.labelFoupBFieldMid.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupBFieldMid.Name = "labelFoupBFieldMid";
            this.labelFoupBFieldMid.Size = new System.Drawing.Size(70, 24);
            this.labelFoupBFieldMid.TabIndex = 6;
            this.labelFoupBFieldMid.Text = "MID";
            // 
            // labelFoupBMidValue
            // 
            this.labelFoupBMidValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupBMidValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFoupBMidValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupBMidValue.Location = new System.Drawing.Point(70, 72);
            this.labelFoupBMidValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupBMidValue.Name = "labelFoupBMidValue";
            this.labelFoupBMidValue.Size = new System.Drawing.Size(236, 24);
            this.labelFoupBMidValue.TabIndex = 7;
            this.labelFoupBMidValue.Text = "-";
            // 
            // labelFoupBFieldLock
            // 
            this.labelFoupBFieldLock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupBFieldLock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelFoupBFieldLock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupBFieldLock.Location = new System.Drawing.Point(0, 96);
            this.labelFoupBFieldLock.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupBFieldLock.Name = "labelFoupBFieldLock";
            this.labelFoupBFieldLock.Size = new System.Drawing.Size(70, 24);
            this.labelFoupBFieldLock.TabIndex = 8;
            this.labelFoupBFieldLock.Text = "Lock Status";
            // 
            // labelFoupBLockValue
            // 
            this.labelFoupBLockValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupBLockValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFoupBLockValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupBLockValue.Location = new System.Drawing.Point(70, 96);
            this.labelFoupBLockValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelFoupBLockValue.Name = "labelFoupBLockValue";
            this.labelFoupBLockValue.Size = new System.Drawing.Size(236, 24);
            this.labelFoupBLockValue.TabIndex = 9;
            this.labelFoupBLockValue.Text = "-";
            // 
            // labelFoupBStatusHeadline
            // 
            this.labelFoupBStatusHeadline.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelFoupBStatusHeadline.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelFoupBStatusHeadline.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelFoupBStatusHeadline.Location = new System.Drawing.Point(0, 24);
            this.labelFoupBStatusHeadline.Name = "labelFoupBStatusHeadline";
            this.labelFoupBStatusHeadline.Size = new System.Drawing.Size(306, 24);
            this.labelFoupBStatusHeadline.TabIndex = 1;
            this.labelFoupBStatusHeadline.Text = "Waiting";
            this.labelFoupBStatusHeadline.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFoupInfoBTitle
            // 
            this.labelFoupInfoBTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelFoupInfoBTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelFoupInfoBTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupInfoBTitle.Location = new System.Drawing.Point(0, 0);
            this.labelFoupInfoBTitle.Name = "labelFoupInfoBTitle";
            this.labelFoupInfoBTitle.Size = new System.Drawing.Size(306, 24);
            this.labelFoupInfoBTitle.TabIndex = 0;
            this.labelFoupInfoBTitle.Text = "FOUP B";
            this.labelFoupInfoBTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFoupSummaryInfo
            // 
            this.labelFoupSummaryInfo.AutoSize = true;
            this.tableProcessMetrics.SetColumnSpan(this.labelFoupSummaryInfo, 2);
            this.labelFoupSummaryInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFoupSummaryInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelFoupSummaryInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelFoupSummaryInfo.Location = new System.Drawing.Point(3, 202);
            this.labelFoupSummaryInfo.Name = "labelFoupSummaryInfo";
            this.labelFoupSummaryInfo.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.labelFoupSummaryInfo.Size = new System.Drawing.Size(978, 32);
            this.labelFoupSummaryInfo.TabIndex = 2;
            this.labelFoupSummaryInfo.Text = "FOUP 정보";
            this.labelFoupSummaryInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelControlPanel
            // 
            this.panelControlPanel.AutoScroll = true;
            this.panelControlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
            this.panelControlPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelControlPanel.Controls.Add(this.flowControlPanelStack);
            this.panelControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlPanel.Location = new System.Drawing.Point(1378, 3);
            this.panelControlPanel.Name = "panelControlPanel";
            this.panelControlPanel.Padding = new System.Windows.Forms.Padding(20, 20, 20, 30);
            this.panelControlPanel.Size = new System.Drawing.Size(314, 724);
            this.panelControlPanel.TabIndex = 2;
            // 
            // flowControlPanelStack
            // 
            this.flowControlPanelStack.AutoScroll = true;
            this.flowControlPanelStack.Controls.Add(this.labelControlTitle);
            this.flowControlPanelStack.Controls.Add(this.groupBoxControlButtons);
            this.flowControlPanelStack.Controls.Add(this.groupBoxFoupReady);
            this.flowControlPanelStack.Controls.Add(this.groupBoxRecipe);
            this.flowControlPanelStack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowControlPanelStack.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowControlPanelStack.Location = new System.Drawing.Point(20, 20);
            this.flowControlPanelStack.Margin = new System.Windows.Forms.Padding(0);
            this.flowControlPanelStack.Name = "flowControlPanelStack";
            this.flowControlPanelStack.Padding = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.flowControlPanelStack.Size = new System.Drawing.Size(272, 672);
            this.flowControlPanelStack.TabIndex = 0;
            this.flowControlPanelStack.WrapContents = false;
            // 
            // labelControlTitle
            // 
            this.labelControlTitle.AutoSize = true;
            this.labelControlTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelControlTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelControlTitle.Location = new System.Drawing.Point(3, 0);
            this.labelControlTitle.Margin = new System.Windows.Forms.Padding(3, 0, 3, 12);
            this.labelControlTitle.Name = "labelControlTitle";
            this.labelControlTitle.Size = new System.Drawing.Size(136, 25);
            this.labelControlTitle.TabIndex = 0;
            this.labelControlTitle.Text = "제어 패널 영역";
            // 
            // groupBoxControlButtons
            // 
            this.groupBoxControlButtons.AutoSize = true;
            this.groupBoxControlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxControlButtons.Controls.Add(this.flowLayoutControlButtons);
            this.groupBoxControlButtons.ForeColor = System.Drawing.Color.Black;
            this.groupBoxControlButtons.Location = new System.Drawing.Point(0, 49);
            this.groupBoxControlButtons.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBoxControlButtons.Name = "groupBoxControlButtons";
            this.groupBoxControlButtons.Padding = new System.Windows.Forms.Padding(10, 12, 10, 10);
            this.groupBoxControlButtons.Size = new System.Drawing.Size(256, 364);
            this.groupBoxControlButtons.TabIndex = 1;
            this.groupBoxControlButtons.TabStop = false;
            this.groupBoxControlButtons.Text = "제어 패널";
            // 
            // flowLayoutControlButtons
            // 
            this.flowLayoutControlButtons.AutoSize = true;
            this.flowLayoutControlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutControlButtons.Controls.Add(this.buttonStart);
            this.flowLayoutControlButtons.Controls.Add(this.buttonPause);
            this.flowLayoutControlButtons.Controls.Add(this.buttonStop);
            this.flowLayoutControlButtons.Controls.Add(this.buttonResetAlarm);
            this.flowLayoutControlButtons.Controls.Add(this.buttonResetProcess);
            this.flowLayoutControlButtons.Controls.Add(this.buttonEquipmentControl);
            this.flowLayoutControlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutControlButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutControlButtons.Location = new System.Drawing.Point(10, 28);
            this.flowLayoutControlButtons.Name = "flowLayoutControlButtons";
            this.flowLayoutControlButtons.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.flowLayoutControlButtons.Size = new System.Drawing.Size(236, 326);
            this.flowLayoutControlButtons.TabIndex = 0;
            this.flowLayoutControlButtons.WrapContents = false;
            // 
            // buttonStart
            // 
            this.buttonStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonStart.FlatAppearance.BorderSize = 0;
            this.buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStart.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.buttonStart.ForeColor = System.Drawing.Color.White;
            this.buttonStart.Location = new System.Drawing.Point(0, 8);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(0, 4, 0, 10);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(236, 44);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "공정 시작";
            this.buttonStart.UseVisualStyleBackColor = false;
            // 
            // buttonPause
            // 
            this.buttonPause.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonPause.FlatAppearance.BorderSize = 0;
            this.buttonPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPause.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.buttonPause.ForeColor = System.Drawing.Color.White;
            this.buttonPause.Location = new System.Drawing.Point(0, 62);
            this.buttonPause.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(236, 44);
            this.buttonPause.TabIndex = 1;
            this.buttonPause.Text = "일시 정지";
            this.buttonPause.UseVisualStyleBackColor = false;
            // 
            // buttonStop
            // 
            this.buttonStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonStop.FlatAppearance.BorderSize = 0;
            this.buttonStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStop.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.buttonStop.ForeColor = System.Drawing.Color.White;
            this.buttonStop.Location = new System.Drawing.Point(0, 116);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(236, 44);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "긴급 정지";
            this.buttonStop.UseVisualStyleBackColor = false;
            // 
            // buttonResetAlarm
            // 
            this.buttonResetAlarm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonResetAlarm.FlatAppearance.BorderSize = 0;
            this.buttonResetAlarm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonResetAlarm.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.buttonResetAlarm.ForeColor = System.Drawing.Color.White;
            this.buttonResetAlarm.Location = new System.Drawing.Point(0, 170);
            this.buttonResetAlarm.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.buttonResetAlarm.Name = "buttonResetAlarm";
            this.buttonResetAlarm.Size = new System.Drawing.Size(236, 44);
            this.buttonResetAlarm.TabIndex = 3;
            this.buttonResetAlarm.Text = "알람 리셋";
            this.buttonResetAlarm.UseVisualStyleBackColor = false;
            // 
            // buttonResetProcess
            // 
            this.buttonResetProcess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonResetProcess.FlatAppearance.BorderSize = 0;
            this.buttonResetProcess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonResetProcess.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.buttonResetProcess.ForeColor = System.Drawing.Color.White;
            this.buttonResetProcess.Location = new System.Drawing.Point(0, 224);
            this.buttonResetProcess.Margin = new System.Windows.Forms.Padding(0);
            this.buttonResetProcess.Name = "buttonResetProcess";
            this.buttonResetProcess.Size = new System.Drawing.Size(236, 44);
            this.buttonResetProcess.TabIndex = 4;
            this.buttonResetProcess.Text = "공정 리셋";
            this.buttonResetProcess.UseVisualStyleBackColor = false;
            // 
            // buttonEquipmentControl
            // 
            this.buttonEquipmentControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonEquipmentControl.FlatAppearance.BorderSize = 0;
            this.buttonEquipmentControl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEquipmentControl.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.buttonEquipmentControl.ForeColor = System.Drawing.Color.White;
            this.buttonEquipmentControl.Location = new System.Drawing.Point(0, 278);
            this.buttonEquipmentControl.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.buttonEquipmentControl.Name = "buttonEquipmentControl";
            this.buttonEquipmentControl.Size = new System.Drawing.Size(236, 44);
            this.buttonEquipmentControl.TabIndex = 5;
            this.buttonEquipmentControl.Text = "장비 제어";
            this.buttonEquipmentControl.UseVisualStyleBackColor = false;
            // 
            // groupBoxFoupReady
            // 
            this.groupBoxFoupReady.AutoSize = true;
            this.groupBoxFoupReady.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxFoupReady.Controls.Add(this.flowLayoutFoupReadyButtons);
            this.groupBoxFoupReady.ForeColor = System.Drawing.Color.Black;
            this.groupBoxFoupReady.Location = new System.Drawing.Point(0, 425);
            this.groupBoxFoupReady.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBoxFoupReady.Name = "groupBoxFoupReady";
            this.groupBoxFoupReady.Padding = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.groupBoxFoupReady.Size = new System.Drawing.Size(256, 192);
            this.groupBoxFoupReady.TabIndex = 2;
            this.groupBoxFoupReady.TabStop = false;
            this.groupBoxFoupReady.Text = "FOUP 장착 / 웨이퍼 로드 상태";
            // 
            // flowLayoutFoupReadyButtons
            // 
            this.flowLayoutFoupReadyButtons.AutoSize = true;
            this.flowLayoutFoupReadyButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutFoupReadyButtons.Controls.Add(this.buttonToggleFoupMount);
            this.flowLayoutFoupReadyButtons.Controls.Add(this.buttonWaferLoading);
            this.flowLayoutFoupReadyButtons.Controls.Add(this.buttonWaferUnloading);
            this.flowLayoutFoupReadyButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutFoupReadyButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutFoupReadyButtons.Location = new System.Drawing.Point(10, 28);
            this.flowLayoutFoupReadyButtons.Name = "flowLayoutFoupReadyButtons";
            this.flowLayoutFoupReadyButtons.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.flowLayoutFoupReadyButtons.Size = new System.Drawing.Size(236, 152);
            this.flowLayoutFoupReadyButtons.TabIndex = 0;
            this.flowLayoutFoupReadyButtons.WrapContents = false;
            // 
            // buttonToggleFoupMount
            // 
            this.buttonToggleFoupMount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonToggleFoupMount.FlatAppearance.BorderSize = 0;
            this.buttonToggleFoupMount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonToggleFoupMount.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonToggleFoupMount.ForeColor = System.Drawing.Color.White;
            this.buttonToggleFoupMount.Location = new System.Drawing.Point(0, 8);
            this.buttonToggleFoupMount.Margin = new System.Windows.Forms.Padding(0, 4, 0, 10);
            this.buttonToggleFoupMount.Name = "buttonToggleFoupMount";
            this.buttonToggleFoupMount.Size = new System.Drawing.Size(236, 40);
            this.buttonToggleFoupMount.TabIndex = 0;
            this.buttonToggleFoupMount.Text = "FOUP 미장착";
            this.buttonToggleFoupMount.UseVisualStyleBackColor = false;
            // 
            // buttonWaferLoading
            // 
            this.buttonWaferLoading.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonWaferLoading.FlatAppearance.BorderSize = 0;
            this.buttonWaferLoading.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWaferLoading.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonWaferLoading.ForeColor = System.Drawing.Color.White;
            this.buttonWaferLoading.Location = new System.Drawing.Point(0, 58);
            this.buttonWaferLoading.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.buttonWaferLoading.Name = "buttonWaferLoading";
            this.buttonWaferLoading.Size = new System.Drawing.Size(236, 40);
            this.buttonWaferLoading.TabIndex = 1;
            this.buttonWaferLoading.Text = "웨이퍼 로딩 대기";
            this.buttonWaferLoading.UseVisualStyleBackColor = false;
            // 
            // buttonWaferUnloading
            // 
            this.buttonWaferUnloading.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonWaferUnloading.FlatAppearance.BorderSize = 0;
            this.buttonWaferUnloading.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWaferUnloading.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonWaferUnloading.ForeColor = System.Drawing.Color.White;
            this.buttonWaferUnloading.Location = new System.Drawing.Point(0, 108);
            this.buttonWaferUnloading.Margin = new System.Windows.Forms.Padding(0);
            this.buttonWaferUnloading.Name = "buttonWaferUnloading";
            this.buttonWaferUnloading.Size = new System.Drawing.Size(236, 40);
            this.buttonWaferUnloading.TabIndex = 2;
            this.buttonWaferUnloading.Text = "웨이퍼 언로딩 대기";
            this.buttonWaferUnloading.UseVisualStyleBackColor = false;
            // 
            // groupBoxRecipe
            // 
            this.groupBoxRecipe.AutoSize = true;
            this.groupBoxRecipe.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxRecipe.Controls.Add(this.buttonApplyRecipe);
            this.groupBoxRecipe.Controls.Add(this.comboRecipeSelect);
            this.groupBoxRecipe.Controls.Add(this.labelRecipe);
            this.groupBoxRecipe.ForeColor = System.Drawing.Color.Black;
            this.groupBoxRecipe.Location = new System.Drawing.Point(0, 629);
            this.groupBoxRecipe.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.groupBoxRecipe.Name = "groupBoxRecipe";
            this.groupBoxRecipe.Padding = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.groupBoxRecipe.Size = new System.Drawing.Size(261, 163);
            this.groupBoxRecipe.TabIndex = 3;
            this.groupBoxRecipe.TabStop = false;
            this.groupBoxRecipe.Text = "레시피 및 작업 설정";
            // 
            // buttonApplyRecipe
            // 
            this.buttonApplyRecipe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonApplyRecipe.FlatAppearance.BorderSize = 0;
            this.buttonApplyRecipe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonApplyRecipe.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonApplyRecipe.ForeColor = System.Drawing.Color.White;
            this.buttonApplyRecipe.Location = new System.Drawing.Point(12, 96);
            this.buttonApplyRecipe.Name = "buttonApplyRecipe";
            this.buttonApplyRecipe.Size = new System.Drawing.Size(236, 36);
            this.buttonApplyRecipe.TabIndex = 2;
            this.buttonApplyRecipe.Text = "레시피 적용";
            this.buttonApplyRecipe.UseVisualStyleBackColor = false;
            // 
            // comboRecipeSelect
            // 
            this.comboRecipeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRecipeSelect.FormattingEnabled = true;
            this.comboRecipeSelect.Items.AddRange(new object[] {
            "PR_STD_PIPE / B(1차)+C(병렬) / 3장",
            "PR_HIGH_PIPE / B(1차)+C(병렬) / 5장",
            "PR_SINGLE_PIPE / B(1차)+C(병렬) / 1장",
            "PR_DOUBLE_EXPO / B(1차)+C(2차) / 5장",
            "PR_DOUBLE_EXPO / B(1차)+C(2차) / 3장",
            "PR_DOUBLE_EXPO / B(1차)+C(2차) / 1장"});
            this.comboRecipeSelect.Location = new System.Drawing.Point(12, 64);
            this.comboRecipeSelect.Name = "comboRecipeSelect";
            this.comboRecipeSelect.Size = new System.Drawing.Size(236, 23);
            this.comboRecipeSelect.TabIndex = 1;
            // 
            // labelRecipe
            // 
            this.labelRecipe.AutoSize = true;
            this.labelRecipe.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelRecipe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelRecipe.Location = new System.Drawing.Point(12, 32);
            this.labelRecipe.Name = "labelRecipe";
            this.labelRecipe.Size = new System.Drawing.Size(151, 19);
            this.labelRecipe.TabIndex = 0;
            this.labelRecipe.Text = "현재 선택 레시피/작업:";
            // 
            // panelAlarmArea
            // 
            this.panelAlarmArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(190)))));
            this.panelAlarmArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelAlarmArea.Controls.Add(this.flowBottomNavigation);
            this.panelAlarmArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAlarmArea.Location = new System.Drawing.Point(3, 909);
            this.panelAlarmArea.Name = "panelAlarmArea";
            this.panelAlarmArea.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.panelAlarmArea.Size = new System.Drawing.Size(1695, 64);
            this.panelAlarmArea.TabIndex = 2;
            // 
            // flowBottomNavigation
            // 
            this.flowBottomNavigation.Controls.Add(this.buttonNavOperate);
            this.flowBottomNavigation.Controls.Add(this.buttonNavRecipe);
            this.flowBottomNavigation.Controls.Add(this.buttonNavMaintenance);
            this.flowBottomNavigation.Controls.Add(this.buttonNavConfig);
            this.flowBottomNavigation.Controls.Add(this.buttonNavTrend);
            this.flowBottomNavigation.Controls.Add(this.buttonNavReport);
            this.flowBottomNavigation.Controls.Add(this.buttonNavSystem);
            this.flowBottomNavigation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowBottomNavigation.Location = new System.Drawing.Point(20, 10);
            this.flowBottomNavigation.Name = "flowBottomNavigation";
            this.flowBottomNavigation.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.flowBottomNavigation.Size = new System.Drawing.Size(1653, 42);
            this.flowBottomNavigation.TabIndex = 1;
            this.flowBottomNavigation.WrapContents = false;
            // 
            // buttonNavOperate
            // 
            this.buttonNavOperate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonNavOperate.FlatAppearance.BorderSize = 0;
            this.buttonNavOperate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNavOperate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonNavOperate.ForeColor = System.Drawing.Color.White;
            this.buttonNavOperate.Location = new System.Drawing.Point(3, 8);
            this.buttonNavOperate.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.buttonNavOperate.Name = "buttonNavOperate";
            this.buttonNavOperate.Size = new System.Drawing.Size(120, 32);
            this.buttonNavOperate.TabIndex = 0;
            this.buttonNavOperate.Text = "OPERATE";
            this.buttonNavOperate.UseVisualStyleBackColor = false;
            // 
            // buttonNavRecipe
            // 
            this.buttonNavRecipe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonNavRecipe.FlatAppearance.BorderSize = 0;
            this.buttonNavRecipe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNavRecipe.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonNavRecipe.ForeColor = System.Drawing.Color.White;
            this.buttonNavRecipe.Location = new System.Drawing.Point(136, 8);
            this.buttonNavRecipe.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.buttonNavRecipe.Name = "buttonNavRecipe";
            this.buttonNavRecipe.Size = new System.Drawing.Size(120, 32);
            this.buttonNavRecipe.TabIndex = 1;
            this.buttonNavRecipe.Text = "RECIPE";
            this.buttonNavRecipe.UseVisualStyleBackColor = false;
            // 
            // buttonNavMaintenance
            // 
            this.buttonNavMaintenance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonNavMaintenance.FlatAppearance.BorderSize = 0;
            this.buttonNavMaintenance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNavMaintenance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonNavMaintenance.ForeColor = System.Drawing.Color.White;
            this.buttonNavMaintenance.Location = new System.Drawing.Point(269, 8);
            this.buttonNavMaintenance.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.buttonNavMaintenance.Name = "buttonNavMaintenance";
            this.buttonNavMaintenance.Size = new System.Drawing.Size(120, 32);
            this.buttonNavMaintenance.TabIndex = 2;
            this.buttonNavMaintenance.Text = "MAINT";
            this.buttonNavMaintenance.UseVisualStyleBackColor = false;
            // 
            // buttonNavConfig
            // 
            this.buttonNavConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonNavConfig.FlatAppearance.BorderSize = 0;
            this.buttonNavConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNavConfig.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonNavConfig.ForeColor = System.Drawing.Color.White;
            this.buttonNavConfig.Location = new System.Drawing.Point(402, 8);
            this.buttonNavConfig.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.buttonNavConfig.Name = "buttonNavConfig";
            this.buttonNavConfig.Size = new System.Drawing.Size(120, 32);
            this.buttonNavConfig.TabIndex = 3;
            this.buttonNavConfig.Text = "CONFIG";
            this.buttonNavConfig.UseVisualStyleBackColor = false;
            // 
            // buttonNavTrend
            // 
            this.buttonNavTrend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonNavTrend.FlatAppearance.BorderSize = 0;
            this.buttonNavTrend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNavTrend.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonNavTrend.ForeColor = System.Drawing.Color.White;
            this.buttonNavTrend.Location = new System.Drawing.Point(535, 8);
            this.buttonNavTrend.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.buttonNavTrend.Name = "buttonNavTrend";
            this.buttonNavTrend.Size = new System.Drawing.Size(120, 32);
            this.buttonNavTrend.TabIndex = 5;
            this.buttonNavTrend.Text = "TREND";
            this.buttonNavTrend.UseVisualStyleBackColor = false;
            // 
            // buttonNavReport
            // 
            this.buttonNavReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonNavReport.FlatAppearance.BorderSize = 0;
            this.buttonNavReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNavReport.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonNavReport.ForeColor = System.Drawing.Color.White;
            this.buttonNavReport.Location = new System.Drawing.Point(668, 8);
            this.buttonNavReport.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.buttonNavReport.Name = "buttonNavReport";
            this.buttonNavReport.Size = new System.Drawing.Size(120, 32);
            this.buttonNavReport.TabIndex = 6;
            this.buttonNavReport.Text = "REPORT";
            this.buttonNavReport.UseVisualStyleBackColor = false;
            // 
            // buttonNavSystem
            // 
            this.buttonNavSystem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(120)))), ((int)(((byte)(130)))));
            this.buttonNavSystem.FlatAppearance.BorderSize = 0;
            this.buttonNavSystem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNavSystem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonNavSystem.ForeColor = System.Drawing.Color.White;
            this.buttonNavSystem.Location = new System.Drawing.Point(801, 8);
            this.buttonNavSystem.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.buttonNavSystem.Name = "buttonNavSystem";
            this.buttonNavSystem.Size = new System.Drawing.Size(120, 32);
            this.buttonNavSystem.TabIndex = 7;
            this.buttonNavSystem.Text = "SYS INFO";
            this.buttonNavSystem.UseVisualStyleBackColor = false;
            // 
            // buttonServoHome
            // 
            this.buttonServoHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.buttonServoHome.FlatAppearance.BorderSize = 0;
            this.buttonServoHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonServoHome.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonServoHome.ForeColor = System.Drawing.Color.White;
            this.buttonServoHome.Location = new System.Drawing.Point(652, 3);
            this.buttonServoHome.Margin = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.buttonServoHome.Name = "buttonServoHome";
            this.buttonServoHome.Size = new System.Drawing.Size(80, 26);
            this.buttonServoHome.TabIndex = 15;
            this.buttonServoHome.Text = "원점복귀";
            this.buttonServoHome.UseVisualStyleBackColor = false;
            // 
            // panelMainLamp
            // 
            this.panelMainLamp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMainLamp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.panelMainLamp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMainLamp.Controls.Add(this.labelMainLampRed);
            this.panelMainLamp.Controls.Add(this.labelMainLampYellow);
            this.panelMainLamp.Controls.Add(this.labelMainLampGreen);
            this.panelMainLamp.Controls.Add(this.panelMainLampRed);
            this.panelMainLamp.Controls.Add(this.panelMainLampYellow);
            this.panelMainLamp.Controls.Add(this.panelMainLampGreen);
            this.panelMainLamp.Controls.Add(this.labelMainLamp);
            this.panelMainLamp.Location = new System.Drawing.Point(529, 16);
            this.panelMainLamp.Margin = new System.Windows.Forms.Padding(6, 18, 12, 6);
            this.panelMainLamp.MinimumSize = new System.Drawing.Size(130, 90);
            this.panelMainLamp.Name = "panelMainLamp";
            this.panelMainLamp.Padding = new System.Windows.Forms.Padding(10, 10, 10, 8);
            this.panelMainLamp.Size = new System.Drawing.Size(188, 90);
            this.panelMainLamp.TabIndex = 0;
            // 
            // labelMainLampRed
            // 
            this.labelMainLampRed.AutoSize = true;
            this.labelMainLampRed.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelMainLampRed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelMainLampRed.Location = new System.Drawing.Point(56, 19);
            this.labelMainLampRed.Name = "labelMainLampRed";
            this.labelMainLampRed.Size = new System.Drawing.Size(117, 15);
            this.labelMainLampRed.TabIndex = 6;
            this.labelMainLampRed.Text = "적색: 공정/장비 오류";
            // 
            // labelMainLampYellow
            // 
            this.labelMainLampYellow.AutoSize = true;
            this.labelMainLampYellow.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelMainLampYellow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelMainLampYellow.Location = new System.Drawing.Point(56, 43);
            this.labelMainLampYellow.Name = "labelMainLampYellow";
            this.labelMainLampYellow.Size = new System.Drawing.Size(88, 15);
            this.labelMainLampYellow.TabIndex = 5;
            this.labelMainLampYellow.Text = "황색: 공정 대기";
            // 
            // labelMainLampGreen
            // 
            this.labelMainLampGreen.AutoSize = true;
            this.labelMainLampGreen.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelMainLampGreen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelMainLampGreen.Location = new System.Drawing.Point(56, 67);
            this.labelMainLampGreen.Name = "labelMainLampGreen";
            this.labelMainLampGreen.Size = new System.Drawing.Size(88, 15);
            this.labelMainLampGreen.TabIndex = 4;
            this.labelMainLampGreen.Text = "녹색: 공정 진행";
            // 
            // panelMainLampRed
            // 
            this.panelMainLampRed.BackColor = System.Drawing.Color.Firebrick;
            this.panelMainLampRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMainLampRed.Location = new System.Drawing.Point(18, 15);
            this.panelMainLampRed.Name = "panelMainLampRed";
            this.panelMainLampRed.Size = new System.Drawing.Size(24, 24);
            this.panelMainLampRed.TabIndex = 3;
            // 
            // panelMainLampYellow
            // 
            this.panelMainLampYellow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.panelMainLampYellow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMainLampYellow.Location = new System.Drawing.Point(18, 39);
            this.panelMainLampYellow.Name = "panelMainLampYellow";
            this.panelMainLampYellow.Size = new System.Drawing.Size(24, 24);
            this.panelMainLampYellow.TabIndex = 2;
            // 
            // panelMainLampGreen
            // 
            this.panelMainLampGreen.BackColor = System.Drawing.Color.ForestGreen;
            this.panelMainLampGreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMainLampGreen.Location = new System.Drawing.Point(18, 63);
            this.panelMainLampGreen.Name = "panelMainLampGreen";
            this.panelMainLampGreen.Size = new System.Drawing.Size(24, 24);
            this.panelMainLampGreen.TabIndex = 1;
            // 
            // labelMainLamp
            // 
            this.labelMainLamp.AutoSize = true;
            this.labelMainLamp.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.labelMainLamp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelMainLamp.Location = new System.Drawing.Point(12, -1);
            this.labelMainLamp.Name = "labelMainLamp";
            this.labelMainLamp.Size = new System.Drawing.Size(107, 20);
            this.labelMainLamp.TabIndex = 0;
            this.labelMainLamp.Text = "장비 상태 램프";
            // 
            // panelFoupB
            // 
            this.panelFoupB.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelFoupB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(66)))), ((int)(((byte)(73)))));
            this.panelFoupB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFoupB.Location = new System.Drawing.Point(422, 185);
            this.panelFoupB.Margin = new System.Windows.Forms.Padding(0);
            this.panelFoupB.MinimumSize = new System.Drawing.Size(160, 110);
            this.panelFoupB.Name = "panelFoupB";
            this.panelFoupB.Padding = new System.Windows.Forms.Padding(12);
            this.panelFoupB.Size = new System.Drawing.Size(200, 110);
            this.panelFoupB.TabIndex = 6;
            // 
            // panelFoupA
            // 
            this.panelFoupA.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelFoupA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(66)))), ((int)(((byte)(73)))));
            this.panelFoupA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFoupA.Location = new System.Drawing.Point(95, 185);
            this.panelFoupA.Margin = new System.Windows.Forms.Padding(0);
            this.panelFoupA.MinimumSize = new System.Drawing.Size(160, 110);
            this.panelFoupA.Name = "panelFoupA";
            this.panelFoupA.Padding = new System.Windows.Forms.Padding(12);
            this.panelFoupA.Size = new System.Drawing.Size(200, 110);
            this.panelFoupA.TabIndex = 5;
            // 
            // panelChamberC
            // 
            this.panelChamberC.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelChamberC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(66)))), ((int)(((byte)(73)))));
            this.panelChamberC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelChamberC.Controls.Add(this.panelDoorChamberC);
            this.panelChamberC.Controls.Add(this.panelLampChamberC);
            this.panelChamberC.Controls.Add(this.labelChamberC);
            this.panelChamberC.Location = new System.Drawing.Point(475, 87);
            this.panelChamberC.Margin = new System.Windows.Forms.Padding(12);
            this.panelChamberC.MinimumSize = new System.Drawing.Size(150, 110);
            this.panelChamberC.Name = "panelChamberC";
            this.panelChamberC.Padding = new System.Windows.Forms.Padding(14);
            this.panelChamberC.Size = new System.Drawing.Size(205, 110);
            this.panelChamberC.TabIndex = 4;
            // 
            // panelDoorChamberC
            // 
            this.panelDoorChamberC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(94)))), ((int)(((byte)(102)))));
            this.panelDoorChamberC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDoorChamberC.Controls.Add(this.panelWaferChamberC);
            this.panelDoorChamberC.Location = new System.Drawing.Point(14, 50);
            this.panelDoorChamberC.Name = "panelDoorChamberC";
            this.panelDoorChamberC.Size = new System.Drawing.Size(168, 58);
            this.panelDoorChamberC.TabIndex = 6;
            // 
            // panelWaferChamberC
            // 
            this.panelWaferChamberC.BackColor = System.Drawing.Color.Transparent;
            this.panelWaferChamberC.Location = new System.Drawing.Point(60, 10);
            this.panelWaferChamberC.Name = "panelWaferChamberC";
            this.panelWaferChamberC.Size = new System.Drawing.Size(48, 48);
            this.panelWaferChamberC.TabIndex = 0;
            this.panelWaferChamberC.Visible = false;
            // 
            // panelLampChamberC
            // 
            this.panelLampChamberC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLampChamberC.BackColor = System.Drawing.Color.ForestGreen;
            this.panelLampChamberC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLampChamberC.Location = new System.Drawing.Point(168, 14);
            this.panelLampChamberC.Name = "panelLampChamberC";
            this.panelLampChamberC.Size = new System.Drawing.Size(20, 20);
            this.panelLampChamberC.TabIndex = 5;
            // 
            // labelChamberC
            // 
            this.labelChamberC.AutoSize = true;
            this.labelChamberC.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelChamberC.ForeColor = System.Drawing.Color.White;
            this.labelChamberC.Location = new System.Drawing.Point(10, 14);
            this.labelChamberC.Name = "labelChamberC";
            this.labelChamberC.Size = new System.Drawing.Size(93, 21);
            this.labelChamberC.TabIndex = 0;
            this.labelChamberC.Text = "Chamber C";
            // 
            // panelChamberA
            // 
            this.panelChamberA.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelChamberA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(66)))), ((int)(((byte)(73)))));
            this.panelChamberA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelChamberA.Controls.Add(this.panelDoorChamberA);
            this.panelChamberA.Controls.Add(this.panelLampChamberA);
            this.panelChamberA.Controls.Add(this.labelChamberA);
            this.panelChamberA.Location = new System.Drawing.Point(37, 87);
            this.panelChamberA.Margin = new System.Windows.Forms.Padding(12);
            this.panelChamberA.MinimumSize = new System.Drawing.Size(150, 110);
            this.panelChamberA.Name = "panelChamberA";
            this.panelChamberA.Padding = new System.Windows.Forms.Padding(14);
            this.panelChamberA.Size = new System.Drawing.Size(205, 110);
            this.panelChamberA.TabIndex = 2;
            // 
            // panelDoorChamberA
            // 
            this.panelDoorChamberA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(94)))), ((int)(((byte)(102)))));
            this.panelDoorChamberA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDoorChamberA.Controls.Add(this.panelWaferChamberA);
            this.panelDoorChamberA.Location = new System.Drawing.Point(14, 50);
            this.panelDoorChamberA.Name = "panelDoorChamberA";
            this.panelDoorChamberA.Size = new System.Drawing.Size(168, 58);
            this.panelDoorChamberA.TabIndex = 6;
            // 
            // panelWaferChamberA
            // 
            this.panelWaferChamberA.BackColor = System.Drawing.Color.Transparent;
            this.panelWaferChamberA.Location = new System.Drawing.Point(60, 10);
            this.panelWaferChamberA.Name = "panelWaferChamberA";
            this.panelWaferChamberA.Size = new System.Drawing.Size(48, 48);
            this.panelWaferChamberA.TabIndex = 0;
            this.panelWaferChamberA.Visible = false;
            // 
            // panelLampChamberA
            // 
            this.panelLampChamberA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLampChamberA.BackColor = System.Drawing.Color.ForestGreen;
            this.panelLampChamberA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLampChamberA.Location = new System.Drawing.Point(168, 14);
            this.panelLampChamberA.Name = "panelLampChamberA";
            this.panelLampChamberA.Size = new System.Drawing.Size(20, 20);
            this.panelLampChamberA.TabIndex = 3;
            // 
            // labelChamberA
            // 
            this.labelChamberA.AutoSize = true;
            this.labelChamberA.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelChamberA.ForeColor = System.Drawing.Color.White;
            this.labelChamberA.Location = new System.Drawing.Point(10, 14);
            this.labelChamberA.Name = "labelChamberA";
            this.labelChamberA.Size = new System.Drawing.Size(94, 21);
            this.labelChamberA.TabIndex = 0;
            this.labelChamberA.Text = "Chamber A";
            // 
            // panelChamberB
            // 
            this.panelChamberB.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelChamberB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(66)))), ((int)(((byte)(73)))));
            this.panelChamberB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelChamberB.Controls.Add(this.panelDoorChamberB);
            this.panelChamberB.Controls.Add(this.panelLampChamberB);
            this.panelChamberB.Controls.Add(this.labelChamberB);
            this.panelChamberB.Location = new System.Drawing.Point(256, 2);
            this.panelChamberB.Margin = new System.Windows.Forms.Padding(12);
            this.panelChamberB.MinimumSize = new System.Drawing.Size(150, 110);
            this.panelChamberB.Name = "panelChamberB";
            this.panelChamberB.Padding = new System.Windows.Forms.Padding(14);
            this.panelChamberB.Size = new System.Drawing.Size(205, 110);
            this.panelChamberB.TabIndex = 3;
            // 
            // panelDoorChamberB
            // 
            this.panelDoorChamberB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(94)))), ((int)(((byte)(102)))));
            this.panelDoorChamberB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDoorChamberB.Controls.Add(this.panelWaferChamberB);
            this.panelDoorChamberB.Location = new System.Drawing.Point(14, 50);
            this.panelDoorChamberB.Name = "panelDoorChamberB";
            this.panelDoorChamberB.Size = new System.Drawing.Size(168, 58);
            this.panelDoorChamberB.TabIndex = 7;
            // 
            // panelWaferChamberB
            // 
            this.panelWaferChamberB.BackColor = System.Drawing.Color.Transparent;
            this.panelWaferChamberB.Location = new System.Drawing.Point(60, 10);
            this.panelWaferChamberB.Name = "panelWaferChamberB";
            this.panelWaferChamberB.Size = new System.Drawing.Size(48, 48);
            this.panelWaferChamberB.TabIndex = 0;
            this.panelWaferChamberB.Visible = false;
            // 
            // panelLampChamberB
            // 
            this.panelLampChamberB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLampChamberB.BackColor = System.Drawing.Color.ForestGreen;
            this.panelLampChamberB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLampChamberB.Location = new System.Drawing.Point(168, 14);
            this.panelLampChamberB.Name = "panelLampChamberB";
            this.panelLampChamberB.Size = new System.Drawing.Size(20, 20);
            this.panelLampChamberB.TabIndex = 4;
            // 
            // labelChamberB
            // 
            this.labelChamberB.AutoSize = true;
            this.labelChamberB.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelChamberB.ForeColor = System.Drawing.Color.White;
            this.labelChamberB.Location = new System.Drawing.Point(10, 14);
            this.labelChamberB.Name = "labelChamberB";
            this.labelChamberB.Size = new System.Drawing.Size(93, 21);
            this.labelChamberB.TabIndex = 0;
            this.labelChamberB.Text = "Chamber B";
            // 
            // flowProcessSummary
            // 
            this.flowProcessSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(190)))));
            this.flowProcessSummary.Controls.Add(this.panelSummaryFoupA);
            this.flowProcessSummary.Controls.Add(this.panelSummaryFoupB);
            this.flowProcessSummary.Controls.Add(this.panelSummaryProcess);
            this.flowProcessSummary.Controls.Add(this.panelSummaryPressure);
            this.flowProcessSummary.Controls.Add(this.panelSummaryTemperature);
            this.flowProcessSummary.Controls.Add(this.panelSummaryDoor);
            this.flowProcessSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowProcessSummary.Location = new System.Drawing.Point(3, 3);
            this.flowProcessSummary.Name = "flowProcessSummary";
            this.flowProcessSummary.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flowProcessSummary.Size = new System.Drawing.Size(741, 1);
            this.flowProcessSummary.TabIndex = 3;
            this.flowProcessSummary.Visible = false;
            // 
            // panelSummaryFoupA
            // 
            this.panelSummaryFoupA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.panelSummaryFoupA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummaryFoupA.Controls.Add(this.labelSummaryFoupAStatus);
            this.panelSummaryFoupA.Controls.Add(this.labelSummaryFoupATitle);
            this.panelSummaryFoupA.Location = new System.Drawing.Point(12, 8);
            this.panelSummaryFoupA.Margin = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.panelSummaryFoupA.Name = "panelSummaryFoupA";
            this.panelSummaryFoupA.Padding = new System.Windows.Forms.Padding(8);
            this.panelSummaryFoupA.Size = new System.Drawing.Size(150, 68);
            this.panelSummaryFoupA.TabIndex = 4;
            // 
            // labelSummaryFoupAStatus
            // 
            this.labelSummaryFoupAStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSummaryFoupAStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelSummaryFoupAStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelSummaryFoupAStatus.Location = new System.Drawing.Point(8, 28);
            this.labelSummaryFoupAStatus.Name = "labelSummaryFoupAStatus";
            this.labelSummaryFoupAStatus.Size = new System.Drawing.Size(132, 30);
            this.labelSummaryFoupAStatus.TabIndex = 1;
            this.labelSummaryFoupAStatus.Text = "FOUP 상태: 대기";
            // 
            // labelSummaryFoupATitle
            // 
            this.labelSummaryFoupATitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSummaryFoupATitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelSummaryFoupATitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelSummaryFoupATitle.Location = new System.Drawing.Point(8, 8);
            this.labelSummaryFoupATitle.Name = "labelSummaryFoupATitle";
            this.labelSummaryFoupATitle.Size = new System.Drawing.Size(132, 20);
            this.labelSummaryFoupATitle.TabIndex = 0;
            this.labelSummaryFoupATitle.Text = "FOUP A";
            // 
            // panelSummaryFoupB
            // 
            this.panelSummaryFoupB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.panelSummaryFoupB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummaryFoupB.Controls.Add(this.labelSummaryFoupBStatus);
            this.panelSummaryFoupB.Controls.Add(this.labelSummaryFoupBTitle);
            this.panelSummaryFoupB.Location = new System.Drawing.Point(186, 8);
            this.panelSummaryFoupB.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.panelSummaryFoupB.Name = "panelSummaryFoupB";
            this.panelSummaryFoupB.Padding = new System.Windows.Forms.Padding(8);
            this.panelSummaryFoupB.Size = new System.Drawing.Size(150, 68);
            this.panelSummaryFoupB.TabIndex = 5;
            // 
            // labelSummaryFoupBStatus
            // 
            this.labelSummaryFoupBStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSummaryFoupBStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelSummaryFoupBStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelSummaryFoupBStatus.Location = new System.Drawing.Point(8, 28);
            this.labelSummaryFoupBStatus.Name = "labelSummaryFoupBStatus";
            this.labelSummaryFoupBStatus.Size = new System.Drawing.Size(132, 30);
            this.labelSummaryFoupBStatus.TabIndex = 1;
            this.labelSummaryFoupBStatus.Text = "FOUP 상태: 대기";
            // 
            // labelSummaryFoupBTitle
            // 
            this.labelSummaryFoupBTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSummaryFoupBTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelSummaryFoupBTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelSummaryFoupBTitle.Location = new System.Drawing.Point(8, 8);
            this.labelSummaryFoupBTitle.Name = "labelSummaryFoupBTitle";
            this.labelSummaryFoupBTitle.Size = new System.Drawing.Size(132, 20);
            this.labelSummaryFoupBTitle.TabIndex = 0;
            this.labelSummaryFoupBTitle.Text = "FOUP B";
            // 
            // panelSummaryProcess
            // 
            this.panelSummaryProcess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.panelSummaryProcess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummaryProcess.Controls.Add(this.labelProcessValue);
            this.panelSummaryProcess.Controls.Add(this.labelSummaryProcessTitle);
            this.panelSummaryProcess.Location = new System.Drawing.Point(336, 16);
            this.panelSummaryProcess.Margin = new System.Windows.Forms.Padding(0, 8, 12, 0);
            this.panelSummaryProcess.Name = "panelSummaryProcess";
            this.panelSummaryProcess.Padding = new System.Windows.Forms.Padding(8);
            this.panelSummaryProcess.Size = new System.Drawing.Size(150, 68);
            this.panelSummaryProcess.TabIndex = 6;
            // 
            // labelProcessValue
            // 
            this.labelProcessValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProcessValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelProcessValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelProcessValue.Location = new System.Drawing.Point(8, 32);
            this.labelProcessValue.Name = "labelProcessValue";
            this.labelProcessValue.Size = new System.Drawing.Size(132, 26);
            this.labelProcessValue.TabIndex = 1;
            this.labelProcessValue.Text = "정보 없음";
            this.labelProcessValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSummaryProcessTitle
            // 
            this.labelSummaryProcessTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSummaryProcessTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelSummaryProcessTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelSummaryProcessTitle.Location = new System.Drawing.Point(8, 8);
            this.labelSummaryProcessTitle.Name = "labelSummaryProcessTitle";
            this.labelSummaryProcessTitle.Size = new System.Drawing.Size(132, 24);
            this.labelSummaryProcessTitle.TabIndex = 0;
            this.labelSummaryProcessTitle.Text = "Process";
            this.labelSummaryProcessTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelSummaryPressure
            // 
            this.panelSummaryPressure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.panelSummaryPressure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummaryPressure.Controls.Add(this.labelPressureValue);
            this.panelSummaryPressure.Controls.Add(this.labelSummaryPressureTitle);
            this.panelSummaryPressure.Location = new System.Drawing.Point(510, 16);
            this.panelSummaryPressure.Margin = new System.Windows.Forms.Padding(12, 8, 12, 0);
            this.panelSummaryPressure.Name = "panelSummaryPressure";
            this.panelSummaryPressure.Padding = new System.Windows.Forms.Padding(8);
            this.panelSummaryPressure.Size = new System.Drawing.Size(150, 68);
            this.panelSummaryPressure.TabIndex = 7;
            // 
            // labelPressureValue
            // 
            this.labelPressureValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPressureValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelPressureValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelPressureValue.Location = new System.Drawing.Point(8, 32);
            this.labelPressureValue.Name = "labelPressureValue";
            this.labelPressureValue.Size = new System.Drawing.Size(132, 26);
            this.labelPressureValue.TabIndex = 1;
            this.labelPressureValue.Text = "정보 없음";
            this.labelPressureValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSummaryPressureTitle
            // 
            this.labelSummaryPressureTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSummaryPressureTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelSummaryPressureTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelSummaryPressureTitle.Location = new System.Drawing.Point(8, 8);
            this.labelSummaryPressureTitle.Name = "labelSummaryPressureTitle";
            this.labelSummaryPressureTitle.Size = new System.Drawing.Size(132, 24);
            this.labelSummaryPressureTitle.TabIndex = 0;
            this.labelSummaryPressureTitle.Text = "Pressure";
            this.labelSummaryPressureTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelSummaryTemperature
            // 
            this.panelSummaryTemperature.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.panelSummaryTemperature.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummaryTemperature.Controls.Add(this.labelTemperatureValue);
            this.panelSummaryTemperature.Controls.Add(this.labelSummaryTemperatureTitle);
            this.panelSummaryTemperature.Location = new System.Drawing.Point(12, 92);
            this.panelSummaryTemperature.Margin = new System.Windows.Forms.Padding(12, 8, 12, 0);
            this.panelSummaryTemperature.Name = "panelSummaryTemperature";
            this.panelSummaryTemperature.Padding = new System.Windows.Forms.Padding(8);
            this.panelSummaryTemperature.Size = new System.Drawing.Size(150, 68);
            this.panelSummaryTemperature.TabIndex = 8;
            // 
            // labelTemperatureValue
            // 
            this.labelTemperatureValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTemperatureValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelTemperatureValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.labelTemperatureValue.Location = new System.Drawing.Point(8, 32);
            this.labelTemperatureValue.Name = "labelTemperatureValue";
            this.labelTemperatureValue.Size = new System.Drawing.Size(132, 26);
            this.labelTemperatureValue.TabIndex = 1;
            this.labelTemperatureValue.Text = "정보 없음";
            this.labelTemperatureValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSummaryTemperatureTitle
            // 
            this.labelSummaryTemperatureTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSummaryTemperatureTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelSummaryTemperatureTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.labelSummaryTemperatureTitle.Location = new System.Drawing.Point(8, 8);
            this.labelSummaryTemperatureTitle.Name = "labelSummaryTemperatureTitle";
            this.labelSummaryTemperatureTitle.Size = new System.Drawing.Size(132, 24);
            this.labelSummaryTemperatureTitle.TabIndex = 0;
            this.labelSummaryTemperatureTitle.Text = "Temperature";
            this.labelSummaryTemperatureTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelSummaryDoor
            // 
            this.panelSummaryDoor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(66)))), ((int)(((byte)(73)))));
            this.panelSummaryDoor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummaryDoor.Controls.Add(this.labelDoorValue);
            this.panelSummaryDoor.Controls.Add(this.labelSummaryDoorTitle);
            this.panelSummaryDoor.Location = new System.Drawing.Point(186, 92);
            this.panelSummaryDoor.Margin = new System.Windows.Forms.Padding(12, 8, 12, 0);
            this.panelSummaryDoor.Name = "panelSummaryDoor";
            this.panelSummaryDoor.Padding = new System.Windows.Forms.Padding(8);
            this.panelSummaryDoor.Size = new System.Drawing.Size(150, 68);
            this.panelSummaryDoor.TabIndex = 9;
            // 
            // labelDoorValue
            // 
            this.labelDoorValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDoorValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelDoorValue.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelDoorValue.Location = new System.Drawing.Point(8, 32);
            this.labelDoorValue.Name = "labelDoorValue";
            this.labelDoorValue.Size = new System.Drawing.Size(132, 26);
            this.labelDoorValue.TabIndex = 1;
            this.labelDoorValue.Text = "정보 없음";
            this.labelDoorValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSummaryDoorTitle
            // 
            this.labelSummaryDoorTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSummaryDoorTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelSummaryDoorTitle.ForeColor = System.Drawing.Color.White;
            this.labelSummaryDoorTitle.Location = new System.Drawing.Point(8, 8);
            this.labelSummaryDoorTitle.Name = "labelSummaryDoorTitle";
            this.labelSummaryDoorTitle.Size = new System.Drawing.Size(132, 24);
            this.labelSummaryDoorTitle.TabIndex = 0;
            this.labelSummaryDoorTitle.Text = "Door";
            this.labelSummaryDoorTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelSummaryTM
            // 
            this.panelSummaryTM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(66)))), ((int)(((byte)(73)))));
            this.panelSummaryTM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummaryTM.Controls.Add(this.labelSummaryTMStatus);
            this.panelSummaryTM.Controls.Add(this.labelSummaryTMTitle);
            this.panelSummaryTM.Location = new System.Drawing.Point(0, 8);
            this.panelSummaryTM.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.panelSummaryTM.Name = "panelSummaryTM";
            this.panelSummaryTM.Padding = new System.Windows.Forms.Padding(8);
            this.panelSummaryTM.Size = new System.Drawing.Size(150, 68);
            this.panelSummaryTM.TabIndex = 0;
            // 
            // labelSummaryTMStatus
            // 
            this.labelSummaryTMStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSummaryTMStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelSummaryTMStatus.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelSummaryTMStatus.Location = new System.Drawing.Point(8, 28);
            this.labelSummaryTMStatus.Name = "labelSummaryTMStatus";
            this.labelSummaryTMStatus.Size = new System.Drawing.Size(132, 30);
            this.labelSummaryTMStatus.TabIndex = 1;
            this.labelSummaryTMStatus.Text = "상태: Idle";
            // 
            // labelSummaryTMTitle
            // 
            this.labelSummaryTMTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSummaryTMTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelSummaryTMTitle.ForeColor = System.Drawing.Color.White;
            this.labelSummaryTMTitle.Location = new System.Drawing.Point(8, 8);
            this.labelSummaryTMTitle.Name = "labelSummaryTMTitle";
            this.labelSummaryTMTitle.Size = new System.Drawing.Size(132, 20);
            this.labelSummaryTMTitle.TabIndex = 0;
            this.labelSummaryTMTitle.Text = "TM";
            // 
            // panelStatusDoorProcess
            // 
            this.panelStatusDoorProcess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(94)))), ((int)(((byte)(102)))));
            this.panelStatusDoorProcess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatusDoorProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusDoorProcess.Location = new System.Drawing.Point(45, 4);
            this.panelStatusDoorProcess.Name = "panelStatusDoorProcess";
            this.panelStatusDoorProcess.Size = new System.Drawing.Size(183, 120);
            this.panelStatusDoorProcess.TabIndex = 3;
            // 
            // panelStatusLampProcess
            // 
            this.panelStatusLampProcess.BackColor = System.Drawing.Color.ForestGreen;
            this.panelStatusLampProcess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatusLampProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusLampProcess.Location = new System.Drawing.Point(4, 4);
            this.panelStatusLampProcess.Name = "panelStatusLampProcess";
            this.panelStatusLampProcess.Size = new System.Drawing.Size(34, 120);
            this.panelStatusLampProcess.TabIndex = 2;
            // 
            // panelStatusDoorPressure
            // 
            this.panelStatusDoorPressure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(94)))), ((int)(((byte)(102)))));
            this.panelStatusDoorPressure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatusDoorPressure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusDoorPressure.Location = new System.Drawing.Point(45, 131);
            this.panelStatusDoorPressure.Name = "panelStatusDoorPressure";
            this.panelStatusDoorPressure.Size = new System.Drawing.Size(183, 120);
            this.panelStatusDoorPressure.TabIndex = 4;
            // 
            // panelStatusLampPressure
            // 
            this.panelStatusLampPressure.BackColor = System.Drawing.Color.ForestGreen;
            this.panelStatusLampPressure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatusLampPressure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusLampPressure.Location = new System.Drawing.Point(4, 131);
            this.panelStatusLampPressure.Name = "panelStatusLampPressure";
            this.panelStatusLampPressure.Size = new System.Drawing.Size(34, 120);
            this.panelStatusLampPressure.TabIndex = 2;
            // 
            // panelStatusDoorTemperature
            // 
            this.panelStatusDoorTemperature.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(94)))), ((int)(((byte)(102)))));
            this.panelStatusDoorTemperature.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatusDoorTemperature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusDoorTemperature.Location = new System.Drawing.Point(45, 258);
            this.panelStatusDoorTemperature.Name = "panelStatusDoorTemperature";
            this.panelStatusDoorTemperature.Size = new System.Drawing.Size(183, 120);
            this.panelStatusDoorTemperature.TabIndex = 5;
            // 
            // panelStatusLampTemperature
            // 
            this.panelStatusLampTemperature.BackColor = System.Drawing.Color.ForestGreen;
            this.panelStatusLampTemperature.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatusLampTemperature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusLampTemperature.Location = new System.Drawing.Point(4, 258);
            this.panelStatusLampTemperature.Name = "panelStatusLampTemperature";
            this.panelStatusLampTemperature.Size = new System.Drawing.Size(34, 120);
            this.panelStatusLampTemperature.TabIndex = 2;
            // 
            // panelStatusDoorOverall
            // 
            this.panelStatusDoorOverall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(94)))), ((int)(((byte)(102)))));
            this.panelStatusDoorOverall.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatusDoorOverall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusDoorOverall.Location = new System.Drawing.Point(45, 385);
            this.panelStatusDoorOverall.Name = "panelStatusDoorOverall";
            this.panelStatusDoorOverall.Size = new System.Drawing.Size(183, 121);
            this.panelStatusDoorOverall.TabIndex = 6;
            // 
            // panelStatusLampDoor
            // 
            this.panelStatusLampDoor.BackColor = System.Drawing.Color.ForestGreen;
            this.panelStatusLampDoor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatusLampDoor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusLampDoor.Location = new System.Drawing.Point(4, 385);
            this.panelStatusLampDoor.Name = "panelStatusLampDoor";
            this.panelStatusLampDoor.Size = new System.Drawing.Size(34, 121);
            this.panelStatusLampDoor.TabIndex = 2;
            // 
            // panelStatusMonitoring
            // 
            this.panelStatusMonitoring.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(58)))));
            this.panelStatusMonitoring.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatusMonitoring.Controls.Add(this.tableStatusIndicators);
            this.panelStatusMonitoring.Controls.Add(this.labelStatusTitle);
            this.panelStatusMonitoring.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusMonitoring.Location = new System.Drawing.Point(3, 3);
            this.panelStatusMonitoring.Name = "panelStatusMonitoring";
            this.panelStatusMonitoring.Padding = new System.Windows.Forms.Padding(20, 20, 20, 15);
            this.panelStatusMonitoring.Size = new System.Drawing.Size(274, 598);
            this.panelStatusMonitoring.TabIndex = 0;
            // 
            // tableStatusIndicators
            // 
            this.tableStatusIndicators.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableStatusIndicators.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(59)))), ((int)(((byte)(66)))));
            this.tableStatusIndicators.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableStatusIndicators.ColumnCount = 2;
            this.tableStatusIndicators.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableStatusIndicators.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableStatusIndicators.Controls.Add(this.panelStatusLampDoor, 0, 3);
            this.tableStatusIndicators.Controls.Add(this.panelStatusDoorOverall, 1, 3);
            this.tableStatusIndicators.Controls.Add(this.panelStatusLampTemperature, 0, 2);
            this.tableStatusIndicators.Controls.Add(this.panelStatusDoorTemperature, 1, 2);
            this.tableStatusIndicators.Controls.Add(this.panelStatusLampPressure, 0, 1);
            this.tableStatusIndicators.Controls.Add(this.panelStatusDoorPressure, 1, 1);
            this.tableStatusIndicators.Controls.Add(this.panelStatusLampProcess, 0, 0);
            this.tableStatusIndicators.Controls.Add(this.panelStatusDoorProcess, 1, 0);
            this.tableStatusIndicators.Location = new System.Drawing.Point(20, 65);
            this.tableStatusIndicators.Name = "tableStatusIndicators";
            this.tableStatusIndicators.RowCount = 4;
            this.tableStatusIndicators.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableStatusIndicators.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableStatusIndicators.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableStatusIndicators.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableStatusIndicators.Size = new System.Drawing.Size(232, 510);
            this.tableStatusIndicators.TabIndex = 1;
            // 
            // labelStatusTitle
            // 
            this.labelStatusTitle.AutoSize = true;
            this.labelStatusTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelStatusTitle.ForeColor = System.Drawing.Color.White;
            this.labelStatusTitle.Location = new System.Drawing.Point(20, 20);
            this.labelStatusTitle.Name = "labelStatusTitle";
            this.labelStatusTitle.Size = new System.Drawing.Size(174, 25);
            this.labelStatusTitle.TabIndex = 0;
            this.labelStatusTitle.Text = "상태 모니터링 패널";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(210)))));
            this.ClientSize = new System.Drawing.Size(1701, 976);
            this.Controls.Add(this.tableLayoutRoot);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.MinimumSize = new System.Drawing.Size(1300, 840);
            this.Name = "Form1";
            this.Text = "반도체 장비 제어 UI";
            this.tableLayoutRoot.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.tableLayoutHeader.ResumeLayout(false);
            this.tableLayoutHeader.PerformLayout();
            this.flowHeaderLogin.ResumeLayout(false);
            this.flowHeaderLogin.PerformLayout();
            this.flowLoginTopRow.ResumeLayout(false);
            this.flowLoginBottomRow.ResumeLayout(false);
            this.panelHeaderStatusSummary.ResumeLayout(false);
            this.panelHeaderStatusSummary.PerformLayout();
            this.tableHeaderStatus.ResumeLayout(false);
            this.flowHeaderStatus.ResumeLayout(false);
            this.panelHeaderCardTM.ResumeLayout(false);
            this.panelHeaderCardTM.PerformLayout();
            this.panelHeaderCardPMA.ResumeLayout(false);
            this.panelHeaderCardPMA.PerformLayout();
            this.panelHeaderCardPMB.ResumeLayout(false);
            this.panelHeaderCardPMB.PerformLayout();
            this.panelHeaderCardPMC.ResumeLayout(false);
            this.panelHeaderCardPMC.PerformLayout();
            this.panelHeaderAlarm.ResumeLayout(false);
            this.flowAlarmIndicator.ResumeLayout(false);
            this.flowAlarmIndicator.PerformLayout();
            this.tableHeaderMessageText.ResumeLayout(false);
            this.tableHeaderMessageText.PerformLayout();
            this.flowHeaderTabs.ResumeLayout(false);
            this.flowHeaderTimeAndUser.ResumeLayout(false);
            this.tableLayoutProcessArea.ResumeLayout(false);
            this.panelMainProcess.ResumeLayout(false);
            this.tableLayoutMainProcess.ResumeLayout(false);
            this.tableLayoutMainProcess.PerformLayout();
            this.tableLayoutMainContent.ResumeLayout(false);
            this.panelEquipment.ResumeLayout(false);
            this.tableLayoutEquipmentArea.ResumeLayout(false);
            this.tableLayoutEquipment.ResumeLayout(false);
            this.tableLayoutChamberCluster.ResumeLayout(false);
            this.panelPmStatus.ResumeLayout(false);
            this.panelPmStatus.PerformLayout();
            this.tableLayoutPmStatus.ResumeLayout(false);
            this.tableProcessMetrics.ResumeLayout(false);
            this.tableProcessMetrics.PerformLayout();
            this.panelFoupStatusA.ResumeLayout(false);
            this.tableFoupACard.ResumeLayout(false);
            this.panelFoupALevelContainer.ResumeLayout(false);
            this.panelFoupALevelTrack.ResumeLayout(false);
            this.panelFoupADetail.ResumeLayout(false);
            this.tableFoupAInfo.ResumeLayout(false);
            this.panelFoupStatusB.ResumeLayout(false);
            this.tableFoupBCard.ResumeLayout(false);
            this.panelFoupBLevelContainer.ResumeLayout(false);
            this.panelFoupBLevelTrack.ResumeLayout(false);
            this.panelFoupBDetail.ResumeLayout(false);
            this.tableFoupBInfo.ResumeLayout(false);
            this.panelControlPanel.ResumeLayout(false);
            this.flowControlPanelStack.ResumeLayout(false);
            this.flowControlPanelStack.PerformLayout();
            this.groupBoxControlButtons.ResumeLayout(false);
            this.groupBoxControlButtons.PerformLayout();
            this.flowLayoutControlButtons.ResumeLayout(false);
            this.groupBoxFoupReady.ResumeLayout(false);
            this.groupBoxFoupReady.PerformLayout();
            this.flowLayoutFoupReadyButtons.ResumeLayout(false);
            this.groupBoxRecipe.ResumeLayout(false);
            this.groupBoxRecipe.PerformLayout();
            this.panelAlarmArea.ResumeLayout(false);
            this.flowBottomNavigation.ResumeLayout(false);
            this.panelMainLamp.ResumeLayout(false);
            this.panelMainLamp.PerformLayout();
            this.panelChamberC.ResumeLayout(false);
            this.panelChamberC.PerformLayout();
            this.panelDoorChamberC.ResumeLayout(false);
            this.panelChamberA.ResumeLayout(false);
            this.panelChamberA.PerformLayout();
            this.panelDoorChamberA.ResumeLayout(false);
            this.panelChamberB.ResumeLayout(false);
            this.panelChamberB.PerformLayout();
            this.panelDoorChamberB.ResumeLayout(false);
            this.flowProcessSummary.ResumeLayout(false);
            this.panelSummaryFoupA.ResumeLayout(false);
            this.panelSummaryFoupB.ResumeLayout(false);
            this.panelSummaryProcess.ResumeLayout(false);
            this.panelSummaryPressure.ResumeLayout(false);
            this.panelSummaryTemperature.ResumeLayout(false);
            this.panelSummaryDoor.ResumeLayout(false);
            this.panelSummaryTM.ResumeLayout(false);
            this.panelStatusMonitoring.ResumeLayout(false);
            this.panelStatusMonitoring.PerformLayout();
            this.tableStatusIndicators.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Panel panelEquipment;
        private System.Windows.Forms.TableLayoutPanel tableLayoutEquipment;
        private System.Windows.Forms.TableLayoutPanel tableLayoutChamberCluster;
        internal System.Windows.Forms.Panel panelEquipmentCanvas;
        private System.Windows.Forms.TableLayoutPanel tableLayoutEquipmentArea;
        private System.Windows.Forms.Panel panelPmStatus;
        internal System.Windows.Forms.Label labelPmStatusTitle;
        internal System.Windows.Forms.TableLayoutPanel tableLayoutPmStatus;
        internal System.Windows.Forms.Panel panelMainLamp;
        private System.Windows.Forms.Label labelMainLamp;
        internal System.Windows.Forms.Panel panelMainLampRed;
        internal System.Windows.Forms.Panel panelMainLampYellow;
        internal System.Windows.Forms.Panel panelMainLampGreen;
        private System.Windows.Forms.Label labelMainLampGreen;
        private System.Windows.Forms.Label labelMainLampYellow;
        private System.Windows.Forms.Label labelMainLampRed;
        internal SemiconductorUi.Controls.TmVisualizationControl tmVisualizationControl;
        internal System.Windows.Forms.Panel panelChamberA;
        internal System.Windows.Forms.Panel panelLampChamberA;
        internal System.Windows.Forms.Panel panelDoorChamberA;
        internal System.Windows.Forms.Panel panelWaferChamberA;
        private System.Windows.Forms.Label labelChamberA;
        internal System.Windows.Forms.Panel panelChamberB;
        internal System.Windows.Forms.Panel panelLampChamberB;
        internal System.Windows.Forms.Panel panelDoorChamberB;
        internal System.Windows.Forms.Panel panelWaferChamberB;
        private System.Windows.Forms.Label labelChamberB;
        internal System.Windows.Forms.Panel panelChamberC;
        internal System.Windows.Forms.Panel panelLampChamberC;
        internal System.Windows.Forms.Panel panelDoorChamberC;
        internal System.Windows.Forms.Panel panelWaferChamberC;
        private System.Windows.Forms.Label labelChamberC;
        internal System.Windows.Forms.Panel panelFoupA;
        internal SemiconductorUi.Controls.FoupVisualizationControl foupVisualizationControlA;
        internal System.Windows.Forms.Panel panelFoupB;
        internal SemiconductorUi.Controls.FoupVisualizationControl foupVisualizationControlB;
        private System.Windows.Forms.TableLayoutPanel tableLayoutRoot;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutHeader;
        private System.Windows.Forms.FlowLayoutPanel flowHeaderLogin;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.Button buttonUserManagement;
        internal System.Windows.Forms.Label labelLoginStatus;
        private System.Windows.Forms.Panel panelHeaderStatusSummary;
        private System.Windows.Forms.FlowLayoutPanel flowHeaderStatus;
        private System.Windows.Forms.FlowLayoutPanel flowLoginTopRow;
        private System.Windows.Forms.FlowLayoutPanel flowLoginBottomRow;
        private System.Windows.Forms.Label labelHeaderStatusTitle;
        private System.Windows.Forms.Panel panelHeaderCardTM;
        internal System.Windows.Forms.Label labelHeaderCardTMStatus;
        private System.Windows.Forms.Label labelHeaderCardTMTitle;
        private System.Windows.Forms.Panel panelHeaderCardPMA;
        internal System.Windows.Forms.Label labelHeaderCardPMAStatus;
        private System.Windows.Forms.Label labelHeaderCardPMATitle;
        private System.Windows.Forms.Panel panelHeaderCardPMB;
        internal System.Windows.Forms.Label labelHeaderCardPMBStatus;
        private System.Windows.Forms.Label labelHeaderCardPMBTitle;
        private System.Windows.Forms.Panel panelHeaderCardPMC;
        internal System.Windows.Forms.Label labelHeaderCardPMCStatus;
        private System.Windows.Forms.Label labelHeaderCardPMCTitle;
        private System.Windows.Forms.TableLayoutPanel tableHeaderStatus;
        private System.Windows.Forms.Label labelHeaderLotTitle;
        private System.Windows.Forms.Label labelHeaderFoupATitle;
        private System.Windows.Forms.Label labelHeaderFoupBTitle;
        private System.Windows.Forms.Label labelHeaderPM1Title;
        private System.Windows.Forms.Label labelHeaderPM2Title;
        private System.Windows.Forms.Label labelHeaderPM3Title;
        private System.Windows.Forms.Label labelHeaderTMTitle;
        private System.Windows.Forms.Label labelHeaderAlarmTitle;
        private System.Windows.Forms.Label labelHeaderLotStatus;
        internal System.Windows.Forms.Label labelHeaderFoupAStatus;
        internal System.Windows.Forms.Label labelHeaderFoupBStatus;
        internal System.Windows.Forms.Label labelHeaderPM1Status;
        internal System.Windows.Forms.Label labelHeaderPM2Status;
        internal System.Windows.Forms.Label labelHeaderPM3Status;
        internal System.Windows.Forms.Label labelHeaderTMStatus;
        private System.Windows.Forms.Label labelHeaderAlarmStatus;
        private System.Windows.Forms.TableLayoutPanel panelHeaderAlarm;
        private System.Windows.Forms.FlowLayoutPanel flowAlarmIndicator;
        internal System.Windows.Forms.Panel panelHeaderMessageAccent;
        private System.Windows.Forms.TableLayoutPanel tableHeaderMessageText;
        private System.Windows.Forms.Label labelHeaderEventLevel;
        internal System.Windows.Forms.Label labelHeaderEventTitle;
        internal System.Windows.Forms.Label labelHeaderEventMessage;
        internal System.Windows.Forms.Label labelHeaderCurrentTime;
        internal System.Windows.Forms.Label labelEthercatStatus;
        private System.Windows.Forms.Button buttonEthercatConnect;
        private System.Windows.Forms.Button buttonEthercatDisconnect;
        private System.Windows.Forms.Button buttonServoOn;
        private System.Windows.Forms.Button buttonServoOff;
        private System.Windows.Forms.Button buttonServoHome;
        internal System.Windows.Forms.Label labelServoStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutProcessArea;
        private System.Windows.Forms.FlowLayoutPanel flowHeaderTabs;
        internal System.Windows.Forms.Button buttonTabMain;
        internal System.Windows.Forms.Button buttonTabVerification;
        internal System.Windows.Forms.Button buttonTabTransfer;
        private System.Windows.Forms.FlowLayoutPanel flowHeaderTimeAndUser;
        private System.Windows.Forms.Panel panelStatusMonitoring;
        private System.Windows.Forms.Label labelStatusTitle;
        private System.Windows.Forms.TableLayoutPanel tableStatusIndicators;
        private System.Windows.Forms.Panel panelStatusLampDoor;
        private System.Windows.Forms.Panel panelStatusDoorOverall;
        private System.Windows.Forms.Panel panelStatusLampTemperature;
        private System.Windows.Forms.Panel panelStatusDoorTemperature;
        private System.Windows.Forms.Panel panelStatusLampPressure;
        private System.Windows.Forms.Panel panelStatusDoorPressure;
        private System.Windows.Forms.Panel panelStatusLampProcess;
        private System.Windows.Forms.Panel panelStatusDoorProcess;
        private System.Windows.Forms.Panel panelMainProcess;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMainProcess;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMainContent;
        private System.Windows.Forms.Label labelMainProcessTitle;
        private System.Windows.Forms.FlowLayoutPanel flowProcessSummary;
        internal System.Windows.Forms.Panel panelSummaryTM;
        internal System.Windows.Forms.Label labelSummaryTMStatus;
        private System.Windows.Forms.Label labelSummaryTMTitle;
        internal System.Windows.Forms.Panel panelSummaryPMA;
        internal System.Windows.Forms.Panel panelSummaryPMB;
        internal System.Windows.Forms.Panel panelSummaryPMC;
        internal System.Windows.Forms.Panel panelSummaryFoupA;
        internal System.Windows.Forms.Label labelSummaryFoupAStatus;
        internal System.Windows.Forms.Label labelSummaryFoupATitle;
        internal System.Windows.Forms.Panel panelSummaryFoupB;
        internal System.Windows.Forms.Label labelSummaryFoupBStatus;
        internal System.Windows.Forms.Label labelSummaryFoupBTitle;
        private System.Windows.Forms.Panel panelSummaryProcess;
        private System.Windows.Forms.Panel panelSummaryPressure;
        private System.Windows.Forms.Panel panelSummaryTemperature;
        private System.Windows.Forms.Panel panelSummaryDoor;
        private System.Windows.Forms.Label labelSummaryProcessTitle;
        private System.Windows.Forms.Label labelProcessValue;
        private System.Windows.Forms.Label labelSummaryPressureTitle;
        private System.Windows.Forms.Label labelPressureValue;
        private System.Windows.Forms.Label labelSummaryTemperatureTitle;
        private System.Windows.Forms.Label labelTemperatureValue;
        private System.Windows.Forms.Label labelSummaryDoorTitle;
        private System.Windows.Forms.Label labelDoorValue;
        private System.Windows.Forms.TableLayoutPanel tableProcessMetrics;
        internal System.Windows.Forms.Panel panelFoupStatusA;
        private System.Windows.Forms.TableLayoutPanel tableFoupACard;
        internal System.Windows.Forms.Panel panelFoupALevelContainer;
        private System.Windows.Forms.Panel panelFoupALevelTrack;
        private System.Windows.Forms.Panel panelFoupALevelFill;
        private System.Windows.Forms.Panel panelFoupADetail;
        internal System.Windows.Forms.Label labelFoupInfoATitle;
        internal System.Windows.Forms.Label labelFoupAStatusHeadline;
        private System.Windows.Forms.TableLayoutPanel tableFoupAInfo;
        private System.Windows.Forms.Label labelFoupAFieldPath;
        internal System.Windows.Forms.Label labelFoupAPathValue;
        private System.Windows.Forms.Label labelFoupAFieldPPID;
        internal System.Windows.Forms.Label labelFoupAPPIDValue;
        private System.Windows.Forms.Label labelFoupAFieldLotId;
        internal System.Windows.Forms.Label labelFoupALotIdValue;
        private System.Windows.Forms.Label labelFoupAFieldMid;
        internal System.Windows.Forms.Label labelFoupAMidValue;
        private System.Windows.Forms.Label labelFoupAFieldLock;
        internal System.Windows.Forms.Label labelFoupALockValue;
        internal System.Windows.Forms.Panel panelFoupStatusB;
        private System.Windows.Forms.TableLayoutPanel tableFoupBCard;
        internal System.Windows.Forms.Panel panelFoupBLevelContainer;
        private System.Windows.Forms.Panel panelFoupBLevelTrack;
        private System.Windows.Forms.Panel panelFoupBLevelFill;
        private System.Windows.Forms.Panel panelFoupBDetail;
        internal System.Windows.Forms.Label labelFoupInfoBTitle;
        internal System.Windows.Forms.Label labelFoupBStatusHeadline;
        private System.Windows.Forms.TableLayoutPanel tableFoupBInfo;
        private System.Windows.Forms.Label labelFoupBFieldPath;
        internal System.Windows.Forms.Label labelFoupBPathValue;
        private System.Windows.Forms.Label labelFoupBFieldPPID;
        internal System.Windows.Forms.Label labelFoupBPPIDValue;
        private System.Windows.Forms.Label labelFoupBFieldLotId;
        internal System.Windows.Forms.Label labelFoupBLotIdValue;
        private System.Windows.Forms.Label labelFoupBFieldMid;
        internal System.Windows.Forms.Label labelFoupBMidValue;
        private System.Windows.Forms.Label labelFoupBFieldLock;
        internal System.Windows.Forms.Label labelFoupBLockValue;
        internal System.Windows.Forms.Label labelFoupSummaryInfo;
        private System.Windows.Forms.Panel panelControlPanel;
        private System.Windows.Forms.FlowLayoutPanel flowControlPanelStack;
        private System.Windows.Forms.Label labelControlTitle;
        private System.Windows.Forms.GroupBox groupBoxControlButtons;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutControlButtons;
        internal System.Windows.Forms.Button buttonStart;
        internal System.Windows.Forms.Button buttonPause;
        internal System.Windows.Forms.Button buttonStop;
        internal System.Windows.Forms.Button buttonResetAlarm;
        internal System.Windows.Forms.Button buttonResetProcess;
        internal System.Windows.Forms.Button buttonEquipmentControl;
        private System.Windows.Forms.GroupBox groupBoxRecipe;
        internal System.Windows.Forms.GroupBox groupBoxFoupReady;
        internal System.Windows.Forms.FlowLayoutPanel flowLayoutFoupReadyButtons;
        internal System.Windows.Forms.Button buttonToggleFoupMount;
        internal System.Windows.Forms.Button buttonWaferLoading;
        internal System.Windows.Forms.Button buttonWaferUnloading;
        internal System.Windows.Forms.Button buttonApplyRecipe;
        internal System.Windows.Forms.ComboBox comboRecipeSelect;
        private System.Windows.Forms.Label labelRecipe;
        private System.Windows.Forms.Panel panelAlarmArea;
        private System.Windows.Forms.FlowLayoutPanel flowBottomNavigation;
        internal System.Windows.Forms.Button buttonNavOperate;
        internal System.Windows.Forms.Button buttonNavRecipe;
        internal System.Windows.Forms.Button buttonNavMaintenance;
        internal System.Windows.Forms.Button buttonNavConfig;
        internal System.Windows.Forms.Button buttonNavTrend;
        internal System.Windows.Forms.Button buttonNavReport;
        internal System.Windows.Forms.Button buttonNavSystem;

        #endregion
    }
}

