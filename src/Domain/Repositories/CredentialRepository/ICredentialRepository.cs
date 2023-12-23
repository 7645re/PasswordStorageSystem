using Domain.Enums;
using Domain.Models;

namespace Domain.Repositories.CredentialRepository;

public interface ICredentialRepository
{
    Task<IEnumerable<CredentialEntity>> GetCredentialsAsync(string userLogin);
    Task<CredentialEntity> GetCredentialAsync(string userLogin, string resourceName, string resourceLogin);
    Task CreateCredentialAsync(CredentialEntity credentialEntity);

    Task DeleteCredentialAsync(
        string userLogin,
        string resourceName,
        string resourceLogin);

    Task<IEnumerable<string>> FindUsersWithSamePasswordAsync(string password);
    Task UpdateCredentialAsync(CredentialEntity newCredentialEntity);
    Task<long> GetCountAsync(string userLogin);
    Task<Dictionary<PasswordSecurityLevel, long>> GetPasswordsLevelsInfoAsync(string userLogin);
}