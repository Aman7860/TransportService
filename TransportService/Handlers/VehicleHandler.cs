using AutoMapper;
using TransportService.DTOs.Requests;
using TransportService.DTOs.Responses;
using TransportService.Exceptions;
using TransportService.Models;
using TransportService.Repositories.Interfaces;
using TransportService.Repositories.Queries;

namespace TransportService.Handlers
{
    public class VehicleHandler
    {
        private readonly IVehicleRepository _repository;
        private readonly IMapper _mapper;

        public VehicleHandler(IVehicleRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<VehicleResponse>> GetAllAsync(
    CancellationToken cancellationToken)
        {
            var vehicles = await _repository
                .GetAllAsync(cancellationToken);

            return _mapper.Map<List<VehicleResponse>>(vehicles);
        }

        public async Task<VehicleResponse> GetByIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var vehicle = await _repository
                .GetByIdAsync(id, cancellationToken);

            if (vehicle is null)
                throw new KeyNotFoundException(
                    $"No records found for vehicle with id {id}.");

            return _mapper.Map<VehicleResponse>(vehicle);
        }

        public async Task<VehicleResponse> CreateAsync(
            VehicleCreateRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            cancellationToken.ThrowIfCancellationRequested();

            var exists = await _repository.ExistsAsync(
                new VehicleDuplicateCheck
                {
                    Name = request.Name,
                    Brand = request.Brand,
                    Year = request.Year
                },
                cancellationToken);

            if (exists)
                throw new DuplicateRecordException(
                    "A vehicle with the same name, brand, and year already exists.",
                    "VEHICLE_DUPLICATE");

            var vehicle = _mapper.Map<Vehicle>(request);

            var created = await _repository
                .AddAsync(vehicle, cancellationToken);

            return _mapper.Map<VehicleResponse>(created);
        }

        public async Task<VehicleResponse> UpdateAsync(
     int id,
     VehicleCreateRequest request,
     CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                throw new KeyNotFoundException($"Vehicle with id {id} not found");

            // Map INTO existing tracked entity
            _mapper.Map(request, existing);

            await _repository.UpdateAsync(existing, cancellationToken);

            return _mapper.Map<VehicleResponse>(existing);
        }

        public async Task<VehicleResponse> PatchAsync(
    int id,
    VehiclePatchRequest request,
    CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            cancellationToken.ThrowIfCancellationRequested();

            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                throw new KeyNotFoundException($"Vehicle with id {id} not found");

            // 🔍 Check if uniqueness fields are changing
            bool uniquenessChanged =
                (request.Name != null &&
                 !string.Equals(existing.Name, request.Name, StringComparison.OrdinalIgnoreCase)) ||
                (request.Brand != null &&
                 !string.Equals(existing.Brand, request.Brand, StringComparison.OrdinalIgnoreCase)) ||
                (request.Year.HasValue &&
                 existing.Year != request.Year.Value);

            if (uniquenessChanged)
            {
                var exists = await _repository.ExistsAsync(
                    new VehicleDuplicateCheck
                    {
                        Name = request.Name ?? existing.Name,
                        Brand = request.Brand ?? existing.Brand,
                        Year = request.Year ?? existing.Year
                    },
                    id,
                    cancellationToken);

                if (exists)
                    throw new DuplicateRecordException(
                        "Another vehicle with the same name, brand, and year already exists.",
                        "VEHICLE_DUPLICATE");
            }

            // ✅ Apply partial updates safely
            if (request.Name != null)
                existing.Name = request.Name;

            if (request.Brand != null)
                existing.Brand = request.Brand;

            if (request.Year.HasValue)
                existing.Year = request.Year.Value;

            if (request.Price.HasValue)
                existing.Price = request.Price.Value;

            await _repository.UpdateAsync(existing, cancellationToken);

            return _mapper.Map<VehicleResponse>(existing);
        }


        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null) throw new KeyNotFoundException($"Vehicle with id {id} not found");

            await _repository.DeleteAsync(id, cancellationToken);
        }
    }
}