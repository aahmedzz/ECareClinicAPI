namespace ECareClinic.Core.DTOs.BookingDtos
{
    public class BookAppointmentDto
    {
        public string PatientId { get; set; } = null!;
        public string DoctorId { get; set; } = null!;
        public int VisitTypeId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime AppointmentDate { get; set; }

        public string reasonForVisit { get; set; } = null!;
    }
}
