using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using IEG3268_Dll;
using SemiconductorUi.Helpers;
using SemiconductorUi.ViewModels;
using SemiconductorUi.Controllers;
using SemiconductorUi.Repositories;
using SemiconductorUi.Models;
using SemiconductorUi.Services;
using SemiconductorUi.Controls;
using SemiconductorUi.Forms;
using SemiconductorUi.EventHandlers;

namespace SemiconductorUi
{
    // ProcessState와 WaferLoadStateType는 ViewModel의 enum을 직접 사용
    using ProcessState = ViewModels.MainFormViewModel.ProcessState;
    using MainFormViewModel = ViewModels.MainFormViewModel;
    
    public partial class Form1 : Form
    {
        // ProcessState는 ViewModel의 enum을 사용 (타입 별칭)
        // WaferLoadStateType는 MainFormViewModel.WaferLoadStateType로 직접 사용

        // 설정값은 AppSettings 클래스에서 읽어옴 (App.config 참조)
        // 하드웨어 모드와 시뮬레이션 모드에서 다른 타이머 주기 사용
        // 색상 값은 AppSettings에서 읽어옴

        // IsLoggedIn, CurrentUser, CurrentRole, CurrentProcessState, hasAlarm, FOUP 상태, 상태 텍스트 등은 이제 ViewModel 속성으로 관리됨 (아래 속성 참조)
        // ProcessState enum은 ViewModel의 MainFormViewModel.ProcessState 사용
        internal readonly Dictionary<string, PmDetailData> PmDetails = new Dictionary<string, PmDetailData>();
        internal readonly Dictionary<string, TableLayoutPanel> PmEnvTables = new Dictionary<string, TableLayoutPanel>();
        // FOUP 상태 필드들은 ViewModel 속성으로 관리됨
        // 개별 웨이퍼 트랙 패널 (1~5층)
        internal Panel[] foupATrackPanels = new Panel[5];
        internal Panel[] foupBTrackPanels = new Panel[5];
        internal Button buttonMountFoupA;
        internal Button buttonMountFoupB;
        // foupVisualizationControlA와 foupVisualizationControlB는 Designer.cs에서 정의됨
        // demoSetupApplied, demoSimulationStarted, WaferLoadState 등은 ViewModel 속성으로 관리됨
        // 로그 엔트리는 LoggerService를 통해 관리 (하위 호환성을 위한 속성)
        private List<string> logEntries => LoggerService?.GetLogEntries() ?? new List<string>();
        internal readonly Dictionary<Control, Color> originalPanelColors = new Dictionary<Control, Color>();
        internal readonly Timer HeaderClockTimer;
        internal readonly Timer HardwareUiUpdateTimer; // UI 전용 고속 타이머 (50ms 주기)
        internal static readonly ChamberController.SimulationStepDefinition[] ChamberProcessFlow =
        {
            new ChamberController.SimulationStepDefinition("PR 도포", 30),
            new ChamberController.SimulationStepDefinition("노광 (Exposure-B)", 45),
            new ChamberController.SimulationStepDefinition("노광 (Exposure-C)", 45)
        };
        // FOUP 관련 필드는 FoupManager로 이동됨 (하위 호환성을 위한 속성 제공)
        internal Queue<Wafer> FoupAQueue => foupManager?.FoupAQueue;
        internal List<Wafer> FoupBCompleted => foupManager?.FoupBCompleted;
        internal int FoupBCompletedBaseline
        {
            get => foupManager?.FoupBCompletedBaseline ?? 0;
            set
            {
                if (foupManager != null)
                {
                    foupManager.FoupBCompletedBaseline = value;
                }
            }
        }
        
        // UI 업데이트 타이머 에러 로그 플래그 (로그 스팸 방지용)
        internal bool _lastHardwareUiUpdateErrorLogged = false;
        
        // 하드웨어 상태 동기화 카운터 (주기적 동기화용)
        internal int _hardwareSyncCounter = 0;
        
        // 알람 발생 전 프로세스 상태 저장 (알람 리셋 시 원래 상태로 복귀하기 위함)
        internal ProcessState? _stateBeforeAlarm = null;
        
        // 서비스 인터페이스 (의존성 주입)
        internal IChamberService ChamberService;
        internal ITransferService TransferService;
        internal ISimulationService SimulationService;
        internal ILoggerService LoggerService;
        
        // UI 상태 관리 ViewModel
        internal MainFormViewModel ViewModel;
        
        // 하드웨어 관리
        internal HardwareManager HardwareManager;
        
        // 이벤트 핸들러 인스턴스
        private readonly LoginEventHandlers loginEventHandlers;
        private readonly ProcessEventHandlers processEventHandlers;
        private readonly WaferEventHandlers waferEventHandlers;
        private readonly NavigationEventHandlers navigationEventHandlers;
        private readonly EquipmentEventHandlers equipmentEventHandlers;
        private readonly HardwareEventHandlers hardwareEventHandlers;
        internal readonly TimerEventHandlers timerEventHandlers;
        private readonly PaintEventHandlers paintEventHandlers;
        private readonly FormEventHandlers formEventHandlers;
        
        // UI 업데이트 헬퍼
        internal readonly Helpers.Form1UiUpdater uiUpdater;
        
        // UI 초기화 헬퍼
        internal readonly Helpers.Form1Initializer uiInitializer;
        
        // UI 설정 헬퍼
        internal readonly Helpers.Form1Configurator uiConfigurator;
        
        // TM 프로세스 헬퍼
        internal readonly Helpers.Form1TmProcessor tmProcessor;
        
        // FOUP 관리 헬퍼
        internal readonly Helpers.FoupManager foupManager;
        
        // 알람 관리 헬퍼
        internal readonly Helpers.AlarmManager alarmManager;
        
        // 웨이퍼 트래킹 서비스
        internal readonly Helpers.WaferTrackingService waferTrackingService;
        
        // Designer.cs에서 필드들이 internal로 변경됨 (헬퍼 클래스 접근용)
        
