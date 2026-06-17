# Android duplicate scan popup

## Purpose

Show a real popup when the user scans an item that has already been scanned in the same room.

## Why this patch

The previous version only changed the small result text. That was easy to miss because the screen refreshed after scan.

This patch does two things:

1. **Local check before sending to server**
   - The current room session already contains expected items.
   - Items already found have `Scanned = true`.
   - If the user scans the same QR again, the app catches it immediately.

2. **API response fallback**
   - If the web API returns `alreadyScanned = true`, the app also shows the popup.

## User-visible behavior

When scanning the same item again:

```text
Ήδη σκαναρισμένο
Το αντικείμενο έχει ήδη σαρωθεί σε αυτόν τον χώρο.
```

The popup includes the item name and code when available.

## Files

- `MainActivity.cs`

## No web change

This patch is Android-only.
