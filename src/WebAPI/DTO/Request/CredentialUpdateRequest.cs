using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI.DTO.Request;

public class CredentialUpdateRequest
{
    [Required]
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
    
    [Required]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [Required]
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}