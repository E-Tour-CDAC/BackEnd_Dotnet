using Backend_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("pay")]
        public IActionResult Pay([FromQuery] int bookingId, [FromQuery] decimal amount)
        {
            return Ok(_paymentService.MakePayment(bookingId, amount));
        }

        [HttpGet("{id}")]
        public IActionResult GetPayment(int id)
        {
            return Ok(_paymentService.GetPaymentById(id));
        }

        [HttpGet("receipt/{bookingId}")]
        public IActionResult GetReceipt(int bookingId)
        {
            return Ok(_paymentService.GetSuccessfulPayment(bookingId));
        }
    }
}
