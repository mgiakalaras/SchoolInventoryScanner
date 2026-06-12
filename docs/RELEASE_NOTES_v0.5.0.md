# School Inventory Scanner v0.5.0

## First inventory room creation workflow

Η έκδοση `v0.5.0` κλειδώνει το Android Scanner ως companion app για τη νέα ροή **Πρώτη απογραφή / από μηδενική βάση** του School Inventory Manager web app.

Η εφαρμογή μπορεί πλέον να συμμετέχει σε απογραφή σχολείου που ξεκινά χωρίς έτοιμη βάση εξοπλισμού.

---

## Κύρια νέα λειτουργικότητα

### Δημιουργία χώρου από Android

Προστέθηκε η επιλογή:

```text
+ Νέος χώρος
```

μέσα στην οθόνη χώρων ενός φακέλου απογραφής.

Ο χρήστης μπορεί να δημιουργήσει χώρους όπως:

- Αίθουσα Α1
- Εργαστήριο Πληροφορικής
- Γραφείο Διευθυντή
- Αποθήκη

Ο χώρος δημιουργείται στο web app και προστίθεται στον ενεργό φάκελο απογραφής.

### Υποστήριξη πρώτης απογραφής

Η ροή πλέον είναι:

1. Web app: δημιουργία φακέλου `Πρώτη απογραφή / από μηδενική βάση`.
2. Android: επιλογή φακέλου.
3. Android: δημιουργία νέου χώρου.
4. Android: άνοιγμα χώρου.
5. Android: προσθήκη νέου αντικειμένου στον χώρο.
6. Web app: review νέων ευρημάτων.
7. Web app: QR labels νέων ευρημάτων.

---

## Quick add item polish

Η φόρμα νέου αντικειμένου έχει καθαρή δομή:

1. `Τι βρέθηκε;`
2. `Κατάσταση`
3. `Βασικά στοιχεία`
4. `Σημείωση`

Υποστηρίζει:

- επιλογή τύπου αντικειμένου από λίστα,
- νέο τύπο αντικειμένου όταν δεν υπάρχει,
- λειτουργική κατάσταση,
- ποσότητα με καθαρή ετικέτα,
- review flag για έλεγχο από web app.

---

## API integration

Η έκδοση χρησιμοποιεί τα εξής web app endpoints:

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

## Warning cleanup

Καθαρίστηκαν τα nullable warnings στο Android quick-add UI.

Η έκδοση κλειδώνει σε κατάσταση:

```text
0 errors
0 warnings
```

---

## Required web app version

Προτείνεται χρήση με:

```text
School Inventory Manager v0.7.0 ή νεότερο
```

---

## Προτεινόμενος GitHub release title

```text
School Inventory Scanner v0.5.0 - First inventory mobile room creation
```

## Προτεινόμενο tag

```text
v0.5.0
```
