namespace LeVietTravel.TourService.DTOs;

public class TourFilterDto
{
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 12;
}

public class TourDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int DurationDays { get; set; }
    public int DurationNights { get; set; }
    public string DestinationName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<TourDepartureDto> Departures { get; set; } = new();
}

public class TourDepartureDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public int TotalSeats { get; set; }
    public int BookedSeats { get; set; }
    public int AvailableSeats => TotalSeats - BookedSeats;
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TourCount { get; set; }
}

public class CreateTourDto
{
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int DurationDays { get; set; } = 3;
    public int DurationNights { get; set; } = 2;
    public string DestinationName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int MaxCapacity { get; set; } = 30;
}
