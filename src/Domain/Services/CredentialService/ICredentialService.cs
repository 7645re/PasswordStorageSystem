using Domain.DTO.Credential;

namespace Domain.Services.CredentialService;

public interface ICredentialService
{
    Task<Credential> CreateCredentialAsync(CredentialCreate credentialCreate);
    Task DeleteCredentialAsync(CredentialDelete credentialDelete);
    Task DeleteUserCredentialAsync(string userLogin);
    Task<Credential[]> GetCredentialsAsync(string userLogin, int pageSize, int pageNumber);
    Task<CredentialUpdated> ChangeCredentialPasswordAsync(CredentialUpdate credentialUpdate);
}