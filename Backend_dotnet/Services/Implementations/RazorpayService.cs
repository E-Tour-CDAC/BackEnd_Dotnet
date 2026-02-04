using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Backend_dotnet.Configuration;
using Backend_dotnet.DTOs;
using Backend_dotnet.Models.Entities;
using Backend_dotnet.Repositories.Interfaces;
using Backend_dotnet.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Backend_dotnet.Services.Implementations
{
    public class RazorpayService : IPaymentGatewayService
    {
        private readonly RazorpayOptions _options;
        private readonly IPaymentRepository _paymentRepository;

        public RazorpayService(
            IOptions<RazorpayOptions> options,
            IPaymentRepository paymentRepository)
        {
            _options = options.Value;
            _paymentRepository = paymentRepository;
        }

        // =========================
        // Razorpay HTTP Client
        // =========================
        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            var authBytes = Encoding.ASCII.GetBytes($"{_options.KeyId}:{_options.KeySecret}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
            return client;
        }

        // =========================
        // CREATE ORDER
        // =========================
        public async Task<CreateOrderResponseDto> CreateOrder(CreateOrderRequestDto request)
        {
            if (_paymentRepository.ExistsByBookingIdAndStatus(request.BookingId, "SUCCESS"))
                throw new Exception("Payment already completed");

            var initiated = _paymentRepository
                .FindByBookingIdAndStatus(request.BookingId, "INITIATED");

            if (initiated != null)
            {
                return new CreateOrderResponseDto
                {
                    OrderId = initiated.transaction_ref,
                    Amount = (long)(initiated.payment_amount * 100),
                    Currency = "INR"
                };
            }

            var client = CreateClient();

            var payload = new
            {
                amount = 50000, // paise
                currency = "INR",
                receipt = $"booking_{request.BookingId}"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(
                "https://api.razorpay.com/v1/orders",
                content
            );

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to create Razorpay order");

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            var payment = new payment_master
            {
                booking_id = request.BookingId,
                payment_amount = 500,
                payment_status = "INITIATED",
                transaction_ref = json["id"]!.ToString(),
                payment_mode = "RAZORPAY",
                payment_date = DateTime.Now
            };

            _paymentRepository.Save(payment);

            return new CreateOrderResponseDto
            {
                OrderId = json["id"]!.ToString(),
                Amount = (long)json["amount"]!,
                Currency = json["currency"]!.ToString()
            };
        }

        // =========================
        // CONFIRM PAYMENT
        // =========================
        public string ConfirmPayment(string orderId, string paymentId, long amount)
        {
            var payment = _paymentRepository.FindByTransactionRef(orderId)
                ?? throw new Exception("Payment not found");

            if (payment.payment_status == "SUCCESS")
                return "Already confirmed";

            payment.payment_status = "SUCCESS";
            payment.transaction_ref = paymentId;
            payment.payment_date = DateTime.Now;

            _paymentRepository.Save(payment);
            return "Payment confirmed";
        }

        // =========================
        // WEBHOOK HANDLER
        // =========================
        public void HandleWebhook(string payload, string signature)
        {
            var expected = ComputeSignature(payload, _options.WebhookSecret);
            if (!expected.Equals(signature))
                throw new Exception("Invalid webhook signature");

            var json = JObject.Parse(payload);
            var eventType = json["event"]!.ToString();

            if (eventType == "payment.captured")
            {
                var orderId =
                    json["payload"]!["payment"]!["entity"]!["order_id"]!.ToString();

                var payment = _paymentRepository.FindByTransactionRef(orderId);
                if (payment != null)
                {
                    payment.payment_status = "SUCCESS";
                    payment.payment_date = DateTime.Now;
                    _paymentRepository.Save(payment);
                }
            }
        }

        // =========================
        // SIGNATURE VERIFY
        // =========================
        private string ComputeSignature(string payload, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
