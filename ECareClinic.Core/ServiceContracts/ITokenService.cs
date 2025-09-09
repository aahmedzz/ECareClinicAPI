using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.Identity;

namespace ECareClinic.Core.ServiceContracts
{
	public interface ITokenService
	{
		string GenerateToken(ApplicationUser applicationUser);
	}
}
