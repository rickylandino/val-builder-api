using val_builder_api.Models;

namespace val_builder_api.Services;

public interface IBracketMappingService
{
    Task<IEnumerable<BracketMapping>> GetAllAsync();
    Task<BracketMapping?> GetByIdAsync(int id);
    Task<BracketMapping> CreateAsync(BracketMapping mapping);
    Task<BracketMapping?> UpdateAsync(int id, BracketMapping mapping);
    Task<bool> DeleteAsync(int id);
}
