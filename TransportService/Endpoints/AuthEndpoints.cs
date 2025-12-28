using TransportService.DTOs.Requests;
using TransportService.DTOs.Responses;
using TransportService.Handlers;
using TransportService.Helpers;

namespace TransportService.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            // LOGIN
            app.MapPost("/auth/login", async (
                HttpContext ctx,
                LoginRequestDto dto,
                AuthHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.LoginAsync(dto, ct);
                return ApiResults.Ok(ctx, result, "Login successful");
            })
            .WithSummary("User Login")
            .WithDescription("Authenticates user credentials and returns JWT access token and refresh token.")
            .Produces<ApiResponse<LoginResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();

            // REFRESH TOKEN
            app.MapPost("/auth/refresh", async (
                HttpContext ctx,
                RefreshRequestDto dto,
                AuthHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.RefreshAsync(dto, ct);
                return ApiResults.Ok(ctx, result, "Token refreshed successfully");
            })
            .WithSummary("Refresh Access Token")
            .WithDescription("Generates a new access token using a valid refresh token.")
            .Produces<ApiResponse<LoginResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();

            // REGISTER USER (ADMIN ONLY)
            app.MapPost("/auth/register", async (
                HttpContext ctx,
                RegisterRequestDto dto,
                AuthHandler handler,
                CancellationToken ct) =>
            {
                await handler.RegisterAsync(dto, ct);
                return ApiResults.Created<object>(ctx, "/auth/login", null, "User created successfully");
            })
            .WithSummary("Create New User")
            .WithDescription("Creates a new user account and assigns role. Admin privileges required.")
            .Produces<ApiResponse<object>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization("AdminOnly");
        }
    }

}
