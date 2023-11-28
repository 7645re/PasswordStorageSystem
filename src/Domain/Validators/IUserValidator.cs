namespace Domain.Validators;

public interface IUserValidator
{
    ValidateResult Validate(string userLogin, string password);
}