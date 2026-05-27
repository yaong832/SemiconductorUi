# 코드 상세 설명 - ChamberController

**파일 경로:** `SemiconductorUi/Controllers/ChamberController.cs`  
**크기:** 약 380줄  
**역할:** 챔버 상태 및 공정 제어

---

## 1. 개요

`ChamberController`는 챔버(Chamber A, B, C)의 상태와 공정을 관리하는 컨트롤러입니다. 각 챔버의 웨이퍼 상태, 공정 시간, 환경 조건을 추적하고 관리합니다.

### 1.1 주요 책임

1. **챔버 상태 관리**: 웨이퍼 할당, 제거, 상태 추적
2. **공정 시간 관리**: 공정 시간 감소, 완료 확인
3. **환경 조건 관리**: 온도, 압력, 습도 목표값 및 실시간 데이터 관리
4. **공정 플로우 관리**: 공정 단계 정의 및 관리

---

## 2. 클래스 구조

### 2.1 클래스 선언

```csharp
public class ChamberController : IChamberService
{
    // IChamberService 인터페이스 구현
}
```

### 2.2 중첩 클래스

#### 2.2.1 SimulationStepDefinition (공정 단계 정의)

```csharp
public class SimulationStepDefinition
{
    public string DisplayName { get; }        // 공정 단계 이름
    public int DurationSeconds { get; set; }   // 공정 시간 (초)
    
    public SimulationStepDefinition(string name, int durationSeconds)
    {
        DisplayName = name;
        DurationSeconds = durationSeconds;
    }
}
```

**예시:**
- Chamber A: "PR 도포", 30초
- Chamber B: "노광 (Exposure-B)", 45초
- Chamber C: "노광 (Exposure-C)", 45초

#### 2.2.2 ChamberState (챔버 상태)

```csharp
public class ChamberState
{
    public string UnitKey { get; }                              // 챔버 식별자 (PMA, PMB, PMC)
    public EquipmentRegion Region { get; }                      // 장비 영역
    public SimulationStepDefinition Step { get; }              // 공정 단계 정의
    public Wafer CurrentWafer { get; set; }                     // 현재 웨이퍼
    public int RemainingSeconds { get; set; }                  // 남은 공정 시간
    public int TotalSeconds { get; set; }                      // 전체 공정 시간
    public string StatusText { get; set; } = "Idle";           // 상태 텍스트
    public bool ReservedForIncoming { get; set; }              // 들어올 웨이퍼 예약 여부
    public bool PickupScheduled { get; set; }                  // 픽업 스케줄 여부
    public double ProcessingAccumulator { get; set; }           // 공정 시간 누적기
    public DateTime LastProcessedTime { get; set; } = DateTime.MinValue; // 마지막 처리 시간
}
```

#### 2.2.3 ChamberEnvironmentSpec (챔버 환경 사양)

```csharp
public class ChamberEnvironmentSpec
{
    public string Temperature { get; }              // 온도 표시 문자열
    public string Pressure { get; }                 // 압력 표시 문자열
    public string Humidity { get; }                 // 습도 표시 문자열
    public string Notes { get; }                    // 비고
    public double TargetTemperatureC { get; }      // 목표 온도 (°C)
    public double TargetPressureTorr { get; }       // 목표 압력 (Torr)
    public double TargetHumidityPercent { get; }    // 목표 습도 (%)
}
```

**기본 환경 사양:**
- **Chamber A (PMA)**: 23.0°C, 760.0 Torr, 45.0% (PR 도포 챔버)
- **Chamber B (PMB)**: 22.5°C, 0.005 Torr, 5.0% (노광 챔버 1차)
- **Chamber C (PMC)**: 110.0°C, 50.0 Torr, 3.0% (노광 챔버 2차/병렬)

#### 2.2.4 ChamberEnvironmentLive (챔버 환경 실시간 데이터)

