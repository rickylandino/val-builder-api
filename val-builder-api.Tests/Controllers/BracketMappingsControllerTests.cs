using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Controllers;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Controllers;

public class BracketMappingsControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BracketMappingService _service;
    private readonly BracketMappingsController _controller;

    public BracketMappingsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _service = new BracketMappingService(_context);
        _controller = new BracketMappingsController(_service);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAll_ReturnsAllMappings()
    {
        await _service.CreateAsync(new BracketMapping { TagName = "Tag1", ObjectPath = "Path1" });
        await _service.CreateAsync(new BracketMapping { TagName = "Tag2", ObjectPath = "Path2" });
        var result = await _controller.GetAll();
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var mappings = ok!.Value as IEnumerable<BracketMapping>;
        mappings.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_AddsMapping()
    {
        var mapping = new BracketMapping { TagName = "Tag1", ObjectPath = "Path1" };
        var result = await _controller.Create(mapping);
        var created = result.Result as CreatedAtActionResult;
        created.Should().NotBeNull();
        var value = created!.Value as BracketMapping;
        value.Should().NotBeNull();
        value!.TagName.Should().Be("Tag1");
    }

    [Fact]
    public async Task Update_UpdatesMapping()
    {
        var mapping = new BracketMapping { TagName = "Tag1", ObjectPath = "Path1" };
        var created = await _service.CreateAsync(mapping);
        created.TagName = "Tag2";
        var result = await _controller.Update(created.Id.Value, created);
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var updated = ok!.Value as BracketMapping;
        updated.Should().NotBeNull();
        updated!.TagName.Should().Be("Tag2");
    }

    [Fact]
    public async Task Delete_RemovesMapping()
    {
        var mapping = new BracketMapping { TagName = "Tag1", ObjectPath = "Path1" };
        var created = await _service.CreateAsync(mapping);
        var result = await _controller.Delete(created.Id.Value);
        result.Should().BeOfType<NoContentResult>();
        (await _service.GetAllAsync()).Should().BeEmpty();
    }
}
