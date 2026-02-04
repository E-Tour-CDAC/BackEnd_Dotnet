using AutoMapper;
using Backend_dotnet.DTOs.Booking;
using Backend_dotnet.DTOs.Common;
using Backend_dotnet.Models.Entities;
using Backend_dotnet.Repositories.Interfaces;
using Backend_dotnet.Services.Interfaces;

namespace Backend_dotnet.Services.Implementations
{
    /// <summary>
    /// Service implementation for booking business logic
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository bookingRepository,
            IMapper mapper,
            ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BookingResponseDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting booking {BookingId}", id);

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);

            if (booking == null)
            {
                _logger.LogWarning("Booking {BookingId} not found", id);
                throw new KeyNotFoundException($"Booking with ID {id} not found");
            }

            return MapToResponseDto(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllAsync()
        {
            _logger.LogInformation("Getting all bookings");

            var allBookings = await _bookingRepository.GetAllAsync();
            return allBookings.Select(MapToResponseDto).ToList();
        }

        public async Task<IEnumerable<BookingResponseDto>> GetCustomerBookingsAsync(int customerId)
        {
            _logger.LogInformation("Getting bookings for customer {CustomerId}", customerId);

            var bookings = await _bookingRepository.GetCustomerBookingsAsync(customerId);
            return bookings.Select(MapToResponseDto);
        }

        public async Task<BookingResponseDto> CreateAsync(BookingCreateDto dto)
        {
            _logger.LogInformation("Creating booking for customer {CustomerId}, tour {TourId}",
                dto.CustomerId, dto.TourId);

            var booking = new booking_header
            {
                booking_date = DateOnly.FromDateTime(DateTime.Now),
                customer_id = dto.CustomerId,
                tour_id = dto.TourId,
                no_of_pax = dto.NoOfPax,
                tour_amount = dto.TourAmount,
                taxes = dto.Taxes,
                status_id = dto.StatusId == 0 ? 1 : dto.StatusId,
                total_amount = dto.TourAmount + dto.Taxes
            };

            var createdBooking = await _bookingRepository.AddAsync(booking);

            _logger.LogInformation("Booking {BookingId} created successfully", createdBooking.booking_id);

            var bookingWithDetails = await _bookingRepository.GetBookingWithDetailsAsync(createdBooking.booking_id);
            return MapToResponseDto(bookingWithDetails);
        }

        public async Task<int> GetPaymentStatusAsync(int bookingId)
        {
            _logger.LogInformation("Getting payment status for booking {BookingId}", bookingId);
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {bookingId} not found");
            }
            return booking.status_id;
        }

        private BookingResponseDto MapToResponseDto(booking_header booking)
        {
            return new BookingResponseDto
            {
                BookingId = booking.booking_id,
                BookingDate = booking.booking_date,
                CustomerId = booking.customer_id,
                CustomerName = booking.customer != null ? $"{booking.customer.first_name} {booking.customer.last_name}" : "Unknown",
                TourId = booking.tour_id,
                TourName = booking.tour?.category?.category_name ?? "Unknown",
                NoOfPax = booking.no_of_pax,
                TourAmount = booking.tour_amount,
                Taxes = booking.taxes,
                TotalAmount = booking.total_amount ?? 0,
                StatusId = booking.status_id,
                StatusName = booking.status?.status_name ?? "Unknown"
            };
        }
    }
}