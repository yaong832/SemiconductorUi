# IEG3268 DLL 메서드 확인 스크립트

$dllPath = ".\IEG3268_Dll.dll"
$outputPath = ".\IEG3268_Methods.txt"

Write-Host "DLL 파일 로드 중: $dllPath"

try {
    Add-Type -Path $dllPath
    
    $type = [IEG3268_Dll.IEG3268]
    $sb = New-Object System.Text.StringBuilder
    
    $sb.AppendLine("=== IEG3268 DLL 메서드 목록 ===") | Out-Null
    $sb.AppendLine("") | Out-Null
    $sb.AppendLine("클래스: $($type.FullName)") | Out-Null
    $sb.AppendLine("네임스페이스: $($type.Namespace)") | Out-Null
    $sb.AppendLine("어셈블리: $($type.Assembly.FullName)") | Out-Null
    $sb.AppendLine("") | Out-Null
    
    # Public 메서드 목록
    $bindingFlags = [System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::Static -bor [System.Reflection.BindingFlags]::DeclaredOnly
    $methods = $type.GetMethods($bindingFlags) | Sort-Object Name
    
    $sb.AppendLine("총 $($methods.Count)개의 public 메서드:") | Out-Null
    $sb.AppendLine("") | Out-Null
    
    foreach($method in $methods) {
        $params = $method.GetParameters()
        $paramList = ($params | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ", "
        $sb.AppendLine("  $($method.ReturnType.Name) $($method.Name)($paramList)") | Out-Null
    }
    
    # Digital 관련 메서드 특별 표시
    $sb.AppendLine("") | Out-Null
    $sb.AppendLine("=== Digital 관련 메서드 ===") | Out-Null
    $sb.AppendLine("") | Out-Null
    
    $digitalMethods = $methods | Where-Object { 
        $_.Name -like "*Digital*" -or 
        $_.Name -like "*Input*" -or 
        $_.Name -like "*Output*" 
    }
    
    if ($digitalMethods) {
        foreach($method in $digitalMethods) {
            $params = $method.GetParameters()
            $paramList = ($params | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ", "
            $sb.AppendLine("  $($method.ReturnType.Name) $($method.Name)($paramList)") | Out-Null
        }
    } else {
        $sb.AppendLine("  Digital 관련 메서드를 찾을 수 없습니다.") | Out-Null
    }
    
    # Properties 목록
    $sb.AppendLine("") | Out-Null
    $sb.AppendLine("=== Properties ===") | Out-Null
    $sb.AppendLine("") | Out-Null
    
    $properties = $type.GetProperties([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::Static) | Sort-Object Name
    
    foreach($prop in $properties) {
        $getSet = if ($prop.CanWrite) { "get; set;" } else { "get;" }
        $sb.AppendLine("  $($prop.PropertyType.Name) $($prop.Name) { $getSet }") | Out-Null
    }
    
    # 파일로 저장
    $sb.ToString() | Out-File -FilePath $outputPath -Encoding UTF8
    
    Write-Host ""
    Write-Host "메서드 목록이 저장되었습니다: $outputPath" -ForegroundColor Green
    Write-Host ""
    Write-Host $sb.ToString()
    
} catch {
    Write-Host "오류 발생: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.Exception
}
