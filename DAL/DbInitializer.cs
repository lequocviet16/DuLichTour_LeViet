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

        // 1. Seed Users if empty or ensure correct admin password
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
                Email = "khachhang@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                PhoneNumber = "0901234567",
                Role = "CUSTOMER",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(admin, customer);
            context.SaveChanges();
        }
        else
        {
            var existingAdmin = context.Users.FirstOrDefault(u => u.Email == "admin@leviet.com");
            if (existingAdmin != null)
            {
                existingAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                existingAdmin.Role = "ADMIN";
                existingAdmin.IsActive = true;
                context.SaveChanges();
            }
        }

        // 1.5. Seed Destinations & Vouchers if empty
        if (!context.Destinations.Any())
        {
            context.Destinations.AddRange(
                new Destination { Name = "Hạ Long, Quảng Ninh", Region = "BAC", Description = "Kỳ quan thiên nhiên thế giới với hàng nghìn đảo đá vôi kỳ vĩ.", ImageUrl = "https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=800&q=80" },
                new Destination { Name = "Đà Nẵng - Hội An", Region = "TRUNG", Description = "Thành phố đáng sống với biển Mỹ Khê và phố cổ Hội An rực rỡ đèn hoa đăng.", ImageUrl = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=800&q=80" },
                new Destination { Name = "Phú Quốc, Kiên Giang", Region = "NAM", Description = "Đảo ngọc phương Nam với biển xanh cát trắng và hoàng hôn lãng mạn.", ImageUrl = "https://images.unsplash.com/photo-1540555700478-4be289fbecef?auto=format&fit=crop&w=800&q=80" },
                new Destination { Name = "Sapa, Lào Cai", Region = "BAC", Description = "Thị trấn trong sương, đỉnh Fansipan hùng vĩ và ruộng bậc thang.", ImageUrl = "https://images.unsplash.com/photo-1570789210967-2cac24afeb00?auto=format&fit=crop&w=800&q=80" },
                new Destination { Name = "Hà Giang - Đồng Văn", Region = "BAC", Description = "Đèo Mã Pí Lèng, hẻm vực Tu Sản và mùa hoa tam giác mạch.", ImageUrl = "https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=800&q=80" },
                new Destination { Name = "Đà Lạt, Lâm Đồng", Region = "NAM", Description = "Thành phố tình yêu ngàn hoa mộng mơ và săn mây đồi chè.", ImageUrl = "https://images.unsplash.com/photo-1509316975850-ff9c5deb0cd9?auto=format&fit=crop&w=800&q=80" }
            );
            context.SaveChanges();
        }

        if (!context.Vouchers.Any())
        {
            context.Vouchers.AddRange(
                new Voucher { Code = "LEVIET100", DiscountType = "FIXED", DiscountValue = 100000, MinBookingAmount = 2000000, MaxUsage = 200, UsedCount = 12, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(3), IsActive = true },
                new Voucher { Code = "HE2026", DiscountType = "PERCENT", DiscountValue = 10, MinBookingAmount = 3000000, MaxUsage = 100, UsedCount = 45, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(2), IsActive = true },
                new Voucher { Code = "TRIANKHACHHANG", DiscountType = "FIXED", DiscountValue = 200000, MinBookingAmount = 4000000, MaxUsage = 50, UsedCount = 8, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(1), IsActive = true }
            );
            context.SaveChanges();
        }

        // 2. Ensure Categories
        var catBien = context.Categories.FirstOrDefault(c => c.Slug == "du-lich-bien-dao");
        if (catBien == null)
        {
            catBien = new Category { Name = "Du lịch Biển Đảo", Slug = "du-lich-bien-dao", Description = "Khám phá các bãi biển đẹp nhất Việt Nam" };
            context.Categories.Add(catBien);
            context.SaveChanges();
        }

        var catNui = context.Categories.FirstOrDefault(c => c.Slug == "du-lich-vung-cao");
        if (catNui == null)
        {
            catNui = new Category { Name = "Du lịch Vùng Cao", Slug = "du-lich-vung-cao", Description = "Trải nghiệm núi rừng Tây Bắc và Tây Nguyên" };
            context.Categories.Add(catNui);
            context.SaveChanges();
        }

        var catDiSan = context.Categories.FirstOrDefault(c => c.Slug == "du-lich-di-san");
        if (catDiSan == null)
        {
            catDiSan = new Category { Name = "Du lịch Di Sản & Văn Hóa", Slug = "du-lich-di-san", Description = "Hành trình di sản văn hóa miền Trung" };
            context.Categories.Add(catDiSan);
            context.SaveChanges();
        }

        // 3. Danh sách đầy đủ 6 tour với thông tin chi tiết, dịch vụ & điều kiện, lịch trình theo ngày
        var tourDefinitions = new List<Tour>
        {
            // TOUR 1: HẠ LONG
            new Tour
            {
                Code = "HL-3N2D",
                Title = "Tour Khám Phá Vịnh Hạ Long 3N2Đ - Du Thuyền 5 Sao",
                Slug = "tour-kham-pha-vinh-ha-long-3n2d",
                Summary = "Thưởng ngoạn kỳ quan thiên nhiên thế giới trên du thuyền đẳng cấp sang trọng, chèo thuyền kayak và khám phá hang động kỳ vĩ.",
                Description = "Hành trình 3 ngày 2 đêm đưa quý khách đến với Vịnh Hạ Long - di sản thiên nhiên thế giới được UNESCO công nhận. Trải nghiệm nghỉ dưỡng đẳng cấp trên du thuyền 5 sao chuẩn quốc tế, ngắm nhìn hàng nghìn hòn đảo đá vôi kỳ vĩ muôn hình vạn trạng. Quý khách sẽ được tham quan Hang Sửng Sốt - hang động lớn và đẹp nhất vịnh, chinh phục đỉnh núi Đảo Ti Tốp ngắm toàn cảnh vịnh từ trên cao, tự tay chèo thuyền kayak qua Hang Luồn khám phá hồ nước xanh biếc được bao bọc bởi vách đá dựng đứng, và thưởng thức tiệc hải sản tươi sống hảo hạng cùng dịch vụ chăm sóc tận tâm.",
                Price = 3850000,
                DiscountPrice = 3450000,
                DurationDays = 3,
                DurationNights = 2,
                DestinationName = "Hạ Long, Quảng Ninh",
                ImageUrl = "https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=1200&q=80",
                Images = "https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1643029891412-92f9a81a8c16?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1504457047772-27faf1c00561?auto=format&fit=crop&w=1200&q=80",
                VideoUrl = "https://www.youtube.com/watch?v=YHBqLih80uw",
                MaxCapacity = 30,
                Status = "ACTIVE",
                CategoryId = catBien.Id,
                DepositPercent = 20,
                IncludedServices = "• Xe du lịch cao cấp đưa đón Hà Nội - Hạ Long khứ hồi qua cao tốc\n• 02 đêm nghỉ dưỡng phòng ban công riêng trên du thuyền 5 sao chuẩn quốc tế\n• 05 bữa chính hải sản thượng hạng và buffet Á - Âu tại nhà hàng du thuyền\n• 02 bữa sáng buffet quốc tế phong phú\n• Vé tham quan trọn gói các danh thắng: Hang Sửng Sốt, Đảo Ti Tốp, Hang Luồn\n• Thuyền kayak hoặc đò nan có người chèo tham quan lòng vịnh\n• Tiệc Sunset Party ngắm hoàng hôn, trà chiều và hoa quả tươi trên boong tàu\n• Lớp học nấu ăn truyền thống và bài tập Thái Cực Quyền (Tai Chi) đón bình minh\n• Hướng dẫn viên song ngữ tận tâm, chuyên nghiệp suốt tuyến\n• Bảo hiểm du lịch nội địa mức bồi thường tối đa 100.000.000 VNĐ/khách\n• Nước suối 02 chai/người/ngày và khăn lạnh miễn phí",
                ExcludedServices = "• Thuế VAT 8% (khi quý khách có nhu cầu xuất hóa đơn đỏ công ty)\n• Phụ thu phòng đơn đối với khách đi 1 mình có nhu cầu ở riêng\n• Dịch vụ spa, massage và đồ uống tại quầy bar trên du thuyền\n• Đồ uống phát sinh trong các bữa ăn (bia, rượu, nước ngọt gọi thêm)\n• Chi phí cá nhân: giặt là, điện thoại, mua hải sản và quà lưu niệm\n• Tiền bồi dưỡng (Tip) cho lái xe, hướng dẫn viên và thủy thủ đoàn",
                CancellationPolicy = "• Hủy trước ngày khởi hành 07 ngày: Hoàn tiền 100% không mất phí\n• Hủy từ 04 đến 06 ngày trước khởi hành: Phí hủy là 50% tổng giá trị tour\n• Hủy từ 01 đến 03 ngày trước khởi hành: Phí hủy là 80% tổng giá trị tour\n• Hủy trong vòng 24 giờ trước khởi hành hoặc không có mặt (No-show): Phí hủy 100%",
                ChangePolicy = "• Hỗ trợ dời ngày khởi hành 01 lần miễn phí trước ít nhất 05 ngày (tùy thuộc tình trạng phòng du thuyền)\n• Đổi ngày từ 02 đến 04 ngày trước ngày đi: Phụ thu 20% giá tour\n• Dưới 02 ngày trước khởi hành: Áp dụng theo biểu phí hủy tour",
                ChildPolicy = "• Trẻ em từ 10 tuổi trở lên: Tính 100% giá vé người lớn\n• Trẻ em từ 05 đến 09 tuổi: Tính 75% giá vé người lớn (suất ăn riêng, ghế ngồi riêng, ngủ chung giường bố mẹ)\n• Trẻ em yêu cầu kê thêm giường phụ tính 90% giá vé người lớn",
                InfantPolicy = "• Trẻ em dưới 05 tuổi: Miễn phí giá tour (bố mẹ tự chi trả các chi phí phát sinh nếu có)\n• Mỗi gia đình chỉ kèm tối đa 01 bé miễn phí; từ bé thứ 2 trở đi phụ thu 50% giá tour",
                SeniorPolicy = "• Du khách từ 60 tuổi trở lên: Bố trí phòng nghỉ tầng thấp thuận tiện đi lại, ưu tiên ghế ngồi đầu trên xe và có chế độ chăm sóc y tế dự phòng chu đáo",
                PickupPoints = "Nhà hát Lớn Hà Nội, Số 1 Tràng Tiền; Sân bay Quốc tế Nội Bài; Cảng tàu khách Quốc tế Tuần Châu, Hạ Long",
                MeetingInstructions = "Quý khách vui lòng có mặt tại điểm hẹn trước giờ khởi hành ít nhất 30 phút. Vui lòng mang theo CMND/CCCD hoặc Hộ chiếu gốc còn hạn để làm thủ tục lưu trú trên vịnh.",
                LuggagePolicy = "Tối đa 20kg hành lý ký gửi và 07kg hành lý xách tay gọn nhẹ. Khuyến khích mang giày mềm thể thao, kính mát, đồ bơi và kem chống nắng.",
                InsuranceDetails = "Bảo hiểm du lịch toàn diện Bảo Việt với mức bảo vệ tối đa 100.000.000 VNĐ/người/vụ đối với mọi sự cố y tế trong suốt hành trình.",
                TermsAndConditions = "Thứ tự các điểm tham quan có thể thay đổi linh hoạt theo sự điều động của Ban quản lý Vịnh Hạ Long và điều kiện thủy triều hoặc thời tiết thực tế nhưng vẫn cam kết đi đầy đủ các điểm trong chương trình.",
                Departures = new List<TourDeparture>
                {
                    new TourDeparture { StartDate = DateTime.UtcNow.AddDays(7), EndDate = DateTime.UtcNow.AddDays(9), Price = 3450000, TotalSeats = 30, BookedSeats = 12 },
                    new TourDeparture { StartDate = DateTime.UtcNow.AddDays(14), EndDate = DateTime.UtcNow.AddDays(16), Price = 3450000, TotalSeats = 30, BookedSeats = 5 }
                },
                Itineraries = new List<TourItinerary>
                {
                    new TourItinerary
                    {
                        DayNumber = 1,
                        Title = "Hà Nội - Hạ Long - Check-in Du Thuyền 5 Sao - Hang Sửng Sốt",
                        Description = "08h00: Xe limousine đón quý khách tại Nhà hát Lớn Hà Nội khởi hành đi Hạ Long qua tuyến cao tốc Hà Nội - Hải Phòng hiện đại. 11h30: Đến Cảng tàu khách Quốc tế Tuần Châu, làm thủ tục lên du thuyền 5 sao đẳng cấp, thưởng thức đồ uống chào mừng Welcome Drink và nhận phòng ban công riêng ngắm vịnh. 13h00: Dùng bữa trưa buffet hải sản trong khi du thuyền bắt đầu rẽ sóng lướt qua hàng nghìn đảo đá vôi kỳ vĩ. 15h00: Tham quan Hang Sửng Sốt - hang động rộng và lung linh bậc nhất vịnh với hệ thống thạch nhũ triệu năm tuổi. 17h30: Tham gia Sunset Party ngắm hoàng hôn rực rỡ buông xuống mặt vịnh, thưởng thức rượu vang nhẹ và hoa quả tươi, thư giãn tại bể sục Jacuzzi. 19h00: Dùng bữa tối lãng mạn Fine Dining tại nhà hàng. Buổi tối tự do thử tài câu mực đêm hoặc hát karaoke."
                    },
                    new TourItinerary
                    {
                        DayNumber = 2,
                        Title = "Đảo Ti Tốp - Chèo Thuyền Kayak Hang Luồn - Hoàng Hôn Vịnh Biển",
                        Description = "06h30: Khởi động ngày mới với bài tập dưỡng sinh Thái Cực Quyền (Tai Chi) trên sundeck boong tàu đón bình minh trong lành. 07h30: Điểm tâm sáng buffet bánh ngọt, trà và cà phê hảo hạng. 08h30: Tàu chuyển tải đưa quý khách sang Đảo Ti Tốp - tự do đắm mình trong làn nước biển trong xanh hoặc thử thách leo hơn 400 bậc đá lên đài quan sát đỉnh núi ngắm trọn vẹn toàn cảnh kỳ quan Vịnh Hạ Long 360 độ từ trên cao. 10h30: Trải nghiệm tự tay chèo kayak hoặc ngồi đò nan xuyên qua Hang Luồn ngắm nhìn vách đá thẳng đứng và đàn khỉ hoang dã. 12h30: Dùng bữa trưa hải sản tươi sống tại nhà hàng du thuyền. Chiều: Nghỉ ngơi thư giãn trên ghế tắm nắng, tham gia lớp học làm nem cuốn truyền thống Việt Nam. 19h00: Tiệc tối BBQ hải sản nướng thơm lừng bên ánh nến lung linh trên boong tàu."
                    },
                    new TourItinerary
                    {
                        DayNumber = 3,
                        Title = "Thưởng Ngoạn Kỳ Quan - Mua Sắm Đặc Sản - Hạ Long - Hà Nội",
                        Description = "07h00: Dùng bữa sáng buffet tráng miệng nhẹ nhàng. Du thuyền nhổ neo di chuyển qua những biểu tượng trứ danh của Vịnh: Hòn Trống Mái (Gà Chọi), Hòn Đỉnh Hương, Hòn Chó Đá. 09h30: Hoàn tất thủ tục trả phòng, dùng bữa trưa sớm (Brunch) phong phú trên tàu trước khi cập bến. 11h30: Du thuyền cập Cảng Tuần Châu. Xe đưa quý khách ghé thăm xưởng chế tác ngọc trai Hạ Long và trung tâm mua sắm đặc sản chả mực giã tay làm quà cho gia đình. 14h00: Lên xe khởi hành về lại Hà Nội. 16h30: Về đến điểm đón ban đầu tại Hà Nội, HDV nói lời cảm ơn và chia tay quý khách, kết thúc chuyến tham quan trọn vẹn và nhiều kỷ niệm."
                    }
                }
            },

            // TOUR 2: ĐÀ NẴNG - BÀ NÀ - HỘI AN
            new Tour
            {
                Code = "DN-4N3D",
                Title = "Tour Đà Nẵng - Bà Nà Hills - Cầu Vàng - Phố Cổ Hội An 4N3Đ",
                Slug = "tour-da-nang-ba-na-hills-hoi-an-4n3d",
                Summary = "Hành trình di sản miền Trung hấp dẫn với Cầu Vàng, Bà Nà Hills bồng lai tiên cảnh và phố cổ Hội An lung linh đèn hoa đăng.",
                Description = "Chuyến du lịch 4 ngày 3 đêm đưa du khách khám phá cung đường di sản rực rỡ sắc màu miền Trung: từ thành phố đáng sống Đà Nẵng với bãi biển Mỹ Khê trong xanh cát trắng, bán đảo Sơn Trà linh thiêng, đến chốn bồng lai tiên cảnh Bà Nà Hills chiêm ngưỡng Cầu Vàng vươn mình giữa mây trời được truyền thông thế giới ca ngợi. Hành trình còn đưa bạn xuôi về phố cổ Hội An bên dòng sông Hoài thơ mộng, thả đèn hoa đăng cầu an và thưởng thức tinh hoa ẩm thực xứ Quảng như mì Quảng, cao lầu, bánh đập nức tiếng.",
                Price = 4690000,
                DiscountPrice = 4190000,
                DurationDays = 4,
                DurationNights = 3,
                DestinationName = "Đà Nẵng - Hội An",
                ImageUrl = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=1200&q=80",
                Images = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1603852452378-a4e8d84324a2?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1563354860-799d15199ac3?auto=format&fit=crop&w=1200&q=80",
                VideoUrl = "https://www.youtube.com/watch?v=twb6d3t1SO0",
                MaxCapacity = 35,
                Status = "ACTIVE",
                CategoryId = catDiSan.Id,
                DepositPercent = 20,
                IncludedServices = "• Xe du lịch đời mới máy lạnh đưa đón tham quan suốt tuyến\n• 03 đêm nghỉ khách sạn 4 sao trung tâm Đà Nẵng gần biển (2 khách/phòng)\n• 06 bữa ăn chính tiêu chuẩn đặc sản miền Trung (150.000 - 200.000 VNĐ/suất)\n• 03 bữa sáng buffet tại khách sạn\n• Vé cáp treo Bà Nà Hills khứ hồi và tham quan Cầu Vàng nổi tiếng thế giới\n• Vé tham quan danh thắng Ngũ Hành Sơn, Làng đá mỹ nghệ Non Nước\n• Vé tham quan trọn gói Phố Cổ Hội An: Chùa Cầu, Nhà cổ Phùng Hưng, Hội quán Phúc Kiến\n• Hướng dẫn viên địa phương nhiệt tình, chu đáo suốt chuyến đi\n• Bảo hiểm du lịch nội địa mức bồi thường 100.000.000 VNĐ/người/vụ\n• Nước suối 02 chai/người/ngày và khăn lạnh miễn phí",
                ExcludedServices = "• Vé máy bay khứ hồi đến Đà Nẵng (công ty hỗ trợ đặt vé theo giá tốt nhất)\n• Vé buffet trưa tại đỉnh Bà Nà Hills\n• Vé xem show biểu diễn Ký Ức Hội An, thuyền thả hoa đăng sông Hoài\n• Chi phí đồ uống ngoài menu, giặt là, điện thoại cá nhân\n• Tiền tip cho HDV và lái xe, thuế VAT 8%",
                CancellationPolicy = "• Hủy trước 07 ngày: Hoàn tiền 100% không mất phí\n• Hủy từ 04 - 06 ngày trước khởi hành: Phí hủy 50% tổng giá trị tour\n• Hủy từ 01 - 03 ngày trước khởi hành: Phí hủy 80% tổng giá trị tour\n• Hủy trong 24 giờ hoặc không có mặt: Phí hủy 100%",
                ChangePolicy = "• Hỗ trợ dời ngày khởi hành 01 lần miễn phí trước tối thiểu 05 ngày nếu còn phòng trống\n• Đổi từ 02 - 04 ngày: Phụ thu 20% phí đặt phòng khách sạn",
                ChildPolicy = "• Trẻ em từ 10 tuổi trở lên: Tính vé người lớn\n• Trẻ em từ 05 đến 09 tuổi: Tính 75% vé người lớn (ghế riêng, ăn riêng, ngủ chung bố mẹ). Vé cáp treo Bà Nà tính theo chiều cao thực tế tại quầy vé",
                InfantPolicy = "• Trẻ em dưới 05 tuổi: Miễn phí tour (bố mẹ tự chi trả các chi phí phát sinh nếu có). Mỗi cặp bố mẹ chỉ kèm 01 bé miễn phí",
                SeniorPolicy = "• Du khách từ 65 tuổi: Hỗ trợ xe lăn khi cần tại Bà Nà Hills và ưu tiên lối đi nhanh tại các điểm soát vé cáp treo",
                PickupPoints = "Sân bay Quốc tế Đà Nẵng; Ga Đà Nẵng; Khách sạn trung tâm thành phố Đà Nẵng; Văn phòng LeViet Travel Đà Nẵng",
                MeetingInstructions = "HDV sẽ cầm biển đón LeViet Travel tại Ga đến Sân bay Đà Nẵng trước giờ hạ cánh 15 phút. Quý khách vui lòng cung cấp mã số chuyến bay khi đặt chỗ.",
                LuggagePolicy = "Tối đa 20kg hành lý ký gửi và 7kg hành lý xách tay. Nên mang giày thể thao chống trơn trượt để đi bộ tham quan phố cổ và hang động Ngũ Hành Sơn.",
                InsuranceDetails = "Bảo hiểm du lịch Bảo Việt trọn gói tai nạn y tế lên đến 100.000.000 VNĐ/người/vụ.",
                TermsAndConditions = "Lịch trình có thể hoán đổi linh hoạt giữa các ngày tùy theo điều kiện thời tiết tại đỉnh Bà Nà nhằm đem lại trải nghiệm ngắm cảnh trọn vẹn nhất cho du khách.",
                Departures = new List<TourDeparture>
                {
                    new TourDeparture { StartDate = DateTime.UtcNow.AddDays(5), EndDate = DateTime.UtcNow.AddDays(8), Price = 4190000, TotalSeats = 35, BookedSeats = 18 },
                    new TourDeparture { StartDate = DateTime.UtcNow.AddDays(12), EndDate = DateTime.UtcNow.AddDays(15), Price = 4190000, TotalSeats = 35, BookedSeats = 8 }
                },
                Itineraries = new List<TourItinerary>
                {
                    new TourItinerary
                    {
                        DayNumber = 1,
                        Title = "Đón Sân Bay Đà Nẵng - Bán Đảo Sơn Trà - Biển Mỹ Khê",
                        Description = "Sáng: Xe đón quý khách tại Sân bay Quốc tế Đà Nẵng, đưa về khách sạn nhận phòng nghỉ ngơi. 12h00: Dùng bữa trưa đặc sản bánh tráng cuốn thịt heo hai đầu da nức tiếng. 14h30: Khởi hành đi Bán đảo Sơn Trà, viếng Chùa Linh Ứng Bãi Bụt ngắm tượng Phật Bà Quan Âm cao 67m hướng biển. 16h30: Tự do tắm biển tại Bãi biển Mỹ Khê - một trong những bãi biển đẹp nhất hành tinh. 18h30: Dùng bữa tối hải sản ven biển. Tối tự do dạo ngắm Cầu Rồng, Cầu Tình Yêu bên bờ sông Hàn thơ mộng."
                    },
                    new TourItinerary
                    {
                        DayNumber = 2,
                        Title = "Đà Nẵng - Đỉnh Bà Nà Hills - Check-in Cầu Vàng - Fantasy Park",
                        Description = "07h30: Điểm tâm sáng buffet tại khách sạn. 08h30: Khởi hành đến chân núi Bà Nà, trải nghiệm tuyến cáp treo đạt nhiều kỷ lục thế giới. Check-in Cầu Vàng độc đáo giữa làn sương mây bồng bềnh, tham quan Vườn hoa Le Jardin D'Amour, Hầm rượu Debay trăm tuổi và Chùa Linh Ứng Bà Nà. 12h00: Bữa trưa buffet quốc tế trên đỉnh núi. 13h30: Vui chơi thỏa thích tại công viên giải trí Fantasy Park với hơn 105 trò chơi hấp dẫn. 16h00: Xuống cáp treo, xe đưa đoàn về khách sạn. 19h00: Dùng bữa tối ấm cúng tại trung tâm Đà Nẵng."
                    },
                    new TourItinerary
                    {
                        DayNumber = 3,
                        Title = "Danh Thắng Ngũ Hành Sơn - Làng Đá Non Nước - Phố Cổ Hội An",
                        Description = "08h00: Xe đưa đoàn tham quan Danh thắng Ngũ Hành Sơn: Chinh phục ngọn Thủy Sơn, khám phá Động Huyền Không kỳ ảo, Chùa Tam Thai cổ kính, ghé thăm Làng điêu khắc đá mỹ nghệ Non Nước lâu đời. 12h00: Dùng bữa trưa tại nhà hàng. 14h30: Di chuyển vào Đô thị cổ Hội An. Tản bộ chiêm ngưỡng Chùa Cầu Nhật Bản, Nhà cổ Phùng Hưng, Hội quán Phúc Kiến. 17h30: Thưởng thức bữa tối với Cao Lầu và Cơm gà Hội An truyền thống. 19h00: Ngắm phố đèn lồng lung linh, đi thuyền thả hoa đăng trên sông Hoài. 21h00: Xe đưa đoàn về lại khách sạn Đà Nẵng."
                    },
                    new TourItinerary
                    {
                        DayNumber = 4,
                        Title = "Chợ Hàn Mua Sắm Đặc Sản - Tiễn Sân Bay Đà Nẵng",
                        Description = "07h30: Điểm tâm sáng buffet tại khách sạn, tự do dạo biển hoặc bơi tại hồ bơi khách sạn. 09h00: Trả phòng. Xe đưa đoàn đến Chợ Hàn hoặc Chợ Cồn - thiên đường quà đặc sản Đà Nẵng: chả bò, tré, mực một nắng, rong biển Mỹ Khê... 11h30: Thưởng thức bữa trưa nhẹ với món Mì Quảng ếch. 13h00: Xe đưa quý khách ra Sân bay Đà Nẵng, HDV hỗ trợ làm thủ tục hàng không. Kết thúc chuyến đi ý nghĩa."
                    }
                }
            },

            // TOUR 3: PHÚ QUỐC
            new Tour
            {
                Code = "PQ-3N2D",
                Title = "Tour Nghỉ Dưỡng Phú Quốc - Sunset Town - Cáp Treo Hòn Thơm 3N2Đ",
                Slug = "tour-phu-quoc-sunset-town-hon-thom-3n2d",
                Summary = "Thiên đường biển đảo ngọc phương Nam, ngắm hoàng hôn Sunset Town rực rỡ và cáp treo vượt biển kỷ lục.",
                Description = "Kỳ nghỉ 3 ngày 2 đêm tại Phú Quốc đưa du khách đến với hòn đảo ngọc quyến rũ bậc nhất Việt Nam. Chiêm ngưỡng vẻ đẹp hoang sơ của Bãi Sao với làn nước biển trong vắt như pha lê, trải nghiệm cáp treo Hòn Thơm vượt biển dài nhất thế giới, thỏa sức vui chơi tại công viên nước Aquatopia và khám phá thị trấn hoàng hôn Sunset Town mang đậm phong cách Địa Trung Hải đầy thơ mộng. Ngoài ra, chuyến đi còn kết hợp tìm hiểu các nghề truyền thống lâu đời như nuôi cấy ngọc trai, làm nước mắm cá cơm truyền thống và vườn tiêu trĩu quả.",
                Price = 5200000,
                DiscountPrice = 4750000,
                DurationDays = 3,
                DurationNights = 2,
                DestinationName = "Phú Quốc, Kiên Giang",
                ImageUrl = "https://images.unsplash.com/photo-1540555700478-4be289fbecef?auto=format&fit=crop&w=1200&q=80",
                Images = "https://images.unsplash.com/photo-1540555700478-4be289fbecef?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1526139334526-f591a54b477c?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1746292448726-9e75b5f1067d?auto=format&fit=crop&w=1200&q=80",
                VideoUrl = "https://www.youtube.com/watch?v=04Kf_0kppPM",
                MaxCapacity = 25,
                Status = "ACTIVE",
                CategoryId = catBien.Id,
                DepositPercent = 25,
                IncludedServices = "• Xe du lịch máy lạnh chất lượng cao đón tiễn sân bay và phục vụ theo chương trình\n• 02 đêm nghỉ tại resort/khách sạn 4-5 sao sang trọng gần biển (2 người/phòng)\n• 04 bữa chính tiêu chuẩn hải sản tươi sống đảo ngọc (200.000 - 250.000 VNĐ/suất)\n• 02 bữa sáng buffet tại resort\n• Vé cáp treo Hòn Thơm khứ hồi và vé vào cổng công viên nước Aquatopia\n• Vé tham quan Dinh Cậu, Bãi Sao, Vườn tiêu, Nhà thùng nước mắm, Cơ sở ngọc trai\n• Hướng dẫn viên bản địa am hiểu văn hóa và các điểm check-in đẹp\n• Bảo hiểm du lịch với hạn mức bồi thường 100.000.000 VNĐ/khách\n• Khăn lạnh và nước khoáng tinh khiết phục vụ hàng ngày",
                ExcludedServices = "• Vé máy bay khứ hồi đến Phú Quốc\n• Vé vào cổng Grand World / VinWonders / Vinpearl Safari (đăng ký theo nhu cầu)\n• Vé canô 4 đảo và dịch vụ lặn ngắm san hô bằng bình khí / Seawalker\n• Tiền tip cho HDV và lái xe, thuế VAT 8%",
                CancellationPolicy = "• Hủy trước 07 ngày: Hoàn tiền 100%\n• Hủy từ 04 - 06 ngày trước khởi hành: Phí hủy 50% tổng giá tour\n• Hủy từ 01 - 03 ngày trước khởi hành: Phí hủy 80% tổng giá tour\n• Hủy trong vòng 24 giờ: Phí hủy 100%",
                ChangePolicy = "• Dời ngày miễn phí 01 lần trước ít nhất 05 ngày. Đổi cận ngày phụ thu phí phát sinh của khách sạn theo mùa cao điểm",
                ChildPolicy = "• Trẻ em từ 10 tuổi trở lên: Tính 100% giá người lớn\n• Trẻ em từ 05 đến 09 tuổi: Tính 75% giá vé người lớn (ăn suất riêng, ngủ chung bố mẹ). Vé cáp treo Hòn Thơm áp dụng tính theo chiều cao trẻ",
                InfantPolicy = "• Trẻ em dưới 05 tuổi: Miễn phí vé tour (bố mẹ tự lo chi phí ăn uống và vé vào cổng nếu địa điểm phát sinh phụ thu theo chiều cao)",
                SeniorPolicy = "• Khách hàng từ 65 tuổi: Bố trí phòng tầng trệt, hỗ trợ đưa đón xe điện nội khu tại resort",
                PickupPoints = "Sân bay Quốc tế Phú Quốc; Cảng tàu cao tốc Bãi Vòng; Sảnh các khách sạn trung tâm thị trấn Dương Đông",
                MeetingInstructions = "Tài xế và HDV đón tại sảnh đến Sân bay Phú Quốc với biển tên LeViet Travel. Quý khách lưu ý mang CCCD còn hạn sử dụng để làm thủ tục bay.",
                LuggagePolicy = "Hành lý ký gửi 20kg + 7kg xách tay. Quý khách nên chuẩn bị đồ bơi, kính râm, kem chống nắng thân thiện với môi trường biển.",
                InsuranceDetails = "Bảo hiểm du lịch toàn diện Bảo Việt trị giá 100.000.000 VNĐ/vụ đối với mọi sự cố y tế trong chuyến đi.",
                TermsAndConditions = "Nếu thời tiết xấu biển động khiến tuyến cáp treo tạm ngừng hoạt động vì an toàn kỹ thuật, công ty sẽ hoàn tiền vé cáp treo hoặc chuyển đổi sang hoạt động tham quan tương đương.",
                Departures = new List<TourDeparture>
                {
                    new TourDeparture { StartDate = DateTime.UtcNow.AddDays(6), EndDate = DateTime.UtcNow.AddDays(8), Price = 4750000, TotalSeats = 25, BookedSeats = 10 }
                },
                Itineraries = new List<TourItinerary>
                {
                    new TourItinerary
                    {
                        DayNumber = 1,
                        Title = "Đón Sân Bay Phú Quốc - Suối Tranh - Dinh Cậu - Chợ Đêm Dương Đông",
                        Description = "Sáng: Xe đón quý khách tại Sân bay Phú Quốc, đưa về resort nhận phòng nghỉ ngơi. 12h00: Bữa trưa hải sản với món gỏi cá trích trứ danh. 14h30: Tham quan Khu du lịch sinh thái Suối Tranh rợp bóng cây xanh mát, viếng Dinh Cậu và Dinh Bà Thủy Long Thánh Mẫu biểu tượng tâm linh của ngư dân đảo ngọc. 17h30: Ngắm hoàng hôn tuyệt mỹ tại quán cà phê ven biển bãi Trường. 19h00: Dùng bữa tối, sau đó tự do hòa mình vào không khí sầm uất tại Chợ đêm Phú Quốc thưởng thức hải sản nướng, kem cuộn và mua quà lưu niệm."
                    },
                    new TourItinerary
                    {
                        DayNumber = 2,
                        Title = "Nam Đảo - Cáp Treo Hòn Thơm - Bãi Sao - Thị Trấn Hoàng Hôn Sunset Town",
                        Description = "07h30: Điểm tâm buffet tại resort. 08h30: Xe đưa đoàn đến Ga Ánh Dương, lên Cáp treo Hòn Thơm vượt biển ngắm nhìn toàn cảnh quần đảo An Thới xanh biếc từ trên cao. Thỏa sức vui chơi tại công viên nước hiện đại Aquatopia. 12h30: Bữa trưa buffet tại nhà hàng Hòn Thơm. 14h30: Trở lại bờ, xe đưa đoàn đến Bãi Sao - bãi biển đẹp nhất Phú Quốc với bờ cát trắng mịn như kem và rặng dừa xanh nghiêng bóng. 17h00: Check-in Thị trấn Hoàng Hôn Sunset Town, ngắm Cầu Hôn (Kiss Bridge) biểu tượng tình yêu lãng mạn. 19h30: Dùng bữa tối và xem show trình diễn ánh sáng nhạc nước Kiss of The Sea."
                    },
                    new TourItinerary
                    {
                        DayNumber = 3,
                        Title = "Khám Phá Làng Nghề Truyền Thống - Mua Sắm Đặc Sản - Tiễn Sân Bay",
                        Description = "08h00: Sau bữa sáng, quý khách tự do tắm biển resort. 09h30: Trả phòng, tham quan Cơ sở nuôi cấy ngọc trai Phú Quốc cao cấp, tiếp tục ghé Vườn tiêu Suối Đá và Nhà thùng nước mắm truyền thống lâu đời tìm hiểu quy trình ủ chượp cá cơm tạo nên giọt nước mắm trứ danh. 12h00: Dùng bữa trưa với món bún quậy Kiến Xây nổi tiếng. 13h30: Xe đưa đoàn ra Sân bay Phú Quốc làm thủ tục về lại điểm đến ban đầu. Chào tạm biệt và hẹn gặp lại."
                    }
                }
            },

            // TOUR 4: SAPA
            new Tour
            {
                Code = "SP-3N2D",
                Title = "Tour Sapa - Fansipan Huyền Thoại - Bản Cát Cát 3N2Đ",
                Slug = "tour-sapa-fansipan-ban-cat-cat-3n2d",
                Summary = "Chinh phục Nóc nhà Đông Dương hùng vĩ, ngắm biển mây bồng bềnh và hòa mình vào sắc màu văn hóa Tây Bắc.",
                Description = "Hành trình 3 ngày 2 đêm đưa quý khách đến với thị trấn trong sương Sapa nằm yên bình dưới chân dãy Hoàng Liên Sơn hùng vĩ. Quý khách sẽ được trải nghiệm tuyến cáp treo Fansipan 3 dây hiện đại nhất thế giới chạm tay vào cột mốc Nóc nhà Đông Dương cao 3.143m ngắm biển mây bao la, khám phá bản làng văn hóa Cát Cát của đồng bào H'Mông với cối xay nước khổng lồ và ruộng bậc thang tầng tầng lớp lớp, thưởng thức đặc sản lẩu cá tầm, cá hồi và thịt lợn bản nướng thơm lừng bên bếp than hồng.",
                Price = 3190000,
                DiscountPrice = 2890000,
                DurationDays = 3,
                DurationNights = 2,
                DestinationName = "Sapa, Lào Cai",
                ImageUrl = "https://images.unsplash.com/photo-1570789210967-2cac24afeb00?auto=format&fit=crop&w=1200&q=80",
                Images = "https://images.unsplash.com/photo-1570789210967-2cac24afeb00?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1570366583862-f91883984fde?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1732099083114-4fb49a280eec?auto=format&fit=crop&w=1200&q=80",
                VideoUrl = "https://www.youtube.com/watch?v=Lgvc0l1UyaU",
                MaxCapacity = 30,
                Status = "ACTIVE",
                CategoryId = catNui.Id,
                DepositPercent = 20,
                IncludedServices = "• Xe giường nằm cao cấp / Limousine Hà Nội - Sapa - Hà Nội khứ hồi chạy cao tốc\n• 02 đêm nghỉ tại khách sạn 3-4 sao trung tâm thị xã Sapa (2 khách/phòng)\n• 05 bữa ăn chính theo ẩm thực vùng cao Tây Bắc (lẩu cá hồi, gà bản nướng, thắng cố...)\n• 02 bữa sáng tại khách sạn\n• Vé tham quan trọn gói Bản Cát Cát và chương trình văn nghệ dân tộc\n• Hướng dẫn viên người bản địa nhiệt tình, giàu kinh nghiệm leo núi và am hiểu văn hóa\n• Bảo hiểm du lịch nội địa mức bồi thường tối đa 100.000.000 VNĐ/người/vụ\n• Nước uống trên xe và khăn lạnh 02 chai/người/ngày",
                ExcludedServices = "• Vé cáp treo Fansipan khứ hồi và vé tàu hỏa leo núi Mường Hoa (hỗ trợ mua trực tiếp)\n• Chi phí thuê trang phục dân tộc chụp ảnh tại Bản Cát Cát\n• Đồ uống tự gọi trong bữa ăn, giặt là, chi phí cá nhân ngoài chương trình\n• Thuế VAT 8% khi xuất hóa đơn đỏ\n• Tiền tip cho lái xe và hướng dẫn viên",
                CancellationPolicy = "• Hủy trước 07 ngày: Miễn phí hủy tour, hoàn tiền 100%\n• Hủy từ 04 - 06 ngày trước ngày đi: Phí hủy 50% tiền tour\n• Hủy từ 01 - 03 ngày trước ngày đi: Phí hủy 80% tiền tour\n• Hủy trong 24h hoặc không đến tập trung: Phí hủy 100%",
                ChangePolicy = "• Miễn phí dời ngày 01 lần nếu báo trước tối thiểu 05 ngày so với lịch khởi hành. Sau thời hạn này áp dụng theo chính sách hủy",
                ChildPolicy = "• Trẻ từ 10 tuổi: Tính giá người lớn\n• Trẻ em từ 05 đến 09 tuổi: Tính 75% giá vé người lớn (có ghế riêng trên xe, suất ăn riêng, ngủ cùng giường với bố mẹ)",
                InfantPolicy = "• Trẻ em dưới 05 tuổi: Miễn phí vé tour (bố mẹ tự lo chi phí phát sinh nếu có)",
                SeniorPolicy = "• Khách hàng cao tuổi: Sắp xếp chỗ ngồi tầng dưới trên xe giường nằm và phòng nghỉ có thang máy, cung cấp gậy chống khi leo bậc thang Fansipan",
                PickupPoints = "Nhà hát Lớn Hà Nội; Công viên Hòa Bình; Sân bay Nội Bài; Cột mốc Km0 Bến xe Sapa",
                MeetingInstructions = "Xe khởi hành lúc 06h30 sáng tại Hà Nội. Quý khách có mặt trước 30 phút để sắp xếp hành lý. Nên mang theo áo khoác ấm vì nhiệt độ ban đêm tại Sapa có thể hạ thấp.",
                LuggagePolicy = "Hành lý gọn nhẹ dưới 20kg. Chuẩn bị giày đi bộ thể thao đế bám tốt, áo ấm chống gió và ô che mưa nhỏ.",
                InsuranceDetails = "Bảo hiểm du lịch Bảo Việt bảo vệ tối đa 100.000.000 VNĐ/người/vụ.",
                TermsAndConditions = "Khi thời tiết trên đỉnh Fansipan có gió bão lớn hoặc bảo trì cáp treo đột xuất, HDV sẽ linh hoạt đổi sang tham quan Cổng Trời Ô Quy Hồ hoặc Đèo Ô Quy Hồ - Thác Bạc.",
                Departures = new List<TourDeparture>
                {
                    new TourDeparture { StartDate = DateTime.UtcNow.AddDays(4), EndDate = DateTime.UtcNow.AddDays(6), Price = 2890000, TotalSeats = 30, BookedSeats = 15 }
                },
                Itineraries = new List<TourItinerary>
                {
                    new TourItinerary
                    {
                        DayNumber = 1,
                        Title = "Hà Nội - Cao Tốc Lào Cai - Sapa - Bản Cát Cát",
                        Description = "06h30: Xe đón quý khách tại điểm hẹn khởi hành đi Sapa qua cao tốc Hà Nội - Lào Cai. 12h30: Đến thị xã Sapa thơ mộng, nhận phòng khách sạn, dùng bữa trưa với món ngon vùng cao. 14h30: Bắt đầu hành trình đi bộ khám phá Bản Cát Cát - nơi sinh sống bình yên của người H'Mông. Ngắm thác Tiên Sa, nhà truyền thống người H'Mông, check-in guồng nước khổng lồ và cầu treo Si. 18h30: Dùng bữa tối tại nhà hàng với lẩu cá tầm đặc sản. Tối tự do dạo phố cổ Cầu Mây, thăm Nhà thờ Đá cổ kính hoặc tham gia phiên Chợ Tình Sapa."
                    },
                    new TourItinerary
                    {
                        DayNumber = 2,
                        Title = "Chinh Phục Đỉnh Fansipan 3.143m - Moana Sapa Check-in",
                        Description = "07h00: Điểm tâm sáng tại khách sạn. 08h00: Xe đưa đoàn đến Ga cáp treo Sun World Fansipan Legend. Trải nghiệm tàu hỏa leo núi và cáp treo băng qua thung lũng Mường Hoa bồng bềnh mây trắng. Chinh phục 600 bậc đá chạm tay vào cột mốc Fansipan huyền thoại tại độ cao 3.143m, chiêm bái Đại tượng Phật A Di Đà bằng đồng lớn nhất Việt Nam. 12h30: Dùng bữa trưa tại nhà hàng. 14h30: Tham quan Phim trường Moana Sapa với nhiều tiểu cảnh sống ảo: Cổng trời Bali, tượng Moana, hồ vô cực. 18h30: Dùng bữa tối, tự do thưởng thức đồ nướng ngô khoai, thịt cuốn cải mèo bên bếp than hồng."
                    },
                    new TourItinerary
                    {
                        DayNumber = 3,
                        Title = "Thung Lũng Mường Hoa - Chợ Sapa Mua Đặc Sản - Hà Nội",
                        Description = "07h30: Bữa sáng tại khách sạn. 08h30: Thong thả dạo bộ ngắm thung lũng Mường Hoa, ghé Chợ Sapa mua sắm quà vùng cao: thịt trâu gác bếp, măng khô, nấm hương rừng, hạt dẻ Sapa và mận tam hoa theo mùa. 11h30: Trả phòng khách sạn, dùng bữa trưa. 13h30: Lên xe khởi hành về lại Hà Nội. 19h00: Về đến trung tâm Hà Nội, kết thúc chuyến du lịch Sapa trọn vẹn và nhiều trải nghiệm đáng nhớ."
                    }
                }
            },

            // TOUR 5: HÀ GIANG
            new Tour
            {
                Code = "HG-3N2D",
                Title = "Tour Hà Giang Mùa Hoa Tam Giác Mạch - Cột Cờ Lũng Cú 3N2Đ",
                Slug = "tour-ha-giang-mua-hoa-tam-giac-mach-3n2d",
                Summary = "Chinh phục Đệ nhất hùng quan Đèo Mã Pí Lèng, hẻm vực Tu Sản và du thuyền trên dòng sông Nho Quế xanh như ngọc bích.",
                Description = "Hành trình 3 ngày 2 đêm đưa quý khách đến mảnh đất cực Bắc thiêng liêng của Tổ quốc - nơi đá nở hoa giữa muôn trùng mây trời Đông Bắc. Quý khách sẽ được chiêm ngưỡng cánh đồng hoa tam giác mạch hồng phớt ngút ngàn, chinh phục đỉnh đèo Mã Pí Lèng - một trong 'Tứ đại đỉnh đèo' hiểm trở và ngoạn mục nhất Việt Nam, đi thuyền xuôi dòng sông Nho Quế qua hẻm vực Tu Sản sâu nhất Đông Nam Á, đứng dưới chân Cột cờ Quốc gia Lũng Cú ngắm nhìn giang sơn gấm vóc, và khám phá kiến trúc độc đáo của Dinh thự họ Vương (Dinh vua Mèo).",
                Price = 3350000,
                DiscountPrice = 2990000,
                DurationDays = 3,
                DurationNights = 2,
                DestinationName = "Hà Giang - Đồng Văn",
                ImageUrl = "https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=1200&q=80",
                Images = "https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1657445449727-017d2f9f44e7?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1570366583862-f91883984fde?auto=format&fit=crop&w=1200&q=80",
                VideoUrl = "https://www.youtube.com/watch?v=04Kf_0kppPM",
                MaxCapacity = 25,
                Status = "ACTIVE",
                CategoryId = catNui.Id,
                DepositPercent = 20,
                IncludedServices = "• Xe du lịch đời mới 16-29 chỗ chuyên phục vụ đường đèo vùng cao, lái xe kinh nghiệm an toàn tuyệt đối\n• 02 đêm nghỉ tại khách sạn / homestay tiêu chuẩn tại Yên Minh và Phố cổ Đồng Văn (2-3 khách/phòng)\n• 05 bữa ăn chính theo tiêu chuẩn thực đơn đặc sản vùng cao (thắng cố, gà đồi, lợn đen, rau rừng)\n• 02 bữa sáng tại nhà hàng địa phương\n• Vé tham quan Cột cờ Lũng Cú, Dinh Vua Mèo, Nhà của Pao, Cổng trời Quản Bạ\n• Thuyền du lịch tham quan Sông Nho Quế và Hẻm vực Tu Sản khứ hồi\n• Vé tham quan các thung lũng hoa tam giác mạch theo mùa\n• Hướng dẫn viên giàu kinh nghiệm, nhiệt tình, chụp ảnh đẹp suốt chuyến đi\n• Bảo hiểm du lịch nội địa mức bồi thường tối đa 100.000.000 VNĐ/người/vụ\n• Nước uống 02 chai/người/ngày",
                ExcludedServices = "• Xe ôm trung chuyển xuống bến thuyền sông Nho Quế (nếu tự đi riêng)\n• Đồ uống trong các bữa ăn, giặt là, điện thoại và chi phí cá nhân\n• Tiền tip cho lái xe và HDV\n• Thuế VAT 8%",
                CancellationPolicy = "• Hủy trước 07 ngày: Hoàn lại 100% chi phí\n• Hủy từ 04 - 06 ngày trước khởi hành: Phí hủy 50% giá tour\n• Hủy từ 01 - 03 ngày trước khởi hành: Phí hủy 80% giá tour\n• Hủy trong vòng 24 giờ trước giờ xe chạy: Phí hủy 100%",
                ChangePolicy = "• Hỗ trợ dời ngày khởi hành 01 lần miễn phí trước ít nhất 05 ngày",
                ChildPolicy = "• Trẻ từ 10 tuổi trở lên: Tính 100% giá người lớn\n• Trẻ em từ 05 đến 09 tuổi: Tính 75% giá vé người lớn (có chỗ ngồi riêng trên xe, ăn theo đoàn, ngủ cùng bố mẹ)",
                InfantPolicy = "• Trẻ dưới 05 tuổi: Miễn phí giá vé (bố mẹ tự chăm sóc). Cung đường đèo dốc uốn lượn, khuyến cáo phụ huynh chuẩn bị thuốc chống say cho trẻ nhỏ",
                SeniorPolicy = "• Khách hàng cao tuổi: Ưu tiên ghế ngồi phía trên xe và hỗ trợ khi lên các bậc thang Cột cờ Lũng Cú",
                PickupPoints = "Nhà hát Lớn Hà Nội; Số 1 Tràng Thi; Ngã tư Nội Bài (đường cao tốc Hà Nội - Lào Cai); Thành phố Hà Giang",
                MeetingInstructions = "Xe khởi hành từ 06h00 sáng tại Hà Nội. Quý khách vui lòng có mặt trước 20 phút. Vui lòng mang theo CMND/CCCD bản gốc.",
                LuggagePolicy = "Nên mang ba lô hoặc vali nhỏ gọn dưới 15kg để thuận tiện di chuyển trên xe qua các cung đèo dốc.",
                InsuranceDetails = "Bảo hiểm du lịch Bảo Việt bảo vệ toàn diện tối đa 100.000.000 VNĐ/khách/vụ.",
                TermsAndConditions = "Chương trình có thể linh hoạt điều chỉnh thứ tự các điểm đến cho phù hợp với mật độ giao thông và điều kiện thời tiết nhưng luôn cam kết thực hiện đầy đủ các điểm tham quan.",
                Departures = new List<TourDeparture>
                {
                    new TourDeparture { StartDate = DateTime.UtcNow.AddDays(8), EndDate = DateTime.UtcNow.AddDays(10), Price = 2990000, TotalSeats = 25, BookedSeats = 7 }
                },
                Itineraries = new List<TourItinerary>
                {
                    new TourItinerary
                    {
                        DayNumber = 1,
                        Title = "Hà Nội - Km0 Hà Giang - Cổng Trời Quản Bạ - Rừng Thông Yên Minh",
                        Description = "06h00: Xe và HDV đón quý khách tại Hà Nội, khởi hành đi Hà Giang. 12h00: Đến TP Hà Giang, dùng bữa trưa và chụp ảnh kỷ niệm tại Cột mốc Km0. 14h00: Tiếp tục hành trình qua Dốc Bắc Sum, dừng chân tại Cổng Trời Quản Bạ ngắm toàn cảnh Núi Đôi Cô Tiên kỳ vĩ. Đi qua rừng thông Yên Minh bạt ngàn ngỡ như Đà Lạt của phương Bắc. 18h30: Đến thị trấn Yên Minh, nhận phòng khách sạn, dùng bữa tối và thưởng thức chén rượu ngô men lá thơm nồng."
                    },
                    new TourItinerary
                    {
                        DayNumber = 2,
                        Title = "Nhà Của Pao - Dinh Vua Mèo - Cột Cờ Lũng Cú - Phố Cổ Đồng Văn",
                        Description = "07h00: Điểm tâm sáng, khởi hành đến Thung lũng Sủng Là tham quan 'Nhà của Pao' - bối cảnh bộ phim nổi tiếng giữa vườn hoa tam giác mạch. 09h30: Thăm Dinh thự họ Vương (Vua Mèo Vương Chính Đức) với kiến trúc đá tinh xảo kết hợp văn hóa H'Mông và Hoa. 11h30: Dùng bữa trưa tại Lũng Cú. 13h00: Chinh phục Cột cờ Lũng Cú - điểm cực Bắc địa đầu thiêng liêng nơi lá cờ đỏ sao vàng 54m2 bay phấp phới. 16h30: Đến Phố cổ Đồng Văn nhận phòng. 19h00: Dùng bữa tối với lẩu gà đen vùng cao, dạo chợ đêm Đồng Văn và thưởng thức bánh cuốn nóng chan nước xương."
                    },
                    new TourItinerary
                    {
                        DayNumber = 3,
                        Title = "Đèo Mã Pí Lèng - Du Thuyền Sông Nho Quế - Hẻm Tu Sản - Hà Nội",
                        Description = "07h00: Ăn sáng, trả phòng. 08h00: Chinh phục Đèo Mã Pí Lèng hiểm trở dài 20km - 'Đệ nhất hùng quan'. Đứng trên mỏm đá ngắm trọn hẻm vực Tu Sản sâu thẳm. 09h30: Xuống bến thuyền, lên thuyền du ngoạn trên dòng sông Nho Quế xanh màu ngọc bích, lướt qua giữa hẻm vực Tu Sản kỳ vĩ. 12h00: Dùng bữa trưa tại thị trấn Đồng Văn hoặc Mèo Vạc. 13h30: Lên xe trở về Hà Nội. 20h30: Về đến điểm đón ban đầu tại Hà Nội, HDV chào tạm biệt và kết thúc hành trình chinh phục Đông Bắc."
                    }
                }
            },

            // TOUR 6: ĐÀ LẠT
            new Tour
            {
                Code = "DL-3N2D",
                Title = "Tour Đà Lạt Thành Phố Ngàn Hoa - Săn Mây Cầu Đất 3N2Đ",
                Slug = "tour-da-lat-thanh-pho-ngan-hoa-3n2d",
                Summary = "Tận hưởng không khí se lạnh cao nguyên Lâm Viên, săn mây đồi chè Cầu Đất và đắm chìm trong hương sắc ngàn hoa mộng mơ.",
                Description = "Hành trình 3 ngày 2 đêm đưa du khách đến với xứ sở ngàn hoa Đà Lạt - thành phố sương mù lãng mạn bậc nhất xứ sở nhiệt đới. Trải nghiệm khoảnh khắc đón bình minh săn biển mây bồng bềnh tại đồi chè Cầu Đất, dạo bước qua Quảng trường Lâm Viên ngắm nụ hoa Atiso khổng lồ bên Hồ Xuân Hương thơ mộng, khám phá Thung Lũng Tình Yêu ngập tràn sắc hoa rực rỡ, tham quan Thác Datanla hùng vĩ với trò chơi máng trượt xuyên rừng thông độc đáo, và thưởng thức ẩm thực chợ đêm Đà Lạt ấm cúng với sữa đậu nành nóng hổi, bánh tráng nướng giòn rụm.",
                Price = 2890000,
                DiscountPrice = 2550000,
                DurationDays = 3,
                DurationNights = 2,
                DestinationName = "Đà Lạt, Lâm Đồng",
                ImageUrl = "https://images.unsplash.com/photo-1509316975850-ff9c5deb0cd9?auto=format&fit=crop&w=1200&q=80",
                Images = "https://images.unsplash.com/photo-1509316975850-ff9c5deb0cd9?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1669413487832-a8733f063ca8?auto=format&fit=crop&w=1200&q=80;https://images.unsplash.com/photo-1531737212413-667205e1cda7?auto=format&fit=crop&w=1200&q=80",
                VideoUrl = "https://www.youtube.com/watch?v=04Kf_0kppPM",
                MaxCapacity = 30,
                Status = "ACTIVE",
                CategoryId = catDiSan.Id,
                DepositPercent = 20,
                IncludedServices = "• Xe du lịch máy lạnh chất lượng cao đón tiễn sân bay Liên Khương và phục vụ tham quan suốt tour\n• 02 đêm nghỉ tại khách sạn 3-4 sao trung tâm Đà Lạt gần chợ (2 khách/phòng)\n• 04 bữa chính tiêu chuẩn ẩm thực Đà Lạt (150.000 - 180.000 VNĐ/suất gồm lẩu gà lá é, lẩu bò Atiso)\n• 02 bữa sáng buffet tại khách sạn\n• Vé tham quan tất cả các điểm: Đồi chè Cầu Đất, Thung Lũng Tình Yêu, Thác Datanla, Quảng trường Lâm Viên, Vườn dâu tây công nghệ cao\n• Vé máng trượt Thác Datanla khứ hồi\n• Hướng dẫn viên bản xứ năng động, nhiệt huyết và chụp ảnh lưu niệm suốt chuyến đi\n• Bảo hiểm du lịch nội địa mức bồi thường 100.000.000 VNĐ/khách\n• Nước uống 02 chai/người/ngày",
                ExcludedServices = "• Vé máy bay khứ hồi từ Hà Nội / TP.HCM đến Đà Lạt\n• Vé xe jeep chinh phục đỉnh Langbiang (nếu có nhu cầu)\n• Dịch vụ hái dâu tây ăn tại vườn hoặc mang về (tính theo ký thực tế)\n• Đồ uống trong các bữa ăn, giặt ủi và chi tiêu cá nhân\n• Tiền tip cho tài xế và HDV, thuế VAT 8%",
                CancellationPolicy = "• Hủy trước 07 ngày: Hoàn tiền 100%\n• Hủy từ 04 - 06 ngày trước khởi hành: Phí hủy 50% giá tour\n• Hủy từ 01 - 03 ngày trước khởi hành: Phí hủy 80% giá tour\n• Hủy trong vòng 24 giờ: Phí hủy 100%",
                ChangePolicy = "• Miễn phí dời ngày 01 lần trước ít nhất 05 ngày so với ngày khởi hành",
                ChildPolicy = "• Trẻ em từ 10 tuổi: Tính giá vé người lớn\n• Trẻ em từ 05 đến 09 tuổi: Tính 75% giá vé người lớn (có suất ăn riêng, ghế ngồi riêng, ngủ chung giường bố mẹ)",
                InfantPolicy = "• Trẻ em dưới 05 tuổi: Miễn phí tour (bố mẹ tự thanh toán chi phí phát sinh nếu có)",
                SeniorPolicy = "• Người cao tuổi: Sắp xếp phòng tầng thấp, hỗ trợ đi lại và ưu tiên trong các chặng di chuyển",
                PickupPoints = "Sân bay Liên Khương Đà Lạt; Bến xe Liên tỉnh Đà Lạt; Các khách sạn trung tâm TP. Đà Lạt",
                MeetingInstructions = "HDV đón khách tại Sân bay Liên Khương hoặc Bến xe theo giờ hẹn. Vui lòng chuẩn bị áo ấm, khăn quàng vì thời tiết Đà Lạt về đêm có sương mù và se lạnh.",
                LuggagePolicy = "Tối đa 20kg ký gửi + 7kg xách tay. Nên mang giày thể thao thoải mái để đi bộ trong các khu du lịch đồi hoa.",
                InsuranceDetails = "Bảo hiểm du lịch Bảo Việt bảo hiểm trọn gói với hạn mức 100.000.000 VNĐ/người/vụ.",
                TermsAndConditions = "Thứ tự các điểm săn mây có thể sắp xếp linh hoạt từ sáng sớm hôm 2 hoặc hôm 3 để đảm bảo du khách bắt được khoảnh khắc biển mây đẹp nhất.",
                Departures = new List<TourDeparture>
                {
                    new TourDeparture { StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(5), Price = 2550000, TotalSeats = 30, BookedSeats = 20 }
                },
                Itineraries = new List<TourItinerary>
                {
                    new TourItinerary
                    {
                        DayNumber = 1,
                        Title = "Đón Sân Bay Liên Khương - Thác Datanla - Quảng Trường Lâm Viên",
                        Description = "Sáng: Xe đón quý khách tại Sân bay Liên Khương hoặc Bến xe Đà Lạt, đưa về khách sạn gửi hành lý. 11h30: Dùng bữa trưa với món Lẩu gà lá é nổi tiếng. 14h00: Khởi hành tham quan Thác Datanla - trải nghiệm hệ thống máng trượt xuyên qua rừng thông dài nhất Đông Nam Á cảm giác phấn khích. 16h30: Check-in Quảng trường Lâm Viên biểu tượng Đà Lạt bên hồ Xuân Hương với nụ hoa Atiso khổng lồ và bông hoa Dã Quỳ rực rỡ bằng kính màu. 18h30: Dùng bữa tối. Tối tự do dạo Chợ đêm Đà Lạt thưởng thức bánh tráng nướng, sữa đậu nành nóng hổi."
                    },
                    new TourItinerary
                    {
                        DayNumber = 2,
                        Title = "Săn Mây Đồi Chè Cầu Đất - Vườn Dâu Tây - Thung Lũng Tình Yêu",
                        Description = "05h00: Khởi hành sớm đến Đồi chè Cầu Đất đón bình minh và săn biển mây trắng bồng bềnh tràn qua các thung lũng chè bát ngát. Thưởng thức ly cà phê nóng ngắm sương sớm. 08h30: Trở về khách sạn ăn sáng buffet. 09h30: Tham quan Vườn dâu tây công nghệ cao, tự tay chụp ảnh và hái những trái dâu chín mọng ngọt ngào. 12h00: Dùng bữa trưa tại nhà hàng. 14h00: Khám phá KDL Thung Lũng Tình Yêu với mê cung tình yêu, hồ Đa Thiện, đồi Vọng Cảnh và hàng triệu đóa hoa khoe sắc. 18h30: Dùng bữa tối lãng mạn tại nhà hàng phố núi."
                    },
                    new TourItinerary
                    {
                        DayNumber = 3,
                        Title = "Chùa Linh Phước - Ga Xe Lửa Cổ - Mua Sắm Đặc Sản - Tiễn Sân Bay",
                        Description = "07h30: Điểm tâm sáng tại khách sạn, hoàn tất thủ tục trả phòng. 08h30: Viếng Chùa Linh Phước (Chùa Ve Chai) - ngôi chùa nắm giữ nhiều kỷ lục với kiến trúc khảm sành sứ độc nhất vô nhị. 10h00: Tham quan Ga Đà Lạt - nhà ga xe lửa cổ kính nhất Đông Dương mang phong cách Art Deco kết hợp nhà rông Tây Nguyên. 11h30: Mua sắm đặc sản Đà Lạt: atiso, mứt hoa quả, hồng sấy gió và rượu vang Đà Lạt. 12h30: Dùng bữa trưa nhẹ. 14h00: Xe đưa quý khách ra Sân bay Liên Khương làm thủ tục bay, kết thúc hành trình Đà Lạt ngọt ngào và thơ mộng."
                    }
                }
            }
        };

        // 4. Seed hoặc cập nhật thông tin tour & lịch trình
        foreach (var tDef in tourDefinitions)
        {
            var existingTour = context.Tours
                .Include(t => t.Itineraries)
                .Include(t => t.Departures)
                .FirstOrDefault(t => t.Code == tDef.Code || t.Slug == tDef.Slug);

            if (existingTour == null)
            {
                context.Tours.Add(tDef);
                context.SaveChanges();
            }
            else
            {
                // Cập nhật tất cả các trường chi tiết và chính sách
                existingTour.Title = tDef.Title;
                existingTour.Slug = tDef.Slug;
                existingTour.Summary = tDef.Summary;
                existingTour.Description = tDef.Description;
                existingTour.Price = tDef.Price;
                existingTour.DiscountPrice = tDef.DiscountPrice;
                existingTour.DurationDays = tDef.DurationDays;
                existingTour.DurationNights = tDef.DurationNights;
                existingTour.DestinationName = tDef.DestinationName;
                existingTour.ImageUrl = tDef.ImageUrl;
                existingTour.Images = tDef.Images;
                existingTour.VideoUrl = tDef.VideoUrl;
                existingTour.MaxCapacity = tDef.MaxCapacity;
                existingTour.Status = "ACTIVE";
                existingTour.DepositPercent = tDef.DepositPercent;
                existingTour.IncludedServices = tDef.IncludedServices;
                existingTour.ExcludedServices = tDef.ExcludedServices;
                existingTour.CancellationPolicy = tDef.CancellationPolicy;
                existingTour.ChangePolicy = tDef.ChangePolicy;
                existingTour.ChildPolicy = tDef.ChildPolicy;
                existingTour.InfantPolicy = tDef.InfantPolicy;
                existingTour.SeniorPolicy = tDef.SeniorPolicy;
                existingTour.PickupPoints = tDef.PickupPoints;
                existingTour.MeetingInstructions = tDef.MeetingInstructions;
                existingTour.LuggagePolicy = tDef.LuggagePolicy;
                existingTour.InsuranceDetails = tDef.InsuranceDetails;
                existingTour.TermsAndConditions = tDef.TermsAndConditions;

                // Nếu chưa có lịch trình hoặc lịch trình rỗng thì thêm mới
                if (!existingTour.Itineraries.Any())
                {
                    foreach (var itin in tDef.Itineraries)
                    {
                        itin.TourId = existingTour.Id;
                        context.TourItineraries.Add(itin);
                    }
                }
                else
                {
                    // Cập nhật nội dung các ngày lịch trình
                    foreach (var itin in tDef.Itineraries)
                    {
                        var existingItin = existingTour.Itineraries.FirstOrDefault(i => i.DayNumber == itin.DayNumber);
                        if (existingItin != null)
                        {
                            existingItin.Title = itin.Title;
                            existingItin.Description = itin.Description;
                        }
                        else
                        {
                            itin.TourId = existingTour.Id;
                            context.TourItineraries.Add(itin);
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        // 5. Seed sample bookings if empty
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

                var sampleBooking2 = new Booking
                {
                    BookingCode = "BK-DEMO-002",
                    UserId = 1,
                    ContactName = "Quản trị viên LeViet",
                    ContactEmail = "admin@leviet.com",
                    ContactPhone = "0988888888",
                    TourId = firstTour.Id,
                    TourTitle = firstTour.Title,
                    DepartureId = 1,
                    StartDate = DateTime.UtcNow.AddDays(10),
                    AdultsCount = 1,
                    ChildrenCount = 0,
                    TotalPrice = 3450000,
                    DiscountAmount = 0,
                    FinalAmount = 3450000,
                    Status = "CONFIRMED",
                    VietQrUrl = "https://img.vietqr.io/image/MB-0988888888-compact2.png?amount=3450000&addInfo=BK-DEMO-002%200988888888",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                };

                context.Bookings.AddRange(sampleBooking, sampleBooking2);
                context.SaveChanges();
            }
        }
    }
}
