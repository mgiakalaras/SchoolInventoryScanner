# Release Checklist - School Inventory Scanner v0.5.0

## Build

- [ ] `dotnet build SchoolInventoryScanner.csproj` ολοκληρώνεται χωρίς errors.
- [ ] `dotnet build SchoolInventoryScanner.csproj` ολοκληρώνεται χωρίς warnings.

## Web app dependency

- [ ] School Inventory Manager web app είναι σε έκδοση `v0.7.0` ή νεότερη.
- [ ] Το web app απαντά στο mobile health endpoint:

```powershell
Invoke-RestMethod -Uri "http://192.168.1.80:5148/api/mobile/health"
```

## Android smoke test

- [ ] Η εφαρμογή ανοίγει.
- [ ] Το server URL είναι σωστό.
- [ ] Το health check περνά.
- [ ] Φορτώνει φακέλους απογραφής.
- [ ] Φορτώνει χώρους φακέλου.

## First inventory test

- [ ] Επιλέγεται φάκελος πρώτης απογραφής.
- [ ] Πατιέται `+ Νέος χώρος`.
- [ ] Δημιουργείται δοκιμαστικός χώρος.
- [ ] Ο χώρος εμφανίζεται στη λίστα.
- [ ] Ο χώρος ανοίγει.
- [ ] Πατιέται `+ Νέο αντικείμενο στον χώρο`.
- [ ] Δημιουργείται δοκιμαστικό αντικείμενο.
- [ ] Το αντικείμενο εμφανίζεται στο web app ως νέο εύρημα.

## QR scan test

- [ ] Ανοίγει υπάρχων χώρος με αντικείμενα.
- [ ] Η κάμερα σκανάρει QR.
- [ ] Το αποτέλεσμα αποστέλλεται στο web app.
- [ ] Η σύνοψη ανανεώνεται.

## Sideload APK test

```powershell
dotnet publish SchoolInventoryScanner.csproj -f net10.0-android -c Debug /p:AndroidPackageFormat=apk /p:EmbedAssembliesIntoApk=true /p:AndroidUseSharedRuntime=false
```

- [ ] Δημιουργείται APK.
- [ ] Το APK εγκαθίσταται σε συσκευή.
- [ ] Η εφαρμογή ανοίγει και συνδέεται με το web app.

## Git

```powershell
git status
git add .
git commit -m "Prepare Android release v0.5.0"
git push origin main
git tag v0.5.0
git push origin v0.5.0
```

## GitHub release

Release title:

```text
School Inventory Scanner v0.5.0 - First inventory mobile room creation
```

Use body from:

```text
docs/RELEASE_NOTES_v0.5.0.md
```
