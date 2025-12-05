# Database Setup Instructions

## Prerequisites

1. **PostgreSQL must be installed and running**
   - Download from: https://www.postgresql.org/download/
   - Default port: 5432
   - Default username: postgres

## Step 1: Create the Database

1. Open PostgreSQL (pgAdmin or psql command line)
2. Create the database:
```sql
CREATE DATABASE LeaveManagementSystem;
```

## Step 2: Update Connection String

Edit `LeaveManagementSystem.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LeaveManagementSystem;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```
Replace `YOUR_PASSWORD` with your PostgreSQL password.

## Step 3: Apply Migrations

Run this command from the Infrastructure project:
```bash
cd "C:\My tasks\Leave Management System\LeaveManagementSystem.Infrastructure"
dotnet ef database update --startup-project ../LeaveManagementSystem.API
```

## Step 4: Seed Data (Automatic)

The database will be automatically seeded with sample data when you start the API:
- **Admin User**: admin@acme.com / Admin123!
- **Employee 1**: john.doe@acme.com / Employee123!
- **Employee 2**: jane.smith@acme.com / Employee123!
- Leave Types: Annual Leave, Sick Leave, Hourly Leave
- Sample leave requests and balances

## Step 5: Start the Backend API

```bash
cd "C:\My tasks\Leave Management System\LeaveManagementSystem.API"
dotnet run
```

## Verification

1. Check Swagger UI: http://localhost:5001/swagger
2. Try the login endpoint with: admin@acme.com / Admin123!
3. Check users endpoint to see seeded data

## Troubleshooting

**Error: "Failed to connect to 127.0.0.1:5432"**
- PostgreSQL is not running
- Start PostgreSQL service
- Check if port 5432 is correct

**Error: "database does not exist"**
- Create the database first (Step 1)

**Error: "password authentication failed"**
- Check your connection string password
- Verify PostgreSQL credentials





