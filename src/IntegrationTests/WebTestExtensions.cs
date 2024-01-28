namespace IntegrationTests;

public static class WebTestExtensions
{
    public static HttpClient WithBearerToken(this HttpClient httpClient, string token)
    {
        httpClient
            .DefaultRequestHeaders
            .Add("Authorization", "Bearer " + token);
        return httpClient;
    }
}