using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Common;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DestinationsController : ControllerBase
{
    private readonly LeVietTravelDbContext _context;

    public DestinationsController(LeVietTravelDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDestinations([FromQuery] int page = 1, [FromQuery] int limit = 100)
    {
        var query = _context.Destinations.AsNoTracking();
        var totalItems = await query.CountAsync();
        var items = await query
            .OrderBy(d => d.Name)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        var result = new
        {
            items,
            pagination = new
            {
                page,
                limit,
                totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)limit)
            }
        };

        return Ok(ApiResponse.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dest = await _context.Destinations.FindAsync(id);
        if (dest == null) return NotFound(ApiResponse.Fail("Không tìm thấy điểm đến.", 404));
        return Ok(ApiResponse.Ok(dest));
    }
}
