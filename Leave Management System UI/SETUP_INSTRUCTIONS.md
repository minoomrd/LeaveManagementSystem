# Setup Instructions

## Quick Start

### Option 1: Using Command Prompt (Recommended)

1. Open **Command Prompt** (not PowerShell)
2. Navigate to the project folder:
   ```
   cd "C:\My tasks\Leave Management System UI"
   ```
3. Install dependencies:
   ```
   npm install
   ```
4. Start the development server:
   ```
   npm run dev
   ```

### Option 2: Using Batch Files

1. Double-click `setup.bat` to install dependencies and start the server
   OR
2. Double-click `start.bat` to start the server (if dependencies are already installed)

### Option 3: Fix PowerShell Execution Policy

If you want to use PowerShell, run this command first (as Administrator):

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

Then you can use npm commands normally.

## Verify Backend is Running

Before starting the UI, make sure your backend API is running:

1. Check if the API is accessible: http://localhost:5132/api/database/status
2. If not running, start it from the backend project:
   ```
   cd "C:\My tasks\Leave Management System"
   cd LeaveManagementSystem.API
   dotnet run
   ```

## Access the Application

Once the dev server starts, open your browser to:
- **http://localhost:3000**

## Login Credentials

- **Admin**: admin@acme.com / Admin123!
- **Employee**: john.doe@acme.com / Employee123!

## Troubleshooting

### Port 3000 already in use
Change the port in `vite.config.ts` or stop the process using port 3000

### Cannot connect to API
- Ensure backend is running on http://localhost:5132
- Check firewall settings
- Verify proxy configuration in `vite.config.ts`

### npm command not found
- Install Node.js from https://nodejs.org/
- Restart your terminal after installation

