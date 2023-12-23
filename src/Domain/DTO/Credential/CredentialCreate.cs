namespace Domain.DTO.Credential;

public record CredentialCreate(
    string UserLogin,
    string ResourceName,
    string ResourceLogin,
    string ResourcePassword);