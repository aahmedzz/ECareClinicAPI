using ECareClinic.Core.Entities;
using ECareClinic.Core.Enums;
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
		public TimeSpan StartTime { get; set; }

		[DataType(DataType.DateTime)]
		public TimeSpan? EndTime { get; set; }

		[Required]
		public AppointmentStatus Status { get; set; }

		public bool ReminderSent { get; set; }

		[MaxLength(250)]
		public string? ReasonForVisit { get; set; }


        public VisitType VisitType { get; set; }
        public string PatientId { get; set; } = null!;
		public Patient Patient { get; set; } = null!;

		public string DoctorId { get; set; } = null!;
		public Doctor Doctor { get; set; } = null!;

        public int ScheduleId { get; set; }
        public DoctorSchedule Schedule { get; set; } = null!;
    }
}
