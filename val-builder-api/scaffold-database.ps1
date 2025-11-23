# Entity Framework Core Database Scaffolding Script
# This script will scaffold your existing SQL Server database into Entity Framework models

# ===========================================
# CONFIGURATION - UPDATE THESE VALUES
# ===========================================

# Your SQL Server connection string
# Example for local SQL Server with Windows Authentication:
$ConnectionString = "Server=RICKY-P16V;Database=VALBuilder;Trusted_Connection=True;TrustServerCertificate=True;"

# Or for SQL Server with username/password:
# $ConnectionString = "Server=localhost;Database=YourDatabaseName;User Id=your_username;Password=your_password;TrustServerCertificate=True;"

# Or for Azure SQL:
# $ConnectionString = "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=YourDatabaseName;User Id=your_username;Password=your_password;TrustServerCertificate=True;"

# The namespace and folder for your DbContext and entity models
$ContextName = "ApplicationDbContext"
$OutputDir = "Data"
$ModelsDir = "Models"

# ===========================================
# SCAFFOLDING COMMAND
# ===========================================

Write-Host "Scaffolding database..." -ForegroundColor Green
Write-Host "Context: $ContextName" -ForegroundColor Cyan
Write-Host "Output Directory: $OutputDir" -ForegroundColor Cyan
Write-Host "Models Directory: $ModelsDir" -ForegroundColor Cyan
Write-Host ""

# Execute the scaffold command
dotnet ef dbcontext scaffold $ConnectionString Microsoft.EntityFrameworkCore.SqlServer `
    --output-dir $ModelsDir `
    --context-dir $OutputDir `
    --context $ContextName `
    --force `
    --data-annotations `
    --verbose

# Optional flags you can add:
# --tables TableName1,TableName2    # Scaffold only specific tables
# --schemas SchemaName              # Scaffold only specific schema
# --no-onconfiguring                # Don't generate OnConfiguring method
# --no-pluralize                    # Don't pluralize DbSet names

Write-Host ""
Write-Host "Scaffolding complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Review the generated models in the '$ModelsDir' folder" -ForegroundColor White
Write-Host "2. Review the DbContext in the '$OutputDir' folder" -ForegroundColor White
Write-Host "3. Update Program.cs to register the DbContext" -ForegroundColor White
Write-Host "4. Move connection string to appsettings.json" -ForegroundColor White
