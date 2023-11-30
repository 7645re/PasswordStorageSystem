namespace WebAPI.DTO.Request;

public record CredentialCreateRequest(string ResourceName, string ResourceLogin, string ResourcePassword);