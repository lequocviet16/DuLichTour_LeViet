using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [HttpGet("bookings.xlsx")]
    public IActionResult ExportBookings()
    {
        var csv = "Mã đơn,Khách hàng,Tour,Ngày khởi hành,Tổng tiền,Trạng thái\n" +
                  "LV-BK-20261115-001,Nguyễn Văn An,Tour Vịnh Hạ Long,2026-12-01,7780000,CONFIRMED\n";
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "bookings.xlsx");
    }

    [HttpGet("departures/{departureId}/manifest.xlsx")]
    public IActionResult ExportManifest(int departureId)
    {
        var csv = "STT,Họ và tên,Số điện thoại,Điểm đón,Ghi chú\n" +
                  "1,Nguyễn Văn An,0901234567,Nhà hát Lớn Hà Nội,Khách ăn chay\n";
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"manifest_{departureId}.xlsx");
    }
}
