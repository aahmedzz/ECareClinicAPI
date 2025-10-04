using System.Security.Claims;
using Asp.Versioning;
using ECareClinic.Core.DTOs.BookingDtos;
using ECareClinic.Core.Enums;
using ECareClinic.Core.Models;
using ECareClinic.Infrastructure.Data;
using ECareClinicAPI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECareClinicAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [ValidateModel]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetSpecialties")]
        public async Task<ActionResult<IEnumerable<SpecialtyDtoResponse>>> GetSpecialties()
        {
            var specialties = await _context.Specialties
                .Select(s => new SpecialtyDtoResponse
                {
                    SpecialtyId = s.SpecialtyId,
                    Name = s.Name
                })
                .ToListAsync();

            return Ok(specialties);
        }

        [HttpGet("get-available-doctors")]
        public async Task<ActionResult<IEnumerable<DoctorDtoResponse>>> GetDoctors([FromQuery]DoctorFilterDto filter)
        {
			IQueryable<Doctor> query = _context.Doctors
		    .Include(d => d.Specialty)
		    .Include(d => d.DoctorSchedules);

			// Filter: General or Specialty
			if (!filter.GeneralDoctorTypes && filter.SpecialtyId.HasValue)
			{
				query = query.Where(d => d.SpecialtyId == filter.SpecialtyId);
			}

			// Filter: Appointment Time
			if (filter.AppointmentTime != AppointmentTimeOption.Anytime)
			{
				var today = DateTime.UtcNow.Date;

				if (filter.AppointmentTime == AppointmentTimeOption.Today)
				{
					query = query.Where(d => d.DoctorSchedules.Any(s => s.Date.Date == today));
				}
				// No need for "Anytime" case — it means no filtering
			}

			// Filter: Visit Type toggles
			if (filter.InPerson && !filter.VideoCall)
			{
				query = query.Where(d => d.VisitTypes.HasFlag(VisitType.InPerson));
			}
			else if (!filter.InPerson && filter.VideoCall)
			{
				query = query.Where(d => d.VisitTypes.HasFlag(VisitType.VideoCall));
			}
			else if (filter.InPerson && filter.VideoCall)
			{
				query = query.Where(d =>
					d.VisitTypes.HasFlag(VisitType.InPerson) ||
					d.VisitTypes.HasFlag(VisitType.VideoCall));
			}
			else
			{
				// Both false → return empty list
				return Ok(new List<DoctorDtoResponse>());
			}

			var doctors = await query
				.Select(d => new DoctorDtoResponse
				{
					DoctorId = d.DoctorId,
					FullName = d.FirstName + " " + d.LastName,
					Specialty = d.Specialty.Name,
					AvailableVisitTypes = d.VisitTypes.ToListOfStrings()
				})
				.ToListAsync();


			return Ok(doctors);
		}

        [HttpGet("{doctorId}/available-slots")]
        public async Task<ActionResult<IEnumerable<AvailableSlotsResponseDto>>> GetAvailableSlots(string doctorId, DateTime date)
        {
			var schedules = await _context.DoctorSchedules
			.Include(s => s.Appointments)
			.Where(s => s.DoctorId == doctorId && s.Date == date.Date)
			.ToListAsync();

			if (!schedules.Any())
				return NotFound(new { message = "No schedule found for this doctor on the given date." });

			var response = new AvailableSlotsResponseDto
			{
				Date = date.ToString("yyyy-MM-dd"),
				TimeSlots = new List<TimeSlotDto>()
			};

			// Flatten all appointments for the day
			var allAppointments = schedules
				.SelectMany(s => s.Appointments)
				.Where(a => a.Status == AppointmentStatus.Confirmed)
				.ToList();

			foreach (var schedule in schedules)
			{
				var current = schedule.StartTime;

				while (current < schedule.EndTime)
				{
					var slotStart = current;
					var slotEnd = current.Add(TimeSpan.FromMinutes(schedule.SlotDurationMinutes));

					// Check if the current slot overlaps with any confirmed appointment
					bool isBooked = allAppointments.Any(a =>
					{
						var appointmentEnd = a.EndTime ?? a.StartTime.Add(TimeSpan.FromMinutes(schedule.SlotDurationMinutes));
						return a.StartTime < slotEnd && appointmentEnd > slotStart;
					});

					response.TimeSlots.Add(new TimeSlotDto
					{
						StartTime = slotStart.ToString(@"hh\:mm\:ss"),
						IsAvailable = !isBooked
					});

					current = slotEnd;
				}
				
			}

			// Sort slots
			response.TimeSlots = response.TimeSlots.OrderBy(t => TimeSpan.Parse(t.StartTime)).ToList();

			return Ok(response);
		}

		[HttpPost("BookAppointement")]
		public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDto dto)
		{
			var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(patientId))
				return Unauthorized();

			if (!TimeSpan.TryParse(dto.StartTime, out var startTime))
				return BadRequest(new { message = "Invalid start time format." });

			if (!Enum.TryParse<VisitType>(dto.VisitType, true, out var visitType))
				return BadRequest(new { message = "Invalid visit type." });

			// Find the doctor's schedule for the given date
			var schedule = await _context.DoctorSchedules
				.Include(s => s.Appointments)
				.FirstOrDefaultAsync(s => s.DoctorId == dto.DoctorId && s.Date == dto.Date.Date);

			if (schedule == null)
				return NotFound(new { message = "No schedule found for this doctor on that date." });

			var slotDuration = TimeSpan.FromMinutes(schedule.SlotDurationMinutes);
			var endTime = startTime.Add(slotDuration);

			// Check if the slot is already booked
			bool isSlotTaken = schedule.Appointments.Any(a =>
				a.StartTime < endTime &&
				a.EndTime.HasValue &&
				a.EndTime.Value > startTime &&
				a.Status == AppointmentStatus.Confirmed
			);

			if (isSlotTaken)
				return BadRequest(new { message = "This slot is already booked." });

			// Create appointment with Pending status until payment is confirmed
			var appointment = new Appointment
			{
				AppointmentDate = dto.Date,
				StartTime = startTime,
				EndTime = endTime,
				Status = AppointmentStatus.Pending,
				VisitType = visitType,
				ReasonForVisit = dto.ReasonForVisit,
				DoctorId = dto.DoctorId,
				PatientId = patientId,
				ScheduleId = schedule.ScheduleId
			};

			_context.Appointments.Add(appointment);
			await _context.SaveChangesAsync();

			return Ok(new BookAppointmentResponseDto
			{
				AppointmentId = appointment.AppointmentId,
				Message = "Appointment created successfully. Awaiting payment confirmation.",
				Status = appointment.Status.ToString()
			});
		}
	}

}
