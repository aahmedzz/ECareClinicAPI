using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure;
using ECareClinic.Core.DTOs;
using ECareClinic.Core.DTOs.LoginDtos;
using ECareClinic.Core.DTOs.RegisterationDtos;
using ECareClinic.Core.Entities;
using ECareClinic.Core.Entities.Auth;
using ECareClinic.Core.Identity;
using ECareClinic.Core.ServiceContracts;
using ECareClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ECareClinic.Infrastructure.Services
{
	public class LoginService : ILoginService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ITokenService _tokenService;
		private readonly AppDbContext _db;
		private readonly IEmailService _emailService;

		public LoginService(UserManager<ApplicationUser> userManager,
			ITokenService tokenService,
			AppDbContext appDbContext,
			IEmailService emailService)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_db = appDbContext;
			_emailService = emailService;
		}

		public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
		{
			var user = await _userManager.FindByEmailAsync(dto.Email);
			if (user == null)
				return new LoginResponseDto
				{
					Success = false,
					Errors = new[] { "Invalid email or password." }
				};

			if (!user.EmailConfirmed)
				return new LoginResponseDto
				{
					Success = false,
					Errors = new[] { "Email not confirmed. Please verify OTP first." }
				};

			var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
			if (!passwordValid)
				return new LoginResponseDto
				{
					Success = false,
					Errors = new[] { "Invalid email or password." }
				};

			TokenDto token = await _tokenService.GenerateToken(user);

			// Save refresh token and its expiration to the user
			user.RefreshToken = token.RefreshToken;
			user.RefreshTokenExpiration = token.RefreshTokenExpiration;
			await _userManager.UpdateAsync(user);

			return new LoginResponseDto
			{
				Success = true,
				Message = "Login successful.",
				Token = token.Token,
				TokenExpiration = token.TokenExpiration,
				RefreshToken = token.RefreshToken,
				RefreshTokenExpiration = token.RefreshTokenExpiration,
				User = new UserDto
				{
					Id = user.Id,
					UserName = user.UserName,
					Email = user.Email
				}
			};
		}
		public async Task<BaseResponseDto> ForgotPasswordByEmailAsync(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = new[] { "User with this email does not exist." }
				};
			}

			var otp = new Random().Next(1000, 9999).ToString();

			// remove old requests
			var oldRequests = _db.passwordResetVerifications.Where(r => r.Email == email);
			_db.passwordResetVerifications.RemoveRange(oldRequests);

			// add new one
			_db.passwordResetVerifications.Add(new PasswordResetVerification
			{
				Email = email,
				OtpCode = otp,
				Expiration = DateTime.UtcNow.AddMinutes(5),
				IsVerified = false
			});

			await _db.SaveChangesAsync();

			await _emailService.SendOtpAsync(email, otp);

			return new BaseResponseDto
			{
				Success = true,
				Message = "Password reset OTP sent to your email."
			};
		}
		public async Task<BaseResponseDto> VerifyPasswordResetOtpAsync(VerifyOtpDto dto)
		{
			var request = await _db.passwordResetVerifications
				.FirstOrDefaultAsync(r => r.Email == dto.Email);

			if (request == null || request.Expiration < DateTime.UtcNow || request.OtpCode != dto.OtpCode)
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = new[] { "This OTP is Invalid or Expired." }
				};
			}

			request.IsVerified = true;
			await _db.SaveChangesAsync();

			return new BaseResponseDto
			{
				Success = true,
				Message = "OTP verified successfully. You can now reset your password."
			};
		}
		public async Task<BaseResponseDto> ResetPasswordAsync(ResetPasswordDto dto)
		{
			var row = await _db.passwordResetVerifications
				.FirstOrDefaultAsync(r => r.Email == dto.Email);

			if (row == null || !row.IsVerified || row.Expiration < DateTime.UtcNow)
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = new[] { "Reset request not verified or expired." }
				};
			}

			var user = await _userManager.FindByEmailAsync(dto.Email);
			if (user == null)
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = new[] { "User not found." }
				};
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

			if (!result.Succeeded)
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = result.Errors.Select(e => e.Description).ToArray()
				};
			}

			// cleanup
			_db.passwordResetVerifications.Remove(row);
			await _db.SaveChangesAsync();

			return new BaseResponseDto
			{
				Success = true,
				Message = "Password reset successfully."
			};
		}
		public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
		{
			if (dto == null)
			{
				return new RefreshTokenResponseDto {
					Success = false,
					Message = "Invalid client request",
					Errors = new[] { "Refresh token request cannot be null" }
				};
			}

			try
			{
				// Extract principal from expired token
				ClaimsPrincipal? principal = _tokenService.GetPrincipalFromExpiredToken(dto.Token);
				if (principal == null)
				{
					return new RefreshTokenResponseDto {
						Success = false,
						Message = "Invalid JWT access token",
						Errors = new[] { "Token validation failed" }
					};
				}

				string? email = principal.FindFirstValue(ClaimTypes.Email);
				if (string.IsNullOrEmpty(email))
				{
					return new RefreshTokenResponseDto {
						Success = false,
						Message = "Email claim missing in token",
						Errors = new[] { "The JWT does not contain an email claim" }
					};
				}

				ApplicationUser? user = await _userManager.FindByEmailAsync(email);
				if (user == null ||
					user.RefreshToken != dto.RefreshToken ||
					user.RefreshTokenExpiration <= DateTime.Now)
				{
					return new RefreshTokenResponseDto {
						Success = false,
						Message = "Invalid refresh token",
						Errors = new[] { "The refresh token is invalid or expired" }
					};
				}

				// Generate new tokens
				var tokenResponse = await _tokenService.GenerateToken(user);

				// Update user with new refresh token
				user.RefreshToken = tokenResponse.RefreshToken;
				user.RefreshTokenExpiration = tokenResponse.RefreshTokenExpiration;
				await _userManager.UpdateAsync(user);

				return new RefreshTokenResponseDto { 
					Success = true,
					Message = "Token refreshed successfully",
					Token = tokenResponse.Token,
					TokenExpiration = tokenResponse.TokenExpiration,
					RefreshToken = tokenResponse.RefreshToken,
					RefreshTokenExpiration = tokenResponse.RefreshTokenExpiration
				};
			}
			catch (Exception ex)
			{
				return new RefreshTokenResponseDto {
					Success = false,
					Message = "An error occurred while refreshing the token",
					Errors = new[] { ex.Message }
				};
			}
		}
	}
}
