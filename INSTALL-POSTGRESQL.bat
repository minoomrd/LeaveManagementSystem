@echo off
cls
color 0E
title Install PostgreSQL
echo.
echo ========================================
echo   PostgreSQL Installation Guide
echo ========================================
echo.
echo PostgreSQL is required for the database to work.
echo.
echo Choose installation method:
echo.
echo [1] Install using winget (Recommended - Automatic)
echo [2] Manual download and install
echo [3] Skip for now (API will work but database won't)
echo.
set /p choice="Enter choice (1, 2, or 3): "

if "%choice%"=="1" (
    echo.
    echo Installing PostgreSQL using winget...
    echo.
    winget install PostgreSQL.PostgreSQL
    if errorlevel 1 (
        echo.
        echo [ERROR] Installation failed!
        echo.
        echo Please try manual installation (option 2)
        echo.
        pause
        exit /b 1
    )
    echo.
    echo ✅ PostgreSQL installed!
    echo.
    echo IMPORTANT: Remember the password you set during installation!
    echo.
    echo Next steps:
    echo   1. Update appsettings.json with your PostgreSQL password
    echo   2. Run: SETUP-POSTGRESQL-DATABASE.bat
    echo   3. Restart the backend API
    echo.
    pause
    exit /b 0
)

if "%choice%"=="2" (
    echo.
    echo Opening PostgreSQL download page...
    echo.
    start https://www.postgresql.org/download/windows/
    echo.
    echo After installation:
    echo   1. Remember your PostgreSQL password!
    echo   2. Update appsettings.json with your password
    echo   3. Run: SETUP-POSTGRESQL-DATABASE.bat
    echo   4. Restart the backend API
    echo.
    pause
    exit /b 0
)

if "%choice%"=="3" (
    echo.
    echo Skipping PostgreSQL installation.
    echo.
    echo The API will run but database operations will fail.
    echo You can still view Swagger UI and test endpoints.
    echo.
    pause
    exit /b 0
)

echo Invalid choice!
pause



