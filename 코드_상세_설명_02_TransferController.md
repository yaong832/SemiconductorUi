# 코드 상세 설명 - TransferController

**파일 경로:** `SemiconductorUi/Controllers/TransferController.cs`  
**크기:** 약 400줄  
**역할:** 웨이퍼 이송 제어 및 큐 관리

---

## 1. 개요

`TransferController`는 웨이퍼 이송을 담당하는 핵심 컨트롤러입니다. TM(Transfer Module)의 동작을 제어하고, 이송 작업을 큐로 관리하며, 상태 머신 패턴을 사용하여 복잡한 이송 프로세스를 관리합니다.

### 1.1 주요 책임

1. **이송 작업 큐 관리**: FIFO 큐로 이송 작업 관리
2. **TM Phase 제어**: 상태 머신으로 TM 동작 단계 제어
3. **이송 작업 스케줄링**: 챔버 간, FOUP-챔버 간 이송 작업 생성
4. **상태 확인**: 챔버 예약 상태, Region 사용 상태 확인

---

## 2. 클래스 구조

### 2.1 클래스 선언

```csharp
public class TransferController : ITransferService
{
    // ITransferService 인터페이스 구현
}
```

### 2.2 중첩 클래스

#### 2.2.1 TmPhase (상태 머신)

```csharp
public enum TmPhase
{
    Idle,                          // 대기 상태
    
    // === 픽업 위치로 이동 ===
    MoveToPickup,                  // 서보 이동 명령 발행
    MoveToPickup_WaitHardware,     // 하드웨어 서보 이동 완료 대기
    
    // === 픽업 도어 처리 ===
    WaitDoorPickupOpen,            // 도어 열기 대기
    
    // === 픽업 동작 ===
    PickupExtend,                  // 픽업 동작 시작
    PickupExtend_CylinderForward,  // 실린더 전진
    PickupExtend_ServoDown,        // 서보 하강 (하강위치→안착위치)
    PickupExtend_VacuumOn,         // 진공 ON
    
    // === 픽업 후 복귀 ===
    PickupRetract,                 // 픽업 후퇴 시작
    PickupRetract_ServoUp,         // 서보 상승 (안착위치→상승위치)
    PickupRetract_CylinderBackward,// 실린더 후진
    
    // === 픽업 도어 닫기 ===
    WaitDoorPickupClose,
    
    // === 드롭오프 위치로 이동 ===
    MoveToDropoff,
    MoveToDropoff_WaitHardware,
    
    // === 드롭오프 도어 처리 ===
    WaitDoorDropoffOpen,
    
    // === 드롭오프 동작 ===
    DropoffExtend,                 // 드롭오프 동작 시작
    DropoffExtend_CylinderForward, // 실린더 전진
    DropoffExtend_ServoDown,       // 서보 하강 (상승위치→안착위치)
    DropoffExtend_VacuumOffExhaust,// 진공 OFF + 배기
    
    // === 드롭오프 후 복귀 ===
    DropoffRetract,
    DropoffRetract_ServoUp,        // 서보 하강 (안착위치→하강위치)
    DropoffRetract_CylinderBackward,// 실린더 후진
    
    // === 드롭오프 도어 닫기 ===
    WaitDoorDropoffClose
}
```

**상태 전환 흐름:**
```
Idle → MoveToPickup → WaitDoorPickupOpen → PickupExtend → 
PickupRetract → WaitDoorPickupClose → MoveToDropoff → 
WaitDoorDropoffOpen → DropoffExtend → DropoffRetract → 
WaitDoorDropoffClose → Idle
```

#### 2.2.2 TransferTask (이송 작업)

```csharp
public class TransferTask
{
    public Wafer Wafer { get; set; }                    // 이송할 웨이퍼
    public EquipmentRegion Pickup { get; set; }          // 픽업 위치
    public EquipmentRegion Dropoff { get; set; }        // 드롭오프 위치
    public ChamberController.ChamberState SourceChamber { get; set; }      // 출발 챔버
    public ChamberController.ChamberState DestinationChamber { get; set; } // 도착 챔버
    public Action<Wafer> OnCompleted { get; set; }     // 완료 콜백
    public bool FromFoup { get; set; }                  // FOUP에서 시작 여부
    public int RetryCount { get; set; } = 0;            // 재시도 횟수
    public const int MaxRetryCount = 3;                 // 최대 재시도 횟수
}
```

---

## 3. 주요 필드 및 속성

### 3.1 필드

```csharp
private readonly Queue<TransferTask> _queue = new Queue<TransferTask>();
private TransferTask _currentTransfer;
private TmPhase _currentPhase = TmPhase.Idle;
private int _phaseTicksRemaining;
```

### 3.2 속성

```csharp
public TransferTask CurrentTransfer => _currentTransfer;
public TmPhase CurrentPhase => _currentPhase;
public int PhaseTicksRemaining => _phaseTicksRemaining;
public int QueueCount => _queue.Count;
public bool IsBusy => _currentPhase != TmPhase.Idle && _currentTransfer != null;
public bool IsQueueEmpty => _queue.Count == 0;
```

