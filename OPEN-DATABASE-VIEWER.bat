@echo off
cls
color 0E
title PostgreSQL Database Viewer
echo.
echo ========================================
echo   Opening PostgreSQL Database Viewer
echo ========================================
echo.

REM Check if backend is running
echo Checking if backend API is running...
curl -s http://localhost:5132/api/database/status >nul 2>&1
if errorlevel 1 (
    echo.
    echo [WARNING] Backend API is not running!
    echo.
    echo Starting backend API first...
    echo.
    start "Backend API" cmd /k "cd /d "%~dp0Leave Management System\LeaveManagementSystem.API" && dotnet run"
    echo Waiting for API to start...
    timeout /t 8 /nobreak >nul
)

echo.
echo Opening database viewer in browser...
echo.
echo The viewer will show:
echo   - Users
echo   - Companies  
echo   - Leave Requests
echo   - Leave Balances
echo.
echo Make sure backend is running on: http://localhost:5132
echo.

start "" "%~dp0VIEW-DATABASE.html"

echo.
echo Database viewer opened!
echo.
echo If you see connection errors:
echo   1. Make sure backend API is running
echo   2. Check: http://localhost:5001/swagger
echo   3. Verify PostgreSQL is connected
echo.
pause

