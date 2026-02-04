using Backend_dotnet.DTOs;
using Backend_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend_dotnet.Controllers
{
    [ApiController]
    [Route("payment-gateway")]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly IPaymentGatewayService _service;

        public PaymentGatewayController(IPaymentGatewayService service)
        {
            _service = service;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto dto)
        {
            var result = await _service.CreateOrder(dto);
            return Ok(result);
        }

        [HttpPost("confirm-payment")]
        public IActionResult ConfirmPayment(
            [FromQuery] string orderId,
            [FromQuery] string paymentId,
            [FromQuery] long amount)
        {
            return Ok(_service.ConfirmPayment(orderId, paymentId, amount));
        }

        [HttpPost("webhook")]
        public IActionResult Webhook(
            [FromBody] string payload,
            [FromHeader(Name = "X-Razorpay-Signature")] string signature)
        {
            _service.HandleWebhook(payload, signature);
            return Ok();
        }
    }
}
