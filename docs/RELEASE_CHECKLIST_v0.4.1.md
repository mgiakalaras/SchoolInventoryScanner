# Release Checklist — School Inventory Scanner v0.4.1

## Before release

Run:

```powershell
git status
dotnet build SchoolInventoryScanner.csproj
```

Expected:

- build succeeded,
- no unexpected untracked/nested files.

## Functional test

Test on emulator, phone, or tablet:

1. Open app.
2. Check server URL.
3. Run connection test.
4. Open audit folders.
5. Open a room.
6. Tap:

```text
+ Νέο αντικείμενο στον χώρο
```

7. Confirm the quick-add form shows:
   - Τύπος αντικειμένου
   - Νέος τύπος αντικειμένου
   - Κατάσταση λειτουργίας
   - Μάρκα
   - Μοντέλο
   - Serial Number
   - Ποσότητα (συνήθως 1)
   - Σημείωση
8. Add a test item.
9. Confirm it appears in the web app:

```text
/InventoryAudits/MobileFindings
```

10. Confirm QR can be printed from:

```text
/Labels/MobileFindings
```

## Commit

```powershell
git add .
git commit -m "Update Android scanner release docs to v0.4.1"
git push origin main
```

## Tag

```powershell
git tag -a v0.4.1 -m "School Inventory Scanner v0.4.1"
git push origin v0.4.1
```

## GitHub release

Title:

```text
School Inventory Scanner v0.4.1 — Quick add form polish
```

Recommended asset:

- debug APK for sideload, if desired.

## Next patch

```text
40d — Android warnings cleanup
```
