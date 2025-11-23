using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

[Keyless]
[Table("VALSubContent")]
public partial class ValsubContent
{
    [Column("SubContentID")]
    public int SubContentId { get; set; }

    [Column("ValDetailsID")]
    public Guid? ValDetailsId { get; set; }

    public int? NumberOfColumns { get; set; }
}
