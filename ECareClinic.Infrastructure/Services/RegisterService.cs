using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.DTOs.RegisterationDtos;
using ECareClinic.Core.Entities;
using ECareClinic.Core.Entities.Auth;
using ECareClinic.Core.Identity;
using ECareClinic.Core.ServiceContracts;
using ECareClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ECareClinic.Infrastructure.Services
{
	public class RegisterService : IRegisterService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly AppDbContext _db;
		private readonly IEmailService _emailService;
		private readonly ITokenService _tokenService;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public RegisterService(
			UserManager<ApplicationUser> userManager,
			AppDbContext db,
			IEmailService emailService,
			ITokenService tokenService,
			RoleManager<ApplicationRole> roleManager)
		{
			_userManager = userManager;
			_db = db;
			_emailService = emailService;
			_tokenService = tokenService;
			_roleManager = roleManager;
		}
		public async Task<RegisterResponseDto> RegisterUserAsync(RegisterDto dto)
		{
			var existingUser = await _userManager.FindByEmailAsync(dto.Email);

			if (existingUser != null)
			{
				if (existingUser.EmailConfirmed)
				{
					return new RegisterResponseDto
					{
						Success = false,
						Errors = new[] { "Email is already registered." }
					};
				}

				// User exists but not confirmed → resend OTP
				await GenerateAndSendOtpAsync(dto.Email);

				return new RegisterResponseDto
				{
					Success = true,
					Message = "OTP resent to email. Please verify to continue."
				};
			}

			// New user
			var user = new ApplicationUser
			{
				UserName = dto.UserName,
				Email = dto.Email,
				PhoneNumber = dto.PhoneNumber,
				EmailConfirmed = false
			};

			var result = await _userManager.CreateAsync(user, dto.Password);
			if (!result.Succeeded)
			{
				return new RegisterResponseDto
				{
					Success = false,
					Errors = result.Errors.Select(e => e.Description).ToArray()
				};
			}

			// Ensure the "Patient" role exists
			if (!await _roleManager.RoleExistsAsync("Patient"))
			{
				var role = new ApplicationRole
				{
					Name = "Patient",
					NormalizedName = "PATIENT"
				};
				await _roleManager.CreateAsync(role);
			}

			// Assign role to user
			await _userManager.AddToRoleAsync(user, "Patient");

			// Send OTP for new user
			await GenerateAndSendOtpAsync(dto.Email);

			return new RegisterResponseDto
			{
				Success = true,
				Message = "OTP sent to email. Please verify to continue."
			};
		}
		public async Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto dto)
		{
			var record = await _db.EmailVerifications.FirstOrDefaultAsync(v => v.Email == dto.Email);

			if (record == null || record.Expiration < DateTime.UtcNow || record.OtpCode != dto.OtpCode)
			{
				return new VerifyOtpResponseDto
				{
					Success = false,
					Errors = new[] { "Invalid or expired OTP." }
				};
			}

			var user = await _userManager.FindByEmailAsync(dto.Email);
			if (user == null)
			{
				return new VerifyOtpResponseDto
				{
					Success = false,
					Errors = new[] { "User not found." }
				};
			}

			user.EmailConfirmed = true;

			// Generate token and refresh token from the TokenService
			var tokenDto = await _tokenService.GenerateToken(user);

			// Save refresh token and its expiration to the user
			user.RefreshToken = tokenDto.RefreshToken;
			user.RefreshTokenExpiration = tokenDto.RefreshTokenExpiration;
			await _userManager.UpdateAsync(user);

			// Cleanup OTP
			_db.EmailVerifications.Remove(record);
			await _db.SaveChangesAsync();

			return new VerifyOtpResponseDto
			{
				Success = true,
				Message = "Email verified successfully.",
				Token = tokenDto.Token,
				TokenExpiration = tokenDto.TokenExpiration,
				RefreshToken = tokenDto.RefreshToken,
				RefreshTokenExpiration = tokenDto.RefreshTokenExpiration,
				User = new UserDto
				{
					Id = user.Id,
					UserName = user.UserName,
					Email = user.Email
				}
			};
		}


		/// <summary>
		/// Generates a new OTP, removes old ones, saves it in DB, and sends it via email.
		/// </summary>
		private async Task GenerateAndSendOtpAsync(string email)
		{
			var otp = new Random().Next(1000, 9999).ToString();

			// Remove old OTPs
			var oldOtps = _db.EmailVerifications.Where(v => v.Email == email);
			_db.EmailVerifications.RemoveRange(oldOtps);

			// Add new OTP
			_db.EmailVerifications.Add(new EmailVerification
			{
				Email = email,
				OtpCode = otp,
				Expiration = DateTime.UtcNow.AddMinutes(5)
			});

			await _db.SaveChangesAsync();

			// Send OTP
			await _emailService.SendOtpAsync(email, otp);
		}
	}
}
