namespace WebAPI.DTO;

public record ResourceCredentialCreateRequest(
    string Login,
    string ResourceName,
    string ResourceLogin,
    string ResourcePassword);
