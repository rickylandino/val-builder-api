using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Dto;
using val_builder_api.Models;

namespace val_builder_api.Services.Impl;

public class ValTemplateItemService : IValTemplateItemService
{
    private readonly ApplicationDbContext _context;

    public ValTemplateItemService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ValtemplateItem>> GetValTemplateItemsByGroupIdAsync(int groupId)
    {
        return await _context.ValtemplateItems
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ValtemplateItem?> GetValTemplateItemByIdAsync(int id)
    {
        return await _context.ValtemplateItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ItemId == id);
    }

    public async Task<ValtemplateItem> CreateValTemplateItemAsync(ValtemplateItem item)
    {
        _context.ValtemplateItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<ValtemplateItem?> UpdateValTemplateItemAsync(int id, ValtemplateItem item)
    {
        var existing = await _context.ValtemplateItems.FindAsync(id);
        if (existing == null)
            return null;
        existing.GroupId = item.GroupId;
        existing.ItemText = item.ItemText;
        existing.DisplayOrder = item.DisplayOrder;
        existing.BlankLineAfter = item.BlankLineAfter;
        existing.Bold = item.Bold;
        existing.Bullet = item.Bullet;
        existing.Center = item.Center;
        existing.DefaultOnVal = item.DefaultOnVal;
        existing.Indent = item.Indent;
        existing.TightLineHeight = item.TightLineHeight;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task UpdateDisplayOrderBulkAsync(int groupId, List<ValTemplateItemDisplayOrderUpdateDto.ItemOrder> items)
    {
        // Example: update items in DB context (pseudo-code, adapt to your ORM)
        foreach (var item in items)
        {
            var entity = await _context.ValtemplateItems
                .FirstOrDefaultAsync(x => x.ItemId == item.ItemId && x.GroupId == groupId);

            if (entity != null)
            {
                entity.DisplayOrder = item.DisplayOrder;
            }
        }
        await _context.SaveChangesAsync();
    }
}
