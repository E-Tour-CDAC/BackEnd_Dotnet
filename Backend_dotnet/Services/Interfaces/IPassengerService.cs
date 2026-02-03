using Backend_dotnet.DTOs;

namespace Backend_dotnet.Services.Interfaces
{
    public interface IPassengerService
    {
        PassengerDto AddPassenger(PassengerDto passengerDto);
        PassengerDto GetPassengerById(int id);
        List<PassengerDto> GetPassengersByBookingId(int bookingId);
    }
}