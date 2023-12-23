namespace Domain.DTO.User;

public record UserCreate(
    string Login,
    string Password);