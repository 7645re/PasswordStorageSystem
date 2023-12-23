namespace Domain.DTO.User;

public class User
{
    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public DateTimeOffset TokenExpire { get; init; }
}