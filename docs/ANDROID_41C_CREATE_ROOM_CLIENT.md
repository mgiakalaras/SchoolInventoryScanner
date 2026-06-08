# Android 41c - Create Room API client

## Purpose

Prepare the Android Scanner app to call the new web endpoint for First Inventory / Discovery Mode.

## Web endpoint

```http
POST /api/mobile/audit-folders/{folderId}/rooms/create
```

## Files

- `Models/ApiModels.cs`
- `Services/ScannerApiClient.cs`

## Added models

- `CreateRoomRequest`
- `CreateRoomResponse`
- `CreatedRoomDto`

## Added API client method

```csharp
PostCreateRoomAsync(int folderId, CreateRoomRequest request)
```

## Not included yet

No UI changes in this patch.

The next patch will add the Android button/form:

```text
+ Νέος χώρος
```

## Build

```powershell
dotnet build SchoolInventoryScanner.csproj
```
