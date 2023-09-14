using AutoMapper;
using CatalogAPI.Dtos;
using CatalogAPI.Model;
using CatalogAPI.Settings;
using MongoDB.Driver;
using Shared.Dtos;

namespace CatalogAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public CategoryService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            _mapper = mapper;

            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
        }

        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _categoryCollection.Find(category => true).ToListAsync();
            return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories), 200);
        }

        public async Task<Response<CategoryDto>> CreateAsync(CategoryCreateDto categoryCreateDto)
        {
            var category = _mapper.Map<Category>(categoryCreateDto);
            await _categoryCollection.InsertOneAsync(category);
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }

        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            var category = await _categoryCollection.Find<Category>(item => item.Id == id).FirstOrDefaultAsync();
            if (category == null)
                return Response<CategoryDto>.Fail($"Catogory not found with id : {id}", 404);
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }

        public Category GetById(string id)
        {
            return _categoryCollection.Find<Category>(item => item.Id == id).FirstOrDefault();
        }
    }
}
