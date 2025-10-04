using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.BookingDtos
{
	public class BookAppointmentResponseDto
	{
		public int AppointmentId { get; set; }
		public string Message { get; set; } = null!;
		public string Status { get; set; } = null!;
	}
}
