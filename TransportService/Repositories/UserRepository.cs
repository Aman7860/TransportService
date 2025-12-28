using Microsoft.EntityFrameworkCore;
using TransportService.Data;
using TransportService.Data.Entities;
using TransportService.Repositories.Interfaces;

namespace TransportService.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext db, ILogger<UserRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
        {
            return await _db.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Email == email, ct);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Users
                .Include(x => x.RefreshTokens)
                .SingleOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct)
        {
            await _db.Users.AddAsync(user, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _db.SaveChangesAsync(ct);
        }
    }

}
