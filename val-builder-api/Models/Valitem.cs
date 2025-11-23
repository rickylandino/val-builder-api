using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

[Table("VALItems")]
public partial class Valitem
{
    [Key]
    [Column("ItemID")]
    public int ItemId { get; set; }

    [Column("ValID")]
    public int? ValId { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }

    [Column("TemplateItemID")]
    public int? TemplateItemId { get; set; }

    public int? TemplateItemVersion { get; set; }

    public int? PreviousItemVersion { get; set; }

    public bool? YellowFlag { get; set; }

    public bool? RedFlag { get; set; }

    public bool? Finalized { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? ItemType { get; set; }

    [StringLength(3000)]
    [Unicode(false)]
    public string? ItemText { get; set; }

    public bool? BulletItem { get; set; }

    public int? IndentItem { get; set; }

    public bool? CenterItem { get; set; }

    public bool? BoldItem { get; set; }

    public int? BlankLinesAfter { get; set; }

    public int? DisplayOrder { get; set; }

    public int Col1Width { get; set; }

    public int Col2Width { get; set; }

    public int Col3Width { get; set; }

    public int Col4Width { get; set; }
}
