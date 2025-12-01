#!/bin/bash

# Clean previous coverage
echo "Cleaning previous coverage reports..."
rm -rf TestResults
rm -rf coveragereport

# Run tests with coverage
echo ""
echo "Running tests with coverage collection..."
dotnet test --collect:"XPlat Code Coverage" --results-directory:"TestResults" --settings:coverlet.runsettings

if [ $? -ne 0 ]; then
    echo ""
    echo "Tests failed! Coverage report generation aborted."
    exit 1
fi

# Check if reportgenerator is installed
echo ""
echo "Checking for reportgenerator tool..."
if ! command -v reportgenerator &> /dev/null; then
    echo "Installing dotnet-reportgenerator-globaltool..."
    dotnet tool install -g dotnet-reportgenerator-globaltool
fi

# Generate HTML report with filters
echo ""
echo "Generating coverage report..."
reportgenerator \
    -reports:"TestResults/**/coverage.cobertura.xml" \
    -targetdir:"coveragereport" \
    -reporttypes:"Html;HtmlSummary;Badges" \
    -classfilters:"-Microsoft.AspNetCore.OpenApi.Generated*;-System.Runtime.CompilerServices*" \
    -filefilters:"-**/Program.cs;-**/*.Generated.cs;-**/Migrations/**"

if [ $? -eq 0 ]; then
    echo ""
    echo "? Coverage report generated successfully!"
    echo "Opening report in browser..."
    
    # Open report based on OS
    if [[ "$OSTYPE" == "darwin"* ]]; then
        open coveragereport/index.html
    elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
        xdg-open coveragereport/index.html
    else
        echo "Please open coveragereport/index.html manually"
    fi
    
    echo ""
    echo "Coverage Report Location: coveragereport/index.html"
else
    echo ""
    echo "? Failed to generate coverage report!"
    exit 1
fi
