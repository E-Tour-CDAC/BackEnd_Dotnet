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
        //private readonly ICustomerRepository _customerRepository;
        //private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository bookingRepository,
            IMapper mapper,
            ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            //_customerRepository = customerRepository;
            //_tourRepository = tourRepository;
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

        //public async Task<BookingResponseDto> GetDetailsAsync(int id)
        //{
        //    _logger.LogInformation("Getting booking details for {BookingId}", id);

        //    var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);

        //    if (booking == null)
        //    {
        //        _logger.LogWarning("Booking {BookingId} not found", id);
        //        throw new KeyNotFoundException($"Booking with ID {id} not found");
        //    }

        //    return MapToResponseDto(booking);
        //}

        public async Task<IEnumerable<BookingResponseDto>> GetAllAsync()
        {
            _logger.LogInformation("Getting all bookings (page {PageNumber}, size {PageSize})");

            var allBookings = await _bookingRepository.GetAllAsync();
            var totalCount = allBookings.Count();

            return allBookings.Select(MapToResponseDto).ToList();

            
        }

        //public async Task<IEnumerable<BookingListDto>> GetCustomerBookingsAsync(int customerId)
        //{
        //    _logger.LogInformation("Getting bookings for customer {CustomerId}", customerId);

        //    // Verify customer exists
        //    var customer = await _customerRepository.GetByIdAsync(customerId);
        //    if (customer == null)
        //    {
        //        _logger.LogWarning("Customer {CustomerId} not found", customerId);
        //        throw new KeyNotFoundException($"Customer with ID {customerId} not found");
        //    }

        //    var bookings = await _bookingRepository.GetCustomerBookingsAsync(customerId);
        //    return bookings.Select(MapToListDto);
        //}

        //public async Task<IEnumerable<BookingListDto>> GetBookingsByStatusAsync(int statusId)
        //{
        //    _logger.LogInformation("Getting bookings with status {StatusId}", statusId);

        //    var bookings = await _bookingRepository.GetBookingsByStatusAsync(statusId);
        //    return bookings.Select(MapToListDto);
        //}

        //public async Task<BookingResponseDto> CreateAsync(BookingCreateDto dto)
        //{
        //    _logger.LogInformation("Creating booking for customer {CustomerId}, tour {TourId}",
        //        dto.CustomerId, dto.TourId);

        //    // Validate customer exists
        //    var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
        //    if (customer == null)
        //    {
        //        _logger.LogWarning("Customer {CustomerId} not found", dto.CustomerId);
        //        throw new KeyNotFoundException($"Customer with ID {dto.CustomerId} not found");
        //    }

        //    // Validate tour exists
        //    var tour = await _tourRepository.GetByIdAsync(dto.TourId);
        //    if (tour == null)
        //    {
        //        _logger.LogWarning("Tour {TourId} not found", dto.TourId);
        //        throw new KeyNotFoundException($"Tour with ID {dto.TourId} not found");
        //    }

        //    // Validate passenger count matches
        //    if (dto.Passengers.Count != dto.NoOfPax)
        //    {
        //        throw new InvalidOperationException(
        //            $"Number of passengers ({dto.Passengers.Count}) does not match NoOfPax ({dto.NoOfPax})");
        //    }

        //    // Create booking entity
        //    var booking = new booking_header
        //    {
        //        booking_date = DateOnly.FromDateTime(DateTime.Now),
        //        customer_id = dto.CustomerId,
        //        tour_id = dto.TourId,
        //        no_of_pax = dto.NoOfPax,
        //        tour_amount = dto.TourAmount,
        //        taxes = dto.Taxes,
        //        status_id = 1, // Default: Pending
        //        passenger = dto.Passengers.Select(p => new passenger
        //        {
        //            pax_name = p.PaxName,
        //            pax_birthdate = p.PaxBirthdate,
        //            pax_type = p.PaxType,
        //            pax_amount = p.PaxAmount
        //        }).ToList()
        //    };

        //    var createdBooking = await _bookingRepository.AddAsync(booking);

        //    _logger.LogInformation("Booking {BookingId} created successfully", createdBooking.booking_id);

        //    // Reload with details
        //    var bookingWithDetails = await _bookingRepository.GetBookingWithDetailsAsync(createdBooking.booking_id);
        //    return MapToResponseDto(bookingWithDetails);
        //}

        //public async Task<BookingResponseDto> UpdateAsync(int id, BookingUpdateDto dto)
        //{
        //    _logger.LogInformation("Updating booking {BookingId}", id);

        //    var booking = await _bookingRepository.GetByIdAsync(id);
        //    if (booking == null)
        //    {
        //        _logger.LogWarning("Booking {BookingId} not found", id);
        //        throw new KeyNotFoundException($"Booking with ID {id} not found");
        //    }

        //    // Update only provided fields
        //    if (dto.StatusId.HasValue)
        //        booking.status_id = dto.StatusId.Value;

        //    if (dto.NoOfPax.HasValue)
        //        booking.no_of_pax = dto.NoOfPax.Value;

        //    if (dto.TourAmount.HasValue)
        //        booking.tour_amount = dto.TourAmount.Value;

        //    if (dto.Taxes.HasValue)
        //        booking.taxes = dto.Taxes.Value;

        //    await _bookingRepository.UpdateAsync(booking);

        //    _logger.LogInformation("Booking {BookingId} updated successfully", id);

        //    var updatedBooking = await _bookingRepository.GetBookingWithDetailsAsync(id);
        //    return MapToResponseDto(updatedBooking);
        //}

        //public async Task<bool> UpdateStatusAsync(int id, int statusId)
        //{
        //    _logger.LogInformation("Updating booking {BookingId} status to {StatusId}", id, statusId);

        //    var result = await _bookingRepository.UpdateBookingStatusAsync(id, statusId);

        //    if (!result)
        //    {
        //        _logger.LogWarning("Failed to update booking {BookingId} status", id);
        //    }

        //    return result;
        //}

        //public async Task<bool> CancelBookingAsync(int id)
        //{
        //    _logger.LogInformation("Cancelling booking {BookingId}", id);

        //    // Assuming status_id 4 = Cancelled
        //    return await _bookingRepository.UpdateBookingStatusAsync(id, 4);
        //}

        //public async Task<bool> DeleteAsync(int id)
        //{
        //    _logger.LogInformation("Deleting booking {BookingId}", id);

        //    var booking = await _bookingRepository.GetByIdAsync(id);
        //    if (booking == null)
        //    {
        //        _logger.LogWarning("Booking {BookingId} not found", id);
        //        throw new KeyNotFoundException($"Booking with ID {id} not found");
        //    }

        //    return await _bookingRepository.DeleteAsync(id);
        //}

        //public async Task<Dictionary<string, int>> GetBookingStatisticsAsync()
        //{
        //    _logger.LogInformation("Getting booking statistics");
        //    return await _bookingRepository.GetBookingCountByStatusAsync();
        //}

        //// Helper mapping methods
        private BookingResponseDto MapToResponseDto(booking_header booking)
        {
            return new BookingResponseDto
            {
                BookingId = booking.booking_id,
                BookingDate = booking.booking_date,
                CustomerId = booking.customer_id,
                CustomerName = $"{booking.customer.first_name} {booking.customer.last_name}",
                TourId = booking.tour_id,
                TourName = booking.tour.category.category_name,
                NoOfPax = booking.no_of_pax,
                TourAmount = booking.tour_amount,
                Taxes = booking.taxes,
                TotalAmount = booking.total_amount ?? 0,
                StatusId = booking.status_id,
                StatusName = booking.status.status_name
            };
        }

        //private BookingDetailsDto MapToDetailsDto(booking_header booking)
        //{
        //    var totalPaid = booking.payment_master?.Sum(p => p.payment_amount) ?? 0;
        //    var totalAmount = booking.total_amount ?? 0;
        //    var balanceDue = totalAmount - totalPaid;

        //    return new BookingDetailsDto
        //    {
        //        BookingId = booking.booking_id,
        //        BookingDate = booking.booking_date,
        //        CustomerId = booking.customer_id,
        //        CustomerName = $"{booking.customer.first_name} {booking.customer.last_name}",
        //        CustomerEmail = booking.customer.email,
        //        CustomerPhone = booking.customer.phone,
        //        TourId = booking.tour_id,
        //        CategoryName = booking.tour.category.category_name,
        //        DepartureDate = booking.tour.departure.depart_date,
        //        EndDate = booking.tour.departure.end_date,
        //        NoOfDays = booking.tour.departure.no_of_days,
        //        NoOfPax = booking.no_of_pax,
        //        TourAmount = booking.tour_amount,
        //        Taxes = booking.taxes,
        //        TotalAmount = totalAmount,
        //        StatusId = booking.status_id,
        //        StatusName = booking.status.status_name,
        //        Passengers = booking.passenger.Select(p => new PassengerDto
        //        {
        //            PaxId = p.pax_id,
        //            BookingId = p.booking_id,
        //            PaxName = p.pax_name,
        //            PaxBirthdate = p.pax_birthdate,
        //            Age = CalculateAge(p.pax_birthdate),
        //            PaxType = p.pax_type,
        //            PaxAmount = p.pax_amount
        //        }).ToList(),
        //        TotalPaid = totalPaid,
        //        BalanceDue = balanceDue,
        //        IsFullyPaid = balanceDue <= 0
        //    };
        //}

        //private BookingListDto MapToListDto(booking_header booking)
        //{
        //    return new BookingListDto
        //    {
        //        BookingId = booking.booking_id,
        //        BookingDate = booking.booking_date,
        //        CustomerName = $"{booking.customer.first_name} {booking.customer.last_name}",
        //        TourName = booking.tour.category.category_name,
        //        NoOfPax = booking.no_of_pax,
        //        TotalAmount = booking.total_amount ?? 0,
        //        StatusName = booking.status.status_name,
        //        DepartureDate = booking.tour.departure.depart_date
        //    };
        //}

        //private int CalculateAge(DateOnly birthdate)
        //{
        //    var today = DateOnly.FromDateTime(DateTime.Today);
        //    int age = today.Year - birthdate.Year;
        //    if (birthdate > today.AddYears(-age)) age--;
        //    return age;
        //}
    }
}