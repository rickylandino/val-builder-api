using System.Text;
using System.Web;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using val_builder_api.Data;
using val_builder_api.Models;

namespace val_builder_api.Services.Impl;

public class ValPdfService : IValPdfService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ValPdfService> _logger;
    private static bool _browserDownloaded = false;
    private static readonly SemaphoreSlim _downloadSemaphore = new(1, 1);

    public ValPdfService(ApplicationDbContext context, ILogger<ValPdfService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ValPdfData?> GetValDataForPdfAsync(int valId)
    {
        // Fetch VAL header with company/plan info
        var valHeader = await _context.Valheaders
            .Where(v => v.ValId == valId)
            .Select(v => new ValPdfHeader
            {
                ValId = v.ValId,
                ValDescription = v.ValDescription ?? "",
                PlanYearBeginDate = v.PlanYearBeginDate,
                PlanYearEndDate = v.PlanYearEndDate,
                RecipientName = v.RecipientName ?? "",
                ClientName = "" // Will be populated from CompanyPlan if needed
            })
            .FirstOrDefaultAsync();

        if (valHeader == null)
            return null;

        // Fetch details grouped by GroupID (which represents sections)
        var details = await _context.Valdetails
            .Where(d => d.ValId == valId)
            .OrderBy(d => d.DisplayOrder)
            .Select(d => new ValPdfDetail
            {
                ValDetailsId = d.ValDetailsId ?? Guid.Empty,
                GroupId = d.GroupId ?? 0,
                DetailText = d.GroupContent ?? "",
                DisplayOrder = d.DisplayOrder ?? 0,
                Bullet = d.Bullet ?? false,
                Indent = d.Indent ?? 0,
                Bold = d.Bold ?? false,
                Center = d.Center ?? false,
                TightLineHeight = d.TightLineHeight ?? false,
                BlankLineAfter = d.BlankLineAfter ?? 0
            })
            .ToListAsync();

        // Group details by GroupId to create sections
        var detailsByGroup = details.GroupBy(d => d.GroupId).ToDictionary(g => g.Key, g => g.ToList());

        // Get section definitions from VALSections (for section headers)
        var sectionDefinitions = await _context.Valsections
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new
            {
                GroupId = s.GroupId,
                SectionText = s.SectionText ?? "",
                DisplayOrder = s.DisplayOrder ?? 0
            })
            .ToListAsync();

        // Create sections with their details
        var sections = new List<ValPdfSection>();

        foreach (var sectionDef in sectionDefinitions)
        {
            // Only include sections that have details for this VAL
            if (detailsByGroup.TryGetValue(sectionDef.GroupId, out var sectionDetails))
            {
                sections.Add(new ValPdfSection
                {
                    GroupId = sectionDef.GroupId,
                    SectionText = sectionDef.SectionText,
                    DisplayOrder = sectionDef.DisplayOrder,
                    Details = sectionDetails
                });
            }
        }

        // Also include any details that don't have a section definition (orphaned details)
        var definedGroupIds = sectionDefinitions.Select(s => s.GroupId).ToHashSet();
        foreach (var groupId in detailsByGroup.Keys.Where(k => !definedGroupIds.Contains(k)))
        {
            sections.Add(new ValPdfSection
            {
                GroupId = groupId,
                SectionText = $"Section {groupId}",
                DisplayOrder = groupId, // Use groupId as display order for orphaned sections
                Details = detailsByGroup[groupId]
            });
        }

        // Sort sections by display order
        sections = sections.OrderBy(s => s.DisplayOrder).ToList();

        return new ValPdfData
        {
            ValHeader = valHeader,
            Sections = sections
        };
    }

    public async Task<byte[]> GeneratePdfAsync(ValPdfData data, bool includeHeaders, bool showWatermark = true)
    {
        // Ensure browser is downloaded (one-time operation)
        await EnsureBrowserDownloadedAsync();

        var html = GenerateHtmlContent(data, includeHeaders, showWatermark);

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage" }
        });

        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);

        var mainPdf = await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.Letter,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "1in",
                Right = "1in",
                Bottom = "1in",
                Left = "1in"
            }
        });

        // Fetch PDF attachments for this VAL, ordered by DisplayOrder
        var attachments = await _context.ValPdfAttachments
            .Where(x => x.ValID == data.ValHeader.ValId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();

        if (attachments.Count == 0)
        {
            return mainPdf;
        }

        // Merge main PDF and attachments
        var pdfsToMerge = new List<byte[]> { mainPdf };
        pdfsToMerge.AddRange(attachments.Where(a => a.PDFContents != null).Select(a => a.PDFContents!));
        return MergePdfs(pdfsToMerge);
    }

    private byte[] MergePdfs(List<byte[]> pdfs)
    {
        using var outputStream = new MemoryStream();
        using var pdfWriter = new PdfWriter(outputStream);
        using var pdfDocument = new PdfDocument(pdfWriter);
        var merger = new PdfMerger(pdfDocument);

        foreach (var pdfBytes in pdfs)
        {
            try
            {
                using var inputStream = new MemoryStream(pdfBytes);
                using var inputPdf = new PdfDocument(new PdfReader(inputStream));
                merger.Merge(inputPdf, 1, inputPdf.GetNumberOfPages());
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Skipping PDF during merge due to error: {ex.Message}");
            }
        }

        pdfDocument.Close();
        return outputStream.ToArray();
    }

    private async Task EnsureBrowserDownloadedAsync()
    {
        if (_browserDownloaded)
            return;

        await _downloadSemaphore.WaitAsync();
        try
        {
            if (!_browserDownloaded)
            {
                _logger.LogInformation("Downloading Chromium browser for PDF generation...");
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();
                _browserDownloaded = true;
                _logger.LogInformation("Chromium browser downloaded successfully.");
            }
        }
        finally
        {
            _downloadSemaphore.Release();
        }
    }

    private string GenerateHtmlContent(ValPdfData data, bool includeHeaders, bool showWatermark)
    {
        var sb = new StringBuilder();

        sb.Append("<!DOCTYPE html><html lang=\"en\"><head>");
        sb.Append("<meta charset=\"UTF-8\">");
        sb.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.Append($"<title>VAL Document - {data.ValHeader.ValId}</title>");
        sb.Append("<style>");
        sb.Append(GetCssStyles());
        sb.Append("</style></head><body>");
        sb.Append("<div class='document-container'>");

        // --- COVER PAGE ---
        sb.Append("<div class='cover-page' style='height: 100vh; position: relative; page-break-after: always;'>");

        // Stamp (top left)
        if (showWatermark)
        {
            sb.Append("<div class='stamp' style='position: absolute; left: 0; top: 15px;'>VAL DRAFT</div>");
        }

        // Centered content
        sb.Append("<div style='display: flex; flex-direction: column; align-items: center; justify-content: center; height: 70vh;'>");
        sb.Append($"<div style='font-size: 1.3rem; font-weight: bold; text-align: center; margin-bottom: 1.5rem;'>{HttpUtility.HtmlEncode(data.ValHeader.ValDescription)}</div>");
        sb.Append($"<div style='font-size: 1.1rem; text-align: center; margin-bottom: 2.5rem;'>");
        if (data.ValHeader.PlanYearEndDate.HasValue)
        {
            sb.Append($"{data.ValHeader.PlanYearEndDate.Value:MM/dd/yyyy} Valuation");
        }
        sb.Append("</div>");
        sb.Append($"<div style='font-size: 1.1rem; font-weight: bold; text-align: center; margin-bottom: 1.5rem;'>Report Prepared for</div>");
        sb.Append($"<div style='font-size: 1.1rem; font-weight: bold; text-align: center;'>{HttpUtility.HtmlEncode(data.ValHeader.RecipientName)}</div>");
        sb.Append("</div>");

        // Placeholder logo and contact info (bottom center)
        sb.Append("<div style='position: absolute; bottom: 0px; left: 0; width: 100%; text-align: center;'>");
        sb.Append("<div style='font-size: 2.5rem; font-weight: bold; color: #222; margin-bottom: 0.2rem;'>Pension <span style='color: #6bc04b;'>Consultants</span></div>");
        sb.Append("<div style='font-size: 1rem; color: #222; margin-bottom: 0.2rem;'>10 WATERSIDE DRIVE, SUITE 200 • FARMINGTON, CONNECTICUT 06032</div>");
        sb.Append("<div style='font-size: 1rem; color: #222;'>TEL 860/676-8000 • FAX 860/678-8925 • <a href='https://www.mypensionconsultants.com' style='color: #1a73e8;'>www.mypensionconsultants.com</a></div>");
        sb.Append("</div>");

        sb.Append("</div>"); // end cover-page

        // --- END COVER PAGE ---

        // Main content
        sb.Append("<div class='document-content'>");

        foreach (var section in data.Sections.OrderBy(s => s.DisplayOrder))
        {
            if (includeHeaders)
            {
                sb.Append("<div class='section'>");
                sb.Append($"<h2 class='section-header'>{HttpUtility.HtmlEncode(section.SectionText)}</h2>");
                sb.Append("<div class='section-content'>");
            }

            foreach (var detail in section.Details.OrderBy(d => d.DisplayOrder))
            {
                sb.Append(RenderValDetail(detail));
            }

            if (includeHeaders)
            {
                sb.Append("</div></div>");
            }
        }

        sb.Append("</div></div></body></html>");

        return sb.ToString();
    }

    private string RenderValDetail(ValPdfDetail detail)
    {
        var classes = new List<string> { "val-detail" };

        if (detail.Bullet) classes.Add("bullet");
        if (detail.Indent > 0) classes.Add($"indent-{Math.Min(detail.Indent, 4)}");
        if (detail.Bold) classes.Add("bold");
        if (detail.Center) classes.Add("text-center");
        if (detail.TightLineHeight) classes.Add("tightLineHeight");
        if (detail.BlankLineAfter > 0)
        {
            var mbClass = $"mb-{Math.Min(detail.BlankLineAfter + 1, 4)}";
            classes.Add(mbClass);
        }

        var classAttr = string.Join(" ", classes);

        var content = detail.DetailText ?? "";

        // If content is wrapped in a <p> tag, process the inner text only
        if (content.TrimStart().StartsWith("<p>") || content.TrimStart().StartsWith("<p "))
        {
            // Find the opening <p> tag and closing </p>
            var pTagEnd = content.IndexOf('>');
            var pTagClose = content.LastIndexOf("</p>");
            if (pTagEnd > 0 && pTagClose > pTagEnd)
            {
                var openTag = content.Substring(0, pTagEnd);
                var innerText = content.Substring(pTagEnd + 1, pTagClose - pTagEnd - 1);

                // Strip chevrons from inner text
                innerText = System.Text.RegularExpressions.Regex.Replace(innerText, @"(&lt;&lt;|\<\<)\s*(.*?)\s*(\>\>|&gt;&gt;)", "$2");

                // Rebuild the <p> tag with classes
                if (openTag.Contains("class="))
                {
                    openTag = openTag.Replace("class=\"", $"class=\"{classAttr} ");
                }
                else
                {
                    openTag += $" class=\"{classAttr}\"";
                }
                content = $"{openTag}>{innerText}</p>";
            }
        }
        else
        {
            // Strip chevrons from plain text or other HTML
            content = System.Text.RegularExpressions.Regex.Replace(content, @"(&lt;&lt;|\<\<)\s*(.*?)\s*(\>\>|&gt;&gt;)", "$2");
            content = $"<p class='{classAttr}'>{content}</p>";
        }

        return content;
    }

    private static string GetCssStyles()
    {
        return @"
@import url('https://fonts.googleapis.com/css2?family=Questrial&display=swap');

@page:first {
  margin: .25in 1in;
@top-right {
    content: none;
  }
}

@page {
  size: 8.5in 11in;
  margin: .5in 1in;
    @top-right {
    content: ""Page "" counter(page);
    font-size: 10pt;
    color: #222;
    font-family: 'Segoe UI', Arial, sans-serif;
  }
}

* {
    font-family: -apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Oxygen,Ubuntu,Cantarell,'Open Sans','Helvetica Neue',sans-serif;
}

body {
    margin: unset !important;
    font-size: 12pt;
    line-height: 1.6;
    color: #000;
    padding: 0;
}

.document-container {
  width: 8.5in;
  min-height: 11in;
  background: white;
  position: relative;
}

.display-flex {
    display: flex;
    justify-content: space-between;
}

.flex-big {
    display: flex;
    width: 65%;
    flex-direction: column;
}

.d-flex-100 {
    display: flex !important;
    width: 100% !important;
}

p {
    font-size: 12pt;
}

/* Watermark/Stamp */
.watermark {
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%) rotate(-45deg);
    font-size: 120pt;
    font-weight: bold;
    color: rgba(200, 200, 200, 0.2);
    z-index: -1;
    pointer-events: none;
    white-space: nowrap;
}

