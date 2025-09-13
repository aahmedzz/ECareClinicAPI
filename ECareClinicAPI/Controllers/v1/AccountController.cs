using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Asp.Versioning;
using ECareClinic.Core.DTOs.LoginDtos;
using ECareClinic.Core.DTOs.RegisterationDtos;
using ECareClinic.Core.Identity;
using ECareClinic.Core.ServiceContracts;
using ECareClinicAPI.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECareClinicAPI.Controllers.v1
{
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiVersion("1.0")]
	[ApiController]
	[ValidateModel]
	public class AccountController : ControllerBase
	{
		private readonly IRegisterService _registerService;
		private readonly ILoginService _loginService;

		public AccountController(IRegisterService registerService,ILoginService loginService)
		{
			_registerService = registerService;
			_loginService = loginService;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto dto)
		{
			//Used custom action filter for model validation instead of this
			//if (!ModelState.IsValid)
			//{
			//	var errors = ModelState.Values
			//		.SelectMany(v => v.Errors)
			//		.Select(e => e.ErrorMessage)
			//		.ToArray();

			//	return BadRequest(new RegisterResponseDto
			//	{
			//		Success = false,
			//		Errors = errors
			//	});
			//}

			var response = await _registerService.RegisterUserAsync(dto);

			if (!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}

		[HttpPost("Verify-register-otp")]
		public async Task<IActionResult> VerifyRegisterOtp([FromBody] VerifyOtpDto dto)
		{
			var response = await _registerService.VerifyOtpAsync(dto);

			if (!response.Success)
				return BadRequest(response);

			return Ok(response);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var result = await _loginService.LoginAsync(dto);
			if (!result.Success) return BadRequest(result);
			return Ok(result);
		}

		[HttpPost("forgot-password-by-email")]
		public async Task<IActionResult> ForgotPasswordByEmail([FromBody] [Required] [EmailAddress]string EmailAddress)
		{
			var result = await _loginService.ForgotPasswordByEmailAsync(EmailAddress);

			return (result.Success) ? Ok(result) : BadRequest(result);
		}

		[HttpPost("verify-password-reset-otp")]
		public async Task<IActionResult> VerifyPasswordResetOtp([FromBody] VerifyOtpDto dto)
		{
			var result = await _loginService.VerifyPasswordResetOtpAsync(dto);

			return (result.Success)?Ok(result): BadRequest(result);
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
		{
			var result = await _loginService.ResetPasswordAsync(dto);
			return (result.Success) ? Ok(result) : BadRequest(result);
		}

		[HttpPost("generate-new-jwt-token")]
		public async Task<IActionResult> GenerateNewAccessToken([FromBody] RefreshTokenDto dto)
		{
			var result = await _loginService.RefreshTokenAsync(dto);
			return (result.Success) ? Ok(result) : BadRequest(result);
		}

	}
}
