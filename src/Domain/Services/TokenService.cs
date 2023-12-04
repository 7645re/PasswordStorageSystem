using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.DTO;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Services;

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
        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.Now.AddHours(_jwtOptions.AccessTokenExpireHours),
            signingCredentials: credentials
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        var expire = DateTimeOffset.UtcNow.AddHours(_jwtOptions.AccessTokenExpireHours);
        _logger.LogInformation($"New JWT access token was generated for the user {userLogin}");

        return new TokenInfo(tokenValue, expire);
    }
}