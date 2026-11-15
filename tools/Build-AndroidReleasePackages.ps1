param(
    [string]$Project = ".\SchoolInventoryScanner.csproj",
    [string]$KeystorePath = ".\release-signing\school-inventory-scanner-upload.keystore",
    [string]$KeyAlias = "schoolinventoryscanner",
    [string]$VersionName = "0.5.0",
    [int]$VersionCode = 50,
    [string]$OutputDir = ".\artifacts\android-v0.5.0",
    [switch]$SkipClean
)

$ErrorActionPreference = "Stop"

function Convert-SecureStringToPlainText {
    param([securestring]$SecureString)

    $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecureString)
    try {
        return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr)
    }
    finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
    }
}

function Find-LatestFile {
    param(
        [string]$Root,
        [string]$Filter
    )

    return Get-ChildItem -Path $Root -Recurse -Filter $Filter -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1
}

if (-not (Test-Path $Project)) {
    throw "Project file not found: $Project"
}

if (-not (Test-Path $KeystorePath)) {
    throw "Keystore not found: $KeystorePath"
}

$fullKeystorePath = (Resolve-Path $KeystorePath).Path
$fullOutputDir = Join-Path (Get-Location) $OutputDir

New-Item -ItemType Directory -Force -Path $fullOutputDir | Out-Null

Write-Host "Android release package build"
Write-Host "Project:     $Project"
Write-Host "Version:     $VersionName ($VersionCode)"
Write-Host "Keystore:    $fullKeystorePath"
Write-Host "Key alias:   $KeyAlias"
Write-Host "Output dir:  $fullOutputDir"
Write-Host ""

$storePassSecure = Read-Host "Keystore/store password" -AsSecureString
$keyPassSecure = Read-Host "Key password (press Enter if same as store password)" -AsSecureString

$storePass = Convert-SecureStringToPlainText $storePassSecure
$keyPass = Convert-SecureStringToPlainText $keyPassSecure

if ([string]::IsNullOrWhiteSpace($keyPass)) {
    $keyPass = $storePass
}

try {
    if (-not $SkipClean) {
        Write-Host "Cleaning..."
        dotnet clean $Project -c Release | Out-Host
    }

    $commonProps = @(
        "/p:AndroidKeyStore=true",
        "/p:AndroidSigningKeyStore=$fullKeystorePath",
        "/p:AndroidSigningKeyAlias=$KeyAlias",
        "/p:AndroidSigningStorePass=$storePass",
        "/p:AndroidSigningKeyPass=$keyPass",
        "/p:ApplicationDisplayVersion=$VersionName",
        "/p:ApplicationVersion=$VersionCode",
        "/p:EmbedAssembliesIntoApk=true",
        "/p:AndroidUseSharedRuntime=false"
    )

    Write-Host ""
    Write-Host "Building Google Play AAB..."
    dotnet publish $Project `
        -f net10.0-android `
        -c Release `
        "/p:AndroidPackageFormat=aab" `
        @commonProps | Out-Host

    $aab = Find-LatestFile -Root ".\bin\Release" -Filter "*.aab"
    if (-not $aab) {
        throw "AAB output was not found."
    }

    $aabOut = Join-Path $fullOutputDir "SchoolInventoryScanner-v$VersionName-google-play.aab"
    Copy-Item $aab.FullName $aabOut -Force

    Write-Host ""
    Write-Host "Building GitHub APK..."
    dotnet publish $Project `
        -f net10.0-android `
        -c Release `
        "/p:AndroidPackageFormat=apk" `
        @commonProps | Out-Host

    $apk = Find-LatestFile -Root ".\bin\Release" -Filter "*.apk"
    if (-not $apk) {
        throw "APK output was not found."
    }

    $apkOut = Join-Path $fullOutputDir "SchoolInventoryScanner-v$VersionName-github.apk"
    Copy-Item $apk.FullName $apkOut -Force

    Write-Host ""
    Write-Host "Done."
    Write-Host "Google Play AAB:"
    Write-Host "  $aabOut"
    Write-Host "GitHub APK:"
    Write-Host "  $apkOut"
    Write-Host ""
    Write-Host "Open output folder:"
    Write-Host "  explorer `"$fullOutputDir`""
}
finally {
    $storePass = $null
    $keyPass = $null
}
