using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs
{
	public class BaseResponseDto
	{
		public bool Success { get; set; }
		public string? Message { get; set; }
		public string[]? Errors { get; set; }
	}
}
