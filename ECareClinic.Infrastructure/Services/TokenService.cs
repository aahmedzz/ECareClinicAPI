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
using System.Security.Cryptography;
using ECareClinic.Core.DTOs.RegisterationDtos;
using Microsoft.AspNetCore.Identity;

namespace ECareClinic.Infrastructure.Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _configuration;
		private readonly UserManager<ApplicationUser> _userManager;

		public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
		{
			_configuration = configuration;
			_userManager = userManager;
		}
		public async Task<TokenDto> GenerateToken(ApplicationUser applicationUser)
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
				new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
				new Claim(ClaimTypes.Email, applicationUser.Email ?? string.Empty),
			};

			var roles = await _userManager.GetRolesAsync(applicationUser);
			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

			var expiration = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]!));
			//create the token
			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: expiration,
				signingCredentials: credentials
			);
			return new TokenDto
			{
				Token = new JwtSecurityTokenHandler().WriteToken(token),
				TokenExpiration = expiration,
				RefreshToken = GenerateRefreshToken(),
				RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
			};
		}

		public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
		{
			var tokenValidationParameters = new TokenValidationParameters()
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),

				ValidateLifetime = false //should be false
			};

			JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

			ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

			if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}
		private string GenerateRefreshToken()
		{
			var randomNumber = new byte[64];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}
	}
}
