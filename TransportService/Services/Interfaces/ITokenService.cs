using TransportService.Data.Entities;

namespace TransportService.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(Guid userId);
    }
}
