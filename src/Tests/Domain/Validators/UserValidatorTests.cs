using Domain.Validators.UserValidator;
using NUnit.Framework;

namespace Tests.Domain.Validators;

[TestFixture]
public class UserValidatorTests
{
    private IUserValidator _userValidator = null!;

    [SetUp]
    public void Setup()
    {
        _userValidator = new UserValidator();
    }

    [Test]
    public void ValidatePassword_PasswordLongerThan50Symbols_ThrowException()
    {
        // Arrange
        var password = string.Join("", Enumerable
            .Range(0, 51)
            .Select(_ => "1"));

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(
            () => _userValidator.ValidatePassword(password));
        Assert.AreEqual("The password is too weak", actualException?.Message);
    }
    
    [Test]
    public void ValidatePassword_PasswordShorterThan6Symbols_ThrowException()
    {
        // Arrange
        var password = "12";

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(
            () => _userValidator.ValidatePassword(password));
        Assert.AreEqual("The password is too weak", actualException?.Message);
    }

    [Test]
    public void ValidatePassword_Valid_NoThrowException()
    {
        // Arrange
        var password = "123456";

        // Act / Assert
        _userValidator.ValidatePassword(password);
    }
    
    [Test]
    public void ValidateLogin_PasswordLongerThan50Symbols_ThrowException()
    {
        // Arrange
        var login = string.Join("", Enumerable
            .Range(0, 51)
            .Select(_ => "1"));

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(
            () => _userValidator.ValidateLogin(login));
        Assert.AreEqual($"Login {login} length is not within the valid range", actualException?.Message);
    }
    
    [Test]
    public void ValidateLogin_PasswordShorterThan3Symbols_ThrowException()
    {
        // Arrange
        var login = "12";

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(
            () => _userValidator.ValidateLogin(login));
        Assert.AreEqual($"Login {login} length is not within the valid range", actualException?.Message);
    }
    
    [Test]
    public void ValidateLogin_Valid_NoThrowException()
    {
        // Arrange
        var login = "alexeev";

        // Act / Assert
        _userValidator.ValidateLogin(login);
    }
}