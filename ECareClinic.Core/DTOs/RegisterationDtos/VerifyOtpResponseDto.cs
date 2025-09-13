using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.RegisterationDtos
{
	public class VerifyOtpResponseDto:BaseResponseDto
	{
		public string? Token { get; set; }
		public DateTime TokenExpiration { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime RefreshTokenExpiration { get; set; }
		public UserDto? User { get; set; }
	}

	public class UserDto
	{
		public string Id { get; set; } = null!;
		public string UserName { get; set; } = null!;
		public string Email { get; set; } = null!;
	}
}
