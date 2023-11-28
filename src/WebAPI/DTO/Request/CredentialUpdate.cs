namespace WebAPI.DTO.Request;

public record UpdateCredential(string ResourceName, string ResourceLogin, string NewResourcePassword);