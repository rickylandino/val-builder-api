using FluentAssertions;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Helpers;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Tests.Helpers
{
    public class BracketUtilTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public BracketUtilTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task ReplaceBracketTagsAsync_ReplacesSystemTag_PYE()
        {
            _context.BracketMappings.Add(new BracketMapping { TagName = "PYE", ObjectPath = "", SystemTag = true });
            await _context.SaveChangesAsync();

            var valHeader = new Valheader { PlanYearEndDate = new DateTime(2024, 12, 31) };
            var mappings = await _context.BracketMappings.ToListAsync();
            var result = await BracketUtil.ReplaceBracketTagsAsync(mappings, "Val ends [[PYE]]", valHeader, null, null);
            result.Should().Contain("12/31/2024");
        }

        [Fact]
        public async Task ReplaceBracketTagsAsync_ReplacesSystemTag_PYEPlus()
        {
            _context.BracketMappings.Add(new BracketMapping { TagName = "PYE+3", ObjectPath = "", SystemTag = true });
            await _context.SaveChangesAsync();

            var valHeader = new Valheader { PlanYearEndDate = new DateTime(2024, 12, 31) };
            var mappings = await _context.BracketMappings.ToListAsync();
            var result = await BracketUtil.ReplaceBracketTagsAsync(mappings, "Val ends [[PYE+3]]", valHeader, null, null);
            result.Should().Contain("03/31/2025");
        }

        [Fact]
        public async Task ReplaceBracketTagsAsync_ReplacesSystemTag_PriorYearPYE()
        {
            _context.BracketMappings.Add(new BracketMapping { TagName = "PriorYearPYE", ObjectPath = "", SystemTag = true });
            await _context.SaveChangesAsync();

            var valHeader = new Valheader { PlanYearBeginDate = new DateTime(2024, 1, 1) };
            var mappings = await _context.BracketMappings.ToListAsync();
            var result = await BracketUtil.ReplaceBracketTagsAsync(mappings, "Prior year [[PriorYearPYE]]", valHeader, null, null);
            result.Should().Contain("12/31/2023");
        }

        [Fact]
        public async Task ReplaceBracketTagsAsync_ReplacesCustomTag_CompanyPlan()
        {
            _context.BracketMappings.Add(new BracketMapping { TagName = "Tech", ObjectPath = "companyPlan.Tech", SystemTag = false });
            await _context.SaveChangesAsync();

            var plan = new CompanyPlan { PlanId = 1, Tech = "ABC" };
            _context.CompanyPlans.Add(plan);
            await _context.SaveChangesAsync();

            var valHeader = new Valheader { PlanId = 1 };
            var mappings = await _context.BracketMappings.ToListAsync();
            var result = await BracketUtil.ReplaceBracketTagsAsync(mappings, "Tech is [[Tech]]", valHeader, null, plan);
            result.Should().Contain("ABC");
        }

        [Fact]
        public async Task ReplaceBracketTagsAsync_LeavesUnknownTagUnchanged()
        {
            var valHeader = new Valheader();
            var mappings = await _context.BracketMappings.ToListAsync();
            var result = await BracketUtil.ReplaceBracketTagsAsync(mappings, "Unknown [[NotMapped]]", valHeader, null, null);
            result.Should().Contain("[[NotMapped]]");
        }

        [Fact]
        public void StripBrackets_RemovesBracketsAndTag()
        {
            var input = "This is [[PYE: 12/31/2024]] and [[Tech: ABC]]";
            var output = System.Text.RegularExpressions.Regex.Replace(input, @"\[\[.*?:\s*(.*?)\]\]", "$1");
            output.Should().Be("This is 12/31/2024 and ABC");
        }
    }
}