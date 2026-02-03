using Backend_dotnet.DTOs;

namespace Backend_dotnet.Services.Interfaces
{
    public interface IPaymentService
    {
        PaymentDto MakePayment(int bookingId, decimal amount);
        PaymentDto GetPaymentById(int paymentId);
        PaymentDto GetSuccessfulPayment(int bookingId);
    }
}
