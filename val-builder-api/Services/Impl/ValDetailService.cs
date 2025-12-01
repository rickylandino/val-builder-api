using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using val_builder_api.Data;
using val_builder_api.Models;

namespace val_builder_api.Services.Impl;

public class ValDetailService : IValDetailService
{
    private readonly ApplicationDbContext _context;

    public ValDetailService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Valdetail>> GetValDetailsAsync(int valId, int? groupId = null)
    {
        var query = _context.Valdetails
            .Where(d => d.ValId == valId);

        if (groupId.HasValue)
        {
            query = query.Where(d => d.GroupId == groupId.Value);
        }

        return await query
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Valdetail?> GetValDetailByIdAsync(Guid id)
    {
        return await _context.Valdetails
            .FirstOrDefaultAsync(d => d.ValDetailsId == id);
    }

    public async Task<Valdetail> CreateValDetailAsync(Valdetail detail)
    {
        // Generate new GUID if not provided
        if (detail.ValDetailsId == Guid.Empty)
        {
            detail.ValDetailsId = Guid.NewGuid();
        }

        _context.Valdetails.Add(detail);
        await _context.SaveChangesAsync();
        return detail;
    }

    public async Task<Valdetail?> UpdateValDetailAsync(Guid id, Valdetail detail)
    {
        var existing = await _context.Valdetails
            .FirstOrDefaultAsync(d => d.ValDetailsId == id);

        if (existing == null)
        {
            return null;
        }

        existing.ValId = detail.ValId;
        existing.GroupId = detail.GroupId;
        existing.GroupContent = detail.GroupContent;
        existing.DisplayOrder = detail.DisplayOrder;
        existing.Bullet = detail.Bullet;
        existing.Indent = detail.Indent;
        existing.Bold = detail.Bold;
        existing.Center = detail.Center;
        existing.BlankLineAfter = detail.BlankLineAfter;
        existing.TightLineHeight = detail.TightLineHeight;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteValDetailAsync(Guid id)
    {
        var detail = await _context.Valdetails
            .FirstOrDefaultAsync(d => d.ValDetailsId == id);

        if (detail == null)
        {
            return false;
        }

        _context.Valdetails.Remove(detail);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ValDetailSaveResult> SaveValDetailChangesAsync(ValDetailChangeDto dto)
    {
        var result = new ValDetailSaveResult { Success = false };
        var errors = new List<string>();

        // Only use transactions if supported
        var providerName = _context.Database.ProviderName ?? string.Empty;
        var useTransaction = !providerName.Equals("Microsoft.EntityFrameworkCore.InMemory", StringComparison.OrdinalIgnoreCase);
        IDbContextTransaction? transaction = null;
        if (useTransaction)
        {
            transaction = await _context.Database.BeginTransactionAsync();
        }

        try
        {
            foreach (var change in dto.Changes)
            {
                if (change.Detail == null)
                {
                    errors.Add($"Detail object is null for action: {change.Action}");
                    continue;
                }

                switch (change.Action.ToLower())
                {
                    case "create":
                        if (change.Detail.ValDetailsId == Guid.Empty)
                        {
                            change.Detail.ValDetailsId = Guid.NewGuid();
                        }
                        _context.Valdetails.Add(change.Detail);
                        result.ItemsCreated++;
                        break;

                    case "update":
                        var existing = await _context.Valdetails
                            .FirstOrDefaultAsync(d => d.ValDetailsId == change.Detail.ValDetailsId);

                        if (existing == null)
                        {
                            errors.Add($"ValDetail with ID {change.Detail.ValDetailsId} not found for update.");
                            continue;
                        }

                        existing.ValId = change.Detail.ValId;
                        existing.GroupId = change.Detail.GroupId;
                        existing.GroupContent = change.Detail.GroupContent;
                        existing.DisplayOrder = change.Detail.DisplayOrder;
                        existing.Bullet = change.Detail.Bullet;
                        existing.Indent = change.Detail.Indent;
                        existing.Bold = change.Detail.Bold;
                        existing.Center = change.Detail.Center;
                        existing.BlankLineAfter = change.Detail.BlankLineAfter;
                        existing.TightLineHeight = change.Detail.TightLineHeight;

                        result.ItemsUpdated++;
                        break;

                    case "delete":
                        var toDelete = await _context.Valdetails
                            .FirstOrDefaultAsync(d => d.ValDetailsId == change.Detail.ValDetailsId);

                        if (toDelete == null)
                        {
                            errors.Add($"ValDetail with ID {change.Detail.ValDetailsId} not found for delete.");
                            continue;
                        }

                        _context.Valdetails.Remove(toDelete);
                        result.ItemsDeleted++;
                        break;

                    default:
                        errors.Add($"Unknown action: {change.Action}");
                        break;
                }
            }

            if (errors.Count > 0)
            {
                result.Success = false;
                result.Errors = errors;
                result.Message = $"Completed with {errors.Count} error(s).";
                if (transaction != null)
                    await transaction.RollbackAsync();
                return result;
            }

            await _context.SaveChangesAsync();
            if (transaction != null)
                await transaction.CommitAsync();

            result.Success = true;
            result.Message = $"Successfully processed {result.ItemsCreated} creates, {result.ItemsUpdated} updates, and {result.ItemsDeleted} deletes.";
            return result;
        }
        catch (Exception ex)
        {
            if (transaction != null)
                await transaction.RollbackAsync();
            result.Success = false;
            result.Error = ex.Message;
            result.Message = "An error occurred while saving changes.";
            return result;
        }
    }
}
