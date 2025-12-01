using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Services;

public class ValTemplateItemServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ValTemplateItemService _service;

    public ValTemplateItemServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _service = new ValTemplateItemService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetValTemplateItemByIdAsync_ReturnsCorrect()
    {
        var item = new ValtemplateItem { GroupId = 1, ItemText = "A" };
        await _context.ValtemplateItems.AddAsync(item);
        await _context.SaveChangesAsync();
        var found = await _service.GetValTemplateItemByIdAsync(item.ItemId);
        found.Should().NotBeNull();
        found!.ItemText.Should().Be("A");
    }

    [Fact]
    public async Task CreateValTemplateItemAsync_AddsItem()
    {
        var item = new ValtemplateItem { GroupId = 1, ItemText = "A" };
        var created = await _service.CreateValTemplateItemAsync(item);
        created.ItemText.Should().Be("A");
        (await _service.GetValTemplateItemsByGroupIdAsync(1)).Should().ContainSingle();
    }

    [Fact]
    public async Task UpdateValTemplateItemAsync_UpdatesItem()
    {
        var item = new ValtemplateItem { GroupId = 1, ItemText = "A" };
        await _service.CreateValTemplateItemAsync(item);
        item.ItemText = "B";
        var updated = await _service.UpdateValTemplateItemAsync(item.ItemId, item);
        updated.Should().NotBeNull();
        updated!.ItemText.Should().Be("B");
    }

    [Fact]
    public async Task UpdateValTemplateItemAsync_WhenNotFound_ReturnsNull()
    {
        var item = new ValtemplateItem { GroupId = 1, ItemText = "A" };
        var updated = await _service.UpdateValTemplateItemAsync(999, item);
        updated.Should().BeNull();
    }
}
