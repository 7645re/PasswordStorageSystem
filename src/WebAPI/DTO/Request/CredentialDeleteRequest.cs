namespace WebAPI.DTO.Request;

public record CredentialDeleteRequest(string ResourceName, string ResourceLogin, Guid CredentialId);