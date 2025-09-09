using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Models
{
	public class MedicalRecord
	{
		[Key]
		public int MedicalRecordId { get; set; }

		[Required]
		[MaxLength(100)]
		public string FileName { get; set; } = null!;

		[Required]
		[MaxLength(300)]
		public string FileURL { get; set; } = null!;

		[Required]
		[MaxLength(20)]
		public string FileType { get; set; } = null!;

		[DataType(DataType.Date)]
		public DateTime RecordDate { get; set; }

		// Foreign keys
		public string PatientId { get; set; } = null!;
		public Patient Patient { get; set; } = null!;

		public string DoctorId { get; set; } = null!;
		public Doctor Doctor { get; set; } = null!;

	}
}
