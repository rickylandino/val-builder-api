using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using val_builder_api.Controllers;
using val_builder_api.Models;
using val_builder_api.Services;
using Xunit;

namespace val_builder_api.Tests.Controllers;

public class ValHeaderControllerTests
{
    private readonly Mock<IValHeaderService> _mockService;
    private readonly ValHeaderController _controller;

    public ValHeaderControllerTests()
    {
        _mockService = new Mock<IValHeaderService>();
        _controller = new ValHeaderController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllValHeaders_ReturnsOkWithHeaders()
    {
        var headers = new List<Valheader> { new Valheader { ValId = 1, ValDescription = "A" } };
        _mockService.Setup(s => s.GetAllValHeadersAsync()).ReturnsAsync(headers);
        var result = await _controller.GetAllValHeaders(null, null);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<IEnumerable<Valheader>>().Subject;
        returned.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetValHeaderById_WhenFound_ReturnsOk()
    {
        var header = new Valheader { ValId = 1, ValDescription = "A" };
        _mockService.Setup(s => s.GetValHeaderByIdAsync(1)).ReturnsAsync(header);
        var result = await _controller.GetValHeaderById(1);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<Valheader>().Subject;
        returned.ValId.Should().Be(1);
    }

    [Fact]
    public async Task GetValHeaderById_WhenNotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetValHeaderByIdAsync(999)).ReturnsAsync((Valheader?)null);
        var result = await _controller.GetValHeaderById(999);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateValHeader_ReturnsCreated()
    {
        var header = new Valheader { ValId = 1, ValDescription = "A" };
        _mockService.Setup(s => s.CreateValHeaderAsync(header)).ReturnsAsync(header);
        var result = await _controller.CreateValHeader(header);
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returned = created.Value.Should().BeAssignableTo<Valheader>().Subject;
        returned.ValId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateValHeader_WhenFound_ReturnsOk()
    {
        var header = new Valheader { ValId = 1, ValDescription = "A" };
        _mockService.Setup(s => s.UpdateValHeaderAsync(1, header)).ReturnsAsync(header);
        var result = await _controller.UpdateValHeader(1, header);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<Valheader>().Subject;
        returned.ValId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateValHeader_WhenNotFound_ReturnsNotFound()
    {
        var header = new Valheader { ValId = 1, ValDescription = "A" };
        _mockService.Setup(s => s.UpdateValHeaderAsync(999, header)).ReturnsAsync((Valheader?)null);
        var result = await _controller.UpdateValHeader(999, header);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
