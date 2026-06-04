# Phone crash log guide

Use these scripts from the Android project folder.

## 1. Unblock scripts

```powershell
.\tools\Unblock-DebugTools.ps1
```

## 2. Find adb

```powershell
.\tools\Find-Adb.ps1
```

This creates:

```text
adb-path.txt
```

## 3. Capture crash

```powershell
.\tools\Capture-PhoneCrashLog.ps1
```

When the script tells you:

```text
Now open the app on the phone and let it crash.
After it crashes, press ENTER here.
```

open the app on the phone, wait for the crash, then press ENTER in PowerShell.

The script creates:

```text
phone-crash.txt
```

Send that file/content for debugging.
