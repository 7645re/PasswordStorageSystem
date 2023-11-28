using Domain.DTO;

namespace Domain.Validators;

public class CredentialValidator : ICredentialValidator
{
    private readonly (int Min, int Max) _resourceNameLengthRange = (2, 50);
    private readonly (int Min, int Max) _resourceLoginLengthRange = (2, 20);
    private readonly (int Min, int Max) _resourcePasswordLengthRange = (5, 100);

    private ValidateResult ValidateCore(CredentialValidate credentialCreate)
    {
        if (credentialCreate.ResourceName.Length < _resourceNameLengthRange.Min ||
            credentialCreate.ResourceName.Length > _resourceNameLengthRange.Max)
        {
            return new ValidateResult(false, "Credential resource name length is not within the valid range");
        }

        if (credentialCreate.ResourceLogin.Length < _resourceLoginLengthRange.Min ||
            credentialCreate.ResourceLogin.Length > _resourceLoginLengthRange.Max)
        {
            return new ValidateResult(false, "Credential resource login length is not within the valid range");
        }

        if (credentialCreate.ResourcePassword.Length < _resourcePasswordLengthRange.Min ||
            credentialCreate.ResourcePassword.Length > _resourcePasswordLengthRange.Max)
        {
            return new ValidateResult(false, "Credential resource password length is not within the valid range");
        }

        return new ValidateResult(true);
    }

    public ValidateResult Validate(CredentialUpdate credentialUpdate)
    {
        return ValidateCore(new CredentialValidate(credentialUpdate.ResourceName, credentialUpdate.ResourceLogin,
            credentialUpdate.NewResourcePassword));
    }

    public ValidateResult Validate(CredentialCreate credentialCreate)
    {
        return ValidateCore(new CredentialValidate(credentialCreate.ResourceName, credentialCreate.ResourceLogin,
            credentialCreate.ResourcePassword));
    }
}

record CredentialValidate(string ResourceName, string ResourceLogin, string ResourcePassword);