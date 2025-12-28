using TransportService.DTOs.Requests;
using TransportService.DTOs.Responses;
using TransportService.Services.Interfaces;

namespace TransportService.Handlers
{
    public class AuthHandler
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthHandler> _logger;

        public AuthHandler(IAuthService authService, ILogger<AuthHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Processing login request");
            return await _authService.LoginAsync(dto, ct);
        }

        public async Task<LoginResponseDto> RefreshAsync(RefreshRequestDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Processing refresh token");
            return await _authService.RefreshAsync(dto, ct);
        }

        public async Task RegisterAsync(RegisterRequestDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Processing user registration");
            await _authService.RegisterAsync(dto, ct);
        }

    }
}
