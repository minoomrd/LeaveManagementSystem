@echo off
cls
color 0E
title PostgreSQL Database Viewer
echo.
echo ========================================
echo   PostgreSQL Database Access
echo ========================================
echo.

REM Check if PostgreSQL is installed
set POSTGRES_PATH=C:\Program Files\PostgreSQL
if not exist "%POSTGRES_PATH%" (
    echo [ERROR] PostgreSQL not found!
    echo.
    echo PostgreSQL needs to be installed first.
    echo Download from: https://www.postgresql.org/download/windows/
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

set PGADMIN_PATH=%POSTGRES_PATH%\%PG_VERSION%\pgAdmin 4
set PGSQL_PATH=%POSTGRES_PATH%\%PG_VERSION%\bin

echo Found PostgreSQL: %PG_VERSION%
echo.

REM Check for pgAdmin
if exist "%PGADMIN_PATH%\bin\pgAdmin4.exe" (
    echo Opening pgAdmin (PostgreSQL Web Interface)...
    echo.
    echo pgAdmin will open in your browser.
    echo Default URL: http://127.0.0.1:5050
    echo.
    start "" "%PGADMIN_PATH%\bin\pgAdmin4.exe"
    timeout /t 5 /nobreak >nul
    start http://127.0.0.1:5050
) else (
    echo pgAdmin not found at: %PGADMIN_PATH%
    echo.
    echo Alternative: Use psql command line
    echo.
    echo To view database, run:
    echo   "%PGSQL_PATH%\psql.exe" -U postgres -d LeaveManagementSystem
    echo.
    echo Or install pgAdmin from PostgreSQL installer.
    echo.
)

echo.
echo ========================================
echo   Database Connection Info
echo ========================================
echo.
echo Host: localhost
echo Port: 5432
echo Database: LeaveManagementSystem
echo Username: postgres
echo Password: (the one you set during installation)
echo.
pause



