using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.LoginDtos
{
	public class RefreshTokenDto
	{
		public string Token { get; set; }
		public string RefreshToken { get; set; }
	}
}
