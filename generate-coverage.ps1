# Clean previous coverage
Write-Host "Cleaning previous coverage reports..." -ForegroundColor Cyan
Remove-Item -Path "TestResults" -Recurse -ErrorAction SilentlyContinue
Remove-Item -Path "coveragereport" -Recurse -ErrorAction SilentlyContinue

# Run tests with coverage
Write-Host "`nRunning tests with coverage collection..." -ForegroundColor Cyan
dotnet test --collect:"XPlat Code Coverage" --results-directory:"TestResults" --settings:coverlet.runsettings

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nTests failed! Coverage report generation aborted." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Check if reportgenerator is installed
Write-Host "`nChecking for reportgenerator tool..." -ForegroundColor Cyan
$reportGeneratorInstalled = dotnet tool list -g | Select-String "dotnet-reportgenerator-globaltool"

if (-not $reportGeneratorInstalled) {
    Write-Host "Installing dotnet-reportgenerator-globaltool..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

# Generate HTML report with additional filters
Write-Host "`nGenerating coverage report..." -ForegroundColor Cyan
reportgenerator `
    -reports:"TestResults/**/coverage.cobertura.xml" `
    -targetdir:"coveragereport" `
    -reporttypes:"Html;HtmlSummary;Badges" `
    -classfilters:"-Microsoft.AspNetCore.OpenApi.Generated*;-System.Runtime.CompilerServices*" `
    -filefilters:"-**/Program.cs;-**/*.Generated.cs;-**/Migrations/**"

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n? Coverage report generated successfully!" -ForegroundColor Green
    Write-Host "Opening report in browser..." -ForegroundColor Cyan
    Start-Process "coveragereport/index.html"
    
    # Display summary
    Write-Host "`nCoverage Report Location: coveragereport/index.html" -ForegroundColor Green
} else {
    Write-Host "`n? Failed to generate coverage report!" -ForegroundColor Red
    exit $LASTEXITCODE
}
