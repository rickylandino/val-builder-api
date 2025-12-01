using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using val_builder_api.Controllers;
using val_builder_api.Models;
using val_builder_api.Services;
using Xunit;

namespace val_builder_api.Tests.Controllers;

public class ValPdfAttachmentsControllerTests
{
    private readonly Mock<IValPdfAttachmentService> _mockService;
    private readonly ValPdfAttachmentsController _controller;

    public ValPdfAttachmentsControllerTests()
    {
        _mockService = new Mock<IValPdfAttachmentService>();
        _controller = new ValPdfAttachmentsController(_mockService.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_WhenAttachmentsExist_ReturnsOkWithAttachments()
    {
        // Arrange
        var attachments = new List<ValPdfAttachment>
        {
            new ValPdfAttachment { PDFId = 1, ValID = 1, DisplayOrder = 1 },
            new ValPdfAttachment { PDFId = 2, ValID = 1, DisplayOrder = 2 }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(attachments);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAttachments = okResult.Value.Should().BeAssignableTo<IEnumerable<ValPdfAttachment>>().Subject;
        returnedAttachments.Should().HaveCount(2);
        returnedAttachments.Should().BeEquivalentTo(attachments);
        _mockService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAll_WhenNoAttachmentsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ValPdfAttachment>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAttachments = okResult.Value.Should().BeAssignableTo<IEnumerable<ValPdfAttachment>>().Subject;
        returnedAttachments.Should().BeEmpty();
        _mockService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WhenAttachmentExists_ReturnsOkWithAttachment()
    {
        // Arrange
        var attachment = new ValPdfAttachment { PDFId = 1, ValID = 1, DisplayOrder = 1 };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(attachment);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAttachment = okResult.Value.Should().BeAssignableTo<ValPdfAttachment>().Subject;
        returnedAttachment.Should().BeEquivalentTo(attachment);
        _mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenAttachmentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((ValPdfAttachment?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _mockService.Verify(s => s.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task GetById_WithValidId_CallsServiceWithCorrectId()
    {
        // Arrange
        var id = 5;
        var attachment = new ValPdfAttachment { PDFId = id, ValID = 1, DisplayOrder = 1 };
        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(attachment);

        // Act
        await _controller.GetById(id);

        // Assert
        _mockService.Verify(s => s.GetByIdAsync(id), Times.Once);
    }

    #endregion

    #region GetByValId Tests

    [Fact]
    public async Task GetByValId_WhenAttachmentsExistForVal_ReturnsOkWithAttachments()
    {
        // Arrange
        var valId = 1;
        var attachments = new List<ValPdfAttachment>
        {
            new ValPdfAttachment { PDFId = 1, ValID = valId, DisplayOrder = 1 },
            new ValPdfAttachment { PDFId = 2, ValID = valId, DisplayOrder = 2 }
        };
        _mockService.Setup(s => s.GetByValIdAsync(valId)).ReturnsAsync(attachments);

        // Act
        var result = await _controller.GetByValId(valId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAttachments = okResult.Value.Should().BeAssignableTo<IEnumerable<ValPdfAttachment>>().Subject;
        returnedAttachments.Should().HaveCount(2);
        returnedAttachments.Should().BeEquivalentTo(attachments);
        returnedAttachments.All(a => a.ValID == valId).Should().BeTrue();
        _mockService.Verify(s => s.GetByValIdAsync(valId), Times.Once);
    }

    [Fact]
    public async Task GetByValId_WhenNoAttachmentsExistForVal_ReturnsOkWithEmptyList()
    {
        // Arrange
        var valId = 999;
        _mockService.Setup(s => s.GetByValIdAsync(valId)).ReturnsAsync(new List<ValPdfAttachment>());

        // Act
        var result = await _controller.GetByValId(valId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAttachments = okResult.Value.Should().BeAssignableTo<IEnumerable<ValPdfAttachment>>().Subject;
        returnedAttachments.Should().BeEmpty();
        _mockService.Verify(s => s.GetByValIdAsync(valId), Times.Once);
    }

    #endregion

    #region Add Tests

    [Fact]
    public async Task Add_WithValidAttachment_ReturnsCreatedAtAction()
    {
        // Arrange
        var newAttachment = new ValPdfAttachment { ValID = 1, DisplayOrder = 1 };
        var createdAttachment = new ValPdfAttachment { PDFId = 1, ValID = 1, DisplayOrder = 1 };
        _mockService.Setup(s => s.AddAsync(newAttachment)).ReturnsAsync(createdAttachment);

        // Act
        var result = await _controller.Add(newAttachment);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetById));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues!["id"].Should().Be(1);
        var returnedAttachment = createdResult.Value.Should().BeAssignableTo<ValPdfAttachment>().Subject;
        returnedAttachment.Should().BeEquivalentTo(createdAttachment);
        _mockService.Verify(s => s.AddAsync(newAttachment), Times.Once);
    }

    [Fact]
    public async Task Add_WithPDFContents_CreatesAttachmentWithContents()
    {
        // Arrange
        var pdfContent = new byte[] { 0x25, 0x50, 0x44, 0x46 };
        var newAttachment = new ValPdfAttachment 
        { 
            ValID = 1, 
            DisplayOrder = 1, 
            PDFContents = pdfContent 
        };
        var createdAttachment = new ValPdfAttachment 
        { 
            PDFId = 1, 
            ValID = 1, 
            DisplayOrder = 1, 
            PDFContents = pdfContent 
        };
        _mockService.Setup(s => s.AddAsync(newAttachment)).ReturnsAsync(createdAttachment);

        // Act
        var result = await _controller.Add(newAttachment);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedAttachment = createdResult.Value.Should().BeAssignableTo<ValPdfAttachment>().Subject;
        returnedAttachment.PDFContents.Should().NotBeNull();
        returnedAttachment.PDFContents.Should().BeEquivalentTo(pdfContent);
        _mockService.Verify(s => s.AddAsync(newAttachment), Times.Once);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WhenAttachmentExists_ReturnsOkWithUpdatedAttachment()
    {
        // Arrange
        var id = 1;
        var updateAttachment = new ValPdfAttachment { PDFId = id, ValID = 1, DisplayOrder = 2 };
        _mockService.Setup(s => s.UpdateAsync(id, updateAttachment)).ReturnsAsync(updateAttachment);

        // Act
        var result = await _controller.Update(id, updateAttachment);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAttachment = okResult.Value.Should().BeAssignableTo<ValPdfAttachment>().Subject;
        returnedAttachment.Should().BeEquivalentTo(updateAttachment);
        _mockService.Verify(s => s.UpdateAsync(id, updateAttachment), Times.Once);
    }

    [Fact]
    public async Task Update_WhenAttachmentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var id = 999;
        var updateAttachment = new ValPdfAttachment { PDFId = id, ValID = 1, DisplayOrder = 2 };
        _mockService.Setup(s => s.UpdateAsync(id, updateAttachment)).ReturnsAsync((ValPdfAttachment?)null);

        // Act
        var result = await _controller.Update(id, updateAttachment);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _mockService.Verify(s => s.UpdateAsync(id, updateAttachment), Times.Once);
    }

    [Fact]
    public async Task Update_WithMismatchedIds_StillCallsServiceWithUrlId()
    {
        // Arrange
        var urlId = 1;
        var bodyId = 2;
        var updateAttachment = new ValPdfAttachment { PDFId = bodyId, ValID = 1, DisplayOrder = 2 };
        var updatedAttachment = new ValPdfAttachment { PDFId = urlId, ValID = 1, DisplayOrder = 2 };
        _mockService.Setup(s => s.UpdateAsync(urlId, updateAttachment)).ReturnsAsync(updatedAttachment);

        // Act
        var result = await _controller.Update(urlId, updateAttachment);

        // Assert
        _mockService.Verify(s => s.UpdateAsync(urlId, updateAttachment), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WhenAttachmentExists_ReturnsNoContent()
    {
        // Arrange
        var id = 1;
        _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenAttachmentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var id = 999;
        _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockService.Verify(s => s.DeleteAsync(id), Times.Once);
    }

    #endregion

    #region Route and Attribute Tests

    [Fact]
    public void Controller_HasCorrectRouteAttribute()
    {
        // Arrange & Act
        var routeAttribute = (RouteAttribute?)Attribute.GetCustomAttribute(
            typeof(ValPdfAttachmentsController), 
            typeof(RouteAttribute));

        // Assert
        routeAttribute.Should().NotBeNull();
        routeAttribute!.Template.Should().Be("api/valpdfattachments");
    }

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {
        // Arrange & Act
        var apiControllerAttribute = Attribute.GetCustomAttribute(
            typeof(ValPdfAttachmentsController), 
            typeof(ApiControllerAttribute));

        // Assert
        apiControllerAttribute.Should().NotBeNull();
    }

    #endregion
}
