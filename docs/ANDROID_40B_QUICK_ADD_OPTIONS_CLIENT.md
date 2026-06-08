# Android 40b - Quick Add Options Client

## Purpose

Prepare the Android app to consume the new web API endpoint:

```http
GET /api/mobile/quick-add-options
```

This endpoint returns the data needed to make the Android quick-add form cleaner and closer to the web item card.

## Files

- `Models/ApiModels.cs`
- `Services/ScannerApiClient.cs`

## Added models

- `QuickAddOptionsResponse`
- `QuickAddCategoryOptionDto`
- `QuickAddConditionOptionDto`
- `QuickAddGuidanceDto`

## Added API method

```csharp
GetQuickAddOptionsAsync()
```

## No UI changes yet

This patch does not change the quick-add screen.

The next patch will use these options to polish the Android form:

- `Τύπος αντικειμένου`
- `Κατάσταση λειτουργίας`
- clear `Ποσότητα (συνήθως 1)` label
- review message `Προς έλεγχο από web app`
