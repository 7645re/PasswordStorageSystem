using System.Text.Json.Serialization;

namespace WebAPI.DTO.Response;

public class TokenInfoResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expire")]
    public DateTimeOffset Expire { get; set; }
}