using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.DTOs.ProfileDtos;
using ECareClinic.Core.Identity;
using ECareClinic.Core.Models;
using ECareClinic.Core.ServiceContracts;
using ECareClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using ECareClinic.Core.Enums;

namespace ECareClinic.Infrastructure.Services
{
	public class ProfileService : IProfileService
	{
		private readonly AppDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IWebHostEnvironment _env;

		public ProfileService(IWebHostEnvironment env, 
			AppDbContext db,
			UserManager<ApplicationUser> userManager)
		{
			_db = db;
			_userManager = userManager;
			_env = env;
		}

		public async Task<CreatePatientProfileResponseDto> CreatePatientProfileAsync(string userId, CreatePatientProfileDto dto)
		{
			if (await _db.Patients.AnyAsync(p => p.PatientId == userId))
				return new CreatePatientProfileResponseDto {
					Success = false,
					Errors = new [] { "Profile already exists for this user." }
				};

			var genderParsed = Enum.TryParse<Gender>(dto.Gender, true, out var genderEnum);

			if (!genderParsed)
			{
				return new CreatePatientProfileResponseDto
				{
					Success = false,
					Errors = new[] { "Invalid gender value. Allowed values: Male, Female" }
				};
			}
			var patient = new Patient
			{
				PatientId = userId,
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				Gender = genderEnum,
				DateOfBirth = dto.DateOfBirth,
				Address = dto.Address,
				Province = dto.Province,
				City = dto.City,
				User = await _userManager.FindByIdAsync(userId)
			};

			var patientSave = _db.Patients.Add(patient);
			await _db.SaveChangesAsync();

			if(patientSave.Entity.PatientId == null)
			{
				return new CreatePatientProfileResponseDto
				{
					Success = false,
					Errors = new[] { "Failed to create patient profile." }
				};
			}
			else
			{
				return new CreatePatientProfileResponseDto
				{
					Success = true,
					Message = "Patient profile created successfully.",
				};
			}
		}
	}
}
