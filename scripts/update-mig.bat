@echo off
cd ..

:: Check if dotnet is installed and is version 8
dotnet --version | find "8" >nul
if %errorlevel% equ 0 (
    :: Run the migrations script
    dotnet ef database update --startup-project Sharecode.Backend.Api --project Sharecode.Backend.Infrastructure
) else (
    :: Throw an error and exit
    echo Error: dotnet version 8 is required.
    exit /b 1
)
