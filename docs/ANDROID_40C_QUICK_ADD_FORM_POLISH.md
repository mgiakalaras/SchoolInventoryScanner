# Android 40c - Quick Add Form Polish

## Purpose

Improve the Android quick-add form so it is closer to the web app item card and clearer for real inventory work.

## What changed

The previous form was too loose and confusing.

This patch changes the workflow to:

1. `Τι βρέθηκε;`
2. `Κατάσταση`
3. `Βασικά στοιχεία`
4. `Σημείωση`

## Important UI fixes

- Replaces `Ονομασία` with `Τύπος αντικειμένου`.
- Loads existing categories/types from the web app using:
  - `GET /api/mobile/quick-add-options`
- Adds a fallback field:
  - `Νέος τύπος αντικειμένου`
- Separates operational condition from review state.
- Adds condition selection:
  - `Λειτουργικό`
  - `Μη λειτουργικό`
  - `Άγνωστο / Προς έλεγχο`
- Keeps review wording:
  - `Προς έλεγχο από web app`
- Fixes the confusing bare `1` by adding:
  - `Ποσότητα (συνήθως 1)`
  - help text explaining when to keep it as 1

## Behavior

When submitting:

- `Name` = selected type or new type
- `CategoryName` = selected type or new type
- `Condition` = selected condition
- `Quantity` = value from the labeled quantity field, default 1
- The item is still reviewed later from the web app

## Requirements

Requires:

- Web patch 40a
- Android patch 40b

## File

- `MainActivity.cs`
