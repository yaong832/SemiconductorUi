# 코드 상세 설명 - Form1.cs

**파일 경로:** `SemiconductorUi/Form1.cs`  
**크기:** 약 7,820줄  
**역할:** 메인 폼, 전체 시스템 조율 및 UI 관리

---

## 1. 개요

`Form1.cs`는 반도체 장비 제어 시스템의 메인 폼으로, 전체 시스템을 조율하고 UI를 관리하는 핵심 클래스입니다. WinForms 기반 애플리케이션의 진입점 역할을 하며, 모든 컨트롤러, 서비스, 헬퍼 클래스를 통합하여 동작합니다.

### 1.1 주요 책임

1. **UI 관리**: 메인 폼의 모든 UI 요소 관리
2. **이벤트 처리**: 사용자 입력 및 시스템 이벤트 처리
3. **시스템 조율**: 컨트롤러, 서비스, 헬퍼 클래스 간 조율
4. **상태 관리**: 공정 상태, 웨이퍼 상태, 하드웨어 상태 관리
5. **타이머 관리**: 시뮬레이션 타이머 및 UI 업데이트 타이머 관리

---

## 2. 클래스 구조

### 2.1 클래스 선언

```csharp
public partial class Form1 : Form
{
    // ProcessState와 WaferLoadStateType는 ViewModel의 enum을 직접 사용
    using ProcessState = ViewModels.MainFormViewModel.ProcessState;
    using MainFormViewModel = ViewModels.MainFormViewModel;
}
```

### 2.2 주요 필드 및 속성

#### 2.2.1 서비스 인터페이스
```csharp
internal IChamberService ChamberService;
internal ITransferService TransferService;
internal ISimulationService SimulationService;
internal ILoggerService LoggerService;
```

#### 2.2.2 ViewModel
```csharp
internal MainFormViewModel ViewModel;
```
- UI 상태를 관리하는 ViewModel
- MVVM 패턴 적용

#### 2.2.3 하드웨어 관리
```csharp
internal HardwareManager HardwareManager;
internal IEG3268 EtherCAT_M;
```

#### 2.2.4 이벤트 핸들러
```csharp
private readonly LoginEventHandlers loginEventHandlers;
private readonly ProcessEventHandlers processEventHandlers;
private readonly WaferEventHandlers waferEventHandlers;
private readonly NavigationEventHandlers navigationEventHandlers;
private readonly EquipmentEventHandlers equipmentEventHandlers;
private readonly HardwareEventHandlers hardwareEventHandlers;
private readonly TimerEventHandlers timerEventHandlers;
private readonly PaintEventHandlers paintEventHandlers;
private readonly FormEventHandlers formEventHandlers;
```

#### 2.2.5 Helper 클래스
```csharp
internal Helpers.Form1UiUpdater uiUpdater;
internal Helpers.Form1Initializer uiInitializer;
internal Helpers.Form1Configurator uiConfigurator;
internal Helpers.Form1TmProcessor tmProcessor;
internal Helpers.FoupManager foupManager;
internal Helpers.AlarmManager alarmManager;
internal Helpers.WaferTrackingService waferTrackingService;
```

#### 2.2.6 챔버 상태
```csharp
internal ChamberController.ChamberState ChamberAState;
internal ChamberController.ChamberState ChamberBState;
internal ChamberController.ChamberState ChamberCState;
```

#### 2.2.7 타이머
```csharp
internal readonly Timer HeaderClockTimer;        // 헤더 시계 타이머 (1초 주기)
internal readonly Timer HardwareUiUpdateTimer;   // UI 업데이트 타이머 (50ms 주기)
```

---

## 3. 생성자 및 초기화

### 3.1 생성자 구조

