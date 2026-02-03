using System.Collections.Generic;
using System.Threading.Tasks;
using Backend_dotnet.Models.Entities;

namespace Backend_dotnet.Repositories.Interfaces
{
    public interface ITourRepository
    {
        // Category Level Data
        Task<category_master?> GetCategoryWithDetailsAsync(int categoryId);
        Task<IEnumerable<category_master>> GetCategoriesWithDetailsAsync(IEnumerable<int> categoryIds);


        // Tour (Scheduled) Operations
        Task<IEnumerable<tour_master>> GetAllToursAsync();
        Task<tour_master?> GetTourByIdAsync(int id);
        Task<tour_master> AddTourAsync(tour_master tour);
        Task<tour_master> UpdateTourAsync(tour_master tour);
        Task<bool> DeleteTourAsync(int id);
        Task<IEnumerable<tour_master>> SearchToursAsync(string query);
        Task<IEnumerable<tour_master>> GetToursByCategoryAsync(int categoryId);


        // Tour Guide Operations
        Task<IEnumerable<tour_guide>> GetGuidesByTourIdAsync(int tourId);
        Task<tour_guide?> GetTourGuideByIdAsync(int guideId);
        Task<tour_guide> AddTourGuideAsync(tour_guide guide);
        Task<tour_guide> UpdateTourGuideAsync(tour_guide guide);
        Task<bool> DeleteTourGuideAsync(int guideId);
    }
}
