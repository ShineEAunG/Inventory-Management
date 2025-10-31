using InventoryManagementSystem.Dtos.Inventory;
using InventoryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        this._categoryService = categoryService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery]string? searchTerm= null)
    {
        var categories = await _categoryService.GetAll(searchTerm);
        return Ok(categories);
    }
    [HttpGet("{categoryId}")]
    public async Task<IActionResult> Get(string categoryId)
    {
        if (!Ulid.TryParse(categoryId, out var id))
            return BadRequest();
        var categoryDetails = await _categoryService.GetById(id);
        return Ok(categoryDetails);
    }
    [Authorize(Policy ="CanAdd")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm]CreateCategoryDto dto)
    {
        var operationResult = await _categoryService.Create(dto);
        if (operationResult.Success)
            return Ok();
        return BadRequest();
    }
    [Authorize(Policy ="CanDelete")]
    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> Delete(string categoryId)
    {
        if (!Ulid.TryParse(categoryId, out var id))
            return BadRequest();
        var operationResult = await _categoryService.Delete(id);
        if (!operationResult.Success)
            return BadRequest();
        return Ok();
    }
    [Authorize(Policy ="CanEdit")]
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromForm] UpdateCategoryDetails details)
    {
        var operationResult = await _categoryService.Update(details);
        if (operationResult.Success)
            return Ok();
        return BadRequest();
    }
}