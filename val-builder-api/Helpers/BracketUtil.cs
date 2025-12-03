using System.Reflection;
using System.Text.RegularExpressions;
using val_builder_api.Models;

namespace val_builder_api.Helpers
{
    public static partial class BracketUtil
    {
        [GeneratedRegex(@"\[\[(.*?)\]\]")]
        private static partial Regex BracketTagRegex();

        public static async Task<string> ReplaceBracketTagsAsync(List<BracketMapping> mappings, string content, Valheader valHeader, Company company, CompanyPlan companyPlan)
        {
            if (string.IsNullOrEmpty(content)) return content;

            return BracketTagRegex().Replace(content, match =>
            {
                var tag = match.Groups[1].Value.Trim();
                var mapping = mappings.FirstOrDefault(m => m.TagName.Equals(tag, StringComparison.OrdinalIgnoreCase));
                if (mapping == null)
                    return match.Value; // leave unchanged

                if (mapping.SystemTag)
                    return ReplaceSystemTag(tag, valHeader);

                return ReplaceCustomTag(tag, mapping.ObjectPath, valHeader, companyPlan, company);
            });
        }

        private static string ReplaceSystemTag(string tag, Valheader valHeader)
        {
            if (tag.Equals("PYE", StringComparison.OrdinalIgnoreCase))
            {
                var date = valHeader.PlanYearEndDate;
                var formatted = date.HasValue ? date.Value.ToString("MM/dd/yyyy") : "";
                return $"[[PYE: {formatted}]]";
            }
            if (tag.StartsWith("PYE+", StringComparison.OrdinalIgnoreCase))
            {
                var date = valHeader.PlanYearEndDate;
                var addMonthsStr = tag.Substring(4);
                if (int.TryParse(addMonthsStr, out int addMonths) && date.HasValue)
                {
                    var newDate = date.Value.AddMonths(addMonths);
                    return $"[[PYE+{addMonths}: {newDate:MM/dd/yyyy}]]";
                }
                return $"[[PYE+{addMonthsStr}: ]]";
            }
            if (tag.Equals("PriorYearPYE", StringComparison.OrdinalIgnoreCase))
            {
                var date = valHeader.PlanYearBeginDate;
                var formatted = date.HasValue ? date.Value.AddDays(-1).ToString("MM/dd/yyyy") : "";
                return $"[[PriorYearPYE: {formatted}]]";
            }
            // Add more system tag logic here as needed
            return $"[[{tag}]]";
        }

        private static string ReplaceCustomTag(string tag, string objectPath, Valheader valHeader, CompanyPlan? companyPlan, Company? company)
        {
            object? contextObj = null;
            if (objectPath.StartsWith("companyPlan.") && companyPlan != null)
                contextObj = companyPlan;
            else if (objectPath.StartsWith("company.") && company != null)
                contextObj = company;
            else if (objectPath.StartsWith("valHeader.") && valHeader != null)
                contextObj = valHeader;

            if (contextObj != null)
            {
                var propName = objectPath.Split('.')[1];
                var prop = contextObj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var value = prop?.GetValue(contextObj)?.ToString() ?? "";
                return $"[[{tag}: {value}]]";
            }
            return $"[[{tag}: ]]";
        }
    }
}
