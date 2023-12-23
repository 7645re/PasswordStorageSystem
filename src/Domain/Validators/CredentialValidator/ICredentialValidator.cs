using Domain.DTO;

namespace Domain.Validators.CredentialValidator;

public interface ICredentialValidator
{
    void Validate(CredentialUpdate credentialUpdate);
    void Validate(CredentialCreate credentialCreate);
}