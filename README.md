# School Inventory Scanner

**School Inventory Scanner** είναι native Android εφαρμογή για επιτόπια απογραφή σχολικού εξοπλισμού με QR codes.

Συνδέεται με το web app **School Inventory Manager** και χρησιμοποιείται μέσα σε αίθουσες/χώρους για:

- επιλογή φακέλου απογραφής,
- επιλογή χώρου,
- δημιουργία νέου χώρου σε φάκελο πρώτης απογραφής,
- σάρωση QR αντικειμένων,
- χειροκίνητη καταχώρηση κωδικού,
- προσθήκη νέου αντικειμένου στον τρέχοντα χώρο,
- συγχρονισμό με το web app.

---

## Current version

```text
v0.5.0
```

Η έκδοση `v0.5.0` αντιστοιχεί στο Android Scanner μετά την ολοκλήρωση της ροής **Πρώτη απογραφή / από μηδενική βάση**.

Η εφαρμογή μπορεί πλέον να χρησιμοποιηθεί σε σχολείο που ξεκινά απογραφή από το μηδέν:

1. επιλογή φακέλου πρώτης απογραφής,
2. δημιουργία νέου χώρου από κινητό/tablet,
3. άνοιγμα χώρου,
4. προσθήκη νέου αντικειμένου,
5. έλεγχος/διόρθωση από το web app,
6. εκτύπωση QR labels για νέα ευρήματα.

---

## Requirements

- Android συσκευή ή emulator.
- Android 6.0+ / API 23+.
- Πρόσβαση στο ίδιο δίκτυο με το School Inventory Manager web app.
- School Inventory Manager web app `v0.7.0` ή νεότερο.
- Ενεργό web app server URL, π.χ.

```text
http://192.168.1.80:5148
```

---

## Main features

### Server connection

Ο χρήστης ορίζει τη διεύθυνση του web app server και κάνει health check.

### Audit folders

Η εφαρμογή φορτώνει τους διαθέσιμους φακέλους απογραφής από το web app.

### First inventory room creation

Σε φάκελο πρώτης απογραφής, ο χρήστης μπορεί να δημιουργήσει νέο χώρο απευθείας από Android:

```text
+ Νέος χώρος
```

Παραδείγματα χώρων:

- Αίθουσα Α1
- Εργαστήριο Πληροφορικής
- Γραφείο Διευθυντή
- Αποθήκη

Ο χώρος δημιουργείται στο web app και εμφανίζεται στη λίστα χώρων του φακέλου.

### Room selection

Ο χρήστης επιλέγει τον χώρο που ελέγχει εκείνη τη στιγμή.

### QR scanning

Η σάρωση QR γίνεται μέσω Google Code Scanner / Google Play Services.

### Manual scan fallback

Αν η κάμερα δεν μπορεί να διαβάσει QR, ο χρήστης μπορεί να περάσει χειροκίνητα τον κωδικό.

### Quick add new item

Αν ο χρήστης βρει αντικείμενο στον χώρο που δεν υπάρχει στα αναμενόμενα, μπορεί να το προσθέσει απευθείας από το κινητό.

Η φόρμα είναι χωρισμένη σε καθαρές ενότητες:

1. **Τι βρέθηκε;**
2. **Κατάσταση**
3. **Βασικά στοιχεία**
4. **Σημείωση**

Η φόρμα υποστηρίζει:

- Τύπος αντικειμένου
- Νέος τύπος αντικειμένου, αν δεν υπάρχει στη λίστα
- Κατάσταση λειτουργίας
- Μάρκα
- Μοντέλο
- Serial Number
- Ποσότητα (συνήθως 1)
- Σημείωση

Το αντικείμενο περνάει στο web app ως νέο εύρημα και μένει για έλεγχο/διόρθωση πριν την εκτύπωση QR.

---

## Related web app endpoints

Το Android Scanner χρησιμοποιεί mobile endpoints όπως:

```text
/api/mobile/health
/api/mobile/audit-folders
/api/mobile/audit-folders/{folderId}/rooms
/api/mobile/audit-folders/{folderId}/rooms/create
/api/mobile/room-sessions/{roomSessionId}
/api/mobile/room-sessions/{roomSessionId}/scan
/api/mobile/room-sessions/{roomSessionId}/add-item
/api/mobile/quick-add-options
```

---

## Related web app pages

Στο School Inventory Manager web app:

```text
/InventoryAudits
/InventoryAudits/MobileFindings
/Labels/MobileFindings
/Scanner
```

Χρήση:

- δημιουργία φακέλου πρώτης απογραφής,
- έλεγχος νέων ευρημάτων,
- επεξεργασία στοιχείων,
- QR labels μόνο για νέα ευρήματα,
- συνέχιση απογραφής.

---

## Build

Από τον φάκελο του project:

```powershell
dotnet build SchoolInventoryScanner.csproj
```

Η έκδοση `v0.5.0` κλειδώνει σε κατάσταση:

```text
0 errors
0 warnings
```

---

## Debug / deploy from Visual Studio

Άνοιγμα:

```text
SchoolInventoryScanner.csproj
```

Μετά επιλογή emulator ή συνδεδεμένη Android συσκευή και Run/Deploy από Visual Studio.

---

## Sideload APK

Για δημιουργία debug APK:

```powershell
dotnet publish SchoolInventoryScanner.csproj -f net10.0-android -c Debug /p:AndroidPackageFormat=apk /p:EmbedAssembliesIntoApk=true /p:AndroidUseSharedRuntime=false
```

Μετά εντοπισμός APK:

```powershell
$apk = Get-ChildItem -Recurse -Filter *.apk | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$apk.FullName
explorer /select,$apk.FullName
```

Το APK μπορεί να περαστεί στο κινητό με USB, Drive, OneDrive, LocalSend ή άλλο τρόπο.

---

## Development workflow

Πρώτα έλεγχος:

```powershell
git status
dotnet build SchoolInventoryScanner.csproj
```

Μετά από αλλαγή:

```powershell
dotnet build SchoolInventoryScanner.csproj
```

Δοκιμή σε emulator/συσκευή και μετά commit.

---

## Suggested version tag

```text
v0.5.0
```
