using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.Enums;
using ECareClinic.Core.Identity;

namespace ECareClinic.Core.Models
{
	public class Patient
	{
		[Key]
		public string PatientId { get; set; } = null!;
		[Required]
		[MaxLength(40)]
		public string FirstName { get; set; } = string.Empty;
		[Required]
		[MaxLength(40)]
		public string LastName { get; set; } = string.Empty;
		public Gender? Gender { get; set; }
		[DataType(DataType.Date)]
		public DateTime DateOfBirth { get; set; }
		[MaxLength(300)]
		public string? PhotoURL { get; set; }
		[MaxLength(150)]
		public string? Address { get; set; }
		[MaxLength(50)]
		public string? Province { get; set; }
		[MaxLength(50)]
		public string? City { get; set; }

		//Foreign keys
		public ApplicationUser User { get; set; } = null!;

		// Navigation properties
		public ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
		public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
		public ICollection<Payment> Payments { get; set; } = new List<Payment>();
		public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
		public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
	}
}
