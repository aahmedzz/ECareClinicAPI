using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECareClinic.Core.DTOs.LoginDtos
{
	public class LoginDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		[StringLength(30, MinimumLength = 8)]
		public string Password { get; set; } = null!;
	}
}
