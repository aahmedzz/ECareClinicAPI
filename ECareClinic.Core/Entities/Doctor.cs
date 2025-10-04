using ECareClinic.Core.Entities;
using ECareClinic.Core.Enums;
using ECareClinic.Core.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Models
{
    public class Doctor
    {
        [Key]
        public string DoctorId { get; set; } = null!;

        [Required, MaxLength(40)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(40)]
        public string LastName { get; set; } = string.Empty;

        public Gender? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(300)]
        public string? PhotoURL { get; set; }

        public int YearsOfExperience { get; set; }

        [MaxLength(50)]
        public string? LicenceNumber { get; set; }

        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; } = null!;

		public VisitType VisitTypes { get; set; }

		// Foreign keys
		public ApplicationUser User { get; set; } = null!;

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();
    }

}
