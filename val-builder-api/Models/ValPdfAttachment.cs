using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace val_builder_api.Models;

[Table("VALPdfAttachments")]
public class ValPdfAttachment
{
    [Key]
    [Column("PDFId")]
    public int PDFId { get; set; }

    [Column("ValID")]
    public int? ValID { get; set; }

    [Column("PDFName")]
    [StringLength(100)]
    public string? PDFName { get; set; }

    [Column("DisplayOrder")]
    public int? DisplayOrder { get; set; }

    [Column("PDFContents")]
    public byte[]? PDFContents { get; set; }
}