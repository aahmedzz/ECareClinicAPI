using ECareClinic.Core.Entities;
using ECareClinic.Core.Entities.Auth;
using ECareClinic.Core.Identity;
using ECareClinic.Core.Models;
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

        public DbSet<DoctorVisitType> DoctorVisitTypes { get; set; } = null!;

        public DbSet<Specialty> Specialties { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Patient>()
				.HasOne(p => p.User)
				.WithOne()
				.HasForeignKey<Patient>(p => p.PatientId);
			
			modelBuilder.Entity<Doctor>()
				.HasOne(p => p.User)
				.WithOne()
				.HasForeignKey<Doctor>(p => p.DoctorId);

			//Making Deletion Restrict to avoid cascading deletes
			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.Patient)
				.WithMany(p => p.Appointments)
				.HasForeignKey(a => a.PatientId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.Doctor)
				.WithMany(d => d.Appointments)
				.HasForeignKey(a => a.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<MedicalRecord>()
				.HasOne(m => m.Patient)
				.WithMany(p => p.MedicalRecords)
				.HasForeignKey(m => m.PatientId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<MedicalRecord>()
				.HasOne(m => m.Doctor)
				.WithMany(d => d.MedicalRecords)
				.HasForeignKey(m => m.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Payment>()
				.HasOne(p => p.Patient)
				.WithMany(pa => pa.Payments)
				.HasForeignKey(p => p.PatientId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Payment>()
				.HasOne(p => p.PaymentMethod)
				.WithMany(pm => pm.Payments)
				.HasForeignKey(p => p.PaymentMethodId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<PaymentMethod>()
				.HasOne(pm => pm.Patient)
				.WithMany(p => p.PaymentMethods)
				.HasForeignKey(pm => pm.PatientId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Insurance>()
				.HasOne(i => i.Patient)
				.WithMany(p => p.Insurances)
				.HasForeignKey(i => i.PatientId)
				.OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorSchedule>()
                .HasOne(s => s.Doctor)
                .WithMany(d => d.DoctorSchedules)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Schedule)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorVisitType>()
                .HasOne(dvt => dvt.Doctor)
                .WithMany(d => d.DoctorVisitTypes)
                .HasForeignKey(dvt => dvt.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorVisitType>()
                .HasOne(dvt => dvt.VisitType)
                .WithMany(vt => vt.DoctorVisitTypes)
                .HasForeignKey(dvt => dvt.VisitTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.VisitType)
                .WithMany(vt => vt.Appointments)
                .HasForeignKey(a => a.VisitTypeId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Specialty>()
                .HasMany(s => s.Doctors)
                .WithOne(d => d.Specialty)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
