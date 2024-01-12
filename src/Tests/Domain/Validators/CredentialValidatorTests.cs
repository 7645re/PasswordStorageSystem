using Domain.DTO.Credential;
using Domain.Validators.CredentialValidator;
using NUnit.Framework;

namespace Tests.Domain.Validators;

[TestFixture]
public class CredentialValidatorTests
{
    private ICredentialValidator _credentialValidator = null!;

    [SetUp]
    public void Setup()
    {
        _credentialValidator = new CredentialValidator();
    }

    [Test]
    public void ValidateCredentialCreate_ResourceNameLengthMoreThan50_ThrowException()
    {
        // Arrange
        var credential = new CredentialCreate(
            string.Empty,
            new string('1', 51),
            string.Empty,
            string.Empty);

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(() =>
            _credentialValidator.ValidateCredentialCreate(credential));
        Assert.AreEqual(
            "Credential resource password length is not within the valid range",
            actualException.Message);
    }
    
    [Test]
    public void ValidateCredentialCreate_ResourceNameLengthLessThan2_ThrowException()
    {
        // Arrange
        var credential = new CredentialCreate(
            string.Empty,
            "1",
            string.Empty,
            string.Empty);

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(() =>
            _credentialValidator.ValidateCredentialCreate(credential));
        Assert.AreEqual(
            "Credential resource password length is not within the valid range",
            actualException.Message);
    }

    [Test]
    public void ValidateCredentialCreate_ResourceLoginLengthMoreThan50_ThrowException()
    {
        // Arrange
        var credential = new CredentialCreate(
            string.Empty,
            "123123132",
            new string('1', 51),
            string.Empty);

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(() =>
            _credentialValidator.ValidateCredentialCreate(credential));
        Assert.AreEqual(
            "Credential resource login length is not within the valid range",
            actualException.Message);
    }
    
    [Test]
    public void ValidateCredentialCreate_ResourceLoginLengthLessThan2_ThrowException()
    {
        // Arrange
        var credential = new CredentialCreate(
            string.Empty,
            "11231313",
            "1",
            string.Empty);

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(() =>
            _credentialValidator.ValidateCredentialCreate(credential));
        Assert.AreEqual(
            "Credential resource login length is not within the valid range",
            actualException.Message);
    }
    
    [Test]
    public void ValidateCredentialCreate_ResourcePasswordLengthMoreThan100_ThrowException()
    {
        // Arrange
        var credential = new CredentialCreate(
            string.Empty,
            "123123132",
            "131313123",
            new string('1', 101));

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(() =>
            _credentialValidator.ValidateCredentialCreate(credential));
        Assert.AreEqual(
            "Credential resource password length is not within the valid range",
            actualException.Message);
    }
    
    [Test]
    public void ValidateCredentialCreate_ResourcePasswordLengthLessThan2_ThrowException()
    {
        // Arrange
        var credential = new CredentialCreate(
            string.Empty,
            "11231313",
            "121313",
            "1");

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(() =>
            _credentialValidator.ValidateCredentialCreate(credential));
        Assert.AreEqual(
            "Credential resource password length is not within the valid range",
            actualException.Message);
    }
    
    [Test]
    public void ValidateCredentialUpdate_ResourcePasswordLengthMoreThan100_ThrowException()
    {
        // Arrange
        var credential = new CredentialUpdate(
            string.Empty,
            DateTimeOffset.Now,
            Guid.Empty,
            new string('1', 101));

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(() =>
            _credentialValidator.ValidateCredentialUpdate(credential));
        Assert.AreEqual(
            "Credential resource password length is not within the valid range",
            actualException.Message);
    }
    
    [Test]
    public void ValidateCredentialUpdate_ResourcePasswordLengthLessThan2_ThrowException()
    {
        // Arrange
        var credential = new CredentialUpdate(
            string.Empty,
            DateTimeOffset.Now,
            Guid.Empty,
            "1");

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(() =>
            _credentialValidator.ValidateCredentialUpdate(credential));
        Assert.AreEqual(
            "Credential resource password length is not within the valid range",
            actualException.Message);
    }
    
    [Test]
    public void ValidateCredentialCreate_Valid_NoThrowException()
    {
        // Arrange
        var credential = new CredentialCreate(
            "123123",
            "12",
            "12313132",
            "12313123");

        // Act
        _credentialValidator.ValidateCredentialCreate(credential);
    }
    
    [Test]
    public void ValidateCredentialUpdate_Valid_NoThrowException()
    {
        // Arrange
        var credential = new CredentialUpdate(
            string.Empty,
            DateTimeOffset.Now,
            Guid.Empty,
            "12345");
        
        // Act
        _credentialValidator.ValidateCredentialUpdate(credential);
    }
}