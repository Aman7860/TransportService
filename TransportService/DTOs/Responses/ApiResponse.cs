namespace TransportService.DTOs.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string TraceId { get; set; } = string.Empty;

        public static ApiResponse<T> SuccessResponse(T data, int statusCode, string message, string traceId)
            => new() { Success = true, StatusCode = statusCode, Message = message, Data = data, TraceId = traceId };

        public static ApiResponse<T> FailureResponse(int statusCode, string message, string traceId)
            => new() { Success = false, StatusCode = statusCode, Message = message, TraceId = traceId };
    }

}
