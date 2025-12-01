using Microsoft.AspNetCore.Mvc;
using val_builder_api.Services;

namespace val_builder_api.Controllers;

[ApiController]
[Route("api/val")]
public class ValPdfController : ControllerBase
{
    private readonly IValPdfService _pdfService;
    private readonly ILogger<ValPdfController> _logger;

    public ValPdfController(IValPdfService pdfService, ILogger<ValPdfController> logger)
    {
        _pdfService = pdfService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a PDF document from VAL data
    /// </summary>
    /// <param name="valId">The VAL ID</param>
    /// <param name="includeHeaders">Include section headers in the output</param>
    /// <param name="showWatermark">Show the draft watermark</param>
    /// <returns>PDF file</returns>
    [HttpGet("{valId}/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateValPdf(
        int valId, 
        [FromQuery] bool includeHeaders = false,
        [FromQuery] bool showWatermark = true)
    {
        try
        {
            _logger.LogInformation("Generating PDF for VAL {ValId}, includeHeaders={IncludeHeaders}, showWatermark={ShowWatermark}", 
                valId, includeHeaders, showWatermark);

            // Fetch data from database
            var valData = await _pdfService.GetValDataForPdfAsync(valId);
            if (valData == null)
            {
                _logger.LogWarning("VAL {ValId} not found", valId);
                return NotFound(new { message = $"VAL {valId} not found" });
            }

            // Generate PDF
            var pdfData = await _pdfService.GeneratePdfAsync(valData, includeHeaders, showWatermark);

            // Return PDF with appropriate filename
            var fileName = $"VAL-{valId}-{DateTime.Now:yyyyMMdd-HHmmss}.pdf";
            _logger.LogInformation("Successfully generated PDF for VAL {ValId}, size: {Size} bytes", valId, pdfData.Length);

            return File(pdfData, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF for VAL {ValId}", valId);
            return StatusCode(500, new { message = "Error generating PDF", error = ex.Message });
        }
    }
}
