# Android release packaging

This document describes how to create release files for:

- Google Play
- GitHub Releases

## Important distinction

For Google Play, prepare an Android App Bundle:

```text
.aab
```

For GitHub Releases / sideload installs, prepare an APK:

```text
.apk
```

Google Play can generate optimized APKs from an uploaded Android App Bundle.

## Files added

```text
tools/New-AndroidUploadKeystore.ps1
tools/Build-AndroidReleasePackages.ps1
```

## 1. Create upload keystore

Run once only:

```powershell
.\tools\New-AndroidUploadKeystore.ps1
```

Default output:

```text
release-signing\school-inventory-scanner-upload.keystore
```

Keep this file safe.

Do not commit it to GitHub.

If the key is lost, future updates to the same Google Play app can become difficult or impossible without Play Console key reset/recovery workflows.

## 2. Build release packages

After the keystore exists:

```powershell
.\tools\Build-AndroidReleasePackages.ps1
```

Default version:

```text
0.5.0
```

Default version code:

```text
50
```

Outputs:

```text
artifacts\android-v0.5.0\SchoolInventoryScanner-v0.5.0-google-play.aab
artifacts\android-v0.5.0\SchoolInventoryScanner-v0.5.0-github.apk
```

## 3. Upload targets

### Google Play

Upload:

```text
SchoolInventoryScanner-v0.5.0-google-play.aab
```

### GitHub Release

Attach:

```text
SchoolInventoryScanner-v0.5.0-github.apk
```

Optionally also attach the `.aab` as a build artifact, but normal users need the `.apk`.

## 4. Version updates

For the next release, increase both:

```text
ApplicationDisplayVersion
ApplicationVersion
```

Example:

```text
0.5.1
51
```

or:

```text
0.6.0
60
```

Google Play requires every uploaded update to use a higher version code than previous uploads.
