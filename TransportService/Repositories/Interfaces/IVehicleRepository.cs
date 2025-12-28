using TransportService.Data.Entities;
using TransportService.Repositories.Queries;

namespace TransportService.Repositories.Interfaces
{
    public interface IVehicleRepository
    {

        Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken);
        Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken);

        Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        // CREATE duplicate check
        Task<bool> ExistsAsync(
            VehicleDuplicateCheck query,
            CancellationToken cancellationToken);

        // UPDATE duplicate check (exclude current record)
        Task<bool> ExistsAsync(
            VehicleDuplicateCheck query,
            int excludeId,
            CancellationToken cancellationToken);
    }
}
