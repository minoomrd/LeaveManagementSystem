# Start PostgreSQL Database Browser
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Starting Database Browser" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if PostgreSQL service is running
Write-Host "Checking PostgreSQL service..." -ForegroundColor Yellow
$postgresService = Get-Service | Where-Object {$_.DisplayName -like "*PostgreSQL*" -or $_.Name -like "*postgres*"} | Select-Object -First 1

if ($postgresService) {
    if ($postgresService.Status -ne 'Running') {
        Write-Host "Starting PostgreSQL service: $($postgresService.Name)..." -ForegroundColor Yellow
        Start-Service -Name $postgresService.Name
        Start-Sleep -Seconds 3
        Write-Host "✓ PostgreSQL service started!" -ForegroundColor Green
    } else {
        Write-Host "✓ PostgreSQL service is already running!" -ForegroundColor Green
    }
} else {
    Write-Host "⚠ PostgreSQL service not found!" -ForegroundColor Red
    Write-Host "Please install PostgreSQL first." -ForegroundColor Yellow
    Write-Host "Run: .\install-postgresql.ps1" -ForegroundColor Cyan
    exit 1
}

# Check if port 5432 is accessible
Write-Host ""
Write-Host "Checking PostgreSQL connection..." -ForegroundColor Yellow
$portCheck = Test-NetConnection -ComputerName localhost -Port 5432 -InformationLevel Quiet -WarningAction SilentlyContinue
if (-not $portCheck) {
    Write-Host "⚠ Cannot connect to PostgreSQL on port 5432" -ForegroundColor Red
    Write-Host "Please ensure PostgreSQL is running and configured correctly." -ForegroundColor Yellow
    exit 1
}
Write-Host "✓ PostgreSQL is accessible on port 5432" -ForegroundColor Green

# Download pgweb if not exists
Write-Host ""
Write-Host "Checking pgweb..." -ForegroundColor Yellow
$pgwebPath = ".\pgweb.exe"
if (-not (Test-Path $pgwebPath)) {
    Write-Host "Downloading pgweb..." -ForegroundColor Yellow
    try {
        $latestUrl = "https://github.com/sosedoff/pgweb/releases/latest/download/pgweb_windows_amd64.exe"
        Invoke-WebRequest -Uri $latestUrl -OutFile $pgwebPath -UseBasicParsing
        Write-Host "✓ pgweb downloaded!" -ForegroundColor Green
    } catch {
        Write-Host "✗ Failed to download pgweb: $_" -ForegroundColor Red
        Write-Host "Please download manually from: https://github.com/sosedoff/pgweb/releases" -ForegroundColor Yellow
        exit 1
    }
}

# Start pgweb
Write-Host ""
Write-Host "Starting pgweb on http://localhost:8081..." -ForegroundColor Yellow
Write-Host "Database: LeaveManagementSystem" -ForegroundColor Cyan
Write-Host "Username: postgres" -ForegroundColor Cyan
Write-Host "Password: postgres" -ForegroundColor Cyan
Write-Host ""

# Start pgweb in background
$pgwebProcess = Start-Process -FilePath $pgwebPath -ArgumentList "--host=localhost", "--port=5432", "--user=postgres", "--db=LeaveManagementSystem", "--bind=0.0.0.0", "--listen=8081" -PassThru -WindowStyle Hidden

Start-Sleep -Seconds 2

# Open browser
Write-Host "Opening browser..." -ForegroundColor Yellow
Start-Process "http://localhost:8081"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database Browser Started!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Access at: http://localhost:8081" -ForegroundColor White
Write-Host "Password: postgres" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop pgweb" -ForegroundColor Yellow
Write-Host ""

# Wait for user to stop
try {
    Wait-Process -Id $pgwebProcess.Id
} catch {
    Write-Host "pgweb stopped." -ForegroundColor Yellow
}

