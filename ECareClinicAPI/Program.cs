using Asp.Versioning.ApiExplorer;
using ECareClinic.Core.Identity;
using ECareClinic.Infrastructure.Data;
using ECareClinic.Infrastructure.Identity;
using ECareClinicAPI.Extensions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		//options.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic API v1");
		var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

		foreach (var description in provider.ApiVersionDescriptions)
		{
			options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
									description.GroupName.ToUpperInvariant());
		}
	});
//}
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
	var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	// Seed Roles
	await AppSeeder.SeedRolesAsync(roleManager);
	await AppSeeder.SeedDoctorsAsync(context, userManager, roleManager);
	await AppSeeder.SeedDoctorSchedulesAsync(context);
}
app.Run();
