namespace val_builder_api.Dto;

public class ValItemChangeDto
{
    public int ValId { get; set; }
    public List<ValItemChange> Changes { get; set; } = new();
}

public class ValItemChange
{
    public string Type { get; set; } = ""; // 'create' | 'update' | 'delete'
    public int? ItemId { get; set; }
    public int GroupId { get; set; }
    public Dictionary<string, object>? Item { get; set; } // Partial<Valitem>
}
