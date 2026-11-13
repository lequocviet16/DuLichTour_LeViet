using LeVietTravel.Common.Constants;
using LeVietTravel.Common.Models;
using LeVietTravel.TourService.Data;
using LeVietTravel.TourService.DTOs;
using LeVietTravel.TourService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeVietTravel.TourService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToursController : ControllerBase
{
    private readonly TourDbContext _context;

    public ToursController(TourDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTours([FromQuery] TourFilterDto filter)
    {
        var query = _context.Tours
            .Include(t => t.Category)
            .Include(t => t.Departures)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var kw = filter.Keyword.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(kw) || t.DestinationName.ToLower().Contains(kw) || t.Summary.ToLower().Contains(kw));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(t => (t.DiscountPrice ?? t.Price) >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(t => (t.DiscountPrice ?? t.Price) <= filter.MaxPrice.Value);
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)filter.Limit);

        var tours = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((filter.Page - 1) * filter.Limit)
            .Take(filter.Limit)
            .Select(t => new TourDto
            {
                Id = t.Id,
                Title = t.Title,
                Slug = t.Slug,
                Code = t.Code,
                Summary = t.Summary,
                Description = t.Description,
                Price = t.Price,
                DiscountPrice = t.DiscountPrice,
                DurationDays = t.DurationDays,
                DurationNights = t.DurationNights,
                DestinationName = t.DestinationName,
                ImageUrl = t.ImageUrl,
                MaxCapacity = t.MaxCapacity,
                Status = t.Status,
                CategoryId = t.CategoryId,
                CategoryName = t.Category != null ? t.Category.Name : string.Empty,
                Departures = t.Departures.Select(d => new TourDepartureDto
                {
                    Id = d.Id,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    Price = d.Price,
                    TotalSeats = d.TotalSeats,
                    BookedSeats = d.BookedSeats
                }).ToList()
            })
            .ToListAsync();

        return Ok(ApiResponse<List<TourDto>>.Ok(tours, "Lấy danh sách tour thành công.", new
        {
            page = filter.Page,
            limit = filter.Limit,
            totalItems,
            totalPages
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTourById(int id)
    {
        var t = await _context.Tours
            .Include(t => t.Category)
            .Include(t => t.Departures)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (t == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy tour du lịch yêu cầu."));
        }

        var dto = new TourDto
        {
            Id = t.Id,
            Title = t.Title,
            Slug = t.Slug,
            Code = t.Code,
            Summary = t.Summary,
            Description = t.Description,
            Price = t.Price,
            DiscountPrice = t.DiscountPrice,
            DurationDays = t.DurationDays,
            DurationNights = t.DurationNights,
            DestinationName = t.DestinationName,
            ImageUrl = t.ImageUrl,
            MaxCapacity = t.MaxCapacity,
            Status = t.Status,
            CategoryId = t.CategoryId,
            CategoryName = t.Category != null ? t.Category.Name : string.Empty,
            Departures = t.Departures.Select(d => new TourDepartureDto
            {
                Id = d.Id,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Price = d.Price,
                TotalSeats = d.TotalSeats,
                BookedSeats = d.BookedSeats
            }).ToList()
        };

        return Ok(ApiResponse<TourDto>.Ok(dto, "Lấy thông tin chi tiết tour thành công."));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                TourCount = c.Tours.Count
            })
            .ToListAsync();

        return Ok(ApiResponse<List<CategoryDto>>.Ok(categories, "Lấy danh mục thành công."));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateTour([FromBody] CreateTourDto dto)
    {
        var slug = dto.Title.ToLower()
            .Replace(" ", "-")
            .Replace("đ", "d")
            .Replace("à", "a").Replace("á", "a").Replace("ạ", "a").Replace("ả", "a").Replace("ã", "a");

        var tour = new Tour
        {
            Title = dto.Title.Trim(),
            Slug = slug + "-" + Guid.NewGuid().ToString("N")[..6],
            Code = "TOUR-" + new Random().Next(1000, 9999),
            Summary = dto.Summary ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            Price = dto.Price,
            DiscountPrice = dto.DiscountPrice,
            DurationDays = dto.DurationDays,
            DurationNights = dto.DurationNights,
            DestinationName = dto.DestinationName,
            ImageUrl = dto.ImageUrl ?? "https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=800&q=80",
            CategoryId = dto.CategoryId,
            MaxCapacity = dto.MaxCapacity,
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow
        };

        // Add a default departure 7 days from now
        tour.Departures.Add(new TourDeparture
        {
            StartDate = DateTime.UtcNow.AddDays(7),
            EndDate = DateTime.UtcNow.AddDays(7 + dto.DurationDays),
            Price = dto.DiscountPrice ?? dto.Price,
            TotalSeats = dto.MaxCapacity,
            BookedSeats = 0,
            Status = "AVAILABLE"
        });

        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        return StatusCode(201, ApiResponse<Tour>.Ok(tour, "Thêm mới tour du lịch thành công (Quyền Quản Trị Viên)."));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTour(int id)
    {
        var tour = await _context.Tours.FindAsync(id);
        if (tour == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy tour cần xóa."));
        }

        _context.Tours.Remove(tour);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Ok("Xóa tour thành công (Quyền Quản Trị Viên)."));
    }
}
