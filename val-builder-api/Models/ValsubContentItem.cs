using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

[Keyless]
[Table("VALSubContentItems")]
public partial class ValsubContentItem
{
    [Column("ItemID")]
    public int ItemId { get; set; }

    [Column("SubContentID")]
    public int? SubContentId { get; set; }

    public string? ItemContent { get; set; }

    public int? DisplayOrder { get; set; }

    [StringLength(500)]
    public string? ContributionType { get; set; }

    [StringLength(50)]
    public string? ContributionAmount { get; set; }

    [StringLength(50)]
    public string? Class { get; set; }

    [StringLength(50)]
    public string? Group { get; set; }

    [StringLength(150)]
    public string? Participant { get; set; }

    [StringLength(50)]
    public string? ExcessContributionAmount { get; set; }

    [StringLength(50)]
    public string? Earnings { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateOfTermination { get; set; }

    [StringLength(50)]
    public string? VestedPercent { get; set; }

    [StringLength(50)]
    public string? VestedBalance { get; set; }

    [Column("IsADP")]
    public bool? IsAdp { get; set; }

    [Column("IsACP")]
    public bool? IsAcp { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateOfEntry { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateOfBirth { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateOfHire { get; set; }
}
