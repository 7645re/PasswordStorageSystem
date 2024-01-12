using System.Net;
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
        var desResponse = await registerUserResponse.DeserializeToAsync<TokenInfoResponse>();
        Assert.NotNull(desResponse);
        var getUserResponse = await GetUser(desResponse.Token);
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
        var desRegisterUserResponse = await registerUserResponse.DeserializeToAsync<TokenInfoResponse>();
        var getUserResponse = await GetUser(desRegisterUserResponse!.Token);
        var desGetUserResponse = await getUserResponse.DeserializeToAsync<UserResponse>();

        // Assert
        Assert.Equivalent(HttpStatusCode.OK, getUserResponse.StatusCode);
        Assert.Equal(body.Login, desGetUserResponse.Login);
        Assert.Equal(body.Password, desGetUserResponse.Password);
        Assert.Equal(desRegisterUserResponse.Token, desGetUserResponse.Token);
        Assert.Equal(desRegisterUserResponse.Expire, desGetUserResponse.TokenExpire);
    }

    [Fact]
    public async Task LogIn_UserExist_ReturnValidToken()
    {
        // Arrange
        var body = new
        {
            Login = "login3",
            Password = "password"
        };

        // Act
        var registerUserResponse = await RegisterUser(body);
        var desRegisterUserResponse = await registerUserResponse.DeserializeToAsync<TokenInfoResponse>();
        var logInUserResponse = await LogInUser(body);
        var desLogInUserResponse = await logInUserResponse.DeserializeToAsync<TokenInfoResponse>();

        // Assert
        Assert.Equivalent(HttpStatusCode.OK, logInUserResponse.StatusCode);
        Assert.Equal(desRegisterUserResponse!.Token, desLogInUserResponse!.Token);
        Assert.Equal(desRegisterUserResponse.Expire, desLogInUserResponse.Expire);
    }

    [Fact]
    public async Task ChangePassword_UserExist_GerUserWithNewPassword()
    {
        // Arrange
        var registerUserBody = new
        {
            Login = "login4",
            Password = "password"
        };
        var changePasswordBody = new
        {
            NewPassword = "NewPassword"
        };


        // Act
        var registerUserResponse = await RegisterUser(registerUserBody);
        var desRegisterUserResponse = await registerUserResponse.DeserializeToAsync<TokenInfoResponse>();
        await ChangeUserPassword(changePasswordBody, desRegisterUserResponse.Token);

        // Assert
        var getUserResponse = await GetUser(desRegisterUserResponse.Token);
        var desGetUserResponse = await getUserResponse.DeserializeToAsync<UserResponse>();
        Assert.Equal(changePasswordBody.NewPassword, desGetUserResponse.Password);
    }
}