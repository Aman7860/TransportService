using SendGrid.Helpers.Errors.Model;
using TransportService.Data.Entities;
using TransportService.DTOs.Requests;
using TransportService.DTOs.Responses;
using TransportService.Exceptions;
using TransportService.Helpers;
using TransportService.Repositories.Interfaces;
using TransportService.Services.Interfaces;

namespace TransportService.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IRefreshTokenRepository _tokens;
        private readonly ITokenService _jwt;
        private readonly ILogger<AuthService> _logger;
        private readonly IRequestContextHelper _requestContext;
        private readonly ISecurityAuditRepository _audit;

        public AuthService(
            IUserRepository users,
            IRefreshTokenRepository tokens,
            ITokenService jwt,
            ILogger<AuthService> logger,
            IRequestContextHelper requestContext,
            ISecurityAuditRepository audit)
        {
            _users = users;
            _tokens = tokens;
            _jwt = jwt;
            _logger = logger;
            _requestContext = requestContext;
            _audit = audit;
        }

        // LOGIN FLOW
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct)
        {
            var (ip, agent) = _requestContext.GetClientInfo();

            _logger.LogInformation("Login attempt | Email={Email} | IP={IP} | Agent={Agent}",
                dto.Email, ip, agent);

            var user = await _users.GetByEmailAsync(dto.Email, ct);

            if (user == null)
            {
                await WriteAuditAsync("LOGIN", dto.Email, false, ip, agent, ct);

                _logger.LogWarning("Login failed: email not found | Email={Email} | IP={IP} | Agent={Agent}",
                    dto.Email, ip, agent);

                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                await WriteAuditAsync("LOGIN", dto.Email, false, ip, agent, ct);

                _logger.LogWarning("Login failed: invalid password | Email={Email} | IP={IP} | Agent={Agent}",
                    dto.Email, ip, agent);

                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (!user.IsActive)
            {
                await WriteAuditAsync("LOGIN", dto.Email, false, ip, agent, ct);

                _logger.LogWarning("Login blocked: account disabled | Email={Email} | IP={IP}",
                    dto.Email, ip);

                throw new ForbiddenException("Account is disabled");
            }

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken(user.Id);

            await _tokens.AddAsync(refreshToken, ct);
            await _tokens.SaveChangesAsync(ct);

            await WriteAuditAsync("LOGIN", dto.Email, true, ip, agent, ct);

            _logger.LogInformation("Login success | Email={Email} | IP={IP}", dto.Email, ip);

            return new LoginResponseDto(accessToken, refreshToken.Token);
        }

        // REFRESH FLOW
        public async Task<LoginResponseDto> RefreshAsync(RefreshRequestDto dto, CancellationToken ct)
        {
            var (ip, agent) = _requestContext.GetClientInfo();

            _logger.LogInformation("Refresh token request | IP={IP} | Agent={Agent}", ip, agent);

            var token = await _tokens.GetAsync(dto.RefreshToken, ct);

            if (token == null)
            {
                await WriteAuditAsync("REFRESH", "UNKNOWN", false, ip, agent, ct);

                _logger.LogWarning("Invalid refresh token used | IP={IP} | Agent={Agent}", ip, agent);

                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            if (token.IsRevoked || token.ExpiresAt <= DateTime.UtcNow)
            {
                await WriteAuditAsync("REFRESH", token.User.Email, false, ip, agent, ct);

                _logger.LogWarning("Expired/revoked refresh token | UserId={UserId} | IP={IP}",
                    token.UserId, ip);

                throw new UnauthorizedAccessException("Refresh token expired");
            }

            var user = token.User;

            if (!user.IsActive)
            {
                await WriteAuditAsync("REFRESH", user.Email, false, ip, agent, ct);

                _logger.LogWarning("Refresh blocked: account disabled | UserId={UserId} | IP={IP}",
                    user.Id, ip);

                throw new ForbiddenException("Account is disabled");
            }

            token.IsRevoked = true;

            var newAccess = _jwt.GenerateAccessToken(user);
            var newRefresh = _jwt.GenerateRefreshToken(user.Id);

            await _tokens.AddAsync(newRefresh, ct);
            await _tokens.SaveChangesAsync(ct);

            await WriteAuditAsync("REFRESH", user.Email, true, ip, agent, ct);

            _logger.LogInformation("Refresh success | UserId={UserId} | IP={IP}", user.Id, ip);

            return new LoginResponseDto(newAccess, newRefresh.Token);
        }

        // REGISTER FLOW
        public async Task RegisterAsync(RegisterRequestDto dto, CancellationToken ct)
        {
            var (ip, agent) = _requestContext.GetClientInfo();

            _logger.LogInformation("Registration attempt | Email={Email} | IP={IP} | Agent={Agent}",
                dto.Email, ip, agent);

            var existing = await _users.GetByEmailAsync(dto.Email, ct);

            if (existing != null)
            {
                await WriteAuditAsync("REGISTER", dto.Email, false, ip, agent, ct);

                _logger.LogWarning("Registration failed: duplicate email | Email={Email} | IP={IP}",
                    dto.Email, ip);

                throw new DuplicateRecordException("Email already registered", "EMAIL_DUPLICATE");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                IsActive = true
            };

            await _users.AddAsync(user, ct);
            await _users.SaveChangesAsync(ct);

            await WriteAuditAsync("REGISTER", dto.Email, true, ip, agent, ct);

            _logger.LogInformation("Registration success | Email={Email} | IP={IP}", dto.Email, ip);
        }

        // 🧾 AUDIT HELPER
        private async Task WriteAuditAsync(
            string eventType,
            string email,
            bool success,
            string ip,
            string agent,
            CancellationToken ct)
        {
            var log = new SecurityAuditLog
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                Email = email,
                IpAddress = ip,
                UserAgent = agent,
                Success = success
            };

            await _audit.AddAsync(log, ct);
            await _audit.SaveChangesAsync(ct);
        }
    }
}
