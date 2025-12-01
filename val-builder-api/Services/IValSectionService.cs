using val_builder_api.Models;

namespace val_builder_api.Services;

public interface IValSectionService
{
    Task<IEnumerable<Valsection>> GetAllValSectionsAsync();
    Task<Valsection?> GetValSectionByGroupIdAsync(int groupId);
    Task<IEnumerable<Valsection>> GetValSectionsByGroupIdAsync(int groupId);
}
