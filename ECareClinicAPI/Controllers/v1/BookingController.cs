using Asp.Versioning;
using ECareClinic.Core.DTOs.BookingDtos;
using ECareClinic.Core.Models;
using ECareClinic.Infrastructure.Data;
using ECareClinicAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECareClinicAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [ValidateModel]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("specialties")]
        public async Task<ActionResult<IEnumerable<SpecialtyDto>>> GetSpecialties()
        {
            var specialties = await _context.Specialties
                .Select(s => new SpecialtyDto
                {
                    SpecialtyId = s.SpecialtyId,
                    Name = s.Name
                })
                .ToListAsync();

            return Ok(specialties);
        }

        [HttpGet("doctors")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctors(int specialtyId, string AppointmentTime)
        {
            IQueryable<Doctor> query = new List<Doctor>().AsQueryable();    
            if (specialtyId == 0)
            {
                 query = _context.Doctors
                    .Include(d => d.Specialty)
                    .Include(d => d.DoctorSchedules)
                    .AsQueryable();
            }
            else
            {
                 query = _context.Doctors
                    .Include(d => d.Specialty)
                    .Include(d => d.DoctorSchedules)
                    .Where(d => d.SpecialtyId == specialtyId)
                    .AsQueryable();
            }

            

            //if (day.HasValue)
            //{
            //    query = query.Where(d =>
            //        d.DoctorSchedules.Any(s =>
            //            s.Date.Date == day.Value.Date && s.IsAvailable));
            //}

            var doctors = await query
                .Select(d => new DoctorDto
                {
                    DoctorId = d.DoctorId,
                    FullName = d.FirstName + " " + d.LastName,
                    Specialty = d.Specialty.Name,
                    VisitTypes = d.DoctorVisitTypes.Select(dv => new VisitTypeDto
                    {
                        VisitTypeId = dv.VisitTypeId,
                        Name = dv.VisitType.Name,
                    }).ToList()

                })
                .ToListAsync();

            return Ok(doctors);
        }
        //[HttpGet("doctors-all")]
        //public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctors(DateTime? day = null)
        //{
        //    var query = _context.Doctors
        //        .Include(d => d.Specialty)
        //        .Include(d => d.DoctorSchedules)
        //        .AsQueryable();

        //    if (day.HasValue)
        //    {
        //        query = query.Where(d =>
        //            d.DoctorSchedules.Any(s =>
        //                s.Date.Date == day.Value.Date && s.IsAvailable));
        //    }

        //    var doctors = await query
        //        .Select(d => new DoctorDto
        //        {
        //            DoctorId = d.DoctorId,
        //            FullName = d.FirstName + " " + d.LastName,
        //            Specialty = d.Specialty.Name
        //        })
        //        .ToListAsync();

        //    return Ok(doctors);
        //}

        [HttpGet("visit-types/{doctorId}")]
        public async Task<ActionResult<IEnumerable<VisitTypeDto>>> GetVisitTypes(string doctorId)
        {
            var visitTypes = await _context.DoctorVisitTypes
                .Include(dv => dv.VisitType)
                .Where(dv => dv.DoctorId == doctorId)
                .Select(dv => new VisitTypeDto
                {
                    VisitTypeId = dv.VisitTypeId,
                    Name = dv.VisitType.Name,
                    //Description = dv.VisitType.Description,
                    //DurationMinutes = dv.VisitType.DurationMinutes
                })
                .ToListAsync();

            return Ok(visitTypes);
        }

        [HttpGet("available-slots")]
        public async Task<ActionResult<IEnumerable<SlotDto>>> GetAvailableSlots(string doctorId, int visitTypeId, DateTime day)
        {
            var slots = await _context.DoctorSchedules
                .Where(s => s.DoctorId == doctorId && s.Date.Date == day.Date && s.IsAvailable)
                .Select(s => new SlotDto
                {
                    ScheduleId = s.ScheduleId,
                    //StartTime = s.StartTime,
                    //EndTime = s.EndTime
                })
                .ToListAsync();

            return Ok(slots);
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctorSchedule = await _context.DoctorSchedules
                .FirstOrDefaultAsync(s => s.ScheduleId == dto.ScheduleId && s.IsAvailable);

            if (doctorSchedule == null)
                return BadRequest("Selected slot is not available.");

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                VisitTypeId = dto.VisitTypeId,
                ScheduleId = dto.ScheduleId,
                AppointmentDate = dto.AppointmentDate
                ,ReasonForVisit = dto.ReasonForVisit,
            };

            doctorSchedule.IsAvailable = false;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Appointment booked successfully", AppointmentId = appointment.AppointmentId });
        }
    }

}
