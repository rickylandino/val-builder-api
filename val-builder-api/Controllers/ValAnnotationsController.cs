using Microsoft.AspNetCore.Mvc;
using val_builder_api.Models;
using val_builder_api.Services;

[ApiController]
[Route("api/valannotations")]
public class ValAnnotationsController : ControllerBase
{
    private readonly IValAnnotationService _service;

    public ValAnnotationsController(IValAnnotationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Valannotation>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Valannotation>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Valannotation>> Add([FromBody] Valannotation annotation)
    {
        var created = await _service.AddAsync(annotation);
        return CreatedAtAction(nameof(GetById), new { id = created.AnnotationId }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Valannotation>> Update(int id, [FromBody] Valannotation annotation)
    {
        var updated = await _service.UpdateAsync(id, annotation);
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
