using Domain.DTO;

namespace Domain.Services.TokenService;

public interface ITokenService
{
    TokenInfo GenerateAccessToken(string userLogin);
}