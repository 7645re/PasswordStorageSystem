using Domain.DTO;

namespace Domain.Validators;

public interface ICredentialValidator
{
    ValidateResult Validate(CredentialUpdate credentialUpdate);
    ValidateResult Validate(CredentialCreate credentialCreate);
}