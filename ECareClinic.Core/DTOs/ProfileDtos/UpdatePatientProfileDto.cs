using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.DTOs.ProfileDtos;

namespace ECareClinic.Core.DTOs.ProfileDtos
{
	public class UpdatePatientProfileDto
	{
		[Required]
		[MaxLength(40)]
		public string FirstName { get; set; } = string.Empty;

		[Required]
		[MaxLength(40)]
		public string LastName { get; set; } = string.Empty;

		[Required]
		public string Gender { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Date)]
		[CustomValidation(typeof(CreatePatientProfileDto), nameof(ValidateDateOfBirth))]
		public DateTime DateOfBirth { get; set; }

		[MaxLength(150)]
		[Required]
		public string? Address { get; set; }

		[MaxLength(50)]
		[Required]
		public string? Province { get; set; }

		[MaxLength(50)]
		[Required]
		public string? City { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 3)]
		[RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
		public string UserName { get; set; } = null!;
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		[RegularExpression("^(?:\\+20|0)(10|11|12|15)\\d{8}$", ErrorMessage = "Must be a valid phone number")]
		public string PhoneNumber { get; set; } = null!;
		public static ValidationResult? ValidateDateOfBirth(DateTime date, ValidationContext context)
		{
			if (date > DateTime.UtcNow)
			{
				return new ValidationResult("Date of birth cannot be in the future.");
			}

			if (date < DateTime.UtcNow.AddYears(-120)) // older than 120 years
			{
				return new ValidationResult("Date of birth is not valid.");
			}

			return ValidationResult.Success;
		}
	}
}
