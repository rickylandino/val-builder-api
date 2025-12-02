using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using val_builder_api.Controllers;
using val_builder_api.Dto;
using val_builder_api.Models;
using val_builder_api.Services;
using Xunit;

namespace val_builder_api.Tests.Controllers;

public class ValTemplateItemsControllerTests
{
    private readonly Mock<IValTemplateItemService> _mockService;
    private readonly ValTemplateItemsController _controller;

    public ValTemplateItemsControllerTests()
    {
        _mockService = new Mock<IValTemplateItemService>();
        _controller = new ValTemplateItemsController(_mockService.Object);
    }

    [Fact]
    public async Task GetValTemplateItems_ReturnsOkOrdered()
    {
        var items = new[] {
            new ValtemplateItem { GroupId = 1, ItemText = "B", DisplayOrder = 1 },
            new ValtemplateItem { GroupId = 1, ItemText = "A", DisplayOrder = 2 }
        };
        _mockService.Setup(s => s.GetValTemplateItemsByGroupIdAsync(1)).ReturnsAsync(items);
        var result = await _controller.GetValTemplateItems(1);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<IEnumerable<ValtemplateItem>>().Subject.ToList();
        returned.Should().HaveCount(2);
        // Check that returned items are ordered by DisplayOrder ascending
        returned[0].DisplayOrder!.Value.Should().Be(1);
    }

    [Fact]
    public async Task GetValTemplateItemById_WhenFound_ReturnsOk()
    {
        var item = new ValtemplateItem { ItemId = 1, ItemText = "A" };
        _mockService.Setup(s => s.GetValTemplateItemByIdAsync(1)).ReturnsAsync(item);
        var result = await _controller.GetValTemplateItemById(1);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<ValtemplateItem>().Subject;
        returned.ItemId.Should().Be(1);
    }

    [Fact]
    public async Task GetValTemplateItemById_WhenNotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetValTemplateItemByIdAsync(999)).ReturnsAsync((ValtemplateItem?)null);
        var result = await _controller.GetValTemplateItemById(999);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateValTemplateItem_ReturnsCreated()
    {
        var item = new ValtemplateItem { ItemId = 1, ItemText = "A" };
        _mockService.Setup(s => s.CreateValTemplateItemAsync(item)).ReturnsAsync(item);
        var result = await _controller.CreateValTemplateItem(item);
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returned = created.Value.Should().BeAssignableTo<ValtemplateItem>().Subject;
        returned.ItemId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateValTemplateItem_WhenFound_ReturnsOk()
    {
        var item = new ValtemplateItem { ItemId = 1, ItemText = "A" };
        _mockService.Setup(s => s.UpdateValTemplateItemAsync(1, item)).ReturnsAsync(item);
        var result = await _controller.UpdateValTemplateItem(1, item);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<ValtemplateItem>().Subject;
        returned.ItemId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateValTemplateItem_WhenNotFound_ReturnsNotFound()
    {
        var item = new ValtemplateItem { ItemId = 1, ItemText = "A" };
        _mockService.Setup(s => s.UpdateValTemplateItemAsync(999, item)).ReturnsAsync((ValtemplateItem?)null);
        var result = await _controller.UpdateValTemplateItem(999, item);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateDisplayOrderBulk_ReturnsNoContent_OnSuccess()
    {
        var mockService = new Mock<IValTemplateItemService>();
        mockService.Setup(s => s.UpdateDisplayOrderBulkAsync(It.IsAny<int>(), It.IsAny<List<ValTemplateItemDisplayOrderUpdateDto.ItemOrder>>()))
            .Returns(Task.CompletedTask);

        var controller = new ValTemplateItemsController(mockService.Object);

        var dto = new ValTemplateItemDisplayOrderUpdateDto
        {
            GroupId = 2,
            Items = new List<ValTemplateItemDisplayOrderUpdateDto.ItemOrder>
            {
                new() { ItemId = 10, DisplayOrder = 1 },
                new() { ItemId = 11, DisplayOrder = 2 }
            }
        };

        var result = await controller.UpdateDisplayOrderBulk(dto);

        Assert.IsType<NoContentResult>(result);
    }
}
