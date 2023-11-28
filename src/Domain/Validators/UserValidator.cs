namespace Domain.Validators;

public class UserValidator : IUserValidator
{
    private readonly (int Min, int Max) _userLoginLengthRange = (3, 50);
    private readonly (int Min, int Max) _passwordLengthRange = (6, 50);
    
    public ValidateResult Validate(string userLogin, string password)
    {
        if (userLogin.Length < _userLoginLengthRange.Min ||
            userLogin.Length > _userLoginLengthRange.Max)
        {
            return new ValidateResult(false, $"Login {userLogin} length is not within the valid range");
        }
        
        if (password.Length < _passwordLengthRange.Min ||
            password.Length > _passwordLengthRange.Max)
        {
            return new ValidateResult(false, $"The password is too weak");
        }

        return new ValidateResult(true);
    }
}