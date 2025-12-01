using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using val_builder_api.Controllers;
using val_builder_api.Models;
using val_builder_api.Services;
using Xunit;

namespace val_builder_api.Tests.Controllers;

public class ValSectionsControllerTests
{
    private readonly Mock<IValSectionService> _mockValSectionService;
    private readonly ValSectionsController _controller;

    public ValSectionsControllerTests()
    {
        _mockValSectionService = new Mock<IValSectionService>();
        _controller = new ValSectionsController(_mockValSectionService.Object);
    }

    [Fact]
    public async Task GetAllValSections_WhenValSectionsExist_ReturnsOkWithValSections()
    {
        // Arrange
        var valSections = new List<Valsection>
        {
            new Valsection
            {
                GroupId = 1,
                SectionText = "Section 1",
                DisplayOrder = 1,
                DefaultColWidth1 = 100
            },
            new Valsection
            {
                GroupId = 2,
                SectionText = "Section 2",
                DisplayOrder = 2,
                DefaultColWidth1 = 150
            }
        };

        _mockValSectionService
            .Setup(s => s.GetAllValSectionsAsync())
            .ReturnsAsync(valSections);

        // Act
        var result = await _controller.GetAllValSections();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValSections = okResult.Value.Should().BeAssignableTo<IEnumerable<Valsection>>().Subject;
        returnedValSections.Should().HaveCount(2);
        returnedValSections.Should().BeEquivalentTo(valSections);
    }

    [Fact]
    public async Task GetAllValSections_WhenNoValSections_ReturnsOkWithEmptyList()
    {
        // Arrange
        var emptyList = new List<Valsection>();
        _mockValSectionService
            .Setup(s => s.GetAllValSectionsAsync())
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetAllValSections();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValSections = okResult.Value.Should().BeAssignableTo<IEnumerable<Valsection>>().Subject;
        returnedValSections.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllValSections_CallsServiceOnce()
    {
        // Arrange
        _mockValSectionService
            .Setup(s => s.GetAllValSectionsAsync())
            .ReturnsAsync(new List<Valsection>());

        // Act
        await _controller.GetAllValSections();

        // Assert
        _mockValSectionService.Verify(s => s.GetAllValSectionsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetValSectionByGroupId_WhenValSectionExists_ReturnsOkWithValSection()
    {
        // Arrange
        var valSection = new Valsection
        {
            GroupId = 1,
            SectionText = "Test Section",
            DisplayOrder = 1,
            DefaultColWidth1 = 100,
            DefaultColType1 = "Text"
        };

        _mockValSectionService
            .Setup(s => s.GetValSectionByGroupIdAsync(1))
            .ReturnsAsync(valSection);

        // Act
        var result = await _controller.GetValSectionByGroupId(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValSection = okResult.Value.Should().BeAssignableTo<Valsection>().Subject;
        returnedValSection.Should().BeEquivalentTo(valSection);
    }

    [Fact]
    public async Task GetValSectionByGroupId_WhenValSectionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockValSectionService
            .Setup(s => s.GetValSectionByGroupIdAsync(999))
            .ReturnsAsync((Valsection?)null);

        // Act
        var result = await _controller.GetValSectionByGroupId(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetValSectionByGroupId_CallsServiceWithCorrectId()
    {
        // Arrange
        _mockValSectionService
            .Setup(s => s.GetValSectionByGroupIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Valsection?)null);

        // Act
        await _controller.GetValSectionByGroupId(42);

        // Assert
        _mockValSectionService.Verify(s => s.GetValSectionByGroupIdAsync(42), Times.Once);
    }

    [Fact]
    public async Task GetValSectionByGroupId_WhenNotFound_ReturnsAppropriateMessage()
    {
        // Arrange
        _mockValSectionService
            .Setup(s => s.GetValSectionByGroupIdAsync(999))
            .ReturnsAsync((Valsection?)null);

        // Act
        var result = await _controller.GetValSectionByGroupId(999);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().BeEquivalentTo(new { message = "VAL section with Group ID 999 not found." });
    }

    [Fact]
    public async Task GetValSectionsByGroupId_WhenValSectionsExist_ReturnsOkWithValSections()
    {
        // Arrange
        var valSections = new List<Valsection>
        {
            new Valsection
            {
                GroupId = 1,
                SectionText = "Section 1A",
                DisplayOrder = 1
            },
            new Valsection
            {
                GroupId = 1,
                SectionText = "Section 1B",
                DisplayOrder = 2
            }
        };

        _mockValSectionService
            .Setup(s => s.GetValSectionsByGroupIdAsync(1))
            .ReturnsAsync(valSections);

        // Act
        var result = await _controller.GetValSectionsByGroupId(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValSections = okResult.Value.Should().BeAssignableTo<IEnumerable<Valsection>>().Subject;
        returnedValSections.Should().HaveCount(2);
        returnedValSections.Should().BeEquivalentTo(valSections);
    }

    [Fact]
    public async Task GetValSectionsByGroupId_WhenNoValSections_ReturnsOkWithEmptyList()
    {
        // Arrange
        var emptyList = new List<Valsection>();
        _mockValSectionService
            .Setup(s => s.GetValSectionsByGroupIdAsync(999))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetValSectionsByGroupId(999);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValSections = okResult.Value.Should().BeAssignableTo<IEnumerable<Valsection>>().Subject;
        returnedValSections.Should().BeEmpty();
    }

    [Fact]
    public async Task GetValSectionsByGroupId_CallsServiceWithCorrectId()
    {
        // Arrange
        _mockValSectionService
            .Setup(s => s.GetValSectionsByGroupIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Valsection>());

        // Act
        await _controller.GetValSectionsByGroupId(42);

        // Assert
        _mockValSectionService.Verify(s => s.GetValSectionsByGroupIdAsync(42), Times.Once);
    }

    [Fact]
    public async Task GetValSectionsByGroupId_ReturnsAllValSectionsForSpecificGroup()
    {
        // Arrange
        var valSections = new List<Valsection>
        {
            new Valsection { GroupId = 1, SectionText = "Section 1A" },
            new Valsection { GroupId = 1, SectionText = "Section 1B" },
            new Valsection { GroupId = 1, SectionText = "Section 1C" }
        };

        _mockValSectionService
            .Setup(s => s.GetValSectionsByGroupIdAsync(1))
            .ReturnsAsync(valSections);

        // Act
        var result = await _controller.GetValSectionsByGroupId(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValSections = okResult.Value.Should().BeAssignableTo<IEnumerable<Valsection>>().Subject;
        returnedValSections.Should().HaveCount(3);
        returnedValSections.Should().AllSatisfy(v => v.GroupId.Should().Be(1));
    }
}
