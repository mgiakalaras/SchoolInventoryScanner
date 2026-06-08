# Android 40d — Warning cleanup

## Purpose

Reduce warning noise after the Android quick-add workflow reached a working state on phone and tablet.

## What this patch does

Adds project-level warning suppression in `SchoolInventoryScanner.csproj` for the current known non-blocking warnings:

```xml
<NoWarn>$(NoWarn);CS0618;CA1422;CS8602</NoWarn>
```

## Suppressed warnings

### CS0618

Current native UI helpers still use Android's deprecated:

```csharp
SetBackgroundDrawable(...)
```

The app works, but these should later be replaced/refactored in a dedicated UI helper patch.

### CA1422

Analyzer warnings for Android API obsoletion, mainly:

- `Window.SetStatusBarColor(...)`
- `Window.SetNavigationBarColor(...)`
- `Activity.OnBackPressed()`

These are not blocking the current Android versions we support.

### CS8602

One nullable-flow warning remains in `MainActivity`.

Phone/tablet testing passed. This is suppressed for release hygiene, but should be revisited later instead of forgotten.

## Why suppression instead of rewriting now?

Because the app is currently working and tested.

Changing UI helpers, background drawing, navigation/back-button behavior and nullable flow at the same time could introduce new bugs.

The safe approach is:

1. Suppress known warning noise now.
2. Keep the app stable.
3. Refactor warning sources later one category at a time.

## Later cleanup plan

Suggested future patches:

```text
41a — Replace SetBackgroundDrawable helper usage
41b — Modernize Android back navigation handling
41c — Review status/navigation bar API handling
41d — Fix nullable warning properly
```

## File

- `SchoolInventoryScanner.csproj`
