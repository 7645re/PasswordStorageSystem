using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Models;
using Domain.Repositories;

namespace Domain.Services;

public class CredentialManagerService : ICredentialManagerService
{
    private readonly IResourceCredentialRepository _resourceCredentialRepository;
    private readonly IUserRepository _userRepository;

    public CredentialManagerService(IResourceCredentialRepository resourceCredentialRepository,
        IUserRepository userRepository)
    {
        _resourceCredentialRepository = resourceCredentialRepository;
        _userRepository = userRepository;
    }

    public async Task<OperationResult<IEnumerable<ResourceCredential>>> GetAllUserCredentialsAsync(string login)
    {
        var userExist = await _userRepository.UserExistByLoginAsync(login);
        if (!userExist) return new OperationResult<IEnumerable<ResourceCredential>>
        {
            IsSuccess = false,
            ErrorMessage = "User doesnt exist",
        };
        var result = await _resourceCredentialRepository.GetAllPasswordsByLoginAsync(login);
        return new OperationResult<IEnumerable<ResourceCredential>>
        {
            IsSuccess = true,
            Result = result
        };
    }

    public async Task<OperationResult<IEnumerable<ResourceCredential>>> GetCredentialsByAtDate(DateTimeOffset dateTimeOffset)
    {
        var result = await _resourceCredentialRepository.GetCredentialsByAtDate(dateTimeOffset);
        return new OperationResult<IEnumerable<ResourceCredential>>
        {
            IsSuccess = true,
            Result = result
        };
    }

    public async Task<OperationResult> CreateUserCredentialAsync(ResourceCredentialToCreate resourceCredentialToCreate)
    {
        var userExist = await _userRepository.UserExistByLoginAsync(resourceCredentialToCreate.UserLogin);
        if (!userExist)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "User doesnt exist"
            };

        var existsCredentialByResourceName = await _resourceCredentialRepository.GetByLoginAndResourceNameAsync(
            resourceCredentialToCreate.UserLogin,
            resourceCredentialToCreate.ResourceName);
        var concreteCredential = existsCredentialByResourceName.FirstOrDefault(c =>
            c.ResourceLogin == resourceCredentialToCreate.ResourceLogin
            && c.ResourcePassword ==
            resourceCredentialToCreate.ResourcePassword);

        if (concreteCredential != null)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "Credential already exist"
            };

        await _resourceCredentialRepository.CreateCredential(
            new ResourceCredential {
                Login = resourceCredentialToCreate.UserLogin,
                ResourceName = resourceCredentialToCreate.ResourceName,
                ResourceLogin = resourceCredentialToCreate.ResourceLogin,
                ResourcePassword = resourceCredentialToCreate.ResourcePassword,
                LastUpdate = DateTimeOffset.Now
                    }
            );
        return new OperationResult
        {
            IsSuccess = true
        };
    }

    public async Task<OperationResult> ChangeUserCredentialAsync(ResourceCredentialToChange resourceCredentialToChange)
    {
        if (resourceCredentialToChange.NewResourceLogin == null &&
            resourceCredentialToChange.NewResourcePassword == null)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "Both fields are not specified NewResourcePassword, NewResourceLogin"
            };

        var existsCredentialByResourceName = await _resourceCredentialRepository.GetByLoginAndResourceNameAsync(
            resourceCredentialToChange.UserLogin,
            resourceCredentialToChange.ResourceName);
        var concreteCredential = existsCredentialByResourceName.FirstOrDefault(c =>
            c.ResourceLogin == resourceCredentialToChange.ResourceLogin);

        if (concreteCredential == null)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "Credential doesnt exist"
            };
        if (concreteCredential.ResourcePassword == resourceCredentialToChange.NewResourcePassword)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "Credential already has such a password"
            };
        if (concreteCredential.ResourceLogin == resourceCredentialToChange.NewResourceLogin)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "Credential already have such ResourceLogin"
            };

        await _resourceCredentialRepository.ChangeCredential(new ResourceCredential
        {
            Login = resourceCredentialToChange.UserLogin,
            ResourceName = resourceCredentialToChange.ResourceName,
            ResourceLogin = resourceCredentialToChange.NewResourceLogin ?? resourceCredentialToChange.ResourceLogin,
            ResourcePassword = resourceCredentialToChange.NewResourcePassword ?? concreteCredential.ResourcePassword,
            LastUpdate = DateTimeOffset.Now,
        });
        
        return new OperationResult
        {
            IsSuccess = true
        };
    }

    public async Task<OperationResult<ResourceCredential?>> GenerateUserCredentialAsync(string login)
    {
        var randomCredential = new ResourceCredentialToCreate(
            login,
            $"{login}ResourceLogin{Guid.NewGuid()}",
            $"{login}ResourceLogin{Guid.NewGuid()}",
            $"{login}ResourcePassword{Guid.NewGuid()}"
        );
        var result = await CreateUserCredentialAsync(randomCredential);
        if (result.IsSuccess)
            return new OperationResult<ResourceCredential?>
            {
                IsSuccess = true,
                Result = new ResourceCredential
                {
                    Login = randomCredential.UserLogin,
                    ResourceName = randomCredential.ResourceName,
                    ResourceLogin = randomCredential.ResourceLogin,
                    ResourcePassword = randomCredential.ResourcePassword,
                    LastUpdate = DateTimeOffset.Now
                }
            };
        return new OperationResult<ResourceCredential?>
        {
            IsSuccess = false,
            ErrorMessage = result.ErrorMessage,
        };
    }

    public async Task<ResourceCredential[]> GetAll()
    {
        return await _resourceCredentialRepository.GetAllAsync();
    }
}