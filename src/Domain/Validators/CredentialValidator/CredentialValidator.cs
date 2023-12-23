using Domain.DTO;
using Domain.Mappers;

namespace Domain.Validators.CredentialValidator;

public class CredentialValidator : ICredentialValidator
{
    private readonly (int Min, int Max) _resourceNameLengthRange = (2, 50);
    private readonly (int Min, int Max) _resourceLoginLengthRange = (2, 20);
    private readonly (int Min, int Max) _resourcePasswordLengthRange = (5, 100);

    private void ValidateCore(CredentialValidate credentialValidate)
    {
        if (credentialValidate.ResourceName.Length < _resourceNameLengthRange.Min ||
            credentialValidate.ResourceName.Length > _resourceNameLengthRange.Max)
        {
            throw new Exception("Credential resource name length is not within the valid range");
        }

        if (credentialValidate.ResourceLogin.Length < _resourceLoginLengthRange.Min ||
            credentialValidate.ResourceLogin.Length > _resourceLoginLengthRange.Max)
        {
            throw new Exception("Credential resource login length is not within the valid range");
        }

        if (credentialValidate.ResourcePassword.Length < _resourcePasswordLengthRange.Min ||
            credentialValidate.ResourcePassword.Length > _resourcePasswordLengthRange.Max)
        {
            throw new Exception("Credential resource password length is not within the valid range");
        }
    }

    public void Validate(CredentialUpdate credentialUpdate)
    {
        ValidateCore(credentialUpdate.ToCredentialValidate());
    }

    public void Validate(CredentialCreate credentialCreate)
    {
        ValidateCore(credentialCreate.ToCredentialValidate());
    }
}