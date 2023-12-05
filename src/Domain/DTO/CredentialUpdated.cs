using Domain.Enums;

namespace Domain.DTO;

public record CredentialUpdated(string ResourcePassword, DateTimeOffset ChangedAt, PasswordSecurityLevel PasswordSecurityLevel);