using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI.DTO.Request;

public class CredentialCreateRequest
{
    [Required]
    [JsonPropertyName("resourceName")]
    public string ResourceName { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("resourceLogin")]
    public string ResourceLogin { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("resourcePassword")]
    public string ResourcePassword { get; set; } = string.Empty;
}