# Release Notes — School Inventory Scanner v0.4.1

## Summary

`v0.4.1` improves the Android quick-add item workflow after testing on phone and tablet.

The goal of this release is to make the mobile quick-add form clearer, more familiar, and closer to the web app item card.

---

## Highlights

### Quick add form polish

The quick-add form is now organized into clearer sections:

1. `Τι βρέθηκε;`
2. `Κατάσταση`
3. `Βασικά στοιχεία`
4. `Σημείωση`

### Better primary field

The generic field:

```text
Ονομασία
```

has been replaced with:

```text
Τύπος αντικειμένου
```

This better matches the inventory workflow.

### Existing types/categories from web app

The Android app now loads available item categories/types from the web app through:

```text
GET /api/mobile/quick-add-options
```

### New type fallback

If the type does not exist in the list, the user can fill:

```text
Νέος τύπος αντικειμένου
```

### Operational condition

The form now separates operational condition from review state.

Condition choices include:

- Λειτουργικό
- Μη λειτουργικό
- Άγνωστο / Προς έλεγχο

### Review state remains separate

The item still remains:

```text
Προς έλεγχο από web app
```

so it can be checked and corrected later from the School Inventory Manager web app.

### Clear quantity label

The confusing bare default number `1` is now shown as:

```text
Ποσότητα (συνήθως 1)
```

with guidance that normal equipment should usually remain quantity `1`.

---

## Tested

Tested successfully on:

- Android phone
- Android tablet

---

## Required web app support

Requires the web app endpoint:

```text
GET /api/mobile/quick-add-options
```

Recommended web app release:

```text
School Inventory Manager v0.6.0+
```

---

## Known warnings

Build warnings remain for deprecated Android APIs and one nullable warning.

These are not blockers, but should be cleaned up in a separate patch.

---

## Suggested tag

```powershell
git tag -a v0.4.1 -m "School Inventory Scanner v0.4.1"
git push origin v0.4.1
```

---

## GitHub release title

```text
School Inventory Scanner v0.4.1 — Quick add form polish
```

---

## GitHub release description

```text
This release improves the Android quick-add item form. It replaces the generic name field with a clearer item type workflow, loads existing categories/types from the web app, adds an operational condition selector, separates review state from condition, and makes the quantity field clearer. Tested successfully on phone and tablet.
```
