using Domain.DTO;
using Domain.DTO.User;

namespace Domain.Validators.UserValidator;

public class UserValidator : IUserValidator
{
    private readonly (int Min, int Max) _userLoginLengthRange = (3, 50);
    private readonly (int Min, int Max) _passwordLengthRange = (6, 50);

    public void Validate(UserCreate userCreate)
    {
        ValidateLogin(userCreate.Login);
        ValidatePassword(userCreate.Password);
    }

    public void ValidatePassword(string password)
    {
        if (password.Length < _passwordLengthRange.Min ||
            password.Length > _passwordLengthRange.Max)
            throw new Exception("The password is too weak");
    }

    public void ValidateLogin(string login)
    {
        if (login.Length < _userLoginLengthRange.Min ||
            login.Length > _userLoginLengthRange.Max)
            throw new Exception($"Login {login} length is not within the valid range");
    }
}