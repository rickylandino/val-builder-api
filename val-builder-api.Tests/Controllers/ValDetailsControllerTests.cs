using Microsoft.AspNetCore.Mvc;
using Moq;
using val_builder_api.Controllers;
using val_builder_api.Models;
using val_builder_api.Services;
using Xunit;

namespace val_builder_api.Tests.Controllers;

public class ValDetailsControllerTests
{
    private readonly Mock<IValDetailService> _mockService;
    private readonly ValDetailsController _controller;

    public ValDetailsControllerTests()
    {
        _mockService = new Mock<IValDetailService>();
        _controller = new ValDetailsController(_mockService.Object);
    }

    [Fact]
    public async Task GetValDetails_ReturnsOkResult_WithDetails()
    {
        // Arrange
        var valId = 1;
        var details = new List<Valdetail>
        {
            new Valdetail { ValDetailsId = Guid.NewGuid(), ValId = valId, GroupContent = "Detail 1" },
            new Valdetail { ValDetailsId = Guid.NewGuid(), ValId = valId, GroupContent = "Detail 2" }
        };

        _mockService.Setup(s => s.GetValDetailsAsync(valId, null))
            .ReturnsAsync(details);

        // Act
        var result = await _controller.GetValDetails(valId, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDetails = Assert.IsType<IEnumerable<Valdetail>>(okResult.Value, exactMatch: false);
        Assert.Equal(2, returnedDetails.Count());
    }

    [Fact]
    public async Task GetValDetails_WithGroupId_FiltersResults()
    {
        // Arrange
        var valId = 1;
        var groupId = 5;
        var details = new List<Valdetail>
        {
            new Valdetail { ValDetailsId = Guid.NewGuid(), ValId = valId, GroupId = groupId, GroupContent = "Detail 1" }
        };

        _mockService.Setup(s => s.GetValDetailsAsync(valId, groupId))
            .ReturnsAsync(details);

        // Act
        var result = await _controller.GetValDetails(valId, groupId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDetails = Assert.IsType<IEnumerable<Valdetail>>(okResult.Value, exactMatch: false);
        Assert.Single(returnedDetails);
        Assert.All(returnedDetails, d => Assert.Equal(groupId, d.GroupId));
    }

    [Fact]
    public async Task GetValDetailById_ReturnsOkResult_WhenDetailExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var detail = new Valdetail { ValDetailsId = id, ValId = 1, GroupContent = "Test Detail" };

        _mockService.Setup(s => s.GetValDetailByIdAsync(id))
            .ReturnsAsync(detail);

        // Act
        var result = await _controller.GetValDetailById(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDetail = Assert.IsType<Valdetail>(okResult.Value);
        Assert.Equal(id, returnedDetail.ValDetailsId);
    }

    [Fact]
    public async Task GetValDetailById_ReturnsNotFound_WhenDetailDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetValDetailByIdAsync(id))
            .ReturnsAsync((Valdetail?)null);

        // Act
        var result = await _controller.GetValDetailById(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateValDetail_ReturnsCreatedAtAction_WithNewDetail()
    {
        // Arrange
        var detail = new Valdetail { ValId = 1, GroupContent = "New Detail" };
        var createdDetail = new Valdetail { ValDetailsId = Guid.NewGuid(), ValId = 1, GroupContent = "New Detail" };

        _mockService.Setup(s => s.CreateValDetailAsync(detail))
            .ReturnsAsync(createdDetail);

        // Act
        var result = await _controller.CreateValDetail(detail);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.GetValDetailById), createdResult.ActionName);
        var returnedDetail = Assert.IsType<Valdetail>(createdResult.Value);
        Assert.Equal(createdDetail.ValDetailsId, returnedDetail.ValDetailsId);
    }

    [Fact]
    public async Task UpdateValDetail_ReturnsOkResult_WhenDetailExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var detail = new Valdetail { ValDetailsId = id, ValId = 1, GroupContent = "Updated" };

        _mockService.Setup(s => s.UpdateValDetailAsync(id, detail))
            .ReturnsAsync(detail);

