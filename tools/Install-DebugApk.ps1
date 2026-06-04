param(
    [string]$ApkPath = ""
)

$ErrorActionPreference = "Stop"

function Find-Adb {
    $cmd = Get-Command adb -ErrorAction SilentlyContinue
    if ($cmd) {
        return $cmd.Source
    }

    $candidates = @(
        "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe",
        "$env:ANDROID_HOME\platform-tools\adb.exe",
        "$env:ANDROID_SDK_ROOT\platform-tools\adb.exe"
    )

    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    return $null
}

if ([string]::IsNullOrWhiteSpace($ApkPath)) {
    $ApkPath = Get-ChildItem -Path "." -Recurse -Filter "*.apk" |
        Where-Object { $_.FullName -like "*\bin\Debug\*" } |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1 |
        ForEach-Object { $_.FullName }
}

if ([string]::IsNullOrWhiteSpace($ApkPath) -or !(Test-Path $ApkPath)) {
    Write-Host "Δεν βρέθηκε APK. Πρώτα τρέξε Build-DebugApk.ps1." -ForegroundColor Red
    exit 1
}

$adb = Find-Adb

if ($null -eq $adb) {
    Write-Host "Δεν βρέθηκε adb." -ForegroundColor Red
    Write-Host "Άνοιξε Visual Studio Installer και βεβαιώσου ότι είναι εγκατεστημένο Android SDK Platform Tools." -ForegroundColor Yellow
    Write-Host "Εναλλακτικά κάνε copy το APK στο κινητό και εγκατάσταση χειροκίνητα." -ForegroundColor Yellow
    exit 1
}

Write-Host "ADB:" $adb -ForegroundColor Cyan
Write-Host "APK:" $ApkPath -ForegroundColor Cyan
Write-Host ""

& $adb devices

Write-Host ""
Write-Host "Εγκατάσταση APK..." -ForegroundColor Cyan

& $adb install -r $ApkPath

Write-Host ""
Write-Host "Done. Άνοιξε το School Inventory Scanner στο κινητό." -ForegroundColor Green