```csharp
public Form1()
{
    InitializeComponent();
    
    // 1. ViewModel 초기화
    ViewModel = new MainFormViewModel();
    
    // 2. 하드웨어 관리 초기화
    EtherCAT_M = new IEG3268();
    HardwareManager = new HardwareManager(EtherCAT_M, AddLogMessage);
    
    // 3. 이벤트 핸들러 초기화
    loginEventHandlers = new LoginEventHandlers(this);
    processEventHandlers = new ProcessEventHandlers(this);
    // ... 기타 이벤트 핸들러들
    
    // 4. Helper 클래스 초기화
    uiUpdater = new Helpers.Form1UiUpdater(this);
    uiInitializer = new Helpers.Form1Initializer(this);
    uiConfigurator = new Helpers.Form1Configurator(this);
    tmProcessor = new Helpers.Form1TmProcessor(this);
    foupManager = new Helpers.FoupManager(this);
    alarmManager = new Helpers.AlarmManager(this);
    waferTrackingService = new Helpers.WaferTrackingService(this);
    
    // 5. UI 초기화
    uiInitializer.InitializeCustomControls();
    uiInitializer.InitializeFoupMountButtons();
    uiInitializer.InitializeWaferOverlays();
    uiConfigurator.LayoutCentralEquipment();
    
    // 6. 타이머 초기화
    HeaderClockTimer = new Timer();
    HeaderClockTimer.Interval = 1000;
    HeaderClockTimer.Tick += timerEventHandlers.HeaderClockTimer_Tick;
    HeaderClockTimer.Start();
    
    HardwareUiUpdateTimer = new Timer();
    HardwareUiUpdateTimer.Interval = 50; // 50ms 주기
    HardwareUiUpdateTimer.Tick += timerEventHandlers.HardwareUiUpdateTimer_Tick;
    
    // 7. 시뮬레이션 상태 초기화
    uiInitializer.InitializeSimulationState();
    
    // 8. 초기 상태 설정
    SetProcessState(ProcessState.Idle, "시스템 초기화 완료.");
    UpdateSimulationUi();
    SetHeaderAlarmIdle();
}
```

### 3.2 초기화 순서

1. **컴포넌트 초기화**: `InitializeComponent()` 호출
2. **ViewModel 초기화**: UI 상태 관리 객체 생성
3. **하드웨어 초기화**: EtherCAT 및 HardwareManager 생성
4. **이벤트 핸들러 초기화**: 각 이벤트 핸들러 인스턴스 생성
5. **Helper 클래스 초기화**: 책임 분리된 Helper 클래스들 생성
6. **UI 초기화**: 커스텀 컨트롤, FOUP 버튼, 웨이퍼 오버레이 등 초기화
7. **타이머 초기화**: 시계 타이머 및 UI 업데이트 타이머 설정
8. **시뮬레이션 상태 초기화**: 공정 상태 초기화
9. **초기 상태 설정**: Idle 상태로 설정

---

## 4. 주요 메서드

### 4.1 공정 제어 메서드

#### 4.1.1 공정 시작
```csharp
internal void StartSimulation()
{
    // 1. 레시피 검증
    // 2. FOUP A 웨이퍼 로드 확인
    // 3. 시뮬레이션 컨트롤러 시작
    // 4. 상태를 Running으로 변경
}
```

#### 4.1.2 공정 일시정지
```csharp
internal void PauseSimulation()
{
    // 1. 시뮬레이션 컨트롤러 일시정지
    // 2. 상태를 Paused로 변경
}
```

#### 4.1.3 공정 재개
```csharp
internal void ResumeSimulation()
{
    // 1. 시뮬레이션 컨트롤러 재개
    // 2. 상태를 Running으로 변경
}
```

#### 4.1.4 공정 중지
```csharp
internal void StopSimulation()
{
    // 1. 시뮬레이션 컨트롤러 중지
    // 2. 상태를 Idle로 변경
    // 3. 챔버 상태 초기화
}
```

### 4.2 웨이퍼 이송 메서드

#### 4.2.1 FOUP A에서 로드
```csharp
internal void TryScheduleLoadFromFoupA()
{
    // 1. FOUP A에 웨이퍼가 있는지 확인
    // 2. Chamber A가 비어있는지 확인
    // 3. 이송 작업 스케줄링
}
```

#### 4.2.2 챔버 간 이송
```csharp
internal void TryScheduleTransferFromChamberA()
{
    // 1. Chamber A 공정 완료 확인
    // 2. Chamber B 또는 C 비어있는지 확인
    // 3. 병렬 가공 모드 또는 2차 노광 모드에 따라 이송 스케줄링
}
```

#### 4.2.3 FOUP B로 언로드
```csharp
internal void TryScheduleTransferToFoupB(ChamberController.ChamberState source)
{
    // 1. 챔버 공정 완료 확인
    // 2. FOUP B 여유 공간 확인
    // 3. 이송 작업 스케줄링
}
```

