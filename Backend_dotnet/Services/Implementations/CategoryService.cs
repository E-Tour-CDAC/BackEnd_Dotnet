using Backend_dotnet.Data;
using Backend_dotnet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend_dotnet.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetCategoryIdsBySubCatAsync(string subcatCode)
        {
            return await _context.category_master
                .AsNoTracking()
                .Where(c => c.subcat_code == subcatCode)
                .Select(c => c.category_id)
                .ToListAsync();
        }
    }
}
