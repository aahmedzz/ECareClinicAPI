using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.Doctor
{
    public class DoctorSearchRequest
    {
        public bool? IsSpecialist { get; set; } // null = both, false = general, true = specialist
        public int? VisitTypeId { get; set; } // online/clinic/video
        public string When { get; set; } = "anytime"; // "now" or "anytime"
        public DateTime? Date { get; set; } // optional, used when picking specific date
        public string? Specialization { get; set; } // optional text filter
    }

}
