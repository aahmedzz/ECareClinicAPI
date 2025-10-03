namespace ECareClinic.Core.DTOs.BookingDtos
{
    public class SlotDto
    {
        public int ScheduleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
