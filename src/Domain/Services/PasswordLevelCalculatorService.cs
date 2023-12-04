using System.Text.RegularExpressions;
using Domain.Enums;
using Domain.Repositories;

namespace Domain.Services;

public class PasswordLevelCalculatorService : IPasswordLevelCalculatorService
{
    private readonly ICredentialRepository _credentialRepository;
    
    public PasswordLevelCalculatorService(ICredentialRepository credentialRepository)
    {
        _credentialRepository = credentialRepository;
    }
    
    public async Task<PasswordSecurityLevel> CalculateLevelAsync(string userLogin, string password)
    {
        var usersHaveThisPassword = await _credentialRepository.FindPasswordDuplicatesAsync(password);
        if (usersHaveThisPassword.Any(l => l != userLogin)) return PasswordSecurityLevel.Compromised;
        
        var score = 0;

        switch (password.Length)
        {
            case < 6:
                return 0;
            case >= 10:
                score++;
                break;
        }

        if (Regex.Match(password, "[a-z]").Success)
            score++;

        if (Regex.Match(password, "[A-Z]").Success)
            score++;

        if (Regex.Match(password, "[0-9]").Success)
            score++;

        if (Regex.Match(password, "[^a-zA-Z0-9]").Success)
            score++;

        return score >= 4 ? PasswordSecurityLevel.Secure : PasswordSecurityLevel.Insecure;
    }
}