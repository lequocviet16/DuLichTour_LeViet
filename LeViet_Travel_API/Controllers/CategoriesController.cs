using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.Entities;

namespace LeViet_Travel_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly LeVietTravelDbContext _context;

    public CategoriesController(LeVietTravelDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var query = _context.Categories.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => c.Name.Contains(keyword) || (c.Description != null && c.Description.Contains(keyword)));
        }

        var totalItems = await query.CountAsync();
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();

        var result = new
        {
            items,
            pagination = new
            {
                page,
                limit,
                totalItems,
                totalPages = (int)Math.Ceiling((double)totalItems / limit)
            }
        };

        return Ok(ApiResponse.Ok(result, "Lấy danh sách danh mục thành công"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy danh mục", 404));
        }
        return Ok(ApiResponse.Ok(category, "Lấy chi tiết danh mục thành công"));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Category model)
    {
        if (string.IsNullOrWhiteSpace(model.Slug))
        {
            model.Slug = model.Name.ToLower().Replace(" ", "-");
        }
        _context.Categories.Add(model);
        await _context.SaveChangesAsync();
        return StatusCode(201, ApiResponse.Ok(model, "Tạo danh mục thành công"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Category model)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy danh mục", 404));
        }

        category.Name = model.Name;
        category.Description = model.Description;
        if (!string.IsNullOrWhiteSpace(model.Slug))
        {
            category.Slug = model.Slug;
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Ok(category, "Cập nhật danh mục thành công"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy danh mục", 404));
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Ok(null, "Xóa danh mục thành công"));
    }
}
