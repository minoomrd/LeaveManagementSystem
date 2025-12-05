# Quick Fix: PostgreSQL Connection Error

## The Error
You're seeing connection errors because **PostgreSQL is not installed or not running**.

## Quick Solution

### Option 1: Install PostgreSQL (Recommended)

1. **Double-click:** `INSTALL-POSTGRESQL.bat`
   - This will guide you through installation

2. **After installation:**
   - Update `LeaveManagementSystem.API/appsettings.json`
   - Change `Password=postgres` to your actual PostgreSQL password
   - Run `SETUP-POSTGRESQL-DATABASE.bat` to create the database
   - Restart the backend API

### Option 2: Continue Without Database

The API will still work, but:
- Database endpoints will show errors
- Swagger UI will work fine
- You can test API endpoints that don't require database

## What Changed

✅ **Retries are now disabled** - API fails fast instead of retrying 3 times
✅ **2-second timeout** - Connection attempts fail quickly
✅ **Better error messages** - Clear messages about what's wrong

## Verify Installation

Check if PostgreSQL is installed:
```powershell
Get-Service | Where-Object { $_.Name -like "*postgresql*" }
```

If nothing shows, PostgreSQL is not installed.

## Manual Installation

1. Download: https://www.postgresql.org/download/windows/
2. Install with default settings
3. **Remember your password!**
4. Update `appsettings.json` with your password
5. Run `SETUP-POSTGRESQL-DATABASE.bat`