### 4.3 TM 처리 메서드

#### 4.3.1 TM 처리 (메인)
```csharp
internal void ProcessTm()
{
    // 1. 하드웨어 모드와 시뮬레이션 모드 분기
    // 2. 현재 Phase에 따라 처리
    // 3. Phase 전환 처리
    // 4. UI 업데이트
}
```

#### 4.3.2 TM 픽업 처리
```csharp
internal bool PerformPickup()
{
    // 1. 픽업 위치로 이동
    // 2. 도어 열기
    // 3. 실린더 전진
    // 4. 서보 하강
    // 5. 진공 ON
    // 6. 서보 상승
    // 7. 실린더 후진
    // 8. 도어 닫기
}
```

#### 4.3.3 TM 드롭오프 처리
```csharp
internal bool PerformDropoff()
{
    // 1. 드롭오프 위치로 이동
    // 2. 도어 열기
    // 3. 실린더 전진
    // 4. 서보 하강
    // 5. 진공 OFF + 배기
    // 6. 서보 하강 (하강 위치)
    // 7. 실린더 후진
    // 8. 도어 닫기
    // 9. OnCompleted 콜백 호출
}
```

### 4.4 챔버 공정 메서드

#### 4.4.1 공정 시간 감소
```csharp
internal void DecrementChamberTime(ChamberController.ChamberState chamber)
{
    // 1. 도어 상태 확인
    // 2. 하드웨어 모드: 실제 경과 시간 기반
    // 3. 시뮬레이션 모드: 틱 기반
    // 4. RemainingSeconds 감소
}
```

#### 4.4.2 챔버 상태 확인
```csharp
internal bool IsChamberAvailable(ChamberController.ChamberState chamber)
{
    // 1. 웨이퍼 없음 확인
    // 2. 예약 상태 확인
    // 3. 픽업 스케줄 확인
}
```

### 4.5 UI 업데이트 메서드

#### 4.5.1 시뮬레이션 UI 업데이트
```csharp
internal void UpdateSimulationUi()
{
    // 1. 공정 상태 표시
    // 2. 챔버 상태 표시
    // 3. FOUP 상태 표시
    // 4. TM 상태 표시
    // 5. 램프 상태 업데이트
}
```

#### 4.5.2 TM 시각화 업데이트
```csharp
internal void UpdateTmVisualization()
{
    // 1. TM 위치 업데이트
    // 2. TM 암 확장 상태 업데이트
    // 3. 웨이퍼 운반 상태 업데이트
}
```

---

## 5. 이벤트 처리

### 5.1 타이머 이벤트

#### 5.1.1 시뮬레이션 타이머
- **주기**: 시뮬레이션 모드 1000ms, 하드웨어 모드 150ms
- **역할**: 공정 진행, TM 처리, 스케줄링

#### 5.1.2 UI 업데이트 타이머
- **주기**: 50ms
- **역할**: 하드웨어 상태 확인 및 UI 업데이트

#### 5.1.3 헤더 시계 타이머
- **주기**: 1000ms
- **역할**: 헤더 시계 업데이트

### 5.2 사용자 입력 이벤트

- **버튼 클릭**: 공정 시작/일시정지/중지, 웨이퍼 로드/언로드
- **메뉴 선택**: 레시피 관리, 사용자 관리, 설정
- **탭 전환**: Main, Verification, Transfer 탭

---

## 6. 상태 관리

### 6.1 공정 상태

```csharp
public enum ProcessState
{
    Idle,      // 대기
    Running,   // 실행 중
    Paused,    // 일시정지
    Error      // 오류
}
```

### 6.2 상태 전환

```
Idle → Running (공정 시작)
Running → Paused (일시정지)
Paused → Running (재개)
Running → Idle (공정 완료)
Running → Error (오류 발생)
Error → Idle (오류 해결)
```

### 6.3 상태별 동작

- **Idle**: 모든 타이머 중지, 초기 상태 유지
- **Running**: 시뮬레이션 타이머 실행, 공정 진행
- **Paused**: 시뮬레이션 타이머 중지, 상태 유지
- **Error**: 모든 타이머 중지, 알람 표시

---

## 7. Helper 클래스 활용

