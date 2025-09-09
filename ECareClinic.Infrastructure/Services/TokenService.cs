using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.Identity;
using ECareClinic.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ECareClinic.Infrastructure.Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _configuration;

		public TokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string GenerateToken(ApplicationUser applicationUser)
		{
			var secretKey = _configuration["Jwt:Key"];
			//check if key is null or empty
			if (string.IsNullOrEmpty(secretKey))
			{
				throw new Exception("JWT Key is Missing.");
			}

			// create symmetric security key and signing credentials
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			// create claims
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
			};

			//create the token
			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]!)),
				signingCredentials: credentials
			);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
