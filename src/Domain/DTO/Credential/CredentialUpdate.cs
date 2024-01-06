namespace Domain.DTO.Credential;

public record CredentialUpdate(
    string UserLogin,
    DateTimeOffset CreatedAt,
    Guid Id,
    string NewPassword);