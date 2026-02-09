using Backend_dotnet.Data;
using Backend_dotnet.DTOs;
using Backend_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBookingService _bookingService;

        public AdminController(AppDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var totalTours = await _context.tour_master.CountAsync();
                var totalBookings = await _context.booking_header.CountAsync();
                var totalCustomers = await _context.customer_master.CountAsync();
                var totalRevenue = await _context.booking_header.SumAsync(b => b.total_amount ?? 0);

                return Ok(new
                {
                    totalTours,
                    totalBookings,
                    totalCustomers,
                    totalRevenue
                });
            }
            catch (Exception ex)
            {
                // Log exception if possible, but return 200 with 0s to avoid frontend break
                return Ok(new
                {
                    totalTours = 0,
                    totalBookings = 0,
                    totalCustomers = 0,
                    totalRevenue = 0,
                    error = ex.Message
                });
            }
        }

        [HttpGet("recent-bookings")]
        public async Task<IActionResult> GetRecentBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllAsync();
                var recentBookings = bookings.OrderByDescending(b => b.BookingId).Take(10).ToList();
                return Ok(recentBookings);
            }
            catch (Exception)
            {
                return Ok(new List<object>()); // Return empty list on error
            }
        }
    }
}
