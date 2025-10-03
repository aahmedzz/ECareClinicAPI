using ECareClinic.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Entities
{
    public class DoctorVisitType
    {
        [Key]
        public int DoctorVisitTypeId { get; set; }

        public string DoctorId { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;

        public int VisitTypeId { get; set; }
        public VisitType VisitType { get; set; } = null!;
    }

}
