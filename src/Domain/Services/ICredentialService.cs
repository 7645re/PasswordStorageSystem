using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Enums;
using Domain.Models;

namespace Domain.Services;

public interface ICredentialService
{
    Task<OperationResult<IEnumerable<Credential>>> GetCredentialsAsync(string userLogin);
    Task<OperationResult> DeleteCredentialAsync(string userLogin, string resourceName, string resourceLogin);
    Task<OperationResult> DeleteResourceCredentialsAsync(string userLogin, string resourceName);
    Task<OperationResult> DeleteAllCredentialsAsync(string userLogin);
    Task<OperationResult<CredentialEntity>> CreateCredentialAsync(CredentialCreate credentialCreate);
    Task<OperationResult<List<CredentialEntity>>> GenerateCredentialsAsync(string userLogin, int count);
    Task<OperationResult<CredentialEntity>> UpdateCredentialAsync(CredentialUpdate credentialUpdate);
    Task<OperationResult<IDictionary<PasswordSecurityLevel, long>>> GetPasswordsLevelsInfoAsync(string userLogin);
    Task<OperationResult<long>> GetCredentialsCountAsync(string userLogin);
}