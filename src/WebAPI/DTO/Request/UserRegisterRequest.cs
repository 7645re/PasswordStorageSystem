using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI.DTO.Request;

public class UserRegisterRequest
{
    [Required]
    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;

    
    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}