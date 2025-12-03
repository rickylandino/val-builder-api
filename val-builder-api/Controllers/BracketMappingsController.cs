using Microsoft.AspNetCore.Mvc;
using val_builder_api.Models;
using val_builder_api.Services;

namespace val_builder_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BracketMappingsController : ControllerBase
{
    private readonly IBracketMappingService _service;

    public BracketMappingsController(IBracketMappingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BracketMapping>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult<BracketMapping>> Create([FromBody] BracketMapping mapping)
    {
        var created = await _service.CreateAsync(mapping);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BracketMapping>> Update(int id, [FromBody] BracketMapping mapping)
    {
        var updated = await _service.UpdateAsync(id, mapping);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
