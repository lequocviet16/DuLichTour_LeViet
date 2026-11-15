using DAL;
using Models.Common;
using Models.DTOs;
using Models.Entities;

namespace BLL;

public class TourBLL
{
    private readonly TourDAL _tourDAL;

    public TourBLL(TourDAL tourDAL)
    {
        _tourDAL = tourDAL;
    }

    public async Task<ApiResponse<object>> GetToursAsync(TourFilterDto filter)
    {
        var (items, totalItems) = await _tourDAL.GetToursAsync(filter);
        var page = filter.Page < 1 ? 1 : filter.Page;
        var limit = filter.Limit < 1 ? 10 : filter.Limit;
        var totalPages = (int)Math.Ceiling(totalItems / (double)limit);

        var tourDtos = items.Select(t => new TourDto
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
            CategoryName = t.Category?.Name,
            Departures = t.Departures.Select(d => new TourDepartureDto
            {
                Id = d.Id,
                TourId = d.TourId,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Price = d.Price,
                TotalSeats = d.TotalSeats,
                BookedSeats = d.BookedSeats,
                Status = d.Status
            }).ToList()
        }).ToList();

        var result = new
        {
            items = tourDtos,
            pagination = new
            {
                page,
                limit,
                totalItems,
                totalPages
            }
        };

        return ApiResponse<object>.Ok(result);
    }

    public async Task<ApiResponse<TourDto>> GetTourByIdOrSlugAsync(string idOrSlug)
    {
        var tour = await _tourDAL.GetTourByIdOrSlugAsync(idOrSlug);
        if (tour == null)
        {
            return ApiResponse<TourDto>.Fail("Không tìm thấy thông tin tour du lịch.", 404);
        }

        return ApiResponse<TourDto>.Ok(MapToDto(tour));
    }

    public async Task<ApiResponse<List<TourDto>>> GetRecommendationsAsync(int tourId)
    {
        var tours = await _tourDAL.GetRecommendationsAsync(tourId);
        return ApiResponse<List<TourDto>>.Ok(tours.Select(MapToDto).ToList());
    }

    private static TourDto MapToDto(Tour tour)
    {
        return new TourDto
        {
            Id = tour.Id,
            Title = tour.Title,
            Slug = tour.Slug,
            Code = tour.Code,
            Summary = tour.Summary,
            Description = tour.Description,
            Price = tour.Price,
            DiscountPrice = tour.DiscountPrice,
            DurationDays = tour.DurationDays,
            DurationNights = tour.DurationNights,
            DestinationName = tour.DestinationName,
            ImageUrl = tour.ImageUrl,
            MaxCapacity = tour.MaxCapacity,
            Status = tour.Status,
            CategoryId = tour.CategoryId,
            CategoryName = tour.Category?.Name,
            Departures = tour.Departures.Select(d => new TourDepartureDto
            {
                Id = d.Id,
                TourId = d.TourId,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Price = d.Price,
                TotalSeats = d.TotalSeats,
                BookedSeats = d.BookedSeats,
                Status = d.Status
            }).ToList()
        };
    }

    public async Task<ApiResponse<TourDto>> GetTourByIdAsync(int id)
    {
        var tour = await _tourDAL.GetTourByIdAsync(id);
        if (tour == null)
        {
            return ApiResponse<TourDto>.Fail("Không tìm thấy thông tin tour du lịch.", 404);
        }

        return ApiResponse<TourDto>.Ok(MapToDto(tour));
    }

    public async Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync()
    {
        var cats = await _tourDAL.GetCategoriesAsync();
        var dtos = cats.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description
        }).ToList();

        return ApiResponse<List<CategoryDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<TourDto>> CreateTourAsync(CreateTourDto dto)
    {
        var slug = GenerateSlug(dto.Title);
        var tour = new Tour
        {
            Title = dto.Title.Trim(),
            Slug = slug,
            Code = string.IsNullOrWhiteSpace(dto.Code) ? $"TOUR-{new Random().Next(100, 999)}" : dto.Code.Trim(),
            Summary = dto.Summary?.Trim() ?? string.Empty,
            Description = dto.Description?.Trim() ?? string.Empty,
            Price = dto.Price,
            DiscountPrice = dto.DiscountPrice,
            DurationDays = dto.DurationDays,
            DurationNights = dto.DurationNights,
            DestinationName = dto.DestinationName?.Trim() ?? string.Empty,
            ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? "https://images.unsplash.com/photo-1528127269322-539801943592?w=800" : dto.ImageUrl.Trim(),
            CategoryId = dto.CategoryId,
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow,
            Departures = new List<TourDeparture>
            {
                new TourDeparture
                {
                    StartDate = DateTime.UtcNow.AddDays(7),
                    EndDate = DateTime.UtcNow.AddDays(7 + dto.DurationDays),
                    Price = dto.DiscountPrice ?? dto.Price,
                    TotalSeats = 30,
                    BookedSeats = 0,
                    Status = "AVAILABLE"
                }
            }
        };

        await _tourDAL.CreateTourAsync(tour);
        return await GetTourByIdAsync(tour.Id);
    }

    public async Task<ApiResponse<object>> DeleteTourAsync(int id)
    {
        var existing = await _tourDAL.GetTourByIdAsync(id);
        if (existing == null)
        {
            return ApiResponse.Fail("Không tìm thấy tour cần xóa.", 404);
        }

        await _tourDAL.DeleteTourAsync(id);
        return ApiResponse.Ok(null, "Xóa tour thành công!");
    }

    private string GenerateSlug(string title)
    {
        var slug = title.ToLower().Trim();
        var unaccented = System.Text.RegularExpressions.Regex.Replace(slug, @"[áàảãạăắằẳẵặâấầẩẫậ]", "a");
        unaccented = System.Text.RegularExpressions.Regex.Replace(unaccented, @"[éèẻẽẹêếềểễệ]", "e");
        unaccented = System.Text.RegularExpressions.Regex.Replace(unaccented, @"[íìỉĩị]", "i");
        unaccented = System.Text.RegularExpressions.Regex.Replace(unaccented, @"[óòỏõọôốồổỗộơớờởỡợ]", "o");
        unaccented = System.Text.RegularExpressions.Regex.Replace(unaccented, @"[úùủũụưứừửữự]", "u");
        unaccented = System.Text.RegularExpressions.Regex.Replace(unaccented, @"[ýỳỷỹỵ]", "y");
        unaccented = System.Text.RegularExpressions.Regex.Replace(unaccented, @"[đ]", "d");
        unaccented = System.Text.RegularExpressions.Regex.Replace(unaccented, @"[^a-z0-9\s-]", "");
        unaccented = System.Text.RegularExpressions.Regex.Replace(unaccented, @"\s+", "-").Trim('-');
        return $"{unaccented}-{DateTime.UtcNow.Ticks % 10000}";
    }
}
