using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HTML_Parser.Models.Entities;

[Table("Laptop")]
public partial class Laptop
{
    [Key]
    [Column("ID_laptop")]
    public Guid IdLaptop { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("OS")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Os { get; set; } = null!;

    [Column("Screen_diagonal")]
    [StringLength(20)]
    [Unicode(false)]
    public string? ScreenDiagonal { get; set; } = null!;

    [Column("Processor_model")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ProcessorModel { get; set; } = null!;

    [Column("Video_card_type")]
    [StringLength(50)]
    [Unicode(false)]
    public string? VideoCardType { get; set; } = null!;

    [Column("Video_card_model")]
    [StringLength(50)]
    [Unicode(false)]
    public string? VideoCardModel { get; set; } = null!;
}
