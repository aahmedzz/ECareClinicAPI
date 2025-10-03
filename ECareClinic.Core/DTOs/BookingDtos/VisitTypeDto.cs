namespace ECareClinic.Core.DTOs.BookingDtos
{
    public class VisitTypeDto 
    {
        public int VisitTypeId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
    }
}
