using ECareClinic.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Entities
{
    public class Specialty
    {
        [Key]
        public int SpecialtyId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        // Navigation
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
