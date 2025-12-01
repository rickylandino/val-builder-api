# Code Coverage - Quick Reference

## ?? Run Coverage
```powershell
.\generate-coverage.ps1
```

## ?? What Gets Excluded Automatically

### ? Infrastructure Code
- `Program.cs` - Startup/configuration code
- `*.Generated.cs` - Auto-generated files
- `Migrations/**` - Database migrations

### ? Auto-Generated Namespaces
- `Microsoft.AspNetCore.OpenApi.Generated`
- `System.Runtime.CompilerServices`

### ? Test Projects
- `*.Tests` assemblies

## ? How to Exclude Additional Code

### Method 1: Add Attribute (Recommended for specific classes/methods)
```csharp
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class MyInfrastructureClass
{
    // This entire class will be excluded
}

public class MyService
{
    [ExcludeFromCodeCoverage]
    public void LegacyMethod()
    {
        // Only this method will be excluded
    }
}
```

### Method 2: Update coverlet.runsettings (Recommended for patterns)

Edit `coverlet.runsettings`:

```xml
<Exclude>
  [*.Tests]*,
  [*]*.Program,
  [*]*Migrations*,
  [*]Microsoft.AspNetCore.OpenApi.Generated*,
  [*]YourNamespace.ClassToExclude
</Exclude>

<ExcludeByFile>
  **/Program.cs,
  **/Migrations/**/*,
  **/*.Generated.cs,
  **/YourFileToExclude.cs
</ExcludeByFile>
```

### Method 3: ReportGenerator Filters (Recommended for post-processing)

Edit `generate-coverage.ps1`:

```powershell
reportgenerator `
    -classfilters:"-Microsoft.AspNetCore.OpenApi.Generated*;-YourNamespace.ClassToExclude*" `
    -filefilters:"-**/Program.cs;-**/YourFileToExclude.cs"
```

## ?? Coverage Goals

| Level | Percentage | Status |
|-------|-----------|--------|
| Excellent | 90-100% | ?? |
| Good | 80-89% | ?? |
| Fair | 70-79% | ?? |
| Poor | <70% | ?? |

### Project Targets
- **Minimum**: 80% line coverage
- **Target**: 90% line coverage
- **Critical Paths**: 100% coverage
- **New Code**: Must maintain or improve coverage

## ?? Common Patterns to Exclude

### DTOs (Data Transfer Objects)
```csharp
[ExcludeFromCodeCoverage]
public class PersonDto
{
    public string Name { get; set; }
    public int Age { get; set; }
}
```

### Configuration Classes
```csharp
[ExcludeFromCodeCoverage]
public class AppSettings
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; }
}
```

### Extension Methods (Infrastructure)
```csharp
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyServices(this IServiceCollection services)
    {
        // DI registration code
        return services;
    }
}
```

### Migration Files
```csharp
[ExcludeFromCodeCoverage]
public partial class InitialCreate : Migration
{
    // Migration code
}
```

## ? Quick Commands

```powershell
# Generate coverage report
.\generate-coverage.ps1

# Run tests without coverage
dotnet test

# Run specific test with coverage
dotnet test --filter "FullyQualifiedName~MyTestClass" --collect:"XPlat Code Coverage"

# View coverage in console (threshold check)
dotnet test /p:CollectCoverage=true /p:Threshold=80 /p:ThresholdType=line
```

## ?? Coverage Files

| File/Folder | Purpose | Git Ignore? |
|------------|---------|-------------|
| `coveragereport/` | HTML report | ? Yes |
| `TestResults/` | Raw coverage data | ? Yes |
| `coverlet.runsettings` | Coverage config | ? No |
| `generate-coverage.*` | Scripts | ? No |

## ?? Troubleshooting

### Program.cs still showing in coverage
1. Verify `coverlet.runsettings` is being used
2. Check that `[ExcludeFromCodeCoverage]` attribute is on Program class
3. Ensure script uses `--settings:coverlet.runsettings`

### Generated OpenAPI code showing
1. Verify class filters in reportgenerator command
2. Check that generated files have `.Generated.cs` extension
3. Update `ExcludeByFile` pattern if needed

### Migrations showing in coverage
1. Ensure migrations are in `Migrations/**` folder
2. Verify file filter pattern in `coverlet.runsettings`
3. Add `[ExcludeFromCodeCoverage]` to migration classes

## ?? References

- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator Filters](https://github.com/danielpalme/ReportGenerator/wiki/Settings)
- [Coverage Best Practices](https://martinfowler.com/bliki/TestCoverage.html)

---

**Last Updated**: 2024-12-01  
**Configuration Files**: `coverlet.runsettings`, `generate-coverage.ps1`
