using Domain.Enums;

namespace WebAPI.DTO.Request;

public record CredentialDeleteRequest(
    string ResourceName,
    string ResourceLogin,
    PasswordSecurityLevel PasswordSecurityLevel,
    DateTimeOffset CreatedAt,
    Guid Id);