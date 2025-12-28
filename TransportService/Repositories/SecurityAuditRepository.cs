using TransportService.Data;
using TransportService.Data.Entities;
using TransportService.Repositories.Interfaces;

namespace TransportService.Repositories
{
    public sealed class SecurityAuditRepository : ISecurityAuditRepository
    {
        private readonly AppDbContext _db;

        public SecurityAuditRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(SecurityAuditLog log, CancellationToken ct)
        {
            await _db.SecurityAuditLogs.AddAsync(log, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct)
            => _db.SaveChangesAsync(ct);
    }
}
