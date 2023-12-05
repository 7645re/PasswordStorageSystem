using Domain.Enums;

namespace Domain.DTO;

public class Credential
{
    public string ResourceName { get; init; } = string.Empty;
    public string ResourceLogin { get; init; } = string.Empty;
    public string ResourcePassword { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ChangedAt { get; init; }
    public PasswordSecurityLevel PasswordSecurityLevel { get; init; }
    public Guid Id { get; set; }
}