using LeVietTravel.TourService.Entities;

namespace LeVietTravel.TourService.Data;

public static class TourDbInitializer
{
    public static void Initialize(TourDbContext context)
    {
        try
        {
            context.Database.EnsureCreated();
        }
        catch { }

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
                    DestinationName = "Sa Pa, Lào Cai",
                    ImageUrl = "https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=800&q=80",
                    MaxCapacity = 30,
                    Status = "ACTIVE",
                    CategoryId = catNui.Id,
                    Departures = new List<TourDeparture>
                    {
                        new TourDeparture { StartDate = DateTime.UtcNow.AddDays(4), EndDate = DateTime.UtcNow.AddDays(6), Price = 2890000, TotalSeats = 30, BookedSeats = 15 }
                    }
                }
            };

            context.Tours.AddRange(tours);
            context.SaveChanges();
        }
    }
}
