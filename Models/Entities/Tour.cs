using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities;

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

    [MaxLength(2000)]
    public string? Images { get; set; }

    public int MaxCapacity { get; set; } = 30;

    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";

    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    public string? IncludedServices { get; set; }
    public string? ExcludedServices { get; set; }
    public string? CancellationPolicy { get; set; }
    public string? ChangePolicy { get; set; }
    public decimal DepositPercent { get; set; } = 20;
    public string? ChildPolicy { get; set; }
    public string? InfantPolicy { get; set; }
    public string? SeniorPolicy { get; set; }
    public string? PickupPoints { get; set; }
    public string? MeetingInstructions { get; set; }
    public string? LuggagePolicy { get; set; }
    public string? InsuranceDetails { get; set; }
    public string? VisaRequirements { get; set; }
    public string? RequiredDocuments { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? VideoUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<TourDeparture> Departures { get; set; } = new();
    public List<TourItinerary> Itineraries { get; set; } = new();
}
