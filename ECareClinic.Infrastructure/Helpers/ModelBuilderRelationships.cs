using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.Entities;
using ECareClinic.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ECareClinic.Infrastructure.Helpers
{
	public static class ModelBuilderRelationships
	{
		public static void ConfigureRelationships(this ModelBuilder modelBuilder)
		{
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

			modelBuilder.Entity<Specialty>()
				.HasMany(s => s.Doctors)
				.WithOne(d => d.Specialty)
				.HasForeignKey(d => d.SpecialtyId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
