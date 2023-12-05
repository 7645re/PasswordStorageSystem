namespace Domain.DTO;

public record CredentialHistoryItem(
    Guid CredentialId,
    string ResourcePassword,
    DateTimeOffset ChangedAt);