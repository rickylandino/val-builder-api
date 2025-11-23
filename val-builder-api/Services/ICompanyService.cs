using val_builder_api.Models;

namespace val_builder_api.Services;

public interface ICompanyService
{
    Task<IEnumerable<Company>> GetAllCompaniesAsync();
    Task<Company?> GetCompanyByIdAsync(int id);
}
