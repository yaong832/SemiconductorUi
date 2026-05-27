# 코드 상세 설명 - TmHardwareController

**파일 경로:** `SemiconductorUi/Controllers/TmHardwareController.cs`  
**크기:** 약 1,240줄  
**역할:** TM(Transfer Module) 하드웨어 제어

---

## 1. 개요

`TmHardwareController`는 TM(Transfer Module)의 하드웨어를 직접 제어하는 컨트롤러입니다. 서보모터(Axis1: 상하, Axis2: 좌우), 실린더, 진공을 제어하여 웨이퍼를 이송합니다.

### 1.1 주요 책임

1. **서보모터 제어**: Axis1(상하), Axis2(좌우) 제어
2. **실린더 제어**: 전진/후진 제어
3. **진공 제어**: 진공 ON/OFF, 배기 제어
4. **위치 제어**: 티칭값 기반 정확한 위치 이동
5. **원점복귀**: 서보모터 원점복귀 수행

---

## 2. 클래스 구조

### 2.1 클래스 선언

```csharp
public class TmHardwareController
{
    private readonly IEG3268 _ethercat;
    private readonly Action<string, string> _logCallback;
    private TmPositionSet _positions;
    private bool _isInitialized;
    private bool _isServoOn;
    private bool _isHomed;
    private bool _isVacuumOn;
}
```

### 2.2 상수 정의

#### 2.2.1 Digital I/O Mapping

```csharp
// 실린더
private const int CYLINDER_FORWARD_OUTPUT = 12;   // 실린더 전진 출력
private const int CYLINDER_BACKWARD_OUTPUT = 13;  // 실린더 후진 출력
private const int CYLINDER_FORWARD_SENSOR = 13;   // 실린더 전진 센서 (Input)
private const int CYLINDER_BACKWARD_SENSOR = 12;  // 실린더 후진 센서 (Input)

// 진공
private const int VACUUM_INTAKE_OUTPUT = 14;      // 흡기 출력
private const int VACUUM_EXHAUST_OUTPUT = 15;     // 배기 출력
```

#### 2.2.2 서보모터 파라미터 (AppSettings에서 읽어옴)

```csharp
private static long DEFAULT_VELOCITY => AppSettings.ServoDefaultVelocity;          // 1,000,000 pulse/s
private static long DEFAULT_MAX_VELOCITY => AppSettings.ServoDefaultMaxVelocity;  // 1,000,000 pulse/s
private static long DEFAULT_DECELERATION => AppSettings.ServoDefaultDeceleration;  // 100,000,000
private static long DEFAULT_ACCELERATION => AppSettings.ServoDefaultAcceleration; // 1,000,000
```

#### 2.2.3 타임아웃 설정 (AppSettings에서 읽어옴)

```csharp
private static int SERVO_MOVE_TIMEOUT_MS => AppSettings.ServoMoveTimeoutMs;              // 3,000ms
private static int CYLINDER_ACTION_TIMEOUT_MS => AppSettings.CylinderActionTimeoutMs;   // 5,000ms
private static int VACUUM_STABILIZE_DELAY_MS => AppSettings.VacuumOnSettleDelayMs;     // 100ms
private static int EXHAUST_DURATION_MS => AppSettings.VacuumOffSettleDelayMs;           // 100ms
private static int HARDWARE_POLLING_INTERVAL_MS => AppSettings.HardwarePollingIntervalMs; // 30ms
```

### 2.3 위치 정의 (TmPositionSet)

