using TransportService.Models;

namespace TransportService.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetAllAsync();
        Task<Vehicle?> GetByIdAsync(int id);
        Task<Vehicle> AddAsync(Vehicle vehicle);
    }
}
