using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Models
{
	public class Appointment
	{
		[Key]
		public int AppointmentId { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime AppointmentDate { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime StartDate { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? EndDate { get; set; }

		[Required]
		[MaxLength(50)]
		public string Status { get; set; } = null!;

		public bool ReminderSent { get; set; }

		[MaxLength(250)]
		public string? ReasonForVisit { get; set; }

		[MaxLength(100)]
		public string? TypeOfVisit { get; set; }

		// Foreign keys
		public string PatientId { get; set; } = null!;
		public Patient Patient { get; set; } = null!;

		public string DoctorId { get; set; } = null!;
		public Doctor Doctor { get; set; } = null!;
	}
}
