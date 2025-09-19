using System.Security.Claims;
using Asp.Versioning;
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
		//[Authorize(Roles = "Patient")]
		[Authorize]
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
	}

}
