using Domain.DTO;

namespace Domain.Validators;

public interface IUserValidator
{
    ValidateResult Validate(UserCreate userCreate);

    ValidateResult ValidatePassword(string password);

    ValidateResult ValidateLogin(string login);
}