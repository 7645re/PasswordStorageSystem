namespace WebAPI.DTO.Request;

public record CredentialUpdateRequest(
    DateTimeOffset CreatedAt,
    Guid Id,
    string NewPassword);