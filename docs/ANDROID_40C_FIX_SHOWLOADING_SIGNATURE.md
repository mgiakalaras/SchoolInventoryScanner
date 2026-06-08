# Android 40c-fix - ShowLoading signature fix

## Problem

Build error:

```text
CS7036 There is no argument given that corresponds to the required parameter 'message' of 'MainActivity.ShowLoading(string, string)'
```

## Cause

`ShowLoading` in this Android project expects two parameters:

```csharp
ShowLoading(string title, string message)
```

Patch 40c called it with one parameter.

## Fix

Changed:

```csharp
ShowLoading("Φόρτωση επιλογών γρήγορης καταχώρησης...");
```

to:

```csharp
ShowLoading("Φόρτωση", "Φόρτωση επιλογών γρήγορης καταχώρησης...");
```

## File

- `MainActivity.cs`

## Notes on warnings

The warning count increase is mostly from existing deprecated Android API calls (`SetBackgroundDrawable`, status/navigation bar color, old back button API).

These are not build blockers, but they should be handled separately in a focused cleanup patch after 40c builds and works.
