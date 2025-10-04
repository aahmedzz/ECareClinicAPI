using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.DTOs.BookingDtos
{

        public class SpecialtyDtoResponse
        {
            public int SpecialtyId { get; set; }
            public string Name { get; set; } = null!;
        }
}