```csharp
public class ChamberEnvironmentLive
{
    public double TemperatureC { get; set; }        // 실시간 온도 (°C)
    public double PressureTorr { get; set; }       // 실시간 압력 (Torr)
    public double HumidityPercent { get; set; }     // 실시간 습도 (%)
}
```

---

## 3. 주요 필드 및 속성

### 3.1 필드

```csharp
private readonly Dictionary<string, ChamberState> _chambers;
private readonly Dictionary<string, ChamberEnvironmentSpec> _chamberEnvSpecs;
private readonly Dictionary<string, ChamberEnvironmentLive> _chamberEnvLive;
private readonly SimulationStepDefinition[] _processFlow;
```

### 3.2 속성

```csharp
public ChamberState ChamberA => _chambers.TryGetValue("PMA", out var state) ? state : null;
public ChamberState ChamberB => _chambers.TryGetValue("PMB", out var state) ? state : null;
public ChamberState ChamberC => _chambers.TryGetValue("PMC", out var state) ? state : null;
public IEnumerable<ChamberState> AllChambers => _chambers.Values;
```

---

## 4. 초기화

### 4.1 생성자

```csharp
public ChamberController(SimulationStepDefinition[] processFlow)
{
    _processFlow = processFlow ?? throw new ArgumentNullException(nameof(processFlow));
    InitializeChambers();
    InitializeEnvironmentSpecs();
}
```

### 4.2 챔버 초기화

```csharp
private void InitializeChambers()
{
    if (_processFlow.Length < 3)
    {
        throw new InvalidOperationException("공정 플로우는 최소 3단계가 필요합니다.");
    }
    
    _chambers["PMA"] = new ChamberState("PMA", EquipmentRegion.ChamberA, _processFlow[0]);
    _chambers["PMB"] = new ChamberState("PMB", EquipmentRegion.ChamberB, _processFlow[1]);
    _chambers["PMC"] = new ChamberState("PMC", EquipmentRegion.ChamberC, _processFlow[2]);
}
```

### 4.3 환경 사양 초기화

```csharp
private void InitializeEnvironmentSpecs()
{
    _chamberEnvSpecs["PMA"] = new ChamberEnvironmentSpec(
        "23.0°C", "760.0 Torr", "45.0%", "PR 도포 챔버",
        23.0, 760.0, 45.0);
    
    _chamberEnvSpecs["PMB"] = new ChamberEnvironmentSpec(
        "22.5°C", "0.005 Torr", "5.0%", "노광 챔버 (1차)",
        22.5, 0.005, 5.0);
    
    _chamberEnvSpecs["PMC"] = new ChamberEnvironmentSpec(
        "110.0°C", "50.0 Torr", "3.0%", "노광 챔버 (2차/병렬)",
        110.0, 50.0, 3.0);
}
```

---

## 5. 주요 메서드

### 5.1 챔버 상태 관리

#### 5.1.1 챔버 상태 가져오기
```csharp
public ChamberState GetChamberStateForUnit(string unitKey)
{
    return _chambers.TryGetValue(unitKey, out var state) ? state : null;
}
```

#### 5.1.2 챔버 상태 초기화
```csharp
public void ResetChamberStates()
{
    foreach (var chamber in _chambers.Values)
    {
        chamber.CurrentWafer = null;
        chamber.RemainingSeconds = 0;
        chamber.TotalSeconds = 0;
        chamber.StatusText = "Idle";
        chamber.ReservedForIncoming = false;
        chamber.PickupScheduled = false;
        chamber.ProcessingAccumulator = 0;
        chamber.LastProcessedTime = DateTime.MinValue;
    }
}
```

#### 5.1.3 웨이퍼 할당
```csharp
public bool AssignWaferToChamber(string unitKey, Wafer wafer)
{
    var chamber = GetChamberStateForUnit(unitKey);
    if (chamber == null || chamber.CurrentWafer != null)
    {
        return false;
    }
    
    chamber.CurrentWafer = wafer;
    chamber.RemainingSeconds = chamber.Step.DurationSeconds;
    chamber.TotalSeconds = chamber.Step.DurationSeconds;
    chamber.StatusText = "Processing";
    return true;
}
```

