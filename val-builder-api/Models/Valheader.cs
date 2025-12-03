using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

[Table("VALHeader")]
public partial class Valheader
{
    [Key]
    [Column("ValID")]
    public int? ValId { get; set; }

    public int? PlanId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ValDescription { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ValDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PlanYearBeginDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PlanYearEndDate { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? RecipientName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? RecipientAddress1 { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? RecipientAddress2 { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? RecipientCity { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string? RecipientState { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? RecipientZip { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FinalizeDate { get; set; }

    [StringLength(16)]
    [Unicode(false)]
    public string? FinalizedBy { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? WordDocPath { get; set; }

    [Column("VALStatusId")]
    public int? ValstatusId { get; set; }

    public int? MarginLeftRight { get; set; }

    public int? MarginTopBottom { get; set; }

    public int? FontSize { get; set; }

    public int? ValYear { get; set; }

    public int? ValQuarter { get; set; }
}
