using Microsoft.EntityFrameworkCore;
using TransportService.Data;
using TransportService.Models;
using TransportService.Repositories.Interfaces;

namespace TransportService.Repositories
{
    public class VehicleRepository : BaseRepository, IVehicleRepository
    {
        public VehicleRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<Vehicle>> GetAllAsync()
        {
            return _context.Vehicles.AsNoTracking().ToListAsync();
        }

        public Task<Vehicle?> GetByIdAsync(int id)
        {
            return _context.Vehicles.FindAsync(id).AsTask();
        }

        public async Task<Vehicle> AddAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await SaveChangesAsync();
            return vehicle;
        }
    }
}
