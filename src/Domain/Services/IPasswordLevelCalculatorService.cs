using Domain.Enums;

namespace Domain.Services;

public interface IPasswordLevelCalculatorService
{
    Task<PasswordSecurityLevel> CalculateLevelAsync(string userLogin, string password);
}