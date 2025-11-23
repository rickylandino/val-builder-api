using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

public partial class CompanyPlan
{
    [Key]
    public int PlanId { get; set; }

    public int? CompanyId { get; set; }

    [StringLength(7)]
    [Unicode(false)]
    public string? PlanType { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? PlanName { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? PlanYearEnd { get; set; }

    [StringLength(4)]
    [Unicode(false)]
    public string? Tech { get; set; }
}
