using System.Security.Claims;

namespace NotificationService.Domain.Interfaces.Infrastructure.Providers
{
    public interface IJwtTokenProvider
    {
        string GenerateAuthenticationToken(TimeSpan? expirationTime = null, params Claim[] claims);
        string? GetUserAuthenticationToken();
    }
}