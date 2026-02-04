using Backend_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            IEmailService emailService,
            IInvoiceService invoiceService,
            ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _invoiceService = invoiceService;
            _logger = logger;
        }

        /// <summary>
        /// Send booking confirmation email
        /// </summary>
        [HttpPost("booking-confirmation")]
        public async Task<IActionResult> SendBookingMail([FromQuery] int paymentId)
        {
            try
            {
                _logger.LogInformation("Sending booking confirmation for payment {PaymentId}", paymentId);
                await _emailService.SendBookingConfirmationAsync(paymentId);
                return Ok($"Booking confirmation email sent for paymentId = {paymentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send booking confirmation for payment {PaymentId}", paymentId);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Send invoice email with PDF attachment
        /// </summary>
        [HttpPost("invoice")]
        public async Task<IActionResult> SendInvoiceMail([FromQuery] int paymentId)
        {
            try
            {
                _logger.LogInformation("Sending invoice email for payment {PaymentId}", paymentId);

                // Generate PDF
                var pdf = await _invoiceService.GenerateInvoiceAsync(paymentId);

                // Send email with attachment
                await _emailService.SendInvoiceWithAttachmentAsync(paymentId, pdf);

                return Ok($"Invoice email sent for paymentId = {paymentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send invoice email for payment {PaymentId}", paymentId);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
