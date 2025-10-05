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
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace ECareClinic.Infrastructure.Services
{
	public class ProfileService : IProfileService
	{
		private readonly AppDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _configuration;
		public ProfileService(
			AppDbContext db,
			UserManager<ApplicationUser> userManager,
			IConfiguration configuration)
		{
			_db = db;
			_userManager = userManager;
			_configuration = configuration;
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
			try
			{
				var cloudinarySettings = _configuration.GetSection("CloudinarySettings");
				var account = new Account(cloudinarySettings["CloudName"], cloudinarySettings["Key"], cloudinarySettings["Secret"]);
				var cloudinary = new Cloudinary(account);

				if (!PatientProfImgSettings.AllowedExtensions.Split(',').Contains(extension.ToLower()))
				{
					return new BaseResponseDto { Success = false, Errors = new[] { "Invalid file type." } };
				}

				var patient = await _db.Patients.FirstOrDefaultAsync(p => p.PatientId == userId);
				if (patient == null)
					return new BaseResponseDto { Success = false, Errors = new[] { "Patient not found." } };

				// 1. Delete old photo if exists
				if (!string.IsNullOrEmpty(patient.PhotoURL))
				{
					// Extract publicId from the old URL (Cloudinary requires this)
					var publicId = GetPublicIdFromUrl(patient.PhotoURL);
					if (!string.IsNullOrEmpty(publicId))
					{
						await cloudinary.DestroyAsync(new DeletionParams(publicId));
					}
				}

				// 2. Upload new photo
				using (var stream = new MemoryStream(photoBytes))
				{
					var uploadParams = new ImageUploadParams()
					{
						File = new FileDescription($"{userId}{extension}", stream),
						Folder = PatientProfImgSettings.PatientProfileImagePath,
						PublicId = userId,
						Overwrite = true 
					};

					var uploadResult = await cloudinary.UploadAsync(uploadParams);

					// 3. Update DB with new photo URL
					patient.PhotoURL = uploadResult.SecureUrl.ToString();
					await _db.SaveChangesAsync();

					return new BaseResponseDto { Success = true, Message = "Photo saved successfully." };
				}
			}
			catch (Exception ex)
			{
				return new BaseResponseDto { Success = false, Errors = new[] { ex.Message, ex.StackTrace ?? "" } };
			}
		}

		public async Task<PatientProfilePhotoResponse> GetProfilePhotoAsync(string userId)
		{
			var patient = await _db.Patients
				.Include(p => p.User)
				.FirstOrDefaultAsync(p => p.PatientId == userId);

			if (patient == null) return new PatientProfilePhotoResponse
			{
				Success = false,
				Errors = new[] { "Patient profile not found." }
			};

			if (string.IsNullOrEmpty(patient.PhotoURL))
			{
				return new PatientProfilePhotoResponse
				{
					Success = false,
					Errors = new[] { "No profile photo found." }
				};
			}
			return new PatientProfilePhotoResponse
			{
				Success = true,
				Message = "Profile photo retrieved successfully.",
				PhotoURL = patient.PhotoURL
			};
		}

		public async Task<BaseResponseDto> RemoveProfilePhotoAsync(string userId)
		{
			try
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

				var uri = new Uri(patient.PhotoURL);
				var publicIdWithExt = Path.GetFileNameWithoutExtension(uri.LocalPath);
				var folder = "PatientsProfPhotos";
				var publicId = $"{folder}/{publicIdWithExt}";

				var cloudinarySettings = _configuration.GetSection("CloudinarySettings");
				var account = new Account(cloudinarySettings["CloudName"], cloudinarySettings["Key"], cloudinarySettings["Secret"]);
				var cloudinary = new Cloudinary(account);

				// Delete from Cloudinary
				var deletionParams = new DeletionParams(publicId)
				{
					ResourceType = ResourceType.Image
				};
				var deletionResult = await cloudinary.DestroyAsync(deletionParams);

				if (deletionResult.Result != "ok" && deletionResult.Result != "not found")
				{
					return new BaseResponseDto
					{
						Success = false,
						Errors = new[] { "Failed to remove profile photo." }
					};
				}

				// Clear DB field
				patient.PhotoURL = null;
				await _db.SaveChangesAsync();

				return new BaseResponseDto
				{
					Success = true,
					Message = "Profile photo removed successfully."
				};
			}
			catch (Exception ex)
			{
				return new BaseResponseDto
				{
					Success = false,
					Errors = new[] { ex.Message, ex.StackTrace ?? "" }
				};
			}
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

			if (!string.IsNullOrEmpty(dto.UserName) && dto.UserName != user.UserName)
			{
				user.UserName = dto.UserName;
				identityResult = await _userManager.UpdateAsync(user);
			}

			if (!string.IsNullOrEmpty(dto.PhoneNumber) && dto.PhoneNumber != user.PhoneNumber)
			{
				bool phoneExists = await _userManager.Users
					.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber);

				if (phoneExists)
				{
					return new BaseResponseDto
					{
						Success = false,
						Errors = new[] { "This phone number is already in use." }
					};
				}
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

			ProfileDTO profileDTO = new ProfileDTO
			{
				PatientId = patient.PatientId,
				FirstName = patient.FirstName,
				LastName = patient.LastName,
				Email = patient.User?.Email ?? string.Empty,
				UserName = patient.User?.UserName ?? string.Empty,
				PhoneNumber = patient.User?.PhoneNumber ?? string.Empty,
				Gender = patient.Gender.ToString(),
				DateOfBirth = patient.DateOfBirth,
				Address = patient.Address,
				Province = patient.Province,
				City = patient.City,
				PhotoUrl = string.IsNullOrEmpty(patient.PhotoURL) ? string.Empty : patient.PhotoURL
			};
			return new PatientProfileResponseDto
			{
				Success = true,
				Profile = profileDTO
			};
		}

		/// <summary>
		/// Extracts the public ID from a Cloudinary URL (needed for deletion).
		/// </summary>
		private string GetPublicIdFromUrl(string url)
		{
			try
			{
				var uri = new Uri(url);
				var segments = uri.AbsolutePath.Split('/');
				// remove extension (.jpg, .png, etc.)
				var fileName = Path.GetFileNameWithoutExtension(segments.Last());
				var folderPath = string.Join("/", segments.SkipWhile(s => s != "upload").Skip(1).Take(segments.Length - 2));
				return $"{folderPath}/{fileName}";
			}
			catch
			{
				return null;
			}
		}
	}
}
