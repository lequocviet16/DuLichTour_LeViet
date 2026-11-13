using LeVietTravel.BookingService.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeVietTravel.BookingService.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(b => b.BookingCode).IsUnique();
            entity.HasIndex(b => b.UserId);
        });
    }
}