---

## 4. 주요 메서드

### 4.1 큐 관리

#### 4.1.1 이송 작업 추가
```csharp
public void EnqueueTransfer(TransferTask task)
{
    if (task == null)
    {
        throw new ArgumentNullException(nameof(task));
    }
    
    _queue.Enqueue(task);
}
```

#### 4.1.2 다음 작업 시작
```csharp
public TransferTask StartNextTransfer()
{
    if (_queue.Count == 0)
    {
        _currentPhase = TmPhase.Idle;
        _currentTransfer = null;
        return null;
    }
    
    // FIFO 방식으로 작업 선택
    _currentTransfer = _queue.Dequeue();
    _currentPhase = TmPhase.MoveToPickup;
    _phaseTicksRemaining = 0;
    
    return _currentTransfer;
}
```

#### 4.1.3 큐 초기화
```csharp
public void ClearQueue()
{
    _queue.Clear();
    _currentTransfer = null;
    _currentPhase = TmPhase.Idle;
    _phaseTicksRemaining = 0;
}
```

### 4.2 Phase 관리

#### 4.2.1 Phase 시작
```csharp
public void BeginPhase(TmPhase phase, int ticks, EquipmentRegion region = EquipmentRegion.TM, bool waitForCompletion = false)
{
    _currentPhase = phase;
    _phaseTicksRemaining = ticks;
}
```

#### 4.2.2 Phase 틱 감소
```csharp
public bool DecrementPhaseTick()
{
    if (_phaseTicksRemaining > 0)
    {
        _phaseTicksRemaining--;
    }
    
    return _phaseTicksRemaining <= 0;
}
```

#### 4.2.3 작업 완료
```csharp
public void CompleteCurrentTransfer()
{
    if (_currentTransfer != null)
    {
        // OnCompleted는 PerformDropoff()에서 이미 호출됨
        _currentTransfer = null;
    }
    
    _currentPhase = TmPhase.Idle;
    _phaseTicksRemaining = 0;
}
```

### 4.3 이송 작업 스케줄링

#### 4.3.1 챔버 간 이송
```csharp
public TransferTask ScheduleChamberTransfer(
    ChamberController.ChamberState source,
    ChamberController.ChamberState destination,
    Wafer wafer)
{
    if (source == null || destination == null || wafer == null)
    {
        return null;
    }
    
    var task = new TransferTask
    {
        Wafer = wafer,
        Pickup = source.Region,
        Dropoff = destination.Region,
        SourceChamber = source,
        DestinationChamber = destination,
        FromFoup = false
    };
    
    EnqueueTransfer(task);
    return task;
}
```

#### 4.3.2 FOUP에서 챔버로 이송
```csharp
public TransferTask ScheduleTransferFromFoup(
    EquipmentRegion foupRegion,
    ChamberController.ChamberState destination,
    Wafer wafer)
{
    if (destination == null || wafer == null)
    {
        return null;
    }
    
    var task = new TransferTask
    {
        Wafer = wafer,
        Pickup = foupRegion,
        Dropoff = destination.Region,
        SourceChamber = null,
        DestinationChamber = destination,
        FromFoup = true
    };
    
    EnqueueTransfer(task);
    return task;
}
```

#### 4.3.3 챔버에서 FOUP로 이송
```csharp
public TransferTask ScheduleTransferToFoup(
    ChamberController.ChamberState source,
    EquipmentRegion foupRegion,
    Wafer wafer)
{
    if (source == null || wafer == null)
    {
        return null;
    }
    
    var task = new TransferTask
    {
        Wafer = wafer,
        Pickup = source.Region,
        Dropoff = foupRegion,
        SourceChamber = source,
        DestinationChamber = null,
        FromFoup = false
    };
    
    EnqueueTransfer(task);
    return task;
}
```

### 4.4 상태 확인

#### 4.4.1 챔버 예약 확인
```csharp
public bool IsChamberReservedInQueue(ChamberController.ChamberState chamber)
{
    if (chamber == null)
    {
        return false;
    }
    
    // 현재 진행 중인 작업이 해당 챔버로 향하는지 확인
    if (_currentTransfer != null && _currentTransfer.DestinationChamber == chamber)
    {
        return true;
    }
    
    // 큐에 해당 챔버로 향하는 작업이 있는지 확인
    return _queue.Any(t => t.DestinationChamber == chamber);
}
```

#### 4.4.2 Region 사용 확인
```csharp
public bool IsRegionInUse(EquipmentRegion region)
{
    if (_currentTransfer != null)
    {
        if (_currentTransfer.Pickup == region || _currentTransfer.Dropoff == region)
        {
            return true;
        }
    }
    
    return _queue.Any(t => t.Pickup == region || t.Dropoff == region);
}
```

---

## 5. 상태 머신 동작

### 5.1 전체 흐름

