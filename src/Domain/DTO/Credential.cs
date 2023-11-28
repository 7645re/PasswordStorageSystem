namespace Domain.DTO;

public class Credential
{
    public string ResourceName { get; init; }
    public string ResourceLogin { get; init; }
    public string ResourcePassword { get; init; }
    public DateTimeOffset CreateAt { get; init; }
    public DateTimeOffset ChangeAt { get; init; }
    public IEnumerable<CredentialHistoryItem>? History { get; set; }
}