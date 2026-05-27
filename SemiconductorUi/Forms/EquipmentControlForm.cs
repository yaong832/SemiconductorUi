using System;
using System.Drawing;
using System.Windows.Forms;
using IEG3268_Dll;
using SemiconductorUi.Controls;

namespace SemiconductorUi.Forms
{
    public class EquipmentControlForm : Form
    {
        private IEG3268 ethercatDevice;
        private bool isConnected = false;
        
        // UI 업데이트를 위한 콜백
        public Action<EquipmentRegion, bool> OnDoorStateChanged; // region, isOpen
        public Action<EquipmentRegion, bool> OnChamberLampStateChanged; // region, isOn
        public Action<bool, bool, bool> OnMainLampStateChanged; // red, yellow, green
        public Action<bool> OnServoStateChanged; // isOn
        public Action OnHomingRequested; // 원점복귀 요청
        public Action<bool> OnCylinderStateChanged; // isExtended
        public Action<bool> OnVacuumStateChanged; // isOn

        // Chamber 램프 버튼
        private Button buttonChamberALampOn;
        private Button buttonChamberALampOff;
        private Button buttonChamberBLampOn;
        private Button buttonChamberBLampOff;
        private Button buttonChamberCLampOn;
        private Button buttonChamberCLampOff;

        // Chamber 도어 버튼
        private Button buttonChamberADoorOpen;
        private Button buttonChamberADoorClose;
        private Button buttonChamberBDoorOpen;
        private Button buttonChamberBDoorClose;
        private Button buttonChamberCDoorOpen;
        private Button buttonChamberCDoorClose;

        // 3색 램프 버튼
        private Button buttonMainLampRedOn;
        private Button buttonMainLampRedOff;
        private Button buttonMainLampYellowOn;
        private Button buttonMainLampYellowOff;
        private Button buttonMainLampGreenOn;
        private Button buttonMainLampGreenOff;

        // 서보 모터 제어 버튼
        private Button buttonServoOn;
        private Button buttonServoOff;
        private Button buttonServoHome;
        private Button buttonCylinderExtend;
        private Button buttonCylinderRetract;
        private Button buttonVacuumOn;
        private Button buttonVacuumOff;
        private Button buttonExhaustOn;
        private Button buttonExhaustOff;
        private Label labelServoStatus;

        private Label labelStatus;
        private Button buttonClose;
        
        // 상태 업데이트 타이머
        private System.Windows.Forms.Timer statusUpdateTimer;

        public EquipmentControlForm(IEG3268 device, bool connected)
        {
            ethercatDevice = device;
            isConnected = connected;
            InitializeComponent();
            InitializeStatusUpdateTimer();
        }
        
        private void InitializeStatusUpdateTimer()
        {
            statusUpdateTimer = new System.Windows.Forms.Timer();
            statusUpdateTimer.Interval = 500; // 500ms마다 상태 업데이트
            statusUpdateTimer.Tick += StatusUpdateTimer_Tick;
            statusUpdateTimer.Start();
        }
        
