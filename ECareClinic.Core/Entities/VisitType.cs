using ECareClinic.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Entities
{
    public class VisitType
    {
        [Key]
        public int VisitTypeId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!; // Online, Clinic, Home

        public ICollection<DoctorVisitType> DoctorVisitTypes { get; set; } = new List<DoctorVisitType>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
