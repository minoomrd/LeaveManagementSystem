# PowerShell script to clean, rebuild, and restart the API
Write-Host "Stopping any running API processes..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*LeaveManagementSystem*" } | Stop-Process -Force -ErrorAction SilentlyContinue

Write-Host "`nCleaning the solution..." -ForegroundColor Yellow
Set-Location "Leave Management System"
dotnet clean

Write-Host "`nRebuilding the solution..." -ForegroundColor Yellow
dotnet build --no-incremental

Write-Host "`nStarting the API..." -ForegroundColor Green
Set-Location "LeaveManagementSystem.API"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run"
Set-Location ..

Write-Host "`nAPI should be starting. Please wait a few seconds and then test the endpoint." -ForegroundColor Green



