using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TransportService.Data;
using TransportService.Handlers;
using TransportService.Mappers;
using TransportService.Repositories;
using TransportService.Repositories.Interfaces;

namespace TransportService.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices( this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<VehicleHandler>();

            services.AddAutoMapper(typeof(VehicleMappingProfile));

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
            });


            return services;
        }
    }
}
