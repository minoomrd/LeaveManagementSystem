# Script to view database contents
# This script shows all data in the LeaveManagementSystem database

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database Viewer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Find PostgreSQL
$postgresPath = "C:\Program Files\PostgreSQL"
if (-not (Test-Path $postgresPath)) {
    Write-Host "ERROR: PostgreSQL not found!" -ForegroundColor Red
    Write-Host "Please install PostgreSQL first." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Press any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

$postgresVersions = Get-ChildItem $postgresPath -Directory | Sort-Object Name -Descending
$latestVersion = $postgresVersions[0]
$psqlPath = Join-Path $latestVersion.FullName "bin\psql.exe"

if (-not (Test-Path $psqlPath)) {
    Write-Host "ERROR: psql.exe not found!" -ForegroundColor Red
    exit 1
}

Write-Host "PostgreSQL found: $($latestVersion.Name)" -ForegroundColor Green
Write-Host ""

# Get password
Write-Host "Enter PostgreSQL password for 'postgres' user:" -ForegroundColor Yellow
$securePassword = Read-Host -AsSecureString
$password = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword))

$env:PGPASSWORD = $password

Write-Host ""
Write-Host "Connecting to database..." -ForegroundColor Cyan
Write-Host ""

# SQL queries to show all data
$queries = @"
-- Users
\echo '========================================'
\echo 'USERS'
\echo '========================================'
SELECT id, full_name, email, role, status, company_id, created_at FROM users ORDER BY created_at;

-- Companies
\echo ''
\echo '========================================'
\echo 'COMPANIES'
\echo '========================================'
SELECT id, name, contact_email, owner_id, status, created_at FROM companies;

-- Leave Types
\echo ''
\echo '========================================'
\echo 'LEAVE TYPES'
\echo '========================================'
SELECT id, name, unit, description, is_sick_leave, created_at FROM leave_types;

-- Leave Policies
\echo ''
\echo '========================================'
\echo 'LEAVE POLICIES'
\echo '========================================'
SELECT id, leave_type_id, entitlement_amount, entitlement_unit, renewal_period FROM leave_policies;

-- Leave Balances
\echo ''
\echo '========================================'
\echo 'LEAVE BALANCES'
\echo '========================================'
SELECT lb.id, u.full_name, lt.name as leave_type, lb.balance_amount, lb.balance_unit, lb.updated_at 
FROM leave_balances lb
JOIN users u ON lb.user_id = u.id
JOIN leave_types lt ON lb.leave_type_id = lt.id
ORDER BY u.full_name, lt.name;

-- Leave Requests
\echo ''
\echo '========================================'
\echo 'LEAVE REQUESTS'
\echo '========================================'
SELECT lr.id, u.full_name, lt.name as leave_type, lr.start_datetime, lr.end_datetime, 
       lr.duration_amount, lr.duration_unit, lr.status, lr.reason, lr.admin_comment, lr.created_at
FROM leave_requests lr
JOIN users u ON lr.user_id = u.id
JOIN leave_types lt ON lr.leave_type_id = lt.id
ORDER BY lr.created_at DESC;

-- Summary
\echo ''
\echo '========================================'
\echo 'SUMMARY'
\echo '========================================'
SELECT 
    (SELECT COUNT(*) FROM users) as total_users,
    (SELECT COUNT(*) FROM companies) as total_companies,
    (SELECT COUNT(*) FROM leave_types) as total_leave_types,
    (SELECT COUNT(*) FROM leave_requests) as total_leave_requests,
    (SELECT COUNT(*) FROM leave_balances) as total_leave_balances;
"@

# Execute queries
try {
    $queries | & $psqlPath -U postgres -d LeaveManagementSystem -h localhost
} catch {
    Write-Host "Error connecting to database: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Make sure:" -ForegroundColor Yellow
    Write-Host "1. PostgreSQL service is running" -ForegroundColor White
    Write-Host "2. Database 'LeaveManagementSystem' exists" -ForegroundColor White
    Write-Host "3. Migrations have been applied" -ForegroundColor White
    Write-Host "4. Password is correct" -ForegroundColor White
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")





