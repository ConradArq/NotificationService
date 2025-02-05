using System.Security.Claims;

namespace NotificationService.Infrastructure.Interfaces.Providers
{
    public interface IJwtTokenProvider
    {
        string GenerateAuthenticationToken(TimeSpan? expirationTime = null, params Claim[] claims);
        string? GetUserAuthenticationToken();
    }
}