namespace WebAPI.DTO.Request;

public record ResourceCredentialChangeRequest(
    string UserLogin,
    string ResourceName,
    string ResourceLogin,
    string? NewResourceLogin,
    string? NewResourcePassword);
