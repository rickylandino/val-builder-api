using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace val_builder_api.Models;

[Table("BracketMappings")]
public class BracketMapping
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string TagName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string ObjectPath { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool SystemTag { get; set; } = false;
}
