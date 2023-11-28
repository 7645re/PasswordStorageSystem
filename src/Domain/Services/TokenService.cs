using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.DTO;
using Domain.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IMemoryCache _cachedTokensByLogins;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptions<JwtOptions> jwtOptions, IMemoryCache cachedTokensByLogins,
        ILogger<TokenService> logger)
    {
        _cachedTokensByLogins = cachedTokensByLogins;
        _logger = logger;
        _jwtOptions = jwtOptions.Value;
    }

    public TokenInfo GenerateToken(string userLogin)
    {
        if (_cachedTokensByLogins.TryGetValue(userLogin, out TokenInfo? cachedToken)
            && cachedToken != null)
        {
            _logger.LogInformation($"JWT token was issued for the user {userLogin} from the cache");
            return cachedToken;
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userLogin)
        };
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.Now.AddHours(_jwtOptions.ExpireHours),
            signingCredentials: credentials
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        var expire = DateTimeOffset.UtcNow.AddHours(_jwtOptions.ExpireHours);
        cachedToken = new TokenInfo(tokenValue, expire);
        _cachedTokensByLogins.Set(userLogin, cachedToken, TimeSpan.FromHours(_jwtOptions.ExpireHours));
        _logger.LogInformation($"New JWT token was generated for the user {userLogin}");

        return cachedToken;
    }
}