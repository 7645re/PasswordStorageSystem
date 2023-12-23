using Domain.Enums;

namespace Domain.DTO.Credential;

public record CredentialUpdated(
    string ResourcePassword,
    DateTimeOffset ChangedAt,
    PasswordSecurityLevel PasswordSecurityLevel);