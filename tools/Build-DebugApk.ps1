param(
    [string]$ProjectPath = ".\SchoolInventoryScanner.csproj"
)

$ErrorActionPreference = "Stop"

if (!(Test-Path $ProjectPath)) {
    Write-Host "Δεν βρέθηκε project file: $ProjectPath" -ForegroundColor Red
    Write-Host "Τρέξε το script μέσα από τον φάκελο του SchoolInventoryScanner ή δώσε -ProjectPath." -ForegroundColor Yellow
    exit 1
}

Write-Host "Building debug APK..." -ForegroundColor Cyan

dotnet restore $ProjectPath
dotnet publish $ProjectPath -f net10.0-android -c Debug /p:AndroidPackageFormat=apk

$projectDir = Split-Path -Parent (Resolve-Path $ProjectPath)
if ([string]::IsNullOrWhiteSpace($projectDir)) {
    $projectDir = "."
}

$apk = Get-ChildItem -Path $projectDir -Recurse -Filter "*.apk" |
    Where-Object { $_.FullName -like "*\bin\Debug\*" } |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if ($null -eq $apk) {
    Write-Host "Δεν βρέθηκε APK στο bin\Debug." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "APK έτοιμο:" -ForegroundColor Green
Write-Host $apk.FullName -ForegroundColor White
Write-Host ""
Write-Host "Μπορείς τώρα να το κάνεις sideload με:" -ForegroundColor Cyan
Write-Host ".\tools\Install-DebugApk.ps1 -ApkPath `"$($apk.FullName)`"" -ForegroundColor White
