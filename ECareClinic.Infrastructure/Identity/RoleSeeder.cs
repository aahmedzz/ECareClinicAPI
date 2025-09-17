
using ECareClinic.Core.Identity;
using Microsoft.AspNetCore.Identity;

namespace ECareClinic.Infrastructure.Identity
{
	public static class RoleSeeder
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
	}
}
