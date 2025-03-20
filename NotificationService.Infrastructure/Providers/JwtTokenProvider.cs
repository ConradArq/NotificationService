using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Infrastructure.Configuration;
using NotificationService.Infrastructure.Interfaces.Providers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotificationService.Infrastructure.Providers
{
    public class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtTokenProvider(IOptions<JwtSettings> jwtSettings, IHttpContextAccessor httpContextAccessor)
        {
            _jwtSettings = jwtSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }


        public string GenerateAuthenticationToken(TimeSpan? expirationTime = null, params Claim[] claims)
        {
            var systemClaims = new List<Claim>();

            if (!claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub))
            {
                systemClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, "System"));
            }

            var allClaims = systemClaims.Concat(claims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: allClaims,
                expires: expirationTime.HasValue ? DateTime.UtcNow.Add(expirationTime.Value) : null,
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }



        public string? GetUserAuthenticationToken()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authorizationHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }
    }
}
