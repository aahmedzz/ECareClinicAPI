using System.Security.Claims;
using Asp.Versioning;
using ECareClinic.Core.DTOs;
using ECareClinic.Core.DTOs.ProfileDtos;
using ECareClinic.Core.Identity;
using ECareClinic.Core.Models;
using ECareClinic.Core.ServiceContracts;
using ECareClinic.Infrastructure.Data;
using ECareClinicAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECareClinicAPI.Controllers.v1
{
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiVersion("1.0")]
	[ApiController]
	[ValidateModel]
	public class ProfileController : ControllerBase
	{
		private readonly IProfileService _profileService;

		public ProfileController(IProfileService profileService)
		{
			_profileService = profileService;
		}

		[HttpPost("create-patient-profile")]
		[Authorize(Roles = "Patient")]
		public async Task<IActionResult> CreatePatientProfile([FromBody] CreatePatientProfileDto dto)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(new CreatePatientProfileResponseDto{
					Success = false,
					Errors = new[] { "Invalid user token" }
				});
			var response = await _profileService.CreatePatientProfileAsync(userId, dto);
			return (response.Success) ? Ok(response) : BadRequest(response);
		}

		[HttpPost("set-patient-profile-photo")]
		[Authorize(Roles = "Patient")]
		public async Task<IActionResult> SetPatientProfilePhoto(IFormFile photo)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(new BaseResponseDto{ Success = false, Errors = new[] { "Invalid user token" } });

			if (photo == null || photo.Length == 0)
				return BadRequest(new { Success = false, Errors = new[] { "Photo is required" } });

			var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

			using var ms = new MemoryStream();
			await photo.CopyToAsync(ms);
			var photoBytes = ms.ToArray();

			var response = await _profileService.SetProfilePhotoAsync(userId, photoBytes , extension);
			return response.Success ? Ok(response) : BadRequest(response);
		}

		[HttpDelete("remove-patient-profile-photo")]
		[Authorize(Roles = "Patient")]
		public async Task<IActionResult> RemovePatientProfilePhoto()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(new { Message = "User not authenticated." });

			var result = await _profileService.RemoveProfilePhotoAsync(userId);

			if (!result.Success)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpPut("update-patient-profile")]
		[Authorize(Roles = "Patient")]
		public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientProfileDto dto)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(new { Success = false, Errors = new[] { "Invalid user token" } });

			var response = await _profileService.UpdatePatientProfileAsync(userId, dto);
			return response.Success ? Ok(response) : BadRequest(response);
		}

		[Authorize(Roles = "Patient")]
		[HttpGet("me")]
		public async Task<IActionResult> GetMyProfile()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized();

			var profile = await _profileService.GetMyPatientProfileAsync(userId);

			if (profile == null)
				return NotFound(new PatientProfileResponseDto{
					Success = false,
					Message = "Profile not found." 
				});

			return Ok(profile);
		}
	}

}
