$ErrorActionPreference = "Stop"

if (!(Test-Path ".\adb-path.txt")) {
    Write-Host "adb-path.txt not found. Run .\tools\Find-Adb.ps1 first." -ForegroundColor Red
    exit 1
}

$adb = Get-Content ".\adb-path.txt" -Raw
$adb = $adb.Trim()

if (!(Test-Path $adb)) {
    Write-Host "adb path is invalid:" $adb -ForegroundColor Red
    Write-Host "Run .\tools\Find-Adb.ps1 again." -ForegroundColor Yellow
    exit 1
}

Write-Host "ADB:" $adb -ForegroundColor Cyan
Write-Host ""
Write-Host "Connected devices:" -ForegroundColor Cyan
& $adb devices

Write-Host ""
Write-Host "Clearing logcat..." -ForegroundColor Cyan
& $adb logcat -c

Write-Host ""
Write-Host "Now open the app on the phone and let it crash." -ForegroundColor Yellow
Write-Host "After it crashes, press ENTER here." -ForegroundColor Yellow
Read-Host

Write-Host "Collecting crash log..." -ForegroundColor Cyan

$raw = & $adb logcat -d -v time
$filtered = $raw | Select-String -Pattern "FATAL EXCEPTION|AndroidRuntime|SchoolInventoryScanner|gr.nyxsystems|mono|DOTNET|System\.|Exception|Fatal signal" -Context 5,20

$filtered | Out-File -FilePath ".\phone-crash.txt" -Encoding utf8

Write-Host ""
Write-Host "Saved crash log to phone-crash.txt" -ForegroundColor Green
Write-Host "Opening Notepad..." -ForegroundColor Cyan
notepad ".\phone-crash.txt"
