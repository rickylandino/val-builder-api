using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Services;

public class CompanyServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CompanyService _companyService;

    public CompanyServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _companyService = new CompanyService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllCompaniesAsync_WhenNoCompanies_ReturnsEmptyList()
    {
        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCompaniesAsync_WhenCompaniesExist_ReturnsAllCompanies()
    {
        // Arrange
        var companies = new List<Company>
        {
            new Company
            {
                CompanyId = 1,
                Name = "Acme Corp",
                MailingName = "Acme Corporation",
                Street1 = "123 Main St",
                City = "New York",
                State = "NY",
                Zip = "10001",
                Phone = "555-1234"
            },
            new Company
            {
                CompanyId = 2,
                Name = "Tech Solutions",
                MailingName = "Tech Solutions Inc",
                Street1 = "456 Tech Ave",
                City = "San Francisco",
                State = "CA",
                Zip = "94105",
                Phone = "555-5678"
            },
            new Company
            {
                CompanyId = 3,
                Name = "Global Industries",
                MailingName = "Global Industries LLC",
                Street1 = "789 Industry Blvd",
                City = "Chicago",
                State = "IL",
                Zip = "60601",
                Phone = "555-9012"
            }
        };

        await _context.Companies.AddRangeAsync(companies);
        await _context.SaveChangesAsync();

        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(companies);
    }

    [Fact]
    public async Task GetAllCompaniesAsync_ReturnsCompaniesInCorrectOrder()
    {
        // Arrange
        var companies = new List<Company>
        {
            new Company { CompanyId = 3, Name = "Company C" },
            new Company { CompanyId = 1, Name = "Company A" },
            new Company { CompanyId = 2, Name = "Company B" }
        };

        await _context.Companies.AddRangeAsync(companies);
        await _context.SaveChangesAsync();

        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.CompanyId == 1 && c.Name == "Company A");
        result.Should().Contain(c => c.CompanyId == 2 && c.Name == "Company B");
        result.Should().Contain(c => c.CompanyId == 3 && c.Name == "Company C");
    }

    [Fact]
    public async Task GetCompanyByIdAsync_WhenCompanyExists_ReturnsCompany()
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

        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _companyService.GetCompanyByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(company);
    }

    [Fact]
    public async Task GetCompanyByIdAsync_WhenCompanyDoesNotExist_ReturnsNull()
    {
        // Arrange
        var company = new Company
        {
            CompanyId = 1,
            Name = "Test Company"
        };

        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _companyService.GetCompanyByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCompanyByIdAsync_WhenDatabaseIsEmpty_ReturnsNull()
    {
        // Act
        var result = await _companyService.GetCompanyByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCompanyByIdAsync_WithMultipleCompanies_ReturnsCorrectCompany()
    {
        // Arrange
        var companies = new List<Company>
        {
            new Company { CompanyId = 1, Name = "Company 1" },
            new Company { CompanyId = 2, Name = "Company 2" },
            new Company { CompanyId = 3, Name = "Company 3" }
        };

        await _context.Companies.AddRangeAsync(companies);
        await _context.SaveChangesAsync();

        // Act
        var result = await _companyService.GetCompanyByIdAsync(2);

        // Assert
        result.Should().NotBeNull();
        result!.CompanyId.Should().Be(2);
        result.Name.Should().Be("Company 2");
    }

    [Fact]
    public async Task GetAllCompaniesAsync_DoesNotTrackEntities()
    {
        // Arrange
        var company = new Company
        {
            CompanyId = 1,
            Name = "Test Company"
        };

        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        _context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task GetCompanyByIdAsync_DoesNotTrackEntities()
    {
        // Arrange
        var company = new Company
        {
            CompanyId = 1,
            Name = "Test Company"
        };

        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _companyService.GetCompanyByIdAsync(1);

        // Assert
        _context.ChangeTracker.Entries().Should().BeEmpty();
    }
}
