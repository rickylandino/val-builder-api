using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Services;

public class ValAnnotationServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ValAnnotationService _service;

    public ValAnnotationServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _service = new ValAnnotationService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAnnotations()
    {
        var items = new[] {
            new Valannotation { AnnotationId = 1, ValId = 1, AnnotationContent = "A" },
            new Valannotation { AnnotationId = 2, ValId = 2, AnnotationContent = "B" }
        };
        await _context.Valannotations.AddRangeAsync(items);
        await _context.SaveChangesAsync();

        var result = await _service.GetAllAsync();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAnnotation_WhenExists()
    {
        var item = new Valannotation { AnnotationId = 1, ValId = 1, AnnotationContent = "A" };
        await _context.Valannotations.AddAsync(item);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(1);
        result.Should().NotBeNull();
        result!.AnnotationContent.Should().Be("A");
    }

    [Fact]
    public async Task AddAsync_CreatesAnnotation()
    {
        var item = new Valannotation { AnnotationId = 3, ValId = 1, AnnotationContent = "A" };
        var result = await _service.AddAsync(item);
        result.Should().NotBeNull();
        result.AnnotationContent.Should().Be("A");
        (await _context.Valannotations.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAnnotation_WhenExists()
    {
        var item = new Valannotation { AnnotationId = 1, ValId = 1, AnnotationContent = "A" };
        await _context.Valannotations.AddAsync(item);
        await _context.SaveChangesAsync();

        var update = new Valannotation { AnnotationId = 1, ValId = 2, AnnotationContent = "B" };
        var result = await _service.UpdateAsync(1, update);
        result.Should().NotBeNull();
        result!.ValId.Should().Be(2);
        result.AnnotationContent.Should().Be("B");
    }

    [Fact]
    public async Task DeleteAsync_RemovesAnnotation_WhenExists()
    {
        var item = new Valannotation { AnnotationId = 1, ValId = 1, AnnotationContent = "A" };
        await _context.Valannotations.AddAsync(item);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteAsync(1);
        result.Should().BeTrue();
        (await _context.Valannotations.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task GetByValIdAsync_ReturnsAnnotationsForGivenValId()
    {
        var items = new[] {
            new Valannotation { AnnotationId = 1, ValId = 1, AnnotationContent = "A" },
            new Valannotation { AnnotationId = 2, ValId = 2, AnnotationContent = "B" },
            new Valannotation { AnnotationId = 3, ValId = 1, AnnotationContent = "C" }
        };
        await _context.Valannotations.AddRangeAsync(items);
        await _context.SaveChangesAsync();

        var result = (await _service.GetByValIdAsync(1)).ToList();
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.AnnotationContent == "A");
        result.Should().Contain(x => x.AnnotationContent == "C");
        result.Should().NotContain(x => x.AnnotationContent == "B");
    }
}