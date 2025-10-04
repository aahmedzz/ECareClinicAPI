
using ECareClinic.Core.Entities;
using ECareClinic.Core.Enums;
using ECareClinic.Core.Identity;
using ECareClinic.Core.Models;
using ECareClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECareClinic.Infrastructure.Identity
{
	public static class AppSeeder
	{
		public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
		{
			string[] roles = { "Admin", "Patient", "Doctor" };

			foreach (var role in roles)
			{
				if (!await roleManager.RoleExistsAsync(role))
				{
					var applicationRole = new ApplicationRole
					{
						Name = role,
						NormalizedName = role.ToUpper()
					};

					await roleManager.CreateAsync(applicationRole);
				}
			}
		}
		public static async Task SeedDoctorsAsync(AppDbContext context,
		UserManager<ApplicationUser> userManager,
		RoleManager<ApplicationRole> roleManager)
		{
			// Ensure the database exists
			await context.Database.EnsureCreatedAsync();

			// Check if Doctors table already has data
			if (await context.Doctors.AnyAsync())
				return;

			// Ensure the "Doctor" role exists
			var doctorRole = await roleManager.FindByNameAsync("Doctor");
			if (doctorRole == null)
			{
				doctorRole = new ApplicationRole { Name = "Doctor", NormalizedName = "DOCTOR" };
				await roleManager.CreateAsync(doctorRole);
			}

			var doctors = new[]
			{
				new { FirstName = "John", LastName = "Smith", UserName = "john.smith", Email = "john.smith@clinic.com", PhoneNumber = "01012345678", Gender = Gender.Male, SpecialtyId = 1, VisitType = VisitType.InPerson|VisitType.VideoCall, DateOfBirth = new DateTime(1981, 5, 15), YearsOfExperience = 10, LicenceNumber = "LIC-1001" },
				new { FirstName = "Sarah", LastName = "Jones", UserName = "sarah.jones", Email = "sarah.jones@clinic.com", PhoneNumber = "01123456789", Gender = Gender.Female, SpecialtyId = 2, VisitType = VisitType.VideoCall, DateOfBirth = new DateTime(1982, 5, 15), YearsOfExperience = 8, LicenceNumber = "LIC-1002" },
				new { FirstName = "Michael", LastName = "Brown", UserName = "michael.brown", Email = "michael.brown@clinic.com", PhoneNumber = "01234567890", Gender = Gender.Male, SpecialtyId = 3, VisitType = VisitType.InPerson | VisitType.VideoCall, DateOfBirth = new DateTime(1983, 5, 15), YearsOfExperience = 12, LicenceNumber = "LIC-1003" },
				new { FirstName = "Emily", LastName = "Davis", UserName = "emily.davis", Email = "emily.davis@clinic.com", PhoneNumber = "01545678901", Gender = Gender.Female, SpecialtyId = 4, VisitType = VisitType.InPerson, DateOfBirth = new DateTime(1984, 5, 15), YearsOfExperience = 7, LicenceNumber = "LIC-1004" },
				new { FirstName = "David", LastName = "Wilson", UserName = "david.wilson", Email = "david.wilson@clinic.com", PhoneNumber = "01056789012", Gender = Gender.Male, SpecialtyId = 5, VisitType = VisitType.VideoCall, DateOfBirth = new DateTime(1985, 5, 15), YearsOfExperience = 9, LicenceNumber = "LIC-1005" },
				new { FirstName = "Olivia", LastName = "Taylor", UserName = "olivia.taylor", Email = "olivia.taylor@clinic.com", PhoneNumber = "01167890123", Gender = Gender.Female, SpecialtyId = 6, VisitType = VisitType.InPerson | VisitType.VideoCall, DateOfBirth = new DateTime(1986, 5, 15), YearsOfExperience = 11, LicenceNumber = "LIC-1006" },
				new { FirstName = "Daniel", LastName = "Martin", UserName = "daniel.martin", Email = "daniel.martin@clinic.com", PhoneNumber = "01278901234", Gender = Gender.Male, SpecialtyId = 7, VisitType = VisitType.InPerson, DateOfBirth = new DateTime(1987, 5, 15), YearsOfExperience = 13, LicenceNumber = "LIC-1007" },
				new { FirstName = "Sophia", LastName = "Anderson", UserName = "sophia.anderson", Email = "sophia.anderson@clinic.com", PhoneNumber = "01589012345", Gender = Gender.Female, SpecialtyId = 1, VisitType = VisitType.VideoCall, DateOfBirth = new DateTime(1988, 5, 15), YearsOfExperience = 6, LicenceNumber = "LIC-1008" },
				new { FirstName = "Liam", LastName = "Thomas", UserName = "liam.thomas", Email = "liam.thomas@clinic.com", PhoneNumber = "01090123456", Gender = Gender.Male, SpecialtyId = 2, VisitType = VisitType.InPerson, DateOfBirth = new DateTime(1989, 5, 15), YearsOfExperience = 10, LicenceNumber = "LIC-1009" },
				new { FirstName = "Emma", LastName = "Jackson", UserName = "emma.jackson", Email = "emma.jackson@clinic.com", PhoneNumber = "01101234567", Gender = Gender.Female, SpecialtyId = 3, VisitType = VisitType.InPerson, DateOfBirth = new DateTime(1990, 5, 15), YearsOfExperience = 8, LicenceNumber = "LIC-1010" },
				new { FirstName = "Noah", LastName = "White", UserName = "noah.white", Email = "noah.white@clinic.com", PhoneNumber = "01212345678", Gender = Gender.Male, SpecialtyId = 4, VisitType = VisitType.VideoCall, DateOfBirth = new DateTime(1991, 5, 15), YearsOfExperience = 9, LicenceNumber = "LIC-1011" },
				new { FirstName = "Ava", LastName = "Harris", UserName = "ava.harris", Email = "ava.harris@clinic.com", PhoneNumber = "01523456789", Gender = Gender.Female, SpecialtyId = 5, VisitType = VisitType.InPerson | VisitType.VideoCall, DateOfBirth = new DateTime(1992, 5, 15), YearsOfExperience = 10, LicenceNumber = "LIC-1012" },
				new { FirstName = "Lucas", LastName = "Clark", UserName = "lucas.clark", Email = "lucas.clark@clinic.com", PhoneNumber = "01034567890", Gender = Gender.Male, SpecialtyId = 6, VisitType = VisitType.InPerson, DateOfBirth = new DateTime(1993, 5, 15), YearsOfExperience = 7, LicenceNumber = "LIC-1013" },
				new { FirstName = "Mia", LastName = "Robinson", UserName = "mia.robinson", Email = "mia.robinson@clinic.com", PhoneNumber = "01145678901", Gender = Gender.Female, SpecialtyId = 7, VisitType = VisitType.VideoCall, DateOfBirth = new DateTime(1994, 5, 15), YearsOfExperience = 8, LicenceNumber = "LIC-1014" },
				new { FirstName = "Ethan", LastName = "Lewis", UserName = "ethan.lewis", Email = "ethan.lewis@clinic.com", PhoneNumber = "01256789012", Gender = Gender.Male, SpecialtyId = 1, VisitType = VisitType.InPerson | VisitType.VideoCall, DateOfBirth = new DateTime(1995, 5, 15), YearsOfExperience = 6, LicenceNumber = "LIC-1015" }
			};

			var doctorEntities = new List<Doctor>();
			foreach (var d in doctors)
			{
				var id = Guid.NewGuid().ToString();

				// Create Identity User
				var user = new ApplicationUser
				{
					Id = id,
					UserName = d.UserName,
					NormalizedUserName = d.UserName.ToUpper(),
					Email = d.Email,
					NormalizedEmail = d.Email.ToUpper(),
					PhoneNumber = d.PhoneNumber,
					EmailConfirmed = true,
					PhoneNumberConfirmed = true
				};

				var existingUser = await userManager.FindByEmailAsync(user.Email);
				if (existingUser == null)
				{
					var result = await userManager.CreateAsync(user, "Doctor@123");
					if (result.Succeeded)
					{
						await userManager.AddToRoleAsync(user, "Doctor");

						// Add Doctor record linked to the user
						doctorEntities.Add(new Doctor
						{
							DoctorId = id,
							FirstName = d.FirstName,
							LastName = d.LastName,
							Gender = d.Gender,
							DateOfBirth = d.DateOfBirth,
							YearsOfExperience = d.YearsOfExperience,
							LicenceNumber = d.LicenceNumber,
							SpecialtyId = d.SpecialtyId,
							VisitTypes = d.VisitType,
							User = user
						});
					}
				}
			}

			if (doctorEntities.Count > 0)
			{
				await context.Doctors.AddRangeAsync(doctorEntities);
				await context.SaveChangesAsync();
			}
		}
		public static async Task SeedDoctorSchedulesAsync(AppDbContext context)
		{
			if (context.DoctorSchedules.Any())
				return; // Already seeded

			var doctors = context.Doctors.ToList();
			var schedules = new List<DoctorSchedule>();

			var startDate = DateTime.UtcNow.Date;
			var endDate = startDate.AddMonths(1);

			foreach (var doctor in doctors)
			{
				for (var date = startDate; date < endDate; date = date.AddDays(1))
				{
					schedules.Add(new DoctorSchedule
					{
						DoctorId = doctor.DoctorId,
						Date = date,
						StartTime = new TimeSpan(8, 0, 0),
						EndTime = new TimeSpan(11, 0, 0),
						IsAvailable = true,
						SlotDurationMinutes = 30
					});

					schedules.Add(new DoctorSchedule
					{
						DoctorId = doctor.DoctorId,
						Date = date,
						StartTime = new TimeSpan(14, 0, 0),
						EndTime = new TimeSpan(17, 0, 0),
						IsAvailable = true,
						SlotDurationMinutes = 30
					});
				}
			}

			await context.DoctorSchedules.AddRangeAsync(schedules);
			await context.SaveChangesAsync();
		}
	}
}
