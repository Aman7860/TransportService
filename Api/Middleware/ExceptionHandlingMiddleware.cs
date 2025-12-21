using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Automobile.Application.Handlers;

namespace Automobile.Api.Middleware
{
    /// <summary>
    /// Global exception handling middleware.
    /// Logs the exception and returns a JSON error response.
    /// Maps known application exceptions to appropriate HTTP status codes.
    /// </summary>
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log with request context
            _logger.LogError(exception, "Unhandled exception processing {Method} {Path}", context.Request?.Method, context.Request?.Path);

            var statusCode = exception switch
            {
                VehicleValidationException => StatusCodes.Status400BadRequest,
                VehicleNotFoundException => StatusCodes.Status404NotFound,
                VehicleOperationException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new
            {
                traceId = context.TraceIdentifier,
                status = statusCode,
                error = exception.Message,
                // include details only in Development to avoid leaking internals
                details = _env.IsDevelopment() ? exception.ToString() : null
            };

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var payload = JsonSerializer.Serialize(response, JsonOptions);
            await context.Response.WriteAsync(payload).ConfigureAwait(false);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Registers the global exception handling middleware.
        /// Call __UseGlobalExceptionHandling__ in Program.cs as early as possible in the pipeline.
        /// </summary>
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        /// <summary>
        /// Overload for WebApplication to allow fluent usage in minimal API Program.cs.
        /// Call __app.UseGlobalExceptionHandling()__.
        /// </summary>
        public static WebApplication UseGlobalExceptionHandling(this WebApplication app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            return app;
        }
    }
}