using Domain.DTO.User;

namespace Domain.Validators.UserValidator;

public interface IUserValidator
{
    void Validate(UserCreate userCreate);
    void ValidatePassword(string password);
}