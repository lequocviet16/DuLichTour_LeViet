using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Common;
using Models.DTOs;
using Models.Entities;

namespace BLL;

public class AuthBLL
{
    private readonly UserDAL _userDAL;
    private readonly IConfiguration _configuration;

    public AuthBLL(UserDAL userDAL, IConfiguration configuration)
    {
        _userDAL = userDAL;
        _configuration = configuration;
    }

    public async Task<ApiResponse<TokenResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (await _userDAL.ExistsEmailAsync(email))
        {
            return ApiResponse<TokenResponseDto>.Fail("Email này đã được sử dụng.");
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

        await _userDAL.CreateUserAsync(user);

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user.Id);

        await _userDAL.SaveRefreshTokenAsync(refreshToken);

        var result = new TokenResponseDto
        {
            AccessToken = accessToken,
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            User = new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            }
        };

        return ApiResponse<TokenResponseDto>.Ok(result, "Đăng ký tài khoản thành công!");
    }

    public async Task<ApiResponse<TokenResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userDAL.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return ApiResponse<TokenResponseDto>.Fail("Email hoặc mật khẩu không chính xác.", 401);
        }

        if (!user.IsActive)
        {
            return ApiResponse<TokenResponseDto>.Fail("Tài khoản đã bị tạm khóa.", 403);
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user.Id);

        await _userDAL.SaveRefreshTokenAsync(refreshToken);

        var result = new TokenResponseDto
        {
            AccessToken = accessToken,
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            User = new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            }
        };

        return ApiResponse<TokenResponseDto>.Ok(result, "Đăng nhập thành công!");
    }

    public async Task<ApiResponse<TokenResponseDto>> RefreshTokenAsync(string token)
    {
        var existingToken = await _userDAL.GetRefreshTokenAsync(token);
        if (existingToken == null || !existingToken.IsActive || existingToken.User == null)
        {
            return ApiResponse<TokenResponseDto>.Fail("Refresh Token không hợp lệ hoặc đã hết hạn.", 401);
        }

        // Revoke token cũ (Rotation)
        existingToken.RevokedAt = DateTime.UtcNow;

        var user = existingToken.User;
        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken(user.Id);

        existingToken.ReplacedByToken = newRefreshToken.Token;
        await _userDAL.UpdateRefreshTokenAsync(existingToken);
        await _userDAL.SaveRefreshTokenAsync(newRefreshToken);

        var result = new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = newRefreshToken.ExpiresAt,
            User = new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            }
        };

        return ApiResponse<TokenResponseDto>.Ok(result, "Làm mới phiên đăng nhập thành công!");
    }

    public async Task<ApiResponse<object>> RevokeTokenAsync(string token)
    {
        var existingToken = await _userDAL.GetRefreshTokenAsync(token);
        if (existingToken != null && existingToken.IsActive)
        {
            existingToken.RevokedAt = DateTime.UtcNow;
            await _userDAL.UpdateRefreshTokenAsync(existingToken);
        }

        return ApiResponse.Ok(null, "Đăng xuất và hủy token thành công!");
    }

    public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId)
    {
        var user = await _userDAL.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse<UserProfileDto>.Fail("Không tìm thấy người dùng.", 404);
        }

        var profile = new UserProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role
        };

        return ApiResponse<UserProfileDto>.Ok(profile);
    }

    private string GenerateAccessToken(User user)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? "LeVietTravel_Super_Secret_Key_For_Jwt_Authentication_2026_Minimum_32_Chars";
        var issuer = _configuration["Jwt:Issuer"] ?? "LeVietTravel";
        var audience = _configuration["Jwt:Audience"] ?? "LeVietTravelApp";
        var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var min) ? min : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private RefreshToken GenerateRefreshToken(int userId)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            UserId = userId,
            Token = Convert.ToBase64String(randomBytes),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
    }
}
