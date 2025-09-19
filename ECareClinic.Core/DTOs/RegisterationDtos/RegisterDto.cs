using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.RegisterationDtos
{
	public class RegisterDto
	{
		[Required]
		[StringLength(50, MinimumLength = 3)]
		[RegularExpression(@"^[a-zA-Z0-9_]+$",ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
		public string UserName { get; set; } = null!;
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		[StringLength(30, MinimumLength = 8)]
		public string Password { get; set; } = null!;
		[Required]
		[RegularExpression("^(?:\\+20|0)(10|11|12|15)\\d{8}$",ErrorMessage = "Must be a valid phone number")] 
		public string PhoneNumber { get; set; } = null!;
	}
}
