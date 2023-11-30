namespace Domain.DTO;

public class User
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string? Token { get; set; }
    public DateTimeOffset? TokenExpire { get; set; }
}