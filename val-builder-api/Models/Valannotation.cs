using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

[Keyless]
[Table("VALAnnotations")]
public partial class Valannotation
{
    [Column("AnnotationID")]
    public int AnnotationId { get; set; }

    [StringLength(50)]
    public string? Author { get; set; }

    [Column("AuthorID")]
    [StringLength(50)]
    public string? AuthorId { get; set; }

    public string? AnnotationContent { get; set; }

    [Column("AnnotationGroupID")]
    public Guid? AnnotationGroupId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateModified { get; set; }

    [Column("ValID")]
    public int? ValId { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }
}
