using System;

namespace Backend_dotnet.DTOs.Tour
{
    public class TourDto
    {
        public int TourId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int DepartureId { get; set; }
        public DateOnly DepartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int NoOfDays { get; set; }
        public List<DepartureDto> AvailableDepartures { get; set; } = new List<DepartureDto>();
    }
}
