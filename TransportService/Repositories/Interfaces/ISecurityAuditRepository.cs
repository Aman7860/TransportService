using TransportService.Data.Entities;

namespace TransportService.Repositories.Interfaces
{
    public interface ISecurityAuditRepository
    {
        Task AddAsync(SecurityAuditLog log, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
