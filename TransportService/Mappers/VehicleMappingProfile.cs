using AutoMapper;
using TransportService.DTOs.Requests;
using TransportService.DTOs.Responses;
using TransportService.Models;

namespace TransportService.Mappers
{
    public class VehicleMappingProfile : Profile
    {
        public VehicleMappingProfile()
        {
            // Request → Entity
            CreateMap<VehicleCreateRequest, Vehicle>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Entity → Response   THIS WAS MISSING
            CreateMap<Vehicle, VehicleResponse>();
        }
    }
}
