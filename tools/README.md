# tools

앱 빌드·실행에는 필요 없는 보조 스크립트입니다.

| 파일 | 용도 |
| --- | --- |
| `CheckDll/CheckDll.cs`, `CheckDllMethods.ps1` | `IEG3268_Dll.dll`에 어떤 메서드가 있는지 확인할 때 쓰는 도구 (DLL 소스가 없어서 만든 것) |
| `upload-to-github.ps1` | 2026-05 처음 GitHub에 올릴 때 쓴 스크립트 (기록용) |

`CheckDllMethods.ps1`은 저장소 맨 위 폴더에서 `.\tools\CheckDllMethods.ps1`로 실행합니다 (현재 폴더의 `IEG3268_Dll.dll`을 읽고 `IEG3268_Methods.txt`를 씁니다).
