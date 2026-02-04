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
        public async Task<IActionResult> Pay([FromQuery] int bookingId, 
            [FromQuery] string paymentMode,
            [FromQuery] string transactionRef,
            [FromQuery] string paymentStatus,
            [FromQuery] decimal paymentAmount)
        {
            return Ok(await _paymentService.MakePayment(bookingId, paymentMode, transactionRef, paymentStatus, paymentAmount));
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