```csharp
public class TmPositionSet
{
    // 하강 위치 오프셋
    public const long DESCEND_OFFSET = 30000;
    
    // Chamber A 위치
    public long ChamberA_X { get; set; } = -59064;
    public long ChamberA_LandY { get; set; } = 806931;    // 안착 위치
    public long ChamberA_RaiseY { get; set; } = 1156931;   // 상승 위치
    public long ChamberA_DescendY => ChamberA_LandY - DESCEND_OFFSET;
    
    // Chamber B 위치
    public long ChamberB_X { get; set; } = -190823;
    public long ChamberB_LandY { get; set; } = 806931;
    public long ChamberB_RaiseY { get; set; } = 1156931;
    public long ChamberB_DescendY => ChamberB_LandY - DESCEND_OFFSET;
    
    // Chamber C 위치
    public long ChamberC_X { get; set; } = -321600;
    public long ChamberC_LandY { get; set; } = 806931;
    public long ChamberC_RaiseY { get; set; } = 1156931;
    public long ChamberC_DescendY => ChamberC_LandY - DESCEND_OFFSET;
    
    // FOUP A 위치
    public long FoupA_X { get; set; } = 14140;
    public long[] FoupA_LandY { get; set; } = new long[] 
    { 
        102379,   // 1층 안착
        782378,   // 2층 안착
        1432388,  // 3층 안착
        2119399,  // 4층 안착
        2818463   // 5층 안착
    };
    public long[] FoupA_RaiseY { get; set; } = new long[] 
    { 
        302380,   // 1층 상승
        982378,   // 2층 상승
        1627604,  // 3층 상승
        2332102,  // 4층 상승
        3018457   // 5층 상승
    };
    
    // FOUP B 위치 (FOUP A와 동일한 높이)
    public long FoupB_X { get; set; } = -394293;
    
    // Home 위치
    public long Home_X { get; set; } = 0;
    public long Home_Y { get; set; } = 0;
}
```

**높이 순서:**
- **하강(Descend)**: 안착 위치 - 30,000 (실린더 전진/후진용)
- **안착(Land)**: 웨이퍼가 놓이는 정확한 위치
- **상승(Raise)**: 웨이퍼를 들어올린 이동 위치

---

## 3. 주요 메서드

### 3.1 초기화

#### 3.1.1 하드웨어 초기화
```csharp
public OperationResult Initialize()
{
    // 1. 서보모터 OFF
    // 2. 서보모터 ON
    // 3. 초기화 완료
}
```

#### 3.1.2 원점복귀
```csharp
public OperationResult PerformHoming()
{
    // 1. Axis1 원점복귀
    // 2. Axis2 원점복귀
    // 3. 원점복귀 완료
}
```

### 3.2 서보모터 제어

#### 3.2.1 XY 동시 이동
```csharp
public OperationResult MoveXY(long targetX, long targetY)
{
    // 1. Axis2 이동 명령 (X축)
    // 2. Axis1 이동 명령 (Y축)
    // 3. 이동 완료 대기
    // 4. 타임아웃 확인
}
```

#### 3.2.2 Y축 이동
```csharp
public OperationResult MoveY(long targetY)
{
    // 1. Axis1 이동 명령
    // 2. 이동 완료 대기
    // 3. 타임아웃 확인
}
```

#### 3.2.3 X축 이동
```csharp
public OperationResult MoveX(long targetX)
{
    // 1. Axis2 이동 명령
    // 2. 이동 완료 대기
    // 3. 타임아웃 확인
}
```

### 3.3 실린더 제어

#### 3.3.1 실린더 전진
```csharp
public OperationResult CylinderForward()
{
    // 1. 실린더 전진 출력 ON
    // 2. 전진 센서 확인 대기
    // 3. 타임아웃 확인
}
```

#### 3.3.2 실린더 후진
```csharp
public OperationResult CylinderBackward()
{
    // 1. 실린더 후진 출력 ON
    // 2. 후진 센서 확인 대기
    // 3. 타임아웃 확인
}
```

### 3.4 진공 제어

#### 3.4.1 진공 ON
```csharp
public OperationResult VacuumOn()
{
    // 1. 진공 흡기 출력 ON
    // 2. 안정화 대기
}
```

#### 3.4.2 진공 OFF + 배기
```csharp
public OperationResult VacuumOffExhaust()
{
    // 1. 진공 흡기 출력 OFF
    // 2. 배기 출력 ON
    // 3. 배기 지속 시간 대기
    // 4. 배기 출력 OFF
}
```

### 3.5 위치 이동

