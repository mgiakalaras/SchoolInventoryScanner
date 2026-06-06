# Android Scanner 0.3.7 - App icon only

## Purpose

Add only the app icon, without touching scanner logic, onboarding, splash, API, or UI flow.

## Files

- `SchoolInventoryScanner.csproj`
- `Properties/AndroidManifest.xml`
- `Resources/drawable/app_icon.xml`

## Version

- Display version: `0.3.7`
- Android version code: `37`

## Build

```powershell
dotnet build SchoolInventoryScanner.csproj
```

## Install

```powershell
.\tools\Find-Adb.ps1
.\tools\Build-SideloadApk.ps1
.\tools\Install-CleanSideloadApk.ps1
```
