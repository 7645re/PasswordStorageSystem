using Domain.Repositories.CredentialRepository;
using Domain.Services.CredentialService;
using Domain.Validators.CredentialValidator;
using Moq;
using NUnit.Framework;

namespace Tests.Domain.Services;

[TestFixture]
public class CredentialServiceTests
{
    private Mock<ICredentialRepository> _credentialRepositoryMock = null!;
    private Mock<ICredentialValidator> _credentialValidatorMock = null!;
    private ICredentialService _credentialService = null!;

    [SetUp]
    public void Setup()
    {
        _credentialRepositoryMock = new Mock<ICredentialRepository>();
        _credentialValidatorMock = new Mock<ICredentialValidator>();
        _credentialService = new CredentialService(
            _credentialRepositoryMock.Object,
            _credentialValidatorMock.Object);
    }

    [Test]
    public async Task GetCredentialsAsync_CredentialsExist_ReturnCredentials()
    {
        // Arrange
        const string userLogin = "login";
        const int pageSize = 10;
        const int pageNumber = 1;
        var expectedCredentialsEntities = new[]
        {
            new CredentialEntityBuilder().WithId(Guid.NewGuid()).Build(),
            new CredentialEntityBuilder().WithId(Guid.NewGuid()).Build(),
            new CredentialEntityBuilder().WithId(Guid.NewGuid()).Build()
        };

        _credentialRepositoryMock
            .Setup(r => r.GetCredentialsByLoginPagedAsync(
                userLogin,
                pageSize,
                pageNumber))
            .ReturnsAsync(expectedCredentialsEntities)
            .Verifiable(Times.Once);

        // Act
        var actualResult = await _credentialService.GetCredentialsAsync(
            userLogin,
            pageSize,
            pageNumber);

        // Assert
        _credentialRepositoryMock.Verify();
        _credentialValidatorMock.Verify();
        Assert.True(expectedCredentialsEntities
            .Select(c => c.Id)
            .SequenceEqual(actualResult
                .Select(a => a.Id)));
    }

    [Test]
    public async Task GetCredentialsCountAsync_CredentialsExist_ReturnCount()
    {
        // Arrange
        const string userLogin = "login";
        const long expectedActual = 10;
        _credentialRepositoryMock
            .Setup(r => r.GetCountOfCredentialsAsync(userLogin))
            .ReturnsAsync(expectedActual)
            .Verifiable(Times.Once);

        // Act
        var actualCount = await _credentialService.GetCredentialsCountAsync(userLogin);

        // Assert
        _credentialRepositoryMock.Verify();
        _credentialValidatorMock.Verify();
        Assert.AreEqual(expectedActual, actualCount);
    }
}