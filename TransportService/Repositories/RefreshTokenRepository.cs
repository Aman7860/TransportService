using Microsoft.EntityFrameworkCore;
using TransportService.Data;
using TransportService.Data.Entities;
using TransportService.Repositories.Interfaces;

namespace TransportService.Repositories
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<RefreshTokenRepository> _logger;

        public RefreshTokenRepository(AppDbContext db, ILogger<RefreshTokenRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<RefreshToken?> GetAsync(string token, CancellationToken ct)
        {
            return await _db.RefreshTokens
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.Token == token, ct);
        }

        public async Task AddAsync(RefreshToken token, CancellationToken ct)
        {
            await _db.RefreshTokens.AddAsync(token, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _db.SaveChangesAsync(ct);
        }
    }

}
