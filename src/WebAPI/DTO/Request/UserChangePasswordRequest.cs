using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI.DTO.Request;

public class UserChangePasswordRequest
{
    [Required]
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}