using System.Text.Json.Serialization;

namespace WebAPI.DTO.Response;

public class UserResponse
{
    [JsonPropertyName("login")]
    public string Login { get; init; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; init; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token { get; init; } = string.Empty;
    
    [JsonPropertyName("tokenExpire")]
    public DateTimeOffset TokenExpire { get; init; }
}