using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Services;

public class ValSectionServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ValSectionService _valSectionService;

    public ValSectionServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestApplicationDbContext(options);
        _valSectionService = new ValSectionService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllValSectionsAsync_WhenNoValSections_ReturnsEmptyList()
    {
        // Act
        var result = await _valSectionService.GetAllValSectionsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllValSectionsAsync_WhenValSectionsExist_ReturnsAllValSections()
    {
        // Arrange
        var valSections = new List<Valsection>
        {
            new Valsection
            {
                GroupId = 1,
                SectionText = "Section 1",
                DisplayOrder = 1,
                DefaultColWidth1 = 100,
                DefaultColType1 = "Text"
            },
            new Valsection
            {
                GroupId = 2,
                SectionText = "Section 2",
                DisplayOrder = 2,
                DefaultColWidth1 = 150,
                DefaultColType1 = "Number"
            },
            new Valsection
            {
                GroupId = 3,
                SectionText = "Section 3",
                DisplayOrder = 3,
                DefaultColWidth1 = 200,
                DefaultColType1 = "Date"
            }
        };

        await _context.Valsections.AddRangeAsync(valSections);
        await _context.SaveChangesAsync();

        // Act
        var result = await _valSectionService.GetAllValSectionsAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(valSections);
    }

    [Fact]
    public async Task GetAllValSectionsAsync_ReturnsValSectionsInDisplayOrder()
    {
        // Arrange
        var valSections = new List<Valsection>
        {
            new Valsection { GroupId = 3, SectionText = "Section C", DisplayOrder = 3 },
            new Valsection { GroupId = 1, SectionText = "Section A", DisplayOrder = 1 },
            new Valsection { GroupId = 2, SectionText = "Section B", DisplayOrder = 2 }
        };

        await _context.Valsections.AddRangeAsync(valSections);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _valSectionService.GetAllValSectionsAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].DisplayOrder.Should().Be(1);
        result[1].DisplayOrder.Should().Be(2);
        result[2].DisplayOrder.Should().Be(3);
    }

    [Fact]
    public async Task GetValSectionByGroupIdAsync_WhenValSectionExists_ReturnsValSection()
    {
        // Arrange
        var valSection = new Valsection
        {
            GroupId = 1,
            SectionText = "Test Section",
            DisplayOrder = 1,
            DefaultColWidth1 = 100,
            DefaultColType1 = "Text",
            AutoIndent = true
        };

        await _context.Valsections.AddAsync(valSection);
        await _context.SaveChangesAsync();

        // Act
        var result = await _valSectionService.GetValSectionByGroupIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(valSection);
    }

    [Fact]
    public async Task GetValSectionByGroupIdAsync_WhenValSectionDoesNotExist_ReturnsNull()
    {
        // Arrange
        var valSection = new Valsection
        {
            GroupId = 1,
            SectionText = "Test Section"
        };

        await _context.Valsections.AddAsync(valSection);
        await _context.SaveChangesAsync();

        // Act
        var result = await _valSectionService.GetValSectionByGroupIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetValSectionByGroupIdAsync_WhenDatabaseIsEmpty_ReturnsNull()
    {
        // Act
        var result = await _valSectionService.GetValSectionByGroupIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetValSectionByGroupIdAsync_WithMultipleValSections_ReturnsCorrectValSection()
    {
        // Arrange
        var valSections = new List<Valsection>
        {
            new Valsection { GroupId = 1, SectionText = "Section 1" },
            new Valsection { GroupId = 2, SectionText = "Section 2" },
            new Valsection { GroupId = 3, SectionText = "Section 3" }
        };

        await _context.Valsections.AddRangeAsync(valSections);
        await _context.SaveChangesAsync();

        // Act
        var result = await _valSectionService.GetValSectionByGroupIdAsync(2);

        // Assert
        result.Should().NotBeNull();
        result!.GroupId.Should().Be(2);
        result.SectionText.Should().Be("Section 2");
    }

    [Fact]
    public async Task GetValSectionsByGroupIdAsync_WhenMultipleValSectionsExist_ReturnsAllForGroupId()
    {
        // Arrange
        var valSections = new List<Valsection>
        {
            new Valsection { GroupId = 1, SectionText = "Section 1A", DisplayOrder = 1 },
            new Valsection { GroupId = 1, SectionText = "Section 1B", DisplayOrder = 2 },
            new Valsection { GroupId = 2, SectionText = "Section 2A", DisplayOrder = 1 },
            new Valsection { GroupId = 1, SectionText = "Section 1C", DisplayOrder = 3 }
        };

        await _context.Valsections.AddRangeAsync(valSections);
        await _context.SaveChangesAsync();

        // Act
        var result = await _valSectionService.GetValSectionsByGroupIdAsync(1);

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(v => v.GroupId.Should().Be(1));
    }

    [Fact]
    public async Task GetValSectionsByGroupIdAsync_ReturnsValSectionsInDisplayOrder()
    {
        // Arrange
        var valSections = new List<Valsection>
        {
            new Valsection { GroupId = 1, SectionText = "Section C", DisplayOrder = 3 },
            new Valsection { GroupId = 1, SectionText = "Section A", DisplayOrder = 1 },
            new Valsection { GroupId = 1, SectionText = "Section B", DisplayOrder = 2 }
        };

        await _context.Valsections.AddRangeAsync(valSections);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _valSectionService.GetValSectionsByGroupIdAsync(1)).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].DisplayOrder.Should().Be(1);
        result[1].DisplayOrder.Should().Be(2);
        result[2].DisplayOrder.Should().Be(3);
    }

    [Fact]
    public async Task GetValSectionsByGroupIdAsync_WhenNoValSectionsForGroupId_ReturnsEmptyList()
    {
        // Arrange
        var valSections = new List<Valsection>
        {
            new Valsection { GroupId = 1, SectionText = "Section 1" },
            new Valsection { GroupId = 2, SectionText = "Section 2" }
        };

        await _context.Valsections.AddRangeAsync(valSections);
        await _context.SaveChangesAsync();

        // Act
        var result = await _valSectionService.GetValSectionsByGroupIdAsync(999);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllValSectionsAsync_DoesNotTrackEntities()
    {
        // Arrange
        var valSection = new Valsection
        {
            GroupId = 1,
            SectionText = "Test Section"
        };

        await _context.Valsections.AddAsync(valSection);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _valSectionService.GetAllValSectionsAsync();

        // Assert
        _context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task GetValSectionByGroupIdAsync_DoesNotTrackEntities()
    {
        // Arrange
        var valSection = new Valsection
        {
            GroupId = 1,
            SectionText = "Test Section"
        };

        await _context.Valsections.AddAsync(valSection);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _valSectionService.GetValSectionByGroupIdAsync(1);

        // Assert
        _context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task GetValSectionsByGroupIdAsync_DoesNotTrackEntities()
    {
        // Arrange
        var valSection = new Valsection
        {
            GroupId = 1,
            SectionText = "Test Section"
        };

        await _context.Valsections.AddAsync(valSection);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _valSectionService.GetValSectionsByGroupIdAsync(1);

        // Assert
        _context.ChangeTracker.Entries().Should().BeEmpty();
    }

    // Test-specific DbContext that configures Valsection with a key for in-memory testing
    private class TestApplicationDbContext : ApplicationDbContext
    {
        public TestApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Override the Keyless attribute for testing purposes
            // Use a composite key of GroupId and SectionText to make entities trackable
            modelBuilder.Entity<Valsection>()
                .HasKey(v => new { v.GroupId, v.SectionText });
        }
    }
}
