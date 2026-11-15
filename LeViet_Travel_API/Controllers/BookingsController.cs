using System.Security.Claims;
using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingBLL _bookingBLL;

    public BookingsController(BookingBLL bookingBLL)
    {
        _bookingBLL = bookingBLL;
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

        var result = await _bookingBLL.CreateBookingAsync(userId, dto);
        return StatusCode(201, result);
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

        var result = await _bookingBLL.GetUserBookingsAsync(userId);
        return Ok(result);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAllBookings()
    {
        var result = await _bookingBLL.GetAllBookingsAsync();
        return Ok(result);
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
}
