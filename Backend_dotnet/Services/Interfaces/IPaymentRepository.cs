using Backend_dotnet.Models.Entities;

namespace Backend_dotnet.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        bool ExistsByBookingIdAndStatus(int bookingId, string status);
        payment_master? FindByBookingIdAndStatus(int bookingId, string status);
        payment_master? FindByTransactionRef(string transactionRef);
        List<payment_master> FindAllByBookingId(int bookingId);
        payment_master Save(payment_master payment);
    }
}
