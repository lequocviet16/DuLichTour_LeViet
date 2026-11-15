using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Models.Entities;

namespace DAL;

public class TourDAL
{
    private readonly LeVietTravelDbContext _context;

    public TourDAL(LeVietTravelDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Tour> Items, int TotalItems)> GetToursAsync(TourFilterDto filter)
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

        var page = filter.Page < 1 ? 1 : filter.Page;
        var limit = filter.Limit < 1 ? 10 : filter.Limit;

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task<Tour?> GetTourByIdAsync(int id)
    {
        return await _context.Tours
            .Include(t => t.Category)
            .Include(t => t.Departures)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tour?> GetTourByIdOrSlugAsync(string idOrSlug)
    {
        if (int.TryParse(idOrSlug, out var id))
        {
            var byId = await GetTourByIdAsync(id);
            if (byId != null) return byId;
        }

        return await _context.Tours
            .Include(t => t.Category)
            .Include(t => t.Departures)
            .FirstOrDefaultAsync(t => t.Slug == idOrSlug || t.Code == idOrSlug);
    }

    public async Task<List<Tour>> GetRecommendationsAsync(int tourId, int limit = 3)
    {
        return await _context.Tours
            .Where(t => t.Id != tourId)
            .Include(t => t.Category)
            .Include(t => t.Departures)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Tour> CreateTourAsync(Tour tour)
    {
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();
        return tour;
    }

    public async Task UpdateTourAsync(Tour tour)
    {
        _context.Tours.Update(tour);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTourAsync(int id)
    {
        var tour = await _context.Tours.FindAsync(id);
        if (tour != null)
        {
            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
        }
    }
}
