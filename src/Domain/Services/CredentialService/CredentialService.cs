using Domain.DTO.Credential;
using Domain.Mappers;
using Domain.Repositories.CredentialRepository;
using Domain.Services.PasswordLevelCalculatorService;

namespace Domain.Services.CredentialService;

public class CredentialService : ICredentialService
{
    private readonly ICredentialRepository _credentialRepository;

    public CredentialService(ICredentialRepository credentialRepository)
    {
        _credentialRepository = credentialRepository;
    }

    public async Task<Credential[]> GetCredentialsAsync(string userLogin, int pageSize, int pageNumber)
    {
        return (await _credentialRepository.GetCredentialsByLoginPagedAsync(userLogin, pageSize, pageNumber))
            .Select(ce => ce.ToCredential())
            .ToArray();
    }

    public async Task<Credential> CreateCredentialAsync(CredentialCreate credentialCreate)
    {
        var credential = new Credential
        {
            UserLogin = credentialCreate.UserLogin,
            ResourceName = credentialCreate.ResourceName,
            ResourceLogin = credentialCreate.ResourceLogin,
            ResourcePassword = credentialCreate.ResourcePassword,
            CreatedAt = DateTimeOffset.UtcNow,
            ChangedAt = null,
            PasswordSecurityLevel = credentialCreate.ResourcePassword.CalculateLevel(),
            Id = Guid.NewGuid()
        };

        await _credentialRepository.CreateCredentialAsync(credential.ToCredentialEntity());

        return credential;
    }

    public async Task DeleteCredentialAsync(CredentialDelete credentialDelete)
    {
        var credential = new Credential
        {
            UserLogin = credentialDelete.UserLogin,
            ResourceName = credentialDelete.ResourceName,
            ResourceLogin = credentialDelete.ResourceLogin,
            CreatedAt = credentialDelete.CreatedAt,
            PasswordSecurityLevel = credentialDelete.PasswordSecurityLevel,
            Id = credentialDelete.Id
        };

        await _credentialRepository.DeleteCredentialAsync(credential.ToCredentialEntity());
    }

    public async Task DeleteUserCredentialAsync(string userLogin)
    {
        await _credentialRepository.DeleteCredentialsAsync(userLogin);
    }
}