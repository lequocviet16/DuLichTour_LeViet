using LeVietTravel.AuthService.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeVietTravel.AuthService.Data;

public static class DbInitializer
{
    public static void Initialize(AuthDbContext context)
    {
        try
        {
            context.Database.EnsureCreated();
        }
        catch
        {
            // If already created or migration handled
        }

        if (!context.Users.Any())
        {
            var admin = new User
            {
                FullName = "Quản trị viên LeViet",
                Email = "admin@leviet.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PhoneNumber = "0988888888",
                Role = "ADMIN",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var customer = new User
            {
                FullName = "Nguyễn Văn An",
                Email = "an@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                PhoneNumber = "0901234567",
                Role = "CUSTOMER",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(admin, customer);
            context.SaveChanges();
        }
    }
}
