namespace ECareClinic.Core.DTOs.BookingDtos
{
    public class DoctorDto 
    {
        public string DoctorId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Specialty { get; set; } = null!;

        public List<VisitTypeDto>? VisitTypes { get; set; }
    }
}
