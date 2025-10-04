namespace ECareClinic.Core.DTOs.BookingDtos
{
	public class AvailableSlotsResponseDto
	{
		public string Date { get; set; } = null!;
		public List<TimeSlotDto> TimeSlots { get; set; } = new();
	}

	public class TimeSlotDto
	{
		public string StartTime { get; set; } = null!;
		public bool IsAvailable { get; set; }
	}
}
