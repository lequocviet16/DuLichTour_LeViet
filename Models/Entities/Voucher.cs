using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities;

[Table("Vouchers")]
public class Voucher
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(20)]
    public string DiscountType { get; set; } = "PERCENT"; // PERCENT, FIXED

    [Column(TypeName = "decimal(12,2)")]
    public decimal DiscountValue { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal MinBookingAmount { get; set; } = 0;

    public int MaxUsage { get; set; } = 100;

    public int UsedCount { get; set; } = 0;

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(3);

    public bool IsActive { get; set; } = true;
}
