using Microsoft.AspNetCore.Mvc;
using val_builder_api.Dto;
using val_builder_api.Models;
using val_builder_api.Services;

namespace val_builder_api.Controllers;

[ApiController]
[Route("api/val/{valId}/details")]
public class ValDetailsController : ControllerBase
{
    private readonly IValDetailService _service;

    public ValDetailsController(IValDetailService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all ValDetails for a valId, optionally filtered by groupId, ordered by DisplayOrder
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Valdetail>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Valdetail>>> GetValDetails(int valId, [FromQuery] int? groupId)
    {
        var details = await _service.GetValDetailsAsync(valId, groupId);
        return Ok(details);
    }

    /// <summary>
    /// Get a specific ValDetail by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Valdetail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Valdetail>> GetValDetailById(Guid id)
    {
        var detail = await _service.GetValDetailByIdAsync(id);
        if (detail == null)
        {
            return NotFound(new { message = $"ValDetail with ID {id} not found." });
        }
        return Ok(detail);
    }

    /// <summary>
    /// Create a new ValDetail
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Valdetail), StatusCodes.Status201Created)]
    public async Task<ActionResult<Valdetail>> CreateValDetail([FromBody] Valdetail detail)
    {
        var created = await _service.CreateValDetailAsync(detail);
        return CreatedAtAction(nameof(GetValDetailById), new { id = created.ValDetailsId }, created);
    }

    /// <summary>
    /// Update an existing ValDetail
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Valdetail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Valdetail>> UpdateValDetail(Guid id, [FromBody] Valdetail detail)
    {
        var updated = await _service.UpdateValDetailAsync(id, detail);
        if (updated == null)
        {
            return NotFound(new { message = $"ValDetail with ID {id} not found." });
        }
        return Ok(updated);
    }

    /// <summary>
    /// Delete a ValDetail
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteValDetail(Guid id)
    {
        var deleted = await _service.DeleteValDetailAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = $"ValDetail with ID {id} not found." });
        }
        return NoContent();
    }

    /// <summary>
    /// Save all changes (creates, updates, deletes) for ValDetails in a single transaction
    /// </summary>
    [HttpPost("save-changes")]
    [ProducesResponseType(typeof(ValDetailSaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ValDetailSaveResult>> SaveValDetailChanges(int valId, [FromBody] ValDetailChangeDto dto)
    {
        if (dto == null || dto.ValId != valId || dto.Changes == null)
        {
            return BadRequest(new ValDetailSaveResult
            {
                Success = false,
                Message = "Invalid valId or changes array",
                Errors = new List<string> { "Request body is invalid or valId mismatch" }
            });
        }

        var result = await _service.SaveValDetailChangesAsync(dto);

        if (!result.Success && result.Error != null)
        {
            return StatusCode(500, result);
        }

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
