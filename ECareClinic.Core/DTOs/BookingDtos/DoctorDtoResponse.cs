using ECareClinic.Core.Entities;

namespace ECareClinic.Core.DTOs.BookingDtos
{
    public class DoctorDtoResponse 
    {
        public string DoctorId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Specialty { get; set; } = null!;

        public List<string>? AvailableVisitTypes { get; set; }
    }
}
