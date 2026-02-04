using Backend_dotnet.DTOs;

namespace Backend_dotnet.Services.Interfaces
{
    public interface IPaymentGatewayService
    {
        Task<CreateOrderResponseDto> CreateOrder(CreateOrderRequestDto request);
        string ConfirmPayment(string orderId, string paymentId, long amount);
        void HandleWebhook(string payload, string signature);
    }
}
