using System.Text.Json;

namespace IntegrationTests;

public static class ControllerTestExtensions
{
    public static async Task<T?> DeserializeToAsync<T>(this HttpResponseMessage httpResponseMessage)
    {
        var responseString = await httpResponseMessage
            .Content
            .ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(responseString);
    }
}