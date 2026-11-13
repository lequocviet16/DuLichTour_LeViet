using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeVietTravel.TourService.Entities;

[Table("Categories")]
public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public List<Tour> Tours { get; set; } = new();
}

[Table("Tours")]
public class Tour
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(250)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Summary { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountPrice { get; set; }

    public int DurationDays { get; set; } = 3;
    public int DurationNights { get; set; } = 2;

    [MaxLength(150)]
    public string DestinationName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public int MaxCapacity { get; set; } = 30;

    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";

    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<TourDeparture> Departures { get; set; } = new();
}

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
