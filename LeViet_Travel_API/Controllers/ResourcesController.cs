using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var items = new[]
        {
            new { id = 1, name = "Khách sạn Novotel Hạ Long", address = "Đường Hạ Long, Bãi Cháy, TP. Hạ Long" },
            new { id = 2, name = "Khách sạn Vinpearl Resort & Spa Đà Nẵng", address = "Trường Sa, Ngũ Hành Sơn, Đà Nẵng" },
            new { id = 3, name = "Khách sạn Sapa Jade Hill Resort", address = "Mường Hoa, Sa Pa, Lào Cai" }
        };
        return Ok(ApiResponse.Ok(new { items, pagination = new { page, limit, totalItems = items.Length, totalPages = 1 } }, "Thành công"));
    }

    [HttpPost]
    public IActionResult Create([FromBody] object body) => StatusCode(201, ApiResponse.Ok(body, "Thành công"));

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] object body) => Ok(ApiResponse.Ok(body, "Thành công"));

    [HttpDelete("{id}")]
    public IActionResult Delete(int id) => Ok(ApiResponse.Ok(null, "Thành công"));
}

[ApiController]
[Route("api/vehicles")]
public class VehiclesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var items = new[]
        {
            new { id = 1, licensePlate = "29B-123.45", vehicleType = "Hyundai Universe 45 Chỗ", seatCapacity = 45 },
            new { id = 2, licensePlate = "43B-678.90", vehicleType = "Ford Transit DCar Limousine 16 Chỗ", seatCapacity = 16 },
            new { id = 3, licensePlate = "51B-543.21", vehicleType = "Thaco Mobihome Giường Nằm 34 Phòng", seatCapacity = 34 }
        };
        return Ok(ApiResponse.Ok(new { items, pagination = new { page, limit, totalItems = items.Length, totalPages = 1 } }, "Thành công"));
    }

    [HttpPost]
    public IActionResult Create([FromBody] object body) => StatusCode(201, ApiResponse.Ok(body, "Thành công"));

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] object body) => Ok(ApiResponse.Ok(body, "Thành công"));

    [HttpDelete("{id}")]
    public IActionResult Delete(int id) => Ok(ApiResponse.Ok(null, "Thành công"));
}

[ApiController]
[Route("api/tour-guides")]
public class TourGuidesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var items = new[]
        {
            new { id = 1, fullName = "Nguyễn Văn Hùng", phoneNumber = "0912345678", isActive = true },
            new { id = 2, fullName = "Trần Thị Lan", phoneNumber = "0987654321", isActive = true },
            new { id = 3, fullName = "Hoàng Đức Anh", phoneNumber = "0933445566", isActive = true }
        };
        return Ok(ApiResponse.Ok(new { items, pagination = new { page, limit, totalItems = items.Length, totalPages = 1 } }, "Thành công"));
    }

    [HttpPost]
    public IActionResult Create([FromBody] object body) => StatusCode(201, ApiResponse.Ok(body, "Thành công"));

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] object body) => Ok(ApiResponse.Ok(body, "Thành công"));

    [HttpDelete("{id}")]
    public IActionResult Delete(int id) => Ok(ApiResponse.Ok(null, "Thành công"));
}
