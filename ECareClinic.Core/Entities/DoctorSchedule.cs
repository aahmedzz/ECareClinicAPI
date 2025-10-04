using ECareClinic.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Entities
{
    public class DoctorSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public string DoctorId { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;

        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required, DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required, DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;
		public int SlotDurationMinutes { get; set; } = 30;

		public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