        // 하위 호환성을 위한 속성 (ViewModel로 매핑)
        // 점진적 마이그레이션을 위해 기존 필드를 ViewModel 속성으로 매핑
        internal bool IsLoggedIn
        {
            get => ViewModel?.IsLoggedIn ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.IsLoggedIn = value;
                }
            }
        }
        
        internal string CurrentUser
        {
            get => ViewModel?.CurrentUser ?? "Guest";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.CurrentUser = value;
                }
            }
        }
        
        internal string CurrentRole
        {
            get => ViewModel?.CurrentRole ?? "없음";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.CurrentRole = value;
                }
            }
        }
        
        // ProcessState는 ViewModel의 enum을 사용 (하위 호환성을 위해 타입 별칭)
        internal MainFormViewModel.ProcessState CurrentProcessState
        {
            get => ViewModel?.CurrentProcessState ?? MainFormViewModel.ProcessState.Idle;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.CurrentProcessState = value;
                }
            }
        }
        
        internal bool HasAlarm
        {
            get => ViewModel?.HasAlarm ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.HasAlarm = value;
                }
            }
        }
        
        // ChamberAlarmStatus는 Dictionary이므로 직접 접근 가능하도록 래퍼 제공
        internal Dictionary<string, bool> ChamberAlarmStatus => ViewModel?.ChamberAlarmStatus ?? new Dictionary<string, bool>();
        
        internal bool VerificationAlarmDismissed
        {
            get => ViewModel?.VerificationAlarmDismissed ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.VerificationAlarmDismissed = value;
                }
            }
        }
        
        internal string LastSelectedRecipe
        {
            get => ViewModel?.LastSelectedRecipe ?? "Default Recipe";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.LastSelectedRecipe = value;
                }
            }
        }
        
        // FOUP 상태 속성
        internal int currentFoupACount
        {
            get => ViewModel?.CurrentFoupACount ?? 0;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.CurrentFoupACount = value;
                }
            }
        }
        
        internal int currentFoupBCount
        {
            get => ViewModel?.CurrentFoupBCount ?? 0;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.CurrentFoupBCount = value;
                }
            }
        }
        
        internal string CurrentFoupExchangeState
        {
            get => ViewModel?.CurrentFoupExchangeState ?? "교환 상태: Standby";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.CurrentFoupExchangeState = value;
                }
            }
        }
        
        internal string CurrentFoupQueueInfo
        {
            get => ViewModel?.CurrentFoupQueueInfo ?? "대기 FOUP: 없음";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.CurrentFoupQueueInfo = value;
                }
            }
        }
        
        internal string currentFoupAStatusText
        {
            get => ViewModel?.CurrentFoupAStatusText ?? "대기";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.CurrentFoupAStatusText = value;
                }
            }
        }
        
        internal string currentFoupBStatusText
        {
            get => ViewModel?.CurrentFoupBStatusText ?? "대기";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.CurrentFoupBStatusText = value;
                }
            }
        }
        
        internal bool IsFoupMounted
        {
            get => ViewModel?.IsFoupMounted ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.IsFoupMounted = value;
                }
            }
        }
        
        internal bool IsFoupAMounted
        {
            get => ViewModel?.IsFoupAMounted ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.IsFoupAMounted = value;
                }
            }
        }
        
        internal bool IsFoupBMounted
        {
            get => ViewModel?.IsFoupBMounted ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.IsFoupBMounted = value;
                }
            }
        }
        
        internal int ConfiguredFoupALoadCount
        {
            get => ViewModel?.ConfiguredFoupALoadCount ?? 0;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.ConfiguredFoupALoadCount = value;
                }
            }
        }
        
        internal int FoupARemainingInventoryCount
        {
            get => ViewModel?.FoupARemainingInventoryCount ?? 0;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.FoupARemainingInventoryCount = value;
                }
            }
        }
        
        // 웨이퍼 로드 상태
        internal MainFormViewModel.WaferLoadStateType WaferLoadState
        {
            get => ViewModel?.WaferLoadState ?? MainFormViewModel.WaferLoadStateType.None;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.WaferLoadState = value;
                }
            }
        }
        
        internal int UserWaferLoadCount
        {
            get => ViewModel?.UserWaferLoadCount ?? AppSettings.MaxFoupCapacity;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.UserWaferLoadCount = value;
                }
            }
        }
        
        internal int activeBatchTargetCount
        {
            get => ViewModel?.ActiveBatchTargetCount ?? 0;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.ActiveBatchTargetCount = value;
                }
            }
        }
        
        internal int CurrentRecipeWaferCount
        {
            get => ViewModel?.CurrentRecipeWaferCount ?? AppSettings.MaxFoupCapacity;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.CurrentRecipeWaferCount = value;
                }
            }
        }
        
        // 상태 텍스트 속성
        internal string StatusProcessText
        {
            get => ViewModel?.StatusProcessText ?? "공정 상태: 대기";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.StatusProcessText = value;
                }
            }
        }
        
        internal string StatusProcessDetail
        {
            get => ViewModel?.StatusProcessDetail ?? "공정 상태: 대기";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.StatusProcessDetail = value;
                }
            }
        }
        
        private string statusPressureText
        {
            get => ViewModel?.StatusPressureText ?? "챔버 압력: 안정";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.StatusPressureText = value;
                }
            }
        }
        
        private string statusTemperatureText
        {
            get => ViewModel?.StatusTemperatureText ?? "온도: 정상";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.StatusTemperatureText = value;
                }
            }
        }
        
        internal string StatusDoorText
        {
            get => ViewModel?.StatusDoorText ?? "문 상태: 모두 닫힘";
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.StatusDoorText = value;
                }
            }
        }
        
        // 챔버 완료 수 속성
        internal int chamberCompletedCountA
        {
            get => ViewModel?.ChamberCompletedCountA ?? 0;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.ChamberCompletedCountA = value;
                }
            }
        }
        
        internal int chamberCompletedCountB
        {
            get => ViewModel?.ChamberCompletedCountB ?? 0;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.ChamberCompletedCountB = value;
                }
            }
        }
        
        internal int chamberCompletedCountC
        {
            get => ViewModel?.ChamberCompletedCountC ?? 0;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.ChamberCompletedCountC = value;
                }
            }
        }
        
        // Transfer Module 상태 속성
        internal EquipmentRegion TmVisualTarget
        {
            get => ViewModel?.TmVisualTarget ?? EquipmentRegion.TM;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.TmVisualTarget = value;
                }
            }
        }
        
        internal bool TmCarryingVisual
        {
            get => ViewModel?.TmCarryingVisual ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.TmCarryingVisual = value;
                }
            }
        }
        
        internal EquipmentRegion TmCurrentPosition
        {
            get => ViewModel?.TmCurrentPosition ?? EquipmentRegion.TM;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.TmCurrentPosition = value;
                }
            }
        }
        
        internal float TmBladeExtensionFactor
        {
            get => ViewModel?.TmBladeExtensionFactor ?? 0.55f;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.TmBladeExtensionFactor = value;
                }
            }
        }
        
        // 시뮬레이션 및 데모 속성
        private bool demoSetupApplied
        {
            get => ViewModel?.DemoSetupApplied ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.DemoSetupApplied = value;
                }
            }
        }
        
        private bool demoSimulationStarted
        {
            get => ViewModel?.DemoSimulationStarted ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.DemoSimulationStarted = value;
                }
            }
        }
        
        // UI 상태 속성
        internal bool mainLampBlinkState
        {
            get => ViewModel?.MainLampBlinkState ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.MainLampBlinkState = value;
                }
            }
        }
        
        internal bool SecondExposureEnabled
        {
            get => ViewModel?.SecondExposureEnabled ?? false;
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.SecondExposureEnabled = value;
                }
            }
        }
        
        // 하위 호환성을 위한 속성 (인터페이스를 통해 접근)
        private ChamberController chamberController => ChamberService as ChamberController;
        internal ChamberController.ChamberState ChamberAState => ChamberService?.ChamberA;
        internal ChamberController.ChamberState ChamberBState => ChamberService?.ChamberB;
        internal ChamberController.ChamberState ChamberCState => ChamberService?.ChamberC;
        
        internal TransferController TransferController => TransferService as TransferController;
        
        internal SimulationController SimulationController => SimulationService as SimulationController;
        internal Timer SimulationTimer => SimulationService?.SimulationTimer;
        internal bool SimulationRunning => SimulationService?.IsRunning ?? false;
        internal bool SimulationPaused => SimulationService?.IsPaused ?? false;
        private int simulationElapsedSeconds => SimulationService?.ElapsedSeconds ?? 0;
        
        // PM 상태 표시용 커스텀 패널 (깜빡임 방지)
        internal PmStatusPanel pmStatusPanelA;
        internal PmStatusPanel pmStatusPanelB;
        internal PmStatusPanel pmStatusPanelC;
        // CurrentRecipeWaferCount, UserWaferLoadCount, activeBatchTargetCount, status 텍스트 필드들,
        // chamberCompletedCount, tm 상태 필드들, mainLampBlinkState, SecondExposureEnabled 등은
        // ViewModel 속성으로 관리됨 (위의 속성 래퍼 참조)
        // TmPhase enum은 TransferController로 분리됨
        // - TransferController.TmPhase 사용
        // TransferTask와 TmPhase는 TransferController로 분리됨
        // - TransferController.cs
        
        // 하위 호환성을 위한 속성 (ITransferService를 통해 접근)
        internal TransferController.TransferTask CurrentTransfer => TransferService?.CurrentTransfer;
        internal TransferController.TmPhase TmPhase => TransferService?.CurrentPhase ?? TransferController.TmPhase.Idle;
        internal int TmPhaseTicksRemaining => TransferService?.PhaseTicksRemaining ?? 0;
        
        // doorOpenStates는 ViewModel의 DoorOpenStates로 이동됨
        private readonly Panel[] chamberWaferPanels;
        
        // 하드웨어 관련 속성 (HardwareManager로 매핑)
        internal bool EthercatConnected
        {
            get => HardwareManager?.IsEthercatConnected ?? false;
            set
            {
                // 읽기 전용 속성이므로 setter는 제거하거나 무시
                // 연결/해제는 HardwareManager 메서드를 통해 수행
            }
        }
        
        // EtherCAT 인스턴스는 필드로 유지 (초기화 필요)
        internal IEG3268 EtherCAT_M;
        
        // 장비 상태 동기화 플래그
        internal bool isSyncingEquipmentState = false;
        
        internal TmHardwareController TmHardwareController => HardwareManager?.TmHardwareController;
        
        internal bool TmHardwareInitialized
        {
            get => HardwareManager?.IsTmHardwareInitialized ?? false;
            set
            {
                // 읽기 전용이지만 리셋이 필요한 경우를 위해
                if (value == false && HardwareManager != null)
                {
                    HardwareManager.ResetTmHardwareInitialized();
                }
            }
        }
        
        // ChamberEnvironmentSpec과 ChamberEnvironmentLive는 ChamberController의 클래스 사용
        internal readonly Dictionary<string, ChamberController.ChamberEnvironmentSpec> ChamberEnvSpecs = new Dictionary<string, ChamberController.ChamberEnvironmentSpec>
        {
            { "PMA", new ChamberController.ChamberEnvironmentSpec("23.0 ±0.3°C (Spin chuck)", "760 Torr (대기압)", "45 ±5% RH", "PR 도포 · Solvent purge 2.0 m/s", 23.0, 760.0, 45.0) },
            { "PMB", new ChamberController.ChamberEnvironmentSpec("22.5 ±0.2°C (Stage)", "5×10⁻³ Torr (노광 챔버)", "<5% RH (건조 N₂)", "ArF 193nm Dose 120 mJ/cm²", 22.5, 0.005, 5.0) },
            { "PMC", new ChamberController.ChamberEnvironmentSpec("110°C PEB Plate", "50 Torr (N₂ 퍼지)", "<3% RH", "2차 노광/후베이크 · N₂ flow 20 slm", 110.0, 50.0, 3.0) }
        };
        internal readonly Dictionary<string, ChamberController.ChamberEnvironmentLive> ChamberEnvLive = new Dictionary<string, ChamberController.ChamberEnvironmentLive>();
        // 가스/RF Setpoint(레시피/사용자 설정) 저장
        internal readonly Dictionary<string, (double NF3, double O2, double CF4, double RF)> UnitGasRfSv = new Dictionary<string, (double, double, double, double)>
        {
            { "PMA", (200, 200, 200, 1000) },
            { "PMB", (0, 0, 0, 0) },
            { "PMC", (0, 0, 0, 0) }
        };

        // 알람 임계값
        internal EnvAlarmThresholds AlarmThresholds = EnvAlarmThresholds.CreateDefault();
        
        // 레시피 적용 여부 추적
        internal bool RecipeApplied = false;

        private void LoadAlarmThresholds()
        {
            try
            {
                var snapshot = ConfigRepository.Load();
                AlarmThresholds = new EnvAlarmThresholds
                {
                    TempWarnDiffC = snapshot.TempWarn,
                    TempAlarmDiffC = snapshot.TempAlarm,
                    PressWarnRatio = snapshot.PressWarnRatio,
                    PressAlarmRatio = snapshot.PressAlarmRatio,
                    PressWarnAbsTorr = snapshot.PressWarnAbs,
                    PressAlarmAbsTorr = snapshot.PressAlarmAbs,
                    RfWarnRatio = snapshot.RfWarnRatio,
                    RfAlarmRatio = snapshot.RfAlarmRatio,
                    GasWarnAbsSccm = snapshot.GasWarn,
                    GasAlarmAbsSccm = snapshot.GasAlarm,
                    GasLeakWarnSccm = snapshot.GasLeakWarn,
                    GasLeakAlarmSccm = snapshot.GasLeakAlarm
                };
            }
            catch
            {
                // 기본값 사용
                AlarmThresholds = EnvAlarmThresholds.CreateDefault();
            }
            
            // AlarmManager에 임계값 동기화 (struct는 값으로 복사)
            var thresholdsCopy = AlarmThresholds;
            alarmManager?.SetThresholds(thresholdsCopy);
        }

        internal EnvThresholdSnapshot GetAlarmThresholdsSnapshot()
        {
            return new EnvThresholdSnapshot
            {
                TempWarn = AlarmThresholds.TempWarnDiffC,
                TempAlarm = AlarmThresholds.TempAlarmDiffC,
                PressWarnRatio = AlarmThresholds.PressWarnRatio,
                PressAlarmRatio = AlarmThresholds.PressAlarmRatio,
                PressWarnAbs = AlarmThresholds.PressWarnAbsTorr,
                PressAlarmAbs = AlarmThresholds.PressAlarmAbsTorr,
                RfWarnRatio = AlarmThresholds.RfWarnRatio,
                RfAlarmRatio = AlarmThresholds.RfAlarmRatio,
                GasWarn = AlarmThresholds.GasWarnAbsSccm,
                GasAlarm = AlarmThresholds.GasAlarmAbsSccm,
                GasLeakWarn = AlarmThresholds.GasLeakWarnSccm,
                GasLeakAlarm = AlarmThresholds.GasLeakAlarmSccm
            };
        }

        internal void SetAlarmThresholdsFromSnapshot(EnvThresholdSnapshot snapshot)
        {
            AlarmThresholds = new EnvAlarmThresholds
            {
                TempWarnDiffC = snapshot.TempWarn,
                TempAlarmDiffC = snapshot.TempAlarm,
                PressWarnRatio = snapshot.PressWarnRatio,
                PressAlarmRatio = snapshot.PressAlarmRatio,
                PressWarnAbsTorr = snapshot.PressWarnAbs,
                PressAlarmAbsTorr = snapshot.PressAlarmAbs,
                RfWarnRatio = snapshot.RfWarnRatio,
                RfAlarmRatio = snapshot.RfAlarmRatio,
                GasWarnAbsSccm = snapshot.GasWarn,
                GasAlarmAbsSccm = snapshot.GasAlarm,
                GasLeakWarnSccm = snapshot.GasLeakWarn,
                GasLeakAlarmSccm = snapshot.GasLeakAlarm
            };
            
            // AlarmManager에 임계값 동기화 (struct는 값으로 복사)
            var thresholdsCopy = AlarmThresholds;
            alarmManager?.SetThresholds(thresholdsCopy);
        }

        public struct EnvAlarmThresholds
        {
            public double TempWarnDiffC;
            public double TempAlarmDiffC;
            public double PressWarnRatio;
            public double PressAlarmRatio;
            public double PressWarnAbsTorr;
            public double PressAlarmAbsTorr;
            public double RfWarnRatio;
            public double RfAlarmRatio;
            public double GasWarnAbsSccm;
            public double GasAlarmAbsSccm;
            public double GasLeakWarnSccm;
            public double GasLeakAlarmSccm;

            public static EnvAlarmThresholds CreateDefault()
            {
                // AppSettings에서 기본값 읽어옴
                return new EnvAlarmThresholds
                {
                    TempWarnDiffC = AppSettings.AlarmTempWarn,
                    TempAlarmDiffC = AppSettings.AlarmTempAlarm,
                    PressWarnRatio = AppSettings.AlarmPressWarnRatio,
                    PressAlarmRatio = AppSettings.AlarmPressAlarmRatio,
                    PressWarnAbsTorr = AppSettings.AlarmPressWarnAbs,
                    PressAlarmAbsTorr = AppSettings.AlarmPressAlarmAbs,
                    RfWarnRatio = AppSettings.AlarmRfWarnRatio,
                    RfAlarmRatio = AppSettings.AlarmRfAlarmRatio,
                    GasWarnAbsSccm = AppSettings.AlarmGasWarn,
                    GasAlarmAbsSccm = AppSettings.AlarmGasAlarm,
                    GasLeakWarnSccm = AppSettings.AlarmGasLeakWarn,
                    GasLeakAlarmSccm = AppSettings.AlarmGasLeakAlarm
                };
            }
        }
        internal readonly Random envRandom = new Random();
        public class PmDetailData
        {
            public string UnitKey;
            public string StatusText;
            public string RecipeName;
            public string StepName;
            public int RecipeTimeCurrent;
            public int RecipeTimeTotal;
            public int StepTimeCurrent;
            public int StepTimeTotal;
            public int StepIndex;
            public int StepCount;
            public string StepMessage;
            public int Progress;
            public int StepProgress;
            public string ActiveWaferText;
        }

        // Wafer, SimulationStepDefinition, ChamberState 클래스는 별도 파일로 분리됨
        // - Wafer.cs
        // - ChamberController.cs (SimulationStepDefinition, ChamberState 포함)
        
        /// <summary>
        /// Form1 생성자
        /// ViewModel, HardwareManager, 이벤트 핸들러, UI 업데이트 헬퍼를 초기화합니다.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            
            // ViewModel 초기화
            ViewModel = new MainFormViewModel();
            
            // EtherCAT 인스턴스 생성 (HardwareManager 초기화 전에 필요)
            EtherCAT_M = new IEG3268();
            
            // HardwareManager 초기화
            HardwareManager = new HardwareManager(EtherCAT_M, AddLogMessage);
            
            // 이벤트 핸들러 인스턴스 초기화
            loginEventHandlers = new LoginEventHandlers(this);
            processEventHandlers = new ProcessEventHandlers(this);
            waferEventHandlers = new WaferEventHandlers(this);
            navigationEventHandlers = new NavigationEventHandlers(this);
            equipmentEventHandlers = new EquipmentEventHandlers(this);
            hardwareEventHandlers = new HardwareEventHandlers(this);
            timerEventHandlers = new TimerEventHandlers(this);
            paintEventHandlers = new PaintEventHandlers(this);
            formEventHandlers = new FormEventHandlers(this);
            
            // UI 업데이트 헬퍼 초기화
            uiUpdater = new Helpers.Form1UiUpdater(this);
            
            // UI 초기화 헬퍼 초기화
            uiInitializer = new Helpers.Form1Initializer(this);
            
            // UI 설정 헬퍼 초기화
            uiConfigurator = new Helpers.Form1Configurator(this);
            
            // TM 프로세스 헬퍼 초기화
            tmProcessor = new Helpers.Form1TmProcessor(this);
            
            // FOUP 관리 헬퍼 초기화
            foupManager = new Helpers.FoupManager(this);
            
            // 알람 관리 헬퍼 초기화
            alarmManager = new Helpers.AlarmManager(this);
            alarmManager.SetThresholds(AlarmThresholds);
            
            // 웨이퍼 트래킹 서비스 초기화
            waferTrackingService = new Helpers.WaferTrackingService(this);
            
            if (IsDesignEnvironment())
            {
                return;
            }
            // 사용자 정의 컨트롤 초기화
            uiInitializer.InitializeCustomControls();
            uiInitializer.InitializeFoupMountButtons();
            uiInitializer.InitializeWaferOverlays();
            uiConfigurator.LayoutCentralEquipment();
            
            // panelEquipmentCanvas Resize 이벤트를 FormEventHandlers로 연결
            if (panelEquipmentCanvas != null)
            {
                panelEquipmentCanvas.Resize -= formEventHandlers.PanelEquipmentCanvas_Resize;
                panelEquipmentCanvas.Resize += formEventHandlers.PanelEquipmentCanvas_Resize;
            }
            
            chamberWaferPanels = new[] { panelWaferChamberA, panelWaferChamberB, panelWaferChamberC };
            foreach (var waferPanel in chamberWaferPanels)
            {
                if (waferPanel == null) continue;
                waferPanel.BackColor = Color.Transparent;
                waferPanel.Paint += paintEventHandlers.WaferPanel_Paint;
                UpdateWaferPanelRegion(waferPanel);
                waferPanel.Resize += (s, eArgs) => UpdateWaferPanelRegion((Panel)s);
            }
            HeaderClockTimer = new Timer();
            HeaderClockTimer.Interval = 1000;
            HeaderClockTimer.Tick += timerEventHandlers.HeaderClockTimer_Tick;
            HeaderClockTimer.Start();
            if (components != null)
            {
                components.Add(HeaderClockTimer);
            }
            // SimulationController는 InitializeSimulationState에서 초기화됨
            // SimulationTimer는 SimulationController에서 관리되므로 components에 추가할 필요 없음
            
            // UI 전용 고속 타이머 초기화 (50ms 주기로 하드웨어 상태 확인 및 UI 업데이트)
            HardwareUiUpdateTimer = new Timer();
            HardwareUiUpdateTimer.Interval = 50; // 50ms 주기
            HardwareUiUpdateTimer.Tick += timerEventHandlers.HardwareUiUpdateTimer_Tick;
            if (components != null)
            {
                components.Add(HardwareUiUpdateTimer);
            }
            // 타이머는 EtherCAT 연결 시 시작됨
            
            uiInitializer.InitializeSimulationState();
            // 초기화 시 모드에 따라 적절한 업데이트 메서드 호출
            if (IsTmHardwareModeAvailable())
            {
                UpdateTmVisualizationFromHardware();
            }
            else
            {
                UpdateTmVisualization();
            }
            UpdateHeaderClock();
            uiConfigurator.ConfigureStatusPanels();
            uiConfigurator.CaptureFoupBaseVisuals();
            uiConfigurator.ResetDoorStates();
            PrepareDemoEnvironment();
            // 5개의 개별 트랙 패널 생성
            uiInitializer.InitializeFoupTrackPanels();
            // fillPanel은 더 이상 사용하지 않으므로 숨김
            if (panelFoupALevelFill != null) panelFoupALevelFill.Visible = false;
            if (panelFoupBLevelFill != null) panelFoupBLevelFill.Visible = false;
            // 기존 단일 트랙 패널 숨김
            if (panelFoupALevelTrack != null) panelFoupALevelTrack.Visible = false;
            if (panelFoupBLevelTrack != null) panelFoupBLevelTrack.Visible = false;
            labelFoupSummaryInfo.Text = $"{CurrentFoupExchangeState} | {CurrentFoupQueueInfo}";
            // 장비 카드 상세 보기 이벤트는 추후 재적용 예정
            SetProcessState(ProcessState.Idle, "시스템 초기화 완료.");
            UpdateSimulationUi();
            SetHeaderAlarmIdle();
            uiConfigurator.LayoutCentralEquipment();
            
            // GroupBox 테두리를 검은색으로 그리기 위한 Paint 이벤트 연결
            if (groupBoxControlButtons != null)
            {
                groupBoxControlButtons.Paint += paintEventHandlers.GroupBox_Paint;
            }
            if (groupBoxFoupReady != null)
            {
                groupBoxFoupReady.Paint += paintEventHandlers.GroupBox_Paint;
            }
            if (groupBoxRecipe != null)
            {
                groupBoxRecipe.Paint += paintEventHandlers.GroupBox_Paint;
            }
            
            // 탭 버튼 클릭 이벤트 연결
            if (buttonTabMain != null)
            {
                buttonTabMain.Click += navigationEventHandlers.ButtonTabMain_Click;
            }
            if (buttonTabVerification != null)
            {
                buttonTabVerification.Click += navigationEventHandlers.ButtonTabVerification_Click;
            }
            if (buttonTabTransfer != null)
            {
                buttonTabTransfer.Click += navigationEventHandlers.ButtonTabTransfer_Click;
            }
            
            // 초기 선택 상태 설정
            UpdateTabButtonStates("Main");
            UpdateNavButtonStates("Operate");
            
            this.Shown += Form1_Shown;
            this.FormClosing += formEventHandlers.Form1_FormClosing;

            WireButtonHandlers();
            
            // 버튼 핸들러 연결 후 로그인 상태 적용
            ApplyLoginState();
            
            // 환경 정보 표 초기화 (모든 PM에 대해)
            uiInitializer.InitializePmEnvironmentTables();
            
            // PM 상태 패널 초기화 (깜빡임 방지용 커스텀 컨트롤)
            uiInitializer.InitializePmStatusPanels();

            // 초기 레시피 목록 로드
            try
            {
                ReloadRecipeCombo();
            }
            catch (Exception ex)
            {
                // 레시피 로드 오류 처리 (ExceptionHandler 사용)
                string errorMessage = ExceptionHandler.HandleException(ex, "레시피 로드", AddLogMessage);
                
                // 사용자에게 경고 메시지 표시 (폼이 완전히 로드된 후)
                this.Shown += (s, e) =>
                {
                    MessageBox.Show(
                        $"레시피 목록을 로드하는 중 오류가 발생했습니다.\n\n" +
                        $"오류 내용: {errorMessage}\n\n" +
                        $"시뮬레이션은 계속 진행되지만, 레시피 기능이 제한될 수 있습니다.\n" +
                        $"레시피 관리 메뉴에서 레시피를 다시 로드해 주세요.",
                        "레시피 로드 오류",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                };
            }

            // 알람 임계값 설정 로드
            try
            {
                LoadAlarmThresholds();
            }
            catch (Exception ex)
            {
                // 알람 임계값 로드 오류 처리 (ExceptionHandler 사용)
                string errorMessage = ExceptionHandler.HandleException(ex, "알람 임계값 로드", AddLogMessage);
                
                // 사용자에게 경고 메시지 표시 (폼이 완전히 로드된 후)
                this.Shown += (s, e) =>
                {
                    MessageBox.Show(
                        $"알람 임계값 설정을 로드하는 중 오류가 발생했습니다.\n\n" +
                        $"오류 내용: {ex.Message}\n\n" +
                        $"기본 알람 임계값이 사용됩니다.\n" +
                        $"설정 메뉴에서 알람 임계값을 다시 설정해 주세요.",
                        "알람 임계값 로드 오류",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                };
            }
        }

        private void PrepareDemoEnvironment()
        {
            if (!AppSettings.DemoModeEnabled || demoSetupApplied)
            {
                return;
            }

            IsLoggedIn = true;
            CurrentUser = "Demo";
            CurrentRole = "DEMO";
            IsFoupMounted = true;
            IsFoupAMounted = true;
            IsFoupBMounted = true;
            SetWaferLoadState(MainFormViewModel.WaferLoadStateType.Loading, logChange: false, refreshUi: false);
            demoSetupApplied = true;
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            // 폼이 완전히 표시된 후 네비게이션 버튼 상태 업데이트
            UpdateNavigationButtons();
            
            // DLL 메서드 목록 확인 (한 번만 실행)
            CheckDllMethodsOnce();
            
            if (!AppSettings.DemoModeEnabled || !AppSettings.DemoModeAutoStart || demoSimulationStarted)
            {
                return;
            }

            demoSimulationStarted = true;
            await Task.Delay(600);
            if (!SimulationRunning)
            {
                StartSimulation();
            }
        }

        private static bool dllMethodsChecked = false;
        private void CheckDllMethodsOnce()
        {
            if (dllMethodsChecked) return;
            dllMethodsChecked = true;

            try
            {
                string methods = DllMethodInspector.ListAllMethods();
                
                // 프로젝트 루트에도 저장
                string projectRoot = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "..", "..", "..");
                projectRoot = System.IO.Path.GetFullPath(projectRoot);
                string projectOutputPath = System.IO.Path.Combine(projectRoot, "IEG3268_Methods.txt");
                
                // 실행 파일 위치에도 저장
                string exeOutputPath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "IEG3268_Methods.txt");
                
                System.IO.File.WriteAllText(projectOutputPath, methods, System.Text.Encoding.UTF8);
                System.IO.File.WriteAllText(exeOutputPath, methods, System.Text.Encoding.UTF8);
                
                AddLogMessage($"DLL 메서드 목록이 저장되었습니다:", "INFO");
                AddLogMessage($"  - {projectOutputPath}", "INFO");
                AddLogMessage($"  - {exeOutputPath}", "INFO");
                
                // 콘솔에도 출력 (디버그용)
                System.Diagnostics.Debug.WriteLine("=== IEG3268 DLL 메서드 목록 ===");
                System.Diagnostics.Debug.WriteLine(methods);
            }
            catch (Exception ex)
            {
                // DLL 메서드 목록 확인 오류 처리 (ExceptionHandler 사용)
                ExceptionHandler.HandleException(ex, "DLL 메서드 목록 확인", AddLogMessage);
            }
        }

        // buttonLogin_Click 메서드는 LoginEventHandlers.ButtonLogin_Click으로 이동됨
        // 중복 실행 방지를 위해 제거됨

        private void buttonUserManagement_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn())
            {
                return;
            }

            if (CurrentRole != "관리자")
            {
                MessageBox.Show("사용자 관리는 관리자만 접근할 수 있습니다.", "권한 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var userManagementForm = new UserManagementForm())
                {
                    userManagementForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"사용자 관리 폼을 여는 중 오류가 발생했습니다.\r\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddLogMessage($"사용자 관리 폼 오류: {ex.Message}", "ERROR");
            }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            if (!IsLoggedIn)
            {
                return;
            }

            IsLoggedIn = false;
            CurrentUser = "Guest";
            CurrentRole = "없음";
            SetProcessState(ProcessState.Idle, "로그아웃으로 인해 공정을 대기 상태로 전환했습니다.");
            ApplyLoginState();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn())
            {
                return;
            }

            if (SimulationRunning && !SimulationPaused)
            {
                MessageBox.Show("이미 공정이 진행 중입니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!SimulationRunning)
            {
                if (!IsFoupAMounted || !IsFoupBMounted)
                {
                    MessageBox.Show("FOUP A/B 장착 상태를 확인해 주세요.", "준비 미완료", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (WaferLoadState != MainFormViewModel.WaferLoadStateType.Loading)
                {
                    MessageBox.Show("웨이퍼를 '로딩' 상태로 전환한 뒤 공정을 시작할 수 있습니다.", "준비 미완료", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // EtherCAT 연결 확인 (경고만 표시, 시뮬레이션은 계속 진행)
                if (!EthercatConnected)
                {
                    var result = MessageBox.Show(
                        "EtherCAT가 연결되지 않았습니다.\n시뮬레이션 모드로 동작합니다.\n계속하시겠습니까?",
                        "EtherCAT 미연결",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            HasAlarm = false;
            if (SimulationRunning && SimulationPaused)
            {
                ResumeSimulation();
            }
            else
            {
                StartSimulation();
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn())
            {
                return;
            }

            if (!SimulationRunning || SimulationPaused)
            {
                MessageBox.Show("진행 중인 공정이 없습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 하드웨어 모드: 새 명령만 중지 (서보 ON 유지)
            // 서보를 OFF하면 원점복귀가 필요하므로, 서보는 ON 상태 유지
            // 진행 중인 동작은 완료되고, 새 명령만 보내지 않음
            if (IsTmHardwareModeAvailable())
            {
                AddLogMessage("하드웨어 일시정지: 새 명령 중지 (서보 ON 유지)", "WARN");
                // 서보, 진공, 실린더 상태 모두 유지
            }

            PauseSimulation();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn())
            {
                return;
            }

            if (!SimulationRunning)
            {
                MessageBox.Show("정지할 공정이 없습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 하드웨어 모드: 긴급 정지
            if (EthercatConnected && EtherCAT_M != null)
            {
                try
                {
                    // 1. 서보 즉시 OFF
                    EtherCAT_M.Axis1_OFF();
                    EtherCAT_M.Axis2_OFF();
                    IsServoOn = false;
                    
                    // 2. 진공 OFF (웨이퍼가 떨어질 수 있으므로 주의 메시지)
                    EtherCAT_M.Digital_Output(14, false); // 진공 OFF
                    EtherCAT_M.Digital_Output(15, false); // 배기 OFF
                    
                    // 3. 실린더 상태 유지 (급격한 동작 방지)
                    // 실린더는 현재 상태 유지
                    
                    AddLogMessage("긴급정지: 서보 OFF, 진공 OFF - 장비 상태를 확인하세요!", "ALARM");
                    
                    // 상태 초기화
                    TmHardwareInitialized = false;
                    TmHardwareActionPending = false;
                    TmSettleWaiting = false;
                }
                catch (Exception ex)
                {
                    AddLogMessage($"긴급정지 하드웨어 제어 오류: {ex.Message}", "ERROR");
                }
            }

            HasAlarm = true;
            AbortSimulation("긴급 정지를 실행했습니다. 장비 상태를 확인하세요.", true);
            
            // 긴급정지 후 안내 메시지
            MessageBox.Show(
                "긴급 정지가 실행되었습니다.\n\n" +
                "⚠️ 장비 상태를 확인하세요:\n" +
                "• 서보: OFF 상태\n" +
                "• 진공: OFF 상태\n" +
                "• 실린더: 현재 상태 유지\n\n" +
                "재시작하려면:\n" +
                "1. 장비 상태 확인\n" +
                "2. 실린더 후진 확인\n" +
                "3. 서보 ON → 원점복귀\n" +
                "4. 공정 리셋 후 재시작",
                "긴급 정지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void buttonResetAlarm_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn())
            {
                return;
            }

            // 헤더에 알람이 표시되어 있는지 확인
            if (!HasHeaderAlarm())
            {
                MessageBox.Show("해제할 알람이 없습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 모든 알람 상태 리셋 (AlarmManager로 위임)
            alarmManager?.ResetAllAlarms();

            // 공정 중이 아닐 때만 시뮬레이션 상태 초기화
            if (!SimulationRunning)
            {
                // 시뮬레이션 상태 초기화 (에러 상태 해제)
                if (CurrentProcessState == ProcessState.Error)
                {
            uiInitializer.InitializeSimulationState();
                }

                // 프로세스 상태를 Idle로 변경
            SetProcessState(ProcessState.Idle, "알람을 리셋했습니다.");
            }
            else
            {
                // 공정 중일 때는 헤더 알람 메시지만 초기화하고 공정은 계속 진행
                // 프로세스 상태는 변경하지 않음 (공정에 영향 없음)
            }
            
            // 로그에 알람 리셋 메시지 추가
            AddLogMessage("알람이 리셋되었습니다.", "INFO");
            
            // 헤더 최근 알람 영역 즉시 초기화 (verification 알람 포함)
            SetHeaderAlarmIdle();
            
            // UI 업데이트
            UpdateSimulationUi();
        }

        private void buttonResetProcess_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn())
            {
                return;
            }

            string confirmMessage = "현재 공정을 완전히 초기화하시겠습니까?\nTM 대기 상태, FOUP 표시, 챔버 상태 등이 리셋됩니다.";
            
            // 하드웨어 모드 추가 안내
            if (EthercatConnected)
            {
                confirmMessage += "\n\n[하드웨어 모드]\n• 실린더 후진 확인\n• 진공 OFF\n• 원점복귀 수행";
            }

            var confirm = MessageBox.Show(
                confirmMessage,
                "공정 리셋 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            SimulationController?.StopTimer();
            
            // 진행 중인 작업 취소 및 하드웨어 동작 중지
            if (TransferService != null && TransferService.CurrentTransfer != null)
            {
                AddLogMessage($"공정 리셋: 진행 중인 작업 취소 - {EquipmentRegionHelper.FormatRegionLabel(TransferService.CurrentTransfer.Pickup)} → {EquipmentRegionHelper.FormatRegionLabel(TransferService.CurrentTransfer.Dropoff)}", "WARN");
            }
            
            // 하드웨어 동작 플래그 리셋 (진행 중인 작업 중지)
            TmHardwareActionPending = false;
            TmSettleWaiting = false;
            
            // Transfer 큐 및 현재 작업 클리어
            TransferService?.ResetToIdle();
            TransferService?.ClearQueue();
            
            // 하드웨어 모드: 안전한 초기화 수행
            if (EthercatConnected && EtherCAT_M != null)
            {
                try
                {
                    AddLogMessage("공정 리셋: 하드웨어 초기화 시작", "INFO");
                    
                    // 1. 진공 OFF (웨이퍼 분리)
                    EtherCAT_M.Digital_Output(14, false); // 진공 OFF
                    EtherCAT_M.Digital_Output(15, false); // 배기 OFF
                    AddLogMessage("공정 리셋: 진공 OFF", "INFO");
                    
                    // 2. 실린더 상태 확인
                    bool cylinderRetracted = false;
                    try
                    {
                        cylinderRetracted = EtherCAT_M.Digital_Input(12); // 후진 센서
                    }
                    catch (Exception ex)
                    {
                        AddLogMessage($"실린더 상태 확인 오류: {ex.Message}", "WARN");
                    }
                    
                    // 3. 실린더 전진 상태면 후진 시도
                    if (!cylinderRetracted)
                    {
                        AddLogMessage("공정 리셋: 실린더 후진 시도", "INFO");
                        EtherCAT_M.Digital_Output(12, false); // 전진 OFF
                        EtherCAT_M.Digital_Output(13, true);  // 후진 ON
                        
                        // 후진 완료 대기 (최대 5초)
                        var timeout = DateTime.Now.AddSeconds(5);
                        while (DateTime.Now < timeout)
                        {
                            try
                            {
                                if (EtherCAT_M.Digital_Input(12))
                                {
                                    cylinderRetracted = true;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"실린더 후진 확인 오류: {ex.Message}");
                            }
                            System.Threading.Thread.Sleep(100);
                            // Application.DoEvents() 제거 - UI 스레드 블로킹 방지를 위해 제거
                        }
                        
                        if (!cylinderRetracted)
                        {
                            AddLogMessage("공정 리셋: 실린더 후진 타임아웃 - 수동 확인 필요", "WARN");
                            MessageBox.Show(
                                "실린더 후진이 완료되지 않았습니다.\n수동으로 실린더 상태를 확인해주세요.",
                                "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            AddLogMessage("공정 리셋: 실린더 후진 완료", "INFO");
                        }
                    }
                    
                    // 4. 서보 ON 상태 확인 및 원점복귀
                    if (IsServoOn && cylinderRetracted)
                    {
                        AddLogMessage("공정 리셋: 원점복귀 시작 - 1단계: 상하(Axis1)", "INFO");
                        
                        // 1단계: Axis1 (상하) 원점복귀
                        EtherCAT_M.Axis1_UD_Homming();
                        
                        // Axis1 원점복귀 완료 대기 (최대 120초)
                        var timeout = DateTime.Now.AddSeconds(120);
                        while (DateTime.Now < timeout)
                        {
                            if (EtherCAT_M.Axis1_Status("HOME_D"))
                            {
                                break;
                            }
                            System.Threading.Thread.Sleep(100);
                        }
                        
                        if (!EtherCAT_M.Axis1_Status("HOME_D"))
                        {
                            AddLogMessage("공정 리셋: 상하(Axis1) 원점복귀 타임아웃", "ERROR");
                            TmHardwareInitialized = false;
                        }
                        else
                        {
                            AddLogMessage("공정 리셋: 상하(Axis1) 원점복귀 완료 - 2단계: 좌우(Axis2)", "INFO");
                            
                            // 2단계: Axis2 (좌우) 원점복귀
                            EtherCAT_M.Axis2_LR_Homming();
                            
                            // Axis2 원점복귀 완료 대기 (최대 120초)
                            timeout = DateTime.Now.AddSeconds(120);
                            while (DateTime.Now < timeout)
                            {
                                if (EtherCAT_M.Axis2_Status("HOME_D"))
                                {
                                    break;
                                }
                                System.Threading.Thread.Sleep(100);
                            }
                            
                            if (!EtherCAT_M.Axis2_Status("HOME_D"))
                            {
                                AddLogMessage("공정 리셋: 좌우(Axis2) 원점복귀 타임아웃", "ERROR");
                                TmHardwareInitialized = false;
                            }
                            else
                            {
                                AddLogMessage("공정 리셋: 원점복귀 완료 (상하 → 좌우 순서)", "INFO");
                                TmHardwareInitialized = true;
                                UpdateServoStatusLabel(); // UI 업데이트 (Home 상태 표시)
                            }
                        }
                    }
                    else if (!IsServoOn)
                    {
                        AddLogMessage("공정 리셋: 서보 OFF 상태 - 서보 ON 후 원점복귀 필요", "WARN");
                        TmHardwareInitialized = false;
                    }
                    
                    // 상태 초기화
                    TmHardwareActionPending = false;
                    TmSettleWaiting = false;
                    
                    // 하드웨어 모드: 실제 TM 위치를 읽어서 UI에 반영
                    if (TmHardwareController != null)
                    {
                        try
                        {
                            TmHardwareController.UpdateCurrentPositions();
                            long currentX = TmHardwareController.CurrentAxis2Position;
                            long currentY = TmHardwareController.CurrentAxis1Position;
                            
                            // 하드웨어 위치를 Region으로 변환
                            var hardwareRegion = EquipmentRegionHelper.DetermineRegionFromPosition(currentX, currentY, TmHardwareController.Positions);
                            
                            // TM 위치 업데이트
                            TmVisualTarget = hardwareRegion;
                            TmCurrentPosition = hardwareRegion;
                            
                            // TM 시각화 업데이트 (하드웨어 모드이므로 하드웨어 업데이트 메서드 사용)
                            UpdateTmVisualizationFromHardware();
                            
                            AddLogMessage($"공정 리셋: TM 위치 업데이트 완료 - {EquipmentRegionHelper.FormatRegionLabel(hardwareRegion)}", "INFO");
                        }
                        catch (Exception ex)
                        {
                            AddLogMessage($"공정 리셋: TM 위치 읽기 오류: {ex.Message}", "WARN");
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddLogMessage($"공정 리셋 하드웨어 오류: {ex.Message}", "ERROR");
                }
            }
            
            uiInitializer.InitializeSimulationState();
            SetProcessState(ProcessState.Idle, "공정을 초기화했습니다.");
            UpdateSimulationUi();
            UpdateTmAnimationIdleTarget();
            UpdateServoStatusLabel();
            AddLogMessage("사용자가 공정을 수동 리셋했습니다.", "INFO");
        }

        private void buttonApplyRecipe_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn())
            {
                return;
            }

            var recipeName = comboRecipeSelect.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                MessageBox.Show(this, "적용할 레시피를 선택하세요.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LastSelectedRecipe = recipeName;
            AddLogMessage($"레시피 '{recipeName}' 적용을 준비했습니다.", "INFO");
            var list = RecipeRepository.LoadAll();
            var snap = list.FirstOrDefault(r => string.Equals(r.Name, recipeName, StringComparison.Ordinal));
            if (snap == null)
            {
                MessageBox.Show(this, "레시피를 찾을 수 없습니다. 레시피 관리에서 다시 저장해 주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ApplyRecipeFromSnapshot(snap);
            RecipeApplied = true; // 레시피 적용 플래그 설정
            // 레시피 적용 후 SV 값만 업데이트 (PV는 공정 시작 시에만 생성되므로 여기서는 SV만 표시)
            foreach (var kvp in PmEnvTables)
            {
                if (kvp.Value != null && kvp.Value.Visible)
                {
                    UpdatePmEnvironmentTable(kvp.Key);
                }
            }
        }

        internal void ApplyRecipeFromSnapshot(RecipeSnapshot r)
        {
            SetRecipeParameters(
                waferCount: Math.Max(1, r.WaferCount),
                chamberADuration: r.DurA,
                chamberBDuration: r.DurB,
                chamberCDuration: r.DurC,
                enableSecondExposure: r.SecondExposure);

            // 환경 스펙을 레시피 값으로 구성 (간단한 표기)
            string pmaTempStr = $"{r.PMA?.T:0.0}°C";
            string pmaPressStr = $"{(r.PMA?.P ?? 0):0.###} Torr";
            string pmaRhStr = $"{(r.PMA?.H ?? 0):0.#}% RH";
            string pmaNotes = "Recipe 기반";

            string pmbTempStr = $"{r.PMB?.T:0.0}°C";
            string pmbPressStr = $"{(r.PMB?.P ?? 0):0.###} Torr";
            string pmbRhStr = $"{(r.PMB?.H ?? 0):0.#}% RH";
            string pmbNotes = "Recipe 기반";

            string pmcTempStr = $"{r.PMC?.T:0.0}°C";
            string pmcPressStr = $"{(r.PMC?.P ?? 0):0.###} Torr";
            string pmcRhStr = $"{(r.PMC?.H ?? 0):0.#}% RH";
            string pmcNotes = "Recipe 기반";

            ChamberEnvSpecs["PMA"] = new ChamberController.ChamberEnvironmentSpec(pmaTempStr, pmaPressStr, pmaRhStr, pmaNotes, r.PMA?.T ?? 23.0, r.PMA?.P ?? 760.0, r.PMA?.H ?? 45.0);
            ChamberEnvSpecs["PMB"] = new ChamberController.ChamberEnvironmentSpec(pmbTempStr, pmbPressStr, pmbRhStr, pmbNotes, r.PMB?.T ?? 22.5, r.PMB?.P ?? 0.005, r.PMB?.H ?? 5.0);
            ChamberEnvSpecs["PMC"] = new ChamberController.ChamberEnvironmentSpec(pmcTempStr, pmcPressStr, pmcRhStr, pmcNotes, r.PMC?.T ?? 110.0, r.PMC?.P ?? 50.0, r.PMC?.H ?? 3.0);
            // 레시피 적용 시에는 PV 값을 변경하지 않음 (공정 시작 시에만 초기화됨)
            // InitializeEnvironmentTelemetry()는 공정 시작 시에만 호출됨
            // ChamberEnvLive는 공정 시작 전까지 이전 값을 유지

            // Gas/RF SV 반영
            UnitGasRfSv["PMA"] = (r.GasRfPMA?.NF3 ?? 0, r.GasRfPMA?.O2 ?? 0, r.GasRfPMA?.CF4 ?? 0, r.GasRfPMA?.RF ?? 0);
            UnitGasRfSv["PMB"] = (r.GasRfPMB?.NF3 ?? 0, r.GasRfPMB?.O2 ?? 0, r.GasRfPMB?.CF4 ?? 0, r.GasRfPMB?.RF ?? 0);
            UnitGasRfSv["PMC"] = (r.GasRfPMC?.NF3 ?? 0, r.GasRfPMC?.O2 ?? 0, r.GasRfPMC?.CF4 ?? 0, r.GasRfPMC?.RF ?? 0);

            AddLogMessage($"레시피 적용 완료: 웨이퍼 {CurrentRecipeWaferCount}장, A:{r.DurA}s, B:{r.DurB}s, C:{r.DurC}s, 2차:{(r.SecondExposure ? "사용" : "미사용")}", "INFO");
            UpdateSimulationUi();
        }

        private void buttonToggleFoupMount_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn())
            {
                return;
            }

            // 기존 단일 토글은 숨김 처리 예정. 눌렸다면 A/B를 함께 토글하는 보조 동작만 수행.
            var newState = !(IsFoupAMounted && IsFoupBMounted);
            IsFoupAMounted = newState;
            IsFoupBMounted = newState;
            IsFoupMounted = IsFoupAMounted && IsFoupBMounted;
            AddLogMessage($"FOUP 장착 상태 일괄 변경: A={BoolToMountText(IsFoupAMounted)}, B={BoolToMountText(IsFoupBMounted)}", "INFO");
            UpdateFoupPreparationButtons();
            UpdateSimulationUi();
        }

        private void buttonWaferLoading_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn())
            {
                return;
            }
            if (!IsFoupAMounted)
            {
                MessageBox.Show("FOUP A가 미장착 상태입니다. 먼저 FOUP A를 장착해 주세요.", "준비 미완료", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var count = foupManager?.PromptForWaferCount(UserWaferLoadCount, 1, AppSettings.MaxFoupCapacity);
            if (count == null)
            {
                return;
            }
            UserWaferLoadCount = count.Value;

            SetWaferLoadState(MainFormViewModel.WaferLoadStateType.Loading);
            AddLogMessage($"웨이퍼 로딩: {UserWaferLoadCount}장 설정", "INFO");
            
            // 웨이퍼 로딩 시 즉시 UI에 반영
            foupManager?.LoadWafersToFoupA(UserWaferLoadCount, SecondExposureEnabled);
            
            // UI 업데이트
            UpdateSimulationUi();
        }

        private void buttonWaferUnloading_Click(object sender, EventArgs e)
        {
            if (!EnsureLoggedIn())
            {
                return;
            }
            if (!IsFoupBMounted)
            {
                MessageBox.Show("FOUP B가 미장착 상태입니다. 먼저 FOUP B를 장착해 주세요.", "준비 미완료", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // FOUP B 언로딩: 완료 웨이퍼 배출 처리
            var unloadedCount = foupManager?.UnloadFoupB() ?? 0;
            if (unloadedCount > 0)
            {
                AddLogMessage($"FOUP B 언로딩: {unloadedCount}장 배출 처리", "INFO");
            }
            SetWaferLoadState(MainFormViewModel.WaferLoadStateType.Unloading);
        }

        // 웨이퍼 로딩 수 입력 다이얼로그 (FoupManager로 위임)
        internal int? PromptForWaferCount(int current, int min, int max)
        {
            return foupManager?.PromptForWaferCount(current, min, max);
        }

        internal void SetRecipeParameters(int waferCount, int chamberADuration, int chamberBDuration, int chamberCDuration, bool enableSecondExposure = false)
        {
            // 레시피 검증 로직은 RecipeValidator로 이동됨
            var validationResult = RecipeValidator.ValidateRecipeParameters(waferCount, chamberADuration, chamberBDuration, chamberCDuration);
            
            CurrentRecipeWaferCount = validationResult.WaferCount;
            int durA = validationResult.ChamberADuration;
            int durB = validationResult.ChamberBDuration;
            int durC = validationResult.ChamberCDuration;
            
            // 공정 단계 시간 업데이트를 ChamberController로 위임
            chamberController?.UpdateStepDurations(durA, durB, durC);
            SecondExposureEnabled = enableSecondExposure;
            
            // 이미 FOUP A에 로딩된 웨이퍼들의 2차 노광 설정 업데이트
            if (foupManager?.HasWafersInFoupA() == true)
            {
                var queue = foupManager.FoupAQueue;
                foreach (var wafer in queue)
                {
                    wafer.RequiresSecondExposure = enableSecondExposure;
                }
                AddLogMessage($"FOUP A 웨이퍼 {queue.Count}장의 2차 노광 설정을 '{(enableSecondExposure ? "사용" : "미사용")}'으로 업데이트", "INFO");
            }
            
            // 0초로 입력된 경우 경고 로그
            if (validationResult.HasWarning)
            {
                AddLogMessage($"공정 시간이 {RecipeValidator.MinChamberDuration}초 미만으로 설정되어 최소값으로 조정되었습니다.", "WARNING");
            }
            
            AddLogMessage($"레시피 설정: 웨이퍼 {CurrentRecipeWaferCount}장, A:{durA}s, B:{durB}s, C:{durC}s, 2차 노광: {(SecondExposureEnabled ? "사용" : "미사용")}", "INFO");
        }

        private void ApplyRecipeEnvironmentSpecs(string recipeKey)
        {
            double pmaTemp = 23.0; string pmaTempStr = "23.0 ±0.3°C (Spin chuck)";
            double pmaPress = 760.0; string pmaPressStr = "760 Torr (대기압)";
            double pmaRh = 45.0; string pmaRhStr = "45 ±5% RH";
            string pmaNotes = "PR 도포 · Solvent purge 2.0 m/s";
            UnitGasRfSv["PMA"] = (200, 200, 200, 1000);

            double pmbTemp = 22.5; string pmbTempStr = "22.5 ±0.2°C (Stage)";
            double pmbPress = 0.005; string pmbPressStr = "5×10⁻³ Torr (노광 챔버)";
            double pmbRh = 5.0; string pmbRhStr = "<5% RH (건조 N₂)";
            string pmbNotes = "ArF 193nm Dose 120 mJ/cm²";
            UnitGasRfSv["PMB"] = (0, 0, 0, 0);

            double pmcTemp = 110.0; string pmcTempStr = "110°C PEB Plate";
            double pmcPress = 50.0; string pmcPressStr = "50 Torr (N₂ 퍼지)";
            double pmcRh = 3.0; string pmcRhStr = "<3% RH";
            string pmcNotes = "후베이크 · N₂ flow 20 slm";
            UnitGasRfSv["PMC"] = (0, 0, 0, 0);

            switch (recipeKey)
            {
                case "PR_HIGH_PIPE":
                    pmbNotes = "ArF 193nm Dose 160 mJ/cm²";
                    pmcTemp = 115.0; pmcTempStr = "115°C PEB Plate";
                    break;
                case "PR_SINGLE_PIPE":
                    pmbNotes = "ArF 193nm Dose 100 mJ/cm²";
                    pmcTemp = 105.0; pmcTempStr = "105°C PEB Plate";
                    break;
                case "PR_DOUBLE_EXPO":
                    pmbNotes = "Double Exposure 1차 · Dose 90 mJ/cm²";
                    pmcNotes = "2차 노광/후베이크 · N₂ flow 25 slm";
                    pmcTemp = 110.0; pmcTempStr = "110°C PEB Plate";
                    break;
            }

            ChamberEnvSpecs["PMA"] = new ChamberController.ChamberEnvironmentSpec(pmaTempStr, pmaPressStr, pmaRhStr, pmaNotes, pmaTemp, pmaPress, pmaRh);
            ChamberEnvSpecs["PMB"] = new ChamberController.ChamberEnvironmentSpec(pmbTempStr, pmbPressStr, pmbRhStr, pmbNotes, pmbTemp, pmbPress, pmbRh);
            ChamberEnvSpecs["PMC"] = new ChamberController.ChamberEnvironmentSpec(pmcTempStr, pmcPressStr, pmcRhStr, pmcNotes, pmcTemp, pmcPress, pmcRh);

            InitializeEnvironmentTelemetry();
            AddLogMessage($"환경 설정 적용: {recipeKey} (PMA {pmaTempStr}, PMB {pmbPressStr}, PMC {pmcTempStr})", "INFO");
        }

        internal void SetWaferLoadState(MainFormViewModel.WaferLoadStateType newState, bool logChange = true, bool refreshUi = true)
        {
            if (WaferLoadState == newState)
            {
                // 같은 상태 재설정이어도 사용자가 수량을 바꿀 수 있으므로 재적용/리프레시 수행
                uiConfigurator.ApplyConfiguredFoupCountsFromState();
                if (refreshUi)
                {
                    UpdateFoupPreparationButtons();
                    UpdateSimulationUi();
                }
                return;
            }

            WaferLoadState = newState;
            uiConfigurator.ApplyConfiguredFoupCountsFromState();

            if (logChange)
            {
                string message;
                switch (newState)
                {
                    case MainFormViewModel.WaferLoadStateType.Loading:
                        message = "웨이퍼가 로딩 상태로 설정되었습니다.";
                        break;
                    case MainFormViewModel.WaferLoadStateType.Unloading:
                        message = "웨이퍼가 언로딩 상태로 설정되었습니다. (FOUP B 배출)";
                        break;
                    default:
                        message = "웨이퍼 상태가 초기화되었습니다.";
                        break;
                }

                AddLogMessage(message, "INFO");
            }

            UpdateFoupPreparationButtons();
            if (refreshUi)
            {
                UpdateSimulationUi();
            }
        }

        // FOUP B 언로딩: 완료 웨이퍼를 설비 밖으로 배출하여 카운트 초기화 (FoupManager로 위임)
        internal void UnloadFoupB()
        {
            var unloadedCount = foupManager?.UnloadFoupB() ?? 0;
            if (unloadedCount > 0)
            {
                AddLogMessage($"FOUP B 언로딩: {unloadedCount}장 배출 처리", "INFO");
            }
            UpdateSimulationUi();
        }

        /// <summary>
        /// 로그인 상태를 확인하고, 로그인되지 않은 경우 경고 메시지를 표시합니다.
        /// </summary>
        /// <returns>로그인되어 있으면 true, 아니면 false</returns>
        internal bool EnsureLoggedIn()
        {
            if (IsLoggedIn)
            {
                return true;
            }

            MessageBox.Show("이 기능을 사용하려면 로그인이 필요합니다.", "권한 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        /// <summary>
        /// 사용자 인증 정보를 검증합니다.
        /// </summary>
        /// <param name="username">사용자명</param>
        /// <param name="password">비밀번호</param>
        /// <returns>인증 성공 시 true, 실패 시 false</returns>
        internal bool IsCredentialValid(string username, string password)
        {
            // UserRepository를 사용한 인증
            return UserRepository.ValidateUser(username, password);
        }

        /// <summary>
        /// 로그인 상태에 따라 UI를 업데이트합니다.
        /// 로그인된 경우 사용자 정보를 표시하고, 로그아웃된 경우 기본 상태로 복원합니다.
        /// </summary>
        internal void ApplyLoginState()
        {
            labelLoginStatus.Text = $"사용자: {CurrentUser} / 권한: {CurrentRole}";
            buttonLogin.Enabled = !IsLoggedIn;
            buttonLogout.Enabled = IsLoggedIn;
            SetControlPanelEnabled(IsLoggedIn);
            UpdateProcessControlButtons();
            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            uiUpdater?.UpdateNavigationButtons();
        }

        private void ConfigureStatusPanels()
        {
            uiConfigurator?.ConfigureStatusPanels();
        }

        internal void ApplyConfiguredFoupCountsFromState()
        {
            uiConfigurator?.ApplyConfiguredFoupCountsFromState();
        }

        private void CaptureFoupBaseVisuals()
        {
            uiConfigurator?.CaptureFoupBaseVisuals();
        }

        private void CacheLabelBaseText(Label label)
        {
            if (label != null && label.Tag == null)
            {
                label.Tag = label.Text;
            }
        }

        private string GetLabelBaseText(Label label)
        {
            if (label == null)
            {
                return string.Empty;
            }

            if (label.Tag is string cached)
            {
                return cached;
            }

            label.Tag = label.Text;
            return label.Text;
        }

        private void CachePanelBaseColor(Panel panel)
        {
            if (panel != null && !originalPanelColors.ContainsKey(panel))
            {
                originalPanelColors[panel] = panel.BackColor;
            }
        }

        private Color GetOriginalPanelColor(Panel panel)
        {
            if (panel != null && originalPanelColors.TryGetValue(panel, out var color))
            {
                return color;
            }

            return panel?.BackColor ?? AppSettings.DefaultBackgroundColor;
        }

        private void MoveSummaryPanel(Panel panel)
        {
            if (panel.Parent != null)
            {
                panel.Parent.Controls.Remove(panel);
            }
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(12, 10, 12, 10);
        }

        private void RegisterEquipmentDetailHandlers()
        {
            // TM은 캔버스에 직접 그리므로 RegisterPanelForDetail 제거
            RegisterPanelForDetail(panelChamberA, "PMA");
            RegisterPanelForDetail(panelChamberB, "PMB");
            RegisterPanelForDetail(panelChamberC, "PMC");
            RegisterPanelForDetail(panelFoupA, "FOUP A");
            RegisterPanelForDetail(panelFoupB, "FOUP B");
            RegisterPanelForDetail(panelMainLamp, "Main Lamp");
            RegisterPanelForDetail(panelSummaryPMA, "PMA");
            RegisterPanelForDetail(panelSummaryPMB, "PMB");
            RegisterPanelForDetail(panelSummaryPMC, "PMC");
            RegisterPanelForDetail(panelFoupStatusA, "FOUP A");
            RegisterPanelForDetail(panelFoupStatusB, "FOUP B");
        }

        // panelEquipmentCanvas_Resize는 FormEventHandlers로 이동됨
        // Designer.cs 호환성을 위해 메서드 유지 (내부에서 formEventHandlers 호출)
        private void panelEquipmentCanvas_Resize(object sender, EventArgs e)
        {
            formEventHandlers.PanelEquipmentCanvas_Resize(sender, e);
        }

        internal void LayoutCentralEquipment()
        {
            uiConfigurator?.LayoutCentralEquipment();
        }

        // TM은 별도 컨트롤로 분리하므로 캔버스 Paint 이벤트 불필요

        /// <summary>
        /// EquipmentRegion에 따른 블레이드 각도 계산
        /// </summary>
        // GetBladeAngleForCanvas는 EquipmentRegionHelper로 이동됨

        private void UpdatePmSummary(PmDetailData data)
        {
            uiUpdater?.UpdatePmSummary(data);
        }

        /// <summary>
        /// PmStatusPanel에 환경 정보 업데이트
        /// </summary>
        private void UpdatePmStatusPanelEnvironment(string unitKey)
        {
            uiUpdater?.UpdatePmStatusPanelEnvironment(unitKey);
        }

        /// <summary>
        /// 가스 PV 값 생성 (SV 근처에서 실시간 변동)
        /// </summary>
        internal double GenerateGasPV(double sv)
        {
            if (sv <= 0) return 0;
            
            // SV의 ±5% 범위에서 랜덤 변동
            double variation = sv * 0.05;
            double deviation = (envRandom.NextDouble() - 0.5) * 2 * variation;
            return Math.Max(0, sv + deviation);
        }

        internal string BuildChamberEnvInfo(string unitKey)
        {
            if (!ChamberEnvSpecs.TryGetValue(unitKey, out var spec))
            {
                return "공정 조건: 데이터 없음";
            }

            var chamberState = GetChamberStateForUnit(unitKey);
            var live = UpdateLiveEnvironment(unitKey, chamberState);
            if (live == null)
            {
                return $"공정 조건: T={spec.Temperature}, P={spec.Pressure}, RH={spec.Humidity}\n운전 메모: {spec.Notes}";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"온도 기준 {spec.Temperature} / 현재 {live.TemperatureC:F1}°C");
            sb.AppendLine($"압력 기준 {spec.Pressure} / 현재 {FormatPressure(live.PressureTorr)}");
            sb.AppendLine($"습도 기준 {spec.Humidity} / 현재 {live.HumidityPercent:F1}% RH");
            sb.Append($"운전 메모: {spec.Notes}");
            return sb.ToString();
        }

        internal ChamberController.ChamberState GetChamberStateForUnit(string unitKey)
        {
            // ChamberController를 통해 챔버 상태 가져오기
            return chamberController?.GetChamberStateForUnit(unitKey);
        }

        internal ChamberController.ChamberEnvironmentLive UpdateLiveEnvironment(string unitKey, ChamberController.ChamberState chamberState)
        {
            if (!ChamberEnvSpecs.TryGetValue(unitKey, out var spec))
            {
                return null;
            }

            if (!ChamberEnvLive.TryGetValue(unitKey, out var live))
            {
                live = new ChamberController.ChamberEnvironmentLive
                {
                    TemperatureC = spec.TargetTemperatureC,
                    PressureTorr = spec.TargetPressureTorr,
                    HumidityPercent = spec.TargetHumidityPercent
                };
                ChamberEnvLive[unitKey] = live;
            }

            var activityFactor = GetEnvironmentActivityFactor(chamberState);
            
            // 1% 확률로 알람 발생, 99% 확률로 안전 범위 유지
            bool shouldTriggerAlarm = envRandom.NextDouble() < 0.01;
            
            // 온도: 1% 확률로 알람 범위까지, 99% 확률로 안전 범위
            double tempMaxDev;
            if (shouldTriggerAlarm)
            {
                // 알람 발생: 경고 임계값의 80-100% 범위
                tempMaxDev = AlarmThresholds.TempWarnDiffC * (0.8 + envRandom.NextDouble() * 0.2);
            }
            else
            {
                // 안전 범위: 경고 임계값의 30% 이내
                tempMaxDev = Math.Min(AlarmThresholds.TempWarnDiffC * 0.3, 0.5);
            }
            live.TemperatureC = UpdateEnvChannel(live.TemperatureC, spec.TargetTemperatureC, 0.15, tempMaxDev);
            
            // 압력: 1% 확률로 알람 발생
            // 압력은 고압/저압에 따라 비율과 절대값 기준이 다르게 적용됨
            double pressMaxDev;
            if (shouldTriggerAlarm)
            {
                // 알람 발생: 경고 임계값의 80-100% 범위
                // 고압(>10 Torr): 절대값 기준 우선, 저압(≤10 Torr): 비율 기준 우선
                double pressWarnLimit;
                if (spec.TargetPressureTorr > 10.0)
                {
                    // 고압: 절대값 기준 (비율 기준은 너무 큼)
                    pressWarnLimit = AlarmThresholds.PressWarnAbsTorr;
                }
                else
                {
                    // 저압: 비율과 절대값 중 작은 값 사용
                    pressWarnLimit = Math.Min(
                        spec.TargetPressureTorr * AlarmThresholds.PressWarnRatio,
                        AlarmThresholds.PressWarnAbsTorr
                    );
                }
                pressMaxDev = pressWarnLimit * (0.8 + envRandom.NextDouble() * 0.2);
            }
            else
            {
                // 안전 범위: 경고 임계값의 30% 이내
                // 고압/저압에 따라 적절한 기준 사용
                if (spec.TargetPressureTorr > 10.0)
                {
                    // 고압: 절대값 기준의 30%
                    pressMaxDev = AlarmThresholds.PressWarnAbsTorr * 0.3;
                }
                else
                {
                    // 저압: 비율과 절대값 중 작은 값의 30%
                    pressMaxDev = Math.Max(
                        Math.Min(spec.TargetPressureTorr * AlarmThresholds.PressWarnRatio * 0.3, AlarmThresholds.PressWarnAbsTorr * 0.3),
                        0.0001
                    );
                }
            }
            live.PressureTorr = UpdateEnvChannel(live.PressureTorr, spec.TargetPressureTorr, 0.2, pressMaxDev);
            
            // 습도: 고정 범위로 제한 (알람 없음)
            live.HumidityPercent = UpdateEnvChannel(live.HumidityPercent, spec.TargetHumidityPercent, 0.18, 1.0);

            live.TemperatureC = Math.Round(live.TemperatureC, 2);
            live.PressureTorr = spec.TargetPressureTorr < 1.0
                ? Math.Round(live.PressureTorr, 4)
                : Math.Round(live.PressureTorr, 2);
            live.HumidityPercent = Math.Round(live.HumidityPercent, 2);
            return live;
        }

        private double UpdateEnvChannel(double value, double target, double settleRatio, double maxDeviation)
        {
            // maxDeviation이 이미 알람 발생 여부를 반영하므로 그대로 사용
            double safeMaxDeviation = maxDeviation;
            if (target > 0)
            {
                // 온도: maxDeviation이 이미 설정된 값이므로 그대로 사용
                if (maxDeviation < 10) // 온도 채널로 추정
                {
                    // maxDeviation이 이미 알람 여부를 반영하므로 추가 제한 없음
                    safeMaxDeviation = maxDeviation;
                }
                // 압력: 고압/저압에 따라 적절히 처리
                else if (target < 1000) // 압력 채널로 추정 (온도보다 큰 값)
                {
                    // maxDeviation이 이미 알람 여부를 반영하므로 추가 제한 없음
                    safeMaxDeviation = maxDeviation;
                }
            }
            
            // 노이즈 범위: 알람 발생 시에는 더 큰 변동 허용
            double noiseFactor = maxDeviation > (AlarmThresholds.TempWarnDiffC * 0.5) ? 0.25 : 0.15;
            var noise = (envRandom.NextDouble() - 0.5) * (safeMaxDeviation * noiseFactor);
            var updated = value + (target - value) * settleRatio + noise;
            return ClampValue(updated, target - safeMaxDeviation, target + safeMaxDeviation);
        }

        internal double GenerateSafeRandomValue(double sv, double warnThreshold, double alarmThreshold)
        {
            // 1% 확률로 알람 발생
            bool shouldTriggerAlarm = envRandom.NextDouble() < 0.01;
            
            if (sv == 0)
            {
                // SV가 0인 경우 누설 경고
                if (shouldTriggerAlarm)
                {
                    // 알람 발생: 누설 경고 임계값의 80-100% 범위
                    double maxLeak = AlarmThresholds.GasLeakWarnSccm * (0.8 + envRandom.NextDouble() * 0.2);
                    return envRandom.NextDouble() * maxLeak;
                }
                else
                {
                    // 안전 범위: 누설 경고 임계값의 20% 이내
                    double maxLeak = AlarmThresholds.GasLeakWarnSccm * 0.2;
                    return envRandom.NextDouble() * maxLeak;
                }
            }
            
            // 가스/RF 값 생성
            double safeRange;
            if (shouldTriggerAlarm)
            {
                // 알람 발생: 경고 임계값의 80-100% 범위
                safeRange = warnThreshold * (0.8 + envRandom.NextDouble() * 0.2);
            }
            else
            {
                // 안전 범위: 경고 임계값의 20% 이내
                safeRange = Math.Min(warnThreshold * 0.2, alarmThreshold * 0.15);
            }
            double deviation = (envRandom.NextDouble() - 0.5) * safeRange * 2;
            return Math.Max(0, sv + deviation);
        }

        private static double GetEnvironmentActivityFactor(ChamberController.ChamberState chamberState)
        {
            if (chamberState == null)
            {
                return 0.0;
            }

            if (chamberState.CurrentWafer == null)
            {
                return 0.1;
            }

            if (chamberState.StatusText != null &&
                (chamberState.StatusText.IndexOf("공정", StringComparison.OrdinalIgnoreCase) >= 0
                 || chamberState.StatusText.IndexOf("처리", StringComparison.OrdinalIgnoreCase) >= 0
                 || chamberState.StatusText.IndexOf("Processing", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return 1.0;
            }

            return 0.5;
        }

        private static double ClampValue(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        private string FormatPressure(double torr)
        {
            if (torr >= 100)
            {
                return $"{torr:F0} Torr";
            }

            if (torr >= 1)
            {
                return $"{torr:F2} Torr";
            }

            if (torr >= 0.01)
            {
                return $"{torr * 1000:F1} mTorr";
            }

            return $"{torr * 1_000_000:F1} µTorr";
        }

        private void UpdateFoupStatusCards(
            string foupAStatus, int foupACount, string foupAPath, string foupAPPID, string foupALotId, string foupAMid, string foupALock,
            string foupBStatus, int foupBCount, string foupBPath, string foupBPPID, string foupBLotId, string foupBMid, string foupBLock,
            string exchangeState, string queueInfo)
        {
            uiUpdater?.UpdateFoupStatusCards(foupAStatus, foupACount, foupAPath, foupAPPID, foupALotId, foupAMid, foupALock,
                foupBStatus, foupBCount, foupBPath, foupBPPID, foupBLotId, foupBMid, foupBLock,
                exchangeState, queueInfo);
        }

        private void UpdateFoupLevelFill(Panel trackPanel, Panel fillPanel, int waferCount)
        {
            uiUpdater?.UpdateFoupLevelFill(trackPanel, fillPanel, waferCount);
        }

        private void DrawFoupLevelSlots(Panel trackPanel, Graphics g, int waferCount, int capacity, Queue<Wafer> FoupAQueue = null, List<Wafer> foupBList = null)
        {
            if (trackPanel == null || g == null)
            {
                return;
            }

            var rect = trackPanel.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // 슬롯 개수는 capacity와 동일 (FOUP는 25층이지만, 여기서는 capacity만큼만 표시)
            int slotCount = capacity;
            float slotHeight = rect.Height / (float)slotCount;
            
            // 배경 그리기
            using (var bgBrush = new SolidBrush(trackPanel.BackColor))
            {
                g.FillRectangle(bgBrush, rect);
            }
            
            // 슬롯 구분선 그리기
            using (var slotPen = new Pen(AppSettings.SeparatorLineColor, 1f))
            {
                for (int i = 1; i < slotCount; i++)
                {
                    float y = rect.Top + i * slotHeight;
                    g.DrawLine(slotPen, rect.Left, y, rect.Right, y);
            }
            }
            
            // 개별 웨이퍼 슬롯 표시 (1층부터 아래로, 1층이 맨 아래)
            // Queue나 List에서 실제 웨이퍼를 확인하여 각 층에 정확히 표시
            waferCount = Math.Max(0, Math.Min(capacity, waferCount));
            using (var waferBrush = new SolidBrush(AppSettings.WaferBrushColor))
            using (var waferPen = new Pen(AppSettings.WaferPenColor, 1f))
            {
                // 실제 웨이퍼 배열 가져오기 (FOUP A는 Queue, FOUP B는 List)
                Wafer[] waferArray = null;
                if (FoupAQueue != null && foupManager?.HasWafersInFoupA() == true)
                {
                    waferArray = FoupAQueue.ToArray();
                }
                else if (foupBList != null && foupBList.Count > 0)
                {
                    waferArray = foupBList.ToArray();
                }
                
                // 각 층을 확인하여 웨이퍼 표시
                // 1층부터 waferCount개만큼 웨이퍼가 있음
                // 예: waferCount=3일 때 1, 2, 3층에 웨이퍼 표시
                // waferCount=2일 때 1, 2층에 웨이퍼 표시 (1층이 사라지면 1층 표시도 사라짐)
                for (int layer = 1; layer <= capacity; layer++)
                {
                    // 해당 층에 웨이퍼가 있는지 확인
                    // 1층부터 waferCount개만큼 있으므로, layer <= waferCount이면 웨이퍼가 있음
                    bool hasWafer = layer <= waferCount;
                    
                    if (!hasWafer)
                    {
                        continue; // 해당 층에 웨이퍼가 없으면 표시하지 않음
                    }
                    
                    // 층 위치 계산: 1층이 맨 아래 (slotIndex = slotCount - 1)
                    int slotIndex = slotCount - layer;
                    float slotY = rect.Top + slotIndex * slotHeight;
                    var slotRect = new RectangleF(
                        rect.X + 2,
                        slotY + 1,
                        rect.Width - 4,
                        slotHeight - 2
                    );
                    
                    // 웨이퍼 슬롯 채우기
                    g.FillRectangle(waferBrush, slotRect);
                    g.DrawRectangle(waferPen, slotRect.X, slotRect.Y, slotRect.Width, slotRect.Height);
                }
            }
            
            // 외곽 테두리
            using (var borderPen = new Pen(AppSettings.BorderColor, 1f))
            {
                g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }

        private void InitializeFoupTrackPanels()
        {
            uiInitializer?.InitializeFoupTrackPanels();
                }
                
        internal void DrawSingleWaferTrack(Panel trackPanel, Graphics g, int layer, Queue<Wafer> FoupAQueue = null, List<Wafer> foupBList = null)
        {
            if (trackPanel == null || g == null)
            {
                return;
            }

            var rect = trackPanel.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // 해당 층에 웨이퍼가 있는지 확인
            // 각 층은 고정된 웨이퍼 ID를 가짐: 1층=1번, 2층=2번, 3층=3번, 4층=4번, 5층=5번
            // 웨이퍼가 나가면 해당 층만 비워지고, 다른 웨이퍼는 내려오지 않음
            bool hasWafer = false;
            if (FoupAQueue != null && foupManager?.HasWafersInFoupA() == true)
            {
                // FOUP A: Queue에서 layer번 ID를 가진 웨이퍼가 있는지 확인
                // 예: 1층에는 1번 웨이퍼만, 2층에는 2번 웨이퍼만 표시
                hasWafer = foupManager.HasWaferAtLayerFoupA(layer);
            }
            else if (foupBList != null && foupBList.Count > 0)
            {
                // FOUP B: List에서 layer번 ID를 가진 웨이퍼가 있는지 확인
                // List도 동일하게 각 층은 고정된 웨이퍼 ID를 가짐
                hasWafer = foupBList.Any(w => w.Id == layer);
            }
            
            // 패널 테두리 영역 (전체 패널 크기)
            RectangleF panelRect = rect;
            
            // 웨이퍼와 패널 테두리 사이 간격 (FOUP에 장착된 느낌을 주기 위해)
            float waferMargin = 2f; // 좌우상하 여백 2px
            
            // 웨이퍼 영역 (패널 테두리보다 약간 작게)
            RectangleF waferRect = new RectangleF(
                panelRect.X + waferMargin,
                panelRect.Y + waferMargin,
                panelRect.Width - waferMargin * 2,
                panelRect.Height - waferMargin * 2
            );
            
            // 배경 그리기
            using (var bgBrush = new SolidBrush(trackPanel.BackColor))
            {
                g.FillRectangle(bgBrush, panelRect);
            }
            
            // 웨이퍼가 있으면 표시 (실제 웨이퍼처럼 좌우로 길고 상하로 얇게)
            if (hasWafer)
            {
                using (var waferBrush = new SolidBrush(AppSettings.WaferBrushColor))
                using (var waferPen = new Pen(AppSettings.WaferPenColor, 1f))
                {
                    g.FillRectangle(waferBrush, waferRect);
                    g.DrawRectangle(waferPen, waferRect.X, waferRect.Y, waferRect.Width, waferRect.Height);
                }
            }
            
            // 슬롯 테두리: 패널 크기에 맞춰 그리기
            using (var borderPen = new Pen(AppSettings.BorderColor, 1f))
            {
                g.DrawRectangle(borderPen, panelRect.X, panelRect.Y, panelRect.Width, panelRect.Height);
            }
        }

        private void RegisterPanelForDetail(Control panel, string unitKey)
        {
            if (panel == null)
            {
                return;
            }

            panel.Tag = unitKey;
            panel.Cursor = Cursors.Hand;
            panel.Click -= equipmentEventHandlers.EquipmentPanel_Click;
            panel.Click += equipmentEventHandlers.EquipmentPanel_Click;

            foreach (Control child in panel.Controls)
            {
                RegisterPanelForDetail(child, unitKey);
            }
        }

        private void EquipmentPanel_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            var unitKey = control?.Tag as string;
            if (string.IsNullOrEmpty(unitKey))
            {
                return;
            }

            HandleEquipmentUnitClick(unitKey);
        }

        internal void HandleEquipmentUnitClick(string unitKey)
        {
            if (!IsLoggedIn)
            {
                MessageBox.Show("상세 정보를 보려면 먼저 로그인하세요.", "권한 필요",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (unitKey == "PMA" || unitKey == "PMB" || unitKey == "PMC")
            {
                ShowPmDetail(unitKey);
                return;
            }

            string status = GetUnitStatusText(unitKey);
            string description = GetUnitDescription(unitKey);

            using (var detailForm = new EquipmentDetailForm())
            {
                detailForm.SetDetail(unitKey, status, description);
                detailForm.ShowDialog(this);
            }
        }

        internal void ShowPmDetail(string unitKey)
        {
            if (!PmDetails.TryGetValue(unitKey, out var detail))
            {
                return;
            }

            using (var detailForm = new PmDetailForm())
            {
                detailForm.SetDetail(
                    unitKey,
                    detail.StatusText,
                    detail.RecipeName,
                    detail.StepName,
                    $"{detail.RecipeTimeCurrent} / {detail.RecipeTimeTotal}",
                    $"{detail.StepTimeCurrent} / {detail.StepTimeTotal}",
                    $"{detail.StepIndex} / {detail.StepCount}",
                    detail.StepMessage,
                    detail.StepProgress);

                // 환경 PV/SV 세팅
                var chamberState = GetChamberStateForUnit(unitKey);
                var live = UpdateLiveEnvironment(unitKey, chamberState);
                double pvTemp = live != null ? live.TemperatureC : 0;
                double pvPress = live != null ? live.PressureTorr : 0;
                double pvHum = live != null ? live.HumidityPercent : 0; // 자리용, RF Power는 시뮬레이션 값 없음

                // 타겟(SV)은 레시피 설정값 사용
                double targetPressureTorr = 0;
                double targetTemperatureC = 0;
                if (ChamberEnvSpecs.TryGetValue(unitKey, out var spec))
                {
                    targetPressureTorr = spec.TargetPressureTorr;
                    targetTemperatureC = spec.TargetTemperatureC;
                }

                // 가스/전력 SV는 레시피에서 설정한 값 사용
                var gasRfSv = UnitGasRfSv.TryGetValue(unitKey, out var sv) ? sv : (NF3: 0.0, O2: 0.0, CF4: 0.0, RF: 0.0);
                double svNF3 = gasRfSv.NF3;
                double svO2 = gasRfSv.O2;
                double svCF4 = gasRfSv.CF4;
                double svRf = gasRfSv.RF;

                // PV는 SV 주변에서 랜덤 생성 (알람 임계값 내에서)
                double pvNF3 = GenerateSafeRandomValue(svNF3, AlarmThresholds.GasWarnAbsSccm, AlarmThresholds.GasAlarmAbsSccm);
                double pvO2 = GenerateSafeRandomValue(svO2, AlarmThresholds.GasWarnAbsSccm, AlarmThresholds.GasAlarmAbsSccm);
                double pvCF4 = GenerateSafeRandomValue(svCF4, AlarmThresholds.GasWarnAbsSccm, AlarmThresholds.GasAlarmAbsSccm);
                // RF: 경고 임계값의 20% 이내에서 변동 (알람 방지)
                double pvRf = 0;
                if (svRf > 0)
                {
                    double rfWarnThreshold = svRf * AlarmThresholds.RfWarnRatio;
                    double rfAlarmThreshold = svRf * AlarmThresholds.RfAlarmRatio;
                    double rfSafeRange = Math.Min(rfWarnThreshold * 0.2, rfAlarmThreshold * 0.15);
                    double rfDeviation = (envRandom.NextDouble() - 0.5) * rfSafeRange * 2;
                    pvRf = Math.Max(0, svRf + rfDeviation);
                }

                detailForm.SetEnvironment(
                    pvNF3, svNF3,
                    pvO2, svO2,
                    pvCF4, svCF4,
                    pvPress, targetPressureTorr,
                    pvRf, 0,
                    pvTemp, targetTemperatureC);
                detailForm.ShowDialog(this);
            }
        }

        internal string GetUnitStatusText(string unitKey)
        {
            switch (unitKey)
            {
                case "TM":
                    return labelSummaryTMStatus.Text;
                case "PMA":
                case "PMB":
                case "PMC":
                    // PmStatusPanel을 사용하므로 기존 라벨은 사용하지 않음
                    return "N/A";
                case "FOUP A":
                    return $"{labelFoupAStatusHeadline.Text} / {labelFoupALockValue.Text}";
                case "FOUP B":
                    return $"{labelFoupBStatusHeadline.Text} / {labelFoupBLockValue.Text}";
                case "Main Lamp":
                    return $"현재 상태: {CurrentProcessState}";
                default:
                    return $"현재 상태: {CurrentProcessState}";
            }
        }

        internal string GetUnitDescription(string unitKey)
        {
            string processInfo;
            switch (CurrentProcessState)
            {
                case ProcessState.Running:
                    processInfo = "장비가 공정을 수행 중입니다.";
                    break;
                case ProcessState.Paused:
                    processInfo = "장비가 일시 정지되어 있습니다.";
                    break;
                case ProcessState.Error:
                    processInfo = "장비가 알람 상태입니다. 이상 여부를 확인해주세요.";
                    break;
                default:
                    processInfo = "장비가 대기 상태입니다.";
                    break;
            }

            switch (unitKey)
            {
                case "TM":
                    return $"{processInfo}{Environment.NewLine}TM은 웨이퍼를 각 챔버로 이송하는 로봇입니다.";
                case "PMA":
                    return $"{processInfo}{Environment.NewLine}Chamber A(PMA)는 전처리 공정을 담당합니다.";
                case "PMB":
                    return $"{processInfo}{Environment.NewLine}Chamber B(PMB)는 메인 공정을 수행합니다.";
                case "PMC":
                    return $"{processInfo}{Environment.NewLine}Chamber C(PMC)는 후처리 혹은 모니터링 용도로 사용됩니다.";
                case "FOUP A":
                    return $"{labelFoupAStatusHeadline.Text}{Environment.NewLine}"
                        + $"Path: {labelFoupAPathValue.Text}{Environment.NewLine}"
                        + $"PPID: {labelFoupAPPIDValue.Text}{Environment.NewLine}"
                        + $"LOTID: {labelFoupALotIdValue.Text}{Environment.NewLine}"
                        + $"MID: {labelFoupAMidValue.Text}{Environment.NewLine}"
                        + $"Lock: {labelFoupALockValue.Text}";
                case "FOUP B":
                    return $"{labelFoupBStatusHeadline.Text}{Environment.NewLine}"
                        + $"Path: {labelFoupBPathValue.Text}{Environment.NewLine}"
                        + $"PPID: {labelFoupBPPIDValue.Text}{Environment.NewLine}"
                        + $"LOTID: {labelFoupBLotIdValue.Text}{Environment.NewLine}"
                        + $"MID: {labelFoupBMidValue.Text}{Environment.NewLine}"
                        + $"Lock: {labelFoupBLockValue.Text}";
                case "Main Lamp":
                    return $"{processInfo}{Environment.NewLine}메인 램프는 장비 전체 상태를 나타냅니다.";
                default:
                    return processInfo;
            }
        }

        private void SetControlPanelEnabled(bool enabled)
        {
            groupBoxControlButtons.Enabled = enabled;
            groupBoxRecipe.Enabled = enabled;
            if (groupBoxFoupReady != null)
            {
                groupBoxFoupReady.Enabled = enabled;
            }
            UpdateProcessControlButtons();
            UpdateFoupPreparationButtons();
        }

        internal void SetProcessState(ProcessState state, string logMessage = null)
        {
            // 알람 발생 시 이전 상태 저장 (Error로 변경될 때만 저장)
            if (state == ProcessState.Error && CurrentProcessState != ProcessState.Error)
            {
                var currentState = CurrentProcessState;
                _stateBeforeAlarm = currentState;
                AddLogMessage($"알람 발생: 이전 상태({currentState}) 저장", "INFO");
            }
            
            CurrentProcessState = state;
            if (state != ProcessState.Running)
            {
                mainLampBlinkState = false;
            }

            switch (state)
            {
                case ProcessState.Running:
                    StatusProcessText = "공정 상태: 진행 중";
                    statusPressureText = "챔버 압력: 공정 압력 유지";
                    statusTemperatureText = "온도: 목표 온도 유지";
                    StatusDoorText = "문 상태: 인터락 유지";
                    mainLampBlinkState = true;
                    UpdateMainLampColors(Color.LimeGreen, false, false);
                    break;
                case ProcessState.Paused:
                    StatusProcessText = "공정 상태: 일시 정지";
                    statusPressureText = "챔버 압력: 안정";
                    statusTemperatureText = "온도: 유지 모드";
                    StatusDoorText = "문 상태: 인터락 유지";
                    UpdateMainLampColors(null, true, false);
                    break;
                case ProcessState.Error:
                    StatusProcessText = "공정 상태: 경고 / 정지";
                    statusPressureText = "챔버 압력: 점검 필요";
                    statusTemperatureText = "온도: 점검 필요";
                    StatusDoorText = "문 상태: 확인 필요";
                    UpdateMainLampColors(null, false, true);
                    break;
                default:
                    StatusProcessText = "공정 상태: 대기";
                    statusPressureText = "챔버 압력: 안정";
                    statusTemperatureText = "온도: 정상 범위 유지";
                    StatusDoorText = "문 상태: 모두 닫힘";
                    UpdateMainLampColors(null, true, false);
                    break;
            }

            StatusProcessDetail = StatusProcessText;
            ApplyStatusTextsToLabels();
            UpdateProcessControlButtons();

            if (!string.IsNullOrWhiteSpace(logMessage))
            {
                var level = state == ProcessState.Error ? "ALARM" : "INFO";
                AddLogMessage(logMessage, level);
            }
        }

        internal void ApplyStatusTextsToLabels()
        {
            var processDisplay = string.IsNullOrEmpty(StatusProcessDetail)
                ? StatusProcessText
                : StatusProcessDetail;

            if (labelProcessValue != null) labelProcessValue.Text = processDisplay;
            if (labelPressureValue != null) labelPressureValue.Text = statusPressureText;
            if (labelTemperatureValue != null) labelTemperatureValue.Text = statusTemperatureText;
            if (labelDoorValue != null) labelDoorValue.Text = StatusDoorText;

            SetLamp(panelStatusLampProcess, true, GetStatusColor(StatusProcessText));
            SetLamp(panelStatusLampPressure, true, GetStatusColor(statusPressureText));
            SetLamp(panelStatusLampTemperature, true, GetStatusColor(statusTemperatureText));
            SetLamp(panelStatusLampDoor, true, GetStatusColor(StatusDoorText));

            SetDoorPanelState(panelStatusDoorProcess, StatusProcessText);
            SetDoorPanelState(panelStatusDoorPressure, statusPressureText);
            SetDoorPanelState(panelStatusDoorTemperature, statusTemperatureText);
            SetDoorPanelState(panelStatusDoorOverall, StatusDoorText);
        }

        private Color GetStatusColor(string statusText)
        {
            if (statusText.IndexOf("경고", StringComparison.OrdinalIgnoreCase) >= 0
                || statusText.IndexOf("점검", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.Firebrick;
            }
            if (statusText.IndexOf("정지", StringComparison.OrdinalIgnoreCase) >= 0
                || statusText.IndexOf("일시", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Color.Goldenrod;
            }

            return Color.FromArgb(76, 175, 80);
        }

        private void UpdateHeaderCardStatuses(string tm, string pma, string pmb, string pmc)
        {
            uiUpdater?.UpdateHeaderCardStatuses(tm, pma, pmb, pmc);
        }

        // Form1_FormClosing 메서드는 FormEventHandlers.Form1_FormClosing으로 이동됨
        // 중복 실행 방지를 위해 제거됨


        internal void UpdateHeaderClock()
        {
            uiUpdater?.UpdateHeaderClock();
        }

        internal void InitializeSimulationState()
        {
            uiInitializer?.InitializeSimulationState();
        }

        private void InitializeEnvironmentTelemetry()
        {
            uiInitializer?.InitializeEnvironmentTelemetry();
        }

        internal void StartSimulation()
        {
            // EtherCAT 연결 확인 (연결되어 있으면 실제 장비 제어)
            if (EthercatConnected)
            {
                try
                {
                    // 초기 상태 설정: 모든 도어 닫힘, Chamber 램프 OFF
                    uiConfigurator.ResetDoorStates();
                    
                    // 모든 Chamber 램프 OFF
                    SetChamberLamp(EquipmentRegion.ChamberA, false);
                    SetChamberLamp(EquipmentRegion.ChamberB, false);
                    SetChamberLamp(EquipmentRegion.ChamberC, false);
                    
                    // 공정 시작 전 서보 ON + 원점복귀 자동 수행
                    if (!TmHardwareInitialized)
                    {
                        AddLogMessage("공정 시작 - TM 하드웨어 초기화 (서보 ON + 원점복귀)", "INFO");
                        if (!PerformAutoServoOnAndHoming())
                        {
                            AddLogMessage("TM 하드웨어 초기화 실패 - 공정 시작 불가", "ERROR");
                            MessageBox.Show(
                                "TM 하드웨어 초기화에 실패했습니다.\n\n" +
                                "확인 사항:\n" +
                                "1. 서보 모터 상태 확인\n" +
                                "2. 실린더가 후진 상태인지 확인\n" +
                                "3. EtherCAT 연결 상태 확인\n\n" +
                                "공정을 시작할 수 없습니다.",
                                "초기화 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // 공정 시작 중단
                        }
                    }
                    else
                    {
                        // 이미 초기화된 경우, 서보 상태 동기화 (장비제어 폼에서 서보 ON한 경우 대비)
                        if (TmHardwareController != null)
                        {
                            AddLogMessage("공정 시작 - 서보 상태 동기화", "INFO");
                            TmHardwareController.SyncServoStatus();
                            
                            // 서보 상태 확인
                            if (!TmHardwareController.IsServoOn)
                            {
                                AddLogMessage("서보가 OFF 상태입니다. 서보를 ON한 후 공정을 시작해주세요.", "ERROR");
                                MessageBox.Show(
                                    "서보가 OFF 상태입니다.\n\n" +
                                    "확인 사항:\n" +
                                    "1. 서보 모터 ON 확인\n" +
                                    "2. 원점복귀 완료 확인\n\n" +
                                    "공정을 시작할 수 없습니다.",
                                    "서보 OFF", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return; // 공정 시작 중단
                            }
                        }
                    }
                    
                    if (TmHardwareInitialized)
                    {
                        AddLogMessage("하드웨어 모드로 공정 시작 - 실제 장비 제어 활성화", "INFO");
                    }
                    else
                    {
                        AddLogMessage("시뮬레이션 모드로 시작 (TM 하드웨어 미초기화)", "INFO");
                    }
                }
                catch (Exception ex)
                {
                    AddLogMessage($"장비 초기화 오류: {ex.Message}", "ERROR");
                }
            }

            uiInitializer.InitializeSimulationState();
            // 공정 시작 시 환경 텔레메트리 초기화 (PV 값이 이제부터 생성됨)
            uiInitializer.InitializeEnvironmentTelemetry();
            // 이전 완료 수량을 기준선으로 저장하여 신규 배치 완료 판정을 분리
            FoupBCompletedBaseline = foupManager?.GetFoupBCount() ?? 0;
            int targetLoadCount = GetEffectiveWaferTarget();
            activeBatchTargetCount = targetLoadCount;
            
            // 웨이퍼 로딩 시 이미 큐에 추가되었을 수 있으므로, 큐가 비어있을 때만 추가
            if (foupManager?.HasWafersInFoupA() != true)
            {
                foupManager?.LoadWafersToFoupA(targetLoadCount, SecondExposureEnabled);
            }

            // SimulationController를 통해 시뮬레이션 시작
            SimulationController?.Start(IsTmHardwareModeAvailable());
            
            // 하드웨어 모드와 시뮬레이션 모드 구분하여 로그 메시지 표시
            if (IsTmHardwareModeAvailable())
            {
                AddLogMessage($"하드웨어 모드 공정 시작 - 웨이퍼 {targetLoadCount}장 투입", "INFO");
                SetProcessState(ProcessState.Running, $"하드웨어 모드 공정 시작 - 웨이퍼 {targetLoadCount}장 투입");
            }
            else
            {
                AddLogMessage($"시뮬레이션 시작 - 웨이퍼 {targetLoadCount}장 투입", "INFO");
                SetProcessState(ProcessState.Running, $"시뮬레이션 시작 - 웨이퍼 {targetLoadCount}장 투입");
            }
            
            TryScheduleLoadFromFoupA();
            ProcessTm();
            UpdateSimulationUi();
        }

        internal void ResumeSimulation()
        {
            if (!SimulationRunning || !SimulationPaused)
            {
                return;
            }

            // 일시정지 시간만큼 tmHardwareActionStartTime 조정 (타임아웃 계산 시 일시정지 시간 제외)
            if (TmHardwareActionPending && pauseStartTime.HasValue)
            {
                var pauseDuration = DateTime.Now - pauseStartTime.Value;
                tmHardwareActionStartTime = tmHardwareActionStartTime.Add(pauseDuration);
                pauseStartTime = null; // 재개 후 초기화
            }

            // 하드웨어 모드: 서보는 ON 상태 유지되어 있음
            // 일시정지 시 서보를 OFF하지 않았으므로 다시 ON할 필요 없음
            if (IsTmHardwareModeAvailable())
            {
                AddLogMessage("하드웨어 모드 공정 재개: 현재 Phase부터 계속", "INFO");
                SetProcessState(ProcessState.Running, "하드웨어 모드 공정을 재개했습니다.");
            }
            else
            {
                AddLogMessage("시뮬레이션 재개: 현재 Phase부터 계속", "INFO");
                SetProcessState(ProcessState.Running, "시뮬레이션을 재개했습니다.");
            }

            // SimulationController를 통해 시뮬레이션 재개
            SimulationController?.Resume(IsTmHardwareModeAvailable());
            UpdateSimulationUi();
        }

        // 사용자 로딩 수를 기준으로 투입 대상 웨이퍼 수를 계산
        private int GetEffectiveWaferTarget()
        {
            // 레시피 장수와 사용자 로딩 수 중 더 작은 값을 목표로 사용
            int cappedUser = Math.Max(1, Math.Min(AppSettings.MaxFoupCapacity, UserWaferLoadCount));
            int cappedRecipe = Math.Max(1, Math.Min(AppSettings.MaxFoupCapacity, CurrentRecipeWaferCount));
            return Math.Min(cappedUser, cappedRecipe);
        }

        // 진행 중에는 고정된 배치 목표를 사용
        internal int GetActiveTarget()
        {
            return activeBatchTargetCount > 0 ? activeBatchTargetCount : GetEffectiveWaferTarget();
        }

        internal void PauseSimulation()
        {
            if (!SimulationRunning || SimulationPaused)
            {
                return;
            }

            // 일시정지 시작 시간 저장 (타임아웃 계산 시 일시정지 시간 제외용)
            if (TmHardwareActionPending)
            {
                pauseStartTime = DateTime.Now;
            }

            // SimulationController를 통해 시뮬레이션 일시정지
            SimulationController?.Pause();
            
            // 하드웨어 모드와 시뮬레이션 모드 구분하여 로그 메시지 표시
            if (IsTmHardwareModeAvailable())
            {
                AddLogMessage("하드웨어 모드 공정 일시정지", "INFO");
                SetProcessState(ProcessState.Paused, "하드웨어 모드 공정을 일시 정지했습니다.");
            }
            else
            {
                AddLogMessage("시뮬레이션 일시정지", "INFO");
                SetProcessState(ProcessState.Paused, "시뮬레이션을 일시 정지했습니다.");
            }
            
            UpdateSimulationUi();
        }

        internal void AbortSimulation(string reason, bool setErrorState)
        {
            SimulationController?.Abort();
            if (setErrorState)
            {
                SetProcessState(ProcessState.Error, reason);
            }
            else
            {
                SetProcessState(ProcessState.Idle, reason);
            }
            UpdateSimulationUi();
            UpdateTmAnimationIdleTarget();
        }


        internal void AdvanceSimulation()
        {
            SimulationService?.IncrementElapsedTime();
            
            // 하드웨어 모드에서 TM 동작 중인 경우
            bool tmBusyInHardwareMode = IsTmHardwareModeAvailable() && TmHardwareActionPending;
            
            // Chamber 시간은 항상 감소 (하드웨어 모드에서도 공정 시간은 흘러야 함)
            DecrementChamberTime(ChamberAState);
            DecrementChamberTime(ChamberBState);
            DecrementChamberTime(ChamberCState);

            // 하드웨어 모드에서 TM 동작 중이면 새로운 스케줄링 스킵
            // TM이 실제 하드웨어 동작을 완료할 때까지 대기
            if (!tmBusyInHardwareMode)
            {
                // 스케줄링 우선순위 (효율성 최적화):
                // 1. Chamber A에서 B/C로 이송 - 빈 Chamber로 웨이퍼 이동 (조건부 최우선)
                //    - A → B/C가 가능할 때만 우선 처리 (B 또는 C가 비어있어야 함)
                //    - A → B/C가 불가능하면 (B와 C 모두 사용 중) → C → FOUP B 우선 처리
                // 2. 완료 웨이퍼를 빼기 (B/C → FOUP B) - Chamber를 비워서 다음 웨이퍼 수용
                // 3. FOUP A에서 Chamber A로 투입 - 새 웨이퍼 로딩
                // 
                // 이유: A → B/C 경로(75초)가 C → FOUP B 경로(45초)보다 전체 프로세스가 더 길기 때문에
                //      A를 먼저 비워서 다음 웨이퍼 투입을 가능하게 하는 것이 전체 처리량 향상에 유리
                //      단, B와 C가 모두 사용 중이면 A → B/C가 불가능하므로 C → FOUP B를 먼저 처리
                // 병렬 가공 모드: Chamber A는 B 또는 C로만 이동 (FOUP B로 직접 이동 불가)
                // 스케줄링 우선순위 (총 가공시간 최소화):
                // 1. A → B/C 스케줄링 (A 비우기)
                // 2. FOUP A → A (새 웨이퍼 투입) ← A를 비웠다면 즉시 새 웨이퍼 투입하여 가공 시작
                // 3. B/C → FOUP B (완료된 웨이퍼 제거) ← 완료된 웨이퍼 제거는 새 웨이퍼 투입보다 우선순위 낮음
                //
                // 이유: A를 비웠다면 즉시 새 웨이퍼를 투입하면 A에서 가공이 시작되어 전체 파이프라인이 더 빨리 돌아감
                //      완료된 웨이퍼 제거는 새 웨이퍼 투입보다 우선순위가 낮음
                
                // 1. A → B/C 스케줄링 시도
                bool aToBCScheduled = TryScheduleTransferFromChamberA();
                
                // 2. A를 비웠다면 즉시 새 웨이퍼 투입 (총 가공시간 최소화)
                // A가 비어있고 FOUP A에 웨이퍼가 있으면 즉시 투입
                TryScheduleLoadFromFoupA();
                
                // 3. 완료된 웨이퍼를 FOUP B로 이송 (새 웨이퍼 투입 후 처리)
                // A → B가 스케줄링되었는지 전달하여 우선순위 가드에 사용
                TryScheduleTransferToFoupB(ChamberBState, aToBCScheduled);
                TryScheduleTransferToFoupB(ChamberCState, aToBCScheduled);
            }
            
            // TM 처리 (하드웨어 모드에서는 실제 동작 완료 대기)
            ProcessTm();

            if ((foupManager?.GetFoupBCount() ?? 0) - FoupBCompletedBaseline >= GetActiveTarget()
                && ChamberAState.CurrentWafer == null
                && ChamberBState.CurrentWafer == null
                && ChamberCState.CurrentWafer == null
                && (TransferService?.QueueCount ?? 0) == 0
                && CurrentTransfer == null)
            {
                // 공정 종료 전 원점복귀 수행 (하드웨어 모드)
                if (IsTmHardwareModeAvailable())
                {
                    AddLogMessage("공정 종료 - TM 원점복귀 수행", "INFO");
                    PerformShutdownHoming();
                }
                
                SimulationController?.Stop();
                SimulationController?.StopTimer();
                SetProcessState(ProcessState.Idle, "시뮬레이션이 완료되었습니다.");
                UpdateSimulationUi();
            }

            UpdateTmAnimationIdleTarget();
        }

        /// <summary>
        /// Chamber 공정 시간 감소 처리
        /// BeforeFinal 로직으로 복원: 도어 상태만 확인, TM 작업과 무관하게 공정 시간 감소
        /// </summary>
        internal void DecrementChamberTime(ChamberController.ChamberState chamber)
        {
            if (chamber?.CurrentWafer == null)
            {
                return;
            }

            // 도어가 열려있거나 닫는 중이면 마지막 처리 시간 초기화 (다시 시작할 때 누적 방지)
            if (!IsChamberDoorClosed(chamber) || chamber.StatusText == "Door Closing")
            {
                chamber.LastProcessedTime = DateTime.MinValue;
                return;
            }

            if (chamber.RemainingSeconds <= 0)
            {
                chamber.RemainingSeconds = 0;
                chamber.ProcessingAccumulator = 0;
                chamber.LastProcessedTime = DateTime.MinValue;
                return;
            }

            // 하드웨어 모드: 실제 경과 시간 기반으로 공정 시간 감소 (Thread.Sleep에 영향받지 않음)
            // 시뮬레이션 모드: 틱 기반 (기존 방식)
            if (IsTmHardwareModeAvailable())
            {
                // 실시간 기반: DateTime을 사용하여 정확한 경과 시간 측정
                DateTime now = DateTime.Now;
                
                if (chamber.LastProcessedTime == DateTime.MinValue)
                {
                    // 첫 호출: 시작 시간 기록
                    chamber.LastProcessedTime = now;
                    return;
                }
                
                // 마지막 처리 이후 경과 시간 (초)
                double elapsedSeconds = (now - chamber.LastProcessedTime).TotalSeconds;
                chamber.ProcessingAccumulator += elapsedSeconds;
                chamber.LastProcessedTime = now;
                
                // 1초 이상 누적되면 RemainingSeconds 감소
                while (chamber.ProcessingAccumulator >= 1.0 && chamber.RemainingSeconds > 0)
                {
                    chamber.RemainingSeconds--;
                    chamber.ProcessingAccumulator -= 1.0;
                    if (chamber.RemainingSeconds == 0)
                    {
                        chamber.StatusText = "Transfer Ready";
                        chamber.ProcessingAccumulator = 0;
                        chamber.LastProcessedTime = DateTime.MinValue;
                        AddLogMessage($"{chamber.UnitKey} 공정 완료 - TM 대기 중 (도어 닫힘 유지) (웨이퍼#{chamber.CurrentWafer?.Id})", "INFO");
                        break;
                    }
                }
            }
            else
            {
                // 시뮬레이션 모드: 틱 기반 (기존 방식)
                double secondsPerTick = AppSettings.ChamberSecondsPerTick;  // 1.0초
                chamber.ProcessingAccumulator += secondsPerTick;
                while (chamber.ProcessingAccumulator >= 1.0 && chamber.RemainingSeconds > 0)
                {
                    chamber.RemainingSeconds--;
                    chamber.ProcessingAccumulator -= 1.0;
                    if (chamber.RemainingSeconds == 0)
                    {
                        chamber.StatusText = "Transfer Ready";
                        chamber.ProcessingAccumulator = 0;
                        AddLogMessage($"{chamber.UnitKey} 공정 완료 - TM 대기 중 (도어 닫힘 유지) (웨이퍼#{chamber.CurrentWafer?.Id})", "INFO");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Chamber A에서 B/C로 이송 스케줄링 시도
        /// </summary>
        /// <returns>스케줄링 성공 여부 (B 또는 C로 이동 가능했는지)</returns>
        internal bool TryScheduleTransferFromChamberA()
        {
            if (!IsReadyForTransfer(ChamberAState))
            {
                return false;
            }

            ChamberController.ChamberState target = null;
            
            // 큐에 이미 예약된 작업이 있는지 확인
            bool bHasReservedTask = IsChamberReservedInQueue(ChamberBState);
            bool cHasReservedTask = IsChamberReservedInQueue(ChamberCState);
            
            // 디버깅: Chamber 상태 확인
            bool bIsAvailableRaw = IsChamberAvailable(ChamberBState);
            bool cIsAvailableRaw = IsChamberAvailable(ChamberCState);
            
            bool bAvailable = bIsAvailableRaw && !bHasReservedTask;
            bool cAvailable = cIsAvailableRaw && !cHasReservedTask;
            
            // 디버깅 로그 추가
            AddLogMessage($"[병렬가공] Chamber A → B/C 스케줄링 체크: " +
                $"B(CurrentWafer={ChamberBState?.CurrentWafer?.Id}, Reserved={ChamberBState?.ReservedForIncoming}, " +
                $"AvailableRaw={bIsAvailableRaw}, HasReservedTask={bHasReservedTask}, Available={bAvailable}), " +
                $"C(CurrentWafer={ChamberCState?.CurrentWafer?.Id}, Reserved={ChamberCState?.ReservedForIncoming}, " +
                $"AvailableRaw={cIsAvailableRaw}, HasReservedTask={cHasReservedTask}, Available={cAvailable})", "INFO");
            
            // 중요: Chamber B가 곧 비게 될 것인지 확인 (B 우선순위 보장)
            // 웨이퍼가 있지만 픽업이 예약되어 있으면 → TM이 꺼내고 있으므로 곧 비게 됨
            // BeforeFinal 로직으로 복원: IsChamberReservedInQueue 체크 제거
            bool bWillBeAvailableSoon = !bAvailable 
                && ChamberBState != null
                && ChamberBState.CurrentWafer != null 
                && ChamberBState.PickupScheduled
                && !ChamberBState.ReservedForIncoming;
            
            bool cWillBeAvailableSoon = !cAvailable 
                && ChamberCState != null
                && ChamberCState.CurrentWafer != null 
                && ChamberCState.PickupScheduled
                && !ChamberCState.ReservedForIncoming;
            
            if (SecondExposureEnabled)
            {
                // ========== 2차 노광 모드 (순차 처리) ==========
                // 반드시 A → B → C 순서로 가야 함
                // A에서 완료된 웨이퍼는 B로만 갈 수 있음 (C는 B 거쳐서만 갈 수 있음)
                if (bAvailable || bWillBeAvailableSoon)
                {
                    target = ChamberBState;
                }
                // B가 사용 불가면 대기 (C로 직접 가면 안 됨!)
            }
            else
            {
                // ========== 병렬 처리 모드 ==========
                // 병렬 가공 규칙:
                // - Chamber A → Chamber B 또는 C로 이송
                // - Chamber B와 C는 동일한 가공을 수행
                // - 비어있는 chamber를 우선으로 선택
                //   1. B가 비어있으면 B로 (우선순위)
                //   2. C가 비어있으면 C로
                //   3. 둘 다 비어있으면 B로
                // - Chamber A를 먼저 비워서 새 웨이퍼 투입을 우선하여 총 가공시간을 줄임
                
                // 규칙 1: B가 비어있고 예약되지 않았으면 B로
                if (bAvailable)
                {
                    target = ChamberBState;
                    AddLogMessage("[병렬가공] Chamber B가 비어있음 → B로 이동", "INFO");
                }
                // 규칙 2: C가 비어있고 예약되지 않았으면 C로
                else if (cAvailable)
                {
                    target = ChamberCState;
                    AddLogMessage("[병렬가공] Chamber B는 사용 중, C가 비어있음 → C로 이동", "INFO");
                }
                // 규칙 3: 둘 다 비어있지만 예약된 경우 처리
                else if (bIsAvailableRaw && cIsAvailableRaw)
                {
                    // 둘 다 비어있지만 예약된 경우: 예약되지 않은 쪽 선택, 둘 다 예약되면 B 우선
                    if (!bHasReservedTask)
                    {
                        target = ChamberBState;
                        AddLogMessage("[병렬가공] Chamber B와 C 모두 비어있지만 B가 예약 안 됨 → B로 이동", "INFO");
                    }
                    else if (!cHasReservedTask)
                    {
                        target = ChamberCState;
                        AddLogMessage("[병렬가공] Chamber B와 C 모두 비어있지만 C가 예약 안 됨 → C로 이동", "INFO");
                    }
                    else
                    {
                        // 둘 다 예약되어 있으면 대기 (다음 틱에서 재시도)
                        AddLogMessage("[병렬가공] Chamber B와 C 모두 비어있지만 둘 다 예약됨 → 대기", "INFO");
                    }
                }
                // 규칙 4: 둘 다 공정 중이면 곧 비게 될 챔버 확인
                else
                {
                    if (bWillBeAvailableSoon && !cWillBeAvailableSoon)
                    {
                        target = ChamberBState;
                        AddLogMessage("[병렬가공] Chamber B가 곧 비게 됨 → B로 이동", "INFO");
                    }
                    else if (cWillBeAvailableSoon && !bWillBeAvailableSoon)
                    {
                        target = ChamberCState;
                        AddLogMessage("[병렬가공] Chamber C가 곧 비게 됨 → C로 이동", "INFO");
                    }
                    else if (bWillBeAvailableSoon && cWillBeAvailableSoon)
                    {
                        // 둘 다 곧 비게 되면 B 우선
                        target = ChamberBState;
                        AddLogMessage("[병렬가공] 둘 다 곧 비게 됨 → B로 이동 (우선순위)", "INFO");
                    }
                    else
                    {
                        // 둘 다 공정 중이고 곧 비게 되지 않음
                        AddLogMessage("[병렬가공] Chamber B와 C 모두 공정 중 → 대기", "INFO");
                    }
                }
            }

            if (target == null)
            {
                ChamberAState.StatusText = "Transfer Waiting";
                return false; // 스케줄링 실패 (B와 C 모두 사용 불가)
            }

            ScheduleChamberTransfer(ChamberAState, target);
            return true; // 스케줄링 성공
        }
        
        private bool IsChamberReservedInQueue(ChamberController.ChamberState chamber)
        {
            if (chamber == null)
            {
                return false;
            }
            
            // TransferController의 메서드 사용
            return TransferService?.IsChamberReservedInQueue(chamber) ?? false;
        }

        internal void TryScheduleTransferToFoupB(ChamberController.ChamberState source, bool aToBCScheduled = false)
        {
            if (!IsReadyForTransfer(source))
            {
                return;
            }

            // 우선순위 가드 1: Chamber A가 비어있으면 Chamber A 공정을 최우선으로 수행
            // Chamber A가 비어있고 FOUP A에 웨이퍼가 있으면 B/C에서 FOUP B로 이동하는 것을 보류
            // A로 향하는 예약이 이미 있지 않은 경우에만 B/C→FOUP B 예약을 잠시 보류
            if (IsChamberAvailable(ChamberAState)
                && foupManager?.HasWafersInFoupA() == true
                && !IsWaferEnRouteToChamber(ChamberAState))
            {
                source.StatusText = "A 투입 우선 대기";
                return;
            }

            // 우선순위 가드 2: 병렬 공정 모드에서만 적용
            // Chamber A가 완료되어 B/C로 갈 수 있으면 B/C → FOUP B를 보류
            // 총 시간을 고려하면 A → B/C를 먼저 처리하는 것이 전체 처리량 향상에 유리
            // 2차 노광 모드(직렬 공정)에서는 적용하지 않음 (A → B → C 순서가 고정되어 있음)
            if (!SecondExposureEnabled && IsReadyForTransfer(ChamberAState))
            {
                // B에서 FOUP B로 가는 경우: A → C가 가능하면 보류
                if (source == ChamberBState
                    && IsChamberAvailable(ChamberCState))
                {
                    // A → C가 이미 스케줄링되었거나 큐에 있으면 B → FOUP B를 보류
                    if (aToBCScheduled || IsChamberReservedInQueue(ChamberCState))
                    {
                        // A → C가 이미 큐에 있음: B → FOUP B를 보류하여 A → C를 우선 처리
                        source.StatusText = "A → C 우선 대기";
                        return;
                    }
                    else
                    {
                        // A → C가 가능하지만 아직 스케줄링되지 않음: B → FOUP B를 보류
                        source.StatusText = "A → C 우선 대기";
                        return;
                    }
                }
                
                // C에서 FOUP B로 가는 경우: A → B가 가능하면 보류
                if (source == ChamberCState
                    && IsChamberAvailable(ChamberBState))
                {
                    // A → B가 이미 스케줄링되었거나 큐에 있으면 C → FOUP B를 보류
                    if (aToBCScheduled || IsChamberReservedInQueue(ChamberBState))
                    {
                        // A → B가 이미 큐에 있음: C → FOUP B를 보류하여 A → B를 우선 처리
                        source.StatusText = "A → B 우선 대기";
                        return;
                    }
                    else
                    {
                        // A → B가 가능하지만 아직 스케줄링되지 않음: C → FOUP B를 보류
                        source.StatusText = "A → B 우선 대기";
                        return;
                    }
                }
            }

            // 2차 노광 모드: Chamber B → Chamber C 이동 처리
            if (source == ChamberBState && source.CurrentWafer != null)
            {
                bool needs2ndExp = NeedsSecondExposure(source.CurrentWafer);
                
                if (SecondExposureEnabled && needs2ndExp)
                {
                    // Chamber C가 비어있거나 곧 비게 될 예정인지 확인
                    bool cAvailableForSecondExposure = IsChamberAvailable(ChamberCState);
                    bool cWillBeAvailableSoonForSecondExposure = !cAvailableForSecondExposure 
                        && ChamberCState != null
                        && ChamberCState.CurrentWafer != null 
                        && ChamberCState.PickupScheduled
                        && !ChamberCState.ReservedForIncoming;
                    
                    if (!cAvailableForSecondExposure && !cWillBeAvailableSoonForSecondExposure)
                    {
                        source.StatusText = "2차 노광 대기";
                        return;
                    }

                    source.StatusText = "2차 노광 이동 준비";
                    AddLogMessage($"[2차노광] 웨이퍼#{source.CurrentWafer.Id}: Chamber B → Chamber C 이동 예약", "INFO");
                    ScheduleChamberTransfer(source, ChamberCState);
                    return;
                }
            }
            if (SecondExposureEnabled && source == ChamberCState && source.CurrentWafer != null)
            {
                MarkSecondExposureComplete(source.CurrentWafer);
            }

            var task = new TransferController.TransferTask
            {
                Wafer = source.CurrentWafer,
                Pickup = source.Region,
                Dropoff = EquipmentRegion.FoupB,
                SourceChamber = source,
                DestinationChamber = null,
                OnCompleted = wafer =>
                {
                    wafer.CurrentStage = "FOUP B";
                    // 1층부터 적재 (리스트 끝에 추가하여 순서대로 쌓임)
                    // 1번 웨이퍼가 먼저 Add되면 인덱스 0 (1층)
                    // 2번 웨이퍼가 Add되면 인덱스 1 (2층)
                    // ...
                    foupManager?.AddCompletedWaferToFoupB(wafer);
                }
            };

            source.StatusText = "Transfer 대기";
            source.PickupScheduled = true;
            EnqueueTransfer(task);
            AddLogMessage($"TM 예약: {EquipmentRegionHelper.FormatRegionLabel(task.Pickup)} -> {EquipmentRegionHelper.FormatRegionLabel(task.Dropoff)} (웨이퍼 #{task.Wafer.Id})", "INFO");
        }

        internal void TryScheduleLoadFromFoupA()
        {
            if (foupManager?.HasWafersInFoupA() != true || !IsChamberAvailable(ChamberAState))
            {
                return;
            }

            var wafer = foupManager.PeekWaferFromFoupA();
            wafer.CurrentStage = "TM Load";

            var task = new TransferController.TransferTask
            {
                Wafer = wafer,
                Pickup = EquipmentRegion.FoupA,
                Dropoff = EquipmentRegion.ChamberA,
                DestinationChamber = ChamberAState,
                FromFoup = true
            };

            ChamberAState.ReservedForIncoming = true;
            EnqueueTransfer(task);
            AddLogMessage($"TM 예약: {EquipmentRegionHelper.FormatRegionLabel(task.Pickup)} -> {EquipmentRegionHelper.FormatRegionLabel(task.Dropoff)} (웨이퍼 #{wafer.Id})", "INFO");
        }

        /// <summary>
        /// Chamber가 이송 준비 상태인지 확인 (공정 완료 판단)
        /// 중요: 실제 장비가 돌아가는 로직이므로 정확한 조건 체크 필수
        /// </summary>
        private bool IsReadyForTransfer(ChamberController.ChamberState chamber)
        {
            // 리팩토링 전 로직으로 복원: RemainingSeconds <= 0만 체크
            // StatusText 체크는 제거 (DecrementChamberTime에서 자동으로 설정됨)
            return chamber != null
                   && chamber.CurrentWafer != null
                   && chamber.RemainingSeconds <= 0
                   && !chamber.PickupScheduled;
        }

        private bool IsChamberAvailable(ChamberController.ChamberState chamber)
        {
            return chamber != null
                   && chamber.CurrentWafer == null
                   && !chamber.ReservedForIncoming;
        }

        private void ScheduleChamberTransfer(ChamberController.ChamberState source, ChamberController.ChamberState destination)
        {
            var task = new TransferController.TransferTask
            {
                Wafer = source.CurrentWafer,
                Pickup = source.Region,
                Dropoff = destination.Region,
                SourceChamber = source,
                DestinationChamber = destination
            };

            source.PickupScheduled = true;
            source.StatusText = "Transfer 대기";
            destination.ReservedForIncoming = true;
            EnqueueTransfer(task);
            AddLogMessage($"TM 예약: {EquipmentRegionHelper.FormatRegionLabel(task.Pickup)} -> {EquipmentRegionHelper.FormatRegionLabel(task.Dropoff)} (웨이퍼 #{task.Wafer.Id})", "INFO");
        }

        private void EnqueueTransfer(TransferController.TransferTask task)
        {
            TransferService?.EnqueueTransfer(task);
            if (TmPhase == TransferController.TmPhase.Idle && CurrentTransfer == null)
            {
                tmProcessor?.StartNextTransfer();
            }
        }

        /// <summary>
        /// 실패한 작업 재시도 처리
        /// 재시도 횟수가 최대값을 초과하면 작업을 제거하고 알림
        /// </summary>
        internal bool HandleFailedTransfer(TransferController.TransferTask task, string errorMessage)
        {
            if (task == null) return false;

            task.RetryCount++;
            
            if (task.RetryCount > TransferController.TransferTask.MaxRetryCount)
            {
                // 최대 재시도 횟수 초과 - 작업 제거
                AddLogMessage(
                    $"[작업 실패] {EquipmentRegionHelper.FormatRegionLabel(task.Pickup)} → {EquipmentRegionHelper.FormatRegionLabel(task.Dropoff)} " +
                    $"(웨이퍼 #{task.Wafer?.Id}) - 재시도 횟수 초과 ({task.RetryCount}/{TransferController.TransferTask.MaxRetryCount}) - 작업 제거",
                    "ERROR");
                
                // 웨이퍼 상태 복원 (원래 위치로)
                if (task.SourceChamber != null && task.Wafer != null)
                {
                    task.SourceChamber.CurrentWafer = task.Wafer;
                    AddLogMessage($"웨이퍼 #{task.Wafer.Id} 상태 복원: {EquipmentRegionHelper.FormatRegionLabel(task.SourceChamber.Region)}", "INFO");
                }
                
                return false; // 작업 제거됨
            }
            else
            {
                // 재시도 가능 - 큐에 다시 추가
                AddLogMessage(
                    $"[작업 재시도] {EquipmentRegionHelper.FormatRegionLabel(task.Pickup)} → {EquipmentRegionHelper.FormatRegionLabel(task.Dropoff)} " +
                    $"(웨이퍼 #{task.Wafer?.Id}) - 재시도 {task.RetryCount}/{TransferController.TransferTask.MaxRetryCount}",
                    "WARN");
                
                // 큐에 다시 추가 (재시도)
                TransferService?.EnqueueTransfer(task);
                return true; // 재시도 예정
            }
        }

        internal bool HasPendingPickupFromFoupA()
        {
            // 현재 진행 중인 작업이 FOUP에서 픽업하는지 확인
            if (CurrentTransfer != null && CurrentTransfer.FromFoup)
            {
                return true;
            }
            // 큐에 FOUP에서 픽업하는 작업이 있는지 확인
            if (TransferService != null)
            {
                foreach (var t in TransferService.GetQueuedTasks())
                {
                    if (t.FromFoup && t.Pickup == EquipmentRegion.FoupA) return true;
                }
            }
            return false;
        }

        private void InitializeCustomControls()
        {
            uiInitializer?.InitializeCustomControls();
        }

        private void InitializeFoupMountButtons()
        {
            uiInitializer?.InitializeFoupMountButtons();
        }

        private static string BoolToMountText(bool mounted) => mounted ? "장착" : "분리";

        internal int GetTmMoveDurationTicks(EquipmentRegion from, EquipmentRegion to)
        {
            if (from == to)
            {
                return 1;
            }

            bool fromFoup = from == EquipmentRegion.FoupA || from == EquipmentRegion.FoupB;
            bool toFoup = to == EquipmentRegion.FoupA || to == EquipmentRegion.FoupB;
            bool fromChamber = !fromFoup && from != EquipmentRegion.TM;
            bool toChamber = !toFoup && to != EquipmentRegion.TM;

            if ((fromFoup && toChamber) || (fromChamber && toFoup))
            {
                return AppSettings.TmMoveLongTicks;
            }

            if (fromChamber && toChamber)
            {
                return AppSettings.TmMoveMediumTicks;
            }

            return AppSettings.TmMoveShortTicks;
        }


        internal void ProcessTm()
        {
            tmProcessor?.ProcessTm();
        }

        /// <summary>
        /// Chamber 공정 시작 처리 (WaferTrackingService로 위임)
        /// 중요: 실제 장비가 돌아가는 로직이므로 정확한 상태 관리 필수
        /// </summary>
        internal void StartChamberProcessing(ChamberController.ChamberState chamber, Wafer wafer)
        {
            waferTrackingService?.StartChamberProcessing(chamber, wafer);
            
            // UI 업데이트
            UpdateChamberWaferIndicators();
        }

        /// <summary>
        /// TM Phase별 하드웨어 완료 조건 확인
        /// </summary>
        internal bool CheckTmPhaseHardwareComplete()
        {
            switch (TmPhase)
            {
                case TransferController.TmPhase.MoveToPickup_WaitHardware:
                case TransferController.TmPhase.MoveToDropoff_WaitHardware:
                    return CheckTmHardwareActionComplete();

                case TransferController.TmPhase.PickupExtend_CylinderForward:
                case TransferController.TmPhase.DropoffExtend_CylinderForward:
                    return CheckTmCylinderExtended();

                case TransferController.TmPhase.PickupRetract_CylinderBackward:
                case TransferController.TmPhase.DropoffRetract_CylinderBackward:
                    return CheckTmCylinderRetracted();

                case TransferController.TmPhase.PickupExtend_ServoDown:
                case TransferController.TmPhase.DropoffExtend_ServoDown:
                case TransferController.TmPhase.PickupRetract_ServoUp:
                case TransferController.TmPhase.DropoffRetract_ServoUp:
                    return CheckTmAxis1MoveComplete();

                // 도어 열기 대기 - 실제 도어 센서 확인
                case TransferController.TmPhase.WaitDoorPickupOpen:
                case TransferController.TmPhase.WaitDoorDropoffOpen:
                    return CheckDoorOpenForCurrentRegion();

                // 도어 닫기 대기 - 실제 도어 센서 확인
                case TransferController.TmPhase.WaitDoorPickupClose:
                case TransferController.TmPhase.WaitDoorDropoffClose:
                    return CheckDoorClosedForCurrentRegion();

                default:
                    return true;
            }
        }
        
        /// <summary>
        /// 현재 작업 영역의 도어가 열렸는지 확인 (하드웨어 센서)
        /// </summary>
        private bool CheckDoorOpenForCurrentRegion()
        {
            if (CurrentTransfer == null) return true;
            
            EquipmentRegion region;
            if (TmPhase == TransferController.TmPhase.WaitDoorPickupOpen)
                region = CurrentTransfer.Pickup;
            else if (TmPhase == TransferController.TmPhase.WaitDoorDropoffOpen)
                region = CurrentTransfer.Dropoff;
            else
                return true;
            
            // 중요: 도어에는 별도의 열림/닫힘 센서가 없음 (EtherTest 참고)
            // 도어 열기 명령 전송 후 시간 기반으로 처리
            return true; // 센서 없음 - 시간 기반으로 처리 (doorOpenConsecutiveChecks 사용)
        }
        
        /// <summary>
        /// 현재 작업 영역의 도어가 닫혔는지 확인 (하드웨어 센서)
        /// </summary>
        private bool CheckDoorClosedForCurrentRegion()
        {
            if (CurrentTransfer == null) return true;
            
            EquipmentRegion region;
            if (TmPhase == TransferController.TmPhase.WaitDoorPickupClose)
                region = CurrentTransfer.Pickup;
            else if (TmPhase == TransferController.TmPhase.WaitDoorDropoffClose)
                region = CurrentTransfer.Dropoff;
            else
                return true;
            
            // 중요: 도어에는 별도의 열림/닫힘 센서가 없음 (EtherTest 참고)
            // 도어 닫기 명령 전송 후 일정 시간이 지나면 닫힌 것으로 간주
            // 실제 장비에서 도어가 닫히는 데 약 2초 소요 추정
            return true; // 센서 없음 - 시간 기반으로 처리 (doorCloseWaitTicks 사용)
        }
        
        /// <summary>
        /// 도어 센서 상태 확인 (EtherCAT 하드웨어)
        /// </summary>
        private bool CheckDoorSensorOpen(EquipmentRegion region)
        {
            if (!EthercatConnected) return true;
            return ChamberHardwareHelper.CheckDoorSensorOpen(EtherCAT_M, region);
        }
        
        /// <summary>
        /// 도어 닫힘 센서 상태 확인 (EtherCAT 하드웨어)
        /// 닫힘 센서가 별도로 존재하는 경우 사용
        /// </summary>
        private bool CheckDoorSensorClosed(EquipmentRegion region)
        {
            if (!EthercatConnected) return true;
            return ChamberHardwareHelper.CheckDoorSensorClosed(EtherCAT_M, region);
        }

        /// <summary>
        /// 픽업 위치 이동 완료 처리
        /// </summary>
        internal void ProcessTmMoveToPickupComplete()
        {
            // 하드웨어 모드: 위치 확인 (PP_D는 이미 CheckTmHardwareActionComplete()에서 확인됨)
            // 중요: PP_D 중복 확인 제거 - CheckTmHardwareActionComplete()에서 타임아웃 포함하여 처리함
            if (IsTmHardwareModeAvailable())
            {
                try
                {
                    // 위치 로그만 출력 (검증 목적)
                    TmHardwareController.UpdateCurrentPositions();
                    long currentX = TmHardwareController.CurrentAxis2Position;
                    long currentY = TmHardwareController.CurrentAxis1Position;
                    
                    AddLogMessage($"TM 픽업 위치 도달 확인: X={currentX}, Y={currentY}", "INFO");
                }
                catch (Exception ex)
                {
                    AddLogMessage($"TM 위치 확인 오류: {ex.Message}", "ERROR");
                }
            }
            
            TmCurrentPosition = CurrentTransfer.Pickup;
            if (EquipmentRegionHelper.RequiresDoor(CurrentTransfer.Pickup))
            {
                // 하드웨어 모드: TM이 실제로 도착했을 때 도어 열기 명령 전송
                if (IsTmHardwareModeAvailable())
                {
                    // 중요: 도어 열기 명령을 명시적으로 전송 (센서 상태와 무관하게)
                    EnsureDoorOpenForRegion(CurrentTransfer.Pickup);
                    AddLogMessage($"{EquipmentRegionHelper.FormatRegionLabel(CurrentTransfer.Pickup)} 도어 열기 명령 전송 (TM 도착)", "INFO");
                    // 하드웨어 모드에서는 WaitDoorPickupOpen Phase에서 도어 열림 확인
                    tmProcessor?.BeginTmPhase(TransferController.TmPhase.WaitDoorPickupOpen, 0, CurrentTransfer.Pickup, false);
                    StartTmHardwareAction();
                }
                else
                {
                    // 시뮬레이션 모드: 즉시 도어 열기
                    EnsureDoorOpenForRegion(CurrentTransfer.Pickup);
                    tmProcessor?.BeginTmPhase(TransferController.TmPhase.WaitDoorPickupOpen, AppSettings.TmDoorActionTicks, CurrentTransfer.Pickup, false);
                }
            }
            else
            {
                if (IsTmHardwareModeAvailable())
                {
                    // 하드웨어 모드: 실린더 전진 시작
                    if (StartTmCylinderExtend())
                    {
                        tmProcessor?.BeginTmPhase(TransferController.TmPhase.PickupExtend_CylinderForward, 0, CurrentTransfer.Pickup, false);
                    }
                    else
                    {
                        AddLogMessage("TM 실린더 전진 실패 - 이송 중단", "ERROR");
                        TmHardwareActionPending = false;
                        TmSettleWaiting = false;
                        TransferService?.ResetToIdle();
                        // 큐에 작업이 있으면 다음 작업 시작 시도
                        if (TransferService?.QueueCount > 0)
                        {
                            tmProcessor?.StartNextTransfer();
                        }
                    }
                }
                else
                {
                    tmProcessor?.BeginTmPhase(TransferController.TmPhase.PickupExtend, AppSettings.TmPickupDurationTicks, CurrentTransfer.Pickup, false);
                }
            }
        }

        /// <summary>
        /// 픽업 후퇴 완료 처리
        /// </summary>
        internal void ProcessTmPickupRetractComplete()
        {
            if (EquipmentRegionHelper.RequiresDoor(CurrentTransfer.Pickup))
            {
                EnsureDoorClosedForRegion(CurrentTransfer.Pickup);
                tmProcessor?.BeginTmPhase(TransferController.TmPhase.WaitDoorPickupClose, AppSettings.TmDoorActionTicks, CurrentTransfer.Pickup, true);
                // 하드웨어 모드: 도어 닫힘 센서 대기
                if (IsTmHardwareModeAvailable())
                {
                    StartTmHardwareAction();
                }
            }
            else
            {
                var moveTicksDirect = GetTmMoveDurationTicks(CurrentTransfer.Pickup, CurrentTransfer.Dropoff);
                tmProcessor?.BeginTmPhase(TransferController.TmPhase.MoveToDropoff, moveTicksDirect, CurrentTransfer.Dropoff, true);
            }
        }

        /// <summary>
        /// 드롭오프 위치 이동 완료 처리
        /// </summary>
        internal void ProcessTmMoveToDropoffComplete()
        {
            // 하드웨어 모드: 위치 확인 (PP_D는 이미 CheckTmHardwareActionComplete()에서 확인됨)
            // 중요: PP_D 중복 확인 제거 - CheckTmHardwareActionComplete()에서 타임아웃 포함하여 처리함
            if (IsTmHardwareModeAvailable())
            {
                try
                {
                    // 위치 로그만 출력 (검증 목적)
                    TmHardwareController.UpdateCurrentPositions();
                    long currentX = TmHardwareController.CurrentAxis2Position;
                    long currentY = TmHardwareController.CurrentAxis1Position;
                    
                    AddLogMessage($"TM 드롭오프 위치 도달 확인: X={currentX}, Y={currentY}", "INFO");
                }
                catch (Exception ex)
                {
                    AddLogMessage($"TM 위치 확인 오류: {ex.Message}", "ERROR");
                }
            }
            
            TmCurrentPosition = CurrentTransfer.Dropoff;
            if (EquipmentRegionHelper.RequiresDoor(CurrentTransfer.Dropoff))
            {
                // 하드웨어 모드: TM이 실제로 도착했을 때 도어 열기 명령 전송
                if (IsTmHardwareModeAvailable())
                {
                    // 중요: 도어 열기 명령을 명시적으로 전송 (센서 상태와 무관하게)
                    EnsureDoorOpenForRegion(CurrentTransfer.Dropoff);
                    AddLogMessage($"{EquipmentRegionHelper.FormatRegionLabel(CurrentTransfer.Dropoff)} 도어 열기 명령 전송 (TM 도착)", "INFO");
                    // 하드웨어 모드에서는 WaitDoorDropoffOpen Phase에서 도어 열림 확인
                    tmProcessor?.BeginTmPhase(TransferController.TmPhase.WaitDoorDropoffOpen, 0, CurrentTransfer.Dropoff, true);
                    StartTmHardwareAction();
                }
                else
                {
                    // 시뮬레이션 모드: 즉시 도어 열기
                    EnsureDoorOpenForRegion(CurrentTransfer.Dropoff);
                    tmProcessor?.BeginTmPhase(TransferController.TmPhase.WaitDoorDropoffOpen, AppSettings.TmDoorActionTicks, CurrentTransfer.Dropoff, true);
                }
            }
            else
            {
                if (IsTmHardwareModeAvailable())
                {
                    if (StartTmCylinderExtend())
                    {
                        tmProcessor?.BeginTmPhase(TransferController.TmPhase.DropoffExtend_CylinderForward, 0, CurrentTransfer.Dropoff, true);
                    }
                    else
                    {
                        tmProcessor?.BeginTmPhase(TransferController.TmPhase.DropoffExtend, AppSettings.TmDropoffDurationTicks, CurrentTransfer.Dropoff, true);
                    }
                }
                else
                {
                    tmProcessor?.BeginTmPhase(TransferController.TmPhase.DropoffExtend, AppSettings.TmDropoffDurationTicks, CurrentTransfer.Dropoff, true);
                }
            }
        }

        /// <summary>
        /// 드롭오프 후퇴 완료 처리
        /// </summary>
        internal void ProcessTmDropoffRetractComplete()
        {
            if (EquipmentRegionHelper.RequiresDoor(CurrentTransfer.Dropoff))
            {
                EnsureDoorClosedForRegion(CurrentTransfer.Dropoff);
                tmProcessor?.BeginTmPhase(TransferController.TmPhase.WaitDoorDropoffClose, AppSettings.TmDoorActionTicks, CurrentTransfer.Dropoff, false);
                // 하드웨어 모드: 도어 닫힘 센서 대기
                if (IsTmHardwareModeAvailable())
                {
                    StartTmHardwareAction();
                }
            }
            else
            {
                TmCurrentPosition = CurrentTransfer.Dropoff;
                FinishCurrentTransfer();
            }
        }

        internal bool PerformPickup()
        {
            if (CurrentTransfer == null)
            {
                return false;
            }

            if (EquipmentRegionHelper.RequiresDoor(CurrentTransfer.Pickup) && !IsRegionDoorOpen(CurrentTransfer.Pickup))
            {
                EnsureDoorOpenForRegion(CurrentTransfer.Pickup);
                return false;
            }

            if (CurrentTransfer.SourceChamber != null)
            {
                var source = CurrentTransfer.SourceChamber;
                source.CurrentWafer = null;
                source.PickupScheduled = false;
                source.StatusText = "대기";
                source.TotalSeconds = 0;
                source.RemainingSeconds = 0;
                source.ProcessingAccumulator = 0;
            }
            else if (CurrentTransfer.FromFoup && foupManager?.HasWafersInFoupA() == true)
            {
                foupManager.DequeueWaferFromFoupA();
                // TM가 FOUP A에서 웨이퍼를 픽업하면 FOUP 실제 잔량 1 감소
                if (FoupARemainingInventoryCount > 0)
                {
                    FoupARemainingInventoryCount--;
                }
            }

            return true;
        }

        internal bool PerformDropoff()
        {
            if (CurrentTransfer == null)
            {
                return false;
            }

            if (EquipmentRegionHelper.RequiresDoor(CurrentTransfer.Dropoff) && !IsRegionDoorOpen(CurrentTransfer.Dropoff))
            {
                EnsureDoorOpenForRegion(CurrentTransfer.Dropoff);
                return false;
            }

            if (CurrentTransfer.SourceChamber == ChamberAState)
            {
                chamberCompletedCountA++;
            }
            else if (CurrentTransfer.SourceChamber == ChamberBState)
            {
                chamberCompletedCountB++;
            }
            else if (CurrentTransfer.SourceChamber == ChamberCState)
            {
                chamberCompletedCountC++;
            }

            if (CurrentTransfer.DestinationChamber != null)
            {
                var destination = CurrentTransfer.DestinationChamber;
                destination.ReservedForIncoming = false;
                destination.CurrentWafer = CurrentTransfer.Wafer;
                destination.TotalSeconds = destination.Step.DurationSeconds;
                destination.RemainingSeconds = destination.Step.DurationSeconds;
                destination.StatusText = "Door Closing";
                destination.ProcessingAccumulator = 0;
                UpdateChamberWaferIndicators();
                // 중요: 공정 시작은 도어가 닫힌 후에만 시작 (WaitDoorDropoffClose Phase에서 처리)
                // 시나리오: T=60초 웨이퍼 투입 → T=63~65초 도어 닫기 → 공정 시작
            }
            else if (CurrentTransfer.OnCompleted != null)
            {
                CurrentTransfer.OnCompleted(CurrentTransfer.Wafer);
            }

            AddLogMessage($"TM 완료: {EquipmentRegionHelper.FormatRegionLabel(CurrentTransfer.Pickup)} -> {EquipmentRegionHelper.FormatRegionLabel(CurrentTransfer.Dropoff)} (웨이퍼 #{CurrentTransfer.Wafer.Id})", "INFO");
            TmCarryingVisual = false;
            return true;
        }

        internal void FinishCurrentTransfer()
        {
            TransferService?.ResetToIdle();
            if (TransferService?.QueueCount > 0)
            {
                tmProcessor?.StartNextTransfer();
            }
            else
            {
                UpdateTmAnimationIdleTarget();
            }
        }

        private bool IsChamberDoorClosed(ChamberController.ChamberState chamber)
        {
            if (chamber == null)
            {
                return true;
            }

            var region = chamber.Region;
            if (!EquipmentRegionHelper.RequiresDoor(region))
            {
                return true;
            }

            return !IsRegionDoorOpen(region);
        }

        internal void UpdateTmAnimationIdleTarget()
        {
            uiUpdater?.UpdateTmAnimationIdleTarget();
        }

        private void UpdateDoorLamp(Panel lampPanel, bool doorClosed, EquipmentRegion? region = null)
        {
            uiUpdater?.UpdateDoorLamp(lampPanel, doorClosed, region);
        }

        // Chamber 램프를 공정 상태에 따라 업데이트
        // 공정 중: 켜짐, 공정 완료(웨이퍼 있음): 깜빡임, 웨이퍼 없음: 꺼짐
        private void UpdateChamberProcessLamps()
        {
            uiUpdater?.UpdateChamberProcessLamps();
        }

        // Chamber가 공정 중인지 확인 (램프 ON 조건)
        // 조건: 웨이퍼 있음 + 공정 시간 설정됨 + 시간 남음 + 도어 닫힘 + 실제 공정 진행 상태
        internal bool IsChamberProcessing(ChamberController.ChamberState chamber)
        {
            if (chamber == null) return false;
            
            // 웨이퍼가 있고, 공정 시간이 남아있고, 도어가 닫혀있고, 실제 공정 진행 중이면 램프 ON
            bool hasWafer = chamber.CurrentWafer != null;
            bool hasProcessTime = chamber.TotalSeconds > 0;
            bool timeRemaining = chamber.RemainingSeconds > 0;  // 프로그레스바가 움직이는 조건
            bool doorClosed = !IsRegionDoorOpen(chamber.Region);
            
            // 실제 공정 진행 중 확인 (StatusText가 "처리 중" 또는 "2차 노광 중")
            // "Door Open 대기", "Door Closing", "TM 대기" 등의 상태에서는 false
            bool isActuallyProcessing = !string.IsNullOrEmpty(chamber.StatusText) &&
                (chamber.StatusText.Contains("처리 중") || 
                 chamber.StatusText.Contains("2차 노광 중") ||
                 chamber.StatusText.Contains("Processing"));
            
            return hasWafer && hasProcessTime && timeRemaining && doorClosed && isActuallyProcessing;
        }

        // Chamber가 공정 완료 상태인지 확인 (웨이퍼는 있지만 공정 완료)
        // 조건: 웨이퍼 있음 + 공정 시간 설정됨 + 시간 없음 + 도어 닫힘
        internal bool IsChamberCompleted(ChamberController.ChamberState chamber)
        {
            if (chamber == null) return false;
            
            bool hasWafer = chamber.CurrentWafer != null;
            bool hasProcessTime = chamber.TotalSeconds > 0;
            bool timeRemaining = chamber.RemainingSeconds <= 0;  // 공정 시간 없음
            bool doorClosed = !IsRegionDoorOpen(chamber.Region);
            
            return hasWafer && hasProcessTime && timeRemaining && doorClosed;
        }

        // 램프 UI만 업데이트 (색상 및 깜빡임)
        private void UpdateChamberLampUI(Panel lampPanel, bool isProcessing, bool isCompleted)
        {
            uiUpdater?.UpdateChamberLampUI(lampPanel, isProcessing, isCompleted);
        }

        // Chamber 램프 제어 (실제 장비)
        // 실제 장비 상태를 읽어서 UI에 반영
        internal void SyncEquipmentStateToUI()
        {
            if (!EthercatConnected || EtherCAT_M == null)
            {
                return;
            }

            isSyncingEquipmentState = true; // 동기화 시작
                try
                {
                    // ReadData_Timer_Start()로 주기적으로 읽고 있으므로,
                    // 연결 직후 약간의 지연을 두고 상태를 읽음
                    System.Threading.Thread.Sleep(200); // 200ms 대기 (ReadData가 한 번 실행될 시간 확보)
                    
                // Chamber A, B, C에 대한 도어 및 램프 상태 매핑
                // (Region, 도어 Digital_Input 인덱스, 램프 Digital_Input 인덱스)
                var chamberMappings = new[]
                {
                    (EquipmentRegion.ChamberA, 5, 3),
                    (EquipmentRegion.ChamberB, 8, 6),
                    (EquipmentRegion.ChamberC, 11, 9)
                };

                // 실제 장비의 도어 상태 읽기 및 UI 반영
                foreach (var (region, doorInputIndex, lampInputIndex) in chamberMappings)
                {
                    bool doorOpen = false;
                    bool lampOn = false;
                    
                    try
                    {
                    // IEG3268_Dll의 Digital_Input 메서드를 사용하여 실제 장비 상태 읽기
                        doorOpen = EtherCAT_M.Digital_Input(doorInputIndex);
                        lampOn = EtherCAT_M.Digital_Input(lampInputIndex);
                }
                catch (Exception ex)
                {
                        AddLogMessage($"장비 상태 읽기 오류 ({region}): {ex.Message}", "WARN");
                        continue; // 이 Chamber는 스킵하고 다음으로
                }

                // 읽은 도어 상태를 UI에 반영 (장비 제어는 하지 않고 UI만 업데이트)
                    ViewModel?.SetDoorState(region, doorOpen);
                
                // 동기화 중이므로 장비 제어 없이 UI만 업데이트
                // ApplyDoorVisualsForRegion 대신 직접 UI 업데이트
                    UpdateDoorIndicatorUIOnly(region, !doorOpen);

                // 읽은 램프 상태를 UI에 반영 (장비 제어는 하지 않고 UI만 업데이트)
                    SetChamberLampUIOnly(region, lampOn);
                }

                // 실제 장비의 3색 램프 상태 읽기 및 UI 반영
                bool mainLampRed = false;
                bool mainLampYellow = false;
                bool mainLampGreen = false;
                try
                {
                    // IEG3268_Dll의 Digital_Input 메서드를 사용하여 실제 장비 상태 읽기
                    // 3색 램프 상태: Digital Input 0(적색), 1(황색), 2(녹색)가 true면 켜짐
                    mainLampRed = EtherCAT_M.Digital_Input(0);
                    mainLampYellow = EtherCAT_M.Digital_Input(1);
                    mainLampGreen = EtherCAT_M.Digital_Input(2);
                }
                catch (Exception ex)
                {
                    AddLogMessage($"장비 상태 읽기 오류 (3색 램프): {ex.Message}", "WARN");
                }

                // 읽은 3색 램프 상태를 UI에 반영 (장비 제어는 하지 않고 UI만 업데이트)
                UpdateMainLampColorsUIOnly(mainLampGreen ? Color.ForestGreen : (Color?)null, mainLampYellow, mainLampRed);
                
                // 서보 상태 동기화 (하드웨어가 OFF인데 UI가 ON인 경우만 동기화)
                // 주의: 하드웨어가 ON인데 UI가 OFF인 경우는 자동 동기화하지 않음
                // (사용자가 명시적으로 서보 ON 버튼을 눌러야 함)
                try
                {
                    // 서보 ON 상태 확인 (위치 데이터 확인)
                    string axis1Pos = EtherCAT_M.Axis1_is_PosData();
                    string axis2Pos = EtherCAT_M.Axis2_is_PosData();
                    bool hasPosition = !string.IsNullOrEmpty(axis1Pos) && axis1Pos != "-" &&
                                      !string.IsNullOrEmpty(axis2Pos) && axis2Pos != "-";
                    
                    // 하드웨어가 OFF인데 UI 상태가 ON인 경우만 동기화 (안전을 위해)
                    if (!hasPosition && IsServoOn)
                    {
                        // 하드웨어는 OFF인데 UI 상태가 ON인 경우 동기화
                        IsServoOn = false;
                        TmHardwareInitialized = false;
                        AddLogMessage("서보 상태 동기화: 하드웨어는 OFF 상태입니다", "WARN");
                    }
                    // 하드웨어가 ON인데 UI가 OFF인 경우는 자동 동기화하지 않음
                    // (사용자가 서보 ON 버튼을 명시적으로 눌러야 함)
                    
                    // 원점복귀 상태 동기화
                    // 공정 진행 중에는 서보가 이동하면서 HOME_D가 false가 될 수 있으므로,
                    // 공정 진행 중에는 TmHardwareInitialized를 false로 설정하지 않음
                    bool axis1Homed = EtherCAT_M.Axis1_Status("HOME_D");
                    bool axis2Homed = EtherCAT_M.Axis2_Status("HOME_D");
                    bool isHomed = axis1Homed && axis2Homed;
                    bool isProcessRunning = CurrentProcessState == ProcessState.Running || TmPhase != TransferController.TmPhase.Idle;
                    
                    if (isHomed && !TmHardwareInitialized)
                    {
                        // 하드웨어는 원점복귀 완료인데 UI 상태가 미완료인 경우 동기화
                        TmHardwareInitialized = true;
                        AddLogMessage("원점복귀 상태 동기화: 완료 상태로 업데이트", "INFO");
                    }
                    else if (!isHomed && TmHardwareInitialized && !IsServoOn && !isProcessRunning)
                    {
                        // 서보가 OFF이고 공정이 진행 중이 아닌 경우에만 원점복귀 상태 초기화
                        // 공정 진행 중에는 서보가 이동하면서 HOME_D가 false가 될 수 있으므로 초기화하지 않음
                        TmHardwareInitialized = false;
                    }
                    
                    // 서보 상태 라벨 업데이트
                    UpdateServoStatusLabel();
                }
                catch (Exception servoEx)
                {
                    AddLogMessage($"서보 상태 동기화 오류: {servoEx.Message}", "WARN");
                }

                AddLogMessage("장비 상태를 UI에 동기화했습니다.", "INFO");
            }
            catch (Exception ex)
            {
                AddLogMessage($"장비 상태 동기화 오류: {ex.Message}", "ERROR");
            }
            finally
            {
                isSyncingEquipmentState = false; // 동기화 완료
            }
        }

        // UI만 업데이트 (장비 제어 없음)
        internal void SetChamberLampUIOnly(EquipmentRegion region, bool on)
        {
            Panel lampPanel = null;
            switch (region)
            {
                case EquipmentRegion.ChamberA:
                    lampPanel = panelLampChamberA;
                    break;
                case EquipmentRegion.ChamberB:
                    lampPanel = panelLampChamberB;
                    break;
                case EquipmentRegion.ChamberC:
                    lampPanel = panelLampChamberC;
                    break;
            }

            if (lampPanel != null)
            {
                SetLamp(lampPanel, on, Color.Yellow);
            }
        }

        // UI만 업데이트 (장비 제어 없음)
        internal void UpdateMainLampColorsUIOnly(Color? greenActive, bool yellowActive, bool redActive)
        {
            uiUpdater?.UpdateMainLampColorsUIOnly(greenActive, yellowActive, redActive);
        }

        // UI만 업데이트 (장비 제어 없음) - 도어 인디케이터
        private void UpdateDoorIndicatorUIOnly(EquipmentRegion region, bool doorClosed)
        {
            uiUpdater?.UpdateDoorIndicatorUIOnly(region, doorClosed);
        }

        internal void SetChamberLamp(EquipmentRegion region, bool on)
        {
            if (!EthercatConnected)
            {
                return;
            }

            try
            {
                if (!ChamberHardwareHelper.ControlChamberLamp(EtherCAT_M, region, on))
                {
                    AddLogMessage($"{EquipmentRegionHelper.FormatRegionLabel(region)} 램프 제어 실패", "ERROR");
                }
            }
            catch (Exception ex)
            {
                AddLogMessage($"{EquipmentRegionHelper.FormatRegionLabel(region)} 램프 제어 오류: {ex.Message}", "ERROR");
            }
        }

        private void UpdateDoorIndicator(Panel lampPanel, Label doorLabel, Panel doorPanel, bool doorClosed, EquipmentRegion? region = null)
        {
            uiUpdater?.UpdateDoorIndicator(lampPanel, doorLabel, doorPanel, doorClosed, region);
        }

        internal bool IsRegionDoorOpen(EquipmentRegion region)
        {
            return ViewModel?.IsDoorOpen(region) ?? false;
        }

        // RequiresDoor은 EquipmentRegionHelper로 이동됨

        internal void ResetDoorStates()
        {
            uiConfigurator?.ResetDoorStates();
        }

        internal void RefreshAllDoorVisuals()
        {
            uiConfigurator?.RefreshAllDoorVisuals();
        }

        internal void ApplyDoorVisualsForRegion(EquipmentRegion region, bool animate)
        {
            if (!EquipmentRegionHelper.RequiresDoor(region))
            {
                return;
            }

            var visuals = GetDoorVisualTargets(region);
            if (visuals.DoorPanel == null && visuals.LampPanel == null)
            {
                return;
            }

            var doorClosed = !IsRegionDoorOpen(region);
            UpdateDoorIndicator(visuals.LampPanel, null, visuals.DoorPanel, doorClosed, region);
            if (visuals.DoorPanel != null)
            {
                if (animate)
                {
                    DoorAnimationHelper.AnimateDoor(visuals.DoorPanel, open: !doorClosed);
                }
                else
                {
                    DoorAnimationHelper.ApplyImmediateState(visuals.DoorPanel, open: !doorClosed);
                }
            }
        }

        internal void SetDoorState(EquipmentRegion region, bool open)
        {
            if (!EquipmentRegionHelper.RequiresDoor(region))
            {
                return;
            }

            // ViewModel의 현재 상태 확인
            if (ViewModel != null && ViewModel.IsDoorOpen(region) == open)
            {
                return;
            }

            // 실제 장비 제어 (EtherCAT 연결 시)
            if (EthercatConnected)
            {
                try
                {
                    if (!ChamberHardwareHelper.ControlChamberDoor(EtherCAT_M, region, open))
                    {
                        AddLogMessage($"{EquipmentRegionHelper.FormatRegionLabel(region)} 도어 제어 실패", "ERROR");
                    }
                }
                catch (Exception ex)
                {
                    AddLogMessage($"{EquipmentRegionHelper.FormatRegionLabel(region)} 도어 제어 오류: {ex.Message}", "ERROR");
                }
            }

            // ViewModel 상태 업데이트
            ViewModel?.SetDoorState(region, open);
            ApplyDoorVisualsForRegion(region, animate: true);
        }

        internal void EnsureDoorOpenForRegion(EquipmentRegion region)
        {
            if (!EquipmentRegionHelper.RequiresDoor(region))
            {
                return;
            }

            SetDoorState(region, true);
        }

        internal void EnsureDoorClosedForRegion(EquipmentRegion region)
        {
            if (!EquipmentRegionHelper.RequiresDoor(region))
            {
                return;
            }

            SetDoorState(region, false);
        }

        internal (Panel LampPanel, Panel DoorPanel) GetDoorVisualTargets(EquipmentRegion region)
        {
            switch (region)
            {
                case EquipmentRegion.ChamberA:
                    return (panelLampChamberA, panelDoorChamberA);
                case EquipmentRegion.ChamberB:
                    return (panelLampChamberB, panelDoorChamberB);
                case EquipmentRegion.ChamberC:
                    return (panelLampChamberC, panelDoorChamberC);
                default:
                    return (null, null);
            }
        }

        private void UpdateChamberWaferIndicators()
        {
            uiUpdater?.UpdateChamberWaferIndicators();
        }

        private void InitializeWaferOverlays()
        {
            uiInitializer?.InitializeWaferOverlays();
        }

        // 웨이퍼 트래킹 메서드 (WaferTrackingService로 위임)
        internal bool IsWaferEnRouteToChamber(ChamberController.ChamberState chamber)
        {
            return waferTrackingService?.IsWaferEnRouteToChamber(chamber) ?? false;
        }

        internal bool IsWaferAwaitingPickup(ChamberController.ChamberState chamber)
        {
            return waferTrackingService?.IsWaferAwaitingPickup(chamber) ?? false;
        }

        private bool NeedsSecondExposure(Wafer wafer)
        {
            return waferTrackingService?.NeedsSecondExposure(wafer) ?? false;
        }

        private void MarkSecondExposureComplete(Wafer wafer)
        {
            waferTrackingService?.MarkSecondExposureComplete(wafer);
        }

        // 웨이퍼 색상 결정 (WaferTrackingService로 위임)
        internal Color GetWaferColorForState(ChamberController.ChamberState chamber)
        {
            return waferTrackingService?.GetWaferColorForState(chamber) ?? Color.Transparent;
        }

        private void UpdateChamberWaferIndicator(ChamberController.ChamberState chamber, Panel waferPanel)
        {
            uiUpdater?.UpdateChamberWaferIndicator(chamber, waferPanel);
        }

        private void WaferPanel_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel panel && panel.Visible)
            {
                var color = panel.Tag is Color c ? c : Color.FromArgb(200, 220, 255);
                var rect = panel.ClientRectangle;
                rect.Inflate(-3, -3);
                using (var brush = new SolidBrush(color))
                using (var pen = new Pen(Color.FromArgb(180, 80, 80, 80), 1.2f))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillEllipse(brush, rect);
                    e.Graphics.DrawEllipse(pen, rect);
                }
            }
        }

        internal void UpdateWaferPanelRegion(Panel panel)
        {
            uiUpdater?.UpdateWaferPanelRegion(panel);
        }

        /// <summary>
        /// 중앙 UI (EquipmentDiagramControl)의 램프 상태 업데이트
        /// </summary>
        internal void UpdateEquipmentDiagramLampState(bool redActive, bool yellowActive, bool greenActive)
        {
            if (panelEquipmentCanvas == null)
            {
                return;
            }

            // panelEquipmentCanvas의 자식 컨트롤 중 EquipmentDiagramControl 찾기
            foreach (Control control in panelEquipmentCanvas.Controls)
            {
                if (control is Controls.EquipmentDiagramControl diagramControl)
                {
                    diagramControl.UpdateMainLampState(redActive, yellowActive, greenActive);
                    return;
                }
            }
        }

        internal static class DoorAnimationHelper
        {
            private sealed class DoorAnimationState
            {
                public int ClosedHeight;
                public int TargetHeight;
                public Timer Timer;
            }

            private static readonly Dictionary<Panel, DoorAnimationState> PanelStates = new Dictionary<Panel, DoorAnimationState>();

            public static void AnimateDoor(Panel panel, bool open)
            {
                var state = GetOrCreateState(panel);
                if (state == null)
                {
                    return;
                }

                state.TargetHeight = open ? Math.Max(6, state.ClosedHeight / 4) : state.ClosedHeight;
                if (!state.Timer.Enabled)
                {
                    state.Timer.Start();
                }
            }

            public static void ApplyImmediateState(Panel panel, bool open)
            {
                var state = GetOrCreateState(panel);
                if (state == null)
                {
                    return;
                }

                state.TargetHeight = open ? Math.Max(6, state.ClosedHeight / 4) : state.ClosedHeight;
                panel.Height = state.TargetHeight;
                if (state.Timer.Enabled)
                {
                    state.Timer.Stop();
                }
            }

            private static DoorAnimationState GetOrCreateState(Panel panel)
            {
                if (panel == null || panel.IsDisposed)
                {
                    return null;
                }

                if (!PanelStates.TryGetValue(panel, out var state))
                {
                    state = new DoorAnimationState
                    {
                        ClosedHeight = Math.Max(8, panel.Height),
                        Timer = new Timer { Interval = 16 }
                    };
                    state.Timer.Tick += (_, __) => OnTick(panel);
                    PanelStates[panel] = state;
                }

                if (state.ClosedHeight <= 0)
                {
                    state.ClosedHeight = Math.Max(8, panel.Height);
                }

                return state;
            }

            private static void OnTick(Panel panel)
            {
                if (panel == null || panel.IsDisposed)
                {
                    StopTimer(panel);
                    return;
                }

                if (!PanelStates.TryGetValue(panel, out var state))
                {
                    return;
                }

                var current = panel.Height;
                if (Math.Abs(current - state.TargetHeight) <= 1)
                {
                    panel.Height = state.TargetHeight;
                    StopTimer(panel);
                    return;
                }

                var delta = Math.Max(1, Math.Abs(state.TargetHeight - current) / 4);
                panel.Height += state.TargetHeight > current ? delta : -delta;
            }

            private static void StopTimer(Panel panel)
            {
                if (PanelStates.TryGetValue(panel, out var state) && state?.Timer != null)
                {
                    state.Timer.Stop();
                }
            }

            public static void CleanupAllTimers()
            {
                foreach (var kvp in PanelStates.ToList())
                {
                    if (kvp.Value?.Timer != null)
                    {
                        kvp.Value.Timer.Stop();
                        kvp.Value.Timer.Dispose();
                    }
                }
                PanelStates.Clear();
            }
        }

        internal void UpdateTmVisualization()
        {
            uiUpdater?.UpdateTmVisualization();
        }

        /// <summary>
        /// 웨이퍼가 블레이드에 있는지 확인 (실제 위치 기반)
        /// FOUP/Chamber에서 웨이퍼가 사라지는 순간(픽업 시작)부터 목적지에 안착하기 전까지
        /// </summary>
        // 웨이퍼 블레이드 위치 확인 (WaferTrackingService로 위임)
        internal bool IsWaferOnBlade()
        {
            return waferTrackingService?.IsWaferOnBlade() ?? false;
        }

        // Region에 따른 웨이퍼 색상 (WaferTrackingService로 위임)
        internal Color GetWaferColorForRegion(EquipmentRegion region)
        {
            return waferTrackingService?.GetWaferColorForRegion(region) ?? Color.FromArgb(210, 230, 255);
        }

        internal void UpdateFoupPreparationButtons()
        {
            uiUpdater?.UpdateFoupPreparationButtons();
        }

        internal void GetFoupDisplayCounts(out int displayFoupACount, out int displayFoupBCount)
        {
            // FoupManager로 위임
            if (foupManager != null)
            {
                foupManager.GetFoupDisplayCounts(out displayFoupACount, out displayFoupBCount);
            }
            else
            {
                displayFoupACount = 0;
                displayFoupBCount = 0;
            }
        }

        private void UpdateCentralFoupVisualState()
        {
            uiUpdater?.UpdateCentralFoupVisualState();
        }

        internal (string Descriptor, Color? Accent) GetFoupAPanelVisualState(int displayWaferCount)
        {
            if (!IsFoupAMounted)
            {
                return ("미장착", AppSettings.FoupPanelAlertColor);
            }

            switch (WaferLoadState)
            {
                case MainFormViewModel.WaferLoadStateType.Loading:
                    return ($"웨이퍼 로딩 · {displayWaferCount}장", AppSettings.FoupPanelLoadingColor);
                case MainFormViewModel.WaferLoadStateType.Unloading:
                    return ($"웨이퍼 언로딩 · {displayWaferCount}장", AppSettings.FoupPanelUnloadingColor);
                default:
                    var descriptor = SimulationRunning
                        ? $"처리 중 · 잔여 {displayWaferCount}장"
                        : $"대기 · {displayWaferCount}장";
                    return (descriptor, null);
            }
        }

        internal (string Descriptor, Color? Accent) GetFoupBPanelVisualState(int displayWaferCount)
        {
            if (!IsFoupBMounted)
            {
                return ("미장착", AppSettings.FoupPanelAlertColor);
            }

            if (WaferLoadState == MainFormViewModel.WaferLoadStateType.Unloading)
            {
                return ($"언로딩 수신 · {displayWaferCount}장", AppSettings.FoupPanelUnloadingColor);
            }

            if (displayWaferCount > 0)
            {
                return ($"완료 적재 중 · {displayWaferCount}장", AppSettings.FoupPanelLoadingColor);
            }

            return ("대기 · 0장", null);
        }

        internal void ApplyFoupPanelVisual(Panel panel, Label titleLabel, string descriptor, Color? accentColor)
        {
            if (panel != null)
            {
                panel.BackColor = accentColor ?? GetOriginalPanelColor(panel);
            }

            if (titleLabel != null)
            {
                var baseText = GetLabelBaseText(titleLabel);
                titleLabel.Text = string.IsNullOrWhiteSpace(descriptor)
                    ? baseText
                    : $"{baseText} · {descriptor}";
            }
        }

        internal string GetWaferLoadStateDisplay()
        {
            if (!IsFoupAMounted || !IsFoupBMounted)
            {
                return "FOUP A/B 미장착";
            }

            switch (WaferLoadState)
            {
                case MainFormViewModel.WaferLoadStateType.Loading:
                    return "웨이퍼 로딩 준비";
                case MainFormViewModel.WaferLoadStateType.Unloading:
                    return "웨이퍼 언로딩 준비";
                default:
                    return "웨이퍼 준비 대기";
            }
        }

        private static void ApplyPreparationButtonStyle(Button button, bool active, string activeText, string inactiveText)
        {
            if (button == null)
            {
                return;
            }

            button.Text = active ? activeText : inactiveText;
            button.BackColor = active ? Color.FromArgb(100, 120, 130) : Color.FromArgb(100, 120, 130);
            button.ForeColor = Color.White;
        }

        internal void UpdateSimulationUi()
        {
            uiUpdater?.UpdateSimulationUi();
        }


        internal PmDetailData BuildPmDetail(ChamberController.ChamberState chamber, int completedCount, int totalCount)
        {
            if (chamber == null)
            {
                return new PmDetailData
                {
                    UnitKey = "Unknown",
                    StatusText = "Idle",
                    RecipeName = "N/A",
                    StepName = "N/A",
                    RecipeTimeCurrent = 0,
                    RecipeTimeTotal = 0,
                    StepTimeCurrent = 0,
                    StepTimeTotal = 0,
                    StepIndex = 0,
                    StepCount = totalCount,
                    StepMessage = "데이터 없음",
                    Progress = 0
                };
            }

            var detail = new PmDetailData
            {
                UnitKey = chamber.UnitKey,
                StatusText = chamber.StatusText,
                RecipeName = chamber.Step.DisplayName,
                StepName = chamber.Step.DisplayName,
                RecipeTimeTotal = chamber.Step.DurationSeconds * Math.Max(1, totalCount),
                RecipeTimeCurrent = completedCount * chamber.Step.DurationSeconds,
                StepTimeTotal = chamber.Step.DurationSeconds,
                StepTimeCurrent = chamber.CurrentWafer != null
                    ? chamber.Step.DurationSeconds - chamber.RemainingSeconds
                    : 0,
                StepIndex = completedCount + (chamber.CurrentWafer != null ? 1 : 0),
                StepCount = Math.Max(1, totalCount),
            };

            if (chamber.CurrentWafer != null)
            {
                detail.RecipeTimeCurrent += detail.StepTimeCurrent;
            }

            if (detail.RecipeTimeTotal > 0)
            {
                detail.Progress = Math.Max(0, Math.Min(100,
                    (int)Math.Round(detail.RecipeTimeCurrent * 100.0 / detail.RecipeTimeTotal)));
            }
            else
            {
                detail.Progress = 0;
            }

            if (chamber.CurrentWafer != null && detail.StepTimeTotal > 0)
            {
                detail.StepProgress = Math.Max(0, Math.Min(100,
                    (int)Math.Round(detail.StepTimeCurrent * 100.0 / detail.StepTimeTotal)));
                detail.ActiveWaferText = $"Wafer #{chamber.CurrentWafer.Id} : {detail.StepProgress}% ({detail.StepTimeCurrent}/{detail.StepTimeTotal}s)";
                if (chamber.RemainingSeconds <= 0 && chamber.PickupScheduled)
                {
                    detail.StepMessage = $"TM 픽업 대기 (Wafer #{chamber.CurrentWafer.Id})";
                }
                else
                {
                    detail.StepMessage = $"공정 진행 중 (#{chamber.CurrentWafer.Id})";
                }
            }
            else if (chamber.ReservedForIncoming)
            {
                detail.StepProgress = 0;
                detail.ActiveWaferText = "웨이퍼 없음";
                detail.StepMessage = "TM 반입 대기";
            }
            else
            {
                detail.StepProgress = 0;
                detail.ActiveWaferText = "웨이퍼 없음";
                detail.StepMessage = chamber == ChamberAState && foupManager?.HasWafersInFoupA() == true
                    ? "웨이퍼 투입 대기"
                    : "유닛 대기";
            }

            return detail;
        }

        // FormatRegionLabel은 EquipmentRegionHelper로 이동됨

        internal void UpdateMainLampColors(Color? greenActive, bool yellowActive, bool redActive)
        {
            uiUpdater?.UpdateMainLampColors(greenActive, yellowActive, redActive);
        }

        internal void SetLamp(Panel panel, bool active, Color activeColor)
        {
            if (panel == null) return;
            panel.BackColor = active ? activeColor : Color.FromArgb(60, 60, 60);
        }

        private void SetDoorPanelState(Panel panel, string statusText)
        {
            if (panel == null)
            {
                return;
            }

            panel.BackColor = GetStatusColor(statusText);
        }

        private void UpdateProcessControlButtons()
        {
            uiUpdater?.UpdateProcessControlButtons();
        }

        internal bool HasHeaderAlarm()
        {
            // 헤더 알람 영역에 알람이 표시되어 있는지 확인
            if (labelHeaderEventMessage == null)
        {
                return false;
            }

            // "알람 없음" 또는 "메시지 없음"이 아니면 알람이 표시된 것으로 간주
            string message = labelHeaderEventMessage.Text ?? "";
            if (string.IsNullOrWhiteSpace(message) || 
                message == "알람 없음" || 
                message == "메시지 없음")
            {
                return false;
            }

            // 레벨이 ALARM, ERROR, CRITICAL, WARN 등이면 알람으로 간주
            if (labelHeaderEventLevel != null)
            {
                string level = labelHeaderEventLevel.Text ?? "";
                if (!string.IsNullOrWhiteSpace(level))
                {
                    string upperLevel = level.ToUpperInvariant();
                    if (upperLevel == "ALARM" || upperLevel == "ERROR" || 
                        upperLevel == "CRITICAL" || upperLevel == "WARN" || 
                        upperLevel == "WARNING")
                    {
                        return true;
            }
        }
            }

            // 메시지가 있고 레벨이 없거나 INFO가 아닌 경우도 알람으로 간주
            return true;
        }


        public enum AppSection
        {
            Main,
            Verification,
            Transfer,
            Operate,
            Recipe,
            Maintenance,
            Config,
            Trend,
            Report,
            SysInfo
        }

        private AppSection currentSection = AppSection.Main;

        internal void NavigateToSection(AppSection section)
        {
            currentSection = section;
            string name = "";
            switch (section)
            {
                case AppSection.Main: name = "Main"; break;
                case AppSection.Verification: name = "Verification"; break;
                case AppSection.Transfer: name = "Transfer"; break;
                case AppSection.Operate: name = "Operate"; break;
                case AppSection.Recipe: name = "Recipe"; break;
                case AppSection.Maintenance: name = "Maint"; break;
                case AppSection.Config: name = "Config"; break;
                case AppSection.Trend: name = "Trend"; break;
                case AppSection.Report: name = "Report"; break;
                case AppSection.SysInfo: name = "SysInfo"; break;
            }

            // 섹션 진입 훅: 반도체 장비 UI에 맞춘 기본 로직
            switch (section)
            {
                case AppSection.Verification:
                    RunVerificationChecks();
                    // 폼은 버튼 클릭 이벤트에서 열도록 변경 (중복 방지)
                    break;
                case AppSection.Operate:
                    // 운전(Operate) 화면 진입 시 인터락 미통과면 경고
                    string opReasons;
                    if (!EvaluateInterlocks(out opReasons))
                    {
                        AddLogMessage($"운전 화면 진입: 인터락 미통과 - {opReasons}", "WARN");
                    }
                    break;
                case AppSection.Transfer:
                    AutoPlanTransferIfNeeded();
                    // 폼은 버튼 클릭 이벤트에서 열도록 변경 (중복 방지)
                    break;
                case AppSection.Trend:
                    StartTrendCapture();
                    // Trend 폼 열기
                    try
                    {
                        using (var tf = new TrendForm())
                        {
                            tf.ShowDialog(this);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogMessage($"Trend 폼 오류: {ex.Message}", "ERROR");
                    }
                    break;
                case AppSection.Maintenance:
                    // Maintenance 폼 열기
                    try
                    {
                        using (var mf = new MaintenanceForm())
                        {
                            mf.ShowDialog(this);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogMessage($"Maintenance 폼 오류: {ex.Message}", "ERROR");
                    }
                    break;
                case AppSection.Report:
                    // Report 폼 열기 (콜백 설정 포함)
                    try
                    {
                        using (var rf = new ReportForm())
                        {
                            // 로그 엔트리 제공 콜백 설정
                            rf.ProvideLogEntries = () => LoggerService?.GetLogEntries() ?? new List<string>();
                            // CSV 내보내기 콜백 설정
                            rf.OnExportCsv = () => ExportLogsToCsv();
                            rf.ShowDialog(this);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogMessage($"Report 폼 오류: {ex.Message}", "ERROR");
                    }
                    break;
                case AppSection.Config:
                    // Config 폼 열기 (NavigateToSection에서만 열도록 변경)
                    // NavigationEventHandlers에서도 열 수 있지만, 여기서만 열도록 함
                    if (!EnsureLoggedIn())
                    {
                        break;
                    }

                    if (CurrentRole != "관리자")
                    {
                        MessageBox.Show("설정 변경은 관리자만 접근할 수 있습니다.", "권한 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }

                    // 현재 알람 임계값을 EnvThresholdSnapshot으로 변환
                    var currentSnapshot = GetAlarmThresholdsSnapshot();
                    
                    try
                    {
                        using (var cf = new ConfigForm(currentSnapshot))
                        {
                            cf.OnSaved = (snapshot) =>
                            {
                                SetAlarmThresholdsFromSnapshot(snapshot);
                                AddLogMessage("알람 임계값 설정이 업데이트되었습니다.", "INFO");
                            };
                            cf.ShowDialog(this);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, $"설정 폼을 여는 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        AddLogMessage($"Config 폼 오류: {ex.Message}", "ERROR");
                    }
                    break;
                case AppSection.SysInfo:
                    ShowSystemInfoSummary();
                    break;
            }

            // 섹션 전환: 동일 이름의 패널이 있으면 표시하고, 나머지는 숨김 (존재하지 않으면 무시)
            string targetPanelName = "panel" + name;
            var allCandidates = new string[]
            {
                "panelMain","panelVerification","panelTransfer","panelOperate","panelRecipe",
                "panelMaint","panelConfig","panelTrend","panelReport","panelSysInfo"
            };
            foreach (var panelName in allCandidates)
            {
                var found = Controls.Find(panelName, true);
                if (found != null && found.Length > 0)
                {
                    var pnl = found[0] as Panel;
                    if (pnl != null) pnl.Visible = string.Equals(panelName, targetPanelName, StringComparison.OrdinalIgnoreCase);
                }
            }

            AddLogMessage($"섹션 전환: {name}", "INFO");
        }

        private void buttonNavMain_Click(object sender, EventArgs e)
        {
            UpdateTabButtonStates("Main");
            NavigateToSection(AppSection.Main);
        }

        private void buttonNavVerification_Click(object sender, EventArgs e)
        {
            UpdateTabButtonStates("Verification");
            NavigateToSection(AppSection.Verification);
            // Verification 폼 열기
            try
            {
                using (var vf = new VerificationForm())
                {
                    var checks = BuildVerificationChecklist();
                    string verReasons;
                    var ok = EvaluateInterlocks(out verReasons);
                    vf.SetResults(checks, ok ? "모든 인터락 통과" : $"인터락 미통과: {verReasons}");
                    vf.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                AddLogMessage($"Verification 폼 오류: {ex.Message}", "ERROR");
            }
        }

        private void buttonNavTransfer_Click(object sender, EventArgs e)
        {
            UpdateTabButtonStates("Transfer");
            NavigateToSection(AppSection.Transfer);
            // Transfer 폼 열기
            try
            {
                using (var tfm = new TransferForm())
                {
                    tfm.OnAutoPlan = () => AutoPlanTransferIfNeeded();
                    tfm.OnClear = () => ClearTransferQueue();
                    tfm.ProvideQueueLines = () => GetTransferQueueLines();
                    tfm.RefreshQueue();
                    tfm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                AddLogMessage($"Transfer 폼 오류: {ex.Message}", "ERROR");
            }
        }

        private void buttonNavOperate_Click(object sender, EventArgs e)
        {
            UpdateNavButtonStates("Operate");
            NavigateToSection(AppSection.Operate);
        }

        private void buttonNavRecipe_Click(object sender, EventArgs e)
        {
            UpdateNavButtonStates("Recipe");
            
            if (!EnsureLoggedIn())
            {
                return;
            }

            if (CurrentRole != "관리자")
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
                            ReloadRecipeCombo(list); 
                        } 
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"레시피 리로드 오류: {ex.Message}");
                        }
                    };
                    f.ShowDialog(this);
                    // 닫힌 뒤에도 한 번 더 로드 보정
                    ReloadRecipeCombo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Recipe 관리 화면을 여는 중 문제가 발생했습니다.\r\n{ex.Message}\r\n\r\n{ex.GetType().FullName}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonNavMaint_Click(object sender, EventArgs e)
        {
            UpdateNavButtonStates("Maintenance");
            NavigateToSection(AppSection.Maintenance);
        }

        private void buttonEquipmentControl_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ecf = new EquipmentControlForm(EtherCAT_M, EthercatConnected))
                {
                    // 도어 상태 변경 시 UI 업데이트 콜백
                    ecf.OnDoorStateChanged = (region, isOpen) =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                ViewModel?.SetDoorState(region, isOpen);
                                ApplyDoorVisualsForRegion(region, animate: true);
                            }));
                        }
                        else
                        {
                            ViewModel?.SetDoorState(region, isOpen);
                            ApplyDoorVisualsForRegion(region, animate: true);
                        }
                    };
                    
                    // Chamber 램프 상태 변경 시 UI 업데이트 콜백
                    ecf.OnChamberLampStateChanged = (region, isOn) =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                SetChamberLampUIOnly(region, isOn);
                            }));
                        }
                        else
                        {
                            SetChamberLampUIOnly(region, isOn);
                        }
                    };
                    
                    // 3색 램프 상태 변경 시 UI 업데이트 콜백
                    ecf.OnMainLampStateChanged = (red, yellow, green) =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                UpdateMainLampColorsUIOnly(green ? Color.ForestGreen : (Color?)null, yellow, red);
                            }));
                        }
                        else
                        {
                            UpdateMainLampColorsUIOnly(green ? Color.ForestGreen : (Color?)null, yellow, red);
                        }
                    };
                    
                    // 서보 상태 변경 시 UI 업데이트 콜백
                    ecf.OnServoStateChanged = (servoIsOn) =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                IsServoOn = servoIsOn;  // 서보 상태 플래그 업데이트
                                if (!servoIsOn)
                                {
                                    TmHardwareInitialized = false;  // 서보 OFF 시 원점복귀 상태 초기화
                                }
                                UpdateServoStatusLabel();
                            }));
                        }
                        else
                        {
                            IsServoOn = servoIsOn;  // 서보 상태 플래그 업데이트
                            if (!servoIsOn)
                            {
                                TmHardwareInitialized = false;  // 서보 OFF 시 원점복귀 상태 초기화
                            }
                            UpdateServoStatusLabel();
                        }
                    };
                    
                    // 원점복귀 요청 시 UI 업데이트 콜백
                    ecf.OnHomingRequested = () =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                TmHardwareInitialized = true;
                                UpdateServoStatusLabel();
                                AddLogMessage("장비제어 폼에서 원점복귀 완료", "INFO");
                            }));
                        }
                        else
                        {
                            TmHardwareInitialized = true;
                            UpdateServoStatusLabel();
                            AddLogMessage("장비제어 폼에서 원점복귀 완료", "INFO");
                        }
                    };
                    
                    ecf.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"장비 제어 폼을 여는 중 오류가 발생했습니다.\r\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddLogMessage($"장비 제어 폼 오류: {ex.Message}", "ERROR");
            }
        }

        private void buttonNavConfig_Click(object sender, EventArgs e)
        {
            UpdateNavButtonStates("Config");
            if (!EnsureLoggedIn())
            {
                return;
            }

            if (CurrentRole != "관리자")
            {
                MessageBox.Show("설정 변경은 관리자만 접근할 수 있습니다.", "권한 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 현재 알람 임계값을 EnvThresholdSnapshot으로 변환
            var currentSnapshot = new EnvThresholdSnapshot
            {
                TempWarn = AlarmThresholds.TempWarnDiffC,
                TempAlarm = AlarmThresholds.TempAlarmDiffC,
                PressWarnRatio = AlarmThresholds.PressWarnRatio,
                PressAlarmRatio = AlarmThresholds.PressAlarmRatio,
                PressWarnAbs = AlarmThresholds.PressWarnAbsTorr,
                PressAlarmAbs = AlarmThresholds.PressAlarmAbsTorr,
                RfWarnRatio = AlarmThresholds.RfWarnRatio,
                RfAlarmRatio = AlarmThresholds.RfAlarmRatio,
                GasWarn = AlarmThresholds.GasWarnAbsSccm,
                GasAlarm = AlarmThresholds.GasAlarmAbsSccm,
                GasLeakWarn = AlarmThresholds.GasLeakWarnSccm,
                GasLeakAlarm = AlarmThresholds.GasLeakAlarmSccm
            };

            using (var configForm = new ConfigForm(currentSnapshot))
            {
                configForm.OnSaved = (snapshot) =>
                {
                    // 저장된 설정을 EnvAlarmThresholds로 변환하여 적용
                    AlarmThresholds = new EnvAlarmThresholds
                    {
                        TempWarnDiffC = snapshot.TempWarn,
                        TempAlarmDiffC = snapshot.TempAlarm,
                        PressWarnRatio = snapshot.PressWarnRatio,
                        PressAlarmRatio = snapshot.PressAlarmRatio,
                        PressWarnAbsTorr = snapshot.PressWarnAbs,
                        PressAlarmAbsTorr = snapshot.PressAlarmAbs,
                        RfWarnRatio = snapshot.RfWarnRatio,
                        RfAlarmRatio = snapshot.RfAlarmRatio,
                        GasWarnAbsSccm = snapshot.GasWarn,
                        GasAlarmAbsSccm = snapshot.GasAlarm,
                        GasLeakWarnSccm = snapshot.GasLeakWarn,
                        GasLeakAlarmSccm = snapshot.GasLeakAlarm
                    };
                    AddLogMessage("알람 임계값 설정이 업데이트되었습니다.", "INFO");
                };
                configForm.ShowDialog(this);
            }
        }

        private void buttonNavTrend_Click(object sender, EventArgs e)
        {
            UpdateNavButtonStates("Trend");
            NavigateToSection(AppSection.Trend);
        }

        private void buttonNavReport_Click(object sender, EventArgs e)
        {
            UpdateNavButtonStates("Report");
            NavigateToSection(AppSection.Report);
        }

        private void buttonNavSysInfo_Click(object sender, EventArgs e)
        {
            UpdateNavButtonStates("System");
            NavigateToSection(AppSection.SysInfo);
        }

        // ===== Verification: 인터락 점검 =====
        private void RunVerificationChecks()
        {
            if (EvaluateInterlocks(out var reasons))
            {
                AddLogMessage("Verification: 모든 인터락 통과", "INFO");
                VerificationAlarmDismissed = false; // 인터락 통과 시 플래그 리셋
            }
            else
            {
                // 알람이 리셋되지 않았을 때만 헤더에 알람 표시
                if (!VerificationAlarmDismissed)
            {
                AddLogMessage($"Verification: 인터락 미통과 - {reasons}", "WARN");
                UpdateHeaderEventMessage("ALARM", $"인터락 미통과: {reasons}");
                }
            }
        }

        internal bool EvaluateInterlocks(out string reasons)
        {
            var failed = new List<string>();
            // EtherCAT 연결
            if (!EthercatConnected)
            {
                failed.Add("EtherCAT 미연결");
            }
            // FOUP 장착 상태
            if (!IsFoupMounted)
            {
                failed.Add("FOUP 미장착");
            }
            // 장비 에러/알람 상태
            if (CurrentProcessState == ProcessState.Error || HasAlarm)
            {
                failed.Add("장비 알람/에러 상태");
            }
            // 챔버 진공/압력(예: PMB가 진공 요구): 스펙 대비 편차 체크
            foreach (var unit in new[] { "PMA", "PMB", "PMC" })
            {
                if (ChamberEnvSpecs.TryGetValue(unit, out var spec) && ChamberEnvLive.TryGetValue(unit, out var live))
                {
                    var diffPress = Math.Abs(live.PressureTorr - spec.TargetPressureTorr);
                    // 간단 기준: 목표 1 Torr 미만인 경우 5x 이상이면 실패, 그 외 ±15 Torr 이상 실패
                    bool failPress = spec.TargetPressureTorr < 1.0
                        ? (live.PressureTorr > spec.TargetPressureTorr * 5.0)
                        : (diffPress > 15.0);
                    if (failPress)
                    {
                        failed.Add($"{unit} 압력 인터락 실패");
                    }
                    var diffTemp = Math.Abs(live.TemperatureC - spec.TargetTemperatureC);
                    if (diffTemp > 10.0) // 온도 이탈 과대 시 실패
                    {
                        failed.Add($"{unit} 온도 인터락 실패");
                    }
                }
            }
            reasons = string.Join(", ", failed);
            return failed.Count == 0;
        }

        internal (string name, bool ok)[] BuildVerificationChecklist()
        {
            var items = new List<(string name, bool ok)>();
            items.Add(("EtherCAT 연결", EthercatConnected));
            items.Add(("FOUP 장착", IsFoupMounted));
            items.Add(("장비 알람/에러 없음", !(CurrentProcessState == ProcessState.Error || HasAlarm)));
            foreach (var unit in new[] { "PMA", "PMB", "PMC" })
            {
                if (ChamberEnvSpecs.TryGetValue(unit, out var spec) && ChamberEnvLive.TryGetValue(unit, out var live))
                {
                    bool pressOk = spec.TargetPressureTorr < 1.0
                        ? (live.PressureTorr <= spec.TargetPressureTorr * 5.0)
                        : (Math.Abs(live.PressureTorr - spec.TargetPressureTorr) <= 15.0);
                    bool tempOk = Math.Abs(live.TemperatureC - spec.TargetTemperatureC) <= 10.0;
                    items.Add(($"{unit} 압력 범위", pressOk));
                    items.Add(($"{unit} 온도 범위", tempOk));
                }
                else
                {
                    items.Add(($"{unit} 환경 데이터 준비", false));
                }
            }
            return items.ToArray();
        }

        // ===== Transfer: 자동 플래닝 훅 =====
        internal void AutoPlanTransferIfNeeded()
        {
            // 데모: FOUP에 웨이퍼가 있고 TM 유휴일 때 간단한 이동 계획을 로그로 남김
            if (TmPhase == TransferController.TmPhase.Idle && (currentFoupACount > 0 || currentFoupBCount > 0))
            {
                var source = currentFoupACount > 0 ? "FOUP A" : "FOUP B";
                var target = "PMA";
                AddLogMessage($"이송 계획 생성: {source} → {target} (데모)", "INFO");
            }
        }

        internal void ClearTransferQueue()
        {
            TransferService?.ClearQueue();
            AddLogMessage("이송 큐를 비웠습니다.", "INFO");
        }

        internal List<string> GetTransferQueueLines()
        {
            var lines = new List<string>();
            lines.Add(CurrentTransfer != null
                ? $"[진행중] {EquipmentRegionHelper.FormatRegionLabel(CurrentTransfer.Pickup)} → {EquipmentRegionHelper.FormatRegionLabel(CurrentTransfer.Dropoff)} (웨이퍼 #{CurrentTransfer.Wafer?.Id})"
                : "[진행중 없음]");
            if (TransferService != null)
            {
                foreach (var t in TransferService.GetQueuedTasks())
                {
                    lines.Add($"{EquipmentRegionHelper.FormatRegionLabel(t.Pickup)} → {EquipmentRegionHelper.FormatRegionLabel(t.Dropoff)} (웨이퍼 #{t.Wafer?.Id})");
                }
            }
            return lines;
        }

        // ===== Trend: PV/SV 샘플 수집 =====
        private readonly List<(DateTime t, string unit, double pvTemp, double svTemp, double pvPress, double svPress)> trendSamples
            = new List<(DateTime, string, double, double, double, double)>();
        private bool trendCaptureEnabled = false;

        private void StartTrendCapture()
        {
            trendCaptureEnabled = true;
            AddLogMessage("Trend 캡처 시작", "INFO");
        }

        private void StopTrendCapture()
        {
            trendCaptureEnabled = false;
            AddLogMessage($"Trend 캡처 중지 (샘플 {trendSamples.Count}개)", "INFO");
        }

        // ===== Maintenance: 기본 점검/조치 구현 =====
        private void PerformLeakTest()
        {
            // 데모: 현재 압력 값이 진공 스펙 배수 이내인지 간단 점검
            int failCount = 0;
            foreach (var unit in new[] { "PMA", "PMB", "PMC" })
            {
                if (ChamberEnvSpecs.TryGetValue(unit, out var spec) && ChamberEnvLive.TryGetValue(unit, out var live))
                {
                    bool ok = spec.TargetPressureTorr < 1.0
                        ? (live.PressureTorr <= spec.TargetPressureTorr * 5.0)
                        : (Math.Abs(live.PressureTorr - spec.TargetPressureTorr) <= 15.0);
                    if (!ok) failCount++;
                }
            }
            AddLogMessage(failCount == 0 ? "Leak Test: OK" : $"Leak Test: FAIL ({failCount}개 유닛)", failCount == 0 ? "INFO" : "WARN");
        }

        private void PerformPumpPurge()
        {
            // 데모: 압력 목표를 조금 낮추는 Purge 적용
            foreach (var unit in new[] { "PMA", "PMB", "PMC" })
            {
                if (ChamberEnvSpecs.TryGetValue(unit, out var spec))
                {
                    var newPress = Math.Max(0, spec.TargetPressureTorr * 0.95);
                    ChamberEnvSpecs[unit] = new ChamberController.ChamberEnvironmentSpec(spec.Temperature, $"{newPress} Torr", spec.Humidity, spec.Notes, spec.TargetTemperatureC, newPress, spec.TargetHumidityPercent);
                }
            }
            InitializeEnvironmentTelemetry();
            AddLogMessage("Pump Purge 수행: 목표 압력 5% 감소 적용", "INFO");
        }

        private void PerformSensorCalibration()
        {
            // 데모: 라이브값을 목표에 한 번에 근접시키는 보정
            foreach (var unit in new[] { "PMA", "PMB", "PMC" })
            {
                if (ChamberEnvSpecs.TryGetValue(unit, out var spec) && ChamberEnvLive.TryGetValue(unit, out var live))
                {
                    live.TemperatureC = spec.TargetTemperatureC;
                    live.PressureTorr = spec.TargetPressureTorr;
                    live.HumidityPercent = spec.TargetHumidityPercent;
                }
            }
            AddLogMessage("Sensor Calibration 완료: 라이브값을 목표로 보정", "INFO");
        }

        private void PerformDoorCycleTest()
        {
            // 데모: 문 개폐 시퀀스 로그만 기록
            AddLogMessage("Door Cycle Test 시작: A,B,C 순차 개폐", "INFO");
            AddLogMessage("PMA Door Open → Close", "INFO");
            AddLogMessage("PMB Door Open → Close", "INFO");
            AddLogMessage("PMC Door Open → Close", "INFO");
            AddLogMessage("Door Cycle Test 완료", "INFO");
        }

        // 시뮬레이션 타이머 틱 등 적절한 위치에서 호출되는 업데이트 루프에 아래 훅 추가되어야 함
        internal void CaptureTrendSampleTick()
        {
            if (!trendCaptureEnabled)
            {
                return;
            }
            foreach (var unit in new[] { "PMA", "PMB", "PMC" })
            {
                if (ChamberEnvSpecs.TryGetValue(unit, out var spec) && ChamberEnvLive.TryGetValue(unit, out var live))
                {
                    trendSamples.Add((DateTime.Now, unit, live.TemperatureC, spec.TargetTemperatureC, live.PressureTorr, spec.TargetPressureTorr));
                    // 간단한 메모리 제한
                    if (trendSamples.Count > 5000)
                    {
                        trendSamples.RemoveRange(0, 1000);
                    }
                }
            }
        }

        // ===== Report: 로그 CSV 내보내기 (레벨별 파일 분리) =====
        internal string ExportLogsToCsv()
        {
            // 기본 저장 위치: Documents 폴더의 SemiconductorUi_Logs 폴더
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var logFolder = System.IO.Path.Combine(documentsPath, "SemiconductorUi_Logs");
            
            // 폴더가 없으면 생성
            if (!System.IO.Directory.Exists(logFolder))
            {
                System.IO.Directory.CreateDirectory(logFolder);
            }

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var savedFiles = new List<string>();

            // 레벨별로 로그 분류
            var logsByLevel = new Dictionary<string, List<string>>();
            foreach (var entry in logEntries)
            {
                // entry 포맷: [HH:mm:ss] [LEVEL] message
                var levelStart = entry.IndexOf(']', 2) + 3; // +3 to skip "] [" and get actual level
                var levelEnd = entry.IndexOf(']', levelStart);
                var level = (levelStart > 2 && levelEnd > levelStart) ? entry.Substring(levelStart, levelEnd - levelStart) : "INFO";
                
                if (!logsByLevel.ContainsKey(level))
                {
                    logsByLevel[level] = new List<string>();
                }
                logsByLevel[level].Add(entry);
            }

            // 레벨별로 파일 저장
            foreach (var kvp in logsByLevel)
            {
                var level = kvp.Key;
                var entries = kvp.Value;
                
                var fileName = $"SemiconductorUi_Logs_{level}_{timestamp}.csv";
                var filePath = System.IO.Path.Combine(logFolder, fileName);
                
                using (var sw = new System.IO.StreamWriter(filePath, false, Encoding.UTF8))
                {
                    sw.WriteLine("Timestamp,Level,Message");
                    foreach (var entry in entries)
                    {
                        // entry 포맷: [HH:mm:ss] [LEVEL] message
                        var ts = entry.Length >= 10 ? entry.Substring(1, 8) : "";
                        var msgStart = entry.IndexOf(']', entry.IndexOf(']') + 1) + 2;
                        var msg = msgStart > 1 && msgStart < entry.Length ? entry.Substring(msgStart) : entry;
                        sw.WriteLine($"{ts},{level},\"{msg.Replace("\"", "\"\"")}\"");
                    }
                }
                savedFiles.Add(filePath);
            }

            // 저장된 파일 목록 반환 (여러 파일이면 첫 번째 파일 경로 + 개수 표시)
            if (savedFiles.Count > 0)
            {
                if (savedFiles.Count == 1)
                {
                    return savedFiles[0];
                }
                else
                {
                    // 여러 파일 저장 완료 메시지
                    return $"{logFolder}\n({savedFiles.Count}개 파일 저장 완료)";
                }
            }
            
            return string.Empty;
        }

        // ===== SysInfo: 시스템 요약 보여주기 =====
        internal void ShowSystemInfoSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"App Version: {Application.ProductVersion}");
            sb.AppendLine($"OS: {Environment.OSVersion}");
            sb.AppendLine($".NET: {Environment.Version}");
            sb.AppendLine($"EtherCAT: {(EthercatConnected ? "Connected" : "Disconnected")}");
            sb.AppendLine($"Login: {(IsLoggedIn ? CurrentUser : "Guest")}");
            sb.AppendLine($"Process: {CurrentProcessState}");
            MessageBox.Show(sb.ToString(), "System Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 챔버별 알람 상태를 한 번에 가져오기 (코드 중복 제거)
        /// </summary>
        private (bool pma, bool pmb, bool pmc) GetChamberAlarmStatuses()
        {
            return (
                ChamberAlarmStatus.ContainsKey("PMA") && ChamberAlarmStatus["PMA"],
                ChamberAlarmStatus.ContainsKey("PMB") && ChamberAlarmStatus["PMB"],
                ChamberAlarmStatus.ContainsKey("PMC") && ChamberAlarmStatus["PMC"]
            );
        }

        /// <summary>
        /// 챔버별 알람 심각도를 한 번에 가져오기
        /// </summary>
        /// <summary>
        /// 상단 헤더의 PM1, PM2, PM3 상태 색상 업데이트
        /// 알람 상태를 확인하여 색상 변경
        /// </summary>
        internal void UpdateHeaderPmStatusColor(string pmaStatus, string pmbStatus, string pmcStatus)
        {
            var (pmaHasAlarm, pmbHasAlarm, pmcHasAlarm) = GetChamberAlarmStatuses();
            
            UpdatePmStatusColorInternal(labelHeaderPM1Status, pmaStatus, pmaHasAlarm);
            UpdatePmStatusColorInternal(labelHeaderPM2Status, pmbStatus, pmbHasAlarm);
            UpdatePmStatusColorInternal(labelHeaderPM3Status, pmcStatus, pmcHasAlarm);
        }

        /// <summary>
        /// 상단 헤더의 PM 상태 색상을 즉시 업데이트 (알람 상태 변경 시 호출)
        /// 이미 업데이트된 PmDetails를 재사용하여 성능 최적화
        /// </summary>
        internal void UpdateHeaderPmStatusColorImmediate()
        {
            // PmDetails Dictionary에서 이미 계산된 상태 재사용 (성능 최적화)
            string pmaStatus = PmDetails.ContainsKey("PMA") ? PmDetails["PMA"].StatusText : ChamberAState?.StatusText ?? "Idle";
            string pmbStatus = PmDetails.ContainsKey("PMB") ? PmDetails["PMB"].StatusText : ChamberBState?.StatusText ?? "Idle";
            string pmcStatus = PmDetails.ContainsKey("PMC") ? PmDetails["PMC"].StatusText : ChamberCState?.StatusText ?? "Idle";

            // 상단 헤더 라벨 색상 업데이트
            UpdateHeaderPmStatusColor(pmaStatus, pmbStatus, pmcStatus);
            
            // 헤더 카드 패널 색상도 업데이트
            UpdateHeaderCardPmStatusColor(pmaStatus, pmbStatus, pmcStatus);
        }

        /// <summary>
        /// 헤더 카드의 PM 상태 색상 업데이트
        /// 알람 상태를 확인하여 색상 변경
        /// </summary>
        internal void UpdateHeaderCardPmStatusColor(string pmaStatus, string pmbStatus, string pmcStatus)
        {
            var (pmaHasAlarm, pmbHasAlarm, pmcHasAlarm) = GetChamberAlarmStatuses();
            
            UpdatePmStatusColorInternal(panelHeaderCardPMA, pmaStatus, pmaHasAlarm);
            UpdatePmStatusColorInternal(panelHeaderCardPMB, pmbStatus, pmbHasAlarm);
            UpdatePmStatusColorInternal(panelHeaderCardPMC, pmcStatus, pmcHasAlarm);
        }

        /// <summary>
        /// PM 상태 색상 업데이트
        /// </summary>
        /// <param name="control">업데이트할 컨트롤</param>
        /// <param name="statusText">상태 텍스트</param>
        /// <param name="hasAlarm">알람 발생 여부</param>
        private void UpdatePmStatusColorInternal(Control control, string statusText, bool hasAlarm = false)
        {
            if (control == null || string.IsNullOrEmpty(statusText))
            {
                return;
            }
            
            // 상태 텍스트를 소문자로 변환하여 비교
            string statusLower = statusText.ToLower();
            
            // 알람 발생 시 빨간색
            if (hasAlarm)
            {
                control.BackColor = Color.FromArgb(244, 67, 54); // 적색
            }
            // 상태 텍스트에 오류/알람 키워드가 있는 경우 (하위 호환성)
            else if (statusLower.IndexOf("오류", StringComparison.OrdinalIgnoreCase) >= 0 || 
                     statusLower.IndexOf("에러", StringComparison.OrdinalIgnoreCase) >= 0 || 
                     statusLower.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     statusLower.IndexOf("알람", StringComparison.OrdinalIgnoreCase) >= 0 || 
                     statusLower.IndexOf("alarm", StringComparison.OrdinalIgnoreCase) >= 0 || 
                     statusLower.IndexOf("실패", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     statusLower.IndexOf("fail", StringComparison.OrdinalIgnoreCase) >= 0 || 
                     statusLower.IndexOf("치명", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                control.BackColor = Color.FromArgb(244, 67, 54); // 적색
            }
            // EtherCAT 연결 전: 회색
            else if (!EthercatConnected)
            {
                control.BackColor = Color.FromArgb(245, 245, 250); // 회색
            }
            // EtherCAT 연결 후: 녹색 (공정 중이든 아니든)
            else
            {
                control.BackColor = Color.FromArgb(76, 175, 80); // 녹색
            }
        }

        internal void AddLogMessage(string message, string level)
        {
            // LoggerService를 통해 로그 기록
            LoggerService?.Log(message, level);
            
            // 알람 관련 레벨만 헤더 표시 업데이트
            var normalizedLevel = string.IsNullOrWhiteSpace(level) ? "INFO" : level.Trim().ToUpperInvariant();
            if (normalizedLevel == "ALARM" || normalizedLevel == "ERROR" || normalizedLevel == "CRITICAL")
            {
                UpdateHeaderEventMessage(normalizedLevel, message);
            }
        }
        
        // ===== 로그 자동 저장 및 초기화 (레벨별 파일 분리) =====
        private void AutoSaveAndClearLogs()
        {
            try
            {
                // 기본 저장 위치: Documents 폴더의 SemiconductorUi_Logs 폴더
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var logFolder = System.IO.Path.Combine(documentsPath, "SemiconductorUi_Logs", "AutoSave");
                
                // 폴더가 없으면 생성
                if (!System.IO.Directory.Exists(logFolder))
                {
                    System.IO.Directory.CreateDirectory(logFolder);
                }

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var savedFiles = new List<string>();
                
                // 레벨별로 로그 분류
                var logsByLevel = new Dictionary<string, List<string>>();
                var currentLogEntries = LoggerService?.GetLogEntries() ?? new List<string>();
                foreach (var entry in currentLogEntries)
                {
                    // entry 포맷: [HH:mm:ss] [LEVEL] message
                    var levelStart = entry.IndexOf(']', 2) + 3; // +3 to skip "] [" and get actual level
                    var levelEnd = entry.IndexOf(']', levelStart);
                    var level = (levelStart > 2 && levelEnd > levelStart) ? entry.Substring(levelStart, levelEnd - levelStart) : "INFO";
                    
                    if (!logsByLevel.ContainsKey(level))
                    {
                        logsByLevel[level] = new List<string>();
                    }
                    logsByLevel[level].Add(entry);
                }
                
                // 레벨별로 파일 저장
                foreach (var kvp in logsByLevel)
                {
                    var level = kvp.Key;
                    var entries = kvp.Value;
                    
                    var fileName = $"AutoSave_{level}_{timestamp}.csv";
                    var filePath = System.IO.Path.Combine(logFolder, fileName);
                    
                    using (var sw = new System.IO.StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        sw.WriteLine("Timestamp,Level,Message");
                        
                        // entries는 최신이 맨 앞이므로 역순으로 저장 (시간순)
                        for (int i = entries.Count - 1; i >= 0; i--)
                        {
                            var entry = entries[i];
                            // entry 포맷: [HH:mm:ss] [LEVEL] message
                            var ts = entry.Length >= 10 ? entry.Substring(1, 8) : "";
                            var msgStart = entry.IndexOf(']', entry.IndexOf(']') + 1) + 2;
                            var msg = msgStart > 1 && msgStart < entry.Length ? entry.Substring(msgStart) : entry;
                            sw.WriteLine($"{ts},{level},\"{msg.Replace("\"", "\"\"")}\"");
                        }
                    }
                    savedFiles.Add(fileName);
                }
                
                // 로그 초기화 (새로 시작)
                LoggerService?.Clear();
                
                // 자동 저장 완료 메시지 추가 (새 로그의 첫 항목)
                var fileCount = savedFiles.Count;
                LoggerService?.Info($"이전 로그 자동 저장 완료: {fileCount}개 파일 (레벨별 분리)");
            }
            catch (Exception ex)
            {
                // 저장 실패 시 LoggerService가 자동으로 관리하므로 별도 처리 불필요
                System.Diagnostics.Debug.WriteLine($"로그 자동 저장 실패: {ex.Message}");
            }
        }

        private void UpdateHeaderEventMessage(string level, string message)
        {
            if (labelHeaderEventTitle == null || labelHeaderEventMessage == null)
            {
                return;
            }

            var normalizedLevel = string.IsNullOrWhiteSpace(level)
                ? "ALARM"
                : level.Trim().ToUpperInvariant();

            labelHeaderEventTitle.Text = "최근 알람";
            labelHeaderEventMessage.Text = message;

            var badgeColor = Color.FromArgb(96, 125, 139);
            switch (normalizedLevel)
            {
                case "WARN":
                case "WARNING":
                    badgeColor = Color.FromArgb(255, 167, 38);
                    break;
                case "ERROR":
                case "ALARM":
                case "CRITICAL":
                    badgeColor = Color.FromArgb(229, 57, 53);
                    break;
                case "INFO":
                    badgeColor = Color.FromArgb(76, 175, 80);
                    break;
            }

            if (labelHeaderEventLevel != null)
            {
                labelHeaderEventLevel.Text = normalizedLevel;
                labelHeaderEventLevel.BackColor = badgeColor;
            }

            if (panelHeaderMessageAccent != null)
            {
                panelHeaderMessageAccent.BackColor = badgeColor;
            }

            // 헤더 알람 업데이트 시 알람 리셋 버튼 상태도 업데이트
            UpdateProcessControlButtons();
        }

        // 런타임 버튼 이벤트 연결 (디자이너 연결 누락 보완)
        private bool handlersWired = false;
        private void WireButtonHandlers()
        {
            if (handlersWired) return;
            handlersWired = true;

            HookButton("buttonNavMain", navigationEventHandlers.ButtonNavMain_Click);
            HookButton("buttonNavVerification", navigationEventHandlers.ButtonNavVerification_Click);
            HookButton("buttonNavTransfer", navigationEventHandlers.ButtonNavTransfer_Click);
            HookButton("buttonNavOperate", navigationEventHandlers.ButtonNavOperate_Click);
            HookButton("buttonNavRecipe", navigationEventHandlers.ButtonNavRecipe_Click);
            HookButton("buttonNavMaint", navigationEventHandlers.ButtonNavMaint_Click);
            HookButton("buttonNavConfig", navigationEventHandlers.ButtonNavConfig_Click);
            HookButton("buttonNavTrend", navigationEventHandlers.ButtonNavTrend_Click);
            HookButton("buttonNavReport", navigationEventHandlers.ButtonNavReport_Click);
            HookButton("buttonNavSysInfo", navigationEventHandlers.ButtonNavSysInfo_Click);

            // 이름이 다를 경우 텍스트 기반 훅(영문/대소문자 무시)
            HookButtonByText(new[] { "verification", "검증", "인터락" }, navigationEventHandlers.ButtonNavVerification_Click);
            HookButtonByText(new[] { "transfer", "이송" }, navigationEventHandlers.ButtonNavTransfer_Click);
            HookButtonByText(new[] { "operate", "운전" }, navigationEventHandlers.ButtonNavOperate_Click);
            HookButtonByText(new[] { "recipe", "레시피" }, navigationEventHandlers.ButtonNavRecipe_Click);
            HookButtonByText(new[] { "maint", "maintenance", "정비" }, navigationEventHandlers.ButtonNavMaint_Click);
            HookButtonByText(new[] { "config", "설정" }, navigationEventHandlers.ButtonNavConfig_Click);
            HookButtonByText(new[] { "trend", "트렌드" }, navigationEventHandlers.ButtonNavTrend_Click);
            HookButtonByText(new[] { "report", "리포트", "보고서" }, navigationEventHandlers.ButtonNavReport_Click);
            HookButtonByText(new[] { "sys info", "system", "시스템" }, navigationEventHandlers.ButtonNavSysInfo_Click);

            HookButton("buttonLogin", loginEventHandlers.ButtonLogin_Click);
            HookButton("buttonLogout", loginEventHandlers.ButtonLogout_Click);
            HookButton("buttonUserManagement", loginEventHandlers.ButtonUserManagement_Click);
            HookButton("buttonStart", processEventHandlers.ButtonStart_Click);
            HookButton("buttonPause", processEventHandlers.ButtonPause_Click);
            HookButton("buttonStop", processEventHandlers.ButtonStop_Click);
            HookButton("buttonResetAlarm", processEventHandlers.ButtonResetAlarm_Click);
            HookButton("buttonResetProcess", processEventHandlers.ButtonResetProcess_Click);
            HookButton("buttonApplyRecipe", processEventHandlers.ButtonApplyRecipe_Click);
            HookButton("buttonToggleFoupMount", waferEventHandlers.ButtonToggleFoupMount_Click);
            HookButton("buttonWaferLoading", waferEventHandlers.ButtonWaferLoading_Click);
            HookButton("buttonWaferUnloading", waferEventHandlers.ButtonWaferUnloading_Click);

            HookButton("buttonEthercatConnect", hardwareEventHandlers.ButtonEthercatConnect_Click);
            HookButton("buttonEthercatDisconnect", hardwareEventHandlers.ButtonEthercatDisconnect_Click);
            HookButton("buttonServoOn", hardwareEventHandlers.ButtonServoOn_Click);
            HookButton("buttonServoOff", hardwareEventHandlers.ButtonServoOff_Click);
            HookButton("buttonServoHome", hardwareEventHandlers.ButtonServoHome_Click);
            
            HookButton("buttonEquipmentControl", navigationEventHandlers.ButtonEquipmentControl_Click);
            
            HookButton("buttonPMAEnvDetail", equipmentEventHandlers.ButtonPMAEnvDetail_Click);
            HookButton("buttonPMBEnvDetail", equipmentEventHandlers.ButtonPMBEnvDetail_Click);
            HookButton("buttonPMCEnvDetail", equipmentEventHandlers.ButtonPMCEnvDetail_Click);
        }

        private void HookButton(string controlName, EventHandler handler)
        {
            var found = Controls.Find(controlName, true);
            if (found == null || found.Length == 0) return;
            foreach (var c in found)
            {
                var b = c as Button;
                if (b == null) continue;
                AttachClick(b, handler);
            }
        }

        private void HookButtonByText(string[] texts, EventHandler handler)
        {
            foreach (Control c in GetAllControls(this))
            {
                var b = c as Button;
                if (b == null) continue;
                var t = (b.Text ?? "").Trim();
                if (t.Length == 0) continue;
                foreach (var key in texts)
                {
                    if (t.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        AttachClick(b, handler);
                        break;
                    }
                }
            }
        }

        private static void AttachClick(Button b, EventHandler handler)
        {
            // 중복 연결 방지
            b.Click -= handler;
            b.Click += handler;
        }

        private static System.Collections.Generic.IEnumerable<Control> GetAllControls(Control root)
        {
            var stack = new System.Collections.Generic.Stack<Control>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();
                foreach (Control child in cur.Controls)
                {
                    stack.Push(child);
                }
                yield return cur;
            }
        }

        internal bool IsDesignEnvironment()
        {
            try
            {
                // LicenseManager 우선
                if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                {
                    return true;
                }
                // Site 기반 (일부 시나리오)
                if (this.Site != null && this.Site.DesignMode)
                {
                    return true;
                }
            }
            catch
            {
                // 무시하고 런타임으로 간주
            }
            return false;
        }

        internal void SetHeaderAlarmIdle()
        {
            if (labelHeaderEventTitle != null)
            {
                labelHeaderEventTitle.Text = "최근 알람";
            }
            if (labelHeaderEventMessage != null)
            {
                labelHeaderEventMessage.Text = "알람 없음";
                labelHeaderEventMessage.Invalidate();
                labelHeaderEventMessage.Update();
            }
            if (labelHeaderEventLevel != null)
            {
                labelHeaderEventLevel.Text = "";
                labelHeaderEventLevel.BackColor = Color.FromArgb(96, 125, 139);
                labelHeaderEventLevel.Invalidate();
                labelHeaderEventLevel.Update();
            }
            if (panelHeaderMessageAccent != null)
            {
                panelHeaderMessageAccent.BackColor = Color.FromArgb(96, 125, 139);
                panelHeaderMessageAccent.Invalidate();
                panelHeaderMessageAccent.Update();
            }

            // 헤더 알람 초기화 시 알람 리셋 버튼 상태도 업데이트
            UpdateProcessControlButtons();
            
            // UI 강제 새로고침
            if (labelHeaderEventMessage != null && labelHeaderEventMessage.Parent != null)
            {
                labelHeaderEventMessage.Parent.Invalidate();
                labelHeaderEventMessage.Parent.Update();
            }
        }

        internal void ShowPmEnvironmentDetail(string unitKey)
        {
            // 더 이상 사용하지 않음 - 환경 정보 표는 항상 표시됨
            // 이 메서드는 버튼 이벤트 핸들러와의 호환성을 위해 유지
        }

        private void InitializePmEnvironmentTables()
        {
            uiInitializer?.InitializePmEnvironmentTables();
        }

        /// <summary>
        /// PM 상태 패널 초기화 - 깜빡임 방지를 위한 커스텀 컨트롤로 교체
        /// </summary>
        private void InitializePmStatusPanels()
        {
            uiInitializer?.InitializePmStatusPanels();
        }

        /// <summary>
        /// PmStatusPanel 생성 및 기존 컨트롤 대체
        /// </summary>
        internal PmStatusPanel CreatePmStatusPanel(string title, Panel parentPanel)
        {
            if (parentPanel == null) return null;

            // 새 PmStatusPanel 생성
            var panel = new PmStatusPanel
            {
                Title = title,
                Dock = DockStyle.Fill,
                Padding = new Padding(0)
            };

            // 부모 패널에 추가
            parentPanel.Controls.Add(panel);
            panel.BringToFront();

            return panel;
        }

        /// <summary>
        /// PM 상태 패널 가져오기
        /// </summary>
        internal PmStatusPanel GetPmStatusPanel(string unitKey)
        {
            switch (unitKey)
            {
                case "PMA": return pmStatusPanelA;
                case "PMB": return pmStatusPanelB;
                case "PMC": return pmStatusPanelC;
                default: return null;
            }
        }

        internal void CreatePmEnvironmentTable(string unitKey)
        {
            Panel parentPanel = null;
            
            switch (unitKey)
            {
                case "PMA":
                    parentPanel = panelSummaryPMA;
                    break;
                case "PMB":
                    parentPanel = panelSummaryPMB;
                    break;
                case "PMC":
                    parentPanel = panelSummaryPMC;
                    break;
            }

            if (parentPanel == null) return;

            var envTable = new TableLayoutPanel
            {
                Name = $"tableEnv{unitKey}",
                ColumnCount = 4,
                RowCount = 7,
                AutoSize = false,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                BackColor = parentPanel.BackColor, // 부모 패널과 동일한 색상
                Padding = new Padding(15, 3, 15, 2), // 좌우 간격을 더 넓게 설정
                Visible = true
            };

            envTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
            envTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            envTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            envTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
            
            // 행 높이 설정 (상하 간격 조정)
            int rowHeight = 30; // 28px에서 30px로 더 늘림
            int lastRowHeight = 35; // 마지막 행만 더 높게 설정
            for (int i = 0; i < 6; i++) // 처음 6개 행
            {
                envTable.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
            }
            // 마지막 행만 더 높게 설정
            envTable.RowStyles.Add(new RowStyle(SizeType.Absolute, lastRowHeight));

            // 헤더
            var headers = new[] { "", "PV", "SV", "단위" };
            for (int i = 0; i < headers.Length; i++)
            {
                var header = new Label
                {
                    Text = headers[i],
                    ForeColor = Color.Gainsboro,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                    BackColor = Color.Transparent // 기존 표와 동일하게 투명
                };
                envTable.Controls.Add(header, i, 0);
            }

            // 데이터 행
            var envNames = new[] { "NF3", "O2", "CF4", "Press.", "RF", "Temp" };
            var envUnits = new[] { "sccm", "sccm", "sccm", "Torr", "W", "°C" };
            var labels = new Label[envNames.Length, 4]; // [row, col]

            for (int i = 0; i < envNames.Length; i++)
            {
                // 이름 (첫 번째 열은 기존 표처럼 MiddleCenter)
                labels[i, 0] = new Label
                {
                    Text = envNames[i],
                    ForeColor = Color.Gainsboro,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9F),
                    BackColor = Color.Transparent // 기존 표와 동일하게 투명
                };
                envTable.Controls.Add(labels[i, 0], 0, i + 1);

                // PV (수치는 MiddleLeft로 정렬)
                labels[i, 1] = new Label
                {
                    Text = "0",
                    ForeColor = Color.White,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    BackColor = Color.Transparent // 기존 표와 동일하게 투명
                };
                envTable.Controls.Add(labels[i, 1], 1, i + 1);

                // SV (수치는 MiddleLeft로 정렬)
                labels[i, 2] = new Label
                {
                    Text = "0",
                    ForeColor = Color.White,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    BackColor = Color.Transparent // 기존 표와 동일하게 투명
                };
                envTable.Controls.Add(labels[i, 2], 2, i + 1);

                // 단위 (MiddleLeft로 정렬)
                labels[i, 3] = new Label
                {
                    Text = envUnits[i],
                    ForeColor = Color.Gainsboro,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    BackColor = Color.Transparent // 기존 표와 동일하게 투명
                };
                envTable.Controls.Add(labels[i, 3], 3, i + 1);
            }

            // 환경 테이블을 패널 내부에 배치 (PmStatusPanel과 함께 표시)
            // 패널의 패딩(12px)을 고려하여 위치 설정
            int padding = 12; // 패널의 패딩
            int topOffset = 40; // 상단 여백
            envTable.Location = new Point(padding, topOffset);
            envTable.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            envTable.Width = 240; // 소수점 두 자리 숫자 표시를 위해 너비를 더 늘림
            // 행 높이 30px × 6행 + 35px(마지막 행) + 패딩(상 3px + 하 2px) = 220px
            envTable.Height = 30 * 6 + 35 + 5;

            parentPanel.Controls.Add(envTable);
            PmEnvTables[unitKey] = envTable;
            
            // 초기 값 업데이트
            UpdatePmEnvironmentTable(unitKey);
        }

        internal void UpdatePmEnvironmentTable(string unitKey)
        {
            uiUpdater?.UpdatePmEnvironmentTable(unitKey);
        }

        // 환경 경고/알람 판정 (AlarmManager로 위임)
        internal void EvaluateEnvironmentAlarms(
            string unitKey,
            double svNF3, double svO2, double svCF4, double svPress, double svRf, double svTemp,
            double pvNF3, double pvO2, double pvCF4, double pvPress, double pvRf, double pvTemp)
        {
            // AlarmManager로 위임 (알람 발생 여부 반환)
            bool hasAlarmInChamber = alarmManager?.EvaluateEnvironmentAlarms(
                unitKey,
                svNF3, svO2, svCF4, svPress, svRf, svTemp,
                pvNF3, pvO2, pvCF4, pvPress, pvRf, pvTemp) ?? false;

            // 알람 상태 업데이트 (각 PM별로 독립적으로 관리)
            if (hasAlarmInChamber)
            {
                alarmManager?.SetChamberAlarm(unitKey, true);
                HasAlarm = true;
                if (CurrentProcessState != ProcessState.Error)
                {
                    SetProcessState(ProcessState.Error, $"{unitKey} 환경 알람 발생");
                }
            }
            else
            {
                // 알람이 발생하지 않으면 해당 PM의 알람 상태를 false로 설정
                // (다른 PM에서 알람이 발생 중일 수 있으므로 HasAlarm은 별도로 확인)
                alarmManager?.SetChamberAlarm(unitKey, false);
            }

            // 상단 패널 색상 즉시 업데이트 (알람 상태 변경 시)
            UpdateHeaderPmStatusColorImmediate();
        }

        // EvaluateGas는 AlarmManager 내부에서 처리됨 (제거)

        private void buttonPMAEnvDetail_Click(object sender, EventArgs e)
        {
            ShowPmEnvironmentDetail("PMA");
        }

        private void buttonPMBEnvDetail_Click(object sender, EventArgs e)
        {
            ShowPmEnvironmentDetail("PMB");
        }

        private void buttonPMCEnvDetail_Click(object sender, EventArgs e)
        {
            ShowPmEnvironmentDetail("PMC");
        }

        // EthercatConnected, EtherCAT_M, TmHardwareController, TmHardwareInitialized는 
        // HardwareManager 속성으로 관리됨 (위의 속성 래퍼 참조)
        internal bool IsSyncingEquipmentState = false; // 장비 상태 동기화 중 플래그

        // === TM 하드웨어 제어 관련 ===
        // TmHardwareController는 HardwareManager 속성으로 관리됨
        internal bool TmHardwareActionPending = false;           // 하드웨어 동작 대기 중
        internal int doorOpenConsecutiveChecks = 0;              // 도어 열림 연속 확인 횟수
        private const int REQUIRED_DOOR_OPEN_CHECKS = 1;        // (미사용 - 시간 기반으로 변경됨)
        // DOOR_OPEN_WAIT_TICKS, DOOR_CLOSE_WAIT_TICKS는 AppSettings에서 읽어옴
        internal DateTime tmHardwareActionStartTime;             // 하드웨어 동작 시작 시간
        private DateTime? pauseStartTime;                        // 일시정지 시작 시간 (타임아웃 계산 시 일시정지 시간 제외용)
        internal bool doorOpenCommandSent = false;               // 도어 열기 명령 전송 여부
        internal bool doorCloseCommandSent = false;              // 도어 닫기 명령 전송 여부
        internal int doorCloseWaitTicks = 0;                     // 도어 닫기 대기 틱 수
        // TM_HARDWARE_ACTION_TIMEOUT_MS, CYLINDER_ACTION_TIMEOUT_MS는 AppSettings에서 읽어옴
        // TmHardwareInitialized는 HardwareManager 속성으로 관리됨
        internal bool IsServoOn = false;                         // 서보 ON 상태 추적
        
        // === 안정화 대기 시간 (밀리초) ===
        // 속도 최적화: 안전을 유지하면서 불필요한 대기 시간 감소
        // 안정화 지연 시간은 AppSettings에서 읽어옴
        internal DateTime tmSettleStartTime;                     // 안정화 시작 시간
        internal bool TmSettleWaiting = false;                   // 안정화 대기 중
        internal int tmCurrentSettleDelay = 0;                   // 현재 안정화 대기 시간
        
        private void buttonEthercatConnect_Click(object sender, EventArgs e)
        {
            // 연결 버튼은 "연결"만 수행 (토글 금지)
            if (EthercatConnected)
            {
                MessageBox.Show("이미 EtherCAT에 연결되어 있습니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // HardwareManager를 통해 EtherCAT 연결
                bool connected = HardwareManager?.ConnectEthercat() ?? false;
                
                if (connected)
                {
                    if (labelEthercatStatus != null)
                    {
                        labelEthercatStatus.Text = "EtherCAT: Connected";
                        labelEthercatStatus.ForeColor = Color.FromArgb(76, 175, 80);
                        labelEthercatStatus.BackColor = Color.FromArgb(170, 170, 180);
                    }
                    
                    // 연결 성공 시 실제 장비 상태를 읽어서 UI에 반영
                    try
                    {
                        SyncEquipmentStateToUI();
                    }
                    catch (Exception initEx)
                    {
                        AddLogMessage($"장비 상태 동기화 오류: {initEx.Message}", "WARN");
                    }

                    // TM 하드웨어 컨트롤러 초기화
                    try
                    {
                        InitializeTmHardwareController();
                    }
                    catch (Exception tmEx)
                    {
                        AddLogMessage($"TM 하드웨어 컨트롤러 초기화 오류: {tmEx.Message}", "WARN");
                    }

                    // 서보 상태 라벨 업데이트
                    UpdateServoStatusLabel();
                    
                    // EtherCAT 연결 시 하드웨어 모니터링을 위해 타이머 시작
                    // (SimulationTimer_Tick에서 하드웨어 모드일 때 TM 위치 업데이트)
                    // 하드웨어 모드에서는 더 빠른 타이머 주기 사용 (150ms)
                    SimulationController?.SetInterval(AppSettings.HardwareModeTickMilliseconds);
                    if (!SimulationController?.IsTimerRunning ?? false)
                    {
                        SimulationController?.StartTimer();
                    }
                    
                    // UI 전용 고속 타이머 시작 (50ms 주기로 하드웨어 상태 확인 및 UI 업데이트)
                    if (HardwareUiUpdateTimer != null && !HardwareUiUpdateTimer.Enabled)
                    {
                        HardwareUiUpdateTimer.Start();
                    }
                }
                else
                {
                    if (labelEthercatStatus != null)
                    {
                        labelEthercatStatus.Text = "EtherCAT: Connection Failed";
                        labelEthercatStatus.ForeColor = Color.FromArgb(244, 67, 54);
                        labelEthercatStatus.BackColor = Color.FromArgb(170, 170, 180);
                    }
                    MessageBox.Show("EtherCAT 연결에 실패했습니다.\n시뮬레이션 모드로 동작합니다.", "연결 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                if (labelEthercatStatus != null)
                {
                    labelEthercatStatus.Text = "EtherCAT: Error";
                    labelEthercatStatus.ForeColor = Color.FromArgb(244, 67, 54);
                    labelEthercatStatus.BackColor = Color.FromArgb(170, 170, 180);
                }
                AddLogMessage($"EtherCAT 연결 오류: {ex.Message}", "ERROR");
                MessageBox.Show($"EtherCAT 연결 중 오류가 발생했습니다: {ex.Message}\n시뮬레이션 모드로 동작합니다.", "연결 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonEthercatDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                // HardwareManager를 통해 EtherCAT 연결 해제 (TM 하드웨어 컨트롤러 종료 포함)
                HardwareManager?.DisconnectEthercat();
                
            IsServoOn = false;  // 서보 상태 초기화
            
            if (labelEthercatStatus != null)
            {
                labelEthercatStatus.Text = "EtherCAT: Disconnected";
                labelEthercatStatus.ForeColor = Color.FromArgb(40, 40, 40);
                labelEthercatStatus.BackColor = Color.FromArgb(170, 170, 180);
            }
            
            // 서보 상태 라벨 업데이트
            UpdateServoStatusLabel();
            
            // 시뮬레이션이 실행 중이 아니면 하드웨어 모니터링 타이머 중지
            if (!SimulationRunning && (SimulationController?.IsTimerRunning ?? false))
            {
                SimulationController?.StopTimer();
            }
            
            // UI 전용 고속 타이머 중지
            if (HardwareUiUpdateTimer != null && HardwareUiUpdateTimer.Enabled)
            {
                HardwareUiUpdateTimer.Stop();
            }
            
            AddLogMessage("EtherCAT 연결 해제됨", "WARN");
            }
            catch (Exception ex)
            {
                AddLogMessage($"EtherCAT 연결 해제 오류: {ex.Message}", "ERROR");
            }
        }

        internal void ReloadRecipeCombo(List<RecipeSnapshot> provided = null)
        {
            var list = provided ?? RecipeRepository.LoadAll();
            if (comboRecipeSelect == null) return;
            var selected = comboRecipeSelect.SelectedItem as string;
            comboRecipeSelect.BeginUpdate();
            comboRecipeSelect.Items.Clear();
            foreach (var r in list.OrderBy(r => r.Name))
            {
                if (!string.IsNullOrWhiteSpace(r.Name))
                    comboRecipeSelect.Items.Add(r.Name);
            }
            comboRecipeSelect.EndUpdate();
            if (!string.IsNullOrEmpty(selected))
            {
                var idx = -1;
                for (int i = 0; i < comboRecipeSelect.Items.Count; i++)
                {
                    if (string.Equals(comboRecipeSelect.Items[i]?.ToString(), selected, StringComparison.Ordinal))
                    {
                        idx = i; break;
                    }
                }
                if (idx >= 0) comboRecipeSelect.SelectedIndex = idx;
            }
        }

        #region Servo Motor Control UI

        /// <summary>
        /// 서보 ON 버튼 클릭
        /// </summary>
        private void buttonServoOn_Click(object sender, EventArgs e)
        {
            if (!EthercatConnected)
            {
                MessageBox.Show("먼저 EtherCAT을 연결해주세요.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 서보 파라미터 설정 (AppSettings에서 읽어옴)
                Int64 velocity = AppSettings.ServoDefaultVelocity;
                Int64 maxVelocity = AppSettings.ServoDefaultMaxVelocity;
                Int64 deceleration = AppSettings.ServoDefaultDeceleration;
                Int64 acceleration = AppSettings.ServoDefaultAcceleration;

                // 서보 OFF 후 파라미터 설정
                EtherCAT_M.Axis1_OFF();
                EtherCAT_M.Axis2_OFF();
                System.Threading.Thread.Sleep(100);

                EtherCAT_M.Axis1_UD_Config_Update(velocity, maxVelocity, deceleration, acceleration);
                EtherCAT_M.Axis2_LR_Config_Update(velocity, maxVelocity, deceleration, acceleration);

                // 서보 ON
                EtherCAT_M.Axis1_ON();
                EtherCAT_M.Axis2_ON();

                IsServoOn = true;  // 서보 ON 상태 플래그 설정
                AddLogMessage("서보모터 ON - 파라미터 설정 완료", "INFO");
                UpdateServoStatusLabel();
            }
            catch (Exception ex)
            {
                AddLogMessage($"서보 ON 오류: {ex.Message}", "ERROR");
                MessageBox.Show($"서보 ON 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 서보 OFF 버튼 클릭
        /// </summary>
        private void buttonServoOff_Click(object sender, EventArgs e)
        {
            if (!EthercatConnected)
            {
                MessageBox.Show("EtherCAT이 연결되지 않았습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                EtherCAT_M.Axis1_OFF();
                EtherCAT_M.Axis2_OFF();

                IsServoOn = false;  // 서보 OFF 상태 플래그 설정
                TmHardwareInitialized = false;  // 원점복귀 상태도 초기화
                AddLogMessage("서보모터 OFF", "INFO");
                UpdateServoStatusLabel();
            }
            catch (Exception ex)
            {
                AddLogMessage($"서보 OFF 오류: {ex.Message}", "ERROR");
                MessageBox.Show($"서보 OFF 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 원점복귀 버튼 클릭
        /// 중요: 상하(Axis1) 원점복귀 → 좌우(Axis2) 원점복귀 순서로 실행
        /// </summary>
        private void buttonServoHome_Click(object sender, EventArgs e)
        {
            if (!EthercatConnected)
            {
                MessageBox.Show("먼저 EtherCAT을 연결해주세요.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 실린더 상태 확인 - 좌우 이동 전 필수
                bool cylinderRetracted = EtherCAT_M.Digital_Input(12); // 후진 센서
                if (!cylinderRetracted)
                {
                    MessageBox.Show("웨이퍼 이송 실린더가 전진되어 있습니다.\n실린더 전진 상태에서는 좌우 이동이 불가합니다.\n먼저 실린더를 후진해주세요.", "안전 경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 비동기로 원점복귀 실행 (UI 블로킹 방지)
                System.Threading.Tasks.Task.Run(() => PerformHomingSequence());
            }
            catch (Exception ex)
            {
                AddLogMessage($"원점복귀 오류: {ex.Message}", "ERROR");
                MessageBox.Show($"원점복귀 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 원점복귀 시퀀스 실행 (상하 → 좌우 순서)
        /// 재시도 로직 포함 (최대 3회)
        /// </summary>
        /// <param name="maxRetries">최대 재시도 횟수 (기본값: 3)</param>
        internal void PerformHomingSequence(int maxRetries = 3)
        {
            int retryCount = 0;
            
            while (retryCount <= maxRetries)
            {
                try
                {
                    if (retryCount > 0)
                    {
                        this.Invoke((Action)(() => {
                            AddLogMessage($"원점복귀 재시도 ({retryCount}/{maxRetries})", "WARN");
                        }));
                        
                        // 재시도 전 대기 (1초)
                        System.Threading.Thread.Sleep(1000);
                        
                        // 재시도 전 상태 확인
                        if (!IsServoOn)
                        {
                            this.Invoke((Action)(() => {
                                AddLogMessage("원점복귀 재시도 실패: 서보가 OFF 상태입니다", "ERROR");
                                MessageBox.Show(
                                    "원점복귀 재시도 중 서보가 OFF 상태가 되었습니다.\n\n" +
                                    "서보를 ON한 후 다시 시도해주세요.",
                                    "원점복귀 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                            return;
                        }
                        
                        // 실린더 상태 확인
                        bool cylinderRetracted = EtherCAT_M.Digital_Input(12);
                        if (!cylinderRetracted)
                        {
                            this.Invoke((Action)(() => {
                                AddLogMessage("원점복귀 재시도 실패: 실린더가 전진 상태입니다", "ERROR");
                                MessageBox.Show(
                                    "원점복귀 재시도 중 실린더가 전진 상태가 되었습니다.\n\n" +
                                    "실린더를 후진한 후 다시 시도해주세요.",
                                    "원점복귀 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                            return;
                        }
                    }
                    
                    this.Invoke((Action)(() => {
                        if (retryCount == 0)
                        {
                            AddLogMessage("원점복귀 시작 - 1단계: 상하(Axis1) 원점복귀", "INFO");
                        }
                        labelServoStatus.Text = "서보: Homing(1/2)...";
                        labelServoStatus.ForeColor = Color.Yellow;
                    }));

                    // 1단계: 상하(Axis1) 원점복귀
                    EtherCAT_M.Axis1_UD_Homming();

                    // Axis1 원점복귀 완료 대기 (최대 120초)
                    var timeout = DateTime.Now.AddSeconds(120);
                    while (DateTime.Now < timeout)
                    {
                        if (EtherCAT_M.Axis1_Status("HOME_D"))
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(100);
                    }

                    if (!EtherCAT_M.Axis1_Status("HOME_D"))
                    {
                        this.Invoke((Action)(() => {
                            AddLogMessage($"상하(Axis1) 원점복귀 타임아웃 (시도 {retryCount + 1}/{maxRetries + 1})", "ERROR");
                            UpdateServoStatusLabel();
                        }));
                        
                        retryCount++;
                        if (retryCount > maxRetries)
                        {
                            this.Invoke((Action)(() => {
                                AddLogMessage("원점복귀 최종 실패: Axis1 타임아웃 (재시도 횟수 초과)", "ERROR");
                                MessageBox.Show(
                                    "원점복귀가 실패했습니다.\n\n" +
                                    "확인 사항:\n" +
                                    "1. 서보 모터 전원 확인\n" +
                                    "2. Axis1 하드웨어 상태 확인\n" +
                                    "3. EtherCAT 연결 상태 확인\n\n" +
                                    "수동으로 원점복귀를 다시 시도해주세요.",
                                    "원점복귀 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                UpdateServoStatusLabel();
                            }));
                            return;
                        }
                        continue; // 재시도
                    }

                    this.Invoke((Action)(() => {
                        AddLogMessage("상하(Axis1) 원점복귀 완료 - 2단계: 좌우(Axis2) 원점복귀", "INFO");
                        labelServoStatus.Text = "서보: Homing(2/2)...";
                    }));

                    // 2단계: 좌우(Axis2) 원점복귀
                    EtherCAT_M.Axis2_LR_Homming();

                    // Axis2 원점복귀 완료 대기 (최대 120초)
                    timeout = DateTime.Now.AddSeconds(120);
                    while (DateTime.Now < timeout)
                    {
                        if (EtherCAT_M.Axis2_Status("HOME_D"))
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(100);
                    }

                    if (!EtherCAT_M.Axis2_Status("HOME_D"))
                    {
                        this.Invoke((Action)(() => {
                            AddLogMessage($"좌우(Axis2) 원점복귀 타임아웃 (시도 {retryCount + 1}/{maxRetries + 1})", "ERROR");
                            UpdateServoStatusLabel();
                        }));
                        
                        retryCount++;
                        if (retryCount > maxRetries)
                        {
                            this.Invoke((Action)(() => {
                                AddLogMessage("원점복귀 최종 실패: Axis2 타임아웃 (재시도 횟수 초과)", "ERROR");
                                MessageBox.Show(
                                    "원점복귀가 실패했습니다.\n\n" +
                                    "확인 사항:\n" +
                                    "1. 서보 모터 전원 확인\n" +
                                    "2. Axis2 하드웨어 상태 확인\n" +
                                    "3. 실린더 후진 상태 확인\n" +
                                    "4. EtherCAT 연결 상태 확인\n\n" +
                                    "수동으로 원점복귀를 다시 시도해주세요.",
                                    "원점복귀 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                UpdateServoStatusLabel();
                            }));
                            return;
                        }
                        continue; // 재시도 (Axis1부터 다시 시작)
                    }

                    // 원점복귀 완료
                    this.Invoke((Action)(() => {
                        if (retryCount > 0)
                        {
                            AddLogMessage($"원점복귀 완료 (재시도 {retryCount}회 후 성공)", "INFO");
                        }
                        else
                        {
                            AddLogMessage("원점복귀 완료 (상하 → 좌우 순서)", "INFO");
                        }
                        
                        // TM 하드웨어 초기화 상태 업데이트 (원점복귀 완료 = 초기화 완료)
                        TmHardwareInitialized = true;
                        
                        // TmHardwareController가 있으면 동기화
                        if (TmHardwareController != null)
                        {
                            // TmHardwareController의 상태도 업데이트 (필요시)
                            // PerformHoming()이 이미 _isHomed = true로 설정했으므로 여기서는 Form1의 상태만 업데이트
                        }

                        UpdateServoStatusLabel();
                    }));
                    return; // 성공
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount > maxRetries)
                    {
                        this.Invoke((Action)(() => {
                            AddLogMessage($"원점복귀 최종 오류: {ex.Message} (재시도 횟수 초과)", "ERROR");
                            MessageBox.Show(
                                $"원점복귀 중 오류가 발생했습니다.\n\n" +
                                $"오류 내용: {ex.Message}\n\n" +
                                $"수동으로 원점복귀를 다시 시도해주세요.",
                                "원점복귀 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            UpdateServoStatusLabel();
                        }));
                        return;
                    }
                    
                    this.Invoke((Action)(() => {
                        AddLogMessage($"원점복귀 오류: {ex.Message} (재시도 예정)", "WARN");
                    }));
                    // 재시도 계속
                }
            }
        }

        /// <summary>
        /// 공정 시작 시 자동으로 서보 ON + 원점복귀 수행
        /// 원점복귀 완료를 기다린 후 공정 시작 (안전성 강화)
        /// </summary>
        private bool PerformAutoServoOnAndHoming()
        {
            try
            {
                // 1. 실린더 후진 상태 확인 (인터락)
                bool cylinderRetracted = EtherCAT_M.Digital_Input(12);
                if (!cylinderRetracted)
                {
                    AddLogMessage("공정 시작 실패: 실린더가 후진 상태가 아닙니다", "ERROR");
                    MessageBox.Show("실린더가 후진 상태가 아닙니다.\n먼저 실린더를 후진시켜 주세요.", "인터락", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // 2. 서보 상태 확인 (하드웨어에서 실제 상태 확인)
                // 서보가 이미 ON 상태면 OFF하지 않음 (불필요한 서보 재시작 방지)
                bool servoAlreadyOn = false;
                bool homingAlreadyDone = false;
                
                try
                {
                    // 하드웨어에서 실제 서보 상태 확인 (위치 데이터)
                    var axis1Pos = EtherCAT_M.Axis1_is_PosData();
                    var axis2Pos = EtherCAT_M.Axis2_is_PosData();
                    bool hasPosition = !string.IsNullOrEmpty(axis1Pos) && axis1Pos != "-" &&
                                      !string.IsNullOrEmpty(axis2Pos) && axis2Pos != "-";
                    
                    // 원점복귀 상태 확인
                    bool axis1Homed = EtherCAT_M.Axis1_Status("HOME_D");
                    bool axis2Homed = EtherCAT_M.Axis2_Status("HOME_D");
                    bool isHomed = axis1Homed && axis2Homed;
                    
                    if (hasPosition)
                    {
                        // 위치 데이터가 있으면 서보가 ON 상태
                        servoAlreadyOn = true;
                        IsServoOn = true; // 플래그 동기화
                        
                        if (isHomed)
                        {
                            // 원점복귀도 완료된 상태
                            homingAlreadyDone = true;
                            TmHardwareInitialized = true; // 플래그 동기화
                            AddLogMessage("서보가 이미 ON 상태이고 원점복귀 완료됨 - 서보 상태 유지, 원점복귀 스킵", "INFO");
                        }
                        else
                        {
                            // 서보는 ON이지만 원점복귀 미완료
                            AddLogMessage("서보가 이미 ON 상태 - 원점복귀만 수행", "INFO");
                        }
                    }
                    else
                    {
                        // 위치 데이터가 없으면 서보가 OFF 상태
                        servoAlreadyOn = false;
                        IsServoOn = false; // 플래그 동기화
                        AddLogMessage("서보가 OFF 상태 - 서보 ON 및 원점복귀 수행", "INFO");
                    }
                }
                catch (Exception ex)
                {
                    // 하드웨어 상태 확인 실패 시 안전하게 OFF 후 재시작
                    AddLogMessage($"서보 상태 확인 오류: {ex.Message} - 서보 초기화 수행", "WARN");
                    servoAlreadyOn = false;
                }
                
                if (!servoAlreadyOn)
                {
                    // 서보 OFF로 초기화 (불안정 상태 방지)
                    AddLogMessage("서보 모터 초기화 중...", "INFO");
                    try
                    {
                        EtherCAT_M.Axis1_OFF();
                        EtherCAT_M.Axis2_OFF();
                        System.Threading.Thread.Sleep(100);
                    }
                    catch (Exception ex)
                    {
                        AddLogMessage($"서보 OFF 오류 (무시): {ex.Message}", "WARN");
                    }
                }

                // 3. 속도 파라미터 설정
                // 원점복귀 서보 파라미터 설정 (AppSettings에서 읽어옴)
                long velocity = AppSettings.ServoHomingVelocity;
                long maxVelocity = AppSettings.ServoHomingMaxVelocity;
                long deceleration = AppSettings.ServoDefaultDeceleration;
                long acceleration = AppSettings.ServoDefaultAcceleration;

                EtherCAT_M.Axis1_UD_Config_Update(velocity, maxVelocity, deceleration, acceleration);
                EtherCAT_M.Axis2_LR_Config_Update(velocity, maxVelocity, deceleration, acceleration);

                // 4. 서보 ON (이미 ON 상태가 아닌 경우에만)
                if (!servoAlreadyOn)
                {
                    AddLogMessage("서보 모터 ON", "INFO");
                    EtherCAT_M.Axis1_ON();
                    EtherCAT_M.Axis2_ON();
                    System.Threading.Thread.Sleep(500); // 서보 안정화 대기
                }
                else
                {
                    // 이미 ON 상태이면 원점복귀만 수행
                    AddLogMessage("서보가 이미 ON 상태 - 원점복귀만 수행", "INFO");
                }
                
                // 서보 ON 상태 확인 (이미 ON 상태가 아닌 경우에만 확인)
                if (!servoAlreadyOn)
                {
                    bool servoOnConfirmed = false;
                    var servoCheckTimeout = DateTime.Now.AddSeconds(2);
                    while (DateTime.Now < servoCheckTimeout)
                    {
                        try
                        {
                            // 위치 데이터가 읽히면 서보가 ON 상태로 간주
                            var axis1Pos = EtherCAT_M.Axis1_is_PosData();
                            var axis2Pos = EtherCAT_M.Axis2_is_PosData();
                            if (!string.IsNullOrEmpty(axis1Pos) && axis1Pos != "-" 
                                && !string.IsNullOrEmpty(axis2Pos) && axis2Pos != "-")
                            {
                                servoOnConfirmed = true;
                                break;
                            }
                        }
                        catch { }
                        System.Threading.Thread.Sleep(100);
                    }
                    
                    if (!servoOnConfirmed)
                    {
                        AddLogMessage("서보 ON 확인 실패: 위치 데이터를 읽을 수 없습니다", "ERROR");
                        IsServoOn = false; // 플래그 동기화
                        return false;
                    }
                    
                    IsServoOn = true;  // 서보 ON 상태 플래그 설정
                    AddLogMessage("서보모터 ON - 파라미터 설정 완료", "INFO");
                }
                else
                {
                    // 이미 ON 상태이면 확인 스킵
                    AddLogMessage("서보가 이미 ON 상태 - 확인 스킵", "INFO");
                }

                // 5. TmHardwareController 서보 상태 동기화 (서보 이동 시 필요)
                if (TmHardwareController != null)
                {
                    TmHardwareController.SyncServoStatus();
                    AddLogMessage("TmHardwareController 서보 상태 동기화 완료", "INFO");
                }
                
                // 6. 원점복귀 수행 (이미 완료된 경우 스킵)
                if (homingAlreadyDone)
                {
                    // 이미 원점복귀가 완료된 상태면 스킵
                    AddLogMessage("원점복귀가 이미 완료됨 - 스킵", "INFO");
                    UpdateServoStatusLabel(); // UI 업데이트
                    // TmHardwareInitialized가 이미 true로 설정되어 있으므로 바로 반환
                    return true;
                }
                
                // 원점복귀가 필요하면 수행 (서보는 이미 ON 상태이거나 방금 ON함)
                
                // 원점복귀 수행 (완료 대기)
                AddLogMessage("공정 시작 원점복귀 - 1단계: 상하(Axis1)", "INFO");
                EtherCAT_M.Axis1_UD_Homming();
                
                // Axis1 원점복귀 완료 대기 (최대 120초)
                var timeout = DateTime.Now.AddSeconds(120);
                while (DateTime.Now < timeout)
                {
                    if (EtherCAT_M.Axis1_Status("HOME_D"))
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                }
                
                if (!EtherCAT_M.Axis1_Status("HOME_D"))
                {
                    AddLogMessage("공정 시작 원점복귀 실패: Axis1 타임아웃 (120초 초과)", "ERROR");
                    TmHardwareInitialized = false;
                    IsServoOn = false; // 타임아웃 시 서보 상태도 초기화
                    return false;
                }
                
                AddLogMessage("공정 시작 원점복귀 - 2단계: 좌우(Axis2)", "INFO");
                EtherCAT_M.Axis2_LR_Homming();
                
                // Axis2 원점복귀 완료 대기 (최대 120초)
                timeout = DateTime.Now.AddSeconds(120);
                while (DateTime.Now < timeout)
                {
                    if (EtherCAT_M.Axis2_Status("HOME_D"))
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                }
                
                if (!EtherCAT_M.Axis2_Status("HOME_D"))
                {
                    AddLogMessage("공정 시작 원점복귀 실패: Axis2 타임아웃 (120초 초과)", "ERROR");
                    TmHardwareInitialized = false;
                    IsServoOn = false; // 타임아웃 시 서보 상태도 초기화
                    return false;
                }
                
                // 원점복귀 완료
                TmHardwareInitialized = true;
                IsServoOn = true; // 서보 ON 상태 확실히 설정
                
                // TmHardwareController 서보 상태 동기화 (서보 이동 시 필요)
                if (TmHardwareController != null)
                {
                    TmHardwareController.SyncServoStatus();
                    AddLogMessage("TmHardwareController 서보 상태 동기화 완료", "INFO");
                }
                
                AddLogMessage("서보 ON + 원점복귀 완료 (상하 → 좌우 순서)", "INFO");
                UpdateServoStatusLabel(); // UI 업데이트 (Home 상태 표시)
                return true;
            }
            catch (Exception ex)
            {
                AddLogMessage($"서보 ON + 원점복귀 오류: {ex.Message}", "ERROR");
                return false;
            }
        }

        /// <summary>
        /// 공정 시작 전 원점복귀 수행
        /// 동기식으로 수행하여 원점복귀 완료 후 공정이 시작되도록 함
        /// </summary>
        private void PerformStartupHoming()
        {
            try
            {
                // 실린더 후진 상태 확인 (인터락)
                bool cylinderRetracted = EtherCAT_M.Digital_Input(12); // 실린더 후진 센서
                if (!cylinderRetracted)
                {
                    AddLogMessage("공정 시작 실패: 실린더가 후진 상태가 아닙니다", "ERROR");
                    MessageBox.Show("실린더가 후진 상태가 아닙니다.\n먼저 실린더를 후진시켜 주세요.", "인터락", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AddLogMessage("공정 시작 원점복귀 - 1단계: 상하(Axis1)", "INFO");

                // 1단계: Axis1 (상하) 원점복귀
                EtherCAT_M.Axis1_UD_Homming();

                // Axis1 원점복귀 완료 대기 (최대 120초)
                var timeout = DateTime.Now.AddSeconds(120);
                while (DateTime.Now < timeout)
                {
                    if (EtherCAT_M.Axis1_Status("HOME_D"))
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                }

                if (!EtherCAT_M.Axis1_Status("HOME_D"))
                {
                    AddLogMessage("공정 시작 원점복귀 실패: Axis1 타임아웃", "ERROR");
                    return;
                }

                AddLogMessage("공정 시작 원점복귀 - 2단계: 좌우(Axis2)", "INFO");

                // 2단계: Axis2 (좌우) 원점복귀
                EtherCAT_M.Axis2_LR_Homming();

                // Axis2 원점복귀 완료 대기 (최대 120초)
                timeout = DateTime.Now.AddSeconds(120);
                while (DateTime.Now < timeout)
                {
                    if (EtherCAT_M.Axis2_Status("HOME_D"))
                    {
                        TmHardwareInitialized = true;
                        AddLogMessage("공정 시작 원점복귀 완료", "INFO");
                        UpdateServoStatusLabel();
                        return;
                    }
                    System.Threading.Thread.Sleep(100);
                }

                AddLogMessage("공정 시작 원점복귀 실패: Axis2 타임아웃", "ERROR");
            }
            catch (Exception ex)
            {
                AddLogMessage($"공정 시작 원점복귀 오류: {ex.Message}", "ERROR");
            }
        }

        /// <summary>
        /// 공정 종료 후 원점복귀 수행
        /// 비동기로 수행하여 UI가 멈추지 않도록 함
        /// </summary>
        internal void PerformShutdownHoming()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    // 실린더 후진 상태 확인
                    bool cylinderRetracted = EtherCAT_M.Digital_Input(12);
                    if (!cylinderRetracted)
                    {
                        this.Invoke((Action)(() => {
                            AddLogMessage("공정 종료 원점복귀 실패: 실린더가 후진 상태가 아닙니다", "WARN");
                        }));
                        return;
                    }

                    this.Invoke((Action)(() => {
                        AddLogMessage("공정 종료 원점복귀 - 1단계: 상하(Axis1)", "INFO");
                    }));

                    // 1단계: Axis1 (상하) 원점복귀
                    EtherCAT_M.Axis1_UD_Homming();

                    var timeout = DateTime.Now.AddSeconds(120);
                    while (DateTime.Now < timeout)
                    {
                        if (EtherCAT_M.Axis1_Status("HOME_D"))
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(100);
                    }

                    if (!EtherCAT_M.Axis1_Status("HOME_D"))
                    {
                        this.Invoke((Action)(() => {
                            AddLogMessage("공정 종료 원점복귀 실패: Axis1 타임아웃", "WARN");
                        }));
                        return;
                    }

                    this.Invoke((Action)(() => {
                        AddLogMessage("공정 종료 원점복귀 - 2단계: 좌우(Axis2)", "INFO");
                    }));

                    // 2단계: Axis2 (좌우) 원점복귀
                    EtherCAT_M.Axis2_LR_Homming();

                    timeout = DateTime.Now.AddSeconds(120);
                    while (DateTime.Now < timeout)
                    {
                        if (EtherCAT_M.Axis2_Status("HOME_D"))
                        {
                            this.Invoke((Action)(() => {
                                // TM 하드웨어 초기화 상태 업데이트
                                TmHardwareInitialized = true;
                                AddLogMessage("공정 종료 원점복귀 완료 - TM이 원점으로 복귀했습니다", "INFO");
                                UpdateServoStatusLabel();
                            }));
                            return;
                        }
                        System.Threading.Thread.Sleep(100);
                    }

                    this.Invoke((Action)(() => {
                        AddLogMessage("공정 종료 원점복귀 실패: Axis2 타임아웃", "WARN");
                    }));
                }
                catch (Exception ex)
                {
                    this.Invoke((Action)(() => {
                        AddLogMessage($"공정 종료 원점복귀 오류: {ex.Message}", "WARN");
                    }));
                }
            });
        }

        /// <summary>
        /// 서보 상태 라벨 업데이트
        /// </summary>
        internal void UpdateServoStatusLabel()
        {
            uiUpdater?.UpdateServoStatusLabel();
        }

        #endregion

        #region TM Hardware Control Methods

        /// <summary>
        /// 실제 서보 위치를 읽어서 TM 시각화 업데이트
        /// </summary>
        internal void UpdateTmVisualizationFromHardware()
        {
            uiUpdater?.UpdateTmVisualizationFromHardware();
        }

        /// <summary>
        /// 서보 위치를 Region으로 변환 (더 정확한 판별)
        /// </summary>
        // DetermineRegionFromPosition은 EquipmentRegionHelper로 이동됨

        /// <summary>
        /// TM 하드웨어 컨트롤러 초기화
        /// </summary>
        internal void InitializeTmHardwareController()
        {
            // HardwareManager를 통해 TM 하드웨어 컨트롤러 초기화
            HardwareManager?.InitializeTmHardwareController();
        }

        /// <summary>
        /// TM 하드웨어 초기화 및 원점복귀 수행
        /// </summary>
        private bool InitializeAndHomeTmHardware()
        {
            // HardwareManager를 통해 TM 하드웨어 초기화 및 원점복귀
            return HardwareManager?.InitializeAndHomeTmHardware() ?? false;
        }

        /// <summary>
        /// TM 하드웨어 컨트롤러 종료
        /// </summary>
        private void ShutdownTmHardwareController()
        {
            // HardwareManager를 통해 TM 하드웨어 컨트롤러 종료
            HardwareManager?.ShutdownTmHardwareController();
        }

        /// <summary>
        /// TM 하드웨어 모드 사용 가능 여부 확인
        /// </summary>
        /// <summary>
        /// 하드웨어 모드 사용 가능 여부 확인
        /// 중요: EtherCAT 연결 상태만으로 판단 (원점복귀 여부와 무관)
        /// </summary>
        internal bool IsTmHardwareModeAvailable()
        {
            return EthercatConnected && EtherCAT_M != null;
        }

        /// <summary>
        /// TM 하드웨어 동작 시작 (비동기 방식)
        /// </summary>
        internal void StartTmHardwareAction()
        {
            TmHardwareActionPending = true;
            tmHardwareActionStartTime = DateTime.Now;
        }

        /// <summary>
        /// TM 하드웨어 동작 완료 확인 (PP_D + 시간 기반)
        /// X/Y 축 동시 이동이므로 더 긴 타임아웃 적용
        /// 중요: 이동 시작 직후에는 PP_D가 이전 상태로 남아있을 수 있으므로 최소 대기 시간 필요
        /// </summary>
        // 서보 XY 동시 이동 타임아웃 및 최소 대기 시간 (AppSettings에서 읽어옴)
        private static int SERVO_XY_MOVE_TIMEOUT_MS => AppSettings.ServoXyMoveTimeoutMs;  // X+Y 동시 이동 최대 시간
        private static int SERVO_MOVE_MIN_WAIT_MS => AppSettings.ServoMoveMinWaitMs;     // PP_D 체크 전 최소 대기 시간
        
        internal bool CheckTmHardwareActionComplete()
        {
            if (TmHardwareController == null) return true;
            
            // 서보 이동 완료 확인 (PP_D 상태)
            try
            {
                double elapsedMs = 0;
                if (TmHardwareActionPending)
                {
                    elapsedMs = (DateTime.Now - tmHardwareActionStartTime).TotalMilliseconds;
                    
                    // 중요: 이동 시작 후 최소 대기 시간이 지나야 PP_D 체크 시작
                    // 이전 이동의 PP_D=true 상태가 새 이동을 즉시 완료로 판정하는 문제 방지
                    if (elapsedMs < SERVO_MOVE_MIN_WAIT_MS)
                    {
                        return false;  // 최소 대기 시간 미경과 - 아직 이동 중으로 간주
                    }
                }
                
                bool axis1Done = EtherCAT_M.Axis1_Status("PP_D");
                bool axis2Done = EtherCAT_M.Axis2_Status("PP_D");
                
                // PP_D가 둘 다 true면 완료 (최소 대기 시간 이후에만 체크)
                if (axis1Done && axis2Done)
                {
                    return true;
                }
                
                // 시간 기반 완료 체크: 서보 이동 시작 후 5초가 지나면 타임아웃 처리
                if (TmHardwareActionPending && elapsedMs >= SERVO_XY_MOVE_TIMEOUT_MS)
                {
                    // 실제 위치 확인 (TmHardwareController를 통해)
                    if (TmHardwareController != null)
                    {
                        TmHardwareController.UpdateCurrentPositions();
                        long currentX = TmHardwareController.CurrentAxis2Position;
                        long currentY = TmHardwareController.CurrentAxis1Position;
                        AddLogMessage($"서보 이동 타임아웃 ({elapsedMs:F0}ms) - 현재 위치: X={currentX}, Y={currentY}", "WARN");
                    }
                    else
                    {
                        AddLogMessage($"서보 이동 타임아웃 ({elapsedMs:F0}ms) - 위치 확인 불가", "WARN");
                    }
                    
                    // 타임아웃 발생 시에도 완료로 처리 (기존 동작 유지)
                    // 하지만 경고 로그를 남겨서 문제 추적 가능하도록 함
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                AddLogMessage($"PP_D 상태 확인 오류: {ex.Message}", "WARN");
                return false;  // 오류 시 완료가 아닌 대기로 처리 (안전)
            }
        }

        /// <summary>
        /// TM 하드웨어 동작 타임아웃 체크
        /// </summary>
        internal bool CheckTmHardwareTimeout()
        {
            if (!TmHardwareActionPending) return false;
            return (DateTime.Now - tmHardwareActionStartTime).TotalMilliseconds > AppSettings.TmHardwareActionTimeoutMs;
        }

        /// <summary>
        /// 하드웨어 오류 발생 시 자동 일시정지 (안전장치)
        /// </summary>
        internal void HandleHardwareError(string errorMessage, string errorType = "ERROR")
        {
            if (!IsTmHardwareModeAvailable()) return;

            AddLogMessage($"[하드웨어 오류] {errorMessage} - 자동 일시정지", errorType);
            
            // 자동 일시정지
            if (SimulationRunning && !SimulationPaused)
            {
                PauseSimulation();
                AddLogMessage("하드웨어 오류로 인한 자동 일시정지", "WARN");
                
                // 사용자에게 알림
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show(
                            $"하드웨어 오류가 발생하여 공정이 일시정지되었습니다.\n\n" +
                            $"오류 내용: {errorMessage}\n\n" +
                            $"확인 사항:\n" +
                            $"1. EtherCAT 연결 상태 확인\n" +
                            $"2. 서보 모터 상태 확인\n" +
                            $"3. 실린더 상태 확인\n" +
                            $"4. 장비 상태 점검\n\n" +
                            $"문제 해결 후 '공정 재개' 버튼을 눌러주세요.",
                            "하드웨어 오류 - 자동 일시정지",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }));
                }
                else
                {
                    MessageBox.Show(
                        $"하드웨어 오류가 발생하여 공정이 일시정지되었습니다.\n\n" +
                        $"오류 내용: {errorMessage}\n\n" +
                        $"확인 사항:\n" +
                        $"1. EtherCAT 연결 상태 확인\n" +
                        $"2. 서보 모터 상태 확인\n" +
                        $"3. 실린더 상태 확인\n" +
                        $"4. 장비 상태 점검\n\n" +
                        $"문제 해결 후 '공정 재개' 버튼을 눌러주세요.",
                        "하드웨어 오류 - 자동 일시정지",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }
        
        /// <summary>
        /// Phase별 안정화 대기 시간 반환
        /// </summary>
        internal int GetSettleDelayForPhase(TransferController.TmPhase phase)
        {
            switch (phase)
            {
                // 서보 이동 완료 후
                case TransferController.TmPhase.MoveToPickup_WaitHardware:
                case TransferController.TmPhase.MoveToDropoff_WaitHardware:
                case TransferController.TmPhase.PickupExtend_ServoDown:
                case TransferController.TmPhase.PickupRetract_ServoUp:
                case TransferController.TmPhase.DropoffExtend_ServoDown:
                case TransferController.TmPhase.DropoffRetract_ServoUp:
                    return AppSettings.ServoSettleDelayMs;
                
                // 실린더 동작 완료 후
                case TransferController.TmPhase.PickupExtend_CylinderForward:
                case TransferController.TmPhase.PickupRetract_CylinderBackward:
                case TransferController.TmPhase.DropoffExtend_CylinderForward:
                case TransferController.TmPhase.DropoffRetract_CylinderBackward:
                    return AppSettings.CylinderSettleDelayMs;
                
                // 진공 ON 후
                case TransferController.TmPhase.PickupExtend_VacuumOn:
                    return AppSettings.VacuumOnSettleDelayMs;
                
                // 진공 OFF/배기 후
                case TransferController.TmPhase.DropoffExtend_VacuumOffExhaust:
                    return AppSettings.VacuumOffSettleDelayMs;
                
                // 도어 동작 완료 후
                case TransferController.TmPhase.WaitDoorPickupOpen:
                case TransferController.TmPhase.WaitDoorPickupClose:
                case TransferController.TmPhase.WaitDoorDropoffOpen:
                case TransferController.TmPhase.WaitDoorDropoffClose:
                    return AppSettings.DoorSettleDelayMs;
                
                default:
                    return 0;
            }
        }

        /// <summary>
        /// TM 서보 이동 명령 발행 - 픽업 위치 (안착 높이)
        /// 웨이퍼를 꺼내기 위해 안착 위치로 이동
        /// </summary>
        internal bool StartTmServoMoveForPickup(EquipmentRegion targetRegion)
        {
            if (!IsTmHardwareModeAvailable()) return false;

            try
            {
                // FOUP 이동 시 현재 층 설정
                UpdateTmFoupSlotForRegion(targetRegion);

                var result = TmHardwareController.MoveToRegionForPickup(targetRegion, waitForCompletion: false);
                if (result.Success)
                {
                    // 서보 이동 시 home 상태 해제
                    if (TmHardwareInitialized)
                    {
                        TmHardwareInitialized = false;
                        UpdateServoStatusLabel();
                    }
                    StartTmHardwareAction();
                    AddLogMessage($"TM 서보 이동 시작 (픽업): {targetRegion} → 안착 높이", "INFO");
                    return true;
                }
                else
                {
                    HandleHardwareError($"TM 서보 이동 실패: {result.ErrorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleHardwareError($"TM 서보 이동 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// TM 서보 이동 명령 발행 - 드롭 위치 (상승 높이)
        /// 웨이퍼를 놓기 위해 상승 위치로 이동
        /// </summary>
        internal bool StartTmServoMoveForDropoff(EquipmentRegion targetRegion)
        {
            if (!IsTmHardwareModeAvailable()) return false;

            try
            {
                // FOUP 이동 시 현재 층 설정
                UpdateTmFoupSlotForRegion(targetRegion);

                var result = TmHardwareController.MoveToRegionForDropoff(targetRegion, waitForCompletion: false);
                if (result.Success)
                {
                    // 서보 이동 시 home 상태 해제
                    if (TmHardwareInitialized)
                    {
                        TmHardwareInitialized = false;
                        UpdateServoStatusLabel();
                    }
                    StartTmHardwareAction();
                    AddLogMessage($"TM 서보 이동 시작 (드롭): {targetRegion} → 상승 높이", "INFO");
                    return true;
                }
                else
                {
                    HandleHardwareError($"TM 서보 이동 실패: {result.ErrorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleHardwareError($"TM 서보 이동 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// FOUP 이동 시 현재 층 설정
        /// </summary>
        private void UpdateTmFoupSlotForRegion(EquipmentRegion region)
        {
            uiUpdater?.UpdateTmFoupSlotForRegion(region);
        }

        /// <summary>
        /// TM 실린더 전진 명령 발행 (비동기)
        /// </summary>
        internal bool StartTmCylinderExtend()
        {
            if (!IsTmHardwareModeAvailable()) return false;

            try
            {
                // 실린더 전진 명령
                EtherCAT_M.Digital_Output(13, false);
                EtherCAT_M.Digital_Output(12, true);
                StartTmHardwareAction();
                AddLogMessage("TM 실린더 전진 시작", "INFO");
                return true;
            }
            catch (Exception ex)
            {
                HandleHardwareError($"TM 실린더 전진 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// TM 실린더 후진 명령 발행 (비동기)
        /// </summary>
        internal bool StartTmCylinderRetract()
        {
            if (!IsTmHardwareModeAvailable()) return false;

            try
            {
                // 실린더 후진 명령
                EtherCAT_M.Digital_Output(12, false);
                EtherCAT_M.Digital_Output(13, true);
                StartTmHardwareAction();
                AddLogMessage("TM 실린더 후진 시작", "INFO");
                return true;
            }
            catch (Exception ex)
            {
                HandleHardwareError($"TM 실린더 후진 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// TM 실린더 전진 완료 확인
        /// </summary>
        internal bool CheckTmCylinderExtended()
        {
            if (!IsTmHardwareModeAvailable()) return true;

            try
            {
                return EtherCAT_M.Digital_Input(13); // 전진 센서
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// TM 실린더 후진 완료 확인
        /// </summary>
        internal bool CheckTmCylinderRetracted()
        {
            if (!IsTmHardwareModeAvailable()) return true;

            try
            {
                return EtherCAT_M.Digital_Input(12); // 후진 센서
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// TM 서보 Axis1 하강 (웨이퍼 픽업/드롭 위치로)
        /// </summary>
        internal bool StartTmServoDown(EquipmentRegion region)
        {
            if (!IsTmHardwareModeAvailable()) return false;

            try
            {
                var targetY = TmHardwareController.GetRegionLandHeight(region);
                var result = TmHardwareController.MoveAxis1(targetY, waitForCompletion: false);
                if (result.Success)
                {
                    // 서보 이동 시 home 상태 해제
                    if (TmHardwareInitialized)
                    {
                        TmHardwareInitialized = false;
                        UpdateServoStatusLabel();
                    }
                    StartTmHardwareAction();
                    AddLogMessage($"TM 서보 하강 시작: {region}", "INFO");
                    return true;
                }
                else
                {
                    HandleHardwareError($"TM 서보 하강 실패: {result.ErrorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleHardwareError($"TM 서보 하강 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// TM 서보 Axis1 상승 (해당 Region의 상승 높이로)
        /// </summary>
        internal bool StartTmServoUp(EquipmentRegion region)
        {
            if (!IsTmHardwareModeAvailable()) return false;

            try
            {
                var targetY = TmHardwareController.GetRegionRaiseHeight(region);
                var result = TmHardwareController.MoveAxis1(targetY, waitForCompletion: false);
                if (result.Success)
                {
                    // 서보 이동 시 home 상태 해제
                    if (TmHardwareInitialized)
                    {
                        TmHardwareInitialized = false;
                        UpdateServoStatusLabel();
                    }
                    StartTmHardwareAction();
                    AddLogMessage("TM 서보 상승 시작", "INFO");
                    return true;
                }
                else
                {
                    HandleHardwareError($"TM 서보 상승 실패: {result.ErrorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleHardwareError($"TM 서보 상승 오류: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// TM 서보 하강 (하강 위치로 이동) - 웨이퍼 아래로
        /// 드롭 후 실린더 후진 전에 호출
        /// </summary>
        internal bool StartTmServoToDescend(EquipmentRegion region)
        {
            if (!IsTmHardwareModeAvailable()) return false;

            try
            {
                var targetY = TmHardwareController.GetRegionDescendHeight(region);
                var result = TmHardwareController.MoveAxis1(targetY, waitForCompletion: false);
                if (result.Success)
                {
                    // 서보 이동 시 home 상태 해제
                    if (TmHardwareInitialized)
                    {
                        TmHardwareInitialized = false;
                        UpdateServoStatusLabel();
                    }
                    StartTmHardwareAction();
                    AddLogMessage($"TM 서보 하강 시작 (하강 위치): {region}", "INFO");
                    return true;
                }
                else
                {
                    HandleHardwareError($"TM 서보 하강 실패: {result.ErrorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleHardwareError($"TM 서보 하강 오류: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// TM 서보 상승 (안착 위치로 이동) - 픽업 시 웨이퍼와 접촉
        /// 픽업 시 하강 위치에서 안착 위치로 이동
        /// </summary>
        internal bool StartTmServoToLand(EquipmentRegion region)
        {
            if (!IsTmHardwareModeAvailable()) return false;

            try
            {
                var targetY = TmHardwareController.GetRegionLandHeight(region);
                var result = TmHardwareController.MoveAxis1(targetY, waitForCompletion: false);
                if (result.Success)
                {
                    // 서보 이동 시 home 상태 해제
                    if (TmHardwareInitialized)
                    {
                        TmHardwareInitialized = false;
                        UpdateServoStatusLabel();
                    }
                    StartTmHardwareAction();
                    AddLogMessage($"TM 서보 이동 시작 (안착 위치): {region}", "INFO");
                    return true;
                }
                else
                {
                    HandleHardwareError($"TM 서보 이동 실패: {result.ErrorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleHardwareError($"TM 서보 이동 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// TM Axis1 이동 완료 확인 (PP_D + 시간 기반)
        /// 실제 서보는 2~3초 내에 이동 완료하지만 PP_D가 일시적으로만 true가 되는 문제 해결
        /// 중요: 이동 시작 직후에는 PP_D가 이전 상태로 남아있을 수 있으므로 최소 대기 시간 필요
        /// </summary>
        // 서보 이동 타임아웃 및 최소 대기 시간 (AppSettings에서 읽어옴)
        private static int SERVO_MOVE_TIMEOUT_MS => AppSettings.ServoMoveTimeoutMs;          // 서보 이동 최대 시간
        private static int SERVO_AXIS1_MIN_WAIT_MS => AppSettings.ServoAxis1MinWaitMs;         // Axis1 PP_D 체크 전 최소 대기 시간
        
        internal bool CheckTmAxis1MoveComplete()
        {
            if (!IsTmHardwareModeAvailable()) return true;

            try
            {
                double elapsedMs = 0;
                if (TmHardwareActionPending)
                {
                    elapsedMs = (DateTime.Now - tmHardwareActionStartTime).TotalMilliseconds;
                    
                    // 중요: 이동 시작 후 최소 대기 시간이 지나야 PP_D 체크 시작
                    if (elapsedMs < SERVO_AXIS1_MIN_WAIT_MS)
                    {
                        return false;  // 최소 대기 시간 미경과 - 아직 이동 중으로 간주
                    }
                }
                
                // PP_D가 true면 완료 (최소 대기 시간 이후에만 체크)
                if (EtherCAT_M.Axis1_Status("PP_D"))
                {
                    return true;
                }
                
                // 시간 기반 완료 체크: 서보 이동 시작 후 3초가 지나면 완료로 처리
                if (TmHardwareActionPending && elapsedMs >= SERVO_MOVE_TIMEOUT_MS)
                {
                    return true;
                }
                
                return false;
            }
            catch
            {
                return false;  // 오류 시 대기 (안전)
            }
        }

        /// <summary>
        /// TM 진공 ON (웨이퍼 흡착)
        /// </summary>
        internal bool SetTmVacuumOn()
        {
            if (!IsTmHardwareModeAvailable()) return false;

            try
            {
                EtherCAT_M.Digital_Output(15, false); // 배기 OFF
                EtherCAT_M.Digital_Output(14, true);  // 흡기 ON
                AddLogMessage("TM 진공 흡기 ON", "INFO");
                return true;
            }
            catch (Exception ex)
            {
                HandleHardwareError($"TM 진공 ON 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// TM 진공 OFF + 배기 (웨이퍼 놓기)
        /// </summary>
        internal bool SetTmVacuumOffAndExhaust()
        {
            if (!IsTmHardwareModeAvailable()) return false;

            try
            {
                EtherCAT_M.Digital_Output(14, false); // 흡기 OFF
                EtherCAT_M.Digital_Output(15, true);  // 배기 ON
                StartTmHardwareAction();
                AddLogMessage("TM 진공 OFF + 배기 시작", "INFO");
                return true;
            }
            catch (Exception ex)
            {
                HandleHardwareError($"TM 진공 OFF 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// TM 배기 완료 처리 (배기 OFF)
        /// </summary>
        internal void CompleteTmExhaust()
        {
            if (!IsTmHardwareModeAvailable()) return;

            try
            {
                EtherCAT_M.Digital_Output(15, false); // 배기 OFF
                AddLogMessage("TM 배기 완료", "INFO");
            }
            catch (Exception ex)
            {
                AddLogMessage($"TM 배기 완료 오류: {ex.Message}", "WARN");
            }
        }

        #endregion

        #region GroupBox Custom Paint

        /// <summary>
        /// GroupBox 테두리를 검은색으로 그리기
        /// </summary>
        private void GroupBox_Paint(object sender, PaintEventArgs e)
        {
            var groupBox = sender as GroupBox;
            if (groupBox == null) return;

            // GroupBox의 텍스트 영역 계산
            var textSize = e.Graphics.MeasureString(groupBox.Text, groupBox.Font);
            var textX = groupBox.Padding.Left;
            var textWidth = textSize.Width;
            var textHeight = textSize.Height;

            // 테두리 그리기
            using (var pen = new Pen(Color.Black, 1))
            {
                // 상단 선 (텍스트 왼쪽)
                e.Graphics.DrawLine(pen, 0, textHeight / 2, textX, textHeight / 2);
                
                // 상단 선 (텍스트 오른쪽)
                e.Graphics.DrawLine(pen, textX + textWidth + 5, textHeight / 2, groupBox.Width - 1, textHeight / 2);
                
                // 좌측 선
                e.Graphics.DrawLine(pen, 0, textHeight / 2, 0, groupBox.Height - 1);
                
                // 우측 선
                e.Graphics.DrawLine(pen, groupBox.Width - 1, textHeight / 2, groupBox.Width - 1, groupBox.Height - 1);
                
                // 하단 선
                e.Graphics.DrawLine(pen, 0, groupBox.Height - 1, groupBox.Width - 1, groupBox.Height - 1);
            }
        }

        #endregion

        #region Tab and Navigation Button States

        /// <summary>
        /// 상단 탭 버튼 상태 업데이트 (선택된 버튼은 더 밝게)
        /// </summary>
        internal void UpdateTabButtonStates(string selectedTab)
        {
            uiUpdater?.UpdateTabButtonStates(selectedTab);
        }

        /// <summary>
        /// 하단 네비게이션 버튼 상태 업데이트 (선택된 버튼은 더 밝게)
        /// </summary>
        internal void UpdateNavButtonStates(string selectedNav)
        {
            uiUpdater?.UpdateNavButtonStates(selectedNav);
        }

        private void ButtonTabMain_Click(object sender, EventArgs e)
        {
            UpdateTabButtonStates("Main");
            NavigateToSection(AppSection.Main);
        }

        private void ButtonTabVerification_Click(object sender, EventArgs e)
        {
            UpdateTabButtonStates("Verification");
            NavigateToSection(AppSection.Verification);
        }

        private void ButtonTabTransfer_Click(object sender, EventArgs e)
        {
            UpdateTabButtonStates("Transfer");
            NavigateToSection(AppSection.Transfer);
        }

        #endregion
    }
}

