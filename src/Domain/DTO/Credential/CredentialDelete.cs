namespace Domain.DTO.Credential;

public record CredentialDelete(
    string UserLogin,
    string ResourceName,
    string ResourceLogin); 