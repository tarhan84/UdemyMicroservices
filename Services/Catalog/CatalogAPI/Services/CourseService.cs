using AutoMapper;
using CatalogAPI.Dtos;
using CatalogAPI.Model;
using CatalogAPI.Settings;
using MongoDB.Driver;
using Shared.Dtos;

namespace CatalogAPI.Services
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseService> _logger;

        public CourseService(IMongoCollection<Category> courseCollection, 
            IMapper mapper,
            IDatabaseSettings databaseSettings,
            ICategoryService categoryService,
            ILogger<CourseService> logger)
        {
            _mapper = mapper;

            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryService = categoryService;
            _logger = logger;
        }

        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find(course => true).ToListAsync();

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = _categoryService.GetById(course.CategoryId);
                }
                return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
            }
            return Response<List<CourseDto>>.Success(new List<CourseDto>(), 200);
        }

        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            var course = await _courseCollection.Find<Course>(course => course.Id == id).FirstOrDefaultAsync();
            if (course == null)
                return Response<CourseDto>.Fail($"Course not found with Id : {id}", 404);
            course.Category = _categoryService.GetById(course.CategoryId);
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
        }

        public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find(course => course.UserId == userId).ToListAsync();

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = _categoryService.GetById(course.CategoryId);
                }
                return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
            }
            return Response<List<CourseDto>>.Success(new List<CourseDto>(), 200);
        }

        public async Task<Response<CourseDto>> createAsync(CourseCreateDto courseCreateDto)
        {
            var course = _mapper.Map<Course>(courseCreateDto);
            course.CreatedTime = DateTime.Now;
            await _courseCollection.InsertOneAsync(course);
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(courseCreateDto), 200);
        }

        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);
            var result = await _courseCollection.FindOneAndReplaceAsync(course => course.Id == updateCourse.Id, updateCourse);
            if (result == null)
                return Response<NoContent>.Fail($"Course not found with Id: {courseUpdateDto.Id}", 404);
            return Response<NoContent>.Success(204);
        }

        public async Task<Response<NoContent>> DeleteByIdAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(id);
            if (result.DeletedCount > 0)
                return Response<NoContent>.Success(204);
            return Response<NoContent>.Fail($"Course not found with Id : {id}", 404);
        }
    }

}
