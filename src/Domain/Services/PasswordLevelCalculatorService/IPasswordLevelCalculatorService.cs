using Domain.Enums;

namespace Domain.Services.PasswordLevelCalculatorService;

public interface IPasswordLevelCalculatorService
{
    Task<PasswordSecurityLevel> CalculateLevelAsync(string userLogin, string password);
}