        // Act
        var result = await _controller.UpdateValDetail(id, detail);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDetail = Assert.IsType<Valdetail>(okResult.Value);
        Assert.Equal("Updated", returnedDetail.GroupContent);
    }

    [Fact]
    public async Task UpdateValDetail_ReturnsNotFound_WhenDetailDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var detail = new Valdetail { ValDetailsId = id, ValId = 1, GroupContent = "Updated" };

        _mockService.Setup(s => s.UpdateValDetailAsync(id, detail))
            .ReturnsAsync((Valdetail?)null);

        // Act
        var result = await _controller.UpdateValDetail(id, detail);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteValDetail_ReturnsNoContent_WhenDetailExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteValDetailAsync(id))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteValDetail(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteValDetail_ReturnsNotFound_WhenDetailDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteValDetailAsync(id))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteValDetail(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task SaveValDetailChanges_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var valId = 1;
        var dto = new ValDetailChangeDto
        {
            ValId = valId,
            Changes = new List<ValDetailChange>
            {
                new ValDetailChange
                {
                    Action = "create",
                    Detail = new Valdetail { ValId = valId, GroupContent = "New" }
                }
            }
        };

        var saveResult = new ValDetailSaveResult
        {
            Success = true,
            Message = "Successfully saved changes",
            ItemsCreated = 1
        };

        _mockService.Setup(s => s.SaveValDetailChangesAsync(dto))
            .ReturnsAsync(saveResult);

        // Act
        var result = await _controller.SaveValDetailChanges(valId, dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResult = Assert.IsType<ValDetailSaveResult>(okResult.Value);
        Assert.True(returnedResult.Success);
        Assert.Equal(1, returnedResult.ItemsCreated);
    }

    [Fact]
    public async Task SaveValDetailChanges_ReturnsBadRequest_WhenDtoIsNull()
    {
        // Arrange
        var valId = 1;

        // Act
        var result = await _controller.SaveValDetailChanges(valId, null!);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var returnedResult = Assert.IsType<ValDetailSaveResult>(badRequestResult.Value);
        Assert.False(returnedResult.Success);
    }

    [Fact]
    public async Task SaveValDetailChanges_ReturnsBadRequest_WhenValIdMismatch()
    {
        // Arrange
        var valId = 1;
        var dto = new ValDetailChangeDto
        {
            ValId = 2, // Mismatch
            Changes = new List<ValDetailChange>()
        };

        // Act
        var result = await _controller.SaveValDetailChanges(valId, dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var returnedResult = Assert.IsType<ValDetailSaveResult>(badRequestResult.Value);
        Assert.False(returnedResult.Success);
    }

    [Fact]
    public async Task SaveValDetailChanges_ReturnsBadRequest_WhenChangesIsNull()
    {
        // Arrange
        var valId = 1;
        var dto = new ValDetailChangeDto
        {
            ValId = valId,
            Changes = null!
        };

        // Act
        var result = await _controller.SaveValDetailChanges(valId, dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var returnedResult = Assert.IsType<ValDetailSaveResult>(badRequestResult.Value);
        Assert.False(returnedResult.Success);
    }

    [Fact]
    public async Task SaveValDetailChanges_ReturnsInternalServerError_WhenServiceThrowsException()
    {
        // Arrange
        var valId = 1;
        var dto = new ValDetailChangeDto
        {
            ValId = valId,
            Changes = new List<ValDetailChange>()
        };

        var saveResult = new ValDetailSaveResult
        {
            Success = false,
            Error = "Database connection failed",
            Message = "An error occurred while saving changes."
        };

        _mockService.Setup(s => s.SaveValDetailChangesAsync(dto))
            .ReturnsAsync(saveResult);

        // Act
        var result = await _controller.SaveValDetailChanges(valId, dto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var returnedResult = Assert.IsType<ValDetailSaveResult>(statusCodeResult.Value);
        Assert.False(returnedResult.Success);
        Assert.NotNull(returnedResult.Error);
    }

    [Fact]
    public async Task SaveValDetailChanges_ReturnsBadRequest_WhenServiceReturnsErrors()
    {
        // Arrange
        var valId = 1;
        var dto = new ValDetailChangeDto
        {
            ValId = valId,
            Changes = new List<ValDetailChange>
            {
                new ValDetailChange
                {
                    Action = "update",
                    Detail = new Valdetail { ValDetailsId = Guid.NewGuid(), ValId = valId }
                }
            }
        };

        var saveResult = new ValDetailSaveResult
        {
            Success = false,
            Errors = new List<string> { "Detail not found" },
            Message = "Validation errors occurred"
        };

        _mockService.Setup(s => s.SaveValDetailChangesAsync(dto))
            .ReturnsAsync(saveResult);

        // Act
        var result = await _controller.SaveValDetailChanges(valId, dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var returnedResult = Assert.IsType<ValDetailSaveResult>(badRequestResult.Value);
        Assert.False(returnedResult.Success);
        Assert.NotNull(returnedResult.Errors);
        Assert.Single(returnedResult.Errors);
    }
}
