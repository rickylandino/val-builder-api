using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Services;

public class ValHeaderServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ValHeaderService _service;

    public ValHeaderServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _service = new ValHeaderService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllValHeadersAsync_WhenNone_ReturnsEmpty()
    {
        var result = await _service.GetAllValHeadersAsync();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateValHeaderAsync_AddsValHeader()
    {
        var header = new Valheader { ValDescription = "Test Header" };
        var created = await _service.CreateValHeaderAsync(header);
        created.ValDescription.Should().Be("Test Header");
        (await _service.GetAllValHeadersAsync()).Should().ContainSingle();
    }

    [Fact]
    public async Task GetValHeaderByIdAsync_ReturnsCorrectHeader()
    {
        var header = new Valheader { ValDescription = "Test Header" };
        await _service.CreateValHeaderAsync(header);
        var found = await _service.GetValHeaderByIdAsync(header.ValId.Value);
        found.Should().NotBeNull();
        found!.ValDescription.Should().Be("Test Header");
    }

    [Fact]
    public async Task UpdateValHeaderAsync_UpdatesHeader()
    {
        var header = new Valheader { ValDescription = "Original" };
        await _service.CreateValHeaderAsync(header);
        header.ValDescription = "Updated";
        var updated = await _service.UpdateValHeaderAsync(header.ValId.Value, header);
        updated.Should().NotBeNull();
        updated!.ValDescription.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateValHeaderAsync_WhenNotFound_ReturnsNull()
    {
        var header = new Valheader { ValDescription = "Doesn't exist" };
        var updated = await _service.UpdateValHeaderAsync(999, header);
        updated.Should().BeNull();
    }

    [Fact]
    public async Task GetValHeadersByCompanyIdAsync_ReturnsHeadersWithMatchingPlanId()
    {
        // Arrange
        var headers = new[]
        {
            new Valheader { ValDescription = "Header 1", PlanId = 10 },
            new Valheader { ValDescription = "Header 2", PlanId = 10 },
            new Valheader { ValDescription = "Header 3", PlanId = 20 }
        };
        await _context.Valheaders.AddRangeAsync(headers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetValHeadersByCompanyIdAsync(10);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(h => h.PlanId.Should().Be(10));
        result.Select(h => h.ValDescription).Should().Contain(new[] { "Header 1", "Header 2" });
    }

    [Fact]
    public async Task GetValHeadersByCompanyIdAsync_ReturnsEmpty_WhenNoMatch()
    {
        // Arrange
        var header = new Valheader { ValDescription = "Header", PlanId = 99 };
        await _context.Valheaders.AddAsync(header);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetValHeadersByCompanyIdAsync(123);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetValHeadersByPlanIdAsync_ReturnsHeadersWithMatchingPlanId()
    {
        // Arrange
        var headers = new[]
        {
            new Valheader { ValDescription = "Header A", PlanId = 5 },
            new Valheader { ValDescription = "Header B", PlanId = 5 },
            new Valheader { ValDescription = "Header C", PlanId = 6 }
        };
        await _context.Valheaders.AddRangeAsync(headers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetValHeadersByPlanIdAsync(5);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(h => h.PlanId.Should().Be(5));
        result.Select(h => h.ValDescription).Should().Contain(new[] { "Header A", "Header B" });
    }

    [Fact]
    public async Task GetValHeadersByPlanIdAsync_ReturnsEmpty_WhenNoMatch()
    {
        // Arrange
        var header = new Valheader { ValDescription = "Header", PlanId = 77 };
        await _context.Valheaders.AddAsync(header);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetValHeadersByPlanIdAsync(88);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateValHeaderAsync_CreatesValDetailsFromDefaultTemplateItems_WithSequentialDisplayOrderPerGroup()
    {
        // Arrange: Add template items with mixed DefaultOnVal and groups
        var templateItems = new[]
        {
            new ValtemplateItem { GroupId = 1, ItemText = "A", DisplayOrder = 1, DefaultOnVal = true, Bold = true },
            new ValtemplateItem { GroupId = 1, ItemText = "B", DisplayOrder = 2, DefaultOnVal = false },
            new ValtemplateItem { GroupId = 1, ItemText = "C", DisplayOrder = 3, DefaultOnVal = true, Bullet = true },
            new ValtemplateItem { GroupId = 2, ItemText = "D", DisplayOrder = 1, DefaultOnVal = true },
            new ValtemplateItem { GroupId = 2, ItemText = "E", DisplayOrder = 2, DefaultOnVal = false },
            new ValtemplateItem { GroupId = 2, ItemText = "F", DisplayOrder = 3, DefaultOnVal = true }
        };
        await _context.ValtemplateItems.AddRangeAsync(templateItems);
        await _context.SaveChangesAsync();

        var header = new Valheader { ValDescription = "Header with details" };

        // Act
        var created = await _service.CreateValHeaderAsync(header);

        // Assert: Only DefaultOnVal items are copied, and DisplayOrder is sequential per group
        var details = await _context.Valdetails.Where(d => d.ValId == created.ValId).OrderBy(d => d.GroupId).ThenBy(d => d.DisplayOrder).ToListAsync();
        details.Should().HaveCount(4);

        // Group 1: Items A and C, should have DisplayOrder 1 and 2
        var group1 = details.Where(d => d.GroupId == 1).OrderBy(d => d.DisplayOrder).ToList();
        group1.Should().HaveCount(2);
        group1[0].GroupContent.Should().Be("A");
        group1[0].DisplayOrder.Should().Be(1);
        group1[0].Bold.Should().BeTrue();
        group1[1].GroupContent.Should().Be("C");
        group1[1].DisplayOrder.Should().Be(2);
        group1[1].Bullet.Should().BeTrue();

        // Group 2: Items D and F, should have DisplayOrder 1 and 2
        var group2 = details.Where(d => d.GroupId == 2).OrderBy(d => d.DisplayOrder).ToList();
        group2.Should().HaveCount(2);
        group2[0].GroupContent.Should().Be("D");
        group2[0].DisplayOrder.Should().Be(1);
        group2[1].GroupContent.Should().Be("F");
        group2[1].DisplayOrder.Should().Be(2);
    }
}
