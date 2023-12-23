using Domain.DTO;

namespace Domain.Validators.UserValidator;

public interface IUserValidator
{
    void Validate(UserCreate userCreate);
    void ValidatePassword(string password);
    void ValidateLogin(string login);
}