using Domain.DTO.Credential;

namespace Domain.Validators.CredentialValidator;

public interface ICredentialValidator
{
    void ValidateCredentialCreate(CredentialCreate credentialCreate);
    void ValidateCredentialUpdate(CredentialUpdate credentialUpdate);
}