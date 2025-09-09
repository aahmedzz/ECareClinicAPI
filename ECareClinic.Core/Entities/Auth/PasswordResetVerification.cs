using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Entities.Auth
{
	public class PasswordResetVerification
	{
		public int Id { get; set; }
		[StringLength(50)]
		public string Email { get; set; } = null!;
		
		[StringLength(10)]
		public string OtpCode { get; set; } = null!;

		public bool IsVerified { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime Expiration { get; set; }
	}
}
