using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Models
{
	public class Payment
	{
		[Key]
		public int PaymentId { get; set; }

		[Required]
		[Column(TypeName = "decimal(18,2)")]
		public decimal Amount { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime PaymentDate { get; set; }

		[Required]
		[MaxLength(50)]
		public string Status { get; set; } = null!;

		// Foreign keys
		public int PaymentMethodId { get; set; }
		public PaymentMethod PaymentMethod { get; set; } = null!;

		public string PatientId { get; set; } = null!;
		public Patient Patient { get; set; } = null!;
	}
}
