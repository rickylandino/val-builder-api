using val_builder_api.Models;

namespace val_builder_api.Services;

public interface ICompanyPlanService
{
    Task<IEnumerable<CompanyPlan>> GetAllCompanyPlansAsync();
    Task<CompanyPlan?> GetCompanyPlanByIdAsync(int id);
    Task<CompanyPlan> CreateCompanyPlanAsync(CompanyPlan plan);
    Task<CompanyPlan?> UpdateCompanyPlanAsync(int id, CompanyPlan plan);
    Task<IEnumerable<CompanyPlan>> GetCompanyPlansByCompanyIdAsync(int companyId);
}
