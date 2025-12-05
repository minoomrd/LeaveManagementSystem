@echo off
cls
color 0E
title PostgreSQL Database Setup
echo.
echo ========================================
echo   PostgreSQL Database Setup
echo ========================================
echo.

REM Check if PostgreSQL is installed
set POSTGRES_PATH=C:\Program Files\PostgreSQL
if not exist "%POSTGRES_PATH%" (
    echo [ERROR] PostgreSQL not found!
    echo.
    echo Please install PostgreSQL first:
    echo   1. Download from: https://www.postgresql.org/download/windows/
    echo   2. Or run: winget install PostgreSQL.PostgreSQL
    echo.
    echo After installation, run this script again.
    echo.
    pause
    exit /b 1
)

REM Find latest PostgreSQL version
for /f "delims=" %%i in ('dir /b /ad /o-n "%POSTGRES_PATH%" 2^>nul') do (
    set PG_VERSION=%%i
    goto :found
)
:found

set PGSQL_PATH=%POSTGRES_PATH%\%PG_VERSION%\bin
set PGSQL_EXE=%PGSQL_PATH%\psql.exe

if not exist "%PGSQL_EXE%" (
    echo [ERROR] psql.exe not found at: %PGSQL_EXE%
    echo.
    pause
    exit /b 1
)

echo Found PostgreSQL: %PG_VERSION%
echo.

REM Check if database exists
echo Checking if database exists...
"%PGSQL_EXE%" -U postgres -c "SELECT 1 FROM pg_database WHERE datname='LeaveManagementSystem'" 2>nul | findstr /C:"1" >nul
if errorlevel 1 (
    echo Database 'LeaveManagementSystem' does not exist.
    echo.
    echo Creating database...
    echo.
    echo Please enter your PostgreSQL password when prompted:
    echo.
    "%PGSQL_EXE%" -U postgres -c "CREATE DATABASE LeaveManagementSystem;"
    if errorlevel 1 (
        echo.
        echo [ERROR] Failed to create database!
        echo.
        echo Possible issues:
        echo   1. Wrong password
        echo   2. PostgreSQL service not running
        echo   3. Connection refused
        echo.
        echo To start PostgreSQL service:
        echo   net start postgresql-x64-%PG_VERSION%
        echo.
        pause
        exit /b 1
    )
    echo.
    echo ✅ Database created successfully!
) else (
    echo ✅ Database 'LeaveManagementSystem' already exists!
)

echo.
echo ========================================
echo   Next Steps
echo ========================================
echo.
echo 1. Make sure your connection string in appsettings.json is correct:
echo    Password=YOUR_POSTGRESQL_PASSWORD
echo.
echo 2. Apply migrations:
echo    cd "C:\My tasks\Leave Management System\LeaveManagementSystem.Infrastructure"
echo    dotnet ef database update --startup-project ../LeaveManagementSystem.API
echo.
echo 3. Start the backend API
echo.
pause



