namespace Backend_dotnet.DTOs
{
    public class TourGuideDto
    {
        public int TourGuideId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
    }
}
