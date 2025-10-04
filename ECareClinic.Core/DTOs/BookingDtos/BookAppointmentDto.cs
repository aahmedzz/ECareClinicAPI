using ECareClinic.Core.Enums;

namespace ECareClinic.Core.DTOs.BookingDtos
{
    public class BookAppointmentDto
    {
		public string DoctorId { get; set; } = null!;
		public DateTime Date { get; set; }
		public string StartTime { get; set; } = ""; 
		public string VisitType { get; set; } = null!;
		public string? ReasonForVisit { get; set; }
	}
}
