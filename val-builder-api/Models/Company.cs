using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace val_builder_api.Models;

public partial class Company
{
    [Key]
    public int? CompanyId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Name { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? MailingName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Street1 { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Street2 { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? City { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string? State { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Zip { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Fax { get; set; }
}
