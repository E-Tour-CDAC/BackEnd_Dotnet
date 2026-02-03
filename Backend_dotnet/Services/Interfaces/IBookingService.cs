using Backend_dotnet.DTOs.Booking;
using Backend_dotnet.DTOs.Common;

namespace Backend_dotnet.Services.Interfaces
{
    
    /// Service interface for booking business logic
    
    public interface IBookingService
    {
        Task<BookingResponseDto> GetByIdAsync(int id);
        //Task<BookingDetailsDto> GetDetailsAsync(int id);
        Task<IEnumerable<BookingResponseDto>> GetAllAsync();
        
        //Task<IEnumerable<BookingListDto>> GetCustomerBookingsAsync(int customerId);
        //Task<IEnumerable<BookingListDto>> GetBookingsByStatusAsync(int statusId);
        //Task<BookingResponseDto> CreateAsync(BookingCreateDto dto);
        //Task<BookingResponseDto> UpdateAsync(int id, BookingUpdateDto dto);
        //Task<bool> UpdateStatusAsync(int id, int statusId);
        //Task<bool> CancelBookingAsync(int id);
        //Task<bool> DeleteAsync(int id);
        //Task<Dictionary<string, int>> GetBookingStatisticsAsync();
    }
}