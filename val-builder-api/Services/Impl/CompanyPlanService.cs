using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;

namespace val_builder_api.Services.Impl;

public class CompanyPlanService : ICompanyPlanService
{
    private readonly ApplicationDbContext _context;

    public CompanyPlanService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CompanyPlan>> GetAllCompanyPlansAsync()
    {
        return await _context.CompanyPlans.AsNoTracking().ToListAsync();
    }

    public async Task<CompanyPlan?> GetCompanyPlanByIdAsync(int id)
    {
        return await _context.CompanyPlans.AsNoTracking().FirstOrDefaultAsync(p => p.PlanId == id);
    }

    public async Task<CompanyPlan> CreateCompanyPlanAsync(CompanyPlan plan)
    {
        _context.CompanyPlans.Add(plan);
        await _context.SaveChangesAsync();
        return plan;
    }

    public async Task<CompanyPlan?> UpdateCompanyPlanAsync(int id, CompanyPlan plan)
    {
        var existing = await _context.CompanyPlans.FindAsync(id);
        if (existing == null)
            return null;
        existing.CompanyId = plan.CompanyId;
        existing.PlanType = plan.PlanType;
        existing.PlanName = plan.PlanName;
        existing.PlanYearEnd = plan.PlanYearEnd;
        existing.Tech = plan.Tech;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<IEnumerable<CompanyPlan>> GetCompanyPlansByCompanyIdAsync(int companyId)
    {
        return await _context.CompanyPlans.AsNoTracking().Where(p => p.CompanyId == companyId).ToListAsync();
    }
}
