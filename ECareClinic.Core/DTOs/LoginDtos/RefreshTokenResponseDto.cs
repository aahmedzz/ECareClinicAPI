using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.LoginDtos
{
	public class RefreshTokenResponseDto : BaseResponseDto
	{
		public string? Token { get; set; }
		public DateTime TokenExpiration { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime RefreshTokenExpiration { get; set; }
	}
}