#### 5.1.4 웨이퍼 제거
```csharp
public Wafer RemoveWaferFromChamber(string unitKey)
{
    var chamber = GetChamberStateForUnit(unitKey);
    if (chamber == null)
    {
        return null;
    }
    
    var wafer = chamber.CurrentWafer;
    chamber.CurrentWafer = null;
    chamber.RemainingSeconds = 0;
    chamber.TotalSeconds = 0;
    chamber.StatusText = "Idle";
    chamber.PickupScheduled = false;
    return wafer;
}
```

### 5.2 상태 확인

#### 5.2.1 공정 중 확인
```csharp
public bool IsChamberProcessing(string unitKey)
{
    var chamber = GetChamberStateForUnit(unitKey);
    return chamber != null 
        && chamber.CurrentWafer != null 
        && chamber.RemainingSeconds > 0;
}
```

#### 5.2.2 이송 준비 확인
```csharp
public bool IsReadyForTransfer(string unitKey)
{
    var chamber = GetChamberStateForUnit(unitKey);
    return chamber != null 
        && chamber.CurrentWafer != null 
        && chamber.RemainingSeconds <= 0 
        && !chamber.PickupScheduled;
}
```

### 5.3 환경 관리

#### 5.3.1 환경 사양 가져오기
```csharp
public ChamberEnvironmentSpec GetEnvironmentSpec(string unitKey)
{
    return _chamberEnvSpecs.TryGetValue(unitKey, out var spec) ? spec : null;
}
```

#### 5.3.2 실시간 환경 데이터 가져오기
```csharp
public ChamberEnvironmentLive GetEnvironmentLive(string unitKey)
{
    return _chamberEnvLive.TryGetValue(unitKey, out var live) ? live : null;
}
```

#### 5.3.3 실시간 환경 데이터 업데이트
```csharp
public ChamberEnvironmentLive UpdateLiveEnvironment(string unitKey, ChamberState chamberState)
{
    if (!_chamberEnvSpecs.TryGetValue(unitKey, out var spec))
    {
        return null;
    }
    
    if (!_chamberEnvLive.TryGetValue(unitKey, out var live))
    {
        live = new ChamberEnvironmentLive
        {
            TemperatureC = spec.TargetTemperatureC,
            PressureTorr = spec.TargetPressureTorr,
            HumidityPercent = spec.TargetHumidityPercent
        };
        _chamberEnvLive[unitKey] = live;
    }
    
    // 실시간 환경 데이터 업데이트 로직은 여기에 구현
    // 현재는 기본값 유지
    
    return live;
}
```

### 5.4 공정 제어

#### 5.4.1 공정 단계 시간 업데이트
```csharp
public void UpdateStepDurations(int durA, int durB, int durC)
{
    if (ChamberA != null) ChamberA.Step.DurationSeconds = durA;
    if (ChamberB != null) ChamberB.Step.DurationSeconds = durB;
    if (ChamberC != null) ChamberC.Step.DurationSeconds = durC;
}
```

#### 5.4.2 공정 진행 업데이트 (시뮬레이션용)
```csharp
public void UpdateProcessing(double secondsPerTick, bool isRunning)
{
    if (!isRunning)
    {
        return;
    }
    
    foreach (var chamber in _chambers.Values)
    {
        if (chamber.CurrentWafer != null && chamber.RemainingSeconds > 0)
        {
            chamber.ProcessingAccumulator += secondsPerTick;
            chamber.RemainingSeconds = Math.Max(0, 
                chamber.TotalSeconds - (int)chamber.ProcessingAccumulator);
            
            if (chamber.RemainingSeconds <= 0)
            {
                chamber.StatusText = "Completed";
            }
        }
    }
}
```

