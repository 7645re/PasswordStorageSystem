using Domain.DTO.Credential;

namespace Domain.Validators.CredentialValidator;

public class CredentialValidator : ICredentialValidator
{
    private readonly (int Min, int Max) _resourceNameLengthRange = (2, 50);
    private readonly (int Min, int Max) _resourceLoginLengthRange = (2, 20);
    private readonly (int Min, int Max) _resourcePasswordLengthRange = (5, 100);

    public void ValidateCredentialCreate(CredentialCreate credentialCreate)
    {
        ValidateResourceName(credentialCreate.ResourceName);
        ValidateResourceLogin(credentialCreate.ResourceLogin);
        ValidateResourcePassword(credentialCreate.ResourcePassword);
    }

    public void ValidateCredentialUpdate(CredentialUpdate credentialUpdate)
    {
        ValidateResourcePassword(credentialUpdate.NewPassword);
    }

    private void ValidateResourcePassword(string resourcePassword)
    {
        if (resourcePassword.Length < _resourcePasswordLengthRange.Min ||
            resourcePassword.Length > _resourcePasswordLengthRange.Max)
            throw new ArgumentException(
                "Credential resource password length is not within the valid range");
    }

    private void ValidateResourceLogin(string resourceLogin)
    {
        if (resourceLogin.Length < _resourceLoginLengthRange.Min ||
            resourceLogin.Length > _resourceLoginLengthRange.Max)
            throw new ArgumentException("Credential resource login length is not within the valid range");
    }

    private void ValidateResourceName(string resourceName)
    {
        if (resourceName.Length < _resourceNameLengthRange.Min ||
            resourceName.Length > _resourceNameLengthRange.Max)
            throw new ArgumentException(
                "Credential resource password length is not within the valid range");
    }
}