#### 3.5.1 Chamber로 이동
```csharp
public OperationResult MoveToChamber(EquipmentRegion chamber, bool toLandPosition = false)
{
    long x, y;
    
    switch (chamber)
    {
        case EquipmentRegion.ChamberA:
            x = _positions.ChamberA_X;
            y = toLandPosition ? _positions.ChamberA_LandY : _positions.ChamberA_RaiseY;
            break;
        case EquipmentRegion.ChamberB:
            x = _positions.ChamberB_X;
            y = toLandPosition ? _positions.ChamberB_LandY : _positions.ChamberB_RaiseY;
            break;
        case EquipmentRegion.ChamberC:
            x = _positions.ChamberC_X;
            y = toLandPosition ? _positions.ChamberC_LandY : _positions.ChamberC_RaiseY;
            break;
        default:
            return OperationResult.Failure("Invalid chamber region");
    }
    
    return MoveXY(x, y);
}
```

#### 3.5.2 FOUP로 이동
```csharp
public OperationResult MoveToFoup(EquipmentRegion foup, int slot, bool toLandPosition = false)
{
    long x;
    long[] landY, raiseY;
    
    if (foup == EquipmentRegion.FoupA)
    {
        x = _positions.FoupA_X;
        landY = _positions.FoupA_LandY;
        raiseY = _positions.FoupA_RaiseY;
    }
    else if (foup == EquipmentRegion.FoupB)
    {
        x = _positions.FoupB_X;
        landY = _positions.FoupB_LandY;
        raiseY = _positions.FoupB_RaiseY;
    }
    else
    {
        return OperationResult.Failure("Invalid FOUP region");
    }
    
    int slotIndex = slot - 1; // 1~5층 → 0~4 인덱스
    if (slotIndex < 0 || slotIndex >= 5)
    {
        return OperationResult.Failure("Invalid slot number");
    }
    
    long y = toLandPosition ? landY[slotIndex] : raiseY[slotIndex];
    return MoveXY(x, y);
}
```

---

## 4. 동작 시퀀스

### 4.1 픽업 시퀀스

```
1. MoveToPickup: 픽업 위치로 이동 (상승 위치)
   ↓
2. WaitDoorPickupOpen: 도어 열기 대기
   ↓
3. CylinderForward: 실린더 전진
   ↓
4. MoveY(DescendY): 서보 하강 (하강 위치)
   ↓
5. MoveY(LandY): 서보 상승 (안착 위치, 웨이퍼 접촉)
   ↓
6. VacuumOn: 진공 ON
   ↓
7. MoveY(RaiseY): 서보 상승 (상승 위치, 웨이퍼 들어올림)
   ↓
8. CylinderBackward: 실린더 후진
   ↓
9. WaitDoorPickupClose: 도어 닫기 대기
```

### 4.2 드롭오프 시퀀스

```
1. MoveToDropoff: 드롭오프 위치로 이동 (상승 위치)
   ↓
2. WaitDoorDropoffOpen: 도어 열기 대기
   ↓
3. CylinderForward: 실린더 전진
   ↓
4. MoveY(LandY): 서보 하강 (안착 위치, 웨이퍼 내려놓기)
   ↓
5. VacuumOffExhaust: 진공 OFF + 배기
   ↓
6. MoveY(DescendY): 서보 하강 (하강 위치)
   ↓
7. CylinderBackward: 실린더 후진
   ↓
8. WaitDoorDropoffClose: 도어 닫기 대기
```

---

## 5. 타임아웃 처리

### 5.1 서보 이동 타임아웃

```csharp
private bool WaitForServoMoveComplete(int axis, int timeoutMs)
{
    DateTime startTime = DateTime.Now;
    
    while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
    {
        if (IsServoMoveComplete(axis))
        {
            return true;
        }
        System.Threading.Thread.Sleep(HARDWARE_POLLING_INTERVAL_MS);
    }
    
    return false; // 타임아웃
}
```

### 5.2 실린더 동작 타임아웃

```csharp
private bool WaitForCylinderPosition(bool forward, int timeoutMs)
{
    DateTime startTime = DateTime.Now;
    
    while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
    {
        if (IsCylinderInPosition(forward))
        {
            return true;
        }
        System.Threading.Thread.Sleep(HARDWARE_POLLING_INTERVAL_MS);
    }
    
    return false; // 타임아웃
}
```

---

## 6. 상태 확인

### 6.1 서보 이동 완료 확인

```csharp
private bool IsServoMoveComplete(int axis)
{
    if (axis == 1)
    {
        return _ethercat.Axis1_MoveDone();
    }
    else if (axis == 2)
    {
        return _ethercat.Axis2_MoveDone();
    }
    return false;
}
```

