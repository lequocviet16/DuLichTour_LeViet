using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Common;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly LeVietTravelDbContext _context;

    public PaymentsController(LeVietTravelDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var bookings = await _context.Bookings.ToListAsync();

        var mockPayments = bookings.Select((b, i) => new
        {
            id = i + 1,
            bookingId = b.Id,
            booking = new
            {
                id = b.Id,
                bookingCode = b.BookingCode,
                finalAmount = (double)b.TotalPrice,
                discountAmount = 0.0,
                voucher = (object?)null,
                departure = new
                {
                    startDate = b.StartDate.ToString("yyyy-MM-dd"),
                    tour = new { name = !string.IsNullOrEmpty(b.TourTitle) ? b.TourTitle : "Tour du lịch Lê Việt" }
                }
            },
            amount = (double)b.TotalPrice,
            paymentMethod = "VIETQR",
            status = b.Status == "CONFIRMED" || b.Status == "COMPLETED" ? "SUCCESS" : "PENDING",
            transactionId = $"VIETQR-20261115-{i + 1:D4}",
            proofUrl = (string?)null,
            createdAt = b.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        if (!string.IsNullOrWhiteSpace(status))
        {
            mockPayments = mockPayments.Where(p => p.status == status).ToList();
        }

        var result = new
        {
            items = mockPayments.Skip((page - 1) * limit).Take(limit).ToList(),
            pagination = new
            {
                page,
                limit,
                totalItems = mockPayments.Count,
                totalPages = (int)Math.Ceiling((double)mockPayments.Count / limit)
            }
        };

        return Ok(ApiResponse.Ok(result, "Lấy danh sách thanh toán thành công"));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest req)
    {
        var b = await _context.Bookings.FindAsync(req.BookingId);
        var amount = b != null ? (double)b.FinalAmount : 3450000.0;
        var code = b != null ? b.BookingCode : "BK-DEMO";
        var qrUrl = $"https://img.vietqr.io/image/MB-0988888888-compact2.png?amount={amount}&addInfo={code}";

        var result = new
        {
            payment = new
            {
                id = 1,
                bookingId = req.BookingId,
                amount = amount,
                paymentMethod = "VIETQR",
                status = "PENDING",
                createdAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            },
            vietQr = new
            {
                qrDataUrl = qrUrl,
                bankAccount = "0988888888",
                bankName = "MB Bank (NHTMCP Quân Đội)",
                accountName = "LE VIET TRAVEL",
                amount = amount,
                transferContent = code
            }
        };

        return Ok(ApiResponse.Ok(result, "Khởi tạo thanh toán VietQR thành công"));
    }

    [HttpPost("{id}/proof")]
    public IActionResult UploadProof(int id)
    {
        return Ok(ApiResponse.Ok(new { success = true }, "Đã tải lên chứng từ chuyển khoản thành công"));
    }

    public class CreatePaymentRequest
    {
        public int BookingId { get; set; }
        public string? PaymentMethod { get; set; }
    }

    [HttpPatch("{id}/status")]
    public IActionResult UpdateStatus(int id, [FromBody] UpdatePaymentStatusRequest req)
    {
        return Ok(ApiResponse.Ok(new { id, req.status, req.transactionId }, "Cập nhật trạng thái giao dịch thành công"));
    }

    [HttpPost("reconciliation/import")]
    public IActionResult ImportReconciliation()
    {
        return Ok(ApiResponse.Ok(new { matched = 3, duplicate = 0, unmatched = 1, failed = 0 }, "Đối soát hoàn tất"));
    }

    public class UpdatePaymentStatusRequest
    {
        public string status { get; set; } = string.Empty;
        public string? transactionId { get; set; }
        public string? rejectionReason { get; set; }
    }
}
