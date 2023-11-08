using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HTML_Parser.Models.Entities;

[Table("TV")]
public partial class Tv
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? Brand { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Diagonal { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Weight { get; set; }
}
