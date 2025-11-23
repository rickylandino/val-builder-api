using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

[Keyless]
[Table("VALTasks")]
public partial class Valtask
{
    [Column("TaskID")]
    public int TaskId { get; set; }

    [StringLength(100)]
    public string? Author { get; set; }

    [Column("AuthorID")]
    [StringLength(100)]
    public string? AuthorId { get; set; }

    [StringLength(255)]
    public string? TaskContent { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateModified { get; set; }

    [Column("ValID")]
    public int? ValId { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }

    public bool? Completed { get; set; }
}
