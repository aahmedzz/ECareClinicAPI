using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.ServiceContracts
{
	public interface IEmailService
	{
		Task SendEmailAsync(string toEmail, string subject, string body);
		Task SendOtpAsync(string toEmail, string otpCode);
	}
}
