using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Models;

namespace Domain.Services;

public interface ICredentialManagerService
{
    public Task<OperationResult<IEnumerable<ResourceCredential>>> GetAllUserCredentialsAsync(string login);

    public Task<OperationResult> CreateUserCredentialAsync(ResourceCredentialToCreate resourceCredentialToCreate);

    public Task<OperationResult<ResourceCredential?>> GenerateUserCredentialAsync(string login);

    public Task<OperationResult> ChangeUserCredentialAsync(ResourceCredentialToChange resourceCredentialToChange);

    public Task<OperationResult<IEnumerable<ResourceCredential>>> GetCredentialsByAtDate(
        DateTimeOffset dateTimeOffset);

    public Task<ResourceCredential[]> GetAll();
}