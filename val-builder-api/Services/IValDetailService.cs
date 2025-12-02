using val_builder_api.Dto;
using val_builder_api.Models;

namespace val_builder_api.Services;

public interface IValDetailService
{
    Task<IEnumerable<Valdetail>> GetValDetailsAsync(int valId, int? groupId = null);
    Task<Valdetail?> GetValDetailByIdAsync(Guid id);
    Task<Valdetail> CreateValDetailAsync(Valdetail detail);
    Task<Valdetail?> UpdateValDetailAsync(Guid id, Valdetail detail);
    Task<bool> DeleteValDetailAsync(Guid id);
    Task<ValDetailSaveResult> SaveValDetailChangesAsync(ValDetailChangeDto dto);
}
