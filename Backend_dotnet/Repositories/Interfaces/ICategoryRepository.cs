using Backend_dotnet.Models.Entities;

namespace Backend_dotnet.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<category_master>
    {
        // Custom methods specific to Category
        Task<IEnumerable<category_master>> GetBySubCatCodeAsync(string subcatCode);
        Task<IEnumerable<int>> GetCategoryIdsBySubCatAsync(string subcatCode);
        Task<category_master> GetByCatAndSubCatCodeAsync(string catCode, string subcatCode);
        Task<IEnumerable<category_master>> GetByCatCodeAsync(string catCode);
        Task<IEnumerable<category_master>> GetCategoriesWithJumpFlagAsync();
        Task<bool> CategoryCodeExistsAsync(string catCode, string subcatCode);
    }
}