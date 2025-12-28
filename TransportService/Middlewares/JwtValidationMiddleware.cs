using System.Security.Claims;

namespace TransportService.Middlewares
{
    public sealed class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtValidationMiddleware> _logger;

        public JwtValidationMiddleware(RequestDelegate next, ILogger<JwtValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;

            if (user?.Identity is { IsAuthenticated: true })
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = user.FindFirst(ClaimTypes.Email)?.Value;
                var role = user.FindFirst(ClaimTypes.Role)?.Value;

                _logger.LogInformation(
                    "Authenticated request | UserId={UserId} | Email={Email} | Role={Role} | Path={Path}",
                    userId ?? "N/A",
                    email ?? "N/A",
                    role ?? "N/A",
                    context.Request.Path);
            }
            else
            {
                _logger.LogWarning(
                    "Unauthenticated request | Path={Path} | AuthorizationHeader={AuthHeader}",
                    context.Request.Path,
                    context.Request.Headers["Authorization"].FirstOrDefault() ?? "None");
            }

            await _next(context);
        }
    }
}