### 6.2 실린더 위치 확인

```csharp
private bool IsCylinderInPosition(bool forward)
{
    if (forward)
    {
        return _ethercat.Digital_Input(CYLINDER_FORWARD_SENSOR);
    }
    else
    {
        return _ethercat.Digital_Input(CYLINDER_BACKWARD_SENSOR);
    }
}
```

---

## 7. 에러 처리

### 7.1 OperationResult

```csharp
public class OperationResult
{
    public bool Success { get; }
    public string ErrorMessage { get; }
    
    public static OperationResult SuccessResult() => new OperationResult(true, null);
    public static OperationResult Failure(string message) => new OperationResult(false, message);
}
```

### 7.2 에러 처리 예시

```csharp
public OperationResult MoveXY(long targetX, long targetY)
{
    try
    {
        // 이동 명령
        _ethercat.Axis2_MoveAbsolute(targetX, DEFAULT_VELOCITY, ...);
        _ethercat.Axis1_MoveAbsolute(targetY, DEFAULT_VELOCITY, ...);
        
        // 이동 완료 대기
        if (!WaitForServoMoveComplete(1, SERVO_MOVE_TIMEOUT_MS) ||
            !WaitForServoMoveComplete(2, SERVO_MOVE_TIMEOUT_MS))
        {
            return OperationResult.Failure("서보 이동 타임아웃");
        }
        
        return OperationResult.SuccessResult();
    }
    catch (Exception ex)
    {
        return OperationResult.Failure($"서보 이동 오류: {ex.Message}");
    }
}
```

---

## 8. 설정 외부화

모든 타임아웃 및 지연 시간은 `AppSettings`에서 읽어옵니다:

- `ServoMoveTimeoutMs`: 서보 이동 타임아웃
- `CylinderActionTimeoutMs`: 실린더 동작 타임아웃
- `VacuumOnSettleDelayMs`: 진공 ON 안정화 대기 시간
- `VacuumOffSettleDelayMs`: 진공 OFF 안정화 대기 시간
- `HardwarePollingIntervalMs`: 하드웨어 폴링 주기

---

## 9. 사용 예시

### 9.1 초기화 및 원점복귀

```csharp
// HardwareManager에서 호출
var initResult = tmHardwareController.Initialize();
if (!initResult.Success)
{
    // 초기화 실패 처리
}

var homeResult = tmHardwareController.PerformHoming();
if (!homeResult.Success)
{
    // 원점복귀 실패 처리
}
```

### 9.2 웨이퍼 픽업

```csharp
// Form1에서 호출
var moveResult = tmHardwareController.MoveToFoup(EquipmentRegion.FoupA, 1, false);
if (moveResult.Success)
{
    var cylinderResult = tmHardwareController.CylinderForward();
    if (cylinderResult.Success)
    {
        var descendResult = tmHardwareController.MoveY(positions.FoupA_DescendY[0]);
        // ... 계속
    }
}
```

---

## 10. 개선 사항

### 10.1 현재 문제점

1. **동기 처리**: 모든 하드웨어 제어가 동기적으로 처리됨
2. **에러 복구 부족**: 타임아웃 발생 시 자동 복구 로직 부족
3. **상태 추적 부족**: 현재 위치 추적이 정확하지 않을 수 있음

### 10.2 개선 방향

1. **비동기 처리**: async/await 패턴 적용
2. **에러 복구**: 타임아웃 발생 시 재시도 또는 원점복귀
3. **상태 추적 강화**: 현재 위치를 정확히 추적

---

## 11. 결론

`TmHardwareController`는 TM 하드웨어를 직접 제어하는 핵심 컨트롤러로, 서보모터, 실린더, 진공을 정확하게 제어하여 웨이퍼를 이송합니다. 티칭값 기반 위치 제어와 타임아웃 처리를 통해 안정적인 하드웨어 제어를 제공합니다.

**주요 특징:**
- ✅ 티칭값 기반 정확한 위치 제어
- ✅ 타임아웃 처리
- ✅ 설정 외부화
- ✅ 에러 처리
- ⚠️ 동기 처리 (비동기 개선 가능)

---

**작성일:** 2025년 1월  
**문서 버전:** 1.0

