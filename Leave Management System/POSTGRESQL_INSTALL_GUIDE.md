# PostgreSQL Installation Guide

## Quick Installation (Recommended)

### Option 1: Using Windows Package Manager (winget)

Open PowerShell as Administrator and run:

```powershell
winget install PostgreSQL.PostgreSQL
```

**OR** if that doesn't work, try:

```powershell
winget install --id PostgreSQL.PostgreSQL --source winget
```

### Option 2: Manual Download

1. **Download PostgreSQL:**
   - Go to: https://www.postgresql.org/download/windows/
   - Click "Download the installer"
   - Download the latest version (16.x recommended)

2. **Run the Installer:**
   - Double-click the downloaded `.exe` file
   - Click "Next" through the welcome screens
   - **Important Settings:**
     - Installation Directory: `C:\Program Files\PostgreSQL\16` (default is fine)
     - Components: Select all (default)
     - Data Directory: `C:\Program Files\PostgreSQL\16\data` (default is fine)
     - **Password for 'postgres' user: SET A PASSWORD AND REMEMBER IT!** ⚠️
     - Port: `5432` (default - keep this!)
     - Locale: Default (or your preference)
   - Click "Next" and "Finish"

3. **Verify Installation:**
   - PostgreSQL service should start automatically
   - You can verify in Services (services.msc) - look for "postgresql-x64-16"

## After Installation

### Step 1: Update Connection String

Edit `LeaveManagementSystem.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LeaveManagementSystem;Username=postgres;Password=YOUR_PASSWORD_HERE"
  }
}
```

Replace `YOUR_PASSWORD_HERE` with the password you set during installation.

### Step 2: Create the Database

Open **pgAdmin** (installed with PostgreSQL) or use **psql** command line:

**Using pgAdmin:**
1. Open pgAdmin from Start Menu
2. Connect to PostgreSQL server (password: your password)
3. Right-click "Databases" → "Create" → "Database"
4. Name: `LeaveManagementSystem`
5. Click "Save"

**Using psql (Command Line):**
```sql
psql -U postgres
-- Enter your password when prompted
CREATE DATABASE LeaveManagementSystem;
\q
```

**OR** using SQL directly:
```powershell
psql -U postgres -c "CREATE DATABASE LeaveManagementSystem;"
```

### Step 3: Apply Migrations

Run this command:

```powershell
cd "C:\My tasks\Leave Management System\LeaveManagementSystem.Infrastructure"
dotnet ef database update --startup-project ../LeaveManagementSystem.API
```

### Step 4: Start the API

The API will automatically seed sample data on startup:

```powershell
cd "C:\My tasks\Leave Management System\LeaveManagementSystem.API"
dotnet run
```

## Test Data

After starting the API, you'll have:

**Users:**
- Admin: `admin@acme.com` / `Admin123!`
- Employee 1: `john.doe@acme.com` / `Employee123!`
- Employee 2: `jane.smith@acme.com` / `Employee123!`

**Leave Types:**
- Annual Leave (20 days/year)
- Sick Leave (10 days/year)
- Hourly Leave (40 hours/month)

**Sample Data:**
- Leave balances for employees
- Sample leave requests

## Troubleshooting

### "Failed to connect to 127.0.0.1:5432"
- **Solution:** Start PostgreSQL service
  ```powershell
  Start-Service postgresql-x64-16
  ```
  Or use Services app (services.msc) and start "postgresql-x64-16"

### "password authentication failed"
- **Solution:** Check your password in appsettings.json
- Make sure you're using the password you set during installation

### "database does not exist"
- **Solution:** Create the database (Step 2 above)

### "psql: command not found"
- **Solution:** Add PostgreSQL to PATH:
  - Add `C:\Program Files\PostgreSQL\16\bin` to your system PATH
  - Or use full path: `"C:\Program Files\PostgreSQL\16\bin\psql.exe"`

## Quick Start Script

After installing PostgreSQL, run:

```powershell
# 1. Create database
& "C:\Program Files\PostgreSQL\16\bin\psql.exe" -U postgres -c "CREATE DATABASE LeaveManagementSystem;"

# 2. Update appsettings.json with your password (edit manually)

# 3. Apply migrations
cd "C:\My tasks\Leave Management System\LeaveManagementSystem.Infrastructure"
dotnet ef database update --startup-project ../LeaveManagementSystem.API

# 4. Start API (will auto-seed data)
cd ..\LeaveManagementSystem.API
dotnet run
```

## Need Help?

If you encounter issues:
1. Check PostgreSQL service is running
2. Verify connection string in appsettings.json
3. Check PostgreSQL logs: `C:\Program Files\PostgreSQL\16\data\log\`





