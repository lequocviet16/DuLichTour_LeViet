using System.Security.Claims;
using LeVietTravel.BookingService.Data;
using LeVietTravel.BookingService.DTOs;
using LeVietTravel.BookingService.Entities;
using LeVietTravel.Common.Constants;
using LeVietTravel.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeVietTravel.BookingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingDbContext _context;

    public BookingsController(BookingDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được danh tính người dùng."));
        }

        var bookingCode = $"BK-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        var totalPrice = (dto.AdultsCount * dto.PricePerPerson) + (dto.ChildrenCount * dto.PricePerPerson * 0.75m);
        var finalAmount = totalPrice;

        // Sinh link VietQR động
        var qrInfo = Uri.EscapeDataString($"{bookingCode} {dto.ContactPhone}");
        var qrUrl = $"https://img.vietqr.io/image/MB-0988888888-compact2.png?amount={(long)finalAmount}&addInfo={qrInfo}";

        var booking = new Booking
        {
            BookingCode = bookingCode,
            UserId = userId,
            ContactName = dto.ContactName.Trim(),
            ContactEmail = dto.ContactEmail.Trim(),
            ContactPhone = dto.ContactPhone.Trim(),
            TourId = dto.TourId,
            TourTitle = dto.TourTitle,
            DepartureId = dto.DepartureId,
            StartDate = dto.StartDate,
            AdultsCount = dto.AdultsCount,
            ChildrenCount = dto.ChildrenCount,
            TotalPrice = totalPrice,
            DiscountAmount = 0,
            FinalAmount = finalAmount,
            Status = "PENDING",
            VietQrUrl = qrUrl,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return StatusCode(201, ApiResponse<BookingDto>.Ok(MapToDto(booking), "Đặt tour thành công. Vui lòng quét mã VietQR để hoàn tất thanh toán."));
    }

    [Authorize]
    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được danh tính người dùng."));
        }

        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => MapToDto(b))
            .ToListAsync();

        return Ok(ApiResponse<List<BookingDto>>.Ok(bookings, "Lấy danh sách đơn đặt tour thành công."));
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được danh tính người dùng."));
        }

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy đơn hàng."));
        }

        // Customer chỉ xem đơn của mình, Admin xem được tất cả
        if (role != Roles.Admin && booking.UserId != userId)
        {
            return StatusCode(403, ApiResponse.Fail("Bạn không có quyền xem đơn hàng này."));
        }

        return Ok(ApiResponse<BookingDto>.Ok(MapToDto(booking), "Lấy chi tiết đơn đặt tour thành công."));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAllBookings([FromQuery] string? status)
    {
        var query = _context.Bookings.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(b => b.Status == status);
        }

        var list = await query
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => MapToDto(b))
            .ToListAsync();

        return Ok(ApiResponse<List<BookingDto>>.Ok(list, "Lấy toàn bộ đơn đặt chỗ thành công (Quyền Quản Trị Viên)."));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}/confirm")]
    public async Task<IActionResult> ConfirmBooking(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy đơn hàng."));
        }

        booking.Status = "CONFIRMED";
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<BookingDto>.Ok(MapToDto(booking), "Xác nhận duyệt thanh toán đơn đặt tour thành công."));
    }

    [Authorize]
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được danh tính người dùng."));
        }

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy đơn hàng."));
        }

        if (role != Roles.Admin && booking.UserId != userId)
        {
            return StatusCode(403, ApiResponse.Fail("Bạn không có quyền hủy đơn hàng này."));
        }

        booking.Status = "CANCELLED";
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<BookingDto>.Ok(MapToDto(booking), "Hủy đơn đặt tour thành công."));
    }

    private static BookingDto MapToDto(Booking b)
    {
        return new BookingDto
        {
            Id = b.Id,
            BookingCode = b.BookingCode,
            UserId = b.UserId,
            ContactName = b.ContactName,
            ContactEmail = b.ContactEmail,
            ContactPhone = b.ContactPhone,
            TourId = b.TourId,
            TourTitle = b.TourTitle,
            DepartureId = b.DepartureId,
            StartDate = b.StartDate,
            AdultsCount = b.AdultsCount,
            ChildrenCount = b.ChildrenCount,
            TotalPrice = b.TotalPrice,
            DiscountAmount = b.DiscountAmount,
            FinalAmount = b.FinalAmount,
            Status = b.Status,
            VietQrUrl = b.VietQrUrl,
            CreatedAt = b.CreatedAt
        };
    }
}
