using Domain.DTO;
using Domain.DTO.User;
using Domain.Models;
using Domain.Repositories.UserRepository;
using Domain.Services.TokenService;
using Domain.Services.UserService;
using Domain.Validators.UserValidator;
using Moq;
using NUnit.Framework;

namespace Tests.Domain.Services;

[TestFixture]
public class UserServiceTests
{
    private IUserService _userService = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IUserValidator> _userValidatorMock = null!;
    private Mock<ITokenService> _tokenServiceMock = null!;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new();
        _userValidatorMock = new();
        _tokenServiceMock = new();
        _userService = new UserService(
            _userRepositoryMock.Object,
            _userValidatorMock.Object,
            _tokenServiceMock.Object);
    }

    [Test]
    public async Task CreateUserAsync_Valid_ReturnGeneratedToken()
    {
        // Arrange
        var tokenInfo = new TokenInfo(Guid.NewGuid().ToString(), DateTimeOffset.Now);
        var userCreate = new UserCreate("login", "password");
        _userValidatorMock
            .Setup(v => v.Validate(userCreate))
            .Verifiable(Times.Once);
        _tokenServiceMock
            .Setup(s => s.GenerateAccessToken(userCreate.Login))
            .Returns(tokenInfo)
            .Verifiable(Times.Once);
        _userRepositoryMock
            .Setup(r => r.CreateUserAsync(It.Is<UserEntity>(
                ue => ue.Login == userCreate.Login
                      && ue.Password == userCreate.Password
                      && ue.AccessToken == tokenInfo.Token
                      && ue.AccessTokenExpire == tokenInfo.Expire)))
            .Verifiable(Times.Once);

        // Act
        var resultTokenInfo = await _userService.CreateUserAsync(userCreate);

        // Assert
        _userValidatorMock.Verify();
        _tokenServiceMock.Verify();
        _userRepositoryMock.Verify();
        Assert.AreEqual(tokenInfo.Token, resultTokenInfo.Token);
    }

    [Test]
    public async Task DeleteUserAsync_Valid_NoThrowException()
    {
        // Arrange
        var userLogin = "login";
        _userRepositoryMock
            .Setup(r => r.DeleteUserAsync(userLogin))
            .Verifiable(Times.Once);

        // Act / Assert
        await _userService.DeleteUserAsync(userLogin);
        _userValidatorMock.Verify();
        _tokenServiceMock.Verify();
        _userRepositoryMock.Verify();
    }

    [Test]
    public async Task GetUserAsync_Valid_NoThrowException()
    {
        // Arrange
        var userLogin = "userLogin";
        var userEntity = new UserEntity
        {
            Login = userLogin,
            Password = "password",
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now
        };
        _userRepositoryMock
            .Setup(r => r.GetUserAsync(userLogin))
            .ReturnsAsync(userEntity)
            .Verifiable(Times.Once);

        // Act
        var userResult = await _userService.GetUserAsync(userLogin);

        // Assert
        _userValidatorMock.Verify();
        _tokenServiceMock.Verify();
        _userRepositoryMock.Verify();
        Assert.AreEqual(userEntity.Login, userResult.Login);
        Assert.AreEqual(userEntity.Password, userResult.Password);
    }

    [Test]
    public async Task GetUserTokenAsync_UserExist_ReturnTokenInfo()
    {
        // Arrange
        var userLogIn = new UserLogIn("login", "pass");
        var userEntity = new UserEntity
        {
            Login = userLogIn.Login,
            Password = userLogIn.Password,
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(1))
        };
        _userRepositoryMock
            .Setup(r => r.GetUserAsync(userLogIn.Login))
            .ReturnsAsync(userEntity)
            .Verifiable(Times.Once);

        // Act
        var tokenInfo = await _userService.GetUserTokenAsync(userLogIn);

        // Assert
        _userValidatorMock.Verify();
        _tokenServiceMock.Verify();
        _userRepositoryMock.Verify();
        Assert.AreEqual(userEntity.AccessToken, tokenInfo.Token);
        Assert.AreEqual(userEntity.AccessTokenExpire, tokenInfo.Expire);
    }

    [Test]
    public async Task GetUserTokenAsync_TokenTimeExpired_ReturnNewTokenInfo()
    {
        // Arrange
        var userLogIn = new UserLogIn("login", "pass");
        var userEntity = new UserEntity
        {
            Login = userLogIn.Login,
            Password = userLogIn.Password,
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(1) * -1)
        };
        var expectedTokenInfo = new TokenInfo(
            Guid.NewGuid().ToString(),
            DateTimeOffset.Now.Add(TimeSpan.FromMinutes(10)));
        _userRepositoryMock
            .Setup(r => r.GetUserAsync(userLogIn.Login))
            .ReturnsAsync(userEntity)
            .Verifiable(Times.Once);
        _tokenServiceMock
            .Setup(s => s.GenerateAccessToken(userLogIn.Login))
            .Returns(expectedTokenInfo)
            .Verifiable(Times.Once);
        _userRepositoryMock
            .Setup(r => r.ChangeAccessTokenAsync(userLogIn.Login, expectedTokenInfo))
            .Verifiable(Times.Once);

        // Act
        var actualTokenInfo = await _userService.GetUserTokenAsync(userLogIn);

        // Assert
        _userValidatorMock.Verify();
        _tokenServiceMock.Verify();
        _userRepositoryMock.Verify();
        Assert.AreEqual(expectedTokenInfo, actualTokenInfo);
    }

    [Test]
    public void GetUserTokenAsync_InvalidPassword_ThrowException()
    {
        // Arrange
        var userLogIn = new UserLogIn("login", "pass");
        var userEntity = new UserEntity
        {
            Login = userLogIn.Login,
            Password = string.Empty,
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(1))
        };
        _userRepositoryMock
            .Setup(r => r.GetUserAsync(userLogIn.Login))
            .ReturnsAsync(userEntity)
            .Verifiable(Times.Once);

        // Act / Assert
        var actualException = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _userService.GetUserTokenAsync(userLogIn));

        _userValidatorMock.Verify();
        _tokenServiceMock.Verify();
        _userRepositoryMock.Verify();
        Assert.AreEqual("Invalid password", actualException?.Message);
    }

    [Test]
    public async Task ChangePasswordAsync_ValidPassword_NoThrowException()
    {
        // Arrange
        var userUpdate = new UserUpdate("login", "passw");
        _userValidatorMock
            .Setup(v => v.ValidatePassword(userUpdate.NewPassword))
            .Verifiable(Times.Once);
        _userRepositoryMock
            .Setup(r => r.ChangePasswordAsync(userUpdate.Login, userUpdate.NewPassword))
            .Verifiable(Times.Once);
        
        // Act
        await _userService.ChangePasswordAsync(userUpdate);

        // Assert
        _userValidatorMock.Verify();
        _tokenServiceMock.Verify();
        _userRepositoryMock.Verify();
    }
}