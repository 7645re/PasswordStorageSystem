namespace Domain.DTO.Credential;

public record CredentialUpdate(
    string UserLogin,
    string ResourceName,
    string ResourceLogin,
    string NewResourcePassword);