using System.ComponentModel.DataAnnotations;

namespace Models.DTOs;

public class CreateBookingDto
{
    [Required]
    public int TourId { get; set; }

    [Required]
    public string TourTitle { get; set; } = string.Empty;

    public int DepartureId { get; set; }
    public DateTime StartDate { get; set; }

    [Range(1, 100)]
    public int AdultsCount { get; set; } = 1;

    [Range(0, 100)]
    public int ChildrenCount { get; set; } = 0;

    public decimal PricePerPerson { get; set; }

    [Required]
    public string ContactName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;

    [Required]
    public string ContactPhone { get; set; } = string.Empty;
}

public class BookingDto
{
    public int Id { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public int TourId { get; set; }
    public string TourTitle { get; set; } = string.Empty;
    public int DepartureId { get; set; }
    public DateTime StartDate { get; set; }
    public int AdultsCount { get; set; }
    public int ChildrenCount { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? VietQrUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateBookingStatusDto
{
    [Required]
    public string Status { get; set; } = "CONFIRMED"; // CONFIRMED, CANCELLED
}
