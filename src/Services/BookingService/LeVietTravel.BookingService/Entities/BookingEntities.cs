using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeVietTravel.BookingService.Entities;

[Table("Bookings")]
public class Booking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string BookingCode { get; set; } = string.Empty;

    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ContactName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string ContactEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ContactPhone { get; set; } = string.Empty;

    public int TourId { get; set; }

    [MaxLength(200)]
    public string TourTitle { get; set; } = string.Empty;

    public int DepartureId { get; set; }
    public DateTime StartDate { get; set; }

    public int AdultsCount { get; set; } = 1;
    public int ChildrenCount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal FinalAmount { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = "PENDING"; // PENDING, CONFIRMED, CANCELLED

    [MaxLength(500)]
    public string? VietQrUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
