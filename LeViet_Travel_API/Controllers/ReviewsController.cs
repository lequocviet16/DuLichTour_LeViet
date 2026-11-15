using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Common;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly LeVietTravelDbContext _context;

    public ReviewsController(LeVietTravelDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var tours = await _context.Tours.Take(5).ToListAsync();
        var mockReviews = new List<object>
        {
            new { id = 1, tourId = tours.Count > 0 ? tours[0].Id : 1, tour = tours.Count > 0 ? new { id = tours[0].Id, name = tours[0].Title } : null, userName = "Trần Thị Mai", rating = 5, comment = "Chuyến đi tuyệt vời! Hướng dẫn viên nhiệt tình, cảnh vịnh Hạ Long rất hùng vĩ.", createdAt = "2026-11-10 14:30:00" },
            new { id = 2, tourId = tours.Count > 1 ? tours[1].Id : 2, tour = tours.Count > 1 ? new { id = tours[1].Id, name = tours[1].Title } : null, userName = "Lê Hoàng Nam", rating = 5, comment = "Khách sạn sạch sẽ, dịch vụ ăn uống ngon, gia đình mình rất hài lòng.", createdAt = "2026-11-12 09:15:00" },
            new { id = 3, tourId = tours.Count > 2 ? tours[2].Id : 3, tour = tours.Count > 2 ? new { id = tours[2].Id, name = tours[2].Title } : null, userName = "Nguyễn Minh Tuấn", rating = 4, comment = "Tour đi đúng giờ, lịch trình hợp lý. Sẽ ủng hộ Lê Việt Travel các chuyến tiếp theo.", createdAt = "2026-11-14 18:00:00" }
        };

        var result = new
        {
            items = mockReviews,
            pagination = new
            {
                page,
                limit,
                totalItems = mockReviews.Count,
                totalPages = 1
            }
        };

        return Ok(ApiResponse.Ok(result, "Lấy danh sách đánh giá thành công"));
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateReviewRequest req)
    {
        return Ok(ApiResponse.Ok(new { id, req.rating, req.comment }, "Cập nhật đánh giá thành công"));
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return Ok(ApiResponse.Ok(null, "Đã xóa đánh giá"));
    }

    public class UpdateReviewRequest
    {
        public int rating { get; set; }
        public string? comment { get; set; }
    }
}
