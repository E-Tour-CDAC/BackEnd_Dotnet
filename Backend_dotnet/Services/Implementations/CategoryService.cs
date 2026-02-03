using Backend_dotnet.Data;
using Backend_dotnet.Repositories.Interfaces;
using Backend_dotnet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend_dotnet.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository=categoryRepository;
        }

        public async Task<IEnumerable<int>> GetCategoryIdsBySubCatAsync(string subcatCode)
        {
            return await _categoryRepository.GetCategoryIdsBySubCatAsync(subcatCode);
        }
    }
}
