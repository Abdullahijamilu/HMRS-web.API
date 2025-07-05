using HMRS_web.API.Models;
using HMRS_web.API.Services;
using HMRS_web.API.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace HMRS_web.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Setup Serilog for logging
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthorization();

            // Database context
            builder.Services.AddDbContext<HmrsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity configuration
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<HmrsContext>()
                .AddDefaultTokenProviders();

            // JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
            {
                logger.Error("JWT Key is missing or too short in appsettings.json. It must be at least 32 characters.");
                throw new InvalidOperationException("JWT Key is missing or too short in appsettings.json. It must be at least 32 characters.");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        
            builder.Services.AddScoped<JwtServices>();
            builder.Services.AddScoped<IAuthenticateServices, AuthenticateServices>();

            var app = builder.Build();

            // 3. Seed initial data (roles, departments, job roles)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var dbContext = services.GetRequiredService<HmrsContext>();

                    // Seed roles
                    var roles = new[] { "Admin", "Manager", "Employee" };
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                            logger.Information("Seeded role: {Role}", role);
                        }
                    }

                    // Seed departments
                    if (!await dbContext.Departments.AnyAsync())
                    {
                        dbContext.Departments.AddRange(
                            new Department
                            {
                                Id = Guid.NewGuid(),
                                Name = "IT",
                                Description = "Information Technology"
                            },
                            new Department
                            {
                                Id = Guid.NewGuid(),
                                Name = "HR",
                                Description = "Human Resources"
                            });
                        await dbContext.SaveChangesAsync();
                        logger.Information("Seeded departments: IT, HR");
                    }

                    
                    if (!await dbContext.JobRoles.AnyAsync())
                    {
                        dbContext.JobRoles.AddRange(
                            new JobRole
                            {
                                Id = Guid.NewGuid(),
                                Title = "Software Developer",
                                Description = "Develops software applications"
                            },
                            new JobRole
                            {
                                Id = Guid.NewGuid(),
                                Title = "HR Manager",
                                Description = "Manages human resources"
                            });
                        await dbContext.SaveChangesAsync();
                        logger.Information("Seeded job roles: Software Developer, HR Manager");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error seeding initial data");
                    throw; 
                }
            }

            
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HRMS API v1"));
            }

            
            app.UseAuthentication(); 
            app.UseAuthorization();
            app.MapControllers();

            
            await app.RunAsync();
        }
    }
}