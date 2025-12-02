using val_builder_api.Models;

namespace val_builder_api.Dto;

public class ValDetailChangeDto
{
    public int? ValId { get; set; }
    public List<ValDetailChange> Changes { get; set; } = new();
}

public class ValDetailChange
{
    public string Action { get; set; } = string.Empty; // "create", "update", "delete"
    public Valdetail? Detail { get; set; }
}

public class ValDetailSaveResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ItemsCreated { get; set; }
    public int ItemsUpdated { get; set; }
    public int ItemsDeleted { get; set; }
    public List<string>? Errors { get; set; }
    public string? Error { get; set; }
}
