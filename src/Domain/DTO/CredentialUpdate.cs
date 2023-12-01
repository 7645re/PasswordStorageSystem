namespace Domain.DTO;

public record CredentialUpdate(
    string UserLogin,
    string ResourceName,
    string ResourceLogin,
    string NewResourcePassword);