.stamp {
    transform: rotate(-12deg);
    color: red;
    font-size: 1rem;
    font-weight: 700;
    border: 0.25rem double red;
    display: inline-block;
    padding: 0.25rem 1rem;
    text-transform: uppercase;
    border-radius: 1rem;
    font-family: 'Courier';
    mix-blend-mode: multiply;
    position: absolute;
}

.is-nope {
    color: #D23;
    border: 0.5rem double #D23;
    transform: rotate(-3deg);
    font-size: 1rem;
}

/* Header */
.val-header {
  text-align: center;
  margin-bottom: 2em;
  padding-bottom: 1em;
  border-bottom: 2px solid #333;
}

.client-name {
  font-size: 16pt;
  font-weight: bold;
  margin-bottom: 0.5em;
}

.val-title {
  font-size: 14pt;
  font-weight: bold;
  margin-bottom: 0.5em;
}

.plan-year {
  font-size: 11pt;
  color: #555;
}

/* Sections */
.section {
  margin-bottom: 2em;
  page-break-inside: avoid;
}

.section-header {
  font-size: 14pt;
  font-weight: bold;
  margin-bottom: 1em;
  padding-bottom: 0.3em;
  border-bottom: 1px solid #999;
}

/* ValDetail Paragraphs */
.val-detail {
  margin: 0;
  padding: 0;
  line-height: 1.6;
}

