using TransportService.DTOs.Responses;
using TransportService.Exceptions;

namespace TransportService.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            var traceId = context.TraceIdentifier;

            try
            {
                await _next(context);
            }
            catch (DuplicateRecordException ex)
            {
                _logger.LogWarning(ex, "Conflict occurred. TraceId: {TraceId}", traceId);

                context.Response.StatusCode = StatusCodes.Status409Conflict;

                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.FailureResponse(
                        StatusCodes.Status409Conflict,
                        ex.Message,
                        traceId));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request. TraceId: {TraceId}", traceId);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.FailureResponse(
                        StatusCodes.Status400BadRequest,
                        ex.Message,
                        traceId));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized request. TraceId: {TraceId}", traceId);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.FailureResponse(
                        StatusCodes.Status401Unauthorized,
                        ex.Message,
                        traceId));
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Request cancelled. TraceId: {TraceId}", traceId);

                context.Response.StatusCode = 499;

                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.FailureResponse(
                        499,
                        "Request was cancelled by the client.",
                        traceId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", traceId);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.FailureResponse(
                        StatusCodes.Status500InternalServerError,
                        "An unexpected error occurred. Please try again later.",
                        traceId));
            }
        }
    }
}
