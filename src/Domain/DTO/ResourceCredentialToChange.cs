namespace Domain.DTO;

public record ResourceCredentialToChange(
    string UserLogin,
    string ResourceName,
    string ResourceLogin,
    string? NewResourceLogin,
    string? NewResourcePassword);
