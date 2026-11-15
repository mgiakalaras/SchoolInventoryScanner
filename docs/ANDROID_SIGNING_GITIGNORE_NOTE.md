# Release signing safety note

Do not commit these to GitHub:

```text
release-signing/
*.keystore
*.jks
artifacts/
```

Recommended local `.gitignore` entries:

```gitignore
release-signing/
*.keystore
*.jks
artifacts/
```

If your repository already has a `.gitignore`, add these lines there.
