# Database Setup Script
# Run this AFTER installing PostgreSQL

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database Setup Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if PostgreSQL is installed
$postgresPath = "C:\Program Files\PostgreSQL"
if (-not (Test-Path $postgresPath)) {
    Write-Host "ERROR: PostgreSQL not found!" -ForegroundColor Red
    Write-Host "Please install PostgreSQL first from: https://www.postgresql.org/download/windows/" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Press any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

# Find PostgreSQL bin directory
$postgresVersions = Get-ChildItem $postgresPath -Directory | Sort-Object Name -Descending
$latestVersion = $postgresVersions[0]
$psqlPath = Join-Path $latestVersion.FullName "bin\psql.exe"

if (-not (Test-Path $psqlPath)) {
    Write-Host "ERROR: psql.exe not found!" -ForegroundColor Red
    Write-Host "Expected at: $psqlPath" -ForegroundColor Yellow
    exit 1
}

Write-Host "Found PostgreSQL at: $($latestVersion.FullName)" -ForegroundColor Green
Write-Host ""

# Get PostgreSQL password
Write-Host "Enter PostgreSQL password for 'postgres' user:" -ForegroundColor Yellow
$securePassword = Read-Host -AsSecureString
$password = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword))

Write-Host ""
Write-Host "Step 1: Creating database..." -ForegroundColor Cyan

# Create database
$env:PGPASSWORD = $password
try {
    & $psqlPath -U postgres -h localhost -c "CREATE DATABASE LeaveManagementSystem;" 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database created successfully!" -ForegroundColor Green
    } else {
        Write-Host "Database might already exist (this is OK)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "Error creating database: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "Step 2: Updating appsettings.json..." -ForegroundColor Cyan

# Update appsettings.json
$appsettingsPath = "LeaveManagementSystem.API\appsettings.json"
if (Test-Path $appsettingsPath) {
    $appsettings = Get-Content $appsettingsPath -Raw | ConvertFrom-Json
    $appsettings.ConnectionStrings.DefaultConnection = "Host=localhost;Port=5432;Database=LeaveManagementSystem;Username=postgres;Password=$password"
    $appsettings | ConvertTo-Json -Depth 10 | Set-Content $appsettingsPath
    Write-Host "✓ appsettings.json updated!" -ForegroundColor Green
} else {
    Write-Host "Warning: appsettings.json not found" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Step 3: Applying migrations..." -ForegroundColor Cyan

# Apply migrations
$infrastructurePath = "LeaveManagementSystem.Infrastructure"
if (Test-Path $infrastructurePath) {
    Push-Location $infrastructurePath
    try {
        dotnet ef database update --startup-project ../LeaveManagementSystem.API
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Migrations applied successfully!" -ForegroundColor Green
        } else {
            Write-Host "Error applying migrations" -ForegroundColor Red
        }
    } catch {
        Write-Host "Error: $_" -ForegroundColor Red
    } finally {
        Pop-Location
    }
} else {
    Write-Host "Warning: Infrastructure project not found" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Start the API: cd LeaveManagementSystem.API && dotnet run" -ForegroundColor White
Write-Host "2. The API will automatically seed sample data" -ForegroundColor White
Write-Host "3. Test users:" -ForegroundColor White
Write-Host "   - Admin: admin@acme.com / Admin123!" -ForegroundColor Gray
Write-Host "   - Employee: john.doe@acme.com / Employee123!" -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")





