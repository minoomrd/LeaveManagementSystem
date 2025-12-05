# Script to start Pgweb database viewer
Write-Host "Starting PostgreSQL Web Viewer..." -ForegroundColor Green

# Check if pgweb exists
$pgwebPath = ".\pgweb.exe"
if (-not (Test-Path $pgwebPath)) {
    Write-Host "Pgweb not found. Downloading..." -ForegroundColor Yellow
    $downloadUrl = "https://github.com/sosedoff/pgweb/releases/latest/download/pgweb_windows_amd64.exe"
    Invoke-WebRequest -Uri $downloadUrl -OutFile $pgwebPath
    Write-Host "Downloaded pgweb.exe" -ForegroundColor Green
}

# Start pgweb
Write-Host "Starting Pgweb on http://localhost:8081" -ForegroundColor Green
Write-Host "Database: LeaveManagementSystem" -ForegroundColor Cyan
Write-Host "Username: postgres" -ForegroundColor Cyan
Write-Host "Password: postgres" -ForegroundColor Cyan
Write-Host ""
Write-Host "Opening browser..." -ForegroundColor Yellow

Start-Process "http://localhost:8081"
Start-Process $pgwebPath -ArgumentList "--host=localhost", "--port=5432", "--user=postgres", "--db=LeaveManagementSystem", "--bind=0.0.0.0", "--listen=8081"

