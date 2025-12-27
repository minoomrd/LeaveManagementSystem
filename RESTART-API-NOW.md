# HOW TO RESTART THE API - STEP BY STEP

## The Problem
The API is running OLD compiled code that causes DbContext concurrency errors for regular users.

## The Solution
You MUST completely stop, clean, rebuild, and restart the API.

## Step-by-Step Instructions

### Option 1: Use the Script (Easiest)
1. Open PowerShell in the project root directory
2. Run: `.\force-restart-api.ps1`
3. Wait for it to complete
4. Test the endpoint

### Option 2: Manual Steps

#### Step 1: Kill ALL dotnet processes
```powershell
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force
```
Wait 2-3 seconds to ensure processes are killed.

#### Step 2: Navigate to solution directory
```powershell
cd "Leave Management System"
```

#### Step 3: Clean the solution
```powershell
dotnet clean
```
This removes all compiled files.

#### Step 4: Rebuild (NO incremental)
```powershell
dotnet build --no-incremental
```
This forces a complete rebuild of all projects.

#### Step 5: Start the API
```powershell
cd LeaveManagementSystem.API
dotnet run
```

#### Step 6: Verify it's working
- Wait 5-10 seconds for the API to start
- Check the terminal for "Now listening on: http://localhost:5132"
- Test in browser: http://localhost:5132/api/LeaveRequests/user/YOUR_USER_ID
- Or test in UI: http://localhost:3000/leave-requests (as a regular user)

## Important Notes

1. **DO NOT** just press Ctrl+C and run `dotnet run` again - this won't rebuild!
2. **MUST** run `dotnet clean` first to remove old compiled files
3. **MUST** use `--no-incremental` flag to force full rebuild
4. **VERIFY** the API terminal shows it's listening on port 5132

## How to Verify the New Code is Running

After restarting, check the API terminal output. You should see:
- "Now listening on: http://localhost:5132"
- No errors about DbContext concurrency

If you still see the error, the API is still running old code. Make sure:
- All dotnet processes were killed
- `dotnet clean` completed successfully
- `dotnet build` completed without errors
- The API was started fresh

## Current Code Status

✅ The code is FIXED in the source files
✅ Uses explicit JOINs (no MapToDtoAsync calls)
✅ No DbContext concurrency issues
❌ The API is still running OLD compiled DLL files

**You MUST restart to load the new code!**

