using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL;

public class BookingDAL
{
    private readonly LeVietTravelDbContext _context;

    public BookingDAL(LeVietTravelDbContext context)
    {
        _context = context;
    }

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings.FindAsync(id);
    }

    public async Task<Booking?> GetByCodeAsync(string code)
    {
        return await _context.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code);
    }

    public async Task<List<Booking>> GetByUserIdAsync(int userId)
    {
        return await _context.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking != null)
        {
            booking.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}
