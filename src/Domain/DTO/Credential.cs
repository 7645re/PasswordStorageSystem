using Domain.Enums;

namespace Domain.DTO;

public class Credential
{
    public string ResourceName { get; init; } = string.Empty;
    public string ResourceLogin { get; init; } = string.Empty;
    public string ResourcePassword { get; init; } = string.Empty;
    public DateTimeOffset CreateAt { get; init; }
    public DateTimeOffset? ChangeAt { get; init; }
    public IEnumerable<CredentialHistoryItem>? History { get; set; }
    public PasswordSecurityLevel PasswordSecurityLevel { get; init; }
}