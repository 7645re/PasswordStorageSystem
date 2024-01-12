using System.Net;
using System.Text.Json;
using WebAPI.DTO.Response;
using Xunit;

namespace IntegrationTests.Controllers;

public class UserControllerTests : WebTest
{
    public UserControllerTests(IntegrationTestWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task RegisterUser_UserDoesntExist_OkResultReturned()
    {
        // Arrange
        var body = new
        {
            Login = "login",
            Password = "password"
        };

        // Act
        var registerUserResponse = await RegisterUser(body);

        // Assert
        var registerUserResponseString = await registerUserResponse.Content.ReadAsStringAsync();
        var deserializedRegisterUserResponseString =
            JsonSerializer.Deserialize<TokenInfoResponse>(registerUserResponseString);
        Assert.NotNull(deserializedRegisterUserResponseString);
        var getUserResponse = await GetUser(deserializedRegisterUserResponseString.Token);
        Assert.Equivalent(HttpStatusCode.OK, getUserResponse.StatusCode);
    }
    
    [Fact]
    public async Task GetUser_UserExist_ReturnExistUser()
    {
        // Arrange
        var body = new
        {
            Login = "login1",
            Password = "password"
        };

        // Act
        var registerUserResponse = await RegisterUser(body);
        var registerUserResponseString = await registerUserResponse
            .Content
            .ReadAsStringAsync();
        var deserializedRegisterUserResponseString =
            JsonSerializer.Deserialize<TokenInfoResponse>(registerUserResponseString);
        var getUserResponse = await GetUser(deserializedRegisterUserResponseString!.Token);
        var getUserResponseString = await getUserResponse.Content.ReadAsStringAsync();
        var deserializedGetUserResponseString = JsonSerializer.Deserialize<UserResponse>(getUserResponseString);

        // Assert
        Assert.Equivalent(HttpStatusCode.OK, getUserResponse.StatusCode);
        Assert.Equal(body.Login, deserializedGetUserResponseString.Login);
        Assert.Equal(body.Password, deserializedGetUserResponseString.Password);
        Assert.Equal(deserializedRegisterUserResponseString.Token, deserializedGetUserResponseString.Token);
        Assert.Equal(deserializedRegisterUserResponseString.Expire, deserializedGetUserResponseString.TokenExpire);
    }
}