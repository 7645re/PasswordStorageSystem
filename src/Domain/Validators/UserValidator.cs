using Domain.DTO;

namespace Domain.Validators;

public class UserValidator : IUserValidator
{
    private readonly (int Min, int Max) _userLoginLengthRange = (3, 50);
    private readonly (int Min, int Max) _passwordLengthRange = (6, 50);
    
    public ValidateResult Validate(UserCreate userCreate)
    {
        var loginValidateResult = ValidateLogin(userCreate.Login);
        if (!loginValidateResult.IsSuccess) return loginValidateResult;
        
        var passwordValidateResult = ValidatePassword(userCreate.Password);
        if (!passwordValidateResult.IsSuccess) return passwordValidateResult;
        

        return new ValidateResult(true);
    }
    
    public ValidateResult ValidatePassword(string password)
    {
        if (password.Length < _passwordLengthRange.Min ||
            password.Length > _passwordLengthRange.Max)
            return new ValidateResult(false, $"The password is too weak");

        return new ValidateResult(true);
    }
    
    public ValidateResult ValidateLogin(string login)
    {
        if (login.Length < _userLoginLengthRange.Min ||
            login.Length > _userLoginLengthRange.Max)
            return new ValidateResult(false, $"Login {login} length is not within the valid range");

        return new ValidateResult(true);
    }
}