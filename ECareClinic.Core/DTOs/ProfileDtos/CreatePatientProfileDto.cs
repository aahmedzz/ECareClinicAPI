using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.Enums;

namespace ECareClinic.Core.DTOs.ProfileDtos
{
	public class CreatePatientProfileDto
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
		public string? Address { get; set; }

		[MaxLength(50)]
		public string? Province { get; set; }

		[MaxLength(50)]
		public string? City { get; set; }

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
