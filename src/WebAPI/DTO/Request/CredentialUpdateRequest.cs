namespace WebAPI.DTO.Request;

public record CredentialUpdateRequest(string ResourceName, string ResourceLogin, string NewResourcePassword);