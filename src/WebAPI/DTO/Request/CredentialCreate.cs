namespace WebAPI.DTO.Request;

public record CredentialCreate(string ResourceName, string ResourceLogin, string ResourcePassword);