```
1. Idle 상태
   ↓
2. StartNextTransfer() 호출
   ↓
3. MoveToPickup: 픽업 위치로 이동
   ↓
4. WaitDoorPickupOpen: 도어 열기 대기
   ↓
5. PickupExtend: 픽업 동작 시작
   ├─ CylinderForward: 실린더 전진
   ├─ ServoDown: 서보 하강
   └─ VacuumOn: 진공 ON
   ↓
6. PickupRetract: 픽업 후퇴
   ├─ ServoUp: 서보 상승
   └─ CylinderBackward: 실린더 후진
   ↓
7. WaitDoorPickupClose: 도어 닫기 대기
   ↓
8. MoveToDropoff: 드롭오프 위치로 이동
   ↓
9. WaitDoorDropoffOpen: 도어 열기 대기
   ↓
10. DropoffExtend: 드롭오프 동작 시작
    ├─ CylinderForward: 실린더 전진
    ├─ ServoDown: 서보 하강
    └─ VacuumOffExhaust: 진공 OFF + 배기
    ↓
11. DropoffRetract: 드롭오프 후퇴
    ├─ ServoUp: 서보 하강 (하강 위치)
    └─ CylinderBackward: 실린더 후진
    ↓
12. WaitDoorDropoffClose: 도어 닫기 대기
    ↓
13. CompleteCurrentTransfer() 호출
    ↓
14. Idle 상태 (다음 작업 시작)
```

### 5.2 Phase별 처리

각 Phase는 `Form1.ProcessTm()` 메서드에서 처리되며, 하드웨어 모드와 시뮬레이션 모드에 따라 다르게 동작합니다.

- **시뮬레이션 모드**: 틱 기반으로 Phase 진행
- **하드웨어 모드**: 실제 하드웨어 상태 확인 후 Phase 진행

---

## 6. 우선순위 규칙

현재는 FIFO(First In First Out) 방식으로 큐를 관리하지만, 실제 우선순위는 `Form1`에서 스케줄링 시 결정됩니다:

1. **FOUP A → Chamber A** (최우선)
2. **Chamber A → B/C** (중간 우선순위)
3. **B/C → FOUP B** (낮은 우선순위)

---

## 7. 재시도 메커니즘

`TransferTask`에 재시도 기능이 포함되어 있으나, 현재는 사용되지 않습니다. 향후 하드웨어 오류 시 재시도 로직을 추가할 수 있습니다.

```csharp
public int RetryCount { get; set; } = 0;
public const int MaxRetryCount = 3;
```

---

## 8. 인터페이스 구현

`TransferController`는 `ITransferService` 인터페이스를 구현하여 의존성 역전 원칙을 따릅니다.

**장점:**
- 테스트 용이성: Mock 객체 사용 가능
- 확장성: 다른 구현체로 교체 가능
- 의존성 주입: DI 프레임워크와 함께 사용 가능

---

## 9. 사용 예시

### 9.1 FOUP A에서 Chamber A로 이송

```csharp
// Form1에서 호출
var task = TransferService.ScheduleTransferFromFoup(
    EquipmentRegion.FoupA,
    ChamberAState,
    wafer);

// TransferController가 큐에 추가
// ProcessTm()에서 자동으로 처리
```

### 9.2 Chamber A에서 Chamber B로 이송

```csharp
// Form1에서 호출
var task = TransferService.ScheduleChamberTransfer(
    ChamberAState,
    ChamberBState,
    wafer);
```

### 9.3 Chamber B에서 FOUP B로 이송

```csharp
// Form1에서 호출
var task = TransferService.ScheduleTransferToFoup(
    ChamberBState,
    EquipmentRegion.FoupB,
    wafer);
```

---

## 10. 개선 사항

### 10.1 현재 문제점

1. **우선순위 큐 부재**: FIFO만 지원, 우선순위 큐 필요
2. **재시도 미구현**: 재시도 메커니즘은 있으나 사용되지 않음
3. **에러 처리 부족**: 작업 실패 시 처리 로직 부족

### 10.2 개선 방향

1. **우선순위 큐 도입**: PriorityQueue 사용
2. **재시도 로직 구현**: 하드웨어 오류 시 자동 재시도
3. **에러 처리 강화**: 작업 실패 시 큐에서 제거 또는 재시도
4. **작업 취소 기능**: 진행 중인 작업 취소 기능 추가

---

## 11. 결론

`TransferController`는 웨이퍼 이송을 담당하는 핵심 컨트롤러로, 상태 머신 패턴을 사용하여 복잡한 TM 동작을 명확하게 모델링합니다. 큐 기반 작업 관리와 인터페이스 기반 설계로 확장성과 테스트 용이성을 확보하고 있습니다.

**주요 특징:**
- ✅ 상태 머신 패턴으로 복잡한 프로세스 모델링
- ✅ 큐 기반 작업 관리
- ✅ 인터페이스 기반 설계
- ⚠️ 우선순위 큐 부재
- ⚠️ 재시도 로직 미구현

---

**작성일:** 2025년 1월  
**문서 버전:** 1.0

