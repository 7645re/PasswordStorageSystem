using Domain.DTO;

namespace Domain.Services;

public interface ITokenService
{
    TokenInfo GenerateAccessToken(string userLogin);
}