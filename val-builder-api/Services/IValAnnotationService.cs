using val_builder_api.Models;

namespace val_builder_api.Services;

public interface IValAnnotationService
{
    Task<IEnumerable<Valannotation>> GetAllAsync();
    Task<Valannotation?> GetByIdAsync(int id);
    Task<Valannotation> AddAsync(Valannotation annotation);
    Task<Valannotation?> UpdateAsync(int id, Valannotation annotation);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Valannotation>> GetByValIdAsync(int valId);
}
