using Backend_dotnet.DTOs.Booking;
using Backend_dotnet.DTOs.Common;
using Backend_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_dotnet.Controllers
{
     
    /// Controller for booking management
     
    [ApiController]
    [Route("api/[controller]")]
    
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

         
        /// Get all bookings with pagination
         
        [HttpGet]
        //[Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Getting all bookings");
            var result = await _bookingService.GetAllAsync();
            return Ok(result);
        }


        /// Get booking by ID

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetById(int id)
        {
            _logger.LogInformation("Getting booking {BookingId}", id);
            var booking = await _bookingService.GetByIdAsync(id);
            return Ok(booking);
        }

         
        /// Get detailed booking information
         
        //[HttpGet("{id}/details")]
        //public async Task<ActionResult<BookingDetailsDto>> GetDetails(int id)
        //{
        //    _logger.LogInformation("Getting booking details {BookingId}", id);
        //    var booking = await _bookingService.GetDetailsAsync(id);
        //    return Ok(booking);
        //}

         
        ///// Get bookings for a specific customer
         
        //[HttpGet("customer/{customerId}")]
        //public async Task<ActionResult<IEnumerable<BookingListDto>>> GetCustomerBookings(int customerId)
        //{
        //    _logger.LogInformation("Getting bookings for customer {CustomerId}", customerId);
        //    var bookings = await _bookingService.GetCustomerBookingsAsync(customerId);
        //    return Ok(bookings);
        //}

         
        ///// Get bookings by status
         
        //[HttpGet("status/{statusId}")]
        //[Authorize(Roles = "ADMIN")]
        //public async Task<ActionResult<IEnumerable<BookingListDto>>> GetByStatus(int statusId)
        //{
        //    _logger.LogInformation("Getting bookings with status {StatusId}", statusId);
        //    var bookings = await _bookingService.GetBookingsByStatusAsync(statusId);
        //    return Ok(bookings);
        //}

         
        ///// Get booking statistics
         
        //[HttpGet("statistics")]
        //[Authorize(Roles = "ADMIN")]
        //public async Task<ActionResult<Dictionary<string, int>>> GetStatistics()
        //{
        //    _logger.LogInformation("Getting booking statistics");
        //    var stats = await _bookingService.GetBookingStatisticsAsync();
        //    return Ok(stats);
        //}

         
        ///// Create new booking
         
        //[HttpPost]
        //public async Task<ActionResult<BookingResponseDto>> Create([FromBody] BookingCreateDto dto)
        //{
        //    _logger.LogInformation("Creating new booking for customer {CustomerId}", dto.CustomerId);

        //    var booking = await _bookingService.CreateAsync(dto);

        //    return CreatedAtAction(
        //        nameof(GetById),
        //        new { id = booking.BookingId },
        //        booking
        //    );
        //}

         
        ///// Update booking
         
        //[HttpPut("{id}")]
        //[Authorize(Roles = "ADMIN")]
        //public async Task<ActionResult<BookingResponseDto>> Update(int id, [FromBody] BookingUpdateDto dto)
        //{
        //    _logger.LogInformation("Updating booking {BookingId}", id);
        //    var booking = await _bookingService.UpdateAsync(id, dto);
        //    return Ok(booking);
        //}

         
        ///// Update booking status
         
        //[HttpPatch("{id}/status/{statusId}")]
        //[Authorize(Roles = "ADMIN")]
        //public async Task<ActionResult> UpdateStatus(int id, int statusId)
        //{
        //    _logger.LogInformation("Updating booking {BookingId} status to {StatusId}", id, statusId);

        //    var result = await _bookingService.UpdateStatusAsync(id, statusId);

        //    if (!result)
        //        return NotFound(new { message = $"Booking with ID {id} not found" });

        //    return NoContent();
        //}

         
        ///// Cancel booking
         
        //[HttpPost("{id}/cancel")]
        //public async Task<ActionResult> Cancel(int id)
        //{
        //    _logger.LogInformation("Cancelling booking {BookingId}", id);

        //    var result = await _bookingService.CancelBookingAsync(id);

        //    if (!result)
        //        return NotFound(new { message = $"Booking with ID {id} not found" });

        //    return NoContent();
        //}
    }
}