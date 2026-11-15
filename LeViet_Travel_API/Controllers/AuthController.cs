using System.Security.Claims;
using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthBLL _authBLL;

    public AuthController(AuthBLL authBLL)
    {
        _authBLL = authBLL;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authBLL.RegisterAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authBLL.LoginAsync(dto);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _authBLL.RefreshTokenAsync(dto.RefreshToken);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }
        return Ok(result);
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequestDto dto)
    {
        var result = await _authBLL.RevokeTokenAsync(dto.Token);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được danh tính người dùng."));
        }

        var result = await _authBLL.GetProfileAsync(userId);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(ApiResponse.Ok(new { User = result.Data }, "Lấy thông tin người dùng thành công"));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("admin-only")]
    public IActionResult TestAdminOnly()
    {
        return Ok(ApiResponse.Ok(new
        {
            User = User.Identity?.Name,
            Role = User.FindFirst(ClaimTypes.Role)?.Value,
            Message = "Xin chúc mừng! Bạn đã xác thực thành công với quyền ADMIN."
        }, "Quyền ADMIN hợp lệ!"));
    }

    [Authorize(Roles = Roles.Customer)]
    [HttpGet("customer-only")]
    public IActionResult TestCustomerOnly()
    {
        return Ok(ApiResponse.Ok(new
        {
            User = User.Identity?.Name,
            Role = User.FindFirst(ClaimTypes.Role)?.Value,
            Message = "Xin chào Khách hàng! Quyền CUSTOMER hợp lệ."
        }, "Quyền CUSTOMER hợp lệ!"));
    }
}
