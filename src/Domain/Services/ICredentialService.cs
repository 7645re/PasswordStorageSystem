using Domain.DTO;
using Domain.Enums;

namespace Domain.Services;

public interface ICredentialService
{
    Task<OperationResult<IEnumerable<Credential>>> GetCredentialsAsync(string userLogin);
    Task<OperationResult> DeleteCredentialAsync(CredentialDelete credentialDelete);
    Task<OperationResult<Credential>> CreateCredentialAsync(CredentialCreate credentialCreate);
    Task<OperationResult<Credential[]>> GenerateCredentialsAsync(string userLogin, int count);
    Task<OperationResult<CredentialUpdated>> UpdateCredentialAsync(CredentialUpdate credentialUpdate);
    Task<OperationResult<IDictionary<PasswordSecurityLevel, long>>> GetPasswordsLevelsInfoAsync(string userLogin);
    Task<OperationResult<long>> GetCredentialsCountAsync(string userLogin);
    Task<OperationResult<IEnumerable<CredentialHistoryItem>>> GetCredentialHistoryAsync(Guid credentialId);
}