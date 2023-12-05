namespace Domain.DTO;

public record CredentialDelete(
    string UserLogin,
    string ResourceName,
    string ResourceLogin,
    Guid CredentialId); 