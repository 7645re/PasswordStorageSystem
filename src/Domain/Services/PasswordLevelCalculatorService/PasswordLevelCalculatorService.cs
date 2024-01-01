using System.Text.RegularExpressions;
using Domain.Enums;

namespace Domain.Services.PasswordLevelCalculatorService;


public static class PasswordLevelCalculatorService
{
    public static PasswordSecurityLevel CalculateLevel(this string password)
    {
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

        return score >= 3 ? PasswordSecurityLevel.Secure : PasswordSecurityLevel.Insecure;
    }
}