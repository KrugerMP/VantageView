# Run all VantageView applications using the "http" launch profile.
# Use: .\scripts\run-all.ps1
#
# Ports (http profile):
#   API:          5157
#   Auth:         5098
#   Frontend:     5059
#   Admin Portal: 5046

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$SrcDir = Join-Path (Split-Path -Parent $ScriptDir) "src"

Push-Location $SrcDir

Write-Host "Starting VantageView applications (http profile)..."
Write-Host ""
Write-Host "  API:          http://localhost:5157"
Write-Host "  Auth:         http://localhost:5098"
Write-Host "  Frontend:     http://localhost:5059"
Write-Host "  Admin Portal: http://localhost:5046"
Write-Host ""
Write-Host "Press Ctrl+C to stop all."
Write-Host ""

$jobs = @()
$jobs += Start-Job -ScriptBlock {
    Set-Location $using:SrcDir
    dotnet run --project VantageView.API --launch-profile http
}
$jobs += Start-Job -ScriptBlock {
    Set-Location $using:SrcDir
    dotnet run --project VantageView.Auth --launch-profile http
}
$jobs += Start-Job -ScriptBlock {
    Set-Location $using:SrcDir
    dotnet run --project VantageView.Frontend --launch-profile http
}
$jobs += Start-Job -ScriptBlock {
    Set-Location $using:SrcDir
    dotnet run --project VantageView.Admin.Portal --launch-profile http
}

try {
    while ($true) { Start-Sleep -Seconds 1 }
} finally {
    Write-Host ""
    Write-Host "Stopping all applications..."
    $jobs | Stop-Job -ErrorAction SilentlyContinue
    $jobs | Remove-Job -Force -ErrorAction SilentlyContinue
    Pop-Location
    Write-Host "Stopped all applications."
}
