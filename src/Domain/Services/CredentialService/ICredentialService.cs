using Domain.DTO.Credential;

namespace Domain.Services.CredentialService;

public interface ICredentialService
{
    Task<Credential> CreateCredentialAsync(Credential credentialCreate);
}