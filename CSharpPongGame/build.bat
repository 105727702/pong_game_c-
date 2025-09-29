@echo off
echo Building C# Pong Game...
echo.

REM Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET is not installed or not in PATH
    echo Please install .NET 8.0 or later from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo Restoring packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo Building project...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo Build successful!
echo.
echo To run the game:
echo   dotnet run
echo.
echo Or run the executable in:
echo   bin\Release\net8.0-windows\PongGame.exe
echo.
pause
