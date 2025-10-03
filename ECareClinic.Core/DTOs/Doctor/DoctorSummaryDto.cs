namespace ECareClinic.Core.DTOs.Doctor
{
    // Response doctor summary
    public class DoctorSummaryDto
    {
        public string DoctorId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? PhotoUrl { get; set; }
        public string? Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime? NextAvailableSlot { get; set; } // optional hint
    }

}
