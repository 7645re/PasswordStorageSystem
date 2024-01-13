using Domain.Calculators;
using Domain.DTO.Credential;
using Domain.Mappers;
using Domain.Repositories.CredentialRepository;
using Domain.Validators.CredentialValidator;

namespace Domain.Services.CredentialService;

public class CredentialService : ICredentialService
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly ICredentialValidator _credentialValidator;

    public CredentialService(ICredentialRepository credentialRepository,
        ICredentialValidator credentialValidator)
    {
        _credentialRepository = credentialRepository;
        _credentialValidator = credentialValidator;
    }

    public async Task<Credential[]> GetCredentialsAsync(string userLogin, int pageSize,
        int pageNumber)
    {
        return (await _credentialRepository.GetCredentialsByLoginPagedAsync(
                userLogin,
                pageSize,
                pageNumber))
            .Select(ce => ce.ToCredential())
            .ToArray();
    }

    public async Task<long> GetCredentialsCountAsync(string userLogin)
    {
        return await _credentialRepository.GetCountOfCredentialsAsync(userLogin);
    }

    public async Task<Credential> CreateCredentialAsync(CredentialCreate credentialCreate)
    {
        _credentialValidator.ValidateCredentialCreate(credentialCreate);

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

        await _credentialRepository.CreateCredentialAsync(
            credential.ToCredentialEntity());

        return credential;
    }

    public async Task DeleteCredentialAsync(CredentialDelete credentialDelete)
    {
        await _credentialRepository.DeleteCredentialAsync(credentialDelete.ToCredentialEntity());
    }

    public async Task DeleteUserCredentialAsync(string userLogin)
    {
        await _credentialRepository.DeleteCredentialsAsync(userLogin);
    }

    public async Task<CredentialUpdated> ChangeCredentialPasswordAsync(
        CredentialUpdate credentialUpdate)
    {
        _credentialValidator.ValidateCredentialUpdate(credentialUpdate);
        var credentialEntity = credentialUpdate
            .ToCredentialEntity(
                DateTimeOffset.UtcNow,
                credentialUpdate
                    .NewPassword
                    .CalculateLevel());
        await _credentialRepository
            .UpdateCredentialAsync(credentialEntity);
        return credentialEntity.ToCredentialUpdated();
    }
}