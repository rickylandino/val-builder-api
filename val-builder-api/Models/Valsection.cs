using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

[Keyless]
[Table("VALSections")]
public partial class Valsection
{
    [Column("GroupID")]
    public int GroupId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? SectionText { get; set; }

    public int? DisplayOrder { get; set; }

    public int? DefaultColWidth1 { get; set; }

    public int? DefaultColWidth2 { get; set; }

    public int? DefaultColWidth3 { get; set; }

    public int? DefaultColWidth4 { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? DefaultColType1 { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? DefaultColType2 { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? DefaultColType3 { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? DefaultColType4 { get; set; }

    public bool? AutoIndent { get; set; }
}
