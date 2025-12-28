namespace TransportService.Helpers
{
    public sealed class RequestContextHelper : IRequestContextHelper
    {
        private readonly IHttpContextAccessor _accessor;

        public RequestContextHelper(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public (string Ip, string UserAgent) GetClientInfo()
        {
            var ctx = _accessor.HttpContext;

            return (
                ctx?.Connection.RemoteIpAddress?.ToString() ?? "UnknownIP",
                ctx?.Request.Headers["User-Agent"].ToString() ?? "UnknownAgent"
            );
        }
    }
}
