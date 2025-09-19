using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	}
}