.bold {
    font-weight: 700;
}

.text-center {
    text-align: center;
}

.align-end {
    align-self: flex-end;
}

/* Indentation */
.indent-1 {
    margin-left: 1.2rem;
}

.indent-2 {
    margin-left: 2.4rem;
}

.indent-3 {
    margin-left: 3.6rem;
}

.indent-4 {
    margin-left: 4.8rem;
}

/* Bullets */
.bullet {
    display: list-item;
    list-style-type: disc;
    list-style-position: outside;
    margin-block-start: unset;
    margin-block-end: unset;
    margin-left: 20px;
}

/* Combine bullet + indent */
.bullet.indent-1 {
    margin-left: calc(20px + 1.2rem) !important;
}

.bullet.indent-2 {
    margin-left: calc(20px + 2.4rem) !important;
}

.bullet.indent-3 {
    margin-left: calc(20px + 3.6rem) !important;
}

.bullet.indent-4 {
    margin-left: calc(20px + 4.8rem) !important;
}

/* Margin utilities */
.mb-1 {
    margin-block-end: .5rem !important;
}

.mb-2 {
    margin-block-end: 1rem !important;
}

.mb-3 {
    margin-block-end: 2rem !important;
}

.mb-4 {
    margin-block-end: 3rem !important;
}

