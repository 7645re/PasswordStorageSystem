using Domain.DTO;

namespace Domain.Services;

public interface ITokenService
{
    TokenInfo GenerateToken(string userLogin);
}