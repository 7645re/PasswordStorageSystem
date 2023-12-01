using Domain.DTO;
using Domain.Enums;

namespace Domain.Services;

public interface ICredentialService
{
    Task<OperationResult<IEnumerable<Credential>>> GetCredentialsAsync(string userLogin);
    Task<OperationResult> DeleteCredentialAsync(CredentialDelete credentialDelete);
    Task<OperationResult> DeleteResourceCredentialsAsync(string userLogin, string resourceName);
    Task<OperationResult> DeleteAllCredentialsAsync(string userLogin);
    Task<OperationResult<Credential>> CreateCredentialAsync(CredentialCreate credentialCreate);
    Task<OperationResult<Credential[]>> GenerateCredentialsAsync(string userLogin, int count);
    Task<OperationResult<Credential>> UpdateCredentialAsync(CredentialUpdate credentialUpdate);
    Task<OperationResult<IDictionary<PasswordSecurityLevel, long>>> GetPasswordsLevelsInfoAsync(string userLogin);
    Task<OperationResult<long>> GetCredentialsCountAsync(string userLogin);
}