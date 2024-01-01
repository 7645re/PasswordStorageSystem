using Domain.Enums;

namespace Domain.DTO.Credential;

public record CredentialDelete(
    string UserLogin,
    string ResourceName,
    string ResourceLogin,
    PasswordSecurityLevel PasswordSecurityLevel,
    DateTimeOffset CreatedAt,
    Guid Id);