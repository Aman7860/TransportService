using TransportService.DTOs.Requests;
using TransportService.DTOs.Responses;
using TransportService.Handlers;
using TransportService.Repositories.Queries;

namespace TransportService.Endpoints
{
    public static class VehicleEndpoints
    {
        public static IEndpointRouteBuilder MapVehicleEndpoints(this IEndpointRouteBuilder app)
        {

            var group = app.MapGroup("/api/vehicles")
                          .WithTags("Vehicles");

            group.MapGet("/GetVehicles/", async (
        VehicleHandler handler,
        CancellationToken cancellationToken) =>
            {
                return Results.Ok(await handler.GetAllAsync(cancellationToken));
            })
        .Produces<List<VehicleResponse>>(StatusCodes.Status200OK)
        .WithSummary("Get all vehicles")
        .WithDescription("Returns a list of all vehicles");

            group.MapGet("/GetVehicleDetails/{id:int}", async (
        int id,
        VehicleHandler handler,
        CancellationToken cancellationToken) =>
            {
                return Results.Ok(await handler.GetByIdAsync(id, cancellationToken));
            })
        .Produces<VehicleResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithSummary("Get vehicle details by Id")
        .WithDescription("Returns vehicle details for the given Id");

            group.MapPost("/", async (
        VehicleCreateRequest request,
        VehicleHandler handler,
        CancellationToken cancellationToken) =>
            {
                var result = await handler.CreateAsync(request, cancellationToken);
                return Results.Created("/api/vehicles", result);
            })
        .Produces<VehicleResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithSummary("Create a vehicle")
        .WithDescription("Creates a new vehicle");

            // PUT: /api/vehicles/{id}  (FULL update)
            group.MapPut("/UpdateVehicle/{id:int}", async (
                int id,
                VehicleCreateRequest request,
                VehicleHandler handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.UpdateAsync(
                    id, request, cancellationToken);

                return Results.Ok(result);
            })
            .Produces<VehicleResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Update a vehicle (full update)");

            // PATCH: /api/vehicles/{id}  (PARTIAL update)
            group.MapPatch("/UpdateVehicleDetails/{id:int}", async (
                int id,
                VehiclePatchRequest request,
                VehicleHandler handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.PatchAsync(
                    id, request, cancellationToken);

                return Results.Ok(result);
            })
            .Produces<VehicleResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Update a vehicle (partial update)");

            // DELETE: /api/vehicles/{id}
            group.MapDelete("/DeleteVehicle/{id:int}", async (
                int id,
                VehicleHandler handler,
                CancellationToken cancellationToken) =>
            {
                await handler.DeleteAsync(id, cancellationToken);
                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Delete a vehicle");

            return app;
        }
    }
}
