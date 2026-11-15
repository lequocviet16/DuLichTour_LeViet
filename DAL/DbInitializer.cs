using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DAL;

public static class DbInitializer
{
    public static void Initialize(LeVietTravelDbContext context)
    {
        try
        {
            context.Database.EnsureCreated();
        }
        catch
        {
            // Database might already exist
        }

        // 1. Seed Users if empty
        if (!context.Users.Any())
        {
            var admin = new User
            {
                FullName = "Quản trị viên LeViet",
                Email = "admin@leviet.com",
                PasswordHash = "$2a$11$0w3fRjPjJbK49fM3yO5W1eY8M3sU6B7tZ4i9p1q3k2m5n7o9p1q3k", // Admin@123 (BCrypt)
                PhoneNumber = "0988888888",
                Role = "ADMIN",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var customer = new User
            {
                FullName = "Nguyễn Văn An",
                Email = "khachhang@gmail.com",
                PasswordHash = "$2a$11$0w3fRjPjJbK49fM3yO5W1eY8M3sU6B7tZ4i9p1q3k2m5n7o9p1q3k", // Customer@123
                PhoneNumber = "0901234567",
                Role = "CUSTOMER",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(admin, customer);
            context.SaveChanges();
        }

        // 2. Seed Categories and Tours if empty
        if (!context.Categories.Any())
        {
            var catBien = new Category { Name = "Du lịch Biển Đảo", Slug = "du-lich-bien-dao", Description = "Khám phá các bãi biển đẹp nhất Việt Nam" };
            var catNui = new Category { Name = "Du lịch Vùng Cao", Slug = "du-lich-vung-cao", Description = "Trải nghiệm núi rừng Tây Bắc và Tây Nguyên" };
            var catDiSan = new Category { Name = "Du lịch Di Sản & Văn Hóa", Slug = "du-lich-di-san", Description = "Hành trình di sản văn hóa miền Trung" };

            context.Categories.AddRange(catBien, catNui, catDiSan);
            context.SaveChanges();

            var tours = new List<Tour>
            {
                new Tour
                {
                    Title = "Tour Khám Phá Vịnh Hạ Long 3N2Đ - Du Thuyền 5 Sao",
                    Slug = "tour-kham-pha-vinh-ha-long-3n2d",
                    Code = "HL-3N2D",
                    Summary = "Thưởng ngoạn kỳ quan thiên nhiên thế giới trên du thuyền đẳng cấp sang trọng.",
                    Description = "Hành trình 3 ngày 2 đêm đưa quý khách tham quan Đảo Ti Tốp, Hang Sửng Sốt, chèo kayak tại Hang Luồn và thưởng thức ẩm thực hải sản thượng hạng.",
                    Price = 3850000,
                    DiscountPrice = 3450000,
                    DurationDays = 3,
                    DurationNights = 2,
                    DestinationName = "Hạ Long, Quảng Ninh",
                    ImageUrl = "https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=800&q=80",
                    MaxCapacity = 30,
                    Status = "ACTIVE",
                    CategoryId = catBien.Id,
                    Departures = new List<TourDeparture>
                    {
                        new TourDeparture { StartDate = DateTime.UtcNow.AddDays(7), EndDate = DateTime.UtcNow.AddDays(9), Price = 3450000, TotalSeats = 30, BookedSeats = 12 },
                        new TourDeparture { StartDate = DateTime.UtcNow.AddDays(14), EndDate = DateTime.UtcNow.AddDays(16), Price = 3450000, TotalSeats = 30, BookedSeats = 5 }
                    }
                },
                new Tour
                {
                    Title = "Tour Đà Nẵng - Bà Nà Hills - Cầu Vàng - Phố Cổ Hội An 4N3Đ",
                    Slug = "tour-da-nang-ba-na-hills-hoi-an-4n3d",
                    Code = "DN-4N3D",
                    Summary = "Hành trình di sản miền Trung hấp dẫn với Cầu Vàng và phố cổ lung linh.",
                    Description = "Khám phá danh thắng Ngũ Hành Sơn, Bà Nà Hills bồng lai tiên cảnh, check-in Cầu Vàng nổi tiếng thế giới và thả đèn hoa đăng lung linh trên sông Hoài Hội An.",
                    Price = 4690000,
                    DiscountPrice = 4190000,
                    DurationDays = 4,
                    DurationNights = 3,
                    DestinationName = "Đà Nẵng - Hội An",
                    ImageUrl = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=800&q=80",
                    MaxCapacity = 35,
                    Status = "ACTIVE",
                    CategoryId = catDiSan.Id,
                    Departures = new List<TourDeparture>
                    {
                        new TourDeparture { StartDate = DateTime.UtcNow.AddDays(5), EndDate = DateTime.UtcNow.AddDays(8), Price = 4190000, TotalSeats = 35, BookedSeats = 18 },
                        new TourDeparture { StartDate = DateTime.UtcNow.AddDays(12), EndDate = DateTime.UtcNow.AddDays(15), Price = 4190000, TotalSeats = 35, BookedSeats = 8 }
                    }
                },
                new Tour
                {
                    Title = "Tour Nghỉ Dưỡng Phú Quốc - Sunset Town - Cáp Treo Hòn Thơm 3N2Đ",
                    Slug = "tour-phu-quoc-sunset-town-hon-thom-3n2d",
                    Code = "PQ-3N2D",
                    Summary = "Thiên đường biển đảo ngọc, chiêm ngưỡng hoàng hôn rực rỡ nhất Việt Nam.",
                    Description = "Trải nghiệm cáp treo vượt biển dài nhất thế giới, khám phá Grand World thành phố không ngủ, ngắm hoàng hôn tại Thị trấn Hoàng Hôn lãng mạn.",
                    Price = 5200000,
                    DiscountPrice = 4750000,
                    DurationDays = 3,
                    DurationNights = 2,
                    DestinationName = "Phú Quốc, Kiên Giang",
                    ImageUrl = "https://images.unsplash.com/photo-1540555700478-4be289fbecef?auto=format&fit=crop&w=800&q=80",
                    MaxCapacity = 25,
                    Status = "ACTIVE",
                    CategoryId = catBien.Id,
                    Departures = new List<TourDeparture>
                    {
                        new TourDeparture { StartDate = DateTime.UtcNow.AddDays(6), EndDate = DateTime.UtcNow.AddDays(8), Price = 4750000, TotalSeats = 25, BookedSeats = 10 }
                    }
                },
                new Tour
                {
                    Title = "Tour Sapa - Fansipan Huyền Thoại - Bản Cát Cát 3N2Đ",
                    Slug = "tour-sapa-fansipan-ban-cat-cat-3n2d",
                    Code = "SP-3N2D",
                    Summary = "Chinh phục nóc nhà Đông Dương và hòa mình vào sắc màu văn hóa Tây Bắc.",
                    Description = "Đi tàu hỏa leo núi Mường Hoa, chinh phục đỉnh Fansipan 3.143m ngắm biển mây, ghé thăm bản Cát Cát của người H'Mông và thưởng thức lẩu cá tầm đặc sản.",
                    Price = 3190000,
                    DiscountPrice = 2890000,
                    DurationDays = 3,
                    DurationNights = 2,
                    DestinationName = "Sapa, Lào Cai",
                    ImageUrl = "https://images.unsplash.com/photo-1570789210967-2cac24afeb00?auto=format&fit=crop&w=800&q=80",
                    MaxCapacity = 30,
                    Status = "ACTIVE",
                    CategoryId = catNui.Id,
                    Departures = new List<TourDeparture>
                    {
                        new TourDeparture { StartDate = DateTime.UtcNow.AddDays(4), EndDate = DateTime.UtcNow.AddDays(6), Price = 2890000, TotalSeats = 30, BookedSeats = 15 }
                    }
                },
                new Tour
                {
                    Title = "Tour Hà Giang Mùa Hoa Tam Giác Mạch - Cột Cờ Lũng Cú 3N2Đ",
                    Slug = "tour-ha-giang-mua-hoa-tam-giac-mach-3n2d",
                    Code = "HG-3N2D",
                    Summary = "Chinh phục đèo Mã Pí Lèng hùng vĩ và dòng sông Nho Quế xanh như ngọc bích.",
                    Description = "Hành trình đưa du khách đến cực Bắc Tổ quốc, chiêm ngưỡng cánh đồng hoa tam giác mạch ngút ngàn, đi thuyền hẻm Tu Sản ngắm sông Nho Quế.",
                    Price = 3350000,
                    DiscountPrice = 2990000,
                    DurationDays = 3,
                    DurationNights = 2,
                    DestinationName = "Hà Giang - Đồng Văn",
                    ImageUrl = "https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=800&q=80",
                    MaxCapacity = 25,
                    Status = "ACTIVE",
                    CategoryId = catNui.Id,
                    Departures = new List<TourDeparture>
                    {
                        new TourDeparture { StartDate = DateTime.UtcNow.AddDays(8), EndDate = DateTime.UtcNow.AddDays(10), Price = 2990000, TotalSeats = 25, BookedSeats = 7 }
                    }
                },
                new Tour
                {
                    Title = "Tour Đà Lạt Thành Phố Ngàn Hoa - Săn Mây Cầu Đất 3N2Đ",
                    Slug = "tour-da-lat-thanh-pho-ngan-hoa-3n2d",
                    Code = "DL-3N2D",
                    Summary = "Tận hưởng không khí se lạnh, đắm chìm trong hương hoa và cà phê ngắm mây.",
                    Description = "Tham quan Quảng trường Lâm Viên, Thung Lũng Tình Yêu, đồi chè Cầu Đất săn mây sáng sớm và thưởng thức ẩm thực chợ đêm Đà Lạt ấm áp.",
                    Price = 2890000,
                    DiscountPrice = 2550000,
                    DurationDays = 3,
                    DurationNights = 2,
                    DestinationName = "Đà Lạt, Lâm Đồng",
                    ImageUrl = "https://images.unsplash.com/photo-1509316975850-ff9c5deb0cd9?auto=format&fit=crop&w=800&q=80",
                    MaxCapacity = 30,
                    Status = "ACTIVE",
                    CategoryId = catDiSan.Id,
                    Departures = new List<TourDeparture>
                    {
                        new TourDeparture { StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(5), Price = 2550000, TotalSeats = 30, BookedSeats = 20 }
                    }
                }
            };

            context.Tours.AddRange(tours);
            context.SaveChanges();
        }

        // 3. Seed sample bookings if empty
        if (!context.Bookings.Any())
        {
            var firstTour = context.Tours.FirstOrDefault();
            if (firstTour != null)
            {
                var sampleBooking = new Booking
                {
                    BookingCode = "BK-DEMO-001",
                    UserId = 2,
                    ContactName = "Nguyễn Văn An",
                    ContactEmail = "khachhang@gmail.com",
                    ContactPhone = "0901234567",
                    TourId = firstTour.Id,
                    TourTitle = firstTour.Title,
                    DepartureId = 1,
                    StartDate = DateTime.UtcNow.AddDays(7),
                    AdultsCount = 2,
                    ChildrenCount = 1,
                    TotalPrice = 8625000,
                    DiscountAmount = 0,
                    FinalAmount = 8625000,
                    Status = "CONFIRMED",
                    VietQrUrl = "https://img.vietqr.io/image/MB-0988888888-compact2.png?amount=8625000&addInfo=BK-DEMO-001%200901234567",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                };

                context.Bookings.Add(sampleBooking);
                context.SaveChanges();
            }
        }
    }
}
