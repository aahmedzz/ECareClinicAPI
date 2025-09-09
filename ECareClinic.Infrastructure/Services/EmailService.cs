using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.ServiceContracts;
using Microsoft.Extensions.Options;

namespace ECareClinic.Infrastructure.Services
{
	public class EmailSettings
	{
		public string SmtpServer { get; set; }
		public int Port { get; set; }
		public string SenderName { get; set; }
		public string SenderEmail { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
	public class EmailService : IEmailService
	{
		private readonly EmailSettings _settings;

		public EmailService(IOptions<EmailSettings> settings)
		{
			_settings = settings.Value;
		}
		public async Task SendEmailAsync(string to, string subject, string body)
		{
			using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
			{
				Credentials = new NetworkCredential(_settings.Username, _settings.Password),
				EnableSsl = true
			};

			var mail = new MailMessage
			{
				From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			mail.To.Add(to);

			await client.SendMailAsync(mail);
		}

		public Task SendOtpAsync(string toEmail, string otpCode)
		{
			string subject = "Your OTP Code";
			string body = $@"
			<html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Verification Code</h2>
                <p>Your One-Time Password (OTP) is:</p>
                <h1 style='color: #2e6c80;'>{otpCode}</h1>
                <p>This code will expire in 5 minutes.</p>
                <p>If you didn’t request this, please ignore this email.</p>
            </body>
			</html>";

			return SendEmailAsync(toEmail, subject, body);
		}
	}
}
