# VisionPlatform 自动化验收入口
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
Set-Location $root
$sln = Get-ChildItem -Filter "VisionPlatform.sln*" | Select-Object -First 1 -ExpandProperty Name

Write-Host "== VisionPlatform verify ==" -ForegroundColor Cyan
dotnet restore $sln
dotnet build $sln -c Release --no-restore
dotnet test $sln -c Release --no-build --logger "console;verbosity=minimal"
Write-Host "BLOCKED: HAL-001, YOLO-001 (see docs/acceptance-criteria.md)" -ForegroundColor Yellow
