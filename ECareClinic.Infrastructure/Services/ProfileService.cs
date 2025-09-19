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
using ECareClinic.Core.DTOs;
using ECareClinic.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;

namespace ECareClinic.Infrastructure.Services
{
	public class ProfileService : IProfileService
	{
		private readonly AppDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IWebHostEnvironment _env;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ProfileService(IWebHostEnvironment env, 
			AppDbContext db,
			UserManager<ApplicationUser> userManager,
			IHttpContextAccessor httpContextAccessor
			)
		{
			_db = db;
			_userManager = userManager;
			_env = env;
			_httpContextAccessor = httpContextAccessor;
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

		public async Task<BaseResponseDto> SetProfilePhotoAsync(string userId, byte[] photoBytes, string extension)
		{
			if (!PatientProfImgSettings.AllowedExtensions.Split(',').Contains(extension.ToLower()))
			{
				return new BaseResponseDto { Success = false, Errors = new[] { "Invalid file type." } };
			}

			var uploadsFolder = Path.Combine(_env.WebRootPath, PatientProfImgSettings.PatientProfileImagePath.TrimStart('/'));
			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var patient = await _db.Patients.FirstOrDefaultAsync(p => p.PatientId == userId);
			if (patient == null)
			{
				return new BaseResponseDto { Success = false, Errors = new[] { "Patient not found." } };
			}

			// Delete old photo if exists
			if (!string.IsNullOrEmpty(patient.PhotoURL))
			{
				var oldFilePath = Path.Combine(_env.WebRootPath, patient.PhotoURL.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
				if (File.Exists(oldFilePath))
					File.Delete(oldFilePath);
			}

			// Save new photo
			var fileName = $"{userId}{extension}";
			var filePath = Path.Combine(uploadsFolder, fileName);
			await File.WriteAllBytesAsync(filePath, photoBytes);

			// Update DB
			var relativePath = Path.Combine(PatientProfImgSettings.PatientProfileImagePath, fileName).Replace("\\", "/");
			patient.PhotoURL = relativePath;
			await _db.SaveChangesAsync();

			return new BaseResponseDto { Success = true, Message = "Photo saved successfully." };
		}

		public async Task<BaseResponseDto> RemoveProfilePhotoAsync(string userId)
		{
			var patient = await _db.Patients.FirstOrDefaultAsync(p => p.PatientId == userId);
			if (patient == null)
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = new[] { "Patient not found." }
				};
			}

			if (string.IsNullOrEmpty(patient.PhotoURL))
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = new[] { "No profile photo to remove." }
				};
			}

			// Delete photo file if it exists
			var filePath = Path.Combine(
				_env.WebRootPath,
				patient.PhotoURL.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
			);

			if (File.Exists(filePath))
				File.Delete(filePath);

			// Clear DB field
			patient.PhotoURL = null;
			await _db.SaveChangesAsync();

			return new BaseResponseDto
			{
				Success = true,
				Message = "Profile photo removed successfully."
			};
		}

		public async Task<BaseResponseDto> UpdatePatientProfileAsync(string userId, UpdatePatientProfileDto dto)
		{
			var patient = await _db.Patients.Include(p => p.User)
											.FirstOrDefaultAsync(p => p.PatientId == userId);

			if (patient == null)
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = new[] { "Patient profile not found." }
				};
			}

			var user = patient.User;
			if (user == null)
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = new[] { "Linked user account not found." }
				};
			}

			// Update Patient-specific fields
			if (!Enum.TryParse<Gender>(dto.Gender, true, out var genderEnum))
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = new[] { "Invalid gender value. Allowed values: Male, Female" }
				};
			}
			patient.Gender = genderEnum;

			patient.FirstName = dto.FirstName;
			patient.LastName = dto.LastName;
			patient.DateOfBirth = dto.DateOfBirth;
			patient.Address = dto.Address;
			patient.Province = dto.Province;
			patient.City = dto.City;

			// Update Identity fields (ApplicationUser)
			IdentityResult identityResult = IdentityResult.Success;

			if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
			{
				user.Email = dto.Email;
				identityResult = await _userManager.UpdateAsync(user);
			}

			if (!string.IsNullOrEmpty(dto.UserName) && dto.UserName != user.UserName)
			{
				user.UserName = dto.UserName;
				identityResult = await _userManager.UpdateAsync(user);
			}

			if (!string.IsNullOrEmpty(dto.PhoneNumber) && dto.PhoneNumber != user.PhoneNumber)
			{
				user.PhoneNumber = dto.PhoneNumber;
				identityResult = await _userManager.UpdateAsync(user);
			}

			if (!identityResult.Succeeded)
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = identityResult.Errors.Select(e => e.Description).ToArray()
				};
			}

			await _db.SaveChangesAsync();

			return new BaseResponseDto
			{
				Success = true,
				Message = "Patient profile updated successfully."
			};
		}

		public async Task<PatientProfileResponseDto?> GetMyPatientProfileAsync(string userId)
		{
			var patient = await _db.Patients
				.Include(p => p.User)
				.FirstOrDefaultAsync(p => p.PatientId == userId);

			if (patient == null) return new PatientProfileResponseDto {
				Success = false,
				Errors = new[] { "Patient profile not found." }
			};

			return new PatientProfileResponseDto
			{
				Success = true,
				PatientId = patient.PatientId,
				FirstName = patient.FirstName,
				LastName = patient.LastName,
				Email = patient.User?.Email ?? string.Empty,
				PhoneNumber = patient.User?.PhoneNumber ?? string.Empty,
				Gender = patient.Gender.ToString(),
				DateOfBirth = patient.DateOfBirth,
				Address = patient.Address,
				Province = patient.Province,
				City = patient.City,
				PhotoUrl = string.IsNullOrEmpty(patient.PhotoURL)
					? string.Empty
					: $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}{patient.PhotoURL}"
			};
		}
	}
}
