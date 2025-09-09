using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Models
{
	public class Insurance
	{
		[Key]
		public int InsuranceId { get; set; }

		[Required]
		[MaxLength(50)]
		public string PolicyNumber { get; set; } = null!;

		[Required]
		[MaxLength(100)]
		public string PolicyHolderName { get; set; } = null!;

		[DataType(DataType.Date)]
		public DateTime ValidFrom { get; set; }

		[DataType(DataType.Date)]
		public DateTime ValidTo { get; set; }

		// Foreign key
		public string PatientId { get; set; } = null!;
		public Patient Patient { get; set; } = null!;
	}
}
