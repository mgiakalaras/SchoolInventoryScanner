# Android 41d - Create Room UI

## Purpose

Add the Android UI needed for First Inventory / Discovery Mode.

A user can now create a room from the Android app while inside an audit folder.

## Added UI

Inside the folder rooms screen:

```text
+ Νέος χώρος
```

This opens a simple form:

```text
Όνομα χώρου *
```

Examples:

- Αίθουσα Α1
- Εργαστήριο Πληροφορικής
- Γραφείο Διευθυντή
- Αποθήκη

## Behavior

On submit, the app calls:

```csharp
_api.PostCreateRoomAsync(folder.Id, new CreateRoomRequest { Name = name })
```

The web app endpoint creates/reuses the room and adds it to the current audit folder.

After success, the Android app reloads the rooms list.

## Why

This is essential for schools doing their first inventory from zero:

1. create room from mobile/tablet
2. open room
3. add discovered items
4. review later from PC
5. print QR labels and final folder

## Requirements

Requires:

- Web patch 41b + 41b-fix
- Android patch 41c

## File

- `MainActivity.cs`
