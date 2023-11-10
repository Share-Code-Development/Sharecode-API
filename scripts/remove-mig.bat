@echo off
setlocal enabledelayedexpansion

cd ..

:: Check for optional parameter -d or -delete
set "deleteMigrations=false"
if "%1"=="-d" set "deleteMigrations=true"
if "%1"=="-delete" set "deleteMigrations=true"

:: Change directory to 'Sharecode.Backend.Infrastructure'
cd Sharecode.Backend.Infrastructure

:: Check if the directory exists
if not exist . (
    :: Throw an error and exit if the directory doesn't exist
    echo Error: 'Sharecode.Backend.Infrastructure' directory not found.
    exit /b 1
)

:: Check if dotnet 8 is installed
dotnet --version | find "8" >nul
if %errorlevel% neq 0 (
    :: Throw an error and exit if dotnet 8 is not installed
    echo Error: dotnet version 8 is required.
    exit /b 1
)

:: Run the 'dotnet ef remove' command
if %deleteMigrations%==true (
    echo Deleting 'Migrations' folder...
    rd /s /q Migrations
) else (
    dotnet ef remove
)
