using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using TransportService.Data;
using TransportService.Handlers;
using TransportService.Helpers;
using TransportService.Mappers;
using TransportService.Repositories;
using TransportService.Repositories.Interfaces;
using TransportService.Services;
using TransportService.Services.Interfaces;

namespace TransportService.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //var env = services.BuildServiceProvider()
            //      .GetRequiredService<IWebHostEnvironment>();

            var env = services.First(x => x.ServiceType == typeof(IWebHostEnvironment)).ImplementationInstance as IWebHostEnvironment;

            if (env.IsDevelopment())
            {
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseSqlServer(configuration.GetConnectionString("DevConnection")));
            }
            else
            {
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseNpgsql(configuration.GetConnectionString("ProdConnection")));
            }


            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<VehicleHandler>();

            services.AddAutoMapper(typeof(VehicleMappingProfile));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<AuthHandler>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRequestContextHelper, RequestContextHelper>();
            services.AddScoped<ISecurityAuditRepository, SecurityAuditRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
          var jwtKey = configuration["Jwt:Key"];

          if (string.IsNullOrWhiteSpace(jwtKey))
          {
              throw new InvalidOperationException("JWT signing key is missing.");
          }

          options.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,

              ValidIssuer = configuration["Jwt:Issuer"],
              ValidAudience = configuration["Jwt:Audience"],
              IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(jwtKey!)),

              NameClaimType = ClaimTypes.NameIdentifier,
              RoleClaimType = ClaimTypes.Role
          };
      });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));
            });

            services.AddHttpContextAccessor();

            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Transport Service API",
                    Version = "v1",
                    Description = "Minimal API for Vehicle Management"
                });

                // JWT Authentication in Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
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


            return services;
        }
    }
}
