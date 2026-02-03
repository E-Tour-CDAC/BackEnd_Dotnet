
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_dotnet.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<int>> GetCategoryIdsBySubCatAsync(string subcatCode);
    }
}

