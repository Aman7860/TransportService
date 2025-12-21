using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Automobile.Application.Handlers;
using Automobile.Domain.Entities;

namespace Automobile.Api.Endpoints
{
    public static class VehicleEndpoints
    {
        public static RouteGroupBuilder MapVehicleEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/vehicles");

            // GET /api/vehicles
            group.MapGet("/", async (VehicleHandler handler, CancellationToken cancellationToken) =>
            {
                var vehicles = await handler.GetAllAsync(cancellationToken).ConfigureAwait(false);
                return Results.Ok(vehicles);
            });

            // GET /api/vehicles/{id}
            group.MapGet("/{id:int}", async (int id, VehicleHandler handler, CancellationToken cancellationToken) =>
            {
                var vehicle = await handler.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
                return Results.Ok(vehicle);
            });

            // POST /api/vehicles
            group.MapPost("/", async (CreateVehicleRequest request, VehicleHandler handler, CancellationToken cancellationToken) =>
            {
                var created = await handler.CreateAsync(request.Name, request.Brand, request.Year, request.Price, cancellationToken).ConfigureAwait(false);
                return Results.Created($"/api/vehicles/{created.Id}", created);
            });

            return group;
        }

        // Simple request DTO local to the endpoint file
        public sealed record CreateVehicleRequest(string Name, string Brand, int Year, decimal Price);
    }
}