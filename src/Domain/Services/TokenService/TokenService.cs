using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.DTO;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Services.TokenService;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptions<JwtOptions> jwtOptions, ILogger<TokenService> logger)
    {
        _logger = logger;
        _jwtOptions = jwtOptions.Value;
    }

    public TokenInfo GenerateAccessToken(string userLogin)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userLogin)
        };
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expire = DateTime.UtcNow.AddHours(_jwtOptions.AccessTokenExpireHours);

        var expireWithoutMilSecAndMacroSec = new DateTime(
            expire.Year,
            expire.Month,
            expire.Day,
            expire.Hour,
            expire.Minute,
            expire.Second);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expireWithoutMilSecAndMacroSec,
            signingCredentials: credentials
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogInformation($"New JWT access token was generated for the user {userLogin}");

        return new TokenInfo(tokenValue, expireWithoutMilSecAndMacroSec);
    }
}