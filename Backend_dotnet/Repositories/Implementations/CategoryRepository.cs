using Backend_dotnet.Data;
using Backend_dotnet.Models.Entities;
using Backend_dotnet.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend_dotnet.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<category_master>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        // Get categories by subcat_code (e.g., "EUP", "KSH", "SEA")
        public async Task<IEnumerable<category_master>> GetBySubCatCodeAsync(string subcatCode)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.subcat_code == subcatCode)
                .ToListAsync();
        }

        // Get only category IDs by subcat_code
        public async Task<IEnumerable<int>> GetCategoryIdsBySubCatAsync(string subcatCode)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.subcat_code == subcatCode)
                .Select(c => c.category_id)
                .ToListAsync();
        }

        // Get category by cat_code and subcat_code combination
        public async Task<category_master> GetByCatAndSubCatCodeAsync(string catCode, string subcatCode)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.cat_code == catCode && c.subcat_code == subcatCode);
        }

        // Get categories by cat_code (e.g., "DOM", "INT", "VSL", "CKD")
        public async Task<IEnumerable<category_master>> GetByCatCodeAsync(string catCode)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.cat_code == catCode)
                .ToListAsync();
        }

        // Get categories where jump_flag is true
        public async Task<IEnumerable<category_master>> GetCategoriesWithJumpFlagAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.jump_flag == true)
                .ToListAsync();
        }

        // Check if category code combination exists
        public async Task<bool> CategoryCodeExistsAsync(string catCode, string subcatCode)
        {
            return await AnyAsync(c => c.cat_code == catCode && c.subcat_code == subcatCode);
        }
    }
}