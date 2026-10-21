# VisionPlatform 自动化验收入口（阶段 1）
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
Set-Location $root

Write-Host "== VisionPlatform verify ==" -ForegroundColor Cyan
dotnet build VisionPlatform.sln -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$resultsDir = Join-Path $root "tests\TestResults"
New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null
dotnet test VisionPlatform.sln -c Release --no-build --logger "trx;LogFileName=results.trx" --results-directory $resultsDir
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "PASS: build + test" -ForegroundColor Green
Write-Host "BLOCKED (manual/env): C-03 Halcon, C-04 YOLO — see docs/acceptance_criteria.md"
