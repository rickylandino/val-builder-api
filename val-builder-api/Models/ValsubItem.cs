using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

[Table("VALSubItems")]
public partial class ValsubItem
{
    [Key]
    [Column("SubItemID")]
    public int SubItemId { get; set; }

    [Column("ParentItemID")]
    public int? ParentItemId { get; set; }

    [Column("ValID")]
    public int? ValId { get; set; }

    [Column("TemplateItemID")]
    public int? TemplateItemId { get; set; }

    public int? TemplateItemVersion { get; set; }

    public int? PreviousItemVersion { get; set; }

    public bool? YellowFlag { get; set; }

    public bool? RedFlag { get; set; }

    public bool? Finalized { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Col1Value { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Col2Value { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Col3Value { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Col4Value { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Col1Type { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Col2Type { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Col3Type { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Col4Type { get; set; }

    public int? DisplayOrder { get; set; }
}
