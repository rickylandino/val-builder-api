using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;

namespace val_builder_api.Services.Impl;

public class ValHeaderService : IValHeaderService
{
    private readonly ApplicationDbContext _context;

    public ValHeaderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Valheader>> GetAllValHeadersAsync()
    {
        return await _context.Valheaders
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Valheader?> GetValHeaderByIdAsync(int id)
    {
        return await _context.Valheaders
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.ValId == id);
    }

    public async Task<Valheader> CreateValHeaderAsync(Valheader valHeader)
    {
        _context.Valheaders.Add(valHeader);
        await _context.SaveChangesAsync();
        return valHeader;
    }

    public async Task<Valheader?> UpdateValHeaderAsync(int id, Valheader valHeader)
    {
        var existing = await _context.Valheaders.FindAsync(id);
        if (existing == null)
            return null;

        existing.PlanId = valHeader.PlanId;
        existing.ValDescription = valHeader.ValDescription;
        existing.ValDate = valHeader.ValDate;
        existing.PlanYearBeginDate = valHeader.PlanYearBeginDate;
        existing.PlanYearEndDate = valHeader.PlanYearEndDate;
        existing.RecipientName = valHeader.RecipientName;
        existing.RecipientAddress1 = valHeader.RecipientAddress1;
        existing.RecipientAddress2 = valHeader.RecipientAddress2;
        existing.RecipientCity = valHeader.RecipientCity;
        existing.RecipientState = valHeader.RecipientState;
        existing.RecipientZip = valHeader.RecipientZip;
        existing.FinalizeDate = valHeader.FinalizeDate;
        existing.FinalizedBy = valHeader.FinalizedBy;
        existing.WordDocPath = valHeader.WordDocPath;
        existing.ValstatusId = valHeader.ValstatusId;
        existing.MarginLeftRight = valHeader.MarginLeftRight;
        existing.MarginTopBottom = valHeader.MarginTopBottom;
        existing.FontSize = valHeader.FontSize;
        existing.ValYear = valHeader.ValYear;
        existing.ValQuarter = valHeader.ValQuarter;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<IEnumerable<Valheader>> GetValHeadersByCompanyIdAsync(int companyId)
    {
        return await _context.Valheaders
            .AsNoTracking()
            .Where(v => v.PlanId == companyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Valheader>> GetValHeadersByPlanIdAsync(int planId)
    {
        return await _context.Valheaders
            .AsNoTracking()
            .Where(v => v.PlanId == planId)
            .ToListAsync();
    }
}
