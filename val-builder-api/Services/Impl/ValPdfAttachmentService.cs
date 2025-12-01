using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;

namespace val_builder_api.Services.Impl;

public class ValPdfAttachmentService : IValPdfAttachmentService
{
    private readonly ApplicationDbContext _context;

    public ValPdfAttachmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ValPdfAttachment>> GetAllAsync()
    {
        return await _context.ValPdfAttachments
            .AsNoTracking()
            .OrderBy(a => a.ValID)
            .ThenBy(a => a.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ValPdfAttachment?> GetByIdAsync(int id)
    {
        return await _context.ValPdfAttachments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.PDFId == id);
    }

    public async Task<IEnumerable<ValPdfAttachment>> GetByValIdAsync(int valId)
    {
        return await _context.ValPdfAttachments
            .AsNoTracking()
            .Where(a => a.ValID == valId)
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ValPdfAttachment> AddAsync(ValPdfAttachment attachment)
    {
        _context.ValPdfAttachments.Add(attachment);
        await _context.SaveChangesAsync();
        return attachment;
    }

    public async Task<ValPdfAttachment?> UpdateAsync(int id, ValPdfAttachment attachment)
    {
        var existing = await _context.ValPdfAttachments.FindAsync(id);
        if (existing == null)
            return null;

        existing.ValID = attachment.ValID;
        existing.PDFName = attachment.PDFName;
        existing.DisplayOrder = attachment.DisplayOrder;
        existing.PDFContents = attachment.PDFContents;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var attachment = await _context.ValPdfAttachments.FindAsync(id);
        if (attachment == null)
            return false;

        _context.ValPdfAttachments.Remove(attachment);
        await _context.SaveChangesAsync();
        return true;
    }
}
