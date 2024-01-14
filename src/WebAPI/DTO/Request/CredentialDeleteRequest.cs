using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace WebAPI.DTO.Request;

public class CredentialDeleteRequest
{
    [Required]
    [JsonPropertyName("resourceName")]
    public string ResourceName { get; set; }
    
    [Required]
    [JsonPropertyName("resourceLogin")]
    public string ResourceLogin { get; set; }
    
    [Required]
    [JsonPropertyName("passwordSecurityLevel")]
    public PasswordSecurityLevel PasswordSecurityLevel { get; set; }
    
    [Required]
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
    
    [Required]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
}