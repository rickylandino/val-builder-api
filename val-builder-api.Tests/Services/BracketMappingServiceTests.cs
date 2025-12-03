using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Services;

public class BracketMappingServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BracketMappingService _service;

    public BracketMappingServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _service = new BracketMappingService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_AddsMapping()
    {
        var mapping = new BracketMapping { TagName = "Tag1", ObjectPath = "Path1", Description = "Desc" };
        var created = await _service.CreateAsync(mapping);
        created.Id.Should().BeGreaterThan(0);
        created.TagName.Should().Be("Tag1");
        (await _service.GetAllAsync()).Should().ContainSingle();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllMappings()
    {
        await _service.CreateAsync(new BracketMapping { TagName = "Tag1", ObjectPath = "Path1" });
        await _service.CreateAsync(new BracketMapping { TagName = "Tag2", ObjectPath = "Path2" });
        var all = await _service.GetAllAsync();
        all.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesMapping()
    {
        var mapping = new BracketMapping { TagName = "Tag1", ObjectPath = "Path1" };
        var created = await _service.CreateAsync(mapping);
        created.TagName = "Tag2";
        created.ObjectPath = "Path2";
        var updated = await _service.UpdateAsync(created.Id.Value, created);
        updated.Should().NotBeNull();
        updated!.TagName.Should().Be("Tag2");
        updated.ObjectPath.Should().Be("Path2");
    }

    [Fact]
    public async Task DeleteAsync_RemovesMapping()
    {
        var mapping = new BracketMapping { TagName = "Tag1", ObjectPath = "Path1" };
        var created = await _service.CreateAsync(mapping);
        var deleted = await _service.DeleteAsync(created.Id.Value);
        deleted.Should().BeTrue();
        (await _service.GetAllAsync()).Should().BeEmpty();
    }
}
