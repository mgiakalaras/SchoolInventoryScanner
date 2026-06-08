# Release Checklist — School Inventory Scanner v0.4.0

## Before release

Run:

```powershell
git status
dotnet build SchoolInventoryScanner.csproj
```

Expected:

- working tree clean or only intentional release docs/version changes,
- build succeeded.

## Functional test

Test on emulator or Android phone:

1. Open app.
2. Check server URL.
3. Run connection test.
4. Open audit folders.
5. Open a room.
6. Scan an existing QR.
7. Confirm web app updates.
8. Add a new item from:

```text
+ Νέο αντικείμενο στον χώρο
```

9. Confirm it appears in the web app:

```text
/InventoryAudits/MobileFindings
```

10. Confirm QR can be printed from:

```text
/Labels/MobileFindings
```

## Commit release prep

```powershell
git add .
git commit -m "Prepare Android scanner v0.4.0 release"
git push origin main
```

## Tag

```powershell
git tag -a v0.4.0 -m "School Inventory Scanner v0.4.0"
git push origin v0.4.0
```

## GitHub release

Title:

```text
School Inventory Scanner v0.4.0 — Mobile quick add workflow
```

Recommended asset:

- debug APK for sideload, if desired.

## Build APK

```powershell
dotnet publish SchoolInventoryScanner.csproj -f net10.0-android -c Debug /p:AndroidPackageFormat=apk /p:EmbedAssembliesIntoApk=true /p:AndroidUseSharedRuntime=false
```

Find APK:

```powershell
$apk = Get-ChildItem -Recurse -Filter *.apk | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$apk.FullName
explorer /select,$apk.FullName
```
