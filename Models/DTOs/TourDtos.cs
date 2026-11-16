using System.ComponentModel.DataAnnotations;

namespace Models.DTOs;

public class TourFilterDto
{
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}

public class TourDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int DurationDays { get; set; }
    public int DurationNights { get; set; }
    public string DestinationName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public int MaxCapacity { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public object? Category { get; set; }
    public object? Rating { get; set; }

    public string? IncludedServices { get; set; }
    public string? ExcludedServices { get; set; }
    public string? CancellationPolicy { get; set; }
    public string? ChangePolicy { get; set; }
    public decimal DepositPercent { get; set; } = 20;
    public string? ChildPolicy { get; set; }
    public string? InfantPolicy { get; set; }
    public string? SeniorPolicy { get; set; }
    public List<string> PickupPoints { get; set; } = new();
    public string? MeetingInstructions { get; set; }
    public string? LuggagePolicy { get; set; }
    public string? InsuranceDetails { get; set; }
    public string? VisaRequirements { get; set; }
    public string? RequiredDocuments { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? VideoUrl { get; set; }

    public List<object> Destinations { get; set; } = new();
    public List<TourDepartureDto> Departures { get; set; } = new();
    public List<ItineraryDto> Itineraries { get; set; } = new();
}

public class ItineraryDto
{
    public int Id { get; set; }
    public int DayNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class TourDepartureDto
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildPrice { get; set; }
    public int TotalSeats { get; set; }
    public int BookedSeats { get; set; }
    public int RemainingSeats { get; set; }
    public string Status { get; set; } = "AVAILABLE";
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateTourDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Summary { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(0, 1000000000)]
    public decimal Price { get; set; }

    public decimal? DiscountPrice { get; set; }

    public int DurationDays { get; set; } = 3;
    public int DurationNights { get; set; } = 2;

    [MaxLength(150)]
    public string DestinationName { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int CategoryId { get; set; }
}
