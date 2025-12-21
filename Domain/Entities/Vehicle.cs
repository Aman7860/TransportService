Application/Handlers/VehicleHandler.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automobile.Domain.Entities;
using Automobile.Domain.Interfaces;

namespace Automobile.Application.Handlers
{
    /// <summary>
    /// Handles input validation, business rules and repository coordination for Vehicle operations.
    /// All data access is performed through IVehicleRepository only. Exceptions are meaningful and
    /// wrap lower-level errors for callers (e.g. endpoints).
    /// </summary>
    public sealed class VehicleHandler
    {
        private const int MaxNameLength = 200;
        private const int MaxBrandLength = 100;
        private const int MinYear = 1886; // First automobile patent year
        private readonly IVehicleRepository _repository;

        public VehicleHandler(IVehicleRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Vehicle> CreateAsync(string name, string brand, int year, decimal price, CancellationToken cancellationToken = default)
        {
            ValidateInput(name, brand, year, price);

            try
            {
                var vehicle = new Vehicle(name.Trim(), brand.Trim(), year, price);
                return await _repository.AddAsync(vehicle, cancellationToken).ConfigureAwait(false);
            }
            catch (VehicleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new VehicleOperationException("An error occurred while creating the vehicle.", ex);
            }
        }

        public async Task<Vehicle> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new VehicleValidationException("Id must be a positive integer.");

            try
            {
                var vehicle = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
                return vehicle ?? throw new VehicleNotFoundException($"Vehicle with id {id} was not found.");
            }
            catch (VehicleValidationException)
            {
                throw;
            }
            catch (VehicleNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new VehicleOperationException("An error occurred while retrieving the vehicle.", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _repository.GetAllAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new VehicleOperationException("An error occurred while retrieving vehicles.", ex);
            }
        }

        public async Task<Vehicle> UpdateAsync(int id, string? name, string? brand, int? year, decimal? price, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new VehicleValidationException("Id must be a positive integer.");

            try
            {
                var existing = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false)
                              ?? throw new VehicleNotFoundException($"Vehicle with id {id} was not found.");

                // Validate only supplied fields; treat null as "no change"
                if (name is not null)
                {
                    ValidateName(name);
                    existing.UpdateName(name.Trim());
                }

                if (brand is not null)
                {
                    ValidateBrand(brand);
                    existing.UpdateBrand(brand.Trim());
                }

                if (year is not null)
                {
                    ValidateYear(year.Value);
                    existing.UpdateYear(year.Value);
                }

                if (price is not null)
                {
                    ValidatePrice(price.Value);
                    existing.UpdatePrice(price.Value);
                }

                await _repository.UpdateAsync(existing, cancellationToken).ConfigureAwait(false);
                return existing;
            }
            catch (VehicleValidationException)
            {
                throw;
            }
            catch (VehicleNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new VehicleOperationException("An error occurred while updating the vehicle.", ex);
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new VehicleValidationException("Id must be a positive integer.");

            try
            {
                var exists = await _repository.ExistsAsync(id, cancellationToken).ConfigureAwait(false);
                if (!exists) throw new VehicleNotFoundException($"Vehicle with id {id} was not found.");

                await _repository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            }
            catch (VehicleValidationException)
            {
                throw;
            }
            catch (VehicleNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new VehicleOperationException("An error occurred while deleting the vehicle.", ex);
            }
        }

        #region Validation Helpers

        private void ValidateInput(string name, string brand, int year, decimal price)
        {
            ValidateName(name);
            ValidateBrand(brand);
            ValidateYear(year);
            ValidatePrice(price);
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new VehicleValidationException("Name is required.");

            if (name.Trim().Length > MaxNameLength)
                throw new VehicleValidationException($"Name must be at most {MaxNameLength} characters.");
        }

        private void ValidateBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                throw new VehicleValidationException("Brand is required.");

            if (brand.Trim().Length > MaxBrandLength)
                throw new VehicleValidationException($"Brand must be at most {MaxBrandLength} characters.");
        }

        private void ValidateYear(int year)
        {
            var maxYear = DateTime.UtcNow.Year + 1;
            if (year < MinYear || year > maxYear)
                throw new VehicleValidationException($"Year must be between {MinYear} and {maxYear}.");
        }

        private void ValidatePrice(decimal price)
        {
            if (price < 0m)
                throw new VehicleValidationException("Price must be greater than or equal to 0.");
        }

        #endregion
    }

    #region Exceptions

    public class VehicleValidationException : Exception
    {
        public VehicleValidationException(string message) : base(message) { }
    }

    public class VehicleNotFoundException : Exception
    {
        public VehicleNotFoundException(string message) : base(message) { }
    }

    public class VehicleOperationException : Exception
    {
        public VehicleOperationException(string message, Exception? innerException = null) : base(message, innerException) { }
    }

    #endregion
}