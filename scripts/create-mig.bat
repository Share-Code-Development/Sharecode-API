@echo off
setlocal enabledelayedexpansion

cd ..

:: Check for optional parameter and set it to initial-migration if not provided
set "migrationName=%1"
if not defined migrationName set "migrationName=initial-migration"

:: Check if dotnet is installed and is version 8
dotnet --version | find "8" >nul
if %errorlevel% equ 0 (
    :: Run the migrations script
    dotnet ef migrations add !migrationName! --startup-project Sharecode.Backend.Api --project Sharecode.Backend.Infrastructure
) else (
    :: Throw an error and exit if dotnet 8 is not installed
    echo Error: dotnet version 8 is required.
    exit /b 1
)
