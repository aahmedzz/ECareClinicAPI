using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Models
{
	public class PaymentMethod
	{
		[Key]
		public int PaymentMethodId { get; set; }

		[Required]
		[MaxLength(100)]
		public string ProviderName { get; set; } = null!;

		[MaxLength(255)]
		public string? Token { get; set; }

		[MaxLength(4)]
		public string? Last4Digits { get; set; }

		[MaxLength(50)]
		public string? CardType { get; set; }

		[Range(1, 12)]
		public int ExpiryMonth { get; set; }

		[Range(2000, 2100)]
		public int ExpiryYear { get; set; }

		public bool IsDefault { get; set; }

		// Foreign key
		public string PatientId { get; set; } = null!;
		public Patient Patient { get; set; } = null!;

		// Navigation
		public ICollection<Payment> Payments { get; set; } = new List<Payment>();
	}
}
