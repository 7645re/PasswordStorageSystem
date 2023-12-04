using Domain.Enums;
using Domain.Models;

namespace Domain.Repositories;

public interface ICredentialRepository
{
    Task<IEnumerable<CredentialEntity>> GetCredentialsAsync(string userLogin);
    Task<IEnumerable<CredentialEntity>> GetCredentialsAsync(string userLogin, string resourceName);
    Task<CredentialEntity?> GetCredentialAsync(string userLogin, string resourceName, string resourceLogin);
    Task CreateCredentialAsync(CredentialEntity credentialEntity);
    Task DeleteCredentialAsync(string userLogin, string resourceName, string resourceLogin);
    Task UpdateCredentialAsync(CredentialEntity newCredentialEntity);
    Task DeleteAllCredentialsAsync(string userLogin);
    Task DeleteResourceCredentialsAsync(string userLogin, string resourceName);
    Task<IEnumerable<string>> FindPasswordDuplicatesAsync(string password);
    Task<Dictionary<PasswordSecurityLevel, long>> GetPasswordsLevelsInfoAsync(string userLogin);
    Task<long> GetCountAsync(string userLogin);
}