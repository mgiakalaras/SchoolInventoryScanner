# School Inventory Scanner

**School Inventory Scanner** είναι native Android εφαρμογή για επιτόπια απογραφή σχολικού εξοπλισμού με QR codes.

Συνδέεται με το web app **School Inventory Manager** και χρησιμοποιείται μέσα σε αίθουσες/χώρους για:

- επιλογή φακέλου απογραφής,
- επιλογή χώρου,
- σάρωση QR αντικειμένων,
- χειροκίνητη καταχώρηση κωδικού,
- προσθήκη νέου αντικειμένου στον τρέχοντα χώρο,
- συγχρονισμό με το web app.

---

## Current version

```text
v0.4.0
```

Η έκδοση `v0.4.0` αντιστοιχεί στην Android εφαρμογή μετά την προσθήκη του **Mobile Quick Add Item** workflow.

---

## Requirements

- Android συσκευή ή emulator.
- Android 6.0+ / API 23+.
- Πρόσβαση στο ίδιο δίκτυο με το School Inventory Manager web app.
- Ενεργό web app server URL, π.χ.

```text
http://172.26.0.1:5148
```

---

## Main features

### Server connection

Ο χρήστης ορίζει τη διεύθυνση του web app server και κάνει health check.

### Audit folders

Η εφαρμογή φορτώνει τους διαθέσιμους φακέλους απογραφής από το web app.

### Room selection

Ο χρήστης επιλέγει τον χώρο που ελέγχει εκείνη τη στιγμή.

### QR scanning

Η σάρωση QR γίνεται μέσω Google Code Scanner / Google Play Services.

### Manual scan fallback

Αν η κάμερα δεν μπορεί να διαβάσει QR, ο χρήστης μπορεί να περάσει χειροκίνητα τον κωδικό.

### Quick add item

Αν ο χρήστης βρει αντικείμενο στον χώρο που δεν υπάρχει στα αναμενόμενα, μπορεί να το προσθέσει απευθείας από το κινητό.

Η φόρμα περιλαμβάνει:

- Ονομασία
- Κατηγορία
- Μάρκα
- Μοντέλο
- Serial Number
- Ποσότητα
- Σημείωση

Το web app καταγράφει το αντικείμενο ως νέο εύρημα και το εμφανίζει στις σχετικές σελίδες review/QR.

---

## Related web app pages

Στο School Inventory Manager web app:

```text
/InventoryAudits/MobileFindings
/Labels/MobileFindings
```

Χρήση:

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

Τα υπάρχοντα warnings για deprecated Android APIs δεν είναι blockers σε αυτή τη φάση.

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

Δοκιμή σε emulator/κινητό.

Μόνο μετά:

```powershell
git add .
git commit -m "Meaningful commit message"
git push origin main
```

---

## Release workflow

Για release:

```powershell
git tag -a v0.4.0 -m "School Inventory Scanner v0.4.0"
git push origin v0.4.0
```

GitHub release title:

```text
School Inventory Scanner v0.4.0 — Mobile quick add workflow
```

---

## Roadmap

Short term:

- Tablet layout test.
- Splash screen.
- First-run mini tutorial.
- Tooltips / user guidance.
- Better visual polish.
- Safer APK release flow.

Later:

- More structured app settings screen.
- Better offline/error states.
- Optional custom scanner screen if needed.
- Signed release APK/AAB.
