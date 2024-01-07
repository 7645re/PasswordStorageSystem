using Domain.DTO.Credential;
using Domain.Validators.CredentialValidator;
using NUnit.Framework;

namespace Tests.Domain.Validators;

public class CredentialValidatorTests
{
    private ICredentialValidator _credentialValidator;

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
            "12345678910123456789101234567891012345678910123456789101",
            string.Empty,
            string.Empty);

        // Act / Assert
        var actualException = Assert.Throws<ArgumentException>(() =>
            _credentialValidator.ValidateCredentialCreate(credential));
        Assert.AreEqual(
            "Credential resource password length is not within the valid range",
            actualException.Message);
    }
}