namespace Domain.DTO;

public class User
{
    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? Token { get; init; }
    public DateTimeOffset? TokenExpire { get; init; }
}