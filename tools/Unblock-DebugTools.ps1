Get-ChildItem ".\tools" -Filter "*.ps1" | ForEach-Object {
    Unblock-File $_.FullName
    Write-Host "Unblocked:" $_.FullName -ForegroundColor Green
}
