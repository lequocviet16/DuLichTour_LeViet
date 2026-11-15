using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL;

public class LeVietTravelDbContext : DbContext
{
    public LeVietTravelDbContext(DbContextOptions<LeVietTravelDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourDeparture> TourDepartures => Set<TourDeparture>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasOne(t => t.Category)
                  .WithMany(c => c.Tours)
                  .HasForeignKey(t => t.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TourDeparture>(entity =>
        {
            entity.HasOne(d => d.Tour)
                  .WithMany(t => t.Departures)
                  .HasForeignKey(d => d.TourId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne(r => r.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
