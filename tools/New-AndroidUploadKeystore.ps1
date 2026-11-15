param(
    [string]$OutputDir = ".\release-signing",
    [string]$KeystoreName = "school-inventory-scanner-upload.keystore",
    [string]$Alias = "schoolinventoryscanner"
)

$ErrorActionPreference = "Stop"

function Find-Keytool {
    $cmd = Get-Command keytool -ErrorAction SilentlyContinue
    if ($cmd) {
        return $cmd.Source
    }

    if ($env:JAVA_HOME) {
        $candidate = Join-Path $env:JAVA_HOME "bin\keytool.exe"
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    throw "keytool was not found. Install/use JDK or set JAVA_HOME."
}

$keytool = Find-Keytool

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
$keystorePath = Join-Path $OutputDir $KeystoreName

if (Test-Path $keystorePath) {
    throw "Keystore already exists: $keystorePath"
}

Write-Host "Creating Android upload keystore..."
Write-Host "Output: $keystorePath"
Write-Host "Alias:  $Alias"
Write-Host ""
Write-Host "IMPORTANT:"
Write-Host "- Keep this keystore safe."
Write-Host "- Do not commit it to GitHub."
Write-Host "- Losing it can block future app updates."
Write-Host ""

& $keytool `
    -genkeypair `
    -v `
    -keystore $keystorePath `
    -alias $Alias `
    -keyalg RSA `
    -keysize 2048 `
    -validity 10000

Write-Host ""
Write-Host "Done."
Write-Host "Keystore: $keystorePath"
Write-Host ""
Write-Host "Next:"
Write-Host ".\tools\Build-AndroidReleasePackages.ps1"
