using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.DTOs;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToursController : ControllerBase
{
    private readonly TourBLL _tourBLL;

    public ToursController(TourBLL tourBLL)
    {
        _tourBLL = tourBLL;
    }

    [HttpGet]
    public async Task<IActionResult> GetTours([FromQuery] TourFilterDto filter)
    {
        var result = await _tourBLL.GetToursAsync(filter);
        return Ok(result);
    }

    [HttpGet("{idOrSlug}")]
    public async Task<IActionResult> GetTourByIdOrSlug(string idOrSlug)
    {
        var result = await _tourBLL.GetTourByIdOrSlugAsync(idOrSlug);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpGet("{id:int}/recommendations")]
    public async Task<IActionResult> GetRecommendations(int id)
    {
        var result = await _tourBLL.GetRecommendationsAsync(id);
        return Ok(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _tourBLL.GetCategoriesAsync();
        return Ok(result);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateTour([FromBody] CreateTourDto dto)
    {
        var result = await _tourBLL.CreateTourAsync(dto);
        return StatusCode(201, result);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTour(int id)
    {
        var result = await _tourBLL.DeleteTourAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
