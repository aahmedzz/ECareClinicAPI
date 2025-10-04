using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using ECareClinic.Core.Identity;
using ECareClinic.Core.ServiceContracts;
using ECareClinic.Infrastructure.Services;
using ECareClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace ECareClinicAPI.Extensions
{
	public static class ApplicationServicesExtension
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
		{
			//Make the api send and receive json only
			services.AddControllers(options =>
			{
				options.Filters.Add(new ProducesAttribute("application/json"));
				//options.Filters.Add(new ConsumesAttribute("application/json"));
			}).AddJsonOptions(options =>
			{
				// Serialize enums as strings instead of numbers
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});

			// API Versioning
			services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ReportApiVersions = true;
				options.ApiVersionReader = new UrlSegmentApiVersionReader();
			})
			.AddApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'VVV"; // v1, v2, etc.
				options.SubstituteApiVersionInUrl = true;
			});

			// Swagger
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(options =>
			{
				//options.SwaggerDoc("v1", new OpenApiInfo
				//{
				//	Title = "Clinic Web API",
				//	Version = "1.0"
				//});
				// resolve versioned API descriptions
				var provider = services.BuildServiceProvider()
									   .GetRequiredService<IApiVersionDescriptionProvider>();

				foreach (var description in provider.ApiVersionDescriptions)
				{
					options.SwaggerDoc(description.GroupName, new OpenApiInfo
					{
						Title = "Clinic Web API",
						Version = description.ApiVersion.ToString(),
					});
				}

				// Add JWT Authentication support
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Enter 'Bearer' [space] and then your valid token.\r\n\r\nExample: \"Bearer abc123\""
				});

				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						Array.Empty<string>()
					}
				});
			});


			//Database
			var connectionString = configuration.GetConnectionString("DefaultConnection")?? 
				throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			services.AddDbContext<AppDbContext>(options =>
			{
				options.UseSqlServer(connectionString,
					sqlOptions =>
					{
						sqlOptions.EnableRetryOnFailure(
							maxRetryCount: 5,              // Number of retries
							maxRetryDelay: TimeSpan.FromSeconds(10), // Delay between retries
							errorNumbersToAdd: null        // You can add specific SQL error codes if needed
						);
					});
			});

			//Identity
			services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
			{
				options.Password.RequiredLength = 8;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireDigit = true;
				options.User.RequireUniqueEmail = true;
			})
			 .AddEntityFrameworkStores<AppDbContext>()
			 .AddDefaultTokenProviders();

			//config JWT authentication
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))

				};
			});

			//Configurations
			services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.SuppressModelStateInvalidFilter = true;
			});

			//Service
			services.AddScoped<ITokenService,TokenService>();
			services.AddTransient<IEmailService, EmailService>();
			services.AddScoped<IRegisterService, RegisterService>();
			services.AddScoped<ILoginService, LoginService>();
			services.AddScoped<IProfileService, ProfileService>();


			return services;
		}
	}
}
