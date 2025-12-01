using Microsoft.AspNetCore.Mvc;
using val_builder_api.Models;
using val_builder_api.Services;

namespace val_builder_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValSectionsController : ControllerBase
{
    private readonly IValSectionService _valSectionService;

    public ValSectionsController(IValSectionService valSectionService)
    {
        _valSectionService = valSectionService;
    }

    /// <summary>
    /// Get all VAL sections
    /// </summary>
    /// <returns>List of all VAL sections</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Valsection>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Valsection>>> GetAllValSections()
    {
        var valSections = await _valSectionService.GetAllValSectionsAsync();
        return Ok(valSections);
    }

    /// <summary>
    /// Get a specific VAL section by Group ID
    /// </summary>
    /// <param name="groupId">Group ID</param>
    /// <returns>VAL section details</returns>
    [HttpGet("{groupId}")]
    [ProducesResponseType(typeof(Valsection), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Valsection>> GetValSectionByGroupId(int groupId)
    {
        var valSection = await _valSectionService.GetValSectionByGroupIdAsync(groupId);
        
        if (valSection == null)
        {
            return NotFound(new { message = $"VAL section with Group ID {groupId} not found." });
        }
        
        return Ok(valSection);
    }

    /// <summary>
    /// Get all VAL sections for a specific Group ID
    /// </summary>
    /// <param name="groupId">Group ID</param>
    /// <returns>List of VAL sections for the group</returns>
    [HttpGet("group/{groupId}")]
    [ProducesResponseType(typeof(IEnumerable<Valsection>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Valsection>>> GetValSectionsByGroupId(int groupId)
    {
        var valSections = await _valSectionService.GetValSectionsByGroupIdAsync(groupId);
        return Ok(valSections);
    }
}
