using Backend_dotnet.DTOs;
using Backend_dotnet.Models.Entities;
using Backend_dotnet.Repositories.Interfaces;
using Backend_dotnet.Services.Interfaces;

namespace Backend_dotnet.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public PaymentDto MakePayment(int bookingId, decimal amount)
        {
            if (_paymentRepository.ExistsByBookingIdAndStatus(bookingId, "SUCCESS"))
                throw new Exception("Payment already completed");

            var payment = new payment_master
            {
                booking_id = bookingId,
                payment_amount = amount,
                payment_status = "INITIATED",
                payment_mode = "RAZORPAY",
                payment_date = DateTime.Now
            };

            _paymentRepository.Save(payment);
            return MapToDto(payment);
        }

        public PaymentDto GetPaymentById(int paymentId)
        {
            var payment = _paymentRepository
                .FindAllByBookingId(paymentId)
                .FirstOrDefault();

            if (payment == null)
                throw new Exception("Payment not found");

            return MapToDto(payment);
        }

        public PaymentDto GetSuccessfulPayment(int bookingId)
        {
            var payment = _paymentRepository
                .FindByBookingIdAndStatus(bookingId, "SUCCESS");

            if (payment == null)
                throw new Exception("No successful payment");

            return MapToDto(payment);
        }

        private PaymentDto MapToDto(payment_master p)
        {
            return new PaymentDto
            {
                PaymentId = p.payment_id,
                BookingId = p.booking_id,
                PaymentAmount = p.payment_amount,
                PaymentStatus = p.payment_status,
                PaymentMode = p.payment_mode,
                TransactionRef = p.transaction_ref,
                PaymentDate = p.payment_date
            };
        }
    }
}
