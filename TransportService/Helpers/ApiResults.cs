using TransportService.DTOs.Responses;

namespace TransportService.Helpers
{
    public static class ApiResults
    {
        public static IResult Ok<T>(HttpContext context, T data, string message)
            => Results.Ok(ApiResponse<T>.SuccessResponse(
                data,
                StatusCodes.Status200OK,
                message,
                context.TraceIdentifier));

        public static IResult Created<T>(HttpContext context, string location, T data, string message)
            => Results.Created(location, ApiResponse<T>.SuccessResponse(
                data,
                StatusCodes.Status201Created,
                message,
                context.TraceIdentifier));

        public static IResult NoContent(HttpContext context, string message)
            => Results.Ok(ApiResponse<object>.SuccessResponse(
                null,
                StatusCodes.Status204NoContent,
                message,
                context.TraceIdentifier));
    }
}
