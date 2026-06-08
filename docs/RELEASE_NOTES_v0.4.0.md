# Release Notes — School Inventory Scanner v0.4.0

## Summary

`v0.4.0` is the Android scanner release that adds support for the mobile quick-add workflow.

The app can now be used not only to scan expected QR labels, but also to add a newly discovered item directly from the current room during an audit.

---

## Highlights

### App icon

Adds a proper launcher icon for the Android scanner app.

### Server connection

The app connects to the School Inventory Manager web app and uses its mobile API.

### Audit folders and room sessions

The app can:

- list audit folders,
- show room sessions,
- open a room,
- display expected items and room progress.

### QR scanning

Uses Google Code Scanner / Google Play Services for camera-based QR scanning.

### Manual fallback

Allows manual code entry when QR scanning is not possible.

### Quick add new item

Inside a room session, the user can press:

```text
+ Νέο αντικείμενο στον χώρο
```

and submit a basic item form.

Fields:

- Ονομασία
- Κατηγορία
- Μάρκα
- Μοντέλο
- Serial Number
- Ποσότητα
- Σημείωση

The item is sent to the web app endpoint:

```text
POST /api/mobile/room-sessions/{id}/add-item
```

The web app then:

- creates the inventory item,
- links it to the current room,
- marks it as found,
- marks it as a mobile audit finding,
- makes it available for review and QR printing.

---

## Required web app version

Recommended web app release:

```text
School Inventory Manager v0.6.0
```

This Android version expects the web app to include the mobile quick-add API and mobile findings workflow.

---

## Known warnings

Current build has warnings related to:

- deprecated Android APIs,
- nullable reference warning in one call path.

These warnings do not block the current release.

---

## Suggested tag

```powershell
git tag -a v0.4.0 -m "School Inventory Scanner v0.4.0"
git push origin v0.4.0
```

---

## GitHub release title

```text
School Inventory Scanner v0.4.0 — Mobile quick add workflow
```

---

## GitHub release description

```text
This release adds the Android mobile quick-add workflow for School Inventory Manager. Users can scan expected QR labels and add newly discovered items directly from the current room during an audit. The app works with School Inventory Manager v0.6.0 and its mobile findings/QR label workflow.
```
