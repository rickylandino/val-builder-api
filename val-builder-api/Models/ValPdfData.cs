namespace val_builder_api.Models;

public class ValPdfData
{
    public ValPdfHeader ValHeader { get; set; } = new();
    public List<ValPdfSection> Sections { get; set; } = new();
}

public class ValPdfHeader
{
    public int ValId { get; set; }
    public string ValDescription { get; set; } = string.Empty;
    public DateTime? PlanYearBeginDate { get; set; }
    public DateTime? PlanYearEndDate { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
}

public class ValPdfSection
{
    public int GroupId { get; set; }
    public string SectionText { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public List<ValPdfDetail> Details { get; set; } = new();
}

public class ValPdfDetail
{
    public Guid ValDetailsId { get; set; }
    public int GroupId { get; set; }
    public string DetailText { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool Bullet { get; set; }
    public int Indent { get; set; }
    public bool Bold { get; set; }
    public bool Center { get; set; }
    public bool TightLineHeight { get; set; }
    public int BlankLineAfter { get; set; }
}
