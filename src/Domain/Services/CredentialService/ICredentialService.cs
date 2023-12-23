using Domain.DTO;
using Domain.Enums;

namespace Domain.Services.CredentialService;

public interface ICredentialService
{
    Task<long> GetCredentialsCountAsync(string userLogin);

    Task<IDictionary<PasswordSecurityLevel, long>> GetPasswordsLevelsInfoAsync(
        string userLogin);

    Task<IEnumerable<Credential>> GetCredentialsAsync(string userLogin);
    Task<IEnumerable<CredentialHistoryItem>> GetCredentialHistoryAsync(Guid credentialId);
    Task DeleteCredentialAsync(CredentialDelete credentialDelete);
    Task<Credential> CreateCredentialAsync(CredentialCreate credentialCreate);
    Task<Credential[]> GenerateCredentialsAsync(string userLogin, int count);
    Task<CredentialUpdated> UpdateCredentialAsync(CredentialUpdate credentialUpdate);
}