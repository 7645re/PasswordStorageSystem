namespace Domain.DTO;

public record CredentialCreate(
    string UserLogin,
    string ResourceName,
    string ResourceLogin,
    string ResourcePassword);