namespace TransportService.Helpers
{
    public interface IRequestContextHelper
    {
        (string Ip, string UserAgent) GetClientInfo();
    }
}
