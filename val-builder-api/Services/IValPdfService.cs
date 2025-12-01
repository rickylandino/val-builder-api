using val_builder_api.Models;

namespace val_builder_api.Services;

public interface IValPdfService
{
    Task<ValPdfData?> GetValDataForPdfAsync(int valId);
    Task<byte[]> GeneratePdfAsync(ValPdfData data, bool includeHeaders, bool showWatermark = true);
}
