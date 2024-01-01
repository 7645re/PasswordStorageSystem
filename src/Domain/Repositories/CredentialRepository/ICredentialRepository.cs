using Domain.Models;

namespace Domain.Repositories.CredentialRepository;

public interface ICredentialRepository
{
    Task<CredentialEntity[]> GetCredentialsByLoginPagedAsync(string login, int pageSize, int pageNumber);
    Task CreateCredentialAsync(CredentialEntity credentialEntity);
    Task DeleteCredentialAsync(CredentialEntity credentialEntity);
    Task DeleteCredentialsAsync(string userLogin);
}