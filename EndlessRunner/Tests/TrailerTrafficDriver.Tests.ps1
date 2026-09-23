param([string]$UnityEditorData = 'F:/Unity/Unity Editor/6000.3.9f1/Editor/Data')
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$runtime = Get-ChildItem -Directory "$UnityEditorData/NetCoreRuntime/shared/Microsoft.NETCore.App" |
    Sort-Object Name -Descending | Select-Object -First 1
$outputDir = Join-Path $projectRoot 'Temp/TrailerTrafficTests'
New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
$testAssembly = Join-Path $outputDir 'TrailerTrafficTests.dll'
$references = @('System.Private.CoreLib.dll', 'System.Runtime.dll', 'System.Console.dll', 'System.Collections.dll') |
    ForEach-Object { '-r:' + (Join-Path $runtime.FullName $_) }
& "$UnityEditorData/NetCoreRuntime/dotnet.exe" "$UnityEditorData/DotNetSdkRoslyn/csc.dll" `
    -nologo -noconfig -target:exe "-out:$testAssembly" @references `
    "$projectRoot/Assets/Scripts/Player/TrailerTrafficDriver.cs" "$PSScriptRoot/TrailerTrafficDriver.Tests.cs"
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
& "$UnityEditorData/NetCoreRuntime/dotnet.exe" exec `
    --runtimeconfig "$UnityEditorData/DotNetSdkRoslyn/csc.runtimeconfig.json" $testAssembly
if ($LASTEXITCODE -ne 0) { throw 'Traffic driver tests failed.' }
