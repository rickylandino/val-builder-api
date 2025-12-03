using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

[Table("VALTemplateItems")]
public partial class ValtemplateItem
{
    [Key]
    [Column("ItemID")]
    public int? ItemId { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? ItemText { get; set; }

    public int? DisplayOrder { get; set; }

    public int? BlankLineAfter { get; set; }

    public bool? Bold { get; set; }

    public bool? Bullet { get; set; }

    public bool? Center { get; set; }

    [Column("DefaultOnVAL")]
    public bool? DefaultOnVal { get; set; }

    public int? Indent { get; set; }

    public bool? TightLineHeight { get; set; }
}
