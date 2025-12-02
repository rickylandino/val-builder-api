using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Services;

public class ValPdfServiceTests
{
    private static ApplicationDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static Mock<ILogger<ValPdfService>> GetMockLogger()
    {
        return new Mock<ILogger<ValPdfService>>();
    }

    [Fact]
    public async Task GetValDataForPdfAsync_ReturnsNull_WhenValNotFound()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var logger = GetMockLogger();
        var service = new ValPdfService(context, logger.Object);

        // Act
        var result = await service.GetValDataForPdfAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetValDataForPdfAsync_ReturnsData_WhenValExists()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var logger = GetMockLogger();
        var service = new ValPdfService(context, logger.Object);

        var valHeader = new Valheader
        {
            ValId = 1,
            ValDescription = "Test VAL",
            PlanYearBeginDate = new DateTime(2024, 1, 1),
            PlanYearEndDate = new DateTime(2024, 12, 31),
            RecipientName = "Test Client"
        };

        context.Valheaders.Add(valHeader);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetValDataForPdfAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ValHeader.ValId);
        Assert.Equal("Test VAL", result.ValHeader.ValDescription);
        Assert.Equal("Test Client", result.ValHeader.RecipientName);
    }

    [Fact]
    public async Task GeneratePdfAsync_ReturnsValidPdfBytes()
    {
        // Arrange
        var logger = GetMockLogger();
        using var context = GetInMemoryContext();
        var service = new ValPdfService(context, logger.Object);

        var data = new ValPdfData
        {
            ValHeader = new ValPdfHeader
            {
                ValId = 1,
                ValDescription = "Test VAL",
                RecipientName = "Test Client",
                PlanYearBeginDate = new DateTime(2024, 1, 1),
                PlanYearEndDate = new DateTime(2024, 12, 31)
            },
            Sections = new List<ValPdfSection>
            {
                new ValPdfSection
                {
                    GroupId = 1,
                    SectionText = "Test Section",
                    DisplayOrder = 1,
                    Details = new List<ValPdfDetail>
                    {
                        new ValPdfDetail
                        {
                            ValDetailsId = Guid.NewGuid(),
                            GroupId = 1,
                            DetailText = "Test detail content",
                            DisplayOrder = 1,
                            Bullet = false,
                            Bold = false,
                            Center = false
                        }
                    }
                }
            }
        };

        // Act
        var result = await service.GeneratePdfAsync(data, includeHeaders: true, showWatermark: false);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
        // PDF files start with %PDF
        Assert.Equal(0x25, result[0]); // %
        Assert.Equal(0x50, result[1]); // P
        Assert.Equal(0x44, result[2]); // D
        Assert.Equal(0x46, result[3]); // F
    }

    [Fact]
    public void MergePdfs_ReturnsMergedPdf_WhenMultipleValidPdfsProvided()
    {
        var logger = GetMockLogger();
        using var context = GetInMemoryContext();
        var service = new ValPdfService(context, logger.Object);

        // Minimal valid PDF bytes
        byte[] pdf1 = Encoding.ASCII.GetBytes("%PDF-1.4\n1 0 obj\n<<>>\nendobj\nxref\n0 1\n0000000000 65535 f \ntrailer\n<<>>\nstartxref\n9\n%%EOF");
        byte[] pdf2 = Encoding.ASCII.GetBytes("%PDF-1.4\n1 0 obj\n<<>>\nendobj\nxref\n0 1\n0000000000 65535 f \ntrailer\n<<>>\nstartxref\n9\n%%EOF");

        var merged = service.GetType()
            .GetMethod("MergePdfs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(service, new object[] { new List<byte[]> { pdf1, pdf2 } }) as byte[];

        Assert.NotNull(merged);
        Assert.True(merged.Length > 0);
        Assert.Equal(0x25, merged[0]); // %
        Assert.Equal(0x50, merged[1]); // P
        Assert.Equal(0x44, merged[2]); // D
        Assert.Equal(0x46, merged[3]); // F
    }

    [Fact]
    public void MergePdfs_ReturnsEmptyPdf_WhenNoPdfsProvided()
    {
        var logger = GetMockLogger();
        using var context = GetInMemoryContext();
        var service = new ValPdfService(context, logger.Object);

        var merged = service.GetType()
            .GetMethod("MergePdfs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(service, new object[] { new List<byte[]>() }) as byte[];

        Assert.NotNull(merged);
        Assert.True(merged.Length > 0);
        Assert.Equal(0x25, merged[0]); // %
        Assert.Equal(0x50, merged[1]); // P
        Assert.Equal(0x44, merged[2]); // D
        Assert.Equal(0x46, merged[3]); // F
    }

    [Fact]
    public void MergePdfs_SkipsInvalidPdf_AndLogsWarning()
    {
        var logger = new Mock<ILogger<ValPdfService>>();
        using var context = GetInMemoryContext();
        var service = new ValPdfService(context, logger.Object);

        byte[] validPdf = Encoding.ASCII.GetBytes("%PDF-1.4\n1 0 obj\n<<>>\nendobj\nxref\n0 1\n0000000000 65535 f \ntrailer\n<<>>\nstartxref\n9\n%%EOF");
        byte[] invalidPdf = Encoding.ASCII.GetBytes("not a pdf");

        var merged = service.GetType()
            .GetMethod("MergePdfs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(service, new object[] { new List<byte[]> { validPdf, invalidPdf } }) as byte[];

        Assert.NotNull(merged);
        Assert.True(merged.Length > 0);
        Assert.Equal(0x25, merged[0]); // %
        Assert.Equal(0x50, merged[1]); // P
        Assert.Equal(0x44, merged[2]); // D
        Assert.Equal(0x46, merged[3]); // F

        logger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Skipping PDF during merge due to error")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Theory]
    [InlineData("<<Test>>", "<p class='val-detail'>Test</p>")]
    [InlineData("&lt;&lt;Important&gt;&gt;", "<p class='val-detail'>Important</p>")]
    [InlineData("<<  Data  >>", "<p class='val-detail'>Data</p>")]
    [InlineData("No chevrons here", "<p class='val-detail'>No chevrons here</p>")]
    public void RenderValDetail_StripsChevrons_FromPlainText(string input, string expected)
    {
        var logger = GetMockLogger();
        using var context = GetInMemoryContext();
        var service = new ValPdfService(context, logger.Object);

        var detail = new ValPdfDetail { DetailText = input };
        var result = service.GetType()
            .GetMethod("RenderValDetail", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(service, new object[] { detail }) as string;

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("<p>&lt;&lt;Secret&gt;&gt;</p>", "<p class=\"val-detail\">Secret</p>")]
    [InlineData("<p>Normal text</p>", "<p class=\"val-detail\">Normal text</p>")]
    public void RenderValDetail_StripsChevrons_FromParagraphHtml(string input, string expected)
    {
        var logger = GetMockLogger();
        using var context = GetInMemoryContext();
        var service = new ValPdfService(context, logger.Object);

        var detail = new ValPdfDetail { DetailText = input };
        var result = service.GetType()
            .GetMethod("RenderValDetail", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(service, new object[] { detail }) as string;

        Assert.Equal(expected, result);
    }
}
