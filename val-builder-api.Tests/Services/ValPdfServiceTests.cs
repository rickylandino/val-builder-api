using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
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
}
