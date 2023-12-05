using Domain.Constants;
using Domain.DTO;
using Domain.Enums;
using Domain.Mappers;
using Domain.Models;
using Domain.Repositories;
using Domain.Validators;
using Microsoft.Extensions.Logging;

namespace Domain.Services;

public class CredentialService : ICredentialService
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly ICredentialHistoryRepository _credentialHistoryRepository;
    private readonly ICredentialValidator _credentialValidator;
    private readonly IPasswordLevelCalculatorService _passwordLevelCalculatorService;
    private readonly ILogger<CredentialService> _logger;
    private readonly IUserService _userService;
    private readonly Random _random = new();

    public CredentialService(ICredentialRepository credentialRepository, ICredentialValidator credentialValidator,
        IPasswordLevelCalculatorService passwordLevelCalculatorService, ILogger<CredentialService> logger,
        IUserService userService, ICredentialHistoryRepository credentialHistoryRepository)
    {
        _credentialRepository = credentialRepository;
        _credentialValidator = credentialValidator;
        _passwordLevelCalculatorService = passwordLevelCalculatorService;
        _logger = logger;
        _userService = userService;
        _credentialHistoryRepository = credentialHistoryRepository;
    }

    public async Task<OperationResult<long>> GetCredentialsCountAsync(string userLogin)
    {
        var result = await _credentialRepository.GetCountAsync(userLogin);
        return new OperationResult<long> { IsSuccess = true, Result = result };
    }

    public async Task<OperationResult<IDictionary<PasswordSecurityLevel, long>>> GetPasswordsLevelsInfoAsync(
        string userLogin)
    {
        var result = await _credentialRepository.GetPasswordsLevelsInfoAsync(userLogin);
        return new OperationResult<IDictionary<PasswordSecurityLevel, long>> { IsSuccess = true, Result = result };
    }

    public async Task<OperationResult<IEnumerable<Credential>>> GetCredentialsAsync(string userLogin)
    {
        var user = await _userService.GetUserAsync(userLogin);
        if (!user.IsSuccess)
            return new OperationResult<IEnumerable<Credential>> { IsSuccess = false, ErrorMessage = user.ErrorMessage };
        var credentialsEntity = await _credentialRepository.GetCredentialsAsync(userLogin);
        var credentials = credentialsEntity.Select(ce => ce.ToCredential());

        return new OperationResult<IEnumerable<Credential>> { IsSuccess = true, Result = credentials };
    }

    public async Task<OperationResult<IEnumerable<CredentialHistoryItem>>> GetCredentialHistoryAsync(Guid credentialId)
    {
        var history = await _credentialHistoryRepository
            .GetHistoryByCredentialIdAsync(credentialId);
        return new OperationResult<IEnumerable<CredentialHistoryItem>>
        {
            IsSuccess = true,
            Result = history.Select(hie => hie.ToCredentialHistoryItem())
        };
    }

    public async Task<OperationResult> DeleteCredentialAsync(CredentialDelete credentialDelete)
    {
        var user = await _userService.GetUserAsync(credentialDelete.UserLogin);
        if (!user.IsSuccess) return new OperationResult { IsSuccess = false, ErrorMessage = user.ErrorMessage };

        var credential = await _credentialRepository.GetCredentialAsync(credentialDelete.UserLogin,
            credentialDelete.ResourceName, credentialDelete.ResourceLogin);
        if (credential == null)
            return new OperationResult { IsSuccess = false, ErrorMessage = "Credential doesnt exist" };

        await _credentialRepository.DeleteCredentialAsync(credentialDelete.UserLogin, credentialDelete.ResourceName,
            credentialDelete.ResourceLogin, credentialDelete.CredentialId);
        return new OperationResult { IsSuccess = true };
    }

    public async Task<OperationResult<Credential>> CreateCredentialAsync(CredentialCreate credentialCreate)
    {
        var user = await _userService.GetUserAsync(credentialCreate.UserLogin);
        if (!user.IsSuccess)
            return new OperationResult<Credential> { IsSuccess = false, ErrorMessage = user.ErrorMessage };

        var validateResult = _credentialValidator.Validate(credentialCreate);
        if (!validateResult.IsSuccess)
            return new OperationResult<Credential> { IsSuccess = false, ErrorMessage = validateResult.ErrorMessage };

        var credential = await _credentialRepository.GetCredentialAsync(credentialCreate.UserLogin,
            credentialCreate.ResourceName, credentialCreate.ResourceLogin);
        if (credential != null)
            return new OperationResult<Credential> { IsSuccess = false, ErrorMessage = "Credential already exist" };

        var passwordLevel =
            await _passwordLevelCalculatorService.CalculateLevelAsync(credentialCreate.UserLogin,
                credentialCreate.ResourcePassword);
        var newCredentialEntity =
            credentialCreate.ToCredentialEntity(passwordLevel, DateTimeOffset.UtcNow, Guid.NewGuid());
        await _credentialRepository.CreateCredentialAsync(newCredentialEntity);

        return new OperationResult<Credential> { IsSuccess = true, Result = newCredentialEntity.ToCredential() };
    }

    public async Task<OperationResult<Credential[]>> GenerateCredentialsAsync(string userLogin, int count)
    {
        var newCredentials = new Credential[count];
        for (var i = 0; i < count; i++)
        {
            try
            {
                var credential = await GenerateCredentialAsync(userLogin);
                newCredentials[i] = credential.ToCredential();
            }
            catch (ArgumentException e)
            {
                _logger.LogInformation(e.Message);
            }
        }

        return new OperationResult<Credential[]> { IsSuccess = true, Result = newCredentials };
    }

    private async Task<CredentialEntity> GenerateCredentialAsync(string userLogin)
    {
        var createdAt = DateTimeOffset.Now.AddTicks(-_random.NextInt64(0, 77760000000001));
        var password = new string(Enumerable.Repeat(Resources.AllSymbols, _random.Next(8, 15))
            .Select(s => s[_random.Next(s.Length)])
            .ToArray());
        var passwordLevel = await _passwordLevelCalculatorService.CalculateLevelAsync(userLogin, password);
        var newCredential = new CredentialEntity
        {
            UserLogin = userLogin,
            ResourceName = Resources.Names[_random.Next(0, Resources.Names.Length)],
            ResourceLogin = Resources.Logins[_random.Next(0, Resources.Logins.Length)],
            ResourcePassword = password,
            PasswordSecurityLevel = passwordLevel,
            Id = Guid.NewGuid(),
            CreatedAt = createdAt
        };

        var credential = await _credentialRepository.GetCredentialAsync(newCredential.UserLogin,
            newCredential.ResourceName, newCredential.ResourceLogin);
        if (credential != null) throw new ArgumentException("Failed to generate unique credentials");

        await _credentialRepository.CreateCredentialAsync(newCredential);
        return newCredential;
    }

    public async Task<OperationResult<CredentialUpdated>> UpdateCredentialAsync(CredentialUpdate credentialUpdate)
    {
        var validateResult = _credentialValidator.Validate(credentialUpdate);
        if (!validateResult.IsSuccess)
            return new OperationResult<CredentialUpdated> { IsSuccess = false, ErrorMessage = validateResult.ErrorMessage };

        // TODO: Maybe you want to get rid of checking for the existence of a credential before modifying,
        // and just validate the result that will return the CQL itself in the driver
        var credential = await _credentialRepository.GetCredentialAsync(credentialUpdate.UserLogin,
            credentialUpdate.ResourceName, credentialUpdate.ResourceLogin);
        if (credential == null)
            return new OperationResult<CredentialUpdated> { IsSuccess = false, ErrorMessage = "Credential doesnt exist" };

        if (credential.ResourcePassword == credentialUpdate.NewResourcePassword)
            return new OperationResult<CredentialUpdated>
            {
                IsSuccess = false, ErrorMessage = "Credential already have this password",
            };

        var passwordLevel = await _passwordLevelCalculatorService.CalculateLevelAsync(credentialUpdate.UserLogin,
            credentialUpdate.NewResourcePassword);

        // TODO: maybe changeAt time should be calculated on repository level
        var updateTime = DateTimeOffset.UtcNow;
        var updatedCredentialEntity = credentialUpdate.ToCredentialEntity(
            passwordLevel, credential.CreatedAt, updateTime);
        await _credentialRepository.UpdateCredentialAsync(updatedCredentialEntity);

        return new OperationResult<CredentialUpdated> { IsSuccess = true, Result = updatedCredentialEntity.ToCredentialUpdated(updateTime) };
    }
}