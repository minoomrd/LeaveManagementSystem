# PostgreSQL Installation Script
# This script will download and install PostgreSQL

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PostgreSQL Installation Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if PostgreSQL is already installed
$postgresPath = "C:\Program Files\PostgreSQL"
if (Test-Path $postgresPath) {
    Write-Host "PostgreSQL appears to be already installed at: $postgresPath" -ForegroundColor Yellow
    $installed = Get-ChildItem $postgresPath -Directory | Select-Object -First 1
    if ($installed) {
        Write-Host "Found version: $($installed.Name)" -ForegroundColor Green
        Write-Host ""
        Write-Host "To proceed with setup:" -ForegroundColor Cyan
        Write-Host "1. Make sure PostgreSQL service is running" -ForegroundColor White
        Write-Host "2. Default port: 5432" -ForegroundColor White
        Write-Host "3. Default username: postgres" -ForegroundColor White
        Write-Host "4. You'll need to set a password during first use" -ForegroundColor White
        exit 0
    }
}

Write-Host "PostgreSQL is not installed." -ForegroundColor Yellow
Write-Host ""
Write-Host "Installation Options:" -ForegroundColor Cyan
Write-Host "1. Automatic installation via winget (recommended)" -ForegroundColor White
Write-Host "2. Manual download and installation" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Enter your choice (1 or 2)"

if ($choice -eq "1") {
    Write-Host ""
    Write-Host "Installing PostgreSQL via winget..." -ForegroundColor Cyan
    Write-Host "This may take a few minutes..." -ForegroundColor Yellow
    Write-Host ""
    
    try {
        # Try to install PostgreSQL
        winget install --id PostgreSQL.PostgreSQL --silent --accept-package-agreements --accept-source-agreements
        
        Write-Host ""
        Write-Host "PostgreSQL installation completed!" -ForegroundColor Green
        Write-Host ""
        Write-Host "IMPORTANT: Please note the password you set during installation!" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Next steps:" -ForegroundColor Cyan
        Write-Host "1. Start PostgreSQL service (it may start automatically)" -ForegroundColor White
        Write-Host "2. Update connection string in appsettings.json with your password" -ForegroundColor White
        Write-Host "3. Run database migrations" -ForegroundColor White
        
    } catch {
        Write-Host ""
        Write-Host "Automatic installation failed. Please use manual installation." -ForegroundColor Red
        Write-Host "Error: $_" -ForegroundColor Red
        Write-Host ""
        Write-Host "Manual Installation:" -ForegroundColor Cyan
        Write-Host "1. Download from: https://www.postgresql.org/download/windows/" -ForegroundColor White
        Write-Host "2. Run the installer" -ForegroundColor White
        Write-Host "3. Remember the password you set!" -ForegroundColor White
    }
} else {
    Write-Host ""
    Write-Host "Manual Installation Instructions:" -ForegroundColor Cyan
    Write-Host "=================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. Download PostgreSQL:" -ForegroundColor White
    Write-Host "   https://www.postgresql.org/download/windows/" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "2. Run the installer and follow these steps:" -ForegroundColor White
    Write-Host "   - Choose installation directory (default is fine)" -ForegroundColor Gray
    Write-Host "   - Select components (all default)" -ForegroundColor Gray
    Write-Host "   - Choose data directory (default is fine)" -ForegroundColor Gray
    Write-Host "   - Set password for 'postgres' user (REMEMBER THIS!)" -ForegroundColor Yellow
    Write-Host "   - Port: 5432 (default)" -ForegroundColor Gray
    Write-Host "   - Locale: Default" -ForegroundColor Gray
    Write-Host ""
    Write-Host "3. After installation:" -ForegroundColor White
    Write-Host "   - PostgreSQL service should start automatically" -ForegroundColor Gray
    Write-Host "   - Update appsettings.json with your password" -ForegroundColor Gray
    Write-Host "   - Run: dotnet ef database update" -ForegroundColor Gray
    Write-Host ""
    
    # Open download page
    $openBrowser = Read-Host "Open download page in browser? (Y/N)"
    if ($openBrowser -eq "Y" -or $openBrowser -eq "y") {
        Start-Process "https://www.postgresql.org/download/windows/"
    }
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")





