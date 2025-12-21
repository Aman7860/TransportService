using TransportService.Data;

namespace TransportService.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly AppDbContext _context;

        protected BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        protected Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
