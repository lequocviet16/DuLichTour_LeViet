using System.ComponentModel.DataAnnotations;

namespace Models.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Họ và tên là bắt buộc")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    public string Password { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
}

public class LoginDto
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public string Password { get; set; } = string.Empty;
}

public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserProfileDto User { get; set; } = new();
}

public class UserProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "RefreshToken là bắt buộc")]
    public string RefreshToken { get; set; } = string.Empty;
}

public class RevokeTokenRequestDto
{
    [Required(ErrorMessage = "Token là bắt buộc")]
    public string Token { get; set; } = string.Empty;
}
