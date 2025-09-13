using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace ECareClinic.Core.Identity
{
	public class ApplicationUser: IdentityUser
	{
		public string? RefreshToken { get; set; }
		public DateTime RefreshTokenExpiration { get; set; }
	}
}
