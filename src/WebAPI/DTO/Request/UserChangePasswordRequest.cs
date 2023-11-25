namespace WebAPI.DTO.Request;

public record UserChangePasswordRequest(string Login, string NewPassword);