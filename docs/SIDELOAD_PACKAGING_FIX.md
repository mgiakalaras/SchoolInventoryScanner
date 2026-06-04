# Sideload packaging fix

## What happened

The phone crash log shows:

```text
No assemblies found in /data/user/0/gr.nyxsystems.schoolinventoryscanner/files/.__override__/arm64-v8a
Assuming this is part of Fast Deployment. Exiting...
ALL entries in APK named lib/arm64-v8a/ MUST be STORED.
Fatal signal 6 (SIGABRT)
```

This means the APK installed on the phone was built in a way that expects Visual Studio/Xamarin fast deployment files.
That is fine for Visual Studio deployment, but not for manual sideload.

## Fix

Build a sideload-safe APK with assemblies embedded inside the APK:

```powershell
dotnet publish SchoolInventoryScanner.csproj -f net10.0-android -c Debug /p:AndroidPackageFormat=apk /p:EmbedAssembliesIntoApk=true /p:AndroidUseSharedRuntime=false
```

## Scripts

```powershell
.\tools\Find-Adb.ps1
.\tools\Build-SideloadApk.ps1
.\tools\Install-CleanSideloadApk.ps1
```

The install script uninstalls the old app first, because the old fast-deploy app data can keep broken override state.
