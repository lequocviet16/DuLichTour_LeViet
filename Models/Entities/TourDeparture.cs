using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities;

[Table("TourDepartures")]
public class TourDeparture
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TourId { get; set; }
    [ForeignKey("TourId")]
    public Tour? Tour { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int TotalSeats { get; set; } = 30;
    public int BookedSeats { get; set; } = 0;

    [MaxLength(20)]
    public string Status { get; set; } = "AVAILABLE";
}
