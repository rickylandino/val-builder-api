using Microsoft.AspNetCore.Mvc;
using val_builder_api.Models;
using val_builder_api.Services;

namespace val_builder_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValHeaderController : ControllerBase
{
    private readonly IValHeaderService _valHeaderService;

    public ValHeaderController(IValHeaderService valHeaderService)
    {
        _valHeaderService = valHeaderService;
    }

    /// <summary>
    /// Get all ValHeaders, or filter by companyId
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Valheader>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Valheader>>> GetAllValHeaders([FromQuery] int? companyId, [FromQuery] int? planId)
    {
        if (companyId.HasValue)
        {
            var filtered = await _valHeaderService.GetValHeadersByCompanyIdAsync(companyId.Value);
            return Ok(filtered);
        } else if (planId.HasValue)
        {
            var filtered = await _valHeaderService.GetValHeadersByPlanIdAsync(planId.Value);
            return Ok(filtered);
        }
        
        var headers = await _valHeaderService.GetAllValHeadersAsync();
        return Ok(headers);
    }

    /// <summary>
    /// Get a specific ValHeader by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Valheader), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Valheader>> GetValHeaderById(int id)
    {
        var header = await _valHeaderService.GetValHeaderByIdAsync(id);
        if (header == null)
        {
            return NotFound(new { message = $"ValHeader with ID {id} not found." });
        }
        return Ok(header);
    }

    /// <summary>
    /// Create a new ValHeader
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Valheader), StatusCodes.Status201Created)]
    public async Task<ActionResult<Valheader>> CreateValHeader([FromBody] Valheader valHeader)
    {
        var created = await _valHeaderService.CreateValHeaderAsync(valHeader);
        return CreatedAtAction(nameof(GetValHeaderById), new { id = created.ValId }, created);
    }

    /// <summary>
    /// Update an existing ValHeader
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Valheader), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Valheader>> UpdateValHeader(int id, [FromBody] Valheader valHeader)
    {
        var updated = await _valHeaderService.UpdateValHeaderAsync(id, valHeader);
        if (updated == null)
        {
            return NotFound(new { message = $"ValHeader with ID {id} not found." });
        }
        return Ok(updated);
    }
}
