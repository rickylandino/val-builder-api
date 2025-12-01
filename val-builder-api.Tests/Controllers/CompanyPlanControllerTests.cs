using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using val_builder_api.Controllers;
using val_builder_api.Models;
using val_builder_api.Services;
using Xunit;

namespace val_builder_api.Tests.Controllers;

public class CompanyPlanControllerTests
{
    private readonly Mock<ICompanyPlanService> _mockService;
    private readonly CompanyPlanController _controller;

    public CompanyPlanControllerTests()
    {
        _mockService = new Mock<ICompanyPlanService>();
        _controller = new CompanyPlanController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllCompanyPlans_ReturnsOkWithPlans()
    {
        var plans = new List<CompanyPlan> { new CompanyPlan { PlanId = 1, PlanName = "A" } };
        _mockService.Setup(s => s.GetAllCompanyPlansAsync()).ReturnsAsync(plans);
        var result = await _controller.GetAllCompanyPlans(null);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<IEnumerable<CompanyPlan>>().Subject;
        returned.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllCompanyPlans_WithCompanyId_ReturnsFiltered()
    {
        var plans = new List<CompanyPlan> { new CompanyPlan { PlanId = 1, CompanyId = 1, PlanName = "A" } };
        _mockService.Setup(s => s.GetCompanyPlansByCompanyIdAsync(1)).ReturnsAsync(plans);
        var result = await _controller.GetAllCompanyPlans(1);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<IEnumerable<CompanyPlan>>().Subject;
        returned.Should().HaveCount(1);
        returned.First().CompanyId.Should().Be(1);
    }

    [Fact]
    public async Task GetCompanyPlanById_WhenFound_ReturnsOk()
    {
        var plan = new CompanyPlan { PlanId = 1, PlanName = "A" };
        _mockService.Setup(s => s.GetCompanyPlanByIdAsync(1)).ReturnsAsync(plan);
        var result = await _controller.GetCompanyPlanById(1);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<CompanyPlan>().Subject;
        returned.PlanId.Should().Be(1);
    }

    [Fact]
    public async Task GetCompanyPlanById_WhenNotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetCompanyPlanByIdAsync(999)).ReturnsAsync((CompanyPlan?)null);
        var result = await _controller.GetCompanyPlanById(999);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateCompanyPlan_ReturnsCreated()
    {
        var plan = new CompanyPlan { PlanId = 1, PlanName = "A" };
        _mockService.Setup(s => s.CreateCompanyPlanAsync(plan)).ReturnsAsync(plan);
        var result = await _controller.CreateCompanyPlan(plan);
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returned = created.Value.Should().BeAssignableTo<CompanyPlan>().Subject;
        returned.PlanId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateCompanyPlan_WhenFound_ReturnsOk()
    {
        var plan = new CompanyPlan { PlanId = 1, PlanName = "A" };
        _mockService.Setup(s => s.UpdateCompanyPlanAsync(1, plan)).ReturnsAsync(plan);
        var result = await _controller.UpdateCompanyPlan(1, plan);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<CompanyPlan>().Subject;
        returned.PlanId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateCompanyPlan_WhenNotFound_ReturnsNotFound()
    {
        var plan = new CompanyPlan { PlanId = 1, PlanName = "A" };
        _mockService.Setup(s => s.UpdateCompanyPlanAsync(999, plan)).ReturnsAsync((CompanyPlan?)null);
        var result = await _controller.UpdateCompanyPlan(999, plan);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCompanyPlansByCompanyId_ReturnsFiltered()
    {
        var plans = new List<CompanyPlan> { new CompanyPlan { PlanId = 1, CompanyId = 1, PlanName = "A" } };
        _mockService.Setup(s => s.GetCompanyPlansByCompanyIdAsync(1)).ReturnsAsync(plans);
        var result = await _controller.GetCompanyPlansByCompanyId(1);
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<IEnumerable<CompanyPlan>>().Subject;
        returned.Should().HaveCount(1);
        returned.First().CompanyId.Should().Be(1);
    }
}
