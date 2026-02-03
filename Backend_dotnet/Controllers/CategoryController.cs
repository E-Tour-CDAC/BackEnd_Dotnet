using Backend_dotnet.Services;
using Microsoft.AspNetCore.Mvc;
using Backend_dotnet.Services.Interfaces;
namespace Backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/categories/ids/by-subcat/EUP
        [HttpGet("ids/{subcatCode}")]
        public async Task<IActionResult> GetCategoryIdsBySubCat(string subcatCode)
        {
            var categoryIds = await _categoryService
                .GetCategoryIdsBySubCatAsync(subcatCode);

            if (!categoryIds.Any())
            {
                return NotFound($"No categories found for subcat_code: {subcatCode}");
            }

            return Ok(categoryIds);
        }
    }
}
