using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Entities.Auth
{
	public class EmailVerification
	{
		public int Id { get; set; }
		public string Email { get; set; } = null!;
		public string OtpCode { get; set; } = null!;
		public DateTime Expiration { get; set; }
	}
}
