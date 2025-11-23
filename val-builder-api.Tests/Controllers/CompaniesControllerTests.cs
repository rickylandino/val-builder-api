using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using val_builder_api.Controllers;
using val_builder_api.Models;
using val_builder_api.Services;
using Xunit;

namespace val_builder_api.Tests.Controllers;

public class CompaniesControllerTests
{
    private readonly Mock<ICompanyService> _mockCompanyService;
    private readonly CompaniesController _controller;

    public CompaniesControllerTests()
    {
        _mockCompanyService = new Mock<ICompanyService>();
        _controller = new CompaniesController(_mockCompanyService.Object);
    }

    [Fact]
    public async Task GetAllCompanies_WhenCompaniesExist_ReturnsOkWithCompanies()
    {
        // Arrange
        var companies = new List<Company>
        {
            new Company
            {
                CompanyId = 1,
                Name = "Acme Corp",
                MailingName = "Acme Corporation",
                City = "New York",
                State = "NY"
            },
            new Company
            {
                CompanyId = 2,
                Name = "Tech Solutions",
                MailingName = "Tech Solutions Inc",
                City = "San Francisco",
                State = "CA"
            }
        };

        _mockCompanyService
            .Setup(s => s.GetAllCompaniesAsync())
            .ReturnsAsync(companies);

        // Act
        var result = await _controller.GetAllCompanies();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCompanies = okResult.Value.Should().BeAssignableTo<IEnumerable<Company>>().Subject;
        returnedCompanies.Should().HaveCount(2);
        returnedCompanies.Should().BeEquivalentTo(companies);
    }

    [Fact]
    public async Task GetAllCompanies_WhenNoCompanies_ReturnsOkWithEmptyList()
    {
        // Arrange
        var emptyList = new List<Company>();
        _mockCompanyService
            .Setup(s => s.GetAllCompaniesAsync())
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetAllCompanies();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCompanies = okResult.Value.Should().BeAssignableTo<IEnumerable<Company>>().Subject;
        returnedCompanies.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCompanies_CallsServiceOnce()
    {
        // Arrange
        _mockCompanyService
            .Setup(s => s.GetAllCompaniesAsync())
            .ReturnsAsync(new List<Company>());

        // Act
        await _controller.GetAllCompanies();

        // Assert
        _mockCompanyService.Verify(s => s.GetAllCompaniesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCompanyById_WhenCompanyExists_ReturnsOkWithCompany()
    {
        // Arrange
        var company = new Company
        {
            CompanyId = 1,
            Name = "Test Company",
            MailingName = "Test Company Inc",
            Street1 = "123 Test St",
            City = "Test City",
            State = "TC",
            Zip = "12345",
            Phone = "555-0000"
        };

        _mockCompanyService
            .Setup(s => s.GetCompanyByIdAsync(1))
            .ReturnsAsync(company);

        // Act
        var result = await _controller.GetCompanyById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCompany = okResult.Value.Should().BeAssignableTo<Company>().Subject;
        returnedCompany.Should().BeEquivalentTo(company);
    }

    [Fact]
    public async Task GetCompanyById_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockCompanyService
            .Setup(s => s.GetCompanyByIdAsync(999))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _controller.GetCompanyById(999);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCompanyById_WhenCompanyDoesNotExist_ReturnsNotFoundWithMessage()
    {
        // Arrange
        int nonExistentId = 999;
        _mockCompanyService
            .Setup(s => s.GetCompanyByIdAsync(nonExistentId))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _controller.GetCompanyById(nonExistentId);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var responseValue = notFoundResult.Value.Should().BeAssignableTo<object>().Subject;
        
        // Check if message property exists
        var messageProperty = responseValue.GetType().GetProperty("message");
        messageProperty.Should().NotBeNull();
        messageProperty!.GetValue(responseValue).Should().Be($"Company with ID {nonExistentId} not found.");
    }

    [Fact]
    public async Task GetCompanyById_CallsServiceOnceWithCorrectId()
    {
        // Arrange
        int companyId = 5;
        _mockCompanyService
            .Setup(s => s.GetCompanyByIdAsync(companyId))
            .ReturnsAsync((Company?)null);

        // Act
        await _controller.GetCompanyById(companyId);

        // Assert
        _mockCompanyService.Verify(s => s.GetCompanyByIdAsync(companyId), Times.Once);
    }

    [Fact]
    public async Task GetCompanyById_WithZeroId_CallsService()
    {
        // Arrange
        _mockCompanyService
            .Setup(s => s.GetCompanyByIdAsync(0))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _controller.GetCompanyById(0);

        // Assert
        _mockCompanyService.Verify(s => s.GetCompanyByIdAsync(0), Times.Once);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCompanyById_WithNegativeId_CallsService()
    {
        // Arrange
        _mockCompanyService
            .Setup(s => s.GetCompanyByIdAsync(-1))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _controller.GetCompanyById(-1);

        // Assert
        _mockCompanyService.Verify(s => s.GetCompanyByIdAsync(-1), Times.Once);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Controller_HasApiControllerAttribute()
    {
        // Assert
        var attribute = _controller.GetType()
            .GetCustomAttributes(typeof(ApiControllerAttribute), false)
            .FirstOrDefault();
        
        attribute.Should().NotBeNull();
    }

    [Fact]
    public async Task Controller_HasCorrectRoute()
    {
        // Assert
        var attribute = _controller.GetType()
            .GetCustomAttributes(typeof(RouteAttribute), false)
            .FirstOrDefault() as RouteAttribute;
        
        attribute.Should().NotBeNull();
        attribute!.Template.Should().Be("api/[controller]");
    }
}
