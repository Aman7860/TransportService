using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automobile.Domain.Entities;

namespace Automobile.Domain.Interfaces
{
    /// <summary>
    /// Repository contract for Vehicle aggregate.
    /// Implements async CRUD operations following the repository pattern.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Adds a new vehicle and returns the persisted entity (with Id).
        /// </summary>
        Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a vehicle by id or null if not found.
        /// </summary>
        Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all vehicles.
        /// </summary>
        Task<IEnumerable<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing vehicle.
        /// </summary>
        Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a vehicle by id.
        /// </summary>
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a vehicle exists.
        /// </summary>
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}