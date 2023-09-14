using CatalogAPI.Dtos;
using CatalogAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace CatalogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : CustomBaseController
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return CreateActionResult(result);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return CreateActionResult(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CategoryCreateDto category)
        {
            var result = await _categoryService.CreateAsync(category);
            return CreateActionResult(result);
        }
    }
}
