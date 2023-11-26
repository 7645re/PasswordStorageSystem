namespace Domain.DTO;

public record ResourceCredentialToCreate(
    string UserLogin,
    string ResourceName,
    string ResourceLogin,
    string ResourcePassword);