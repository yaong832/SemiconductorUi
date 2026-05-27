# Semiconductor UI - 반도체 통합 공정 및 실장비 제어 시스템 🖥️⚡

본 프로젝트는 반도체 제조 공정 중 웨이퍼 이송 및 챔버 공정 시뮬레이션을 관리하고 실제 제어 장비와 실시간으로 연동하기 위한 **.NET WinForms 기반 통합 제어 시스템 (SemiconductorUi)** 입니다.

실제 반도체 이송 공정 사양에 맞춰 설계되었으며, **EtherCAT(IEG3268 DLL)** 하드웨어 통신 모듈과 정밀 상태 머신(State Machine)을 통해 완벽한 자동화 시퀀스를 제공합니다.

---

## 🛠️ 기술 스택 (Tech Stack)

- **Language & Framework**: C#, .NET Framework (WinForms)
- **Hardware Interface**: `IEG3268 DLL` (EtherCAT 기반 실시간 드라이버 연동)
- **Data Access**: XML 파일 기반 (사용자 계정, 공정 레시피 및 시스템 설정 보존)
- **Architecture**: 4-Tier 계정 분리 아키텍처 (Presentation, BLL, Service, DAL)
- **Design Patterns**: Repository, Service Locator, MVVM (부분 적용), State Machine, Observer
- **Unit Testing**: MSTest Framework (`SemiconductorUi.Tests` 통합)

---

## 📐 아키텍처 다이어그램 (Architecture)

본 시스템은 높은 유지보수성과 확장성을 위해 각 계층이 엄격히 분리된 **4계층 구조**를 준수합니다.

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer (표현 계층)         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   Form1.cs   │  │  Forms/*.cs  │  │ Controls/*.cs│  │
│  └──────┬───────┘  └──────────────┘  └──────────────┘  │
└─────────┼───────────────────────────────────────────────┘
          │
┌─────────┼───────────────────────────────────────────────┐
│         │         Business Logic Layer (비즈니스 로직)    │
│  ┌──────▼───────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Controllers/ │  │  ViewModels/  │  │  Helpers/    │  │
│  │  - Transfer  │  │  MainFormVM   │  │  - AlarmMgr  │  │
│  │  - Chamber   │  └──────────────┘  │  - FoupMgr    │  │
│  │  - Simulation│                    │  - UI Updater│  │
│  └──────┬───────┘                    └──────────────┘  │
└─────────┼───────────────────────────────────────────────┘
          │
┌─────────┼───────────────────────────────────────────────┐
│         │         Service Layer (서비스 추상화 계층)       │
│  ┌──────▼───────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Services/    │  │  Repositories/│  │  Interfaces/ │  │
│  │  - Hardware  │  │  - User       │  │  - ITransfer │  │
│  │  - Logger    │  │  - Recipe     │  │  - IChamber   │  │
│  │  - Container │  │  - Config     │  │  - ILogger   │  │
│  └──────┬───────┘  └──────────────┘  └──────────────┘  │
└─────────┼───────────────────────────────────────────────┘
          │
┌─────────┼───────────────────────────────────────────────┐
│         │         Data Access Layer (데이터 접근 계층)     │
│  ┌──────▼───────┐  ┌──────────────┐                    │
│  │  XML Files   │  │  IEG3268 DLL  │                    │
│  │  - Users.xml │  │  (Hardware)   │                    │
│  │  - Recipes   │  └──────────────┘                    │
│  │  - Config    │                                       │
│  └──────────────┘                                       │
└──────────────────────────────────────────────────────────┘
```

---

## 🌟 핵심 기능 (Core Features)

### 1. 자동화 공정 시퀀스 (Automated Process Flow)
웨이퍼를 로딩 포트(FOUP A)에서 공급받아 각 챔버(Chamber A, B, C)에서 레시피에 정의된 공정을 독립적으로 진행한 후 언로딩 포트(FOUP B)로 안전하게 퇴출하는 일련의 전 공정을 실시간 제어 스레드로 자동화합니다.

### 2. EtherCAT 실장비 및 시뮬레이션 모드 지원 (EtherCAT & Simulation Mode)
- **하드웨어 제어 모드**: `IEG3268 DLL` 모듈을 로드하여 실제 서보 및 IO 노드와 통신
- **시뮬레이션 모드**: 실제 물리 장비가 준비되지 않은 환경에서도 완벽한 공정 디버깅 및 시퀀스 테스트가 가능하도록 고속 타이머 기반 가상 장치 제어 로직 동작

### 3. 구조적 디자인 패턴 활용
- **상태 머신 (State Machine)**: `TransferController`의 `TmPhase` 상태 열거형을 통하여 복잡한 암(Arm)의 반송 상태를 정밀 제어 및 교착상태(Deadlock) 사전 차단
- **저장소 패턴 (Repository Pattern)**: 파일 I/O 시스템과 비즈니스 로직을 명확히 격리하여 안전한 동시성 처리(`lock` 메커니즘) 지원
- **이벤트 핸들러 분리**: 거대한 메인 폼의 책임을 경감하기 위해 마우스 페인트, 타이머, 네비게이션, 로그인 등 9가지 책임별로 `EventHandlers/`를 엄격히 구조화

---

## 📂 디렉토리 구조 (Directory Structure)

```
SemiconductorUi/
├── Controllers/          # 비즈니스 로직 컨트롤러 (Transfer, Chamber 등)
├── Controls/             # 커스텀 UI 컨트롤 (실시간 포트폴리오 다이어그램 컴포넌트)
├── EventHandlers/        # UI 결합도를 낮추기 위해 분리된 이벤트 처리 모듈
├── Forms/                # 계정 관리 및 레시피 구성 폼
├── Helpers/              # 알람 관리, 장비 티칭값 세팅 헬퍼
├── Interfaces/           # 서비스 지향 아키텍처를 위한 추상 인터페이스
├── Models/               # Wafer, User 등 데이터 가상 모델
├── Repositories/         # 파일 기반 데이터 보존 처리
├── Services/             # 하드웨어 초기화 및 글로벌 의존성 컨테이너
├── ViewModels/           # 메인 폼 UI 상태 관리 (MVVM)
├── AppSettings.cs        # 챔버 사양 설정 파싱
└── Program.cs            # 애플리케이션 진입점
```
