using System.Collections.Generic;
using System.Threading.Tasks;
using Backend_dotnet.DTOs.Tour;

namespace Backend_dotnet.Services.Interfaces
{
    public interface ITourService
    {
        // Aggregate Data
        Task<TourDetailsDto?> GetTourPackageDetailsAsync(int categoryId);
        Task<IEnumerable<TourDetailsDto>> GetTourPackagesDetailsAsync(IEnumerable<int> categoryIds);

        // Tour CRUD
        Task<IEnumerable<TourDto>> GetAllToursAsync();
        Task<TourDto?> GetTourByIdAsync(int id);
        Task<TourDto> CreateTourAsync(CreateTourDto dto);
        Task<TourDto> UpdateTourAsync(int id, UpdateTourDto dto);
        Task<bool> DeleteTourAsync(int id);
        Task<IEnumerable<TourDto>> SearchToursAsync(string query);

        // Tour Guide Management
        Task<IEnumerable<TourGuideDto>> GetGuidesForTourAsync(int tourId);
        Task<TourGuideDto> AddGuideToTourAsync(int tourId, CreateTourGuideDto dto);
        Task<bool> RemoveGuideFromTourAsync(int tourId, int guideId);
        Task<TourGuideDto> UpdateGuideAsync(int tourId, int guideId, CreateTourGuideDto dto);
    }
}
