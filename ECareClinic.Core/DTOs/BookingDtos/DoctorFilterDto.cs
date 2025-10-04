using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.BookingDtos
{
	public enum AppointmentTimeOption
	{
		Anytime,
		Today
	}
	public class DoctorFilterDto
	{
		public bool GeneralDoctorTypes { get; set; } = true;
		public int? SpecialtyId { get; set; }
		public AppointmentTimeOption AppointmentTime { get; set; } = AppointmentTimeOption.Anytime;
		public bool InPerson { get; set; }
		public bool VideoCall { get; set; }
	}
}
