using CatalogAPI.Dtos;
using CatalogAPI.Model;
using Shared.Dtos;

namespace CatalogAPI.Services
{
    public interface ICategoryService
    {
        Task<Response<List<CategoryDto>>> GetAllAsync();
        Task<Response<CategoryDto>> CreateAsync(CategoryCreateDto category);
        Task<Response<CategoryDto>> GetByIdAsync(string id);
        Category GetById(string id);
    }
}
