using Microsoft.EntityFrameworkCore;
using TransportService.Data;
using TransportService.Data.Entities;
using TransportService.Repositories.Interfaces;
using TransportService.Repositories.Queries;

namespace TransportService.Repositories
{
    public class VehicleRepository : BaseRepository, IVehicleRepository
    {
        public VehicleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Vehicle>> GetAllAsync(
      CancellationToken cancellationToken)
        {
            return await _context.Vehicles
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Vehicle?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _context.Vehicles
                .FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<bool> ExistsAsync(
            VehicleDuplicateCheck query,
            CancellationToken cancellationToken)
        {
            return await _context.Vehicles.AnyAsync(v =>
                v.Name == query.Name &&
                v.Brand == query.Brand &&
                v.Year == query.Year,
                cancellationToken);
        }

        public async Task<Vehicle> AddAsync(
            Vehicle vehicle,
            CancellationToken cancellationToken)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync(cancellationToken);
            return vehicle;
        }

        public async Task<bool> ExistsAsync(
    VehicleDuplicateCheck query,
    int excludeId,
    CancellationToken cancellationToken)
        {
            return await _context.Vehicles.AnyAsync(v =>
                v.Id != excludeId &&
                v.Name == query.Name &&
                v.Brand == query.Brand &&
                v.Year == query.Year,
                cancellationToken);
        }

        // UPDATE
        public async Task UpdateAsync(
            Vehicle toUpdate,
            CancellationToken cancellationToken)
        {
            if (toUpdate == null)
                throw new ArgumentNullException(nameof(toUpdate));

            _context.Vehicles.Update(toUpdate);

            await _context.SaveChangesAsync(cancellationToken);
        }

        // ✅ DELETE
        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var vehicle = await _context.Vehicles
                .FindAsync(new object[] { id }, cancellationToken);

            if (vehicle == null)
                throw new KeyNotFoundException(
                    $"Vehicle with id {id} not found.");

            _context.Vehicles.Remove(vehicle);

            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
