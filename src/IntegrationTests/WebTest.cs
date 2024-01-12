using System.Text;
using System.Text.Json;
using Xunit;

namespace IntegrationTests;

public abstract class WebTest : IClassFixture<IntegrationTestWebApplicationFactory<Program>>
{
    private readonly IntegrationTestWebApplicationFactory<Program> _factory;

    protected WebTest(IntegrationTestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    protected Task<HttpResponseMessage> RegisterUser(object request)
    {
        return _factory
            .CreateClient()
            .PostAsync($"/user/register", Serialize(request));
    }
    
    protected Task<HttpResponseMessage> GetUser(string token)
    {
        var client = _factory
            .CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        return client
            .GetAsync("/user");
    }

    protected Task<HttpResponseMessage> LogInUser(object request, string token)
    {
        var client = _factory
            .CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", token);
        return client
            .PostAsync($"/user/login", Serialize(request));
    }
    
    private StringContent Serialize(object source)
    {
        var json = JsonSerializer.Serialize(source);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}