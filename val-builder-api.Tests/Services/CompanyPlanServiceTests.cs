using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Services;

public class CompanyPlanServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CompanyPlanService _service;

    public CompanyPlanServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _service = new CompanyPlanService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllCompanyPlansAsync_WhenNone_ReturnsEmpty()
    {
        var result = await _service.GetAllCompanyPlansAsync();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateCompanyPlanAsync_AddsPlan()
    {
        var plan = new CompanyPlan { CompanyId = 1, PlanType = "401k", PlanName = "Test" };
        var created = await _service.CreateCompanyPlanAsync(plan);
        created.PlanName.Should().Be("Test");
        (await _service.GetAllCompanyPlansAsync()).Should().ContainSingle();
    }

    [Fact]
    public async Task GetCompanyPlanByIdAsync_ReturnsCorrectPlan()
    {
        var plan = new CompanyPlan { CompanyId = 1, PlanType = "401k", PlanName = "Test" };
        await _service.CreateCompanyPlanAsync(plan);
        var found = await _service.GetCompanyPlanByIdAsync(plan.PlanId);
        found.Should().NotBeNull();
        found!.PlanName.Should().Be("Test");
    }

    [Fact]
    public async Task UpdateCompanyPlanAsync_UpdatesPlan()
    {
        var plan = new CompanyPlan { CompanyId = 1, PlanType = "401k", PlanName = "Original" };
        await _service.CreateCompanyPlanAsync(plan);
        plan.PlanName = "Updated";
        var updated = await _service.UpdateCompanyPlanAsync(plan.PlanId, plan);
        updated.Should().NotBeNull();
        updated!.PlanName.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateCompanyPlanAsync_WhenNotFound_ReturnsNull()
    {
        var plan = new CompanyPlan { CompanyId = 1, PlanType = "401k", PlanName = "Doesn't exist" };
        var updated = await _service.UpdateCompanyPlanAsync(999, plan);
        updated.Should().BeNull();
    }

    [Fact]
    public async Task GetCompanyPlansByCompanyIdAsync_ReturnsFiltered()
    {
        var plan1 = new CompanyPlan { CompanyId = 1, PlanType = "401k", PlanName = "A" };
        var plan2 = new CompanyPlan { CompanyId = 2, PlanType = "401k", PlanName = "B" };
        await _service.CreateCompanyPlanAsync(plan1);
        await _service.CreateCompanyPlanAsync(plan2);
        var filtered = await _service.GetCompanyPlansByCompanyIdAsync(1);
        filtered.Should().ContainSingle();
        filtered.First().PlanName.Should().Be("A");
    }
}
