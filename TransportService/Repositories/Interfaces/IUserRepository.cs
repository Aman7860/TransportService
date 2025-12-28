using TransportService.Data.Entities;

namespace TransportService.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
