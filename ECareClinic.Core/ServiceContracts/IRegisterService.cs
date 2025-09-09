using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.DTOs.RegisterationDtos;

namespace ECareClinic.Core.ServiceContracts
{
	public interface IRegisterService
	{
		Task<RegisterResponseDto> RegisterUserAsync(RegisterDto registerDto);
		Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto dto);
	}
}
