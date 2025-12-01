@echo off
echo Cleaning previous coverage reports...
rmdir /s /q TestResults 2>nul
rmdir /s /q coveragereport 2>nul

echo.
echo Running tests with coverage collection...
dotnet test --collect:"XPlat Code Coverage" --results-directory:"TestResults" --settings:coverlet.runsettings

if %ERRORLEVEL% neq 0 (
    echo.
    echo Tests failed! Coverage report generation aborted.
    exit /b %ERRORLEVEL%
)

echo.
echo Generating coverage report...
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:"Html;HtmlSummary;Badges" -classfilters:"-Microsoft.AspNetCore.OpenApi.Generated*;-System.Runtime.CompilerServices*" -filefilters:"-**/Program.cs;-**/*.Generated.cs;-**/Migrations/**"

if %ERRORLEVEL% equ 0 (
    echo.
    echo Coverage report generated successfully!
    echo Opening report in browser...
    start coveragereport\index.html
) else (
    echo.
    echo Failed to generate coverage report!
    echo You may need to install reportgenerator:
    echo   dotnet tool install -g dotnet-reportgenerator-globaltool
    exit /b %ERRORLEVEL%
)
