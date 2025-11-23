# Entity Framework Core Scaffolding Guide

## Quick Start

### 1. Update the Connection String
Edit `scaffold-database.ps1` and update the `$ConnectionString` variable with your SQL Server database connection details.

**Examples:**
```powershell
# Local SQL Server (Windows Authentication)
$ConnectionString = "Server=localhost;Database=YourDatabaseName;Trusted_Connection=True;TrustServerCertificate=True;"

# SQL Server (SQL Authentication)
$ConnectionString = "Server=localhost;Database=YourDatabaseName;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"

# Azure SQL Database
$ConnectionString = "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=YourDatabaseName;User Id=yourusername;Password=yourpassword;TrustServerCertificate=True;"
```

### 2. Run the Scaffold Script
```powershell
.\scaffold-database.ps1
```

This will generate:
- **Models/** - Entity classes representing your database tables
- **Data/ApplicationDbContext.cs** - The DbContext class

---

## Manual Scaffolding (Alternative)

If you prefer to run the command directly:

```bash
dotnet ef dbcontext scaffold "YOUR_CONNECTION_STRING" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context ApplicationDbContext --force --data-annotations
```

### Useful Options:

| Option | Description |
|--------|-------------|
| `--tables Table1,Table2` | Scaffold only specific tables |
| `--schemas dbo,custom` | Scaffold only specific schemas |
| `--no-onconfiguring` | Don't include connection string in DbContext (recommended) |
| `--no-pluralize` | Don't pluralize DbSet property names |
| `--force` | Overwrite existing files |
| `--data-annotations` | Use data annotations instead of Fluent API |

---

## Post-Scaffolding Setup

### 3. Create appsettings.json (if not exists)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourDatabaseName;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### 4. Update Program.cs
Add the DbContext to your services:

```csharp
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data; // Adjust namespace as needed

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 5. Update DbContext (Remove hardcoded connection string)
If your DbContext has the connection string hardcoded in `OnConfiguring`, remove it since we're configuring it in Program.cs:

```csharp
// REMOVE or comment out the OnConfiguring method if it exists
// protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// {
//     optionsBuilder.UseSqlServer("...");
// }
```

---

## Creating Your First API Controller

Example controller using the DbContext:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;

[ApiController]
[Route("api/[controller]")]
public class YourEntityController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public YourEntityController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<YourEntity>>> GetAll()
    {
        return await _context.YourEntities.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<YourEntity>> GetById(int id)
    {
        var entity = await _context.YourEntities.FindAsync(id);
        
        if (entity == null)
        {
            return NotFound();
        }
        
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<YourEntity>> Create(YourEntity entity)
    {
        _context.YourEntities.Add(entity);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, YourEntity entity)
    {
        if (id != entity.Id)
        {
            return BadRequest();
        }

        _context.Entry(entity).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.YourEntities.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.YourEntities.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        _context.YourEntities.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
```

---

## Tips & Best Practices

1. **Connection String Security**: Never commit connection strings with passwords to source control. Use User Secrets for development:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
   ```

2. **Partial Classes**: Consider making your entities partial classes so you can extend them without modifying generated code.

3. **Re-scaffolding**: When your database schema changes, run the scaffold script again with `--force` flag.

4. **Migrations**: After scaffolding, you can switch to code-first migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Repository Pattern**: Consider implementing the Repository pattern for better separation of concerns.

---

## Installed Packages

The following NuGet packages have been installed:
- ? Microsoft.EntityFrameworkCore.SqlServer (10.0.0)
- ? Microsoft.EntityFrameworkCore.Design (10.0.0)
- ? dotnet-ef CLI tool (10.0.0)

---

## Troubleshooting

**Error: "Build failed"**
- Make sure your project compiles before scaffolding

**Error: "Cannot connect to database"**
- Verify your connection string
- Check if SQL Server is running
- Verify firewall settings

**Error: "No DbContext was found"**
- Make sure Microsoft.EntityFrameworkCore.Design is installed

**DbContext has hardcoded connection string**
- Remove the `OnConfiguring` method from the generated DbContext
- Configure the connection string in Program.cs instead

---

## Next Steps After Scaffolding

1. ? Install required packages (DONE)
2. ?? Update connection string in scaffold-database.ps1
3. ?? Run scaffold-database.ps1
4. ?? Create/update appsettings.json with connection string
5. ?? Register DbContext in Program.cs
6. ?? Create API controllers for your entities
7. ?? Test your API endpoints

Happy coding! ??
