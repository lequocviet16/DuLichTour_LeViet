using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Common;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly LeVietTravelDbContext _context;

    public UsersController(LeVietTravelDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] string? role, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _context.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(u => u.Role == role);
        }

        var totalItems = await query.CountAsync();
        var items = await query.Skip((page - 1) * limit).Take(limit).Select(u => new
        {
            id = u.Id,
            fullName = u.FullName,
            email = u.Email,
            phoneNumber = u.PhoneNumber,
            role = u.Role,
            isActive = u.IsActive,
            createdAt = u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToListAsync();

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

        return Ok(ApiResponse.Ok(result, "Lấy danh sách người dùng thành công"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound(ApiResponse.Fail("Không tìm thấy người dùng", 404));

        var bookings = await _context.Bookings.Where(b => b.UserId == id).ToListAsync();

        return Ok(ApiResponse.Ok(new
        {
            id = user.Id,
            fullName = user.FullName,
            email = user.Email,
            phoneNumber = user.PhoneNumber,
            role = user.Role,
            isActive = user.IsActive,
            createdAt = user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            bookings = bookings.Select(b => new
            {
                id = b.Id,
                bookingCode = b.BookingCode,
                bookingDate = b.CreatedAt.ToString("yyyy-MM-dd"),
                finalAmount = (double)b.TotalPrice,
                status = b.Status
            })
        }, "Lấy thông tin người dùng thành công"));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateUserStatusRequest req)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound(ApiResponse.Fail("Không tìm thấy người dùng", 404));

        user.IsActive = req.IsActive;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Ok(user, "Cập nhật trạng thái người dùng thành công"));
    }

    public class UpdateUserStatusRequest
    {
        public bool IsActive { get; set; }
    }
}
