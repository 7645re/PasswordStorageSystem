namespace Domain.Repositories;

public interface ITokenBlackListRepository
{
    Task<bool> ValidateTokenAsync(string token);
}