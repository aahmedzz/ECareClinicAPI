using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.LoginDtos
{
	public class ResetPasswordDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		[StringLength(30, MinimumLength = 8)]
		public string NewPassword { get; set; } = null!;
		[Required]
		[StringLength(30, MinimumLength = 8)]
		[Compare(nameof(NewPassword),ErrorMessage ="The 'Confirm password' must be equal to the 'New Password'.")]
		public string ConfirmPassword { get; set; } = null!;
	}
}
