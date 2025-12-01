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
        var found = await _service.GetValHeaderByIdAsync(header.ValId);
        found.Should().NotBeNull();
        found!.ValDescription.Should().Be("Test Header");
    }

    [Fact]
    public async Task UpdateValHeaderAsync_UpdatesHeader()
    {
        var header = new Valheader { ValDescription = "Original" };
        await _service.CreateValHeaderAsync(header);
        header.ValDescription = "Updated";
        var updated = await _service.UpdateValHeaderAsync(header.ValId, header);
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
}
