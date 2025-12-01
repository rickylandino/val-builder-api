# Clean previous coverage
Remove-Item -Path "TestResults" -Recurse -ErrorAction SilentlyContinue
Remove-Item -Path "coveragereport" -Recurse -ErrorAction SilentlyContinue

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory:"TestResults" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura

# Generate HTML report
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:"Html;HtmlSummary"

# Open report
Start-Process "coveragereport/index.html"