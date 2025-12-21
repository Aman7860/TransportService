using Microsoft.AspNetCore.Builder;
using TransportService.DTOs.Requests;
using TransportService.DTOs.Responses;
using TransportService.Handlers;

namespace TransportService.Endpoints
{
    public static class VehicleEndpoints
    {
        public static IEndpointRouteBuilder MapVehicleEndpoints(this IEndpointRouteBuilder app)
        {

            var group = app.MapGroup("/api/vehicles")
                          .WithTags("Vehicles");

            // GET: /api/vehicles
            group.MapGet("/", async (VehicleHandler handler) =>
                    Results.Ok(await handler.GetAllAsync()))
                .Produces<List<VehicleResponse>>(StatusCodes.Status200OK)
                .WithSummary("Get all vehicles")
                .WithDescription("Returns a list of all vehicles");

            // POST: /api/vehicles
            group.MapPost("/", async (VehicleCreateRequest request, VehicleHandler handler) =>
                    Results.Created("/api/vehicles", await handler.CreateAsync(request)))
                .Produces<VehicleResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithSummary("Create a vehicle")
                .WithDescription("Creates a new vehicle");


            return app;
        }
    }
}
