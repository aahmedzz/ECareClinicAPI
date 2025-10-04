using ECareClinic.Core.Entities;
using ECareClinic.Core.Entities.Auth;
using ECareClinic.Core.Identity;
using ECareClinic.Core.Models;
using ECareClinic.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECareClinic.Infrastructure.Data
{
	public class AppDbContext:IdentityDbContext<ApplicationUser, ApplicationRole, string>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		public DbSet<Patient> Patients { get; set; } = null!;
		public DbSet<Doctor> Doctors { get; set; } = null!;
		public DbSet<Appointment> Appointments { get; set; } = null!;
		public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
		public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
		public DbSet<Payment> Payments { get; set; } = null!;
		public DbSet<Insurance> Insurances { get; set; } = null!;

		public DbSet<EmailVerification> EmailVerifications { get; set; } = null!;
		public DbSet<PasswordResetVerification> passwordResetVerifications { get; set; } = null!;

        public DbSet<DoctorSchedule> DoctorSchedules { get; set; } = null!;

        public DbSet<Specialty> Specialties { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ConfigureRelationships();
			modelBuilder.SeedSpecialties();
		}
    }
}
