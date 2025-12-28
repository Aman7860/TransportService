using TransportService.DTOs.Requests;
using TransportService.DTOs.Responses;
using TransportService.Handlers;
using TransportService.Helpers;
using TransportService.Repositories.Queries;

namespace TransportService.Endpoints
{
    public static class VehicleEndpoints
    {
        public static IEndpointRouteBuilder MapVehicleEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/vehicles")
                           .WithTags("Vehicles")
                           .RequireAuthorization();

            group.MapGet("/", async (HttpContext ctx, VehicleHandler handler, CancellationToken ct) =>
            {
                var data = await handler.GetAllAsync(ct);
                return ApiResults.Ok(ctx, data, "Vehicles fetched successfully");
            })
            .Produces<ApiResponse<List<VehicleResponse>>>(200)
            .WithSummary("Get all vehicles")
            .WithDescription("Returns all vehicles");

            group.MapGet("/{id:int}", async (HttpContext ctx, int id, VehicleHandler handler, CancellationToken ct) =>
            {
                var data = await handler.GetByIdAsync(id, ct);
                return ApiResults.Ok(ctx, data, "Vehicle fetched successfully");
            })
            .Produces<ApiResponse<VehicleResponse>>(200)
            .Produces(404)
            .WithSummary("Get vehicle by id");

            group.MapPost("/", async (HttpContext ctx, VehicleCreateRequest request, VehicleHandler handler, CancellationToken ct) =>
            {
                var result = await handler.CreateAsync(request, ct);
                return ApiResults.Created(ctx, $"/api/vehicles/{result.Id}", result, "Vehicle created successfully");
            })
            .Produces<ApiResponse<VehicleResponse>>(201)
            .Produces(400)
            .WithSummary("Create a vehicle")
            .RequireAuthorization("AdminOnly");

            group.MapPut("/{id:int}", async (HttpContext ctx, int id, VehicleCreateRequest request, VehicleHandler handler, CancellationToken ct) =>
            {
                var result = await handler.UpdateAsync(id, request, ct);
                return ApiResults.Ok(ctx, result, "Vehicle updated successfully");
            })
            .Produces<ApiResponse<VehicleResponse>>(200)
            .Produces(404)
            .WithSummary("Update a vehicle")
            .RequireAuthorization("AdminOnly");

            group.MapPatch("/{id:int}", async (HttpContext ctx, int id, VehiclePatchRequest request, VehicleHandler handler, CancellationToken ct) =>
            {
                var result = await handler.PatchAsync(id, request, ct);
                return ApiResults.Ok(ctx, result, "Vehicle partially updated");
            })
            .Produces<ApiResponse<VehicleResponse>>(200)
            .Produces(404)
            .WithSummary("Partially update a vehicle")
            .RequireAuthorization("AdminOnly");

            group.MapDelete("/{id:int}", async (HttpContext ctx, int id, VehicleHandler handler, CancellationToken ct) =>
            {
                await handler.DeleteAsync(id, ct);
                return ApiResults.NoContent(ctx, "Vehicle deleted successfully");
            })
            .Produces<ApiResponse<object>>(200)
            .Produces(404)
            .WithSummary("Delete a vehicle")
            .RequireAuthorization("AdminOnly");

            return app;
        }
    }

}
