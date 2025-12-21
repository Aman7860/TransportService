using TransportService.DTOs.Requests;
using TransportService.DTOs.Responses;
using TransportService.Models;
using TransportService.Repositories.Interfaces;

namespace TransportService.Handlers
{
    public class VehicleHandler
    {
        private readonly IVehicleRepository _repository;

        public VehicleHandler(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<VehicleResponse>> GetAllAsync()
        {
            var vehicles = await _repository.GetAllAsync();

            return vehicles.Select(v => new VehicleResponse
            {
                Id = v.Id,
                Name = v.Name,
                Brand = v.Brand,
                Year = v.Year,
                Price = v.Price
            }).ToList();
        }

        public async Task<VehicleResponse> CreateAsync(VehicleCreateRequest request)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Vehicle name is required");

            if (request.Year < 2000)
                throw new ArgumentException("Invalid vehicle year");

            // DTO → Entity
            var vehicle = new Vehicle
            {
                Name = request.Name,
                Brand = request.Brand,
                Year = request.Year,
                Price = request.Price
            };

            var created = await _repository.AddAsync(vehicle);

            // Entity → DTO
            return new VehicleResponse
            {
                Id = created.Id,
                Name = created.Name,
                Brand = created.Brand,
                Year = created.Year,
                Price = created.Price
            };
        }
    }
}
