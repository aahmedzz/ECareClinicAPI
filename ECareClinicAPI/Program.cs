using Asp.Versioning.ApiExplorer;
using ECareClinicAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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
}
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
