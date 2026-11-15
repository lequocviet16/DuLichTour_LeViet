using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.Entities;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeparturesController : ControllerBase
{
    private readonly LeVietTravelDbContext _context;

    public DeparturesController(LeVietTravelDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDepartures([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _context.TourDepartures.Include(d => d.Tour).AsQueryable();
        var totalItems = await query.CountAsync();
        var departures = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();

        var items = departures.Select(d => new
        {
            id = d.Id,
            tourId = d.TourId,
            tour = d.Tour != null ? new
            {
                id = d.Tour.Id,
                code = d.Tour.Code,
                name = d.Tour.Title,
                thumbnail = d.Tour.ImageUrl
            } : null,
            startDate = d.StartDate.ToString("yyyy-MM-dd"),
            endDate = d.EndDate.ToString("yyyy-MM-dd"),
            capacity = d.TotalSeats,
            bookedSeats = d.BookedSeats,
            adultPrice = (double)d.Price,
            childPrice = (double)d.Price * 0.75,
            departureCity = "Hà Nội / TP.HCM",
            status = d.Status == "AVAILABLE" ? "OPEN" : d.Status
        }).ToList();

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

        return Ok(ApiResponse.Ok(result, "Lấy danh sách khởi hành thành công"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var d = await _context.TourDepartures.Include(x => x.Tour).FirstOrDefaultAsync(x => x.Id == id);
        if (d == null) return NotFound(ApiResponse.Fail("Không tìm thấy đợt khởi hành", 404));

        return Ok(ApiResponse.Ok(new
        {
            id = d.Id,
            tourId = d.TourId,
            tour = d.Tour != null ? new { id = d.Tour.Id, code = d.Tour.Code, name = d.Tour.Title } : null,
            startDate = d.StartDate.ToString("yyyy-MM-dd"),
            endDate = d.EndDate.ToString("yyyy-MM-dd"),
            capacity = d.TotalSeats,
            bookedSeats = d.BookedSeats,
            adultPrice = (double)d.Price,
            childPrice = (double)d.Price * 0.75,
            status = d.Status == "AVAILABLE" ? "OPEN" : d.Status
        }, "Lấy chi tiết khởi hành thành công"));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TourDeparture model)
    {
        _context.TourDepartures.Add(model);
        await _context.SaveChangesAsync();
        return StatusCode(201, ApiResponse.Ok(model, "Mở chuyến khởi hành thành công"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TourDeparture model)
    {
        var existing = await _context.TourDepartures.FindAsync(id);
        if (existing == null) return NotFound(ApiResponse.Fail("Không tìm thấy đợt khởi hành", 404));

        existing.StartDate = model.StartDate;
        existing.EndDate = model.EndDate;
        existing.Price = model.Price;
        existing.TotalSeats = model.TotalSeats;
        existing.Status = model.Status;

        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Ok(existing, "Cập nhật đợt khởi hành thành công"));
    }
}
