using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.DTOs.RegisterationDtos;
using ECareClinic.Core.Identity;

namespace ECareClinic.Core.ServiceContracts
{
	public interface ITokenService
	{
		TokenDto GenerateToken(ApplicationUser applicationUser);
		ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
	}
}
