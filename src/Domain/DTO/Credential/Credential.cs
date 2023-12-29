using Domain.Enums;

namespace Domain.DTO.Credential;

public class Credential
{
    public string UserLogin { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceLogin { get; set; } = string.Empty;
    public string ResourcePassword { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ChangedAt { get; set; }
    public PasswordSecurityLevel PasswordSecurityLevel { get; set; }
    public Guid Id { get; set; }
}