using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using val_builder_api.Controllers;
using val_builder_api.Models;
using val_builder_api.Services;
using Xunit;

namespace val_builder_api.Tests.Controllers;

public class ValPdfControllerTests
{
    private readonly Mock<IValPdfService> _mockPdfService;
    private readonly ValPdfController _controller;

    public ValPdfControllerTests()
    {
        _mockPdfService = new Mock<IValPdfService>();
        _controller = new ValPdfController(_mockPdfService.Object);
    }

    [Fact]
    public async Task GenerateValPdf_ReturnsNotFound_WhenValDoesNotExist()
    {
        // Arrange
        var valId = 999;
        _mockPdfService.Setup(s => s.GetValDataForPdfAsync(valId))
            .ReturnsAsync((ValPdfData?)null);

        // Act
        var result = await _controller.GenerateValPdf(valId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    [Fact]
    public async Task GenerateValPdf_ReturnsFileResult_WhenSuccessful()
    {
        // Arrange
        var valId = 1;
        var valData = new ValPdfData
        {
            ValHeader = new ValPdfHeader
            {
                ValId = valId,
                ValDescription = "Test VAL",
                RecipientName = "Test Client"
            },
            Sections = new List<ValPdfSection>()
        };

        var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // %PDF

        _mockPdfService.Setup(s => s.GetValDataForPdfAsync(valId))
            .ReturnsAsync(valData);
        _mockPdfService.Setup(s => s.GeneratePdfAsync(valData, false, true))
            .ReturnsAsync(pdfBytes);

        // Act
        var result = await _controller.GenerateValPdf(valId);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", fileResult.ContentType);
        Assert.Equal(pdfBytes, fileResult.FileContents);
        Assert.StartsWith("VAL-1-", fileResult.FileDownloadName);
        Assert.EndsWith(".pdf", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task GenerateValPdf_PassesIncludeHeadersParameter()
    {
        // Arrange
        var valId = 1;
        var includeHeaders = true;
        var valData = new ValPdfData
        {
            ValHeader = new ValPdfHeader { ValId = valId },
            Sections = new List<ValPdfSection>()
        };

        _mockPdfService.Setup(s => s.GetValDataForPdfAsync(valId))
            .ReturnsAsync(valData);
        _mockPdfService.Setup(s => s.GeneratePdfAsync(valData, includeHeaders, true))
            .ReturnsAsync(new byte[] { 0x25, 0x50, 0x44, 0x46 });

        // Act
        await _controller.GenerateValPdf(valId, includeHeaders);

        // Assert
        _mockPdfService.Verify(s => s.GeneratePdfAsync(valData, includeHeaders, true), Times.Once);
    }

    [Fact]
    public async Task GenerateValPdf_PassesShowWatermarkParameter()
    {
        // Arrange
        var valId = 1;
        var showWatermark = false;
        var valData = new ValPdfData
        {
            ValHeader = new ValPdfHeader { ValId = valId },
            Sections = new List<ValPdfSection>()
        };

        _mockPdfService.Setup(s => s.GetValDataForPdfAsync(valId))
            .ReturnsAsync(valData);
        _mockPdfService.Setup(s => s.GeneratePdfAsync(valData, false, showWatermark))
            .ReturnsAsync(new byte[] { 0x25, 0x50, 0x44, 0x46 });

        // Act
        await _controller.GenerateValPdf(valId, showWatermark: showWatermark);

        // Assert
        _mockPdfService.Verify(s => s.GeneratePdfAsync(valData, false, showWatermark), Times.Once);
    }

    [Fact]
    public async Task GenerateValPdf_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var valId = 1;
        var valData = new ValPdfData
        {
            ValHeader = new ValPdfHeader { ValId = valId },
            Sections = new List<ValPdfSection>()
        };

        _mockPdfService.Setup(s => s.GetValDataForPdfAsync(valId))
            .ReturnsAsync(valData);
        _mockPdfService.Setup(s => s.GeneratePdfAsync(It.IsAny<ValPdfData>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ThrowsAsync(new Exception("PDF generation failed"));

        // Act
        var result = await _controller.GenerateValPdf(valId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GenerateValPdf_DefaultParameters_IncludeHeadersFalse_WatermarkTrue()
    {
        // Arrange
        var valId = 1;
        var valData = new ValPdfData
        {
            ValHeader = new ValPdfHeader { ValId = valId },
            Sections = new List<ValPdfSection>()
        };

        _mockPdfService.Setup(s => s.GetValDataForPdfAsync(valId))
            .ReturnsAsync(valData);
        _mockPdfService.Setup(s => s.GeneratePdfAsync(valData, false, true))
            .ReturnsAsync(new byte[] { 0x25, 0x50, 0x44, 0x46 });

        // Act
        await _controller.GenerateValPdf(valId); // No parameters

        // Assert
        _mockPdfService.Verify(s => s.GeneratePdfAsync(valData, false, true), Times.Once);
    }
}
