using System.Security.Claims;
using BLL;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.DTOs;
using Models.Entities;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingBLL _bookingBLL;
    private readonly LeVietTravelDbContext _context;

    public BookingsController(BookingBLL bookingBLL, LeVietTravelDbContext context)
    {
        _bookingBLL = bookingBLL;
        _context = context;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetBookings([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được danh tính người dùng.", 401));
        }

        var query = _context.Bookings.AsQueryable();

        // Nếu không phải ADMIN, chỉ lấy đơn của chính người dùng
        if (roleClaim != "ADMIN")
        {
            query = query.Where(b => b.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(b => b.Status == status);
        }

        var totalItems = await query.CountAsync();
        var bookings = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        var items = bookings.Select(MapBooking).ToList();

        var result = new
        {
            items,
            pagination = new
            {
                page,
                limit,
                totalItems,
                totalPages = (int)Math.Ceiling((double)totalItems / limit)
            }
        };

        return Ok(ApiResponse.Ok(result, "Lấy danh sách đơn đặt thành công"));
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var b = await _context.Bookings.FindAsync(id);
        if (b == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy đơn đặt tour", 404));
        }

        return Ok(ApiResponse.Ok(MapBooking(b), "Lấy chi tiết đơn đặt thành công"));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được danh tính người dùng.", 401));
        }

        var result = await _bookingBLL.CreateBookingAsync(userId, dto);
        return StatusCode(201, result);
    }

    [Authorize]
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelBooking(int id, [FromBody] CancelBookingRequest? req)
    {
        var b = await _context.Bookings.FindAsync(id);
        if (b == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy đơn đặt tour", 404));
        }

        b.Status = "CANCELLED";
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Ok(MapBooking(b), "Hủy đơn đặt thành công"));
    }

    [Authorize]
    [HttpGet("{id:int}/ticket")]
    public async Task<IActionResult> GetTicket(int id)
    {
        var b = await _context.Bookings.FindAsync(id);
        if (b == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy đơn đặt tour", 404));
        }

        var ticket = new
        {
            bookingCode = b.BookingCode,
            checkInCode = $"CHK-{b.BookingCode}",
            checkedInAt = (string?)null,
            qrCodeDataUrl = "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='140' height='140' viewBox='0 0 140 140'><rect width='140' height='140' fill='white'/><text x='15' y='75' font-size='12' font-family='sans-serif' fill='%23087f5b' font-weight='bold'>LEVIET-TRAVEL-TICKET</text></svg>"
        };

        return Ok(ApiResponse.Ok(ticket, "Lấy vé check-in thành công"));
    }

    [Authorize]
    [HttpPost("{id:int}/change-requests")]
    public IActionResult CreateChangeRequest(int id, [FromBody] object body)
    {
        return Ok(ApiResponse.Ok(body, "Đã gửi yêu cầu thay đổi thành công"));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusDto dto)
    {
        var result = await _bookingBLL.UpdateStatusAsync(id, dto.Status);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    private static object MapBooking(Booking b)
    {
        return new
        {
            id = b.Id,
            bookingCode = b.BookingCode,
            bookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            numAdults = b.AdultsCount,
            numChildren = b.ChildrenCount,
            numInfants = 0,
            numSeniors = 0,
            singleRoomCount = 0,
            totalAmount = (double)b.TotalPrice,
            discountAmount = (double)b.DiscountAmount,
            depositAmount = (double)b.TotalPrice * 0.5,
            finalAmount = (double)b.FinalAmount,
            status = b.Status,
            paymentDueAt = b.CreatedAt.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            checkInCode = $"CHK-{b.BookingCode}",
            departure = new
            {
                id = b.DepartureId,
                startDate = b.StartDate.ToString("yyyy-MM-dd"),
                endDate = b.StartDate.AddDays(3).ToString("yyyy-MM-dd"),
                tour = new
                {
                    id = b.TourId,
                    name = b.TourTitle,
                    code = "TOUR",
                    thumbnail = "https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=800&q=80"
                }
            },
            payments = new[]
            {
                new
                {
                    id = 1,
                    amount = (double)b.FinalAmount,
                    paymentMethod = "VIETQR",
                    status = b.Status == "CONFIRMED" || b.Status == "COMPLETED" ? "SUCCESS" : "PENDING",
                    createdAt = b.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                }
            }
        };
    }

    public class CancelBookingRequest
    {
        public string? Reason { get; set; }
    }
}
