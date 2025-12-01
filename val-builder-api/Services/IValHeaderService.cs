using val_builder_api.Models;

namespace val_builder_api.Services;

public interface IValHeaderService
{
    Task<IEnumerable<Valheader>> GetAllValHeadersAsync();
    Task<Valheader?> GetValHeaderByIdAsync(int id);
    Task<Valheader> CreateValHeaderAsync(Valheader valHeader);
    Task<Valheader?> UpdateValHeaderAsync(int id, Valheader valHeader);
    Task<IEnumerable<Valheader>> GetValHeadersByCompanyIdAsync(int companyId);
    Task<IEnumerable<Valheader>> GetValHeadersByPlanIdAsync(int planId);
}
