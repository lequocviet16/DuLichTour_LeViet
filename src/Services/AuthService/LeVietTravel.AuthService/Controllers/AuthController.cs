using System.Security.Claims;
using LeVietTravel.AuthService.Data;
using LeVietTravel.AuthService.DTOs;
using LeVietTravel.AuthService.Entities;
using LeVietTravel.AuthService.Services;
using LeVietTravel.Common.Constants;
using LeVietTravel.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeVietTravel.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(AuthDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            return BadRequest(ApiResponse.Fail("Email này đã được sử dụng."));
        }

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber?.Trim(),
            Role = Roles.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        var response = new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            User = MapToUserDto(user)
        };

        return StatusCode(201, ApiResponse<TokenResponseDto>.Ok(response, "Đăng ký tài khoản thành công."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(ApiResponse.Fail("Email hoặc mật khẩu không chính xác."));
        }

        if (!user.IsActive)
        {
            return StatusCode(403, ApiResponse.Fail("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên."));
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        var response = new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            User = MapToUserDto(user)
        };

        return Ok(ApiResponse<TokenResponseDto>.Ok(response, "Đăng nhập thành công."));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        var existingToken = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

        if (existingToken == null || !existingToken.IsActive || existingToken.User == null)
        {
            return Unauthorized(ApiResponse.Fail("Refresh Token không hợp lệ hoặc đã hết hạn/bị thu hồi."));
        }

        if (!existingToken.User.IsActive)
        {
            return StatusCode(403, ApiResponse.Fail("Tài khoản người dùng đã bị khóa."));
        }

        // Token Rotation: Thu hồi token cũ và sinh token mới
        var newAccessToken = _tokenService.GenerateAccessToken(existingToken.User);
        var newRefreshToken = _tokenService.GenerateRefreshToken(existingToken.UserId);

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByToken = newRefreshToken.Token;

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        var response = new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = newRefreshToken.ExpiresAt,
            User = MapToUserDto(existingToken.User)
        };

        return Ok(ApiResponse<TokenResponseDto>.Ok(response, "Làm mới Token thành công."));
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequestDto dto)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

        if (token == null || !token.IsActive)
        {
            return BadRequest(ApiResponse.Fail("Token không tồn tại hoặc đã bị thu hồi trước đó."));
        }

        token.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Ok("Thu hồi Refresh Token thành công (Đăng xuất khỏi thiết bị)."));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse.Fail("Không tìm thấy thông tin định danh người dùng trong Token."));
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy thông tin người dùng."));
        }

        return Ok(ApiResponse<UserDto>.Ok(MapToUserDto(user), "Lấy thông tin người dùng thành công."));
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse.Fail("Token không hợp lệ."));
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy người dùng."));
        }

        if (!string.IsNullOrWhiteSpace(dto.FullName))
        {
            user.FullName = dto.FullName.Trim();
        }

        if (dto.PhoneNumber != null)
        {
            user.PhoneNumber = dto.PhoneNumber.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(dto.OldPassword) || !BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            {
                return BadRequest(ApiResponse.Fail("Mật khẩu cũ không chính xác."));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        }

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<UserDto>.Ok(MapToUserDto(user), "Cập nhật thông tin cá nhân thành công."));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("admin-only")]
    public IActionResult AdminOnly()
    {
        return Ok(ApiResponse.Ok("Xin chào Admin! Bạn có quyền truy cập vào khu vực Quản trị tối cao."));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpGet("customer-only")]
    public IActionResult CustomerOnly()
    {
        return Ok(ApiResponse.Ok("Xin chào Khách hàng! Bạn có quyền truy cập vào khu vực Dành cho khách hàng."));
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
