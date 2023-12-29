using Domain.Models;

namespace Domain.Repositories.CredentialRepository;

public interface ICredentialRepository
{
    Task<CredentialEntity[]> GetAllCredentialsByLoginAsync(string login);
    Task CreateCredentialAsync(CredentialEntity credentialEntity);
    Task DeleteCredentialAsync(CredentialEntity credentialEntity);
}