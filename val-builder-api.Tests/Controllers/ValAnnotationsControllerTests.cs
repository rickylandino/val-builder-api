using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using val_builder_api.Controllers;
using val_builder_api.Models;
using val_builder_api.Services;
using Xunit;

namespace val_builder_api.Tests.Controllers;

public class ValAnnotationsControllerTests
{
    private readonly Mock<IValAnnotationService> _mockService;
    private readonly ValAnnotationsController _controller;

    public ValAnnotationsControllerTests()
    {
        _mockService = new Mock<IValAnnotationService>();
        _controller = new ValAnnotationsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithAnnotations()
    {
        var items = new[] { new Valannotation { AnnotationId = 1 }, new Valannotation { AnnotationId = 2 } };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(items);

        var result = await _controller.GetAll();
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ((IEnumerable<Valannotation>)ok.Value!).Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenExists()
    {
        var item = new Valannotation { AnnotationId = 1 };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _controller.GetById(1);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Valannotation?)null);

        var result = await _controller.GetById(1);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Add_ReturnsCreatedAtAction()
    {
        var item = new Valannotation { AnnotationId = 1 };
        _mockService.Setup(s => s.AddAsync(item)).ReturnsAsync(item);

        var result = await _controller.Add(item);
        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenExists()
    {
        var item = new Valannotation { AnnotationId = 1 };
        _mockService.Setup(s => s.UpdateAsync(1, item)).ReturnsAsync(item);

        var result = await _controller.Update(1, item);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMissing()
    {
        var item = new Valannotation { AnnotationId = 1 };
        _mockService.Setup(s => s.UpdateAsync(1, item)).ReturnsAsync((Valannotation?)null);

        var result = await _controller.Update(1, item);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

        var result = await _controller.Delete(1);
        result.Should().BeOfType<NotFoundResult>();
    }
}
