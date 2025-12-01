using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;

namespace val_builder_api.Services.Impl;

public class ValSectionService : IValSectionService
{
    private readonly ApplicationDbContext _context;

    public ValSectionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Valsection>> GetAllValSectionsAsync()
    {
        return await _context.Valsections
            .AsNoTracking()
            .OrderBy(v => v.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Valsection?> GetValSectionByGroupIdAsync(int groupId)
    {
        return await _context.Valsections
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.GroupId == groupId);
    }

    public async Task<IEnumerable<Valsection>> GetValSectionsByGroupIdAsync(int groupId)
    {
        return await _context.Valsections
            .AsNoTracking()
            .Where(v => v.GroupId == groupId)
            .OrderBy(v => v.DisplayOrder)
            .ToListAsync();
    }
}
