using TransportService.DTOs.Requests;
using TransportService.DTOs.Responses;

namespace TransportService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct);
        Task<LoginResponseDto> RefreshAsync(RefreshRequestDto dto, CancellationToken ct);
        Task RegisterAsync(RegisterRequestDto dto, CancellationToken ct);
    }
}
