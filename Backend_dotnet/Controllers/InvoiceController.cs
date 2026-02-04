using Backend_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        /// <summary>
        /// Download invoice PDF for a booking
        /// </summary>
        [HttpGet("{bookingId}/download")]
        public async Task<IActionResult> DownloadInvoice(int bookingId)
        {
            try
            {
                _logger.LogInformation("Downloading invoice for booking {BookingId}", bookingId);

                // First get the payment ID for this booking
                var paymentId = _invoiceService.GetPaymentIdByBookingId(bookingId);
                
                if (paymentId == null)
                {
                    return NotFound(new { message = "No successful payment found for this booking" });
                }

                var pdfBytes = await _invoiceService.GenerateInvoiceAsync(paymentId.Value);

                return File(pdfBytes, "application/pdf", $"Invoice-Booking-{bookingId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate invoice for booking {BookingId}", bookingId);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
