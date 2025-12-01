using Microsoft.AspNetCore.Mvc;
using val_builder_api.Models;
using val_builder_api.Services;

namespace val_builder_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValTemplateItemsController : ControllerBase
{
    private readonly IValTemplateItemService _service;

    public ValTemplateItemsController(IValTemplateItemService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all ValTemplateItems by groupId, ordered by DisplayOrder
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ValtemplateItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ValtemplateItem>>> GetValTemplateItems([FromQuery] int groupId)
    {
        var items = await _service.GetValTemplateItemsByGroupIdAsync(groupId);
        return Ok(items);
    }

    /// <summary>
    /// Get a specific ValTemplateItem by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ValtemplateItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ValtemplateItem>> GetValTemplateItemById(int id)
    {
        var item = await _service.GetValTemplateItemByIdAsync(id);
        if (item == null)
        {
            return NotFound(new { message = $"ValTemplateItem with ID {id} not found." });
        }
        return Ok(item);
    }

    /// <summary>
    /// Create a new ValTemplateItem
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ValtemplateItem), StatusCodes.Status201Created)]
    public async Task<ActionResult<ValtemplateItem>> CreateValTemplateItem([FromBody] ValtemplateItem item)
    {
        var created = await _service.CreateValTemplateItemAsync(item);
        return CreatedAtAction(nameof(GetValTemplateItemById), new { id = created.ItemId }, created);
    }

    /// <summary>
    /// Update an existing ValTemplateItem
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ValtemplateItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ValtemplateItem>> UpdateValTemplateItem(int id, [FromBody] ValtemplateItem item)
    {
        var updated = await _service.UpdateValTemplateItemAsync(id, item);
        if (updated == null)
        {
            return NotFound(new { message = $"ValTemplateItem with ID {id} not found." });
        }
        return Ok(updated);
    }
}
