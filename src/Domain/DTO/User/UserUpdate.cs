namespace Domain.DTO.User;

public record UserUpdate(
    string Login,
    string NewPassword);