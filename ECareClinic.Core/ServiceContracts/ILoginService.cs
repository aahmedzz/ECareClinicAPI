using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.DTOs;
using ECareClinic.Core.DTOs.LoginDtos;
using ECareClinic.Core.DTOs.RegisterationDtos;

namespace ECareClinic.Core.ServiceContracts
{
	public interface ILoginService
	{
		public Task<LoginResponseDto> LoginAsync(LoginDto dto);
		public Task<BaseResponseDto> ForgotPasswordByEmailAsync(string email);
		public Task<BaseResponseDto> VerifyPasswordResetOtpAsync(VerifyOtpDto verifyOtpDto);
		public Task<BaseResponseDto> ResetPasswordAsync(ResetPasswordDto dto);
		public Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
	}
}
