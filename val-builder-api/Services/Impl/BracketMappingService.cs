using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;

namespace val_builder_api.Services.Impl;

public class BracketMappingService : IBracketMappingService
{
    private readonly ApplicationDbContext _context;

    public BracketMappingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BracketMapping>> GetAllAsync()
        => await _context.BracketMappings.AsNoTracking().ToListAsync();

    public async Task<BracketMapping?> GetByIdAsync(int id)
        => await _context.BracketMappings.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    public async Task<BracketMapping> CreateAsync(BracketMapping mapping)
    {
        _context.BracketMappings.Add(mapping);
        await _context.SaveChangesAsync();
        return mapping;
    }

    public async Task<BracketMapping?> UpdateAsync(int id, BracketMapping mapping)
    {
        var existing = await _context.BracketMappings.FindAsync(id);
        if (existing == null) return null;
        if (existing.SystemTag) return null; // Prevent update if systemTag
        existing.TagName = mapping.TagName;
        existing.ObjectPath = mapping.ObjectPath;
        existing.Description = mapping.Description;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var mapping = await _context.BracketMappings.FindAsync(id);
        if (mapping == null || mapping.SystemTag) return false; // Prevent delete if systemTag
        _context.BracketMappings.Remove(mapping);
        await _context.SaveChangesAsync();
        return true;
    }
}
