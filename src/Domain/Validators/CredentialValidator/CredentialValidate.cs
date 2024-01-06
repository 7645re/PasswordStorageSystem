namespace Domain.Validators.CredentialValidator;

public record CredentialValidate(
    string ResourceName,
    string ResourceLogin,
    string ResourcePassword);