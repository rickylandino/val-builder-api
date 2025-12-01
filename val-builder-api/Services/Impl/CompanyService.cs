using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;

namespace val_builder_api.Services.Impl;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _context;

    public CompanyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        return await _context.Companies
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Company?> GetCompanyByIdAsync(int id)
    {
        return await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CompanyId == id);
    }

    public async Task<Company> CreateCompanyAsync(Company company)
    {
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
        return company;
    }

    public async Task<Company?> UpdateCompanyAsync(int id, Company company)
    {
        var existing = await _context.Companies.FindAsync(id);
        if (existing == null)
            return null;

        // Update properties
        existing.Name = company.Name;
        existing.MailingName = company.MailingName;
        existing.Street1 = company.Street1;
        existing.Street2 = company.Street2;
        existing.City = company.City;
        existing.State = company.State;
        existing.Zip = company.Zip;
        existing.Phone = company.Phone;
        existing.Fax = company.Fax;

        await _context.SaveChangesAsync();
        return existing;
    }
}
