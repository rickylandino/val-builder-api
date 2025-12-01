using Microsoft.AspNetCore.Mvc;
using val_builder_api.Models;
using val_builder_api.Services;

namespace val_builder_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyPlanController : ControllerBase
{
    private readonly ICompanyPlanService _service;

    public CompanyPlanController(ICompanyPlanService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all company plans, or filter by companyId
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyPlan>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CompanyPlan>>> GetAllCompanyPlans([FromQuery] int? companyId)
    {
        if (companyId.HasValue)
        {
            var filtered = await _service.GetCompanyPlansByCompanyIdAsync(companyId.Value);
            return Ok(filtered);
        }
        var plans = await _service.GetAllCompanyPlansAsync();
        return Ok(plans);
    }

    /// <summary>
    /// Get a specific company plan by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CompanyPlan), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyPlan>> GetCompanyPlanById(int id)
    {
        var plan = await _service.GetCompanyPlanByIdAsync(id);
        if (plan == null)
        {
            return NotFound(new { message = $"CompanyPlan with ID {id} not found." });
        }
        return Ok(plan);
    }

    /// <summary>
    /// Create a new company plan
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyPlan), StatusCodes.Status201Created)]
    public async Task<ActionResult<CompanyPlan>> CreateCompanyPlan([FromBody] CompanyPlan plan)
    {
        var created = await _service.CreateCompanyPlanAsync(plan);
        return CreatedAtAction(nameof(GetCompanyPlanById), new { id = created.PlanId }, created);
    }

    /// <summary>
    /// Update an existing company plan
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CompanyPlan), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyPlan>> UpdateCompanyPlan(int id, [FromBody] CompanyPlan plan)
    {
        var updated = await _service.UpdateCompanyPlanAsync(id, plan);
        if (updated == null)
        {
            return NotFound(new { message = $"CompanyPlan with ID {id} not found." });
        }
        return Ok(updated);
    }

    /// <summary>
    /// Get all plans for a specific company
    /// </summary>
    [HttpGet("company/{companyId}")]
    [ProducesResponseType(typeof(IEnumerable<CompanyPlan>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CompanyPlan>>> GetCompanyPlansByCompanyId(int companyId)
    {
        var plans = await _service.GetCompanyPlansByCompanyIdAsync(companyId);
        return Ok(plans);
    }
}
