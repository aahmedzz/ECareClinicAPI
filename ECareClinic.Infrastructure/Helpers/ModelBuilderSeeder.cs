using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECareClinic.Core.Entities;
using ECareClinic.Core.Enums;
using ECareClinic.Core.Identity;
using ECareClinic.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECareClinic.Infrastructure.Helpers
{
	public static class ModelBuilderSeeder
	{
		public static void SeedSpecialties(this ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Specialty>().HasData(
				new Specialty { SpecialtyId = 1, Name = "Orthopedic" },
				new Specialty { SpecialtyId = 2, Name = "Pediatric" },
				new Specialty { SpecialtyId = 3, Name = "Neurosurgeon" },
				new Specialty { SpecialtyId = 4, Name = "Orthopedics" },
				new Specialty { SpecialtyId = 5, Name = "Pediatrics" },
				new Specialty { SpecialtyId = 6, Name = "Psychiatry" },
				new Specialty { SpecialtyId = 7, Name = "Radiology" }
			);
		}
	}
}
