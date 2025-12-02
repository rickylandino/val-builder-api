using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using val_builder_api.Data;
using val_builder_api.Dto;
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

    protected static async Task HandleTransaction(IDbContextTransaction? transaction, string action)
    {
        switch(action)
        {
            case "commit":
                if (transaction != null)
                    await transaction.CommitAsync();
                break;
            case "rollback":
                if (transaction != null)
                    await transaction.RollbackAsync();
                break;
        }
    }

    public async Task<ValDetailSaveResult> SaveValDetailChangesAsync(ValDetailChangeDto dto)
    {
        var result = new ValDetailSaveResult { Success = false };
        var errors = new List<string>();

        var providerName = _context.Database.ProviderName ?? string.Empty;
        var useTransaction = !providerName.Equals("Microsoft.EntityFrameworkCore.InMemory", StringComparison.OrdinalIgnoreCase);
        IDbContextTransaction? transaction = null;
        if (useTransaction)
            transaction = await _context.Database.BeginTransactionAsync();

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
                        HandleCreate(change, result);
                        break;
                    case "update":
                        await HandleUpdateAsync(change, result, errors);
                        break;
                    case "delete":
                        await HandleDeleteAsync(change, result, errors);
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
                await HandleTransaction(transaction, "rollback");
                return result;
            }

            await _context.SaveChangesAsync();
            await HandleTransaction(transaction, "commit");

            result.Success = true;
            result.Message = $"Successfully processed {result.ItemsCreated} creates, {result.ItemsUpdated} updates, and {result.ItemsDeleted} deletes.";
            return result;
        }
        catch (Exception ex)
        {
            await HandleTransaction(transaction, "rollback");
            result.Success = false;
            result.Error = ex.Message;
            result.Message = "An error occurred while saving changes.";
            return result;
        }
    }

    private void HandleCreate(ValDetailChange change, ValDetailSaveResult result)
    {
        if (change.Detail != null)
        {
            if (change.Detail.ValDetailsId == Guid.Empty)
                change.Detail.ValDetailsId = Guid.NewGuid();
            _context.Valdetails.Add(change.Detail);
            result.ItemsCreated++;
        }
    }

    private async Task HandleUpdateAsync(ValDetailChange change, ValDetailSaveResult result, List<string> errors)
    {
        if (change.Detail != null)
        {
            var existing = await _context.Valdetails.FirstOrDefaultAsync(d => d.ValDetailsId == change.Detail.ValDetailsId);
            if (existing == null)
            {
                errors.Add($"ValDetail with ID {change.Detail.ValDetailsId} not found for update.");
                return;
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
        }
    }

    private async Task HandleDeleteAsync(ValDetailChange change, ValDetailSaveResult result, List<string> errors)
    {
        if (change.Detail != null)
        {
            var toDelete = await _context.Valdetails.FirstOrDefaultAsync(d => d.ValDetailsId == change.Detail.ValDetailsId);
            if (toDelete == null)
            {
                errors.Add($"ValDetail with ID {change.Detail.ValDetailsId} not found for delete.");
                return;
            }
            _context.Valdetails.Remove(toDelete);
            result.ItemsDeleted++;
        }
    }
}