.mt-1 {
    margin-block-start: .5rem !important;
}

.mt-2 {
    margin-block-start: 1rem !important;
}

/* Tight line height */
.tightLineHeight {
    margin-block-start: unset !important;
    margin-block-end: unset !important;
}

.noTopMargin {
    margin-block-start: unset !important;
}

.noBottomMargin {
    margin-block-end: unset !important;
}

/* Table styles */
td, th {
    border: none;
    border-style: hidden !important;
    padding: 0;
    margin: 0;
    line-height: 1;
}

table.smallLineHeight {
    line-height: 1 !important;
    font-family: -apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Oxygen,Ubuntu,Cantarell,'Open Sans','Helvetica Neue',sans-serif;
    margin-top: 10px;
    border-collapse: collapse;
}

table.smallLineHeight caption {
    margin-bottom: 20px;
    font-size: 14px;
    font-weight: bold;
}

table:not([cellpadding]):not(.safaTable) td, 
table:not([cellpadding]):not(.safaTable) th {
    padding: unset !important;
}

.fixed-fs {
    font-size: 14px !important;
}

/* Page breaks */
@media print {
    .pagebreak {
        page-break-before: always;
    }
    
    .document-container {
        width: 100%;
        margin: 0;
        box-shadow: none;
    }
}
";
    }
}
