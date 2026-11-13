using LeVietTravel.BookingService.Entities;

namespace LeVietTravel.BookingService.Data;

public static class BookingDbInitializer
{
    public static void Initialize(BookingDbContext context)
    {
        try
        {
            context.Database.EnsureCreated();
        }
        catch { }

        if (!context.Bookings.Any())
        {
            context.Bookings.Add(new Booking
            {
                BookingCode = "BK-202611-001",
                UserId = 2, // An (Customer)
                ContactName = "Nguyễn Văn An",
                ContactEmail = "an@example.com",
                ContactPhone = "0901234567",
                TourId = 1,
                TourTitle = "Tour Khám Phá Vịnh Hạ Long 3N2Đ - Du Thuyền 5 Sao",
                DepartureId = 1,
                StartDate = DateTime.UtcNow.AddDays(7),
                AdultsCount = 2,
                ChildrenCount = 0,
                TotalPrice = 6900000,
                DiscountAmount = 0,
                FinalAmount = 6900000,
                Status = "CONFIRMED",
                VietQrUrl = "https://img.vietqr.io/image/MB-0988888888-compact2.png?amount=6900000&addInfo=BK202611001",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });

            context.SaveChanges();
        }
    }
}
