# Android 41e - Nullable warning cleanup

## Purpose

Clean the six nullable warnings in `MainActivity.cs` without suppressing them.

## Fixed warnings

The warnings came from nullable values returned by the web API guidance/options object:

- `options.DefaultCategoryName`
- `guidance.NewTypeLabel`
- `guidance.NotesLabel`
- `options.QuantityLabel`
- `guidance.ReviewFlagText`

## Fix strategy

Instead of passing nullable strings directly into UI helpers, the screen now creates safe local fallback strings:

- `primaryFieldLabel`
- `newTypeLabel`
- `conditionLabel`
- `notesLabel`
- `reviewFlagText`
- `quantityLabel`
- `quantityHelpText`
- `defaultTypeName`

## Behavior

No functional change.

The quick-add form still behaves the same, but if the server ever returns missing guidance text, the Android app uses safe Greek fallback labels.

## File

- `MainActivity.cs`
