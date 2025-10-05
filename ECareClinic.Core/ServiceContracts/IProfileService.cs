using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.DTOs;
using ECareClinic.Core.DTOs.ProfileDtos;

namespace ECareClinic.Core.ServiceContracts
{
	public class FileUpload
	{
		public string FileName { get; set; } = default!;
		public byte[] Content { get; set; } = default!;
	}
	public interface IProfileService
	{
		public Task<CreatePatientProfileResponseDto> CreatePatientProfileAsync(string userId, CreatePatientProfileDto patientProfileDto);
		public Task<BaseResponseDto> SetProfilePhotoAsync(string userId, byte[] photo, string extension);
		public Task<PatientProfilePhotoResponse> GetProfilePhotoAsync(string userId);
		public Task<BaseResponseDto> RemoveProfilePhotoAsync(string userId);
		public Task<BaseResponseDto> UpdatePatientProfileAsync(string userId, UpdatePatientProfileDto dto);
		public Task<PatientProfileResponseDto?> GetMyPatientProfileAsync(string userId);

	}
}
