﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.RegisterationDtos
{
	public class VerifyOtpDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;

		[Required]
		public string OtpCode { get; set; } = null!;
	}
}