        private void StatusUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (isConnected && ethercatDevice != null)
            {
                try
                {
                    UpdateServoStatusFromDevice();
                }
                catch (Exception ex)
                {
                    // 타이머 오류는 조용히 처리 (로그 스팸 방지)
                    System.Diagnostics.Debug.WriteLine($"EquipmentControlForm 상태 업데이트 오류: {ex.Message}");
                }
            }
        }
        
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (statusUpdateTimer != null)
            {
                statusUpdateTimer.Stop();
                statusUpdateTimer.Dispose();
                statusUpdateTimer = null;
            }
            base.OnFormClosed(e);
        }

        private void InitializeComponent()
        {
            this.Text = "장비 직접 제어";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(250, 250, 255);
            this.ClientSize = new Size(800, 700);

            // 상태 레이블
            labelStatus = new Label();
            labelStatus.Text = isConnected ? "EtherCAT 연결됨" : "EtherCAT 미연결";
            labelStatus.ForeColor = isConnected ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
            labelStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labelStatus.Location = new Point(16, 12);
            labelStatus.Size = new Size(760, 24);
            labelStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            int yPos = 50;

            // Chamber A 제어 그룹
            var groupChamberA = CreateChamberGroup("Chamber A", yPos, 
                ref buttonChamberALampOn, ref buttonChamberALampOff,
                ref buttonChamberADoorOpen, ref buttonChamberADoorClose,
                () => ControlChamberLamp(3, true),
                () => ControlChamberLamp(3, false),
                () => ControlChamberDoor(4, 5, true),
                () => ControlChamberDoor(4, 5, false));
            this.Controls.Add(groupChamberA);
            yPos += 120;

            // Chamber B 제어 그룹
            var groupChamberB = CreateChamberGroup("Chamber B", yPos,
                ref buttonChamberBLampOn, ref buttonChamberBLampOff,
                ref buttonChamberBDoorOpen, ref buttonChamberBDoorClose,
                () => ControlChamberLamp(6, true),
                () => ControlChamberLamp(6, false),
                () => ControlChamberDoor(7, 8, true),
                () => ControlChamberDoor(7, 8, false));
            this.Controls.Add(groupChamberB);
            yPos += 120;

            // Chamber C 제어 그룹
            var groupChamberC = CreateChamberGroup("Chamber C", yPos,
                ref buttonChamberCLampOn, ref buttonChamberCLampOff,
                ref buttonChamberCDoorOpen, ref buttonChamberCDoorClose,
                () => ControlChamberLamp(9, true),
                () => ControlChamberLamp(9, false),
                () => ControlChamberDoor(10, 11, true),
                () => ControlChamberDoor(10, 11, false));
            this.Controls.Add(groupChamberC);
            yPos += 120;

            // 3색 램프 제어 그룹
            var groupMainLamp = CreateMainLampGroup(yPos);
            this.Controls.Add(groupMainLamp);
            yPos += 100;

            // 일괄 제어 그룹
            var groupBatchControl = CreateBatchControlGroup(yPos);
            this.Controls.Add(groupBatchControl);
            yPos += 100;

            // 서보 모터 제어 그룹 (TM 제어)
            var groupServoControl = CreateServoControlGroup(yPos);
            this.Controls.Add(groupServoControl);
            yPos += 130;

            // 폼 높이 조정
            this.ClientSize = new Size(800, yPos + 60);

            // 닫기 버튼
            var bottomPanel = new FlowLayoutPanel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 52;
            bottomPanel.Padding = new Padding(0, 8, 12, 8);
            bottomPanel.FlowDirection = FlowDirection.RightToLeft;
            bottomPanel.WrapContents = false;
            bottomPanel.BackColor = Color.FromArgb(240, 240, 245);

            buttonClose = new Button();
            buttonClose.Text = "닫기";
            buttonClose.Width = 100;
            buttonClose.Height = 32;
            buttonClose.Margin = new Padding(8, 8, 0, 8);
            buttonClose.BackColor = Color.FromArgb(100, 120, 130);
            buttonClose.FlatStyle = FlatStyle.Flat;
            buttonClose.FlatAppearance.BorderSize = 0;
            buttonClose.ForeColor = Color.White;
            buttonClose.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            buttonClose.Click += (s, e) => this.Close();

            bottomPanel.Controls.Add(buttonClose);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(labelStatus);

            UpdateButtonStates();
        }

        private GroupBox CreateChamberGroup(string title, int yPos,
            ref Button btnLampOn, ref Button btnLampOff,
            ref Button btnDoorOpen, ref Button btnDoorClose,
            Action lampOnAction, Action lampOffAction,
            Action doorOpenAction, Action doorCloseAction)
        {
            var group = new GroupBox();
            group.Text = title;
            group.ForeColor = Color.FromArgb(40, 40, 40);
            group.BackColor = Color.FromArgb(240, 240, 245);
            group.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            group.Location = new Point(16, yPos);
            group.Size = new Size(760, 110);
            group.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // 램프 제어
            var labelLamp = new Label();
            labelLamp.Text = "램프:";
            labelLamp.ForeColor = Color.FromArgb(40, 40, 40);
            labelLamp.Location = new Point(12, 25);
            labelLamp.Size = new Size(60, 20);

            btnLampOn = CreateControlButton("ON", new Size(80, 28), Color.FromArgb(76, 175, 80), lampOnAction);
            btnLampOn.Location = new Point(80, 22);

            btnLampOff = CreateControlButton("OFF", new Size(80, 28), Color.FromArgb(158, 158, 158), lampOffAction);
            btnLampOff.Location = new Point(170, 22);

            // 도어 제어
            var labelDoor = new Label();
            labelDoor.Text = "도어:";
            labelDoor.ForeColor = Color.FromArgb(40, 40, 40);
            labelDoor.Location = new Point(12, 60);
            labelDoor.Size = new Size(60, 20);

            btnDoorOpen = CreateControlButton("열기", new Size(80, 28), Color.FromArgb(33, 150, 243), doorOpenAction);
            btnDoorOpen.Location = new Point(80, 57);

            btnDoorClose = CreateControlButton("닫기", new Size(80, 28), Color.FromArgb(244, 67, 54), doorCloseAction);
            btnDoorClose.Location = new Point(170, 57);

            group.Controls.Add(labelLamp);
            group.Controls.Add(btnLampOn);
            group.Controls.Add(btnLampOff);
            group.Controls.Add(labelDoor);
            group.Controls.Add(btnDoorOpen);
            group.Controls.Add(btnDoorClose);

            return group;
        }

        private GroupBox CreateMainLampGroup(int yPos)
        {
            var group = new GroupBox();
            group.Text = "3색 램프";
            group.ForeColor = Color.FromArgb(40, 40, 40);
            group.BackColor = Color.FromArgb(240, 240, 245);
            group.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            group.Location = new Point(16, yPos);
            group.Size = new Size(760, 80);
            group.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // 적색 램프
            var labelRed = new Label();
            labelRed.Text = "적색:";
            labelRed.ForeColor = Color.FromArgb(244, 67, 54);
            labelRed.Location = new Point(12, 25);
            labelRed.Size = new Size(50, 20);

            buttonMainLampRedOn = CreateControlButton("ON", new Size(70, 28), Color.FromArgb(244, 67, 54), () => ControlMainLamp(0, true));
            buttonMainLampRedOn.Location = new Point(70, 22);

            buttonMainLampRedOff = CreateControlButton("OFF", new Size(70, 28), Color.FromArgb(158, 158, 158), () => ControlMainLamp(0, false));
            buttonMainLampRedOff.Location = new Point(150, 22);

            // 황색 램프
            var labelYellow = new Label();
            labelYellow.Text = "황색:";
            labelYellow.ForeColor = Color.FromArgb(255, 167, 38);
            labelYellow.Location = new Point(240, 25);
            labelYellow.Size = new Size(50, 20);

            buttonMainLampYellowOn = CreateControlButton("ON", new Size(70, 28), Color.FromArgb(255, 167, 38), () => ControlMainLamp(1, true));
            buttonMainLampYellowOn.Location = new Point(298, 22);

            buttonMainLampYellowOff = CreateControlButton("OFF", new Size(70, 28), Color.FromArgb(158, 158, 158), () => ControlMainLamp(1, false));
            buttonMainLampYellowOff.Location = new Point(378, 22);

            // 녹색 램프
            var labelGreen = new Label();
            labelGreen.Text = "녹색:";
            labelGreen.ForeColor = Color.FromArgb(76, 175, 80);
            labelGreen.Location = new Point(468, 25);
            labelGreen.Size = new Size(50, 20);

            buttonMainLampGreenOn = CreateControlButton("ON", new Size(70, 28), Color.FromArgb(76, 175, 80), () => ControlMainLamp(2, true));
            buttonMainLampGreenOn.Location = new Point(526, 22);

            buttonMainLampGreenOff = CreateControlButton("OFF", new Size(70, 28), Color.FromArgb(158, 158, 158), () => ControlMainLamp(2, false));
            buttonMainLampGreenOff.Location = new Point(606, 22);

            group.Controls.Add(labelRed);
            group.Controls.Add(buttonMainLampRedOn);
            group.Controls.Add(buttonMainLampRedOff);
            group.Controls.Add(labelYellow);
            group.Controls.Add(buttonMainLampYellowOn);
            group.Controls.Add(buttonMainLampYellowOff);
            group.Controls.Add(labelGreen);
            group.Controls.Add(buttonMainLampGreenOn);
            group.Controls.Add(buttonMainLampGreenOff);

            return group;
        }

        private GroupBox CreateServoControlGroup(int yPos)
        {
            var group = new GroupBox();
            group.Text = "TM 서보 모터 제어";
            group.ForeColor = Color.FromArgb(40, 40, 40);
            group.BackColor = Color.FromArgb(240, 240, 245);
            group.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            group.Location = new Point(16, yPos);
            group.Size = new Size(760, 120);
            group.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // 서보 ON/OFF
            var labelServo = new Label();
            labelServo.Text = "서보 전원:";
            labelServo.ForeColor = Color.FromArgb(40, 40, 40);
            labelServo.Location = new Point(12, 25);
            labelServo.Size = new Size(70, 20);

            buttonServoOn = CreateControlButton("ON", new Size(70, 28), Color.FromArgb(66, 133, 244), ControlServoOn);
            buttonServoOn.Location = new Point(90, 22);

            buttonServoOff = CreateControlButton("OFF", new Size(70, 28), Color.FromArgb(158, 158, 158), ControlServoOff);
            buttonServoOff.Location = new Point(170, 22);

            // 원점복귀
            buttonServoHome = CreateControlButton("원점복귀", new Size(90, 28), Color.FromArgb(255, 152, 0), ControlServoHome);
            buttonServoHome.Location = new Point(260, 22);

            // 서보 상태 표시
            labelServoStatus = new Label();
            labelServoStatus.ForeColor = Color.FromArgb(40, 40, 40);
            labelServoStatus.Font = new Font("Segoe UI", 8F);
            labelServoStatus.Location = new Point(370, 27);
            labelServoStatus.Size = new Size(150, 20);
            // 초기 상태: 현재 서보 상태 확인
            UpdateServoStatusFromDevice();

            // 실린더 제어
            var labelCylinder = new Label();
            labelCylinder.Text = "실린더:";
            labelCylinder.ForeColor = Color.FromArgb(40, 40, 40);
            labelCylinder.Location = new Point(12, 60);
            labelCylinder.Size = new Size(70, 20);

            buttonCylinderExtend = CreateControlButton("전진", new Size(70, 28), Color.FromArgb(33, 150, 243), ControlCylinderExtend);
            buttonCylinderExtend.Location = new Point(90, 57);

            buttonCylinderRetract = CreateControlButton("후진", new Size(70, 28), Color.FromArgb(244, 67, 54), ControlCylinderRetract);
            buttonCylinderRetract.Location = new Point(170, 57);

            // 진공/배기 제어
            var labelVacuum = new Label();
            labelVacuum.Text = "진공/배기:";
            labelVacuum.ForeColor = Color.FromArgb(40, 40, 40);
            labelVacuum.Location = new Point(260, 60);
            labelVacuum.Size = new Size(70, 20);

            buttonVacuumOn = CreateControlButton("진공 ON", new Size(80, 28), Color.FromArgb(76, 175, 80), ControlVacuumOn);
            buttonVacuumOn.Location = new Point(340, 57);

            buttonVacuumOff = CreateControlButton("진공 OFF", new Size(80, 28), Color.FromArgb(158, 158, 158), ControlVacuumOff);
            buttonVacuumOff.Location = new Point(430, 57);

            buttonExhaustOn = CreateControlButton("배기 ON", new Size(80, 28), Color.FromArgb(156, 39, 176), ControlExhaustOn);
            buttonExhaustOn.Location = new Point(520, 57);

            buttonExhaustOff = CreateControlButton("배기 OFF", new Size(80, 28), Color.FromArgb(158, 158, 158), ControlExhaustOff);
            buttonExhaustOff.Location = new Point(610, 57);

            group.Controls.Add(labelServo);
            group.Controls.Add(buttonServoOn);
            group.Controls.Add(buttonServoOff);
            group.Controls.Add(buttonServoHome);
            group.Controls.Add(labelServoStatus);
            group.Controls.Add(labelCylinder);
            group.Controls.Add(buttonCylinderExtend);
            group.Controls.Add(buttonCylinderRetract);
            group.Controls.Add(labelVacuum);
            group.Controls.Add(buttonVacuumOn);
            group.Controls.Add(buttonVacuumOff);
            group.Controls.Add(buttonExhaustOn);
            group.Controls.Add(buttonExhaustOff);

            return group;
        }

        private GroupBox CreateBatchControlGroup(int yPos)
        {
            var group = new GroupBox();
            group.Text = "일괄 제어";
            group.ForeColor = Color.FromArgb(40, 40, 40);
            group.BackColor = Color.FromArgb(240, 240, 245);
            group.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            group.Location = new Point(16, yPos);
            group.Size = new Size(760, 90);
            group.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // 모든 문 제어
            var labelAllDoors = new Label();
            labelAllDoors.Text = "모든 문:";
            labelAllDoors.ForeColor = Color.FromArgb(40, 40, 40);
            labelAllDoors.Location = new Point(12, 25);
            labelAllDoors.Size = new Size(70, 20);

            var buttonAllDoorsOpen = CreateControlButton("모두 열기", new Size(100, 32), Color.FromArgb(33, 150, 243), ControlAllDoorsOpen);
            buttonAllDoorsOpen.Location = new Point(90, 22);

            var buttonAllDoorsClose = CreateControlButton("모두 닫기", new Size(100, 32), Color.FromArgb(244, 67, 54), ControlAllDoorsClose);
            buttonAllDoorsClose.Location = new Point(200, 22);

            // 모든 램프 제어
            var labelAllLamps = new Label();
            labelAllLamps.Text = "모든 램프:";
            labelAllLamps.ForeColor = Color.FromArgb(40, 40, 40);
            labelAllLamps.Location = new Point(320, 25);
            labelAllLamps.Size = new Size(80, 20);

            var buttonAllLampsOn = CreateControlButton("모두 켜기", new Size(100, 32), Color.FromArgb(76, 175, 80), ControlAllLampsOn);
            buttonAllLampsOn.Location = new Point(410, 22);

            var buttonAllLampsOff = CreateControlButton("모두 끄기", new Size(100, 32), Color.FromArgb(158, 158, 158), ControlAllLampsOff);
            buttonAllLampsOff.Location = new Point(520, 22);

            group.Controls.Add(labelAllDoors);
            group.Controls.Add(buttonAllDoorsOpen);
            group.Controls.Add(buttonAllDoorsClose);
            group.Controls.Add(labelAllLamps);
            group.Controls.Add(buttonAllLampsOn);
            group.Controls.Add(buttonAllLampsOff);

            return group;
        }

        private Button CreateControlButton(string text, Size size, Color backColor, Action onClick)
        {
            var button = new Button();
            button.Text = text;
            button.Size = size;
            button.BackColor = backColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            button.Click += (s, e) => onClick?.Invoke();
            return button;
        }

        private void UpdateButtonStates()
        {
            bool enabled = isConnected && ethercatDevice != null;
            
            if (buttonChamberALampOn != null) buttonChamberALampOn.Enabled = enabled;
            if (buttonChamberALampOff != null) buttonChamberALampOff.Enabled = enabled;
            if (buttonChamberADoorOpen != null) buttonChamberADoorOpen.Enabled = enabled;
            if (buttonChamberADoorClose != null) buttonChamberADoorClose.Enabled = enabled;

            if (buttonChamberBLampOn != null) buttonChamberBLampOn.Enabled = enabled;
            if (buttonChamberBLampOff != null) buttonChamberBLampOff.Enabled = enabled;
            if (buttonChamberBDoorOpen != null) buttonChamberBDoorOpen.Enabled = enabled;
            if (buttonChamberBDoorClose != null) buttonChamberBDoorClose.Enabled = enabled;

            if (buttonChamberCLampOn != null) buttonChamberCLampOn.Enabled = enabled;
            if (buttonChamberCLampOff != null) buttonChamberCLampOff.Enabled = enabled;
            if (buttonChamberCDoorOpen != null) buttonChamberCDoorOpen.Enabled = enabled;
            if (buttonChamberCDoorClose != null) buttonChamberCDoorClose.Enabled = enabled;

            if (buttonMainLampRedOn != null) buttonMainLampRedOn.Enabled = enabled;
            if (buttonMainLampRedOff != null) buttonMainLampRedOff.Enabled = enabled;
            if (buttonMainLampYellowOn != null) buttonMainLampYellowOn.Enabled = enabled;
            if (buttonMainLampYellowOff != null) buttonMainLampYellowOff.Enabled = enabled;
            if (buttonMainLampGreenOn != null) buttonMainLampGreenOn.Enabled = enabled;
            if (buttonMainLampGreenOff != null) buttonMainLampGreenOff.Enabled = enabled;

            // 서보 모터 제어 버튼들 활성화 상태
            if (buttonServoOn != null) buttonServoOn.Enabled = enabled;
            if (buttonServoOff != null) buttonServoOff.Enabled = enabled;
            if (buttonServoHome != null) buttonServoHome.Enabled = enabled;
            if (buttonCylinderExtend != null) buttonCylinderExtend.Enabled = enabled;
            if (buttonCylinderRetract != null) buttonCylinderRetract.Enabled = enabled;
            if (buttonVacuumOn != null) buttonVacuumOn.Enabled = enabled;
            if (buttonVacuumOff != null) buttonVacuumOff.Enabled = enabled;
            if (buttonExhaustOn != null) buttonExhaustOn.Enabled = enabled;
            if (buttonExhaustOff != null) buttonExhaustOff.Enabled = enabled;

            // 일괄 제어 버튼들도 활성화 상태 업데이트
            var allDoorsOpen = this.Controls.Find("buttonAllDoorsOpen", true);
            var allDoorsClose = this.Controls.Find("buttonAllDoorsClose", true);
            var allLampsOn = this.Controls.Find("buttonAllLampsOn", true);
            var allLampsOff = this.Controls.Find("buttonAllLampsOff", true);

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is GroupBox)
                {
                    foreach (Control child in ctrl.Controls)
                    {
                        if (child is Button)
                        {
                            var btn = child as Button;
                            if (btn != null && (btn.Text == "모두 열기" || btn.Text == "모두 닫기" || 
                                btn.Text == "모두 켜기" || btn.Text == "모두 끄기"))
                            {
                                btn.Enabled = enabled;
                            }
                        }
                    }
                }
            }
        }

        private void ControlChamberLamp(int outputIndex, bool on)
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ethercatDevice.Digital_Output(outputIndex, on);
                
                EquipmentRegion? region = null;
                if (outputIndex == 3) region = EquipmentRegion.ChamberA;
                else if (outputIndex == 6) region = EquipmentRegion.ChamberB;
                else if (outputIndex == 9) region = EquipmentRegion.ChamberC;
                
                labelStatus.Text = $"Chamber {(outputIndex == 3 ? "A" : outputIndex == 6 ? "B" : "C")} 램프 {(on ? "ON" : "OFF")}";
                labelStatus.ForeColor = Color.LimeGreen;
                
                // Form1의 UI 업데이트를 위한 콜백 호출
                if (region.HasValue && OnChamberLampStateChanged != null)
                {
                    OnChamberLampStateChanged(region.Value, on);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"램프 제어 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlChamberDoor(int outputIndex1, int outputIndex2, bool open)
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string chamberName = "";
                EquipmentRegion? region = null;
                if (outputIndex1 == 4)
                {
                    chamberName = "A";
                    region = EquipmentRegion.ChamberA;
                }
                else if (outputIndex1 == 7)
                {
                    chamberName = "B";
                    region = EquipmentRegion.ChamberB;
                }
                else if (outputIndex1 == 10)
                {
                    chamberName = "C";
                    region = EquipmentRegion.ChamberC;
                }

                if (open)
                {
                    // Chamber A는 특별 처리 (4=false, 5=true)
                    if (outputIndex1 == 4)
                    {
                        ethercatDevice.Digital_Output(4, false);
                        ethercatDevice.Digital_Output(5, true);
                    }
                    else
                    {
                        ethercatDevice.Digital_Output(outputIndex2, true);
                        ethercatDevice.Digital_Output(outputIndex1, false);
                    }
                }
                else
                {
                    // Chamber A는 특별 처리 (4=true, 5=false)
                    if (outputIndex1 == 4)
                    {
                        ethercatDevice.Digital_Output(4, true);
                        ethercatDevice.Digital_Output(5, false);
                    }
                    else
                    {
                        ethercatDevice.Digital_Output(outputIndex1, true);
                        ethercatDevice.Digital_Output(outputIndex2, false);
                    }
                }

                labelStatus.Text = $"Chamber {chamberName} 도어 {(open ? "열림" : "닫힘")}";
                labelStatus.ForeColor = Color.LimeGreen;
                
                // Form1의 UI 업데이트를 위한 콜백 호출
                if (region.HasValue && OnDoorStateChanged != null)
                {
                    OnDoorStateChanged(region.Value, open);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"도어 제어 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlMainLamp(int outputIndex, bool on)
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ethercatDevice.Digital_Output(outputIndex, on);
                string colorName = outputIndex == 0 ? "적색" : outputIndex == 1 ? "황색" : "녹색";
                labelStatus.Text = $"3색 램프 {colorName} {(on ? "ON" : "OFF")}";
                labelStatus.ForeColor = Color.LimeGreen;
                
                // 3색 램프 상태 읽기 (현재 상태 확인)
                bool red = false, yellow = false, green = false;
                try
                {
                    red = ethercatDevice.Digital_Input(0);
                    yellow = ethercatDevice.Digital_Input(1);
                    green = ethercatDevice.Digital_Input(2);
                }
                catch { }
                
                // Form1의 UI 업데이트를 위한 콜백 호출
                if (OnMainLampStateChanged != null)
                {
                    OnMainLampStateChanged(red, yellow, green);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"3색 램프 제어 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlAllDoorsOpen()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Chamber A 도어 열기 (4=false, 5=true)
                ethercatDevice.Digital_Output(4, false);
                ethercatDevice.Digital_Output(5, true);

                // Chamber B 도어 열기 (7=false, 8=true)
                ethercatDevice.Digital_Output(7, false);
                ethercatDevice.Digital_Output(8, true);

                // Chamber C 도어 열기 (10=false, 11=true)
                ethercatDevice.Digital_Output(10, false);
                ethercatDevice.Digital_Output(11, true);

                labelStatus.Text = "모든 문이 열렸습니다.";
                labelStatus.ForeColor = Color.LimeGreen;
                
                // Form1의 UI 업데이트를 위한 콜백 호출
                if (OnDoorStateChanged != null)
                {
                    OnDoorStateChanged(EquipmentRegion.ChamberA, true);
                    OnDoorStateChanged(EquipmentRegion.ChamberB, true);
                    OnDoorStateChanged(EquipmentRegion.ChamberC, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"모든 문 열기 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlAllDoorsClose()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Chamber A 도어 닫기 (4=true, 5=false)
                ethercatDevice.Digital_Output(4, true);
                ethercatDevice.Digital_Output(5, false);

                // Chamber B 도어 닫기 (7=true, 8=false)
                ethercatDevice.Digital_Output(7, true);
                ethercatDevice.Digital_Output(8, false);

                // Chamber C 도어 닫기 (10=true, 11=false)
                ethercatDevice.Digital_Output(10, true);
                ethercatDevice.Digital_Output(11, false);

                labelStatus.Text = "모든 문이 닫혔습니다.";
                labelStatus.ForeColor = Color.LimeGreen;
                
                // Form1의 UI 업데이트를 위한 콜백 호출
                if (OnDoorStateChanged != null)
                {
                    OnDoorStateChanged(EquipmentRegion.ChamberA, false);
                    OnDoorStateChanged(EquipmentRegion.ChamberB, false);
                    OnDoorStateChanged(EquipmentRegion.ChamberC, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"모든 문 닫기 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlAllLampsOn()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Chamber A, B, C 램프 켜기
                ethercatDevice.Digital_Output(3, true);  // Chamber A
                ethercatDevice.Digital_Output(6, true);  // Chamber B
                ethercatDevice.Digital_Output(9, true);  // Chamber C

                // 3색 램프 모두 켜기
                ethercatDevice.Digital_Output(0, true);  // 적색
                ethercatDevice.Digital_Output(1, true);  // 황색
                ethercatDevice.Digital_Output(2, true);  // 녹색

                labelStatus.Text = "모든 램프가 켜졌습니다.";
                labelStatus.ForeColor = Color.LimeGreen;
                
                // Form1의 UI 업데이트를 위한 콜백 호출
                if (OnChamberLampStateChanged != null)
                {
                    OnChamberLampStateChanged(EquipmentRegion.ChamberA, true);
                    OnChamberLampStateChanged(EquipmentRegion.ChamberB, true);
                    OnChamberLampStateChanged(EquipmentRegion.ChamberC, true);
                }
                
                // 3색 램프 상태 읽기
                bool red = false, yellow = false, green = false;
                try
                {
                    red = ethercatDevice.Digital_Input(0);
                    yellow = ethercatDevice.Digital_Input(1);
                    green = ethercatDevice.Digital_Input(2);
                }
                catch { }
                
                if (OnMainLampStateChanged != null)
                {
                    OnMainLampStateChanged(red, yellow, green);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"모든 램프 켜기 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlAllLampsOff()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Chamber A, B, C 램프 끄기
                ethercatDevice.Digital_Output(3, false);  // Chamber A
                ethercatDevice.Digital_Output(6, false);  // Chamber B
                ethercatDevice.Digital_Output(9, false);  // Chamber C

                // 3색 램프 모두 끄기
                ethercatDevice.Digital_Output(0, false);  // 적색
                ethercatDevice.Digital_Output(1, false);  // 황색
                ethercatDevice.Digital_Output(2, false);  // 녹색

                labelStatus.Text = "모든 램프가 꺼졌습니다.";
                labelStatus.ForeColor = Color.LimeGreen;
                
                // Form1의 UI 업데이트를 위한 콜백 호출
                if (OnChamberLampStateChanged != null)
                {
                    OnChamberLampStateChanged(EquipmentRegion.ChamberA, false);
                    OnChamberLampStateChanged(EquipmentRegion.ChamberB, false);
                    OnChamberLampStateChanged(EquipmentRegion.ChamberC, false);
                }
                
                // 3색 램프 상태 읽기
                bool red = false, yellow = false, green = false;
                try
                {
                    red = ethercatDevice.Digital_Input(0);
                    yellow = ethercatDevice.Digital_Input(1);
                    green = ethercatDevice.Digital_Input(2);
                }
                catch { }
                
                if (OnMainLampStateChanged != null)
                {
                    OnMainLampStateChanged(red, yellow, green);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"모든 램프 끄기 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        public void UpdateConnectionStatus(bool connected)
        {
            isConnected = connected;
            if (labelStatus != null)
            {
                labelStatus.Text = connected ? "EtherCAT 연결됨" : "EtherCAT 미연결";
                labelStatus.ForeColor = connected ? Color.LimeGreen : Color.Red;
            }
            UpdateButtonStates();
        }

        #region Servo Motor Control Methods

        /// <summary>
        /// 현재 서보 상태를 장치에서 읽어서 표시
        /// </summary>
        private void UpdateServoStatusFromDevice()
        {
            if (!isConnected || ethercatDevice == null)
            {
                if (labelServoStatus != null)
                {
                    labelServoStatus.Text = "서보 상태: 미연결";
                    labelServoStatus.ForeColor = Color.FromArgb(100, 100, 100);
                }
                return;
            }

            try
            {
                // 원점복귀 상태 확인
                bool axis1Home = ethercatDevice.Axis1_Status("HOME_D");
                bool axis2Home = ethercatDevice.Axis2_Status("HOME_D");
                bool isHomed = axis1Home && axis2Home;

                // 위치 데이터 확인 (서보 ON 여부 추정)
                string axis1Pos = ethercatDevice.Axis1_is_PosData();
                bool hasPosition = !string.IsNullOrEmpty(axis1Pos) && axis1Pos != "-";

                if (labelServoStatus != null)
                {
                    if (isHomed)
                    {
                        labelServoStatus.Text = "서보 상태: ON+Homed";
                        labelServoStatus.ForeColor = Color.LimeGreen;
                    }
                    else if (hasPosition)
                    {
                        labelServoStatus.Text = "서보 상태: ON";
                        labelServoStatus.ForeColor = Color.Orange;
                    }
                    else
                    {
                        labelServoStatus.Text = "서보 상태: 대기";
                        labelServoStatus.ForeColor = Color.FromArgb(100, 100, 100);
                    }
                }
            }
            catch
            {
                if (labelServoStatus != null)
                {
                    labelServoStatus.Text = "서보 상태: 확인 불가";
                    labelServoStatus.ForeColor = Color.FromArgb(100, 100, 100);
                }
            }
        }

        private void ControlServoOn()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 파라미터 설정 (velocity, maxVelocity, deceleration, acceleration)
                long velocity = 300000;
                long maxVelocity = 500000;
                long deceleration = 100000000;
                long acceleration = 1000000;
                
                ethercatDevice.Axis1_UD_Config_Update(velocity, maxVelocity, deceleration, acceleration);
                ethercatDevice.Axis2_LR_Config_Update(velocity, maxVelocity, deceleration, acceleration);
                
                // 서보 ON
                ethercatDevice.Axis1_ON();
                ethercatDevice.Axis2_ON();
                
                // 서보 ON 완료 대기 (최대 2초)
                System.Threading.Thread.Sleep(500); // 초기 안정화 대기
                bool servoOnConfirmed = false;
                var servoCheckTimeout = DateTime.Now.AddSeconds(2);
                while (DateTime.Now < servoCheckTimeout)
                {
                    try
                    {
                        // 서보 ON 상태 확인 (위치 데이터가 있으면 서보 ON으로 간주)
                        string axis1Pos = ethercatDevice.Axis1_is_PosData();
                        string axis2Pos = ethercatDevice.Axis2_is_PosData();
                        bool hasPosition = !string.IsNullOrEmpty(axis1Pos) && axis1Pos != "-" &&
                                          !string.IsNullOrEmpty(axis2Pos) && axis2Pos != "-";
                        
                        if (hasPosition)
                        {
                            servoOnConfirmed = true;
                            break;
                        }
                    }
                    catch (Exception checkEx)
                    {
                        // 상태 확인 오류는 무시하고 계속 시도
                        System.Diagnostics.Debug.WriteLine($"서보 ON 상태 확인 오류: {checkEx.Message}");
                    }
                    System.Threading.Thread.Sleep(100);
                }
                
                if (servoOnConfirmed)
                {
                    labelStatus.Text = "서보 모터 ON";
                    labelStatus.ForeColor = Color.LimeGreen;
                    if (labelServoStatus != null)
                    {
                        labelServoStatus.Text = "서보 상태: ON";
                        labelServoStatus.ForeColor = Color.LimeGreen;
                    }
                    OnServoStateChanged?.Invoke(true);
                }
                else
                {
                    labelStatus.Text = "서보 ON 명령 전송 완료, 상태 확인 실패";
                    labelStatus.ForeColor = Color.Orange;
                    if (labelServoStatus != null)
                    {
                        labelServoStatus.Text = "서보 상태: 확인 불가";
                        labelServoStatus.ForeColor = Color.Orange;
                    }
                    MessageBox.Show(
                        "서보 ON 명령을 전송했지만 상태 확인에 실패했습니다.\n\n" +
                        "확인 사항:\n" +
                        "1. 서보 모터 전원 확인\n" +
                        "2. EtherCAT 연결 상태 확인\n" +
                        "3. 서보 모터 상태 수동 확인",
                        "서보 ON 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    OnServoStateChanged?.Invoke(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서보 ON 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlServoOff()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ethercatDevice.Axis1_OFF();
                ethercatDevice.Axis2_OFF();

                labelStatus.Text = "서보 모터 OFF";
                labelStatus.ForeColor = Color.Orange;
                if (labelServoStatus != null)
                {
                    labelServoStatus.Text = "서보 상태: OFF";
                    labelServoStatus.ForeColor = Color.Orange;
                }

                OnServoStateChanged?.Invoke(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서보 OFF 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlServoHome()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 실린더 후진 상태 확인
            try
            {
                bool cylinderRetracted = ethercatDevice.Digital_Input(12);
                if (!cylinderRetracted)
                {
                    MessageBox.Show("실린더가 후진 상태가 아닙니다.\n먼저 실린더를 후진시켜 주세요.", "인터락", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"실린더 상태 확인 오류: {ex.Message}\n원점복귀를 진행합니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            try
            {
                // 원점복귀 시작
                labelStatus.Text = "원점복귀 시작 - 1단계: 상하(Axis1)";
                labelStatus.ForeColor = Color.Yellow;
                if (labelServoStatus != null)
                {
                    labelServoStatus.Text = "원점복귀 중 (1/2)...";
                    labelServoStatus.ForeColor = Color.Yellow;
                }

                // 1단계: Axis1 (상하) 원점복귀
                ethercatDevice.Axis1_UD_Homming();

                // Axis1 원점복귀 완료 대기 (최대 120초)
                var timeout = DateTime.Now.AddSeconds(120);
                bool axis1Homed = false;
                while (DateTime.Now < timeout)
                {
                    try
                    {
                        if (ethercatDevice.Axis1_Status("HOME_D"))
                        {
                            axis1Homed = true;
                            break;
                        }
                    }
                    catch (Exception checkEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Axis1 상태 확인 오류: {checkEx.Message}");
                    }
                    System.Threading.Thread.Sleep(100);
 // UI 응답성 유지
                }

                if (!axis1Homed)
                {
                    labelStatus.Text = "원점복귀 실패: Axis1 타임아웃";
                    labelStatus.ForeColor = Color.Red;
                    if (labelServoStatus != null)
                    {
                        labelServoStatus.Text = "원점복귀 실패";
                        labelServoStatus.ForeColor = Color.Red;
                    }
                    MessageBox.Show(
                        "상하(Axis1) 원점복귀가 타임아웃되었습니다.\n\n" +
                        "확인 사항:\n" +
                        "1. 서보 모터 전원 확인\n" +
                        "2. Axis1 하드웨어 상태 확인\n" +
                        "3. EtherCAT 연결 상태 확인",
                        "원점복귀 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2단계: Axis2 (좌우) 원점복귀
                labelStatus.Text = "원점복귀 - 2단계: 좌우(Axis2)";
                if (labelServoStatus != null)
                {
                    labelServoStatus.Text = "원점복귀 중 (2/2)...";
                }

                ethercatDevice.Axis2_LR_Homming();

                // Axis2 원점복귀 완료 대기 (최대 120초)
                timeout = DateTime.Now.AddSeconds(120);
                bool axis2Homed = false;
                while (DateTime.Now < timeout)
                {
                    try
                    {
                        if (ethercatDevice.Axis2_Status("HOME_D"))
                        {
                            axis2Homed = true;
                            break;
                        }
                    }
                    catch (Exception checkEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Axis2 상태 확인 오류: {checkEx.Message}");
                    }
                    System.Threading.Thread.Sleep(100);
 // UI 응답성 유지
                }

                if (!axis2Homed)
                {
                    labelStatus.Text = "원점복귀 실패: Axis2 타임아웃";
                    labelStatus.ForeColor = Color.Red;
                    if (labelServoStatus != null)
                    {
                        labelServoStatus.Text = "원점복귀 실패";
                        labelServoStatus.ForeColor = Color.Red;
                    }
                    MessageBox.Show(
                        "좌우(Axis2) 원점복귀가 타임아웃되었습니다.\n\n" +
                        "확인 사항:\n" +
                        "1. 서보 모터 전원 확인\n" +
                        "2. Axis2 하드웨어 상태 확인\n" +
                        "3. 실린더 후진 상태 확인\n" +
                        "4. EtherCAT 연결 상태 확인",
                        "원점복귀 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 원점복귀 완료
                labelStatus.Text = "원점복귀 완료!";
                labelStatus.ForeColor = Color.LimeGreen;
                if (labelServoStatus != null)
                {
                    labelServoStatus.Text = "원점복귀 완료";
                    labelServoStatus.ForeColor = Color.LimeGreen;
                }

                OnHomingRequested?.Invoke();
                MessageBox.Show("원점복귀가 완료되었습니다.\n(상하 → 좌우 순서)", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"원점복귀 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
                if (labelServoStatus != null)
                {
                    labelServoStatus.Text = "원점복귀 오류";
                    labelServoStatus.ForeColor = Color.Red;
                }
            }
        }

        private void ControlCylinderExtend()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ethercatDevice.Digital_Output(13, false); // 후진 OFF
                ethercatDevice.Digital_Output(12, true);  // 전진 ON

                labelStatus.Text = "실린더 전진";
                labelStatus.ForeColor = Color.LimeGreen;
                OnCylinderStateChanged?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"실린더 전진 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlCylinderRetract()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ethercatDevice.Digital_Output(12, false); // 전진 OFF
                ethercatDevice.Digital_Output(13, true);  // 후진 ON

                labelStatus.Text = "실린더 후진";
                labelStatus.ForeColor = Color.LimeGreen;
                OnCylinderStateChanged?.Invoke(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"실린더 후진 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlVacuumOn()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ethercatDevice.Digital_Output(15, false); // 배기 OFF
                ethercatDevice.Digital_Output(14, true);  // 진공 ON

                labelStatus.Text = "진공 ON (흡착)";
                labelStatus.ForeColor = Color.LimeGreen;
                OnVacuumStateChanged?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"진공 ON 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlVacuumOff()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ethercatDevice.Digital_Output(14, false); // 진공 OFF

                labelStatus.Text = "진공 OFF";
                labelStatus.ForeColor = Color.Orange;
                OnVacuumStateChanged?.Invoke(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"진공 OFF 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlExhaustOn()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ethercatDevice.Digital_Output(14, false); // 진공 OFF
                ethercatDevice.Digital_Output(15, true);  // 배기 ON

                labelStatus.Text = "배기 ON";
                labelStatus.ForeColor = Color.FromArgb(156, 39, 176);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"배기 ON 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ControlExhaustOff()
        {
            if (!isConnected || ethercatDevice == null)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ethercatDevice.Digital_Output(15, false); // 배기 OFF

                labelStatus.Text = "배기 OFF";
                labelStatus.ForeColor = Color.Orange;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"배기 OFF 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = $"오류: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        #endregion
    }
}

