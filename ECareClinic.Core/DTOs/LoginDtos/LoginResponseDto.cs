using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.DTOs.RegisterationDtos;

namespace ECareClinic.Core.DTOs.LoginDtos
{
	public class LoginResponseDto:BaseResponseDto
	{
		public string? Token { get; set; }
		public UserDto? User { get; set; }
	}
}
