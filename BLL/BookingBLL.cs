using DAL;
using Models.Common;
using Models.DTOs;
using Models.Entities;

namespace BLL;

public class BookingBLL
{
    private readonly BookingDAL _bookingDAL;

    public BookingBLL(BookingDAL bookingDAL)
    {
        _bookingDAL = bookingDAL;
    }

    public async Task<ApiResponse<BookingDto>> CreateBookingAsync(int userId, CreateBookingDto dto)
    {
        var bookingCode = $"BK-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        var totalPrice = (dto.AdultsCount * dto.PricePerPerson) + (dto.ChildrenCount * dto.PricePerPerson * 0.75m);
        var finalAmount = totalPrice;

        // Sinh link VietQR động chuẩn Napas 247
        var qrInfo = Uri.EscapeDataString($"{bookingCode} {dto.ContactPhone}");
        var qrUrl = $"https://img.vietqr.io/image/MB-0988888888-compact2.png?amount={(long)finalAmount}&addInfo={qrInfo}";

        var booking = new Booking
        {
            BookingCode = bookingCode,
            UserId = userId,
            ContactName = dto.ContactName.Trim(),
            ContactEmail = dto.ContactEmail.Trim(),
            ContactPhone = dto.ContactPhone.Trim(),
            TourId = dto.TourId,
            TourTitle = dto.TourTitle,
            DepartureId = dto.DepartureId,
            StartDate = dto.StartDate,
            AdultsCount = dto.AdultsCount,
            ChildrenCount = dto.ChildrenCount,
            TotalPrice = totalPrice,
            DiscountAmount = 0,
            FinalAmount = finalAmount,
            Status = "PENDING",
            VietQrUrl = qrUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _bookingDAL.CreateBookingAsync(booking);

        var result = MapToDto(booking);
        return ApiResponse<BookingDto>.Ok(result, "Tạo đơn đặt tour thành công!");
    }

    public async Task<ApiResponse<List<BookingDto>>> GetUserBookingsAsync(int userId)
    {
        var bookings = await _bookingDAL.GetByUserIdAsync(userId);
        var dtos = bookings.Select(MapToDto).ToList();
        return ApiResponse<List<BookingDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<List<BookingDto>>> GetAllBookingsAsync()
    {
        var bookings = await _bookingDAL.GetAllAsync();
        var dtos = bookings.Select(MapToDto).ToList();
        return ApiResponse<List<BookingDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<object>> UpdateStatusAsync(int id, string status)
    {
        var existing = await _bookingDAL.GetByIdAsync(id);
        if (existing == null)
        {
            return ApiResponse.Fail("Không tìm thấy đơn đặt tour.", 404);
        }

        await _bookingDAL.UpdateStatusAsync(id, status.ToUpper());
        return ApiResponse.Ok(null, $"Cập nhật trạng thái đơn thành {status.ToUpper()} thành công!");
    }

    private static BookingDto MapToDto(Booking b)
    {
        return new BookingDto
        {
            Id = b.Id,
            BookingCode = b.BookingCode,
            UserId = b.UserId,
            ContactName = b.ContactName,
            ContactEmail = b.ContactEmail,
            ContactPhone = b.ContactPhone,
            TourId = b.TourId,
            TourTitle = b.TourTitle,
            DepartureId = b.DepartureId,
            StartDate = b.StartDate,
            AdultsCount = b.AdultsCount,
            ChildrenCount = b.ChildrenCount,
            TotalPrice = b.TotalPrice,
            DiscountAmount = b.DiscountAmount,
            FinalAmount = b.FinalAmount,
            Status = b.Status,
            VietQrUrl = b.VietQrUrl,
            CreatedAt = b.CreatedAt
        };
    }
}
