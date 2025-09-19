using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.ProfileDtos
{
	public class PatientProfileResponseDto : BaseResponseDto
	{
		public string PatientId { get; set; } = null!;
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;
		public string Gender { get; set; } = null!;
		public DateTime DateOfBirth { get; set; }
		public string Address { get; set; } = null!;
		public string Province { get; set; } = null!;
		public string City { get; set; } = null!;
		public string? PhotoUrl { get; set; }
	}
}
