using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;

public class ValAnnotationService : IValAnnotationService
{
    private readonly ApplicationDbContext _context;

    public ValAnnotationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Valannotation>> GetAllAsync()
        => await _context.Valannotations.AsNoTracking().ToListAsync();

    public async Task<Valannotation?> GetByIdAsync(int id)
        => await _context.Valannotations.AsNoTracking().FirstOrDefaultAsync(a => a.AnnotationId == id);

    public async Task<Valannotation> AddAsync(Valannotation annotation)
    {
        _context.Valannotations.Add(annotation);
        await _context.SaveChangesAsync();
        return annotation;
    }

    public async Task<Valannotation?> UpdateAsync(int id, Valannotation annotation)
    {
        var existing = await _context.Valannotations.FindAsync(id);
        if (existing == null) return null;

        existing.ValId = annotation.ValId;
        existing.AnnotationContent = annotation.AnnotationContent;
        existing.AuthorId = annotation.AuthorId;
        existing.DateModified = annotation.DateModified;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var annotation = await _context.Valannotations.FindAsync(id);
        if (annotation == null) return false;
        _context.Valannotations.Remove(annotation);
        await _context.SaveChangesAsync();
        return true;
    }
}
