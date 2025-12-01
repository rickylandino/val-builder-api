using Microsoft.AspNetCore.Mvc;
using val_builder_api.Models;
using val_builder_api.Services;

namespace val_builder_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    /// <summary>
    /// Get all companies
    /// </summary>
    /// <returns>List of all companies</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Company>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Company>>> GetAllCompanies()
    {
        var companies = await _companyService.GetAllCompaniesAsync();
        return Ok(companies);
    }

    /// <summary>
    /// Get a specific company by ID
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <returns>Company details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Company>> GetCompanyById(int id)
    {
        var company = await _companyService.GetCompanyByIdAsync(id);
        
        if (company == null)
        {
            return NotFound(new { message = $"Company with ID {id} not found." });
        }
        
        return Ok(company);
    }

    /// <summary>
    /// Create a new company
    /// </summary>
    /// <param name="company">Company data</param>
    /// <returns>Created company</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Company), StatusCodes.Status201Created)]
    public async Task<ActionResult<Company>> CreateCompany([FromBody] Company company)
    {
        var created = await _companyService.CreateCompanyAsync(company);
        return CreatedAtAction(nameof(GetCompanyById), new { id = created.CompanyId }, created);
    }

    /// <summary>
    /// Update an existing company
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <param name="company">Updated company data</param>
    /// <returns>Updated company</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Company>> UpdateCompany(int id, [FromBody] Company company)
    {
        var updated = await _companyService.UpdateCompanyAsync(id, company);
        if (updated == null)
        {
            return NotFound(new { message = $"Company with ID {id} not found." });
        }
        return Ok(updated);
    }
}
