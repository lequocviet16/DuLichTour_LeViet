using LeVietTravel.TourService.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeVietTravel.TourService.Data;

public class TourDbContext : DbContext
{
    public TourDbContext(DbContextOptions<TourDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourDeparture> TourDepartures => Set<TourDeparture>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasIndex(t => t.Slug);
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
    }
}
