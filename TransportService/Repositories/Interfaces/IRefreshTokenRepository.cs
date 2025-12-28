using TransportService.Data.Entities;

namespace TransportService.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetAsync(string token, CancellationToken ct);
        Task AddAsync(RefreshToken token, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