### 7.1 Form1UiUpdater
- **역할**: UI 업데이트 로직 분리
- **메서드**: `UpdateSimulationUi()`, `UpdateTmVisualization()` 등

### 7.2 Form1Initializer
- **역할**: 초기화 로직 분리
- **메서드**: `InitializeCustomControls()`, `InitializeSimulationState()` 등

### 7.3 Form1Configurator
- **역할**: 설정 및 레이아웃 로직 분리
- **메서드**: `LayoutCentralEquipment()`, `ConfigureStatusPanels()` 등

### 7.4 Form1TmProcessor
- **역할**: TM 처리 로직 분리
- **메서드**: `ProcessTm()`, `PerformPickup()`, `PerformDropoff()` 등

### 7.5 FoupManager
- **역할**: FOUP 관리 로직 분리
- **메서드**: `HasWafersInFoupA()`, `AddCompletedWaferToFoupB()` 등

### 7.6 AlarmManager
- **역할**: 알람 관리 로직 분리
- **메서드**: `EvaluateAlarms()`, `TriggerAlarm()` 등

---

## 8. 하드웨어 모드 vs 시뮬레이션 모드

### 8.1 모드 확인
```csharp
internal bool IsTmHardwareModeAvailable()
{
    return HardwareManager?.IsEthercatConnected == true 
        && HardwareManager?.IsTmHardwareInitialized == true;
}
```

### 8.2 모드별 동작

#### 8.2.1 시뮬레이션 모드
- **TM 처리**: 틱 기반 시뮬레이션
- **공정 시간**: 틱 기반 감소
- **하드웨어 통신**: 없음

#### 8.2.2 하드웨어 모드
- **TM 처리**: 실제 하드웨어 제어
- **공정 시간**: 실제 경과 시간 기반
- **하드웨어 통신**: EtherCAT 통신

---

## 9. 주요 흐름

### 9.1 공정 시작 흐름

```
1. StartSimulation() 호출
2. 레시피 검증
3. FOUP A 웨이퍼 확인
4. SimulationController.Start() 호출
5. 상태를 Running으로 변경
6. 시뮬레이션 타이머 시작
7. TryScheduleLoadFromFoupA() 호출
8. 이송 작업 스케줄링
9. ProcessTm() 호출
10. 공정 진행
```

### 9.2 웨이퍼 이송 흐름

```
1. TryScheduleLoadFromFoupA() 호출
2. TransferController.EnqueueTransfer() 호출
3. ProcessTm()에서 작업 시작
4. PerformPickup() 실행
5. PerformDropoff() 실행
6. OnCompleted 콜백 호출
7. 다음 작업 시작
```

### 9.3 공정 완료 흐름

```
1. 모든 웨이퍼 처리 완료 확인
2. 모든 챔버 비어있음 확인
3. 큐 비어있음 확인
4. SimulationController.Stop() 호출
5. 상태를 Idle로 변경
6. TM 원점복귀 (하드웨어 모드)
```

---

## 10. 개선 사항

### 10.1 현재 문제점

1. **파일 크기**: 약 7,820줄로 너무 큼
2. **책임 과다**: 너무 많은 책임을 가짐
3. **테스트 어려움**: 의존성이 많아 단위 테스트 어려움

### 10.2 개선 방향

1. **추가 Helper 클래스 생성**: 더 많은 책임 분리
2. **Command 패턴 적용**: 사용자 액션을 Command 객체로 분리
3. **Mediator 패턴 고려**: 컴포넌트 간 통신을 Mediator로 중재
4. **의존성 주입 프레임워크 도입**: DI 프레임워크로 의존성 관리

---

## 11. 결론

`Form1.cs`는 전체 시스템의 핵심 조율자 역할을 하며, 다양한 컨트롤러, 서비스, 헬퍼 클래스를 통합하여 동작합니다. 이미 일부 책임이 Helper 클래스로 분리되었으나, 여전히 많은 책임을 가지고 있어 추가적인 리팩토링이 필요합니다.

**주요 특징:**
- ✅ 이벤트 핸들러 분리 (9개 파일)
- ✅ Helper 클래스로 책임 분리 시도
- ✅ ViewModel 패턴 적용
- ⚠️ 파일 크기 여전히 큼 (약 7,820줄)
- ⚠️ 추가 리팩토링 필요

---

**작성일:** 2025년 1월  
**문서 버전:** 1.0

