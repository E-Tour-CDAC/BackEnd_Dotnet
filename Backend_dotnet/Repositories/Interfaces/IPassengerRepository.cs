using Backend_dotnet.Models.Entities;

namespace Backend_dotnet.Repositories.Interfaces
{
    public interface IPassengerRepository : IGenericRepository<passenger>
    {
        // Equivalent to Spring Data JPA: findByBookingId(Integer bookingId)
        Task<IEnumerable<passenger>> GetByBookingIdAsync(int bookingId);
    }
}
