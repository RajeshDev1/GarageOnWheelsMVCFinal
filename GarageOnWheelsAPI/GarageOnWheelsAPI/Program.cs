    using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Services;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using GarageOnWheelsAPI.Models.DatabaseModels;
using GarageOnWheelsAPI.Data;
using Microsoft.Extensions.Options;
using GarageOnWheelsAPI.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using GarageOnWheelsAPI.Utils;

namespace GarageOnWheelsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]); 

            var provider = builder.Services.BuildServiceProvider();
            var config = provider.GetRequiredService<IConfiguration>();


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, 
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(config.GetConnectionString("MyConnection")).UseLazyLoadingProxies());

            builder.Services.AddScoped<JwtTokenGenerator>(); 
            builder.Services.AddScoped<PasswordHasher>();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Services.AddSingleton<ILoggerProvider,DatabaseLoggerProvider>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IRevenueService, RevenueService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IGarageService, GarageService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<IOtpService,OtpService>();

            // Register repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IGarageRepository, GarageRepository>();
            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            builder.Services.AddScoped<IOtpRepository, OtpRepository>();



            // Add controllers and API versioning
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GarageOnWheelsAPI", Version = "v1" });
            });

        

            var app = builder.Build();

            app.UseCors("AllowSpecificOrigin");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GarageOnWheelsAPI v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
