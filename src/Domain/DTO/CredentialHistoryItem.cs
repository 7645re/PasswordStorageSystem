namespace Domain.DTO;


public record CredentialHistoryItem(
    string ResourceName,
    string ResourceLogin,
    string ResourcePassword,
    DateTimeOffset ChangeAt);