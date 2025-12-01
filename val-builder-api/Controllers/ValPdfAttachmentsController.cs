using Microsoft.AspNetCore.Mvc;
using val_builder_api.Models;
using val_builder_api.Services;

namespace val_builder_api.Controllers;

[ApiController]
[Route("api/valpdfattachments")]
public class ValPdfAttachmentsController : ControllerBase
{
    private readonly IValPdfAttachmentService _service;

    public ValPdfAttachmentsController(IValPdfAttachmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ValPdfAttachment>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ValPdfAttachment>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpGet("by-val/{valId}")]
    public async Task<ActionResult<IEnumerable<ValPdfAttachment>>> GetByValId(int valId)
        => Ok(await _service.GetByValIdAsync(valId));

    [HttpPost]
    public async Task<ActionResult<ValPdfAttachment>> Add([FromBody] ValPdfAttachment attachment)
    {
        var created = await _service.AddAsync(attachment);
        return CreatedAtAction(nameof(GetById), new { id = created.PDFId }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ValPdfAttachment>> Update(int id, [FromBody] ValPdfAttachment attachment)
    {
        var updated = await _service.UpdateAsync(id, attachment);
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
