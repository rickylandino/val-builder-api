using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

[Keyless]
[Table("VALDetails")]
public partial class Valdetail
{
    [Column("ValDetailsID")]
    public Guid? ValDetailsId { get; set; }

    [Column("ValID")]
    public int? ValId { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }

    public string? GroupContent { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? Bullet { get; set; }

    public int? Indent { get; set; }

    public bool? Bold { get; set; }

    public bool? Center { get; set; }

    public int? BlankLineAfter { get; set; }

    public bool? TightLineHeight { get; set; }
}
