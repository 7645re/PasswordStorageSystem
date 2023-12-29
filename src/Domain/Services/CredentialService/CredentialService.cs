using Domain.DTO.Credential;
using Domain.Enums;
using Domain.Mappers;
using Domain.Repositories.CredentialRepository;

namespace Domain.Services.CredentialService;

public class CredentialService : ICredentialService
{
    private readonly ICredentialRepository _credentialRepository;

    public CredentialService(ICredentialRepository credentialRepository)
    {
        _credentialRepository = credentialRepository;
    }

    public async Task<Credential> CreateCredentialAsync(Credential credentialCreate)
    {
        var credential = new Credential
        {
            ResourceName = credentialCreate.ResourceName,
            ResourceLogin = credentialCreate.ResourceLogin,
            ResourcePassword = credentialCreate.ResourcePassword,
            CreatedAt = DateTimeOffset.UtcNow,
            ChangedAt = null,
            PasswordSecurityLevel = PasswordSecurityLevel.Secure,
            Id = Guid.NewGuid()
        };

        await _credentialRepository.CreateCredentialAsync(credential.ToCredentialEntity());

        return credential;
    }
}