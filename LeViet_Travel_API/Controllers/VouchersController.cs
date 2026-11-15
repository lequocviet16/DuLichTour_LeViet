using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Common;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VouchersController : ControllerBase
{
    private readonly LeVietTravelDbContext _context;

    public VouchersController(LeVietTravelDbContext context)
    {
        _context = context;
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublicVouchers()
    {
        var now = DateTime.UtcNow;
        var vouchers = await _context.Vouchers
            .Where(v => v.IsActive && v.StartDate <= now && v.EndDate >= now && v.UsedCount < v.MaxUsage)
            .OrderByDescending(v => v.DiscountValue)
            .AsNoTracking()
            .ToListAsync();

        return Ok(ApiResponse.Ok(vouchers));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVouchers()
    {
        var vouchers = await _context.Vouchers.AsNoTracking().ToListAsync();
        return Ok(ApiResponse.Ok(vouchers));
    }
}
