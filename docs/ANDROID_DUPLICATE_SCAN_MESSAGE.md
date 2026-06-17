# Android duplicate scan message

## Purpose

Show clear feedback when the user scans an item that has already been scanned in the same room session.

## Requires web patch

Requires web patch 43b, because the web API returns:

```json
{
  "alreadyScanned": true
}
```

## Android behavior

When `AlreadyScanned = true`:

- the message is shown in amber
- the screen waits a bit longer before refreshing
- the user can actually read the message

Example:

```text
Το αντικείμενο έχει ήδη σαρωθεί σε αυτόν τον χώρο.
```

## Files

- `Models/ApiModels.cs`
- `MainActivity.cs`