---

## 6. 공정 흐름

### 6.1 기본 공정 흐름

```
1. FOUP A → Chamber A (PR 도포, 30초)
   ↓
2. Chamber A → Chamber B (노광 1차, 45초)
   ↓
3. Chamber B → Chamber C (노광 2차/병렬, 45초)
   ↓
4. Chamber C → FOUP B (완료)
```

### 6.2 병렬 가공 모드

```
1. FOUP A → Chamber A (PR 도포, 30초)
   ↓
2. Chamber A → Chamber B 또는 C (노광, 45초)
   - 비어있는 챔버 우선 선택
   - 둘 다 비어있으면 B 우선
   ↓
3. Chamber B/C → FOUP B (완료)
```

### 6.3 2차 노광 모드

```
1. FOUP A → Chamber A (PR 도포, 30초)
   ↓
2. Chamber A → Chamber B (노광 1차, 45초)
   ↓
3. Chamber B → Chamber C (노광 2차, 45초)
   ↓
4. Chamber C → FOUP B (완료)
```

---

## 7. 환경 조건 관리

### 7.1 환경 사양

각 챔버는 고유한 환경 사양을 가집니다:

- **Chamber A**: 상온, 대기압 (PR 도포용)
- **Chamber B**: 저온, 고진공 (노광용)
- **Chamber C**: 고온, 중진공 (2차 노광/후베이크용)

### 7.2 실시간 환경 데이터

실시간 환경 데이터는 `ChamberEnvironmentLive`로 관리되며, 알람 시스템에서 사용됩니다.

---

## 8. 인터페이스 구현

`ChamberController`는 `IChamberService` 인터페이스를 구현하여 의존성 역전 원칙을 따릅니다.

**장점:**
- 테스트 용이성: Mock 객체 사용 가능
- 확장성: 다른 구현체로 교체 가능
- 의존성 주입: DI 프레임워크와 함께 사용 가능

---

## 9. 사용 예시

### 9.1 챔버에 웨이퍼 할당

```csharp
// Form1에서 호출
bool success = ChamberService.AssignWaferToChamber("PMA", wafer);
if (success)
{
    // 웨이퍼 할당 성공
}
```

### 9.2 챔버에서 웨이퍼 제거

```csharp
// Form1에서 호출
Wafer wafer = ChamberService.RemoveWaferFromChamber("PMA");
if (wafer != null)
{
    // 웨이퍼 제거 성공
}
```

### 9.3 이송 준비 확인

```csharp
// Form1에서 호출
if (ChamberService.IsReadyForTransfer("PMA"))
{
    // Chamber A에서 이송 가능
    TryScheduleTransferFromChamberA();
}
```

---

## 10. 개선 사항

### 10.1 현재 문제점

1. **실시간 환경 데이터 업데이트 미구현**: 기본값만 유지
2. **환경 조건 변경 불가**: 하드코딩된 환경 사양
3. **레시피 연동 부족**: 레시피와 환경 조건 연동 부족

### 10.2 개선 방향

1. **실시간 환경 데이터 업데이트**: 하드웨어 센서 데이터 연동
2. **환경 조건 동적 변경**: 레시피 기반 환경 조건 설정
3. **레시피 연동 강화**: 레시피에 환경 조건 포함

---

## 11. 결론

`ChamberController`는 챔버 상태와 공정을 관리하는 핵심 컨트롤러로, 웨이퍼 할당, 공정 시간 추적, 환경 조건 관리 등의 기능을 제공합니다. 인터페이스 기반 설계로 확장성과 테스트 용이성을 확보하고 있습니다.

**주요 특징:**
- ✅ 챔버 상태 추적
- ✅ 공정 시간 관리
- ✅ 환경 조건 관리
- ✅ 인터페이스 기반 설계
- ⚠️ 실시간 환경 데이터 업데이트 미구현

---

**작성일:** 2025년 1월  
**문서 버전:** 1.0

