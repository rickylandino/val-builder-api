using val_builder_api.Models;

namespace val_builder_api.Services;

public interface IValPdfAttachmentService
{
    Task<IEnumerable<ValPdfAttachment>> GetAllAsync();
    Task<ValPdfAttachment?> GetByIdAsync(int id);
    Task<IEnumerable<ValPdfAttachment>> GetByValIdAsync(int valId);
    Task<ValPdfAttachment> AddAsync(ValPdfAttachment attachment);
    Task<ValPdfAttachment?> UpdateAsync(int id, ValPdfAttachment attachment);
    Task<bool> DeleteAsync(int id);
}
