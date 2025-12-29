# Force restart API script - completely stops, cleans, rebuilds, and restarts
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FORCE RESTART API - CLEAN BUILD" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Step 1: Kill ALL dotnet processes
Write-Host "`n[1/5] Stopping ALL dotnet processes..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "   ✓ All dotnet processes stopped" -ForegroundColor Green

# Step 2: Navigate to solution directory
Write-Host "`n[2/5] Navigating to solution directory..." -ForegroundColor Yellow
Set-Location "Leave Management System"
Write-Host "   ✓ Current directory: $(Get-Location)" -ForegroundColor Green

# Step 3: Clean solution
Write-Host "`n[3/5] Cleaning solution (removing all build artifacts)..." -ForegroundColor Yellow
dotnet clean --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Solution cleaned" -ForegroundColor Green
} else {
    Write-Host "   ✗ Clean failed" -ForegroundColor Red
    exit 1
}

# Step 4: Rebuild solution (no incremental)
Write-Host "`n[4/5] Rebuilding solution (full rebuild, no incremental)..." -ForegroundColor Yellow
dotnet build --no-incremental --verbosity minimal
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Solution rebuilt successfully" -ForegroundColor Green
} else {
    Write-Host "   ✗ Build failed - check errors above" -ForegroundColor Red
    exit 1
}

# Step 5: Start API
Write-Host "`n[5/5] Starting API..." -ForegroundColor Yellow
Set-Location "LeaveManagementSystem.API"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Write-Host 'API Starting...' -ForegroundColor Green; dotnet run"
Set-Location "..\.."

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "API RESTART COMPLETE!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "`nThe API should be starting in a new window." -ForegroundColor Yellow
Write-Host "Wait 5-10 seconds for it to fully start, then test the endpoint." -ForegroundColor Yellow
Write-Host "`nTest URL: http://localhost:5132/api/LeaveRequests/user/YOUR_USER_ID" -ForegroundColor Cyan



