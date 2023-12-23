using Domain.Constants;
using Domain.DTO;
using Domain.Enums;
using Domain.Mappers;
using Domain.Models;
using Domain.Repositories.CredentialHistoryRepository;
using Domain.Repositories.CredentialRepository;
using Domain.Repositories.UserRepository;
using Domain.Services.PasswordLevelCalculatorService;
using Domain.Validators.CredentialValidator;
using Microsoft.Extensions.Logging;

namespace Domain.Services.CredentialService;

public class CredentialService : ICredentialService
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly ICredentialHistoryRepository _credentialHistoryRepository;
    private readonly ICredentialValidator _credentialValidator;
    private readonly IPasswordLevelCalculatorService _passwordLevelCalculatorService;
    private readonly ILogger<CredentialService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly Random _random = new();

    public CredentialService(ICredentialRepository credentialRepository, ICredentialValidator credentialValidator,
        IPasswordLevelCalculatorService passwordLevelCalculatorService, ILogger<CredentialService> logger,
        ICredentialHistoryRepository credentialHistoryRepository, IUserRepository userRepository)
    {
        _credentialRepository = credentialRepository;
        _credentialValidator = credentialValidator;
        _passwordLevelCalculatorService = passwordLevelCalculatorService;
        _logger = logger;
        _credentialHistoryRepository = credentialHistoryRepository;
        _userRepository = userRepository;
    }

    public async Task<long> GetCredentialsCountAsync(string userLogin)
    {
        return await _credentialRepository.GetCountAsync(userLogin);
    }

    public async Task<IDictionary<PasswordSecurityLevel, long>> GetPasswordsLevelsInfoAsync(
        string userLogin)
    {
        return await _credentialRepository.GetPasswordsLevelsInfoAsync(userLogin);
    }

    public async Task<IEnumerable<Credential>> GetCredentialsAsync(string userLogin)
    {
        var credentialsEntity = await _credentialRepository.GetCredentialsAsync(userLogin);
        return credentialsEntity.Select(ce => ce.ToCredential());
    }

    public async Task<IEnumerable<CredentialHistoryItem>> GetCredentialHistoryAsync(Guid credentialId)
    {
        var history = await _credentialHistoryRepository
            .GetHistoryByCredentialIdAsync(credentialId);
        return history.Select(hie => hie.ToCredentialHistoryItem());
    }

    public async Task DeleteCredentialAsync(CredentialDelete credentialDelete)
    {
        await _credentialRepository.DeleteCredentialAsync(credentialDelete.UserLogin, credentialDelete.ResourceName,
            credentialDelete.ResourceLogin);
    }

    public async Task<Credential> CreateCredentialAsync(CredentialCreate credentialCreate)
    {
        await _userRepository.CheckExistAsync(credentialCreate.UserLogin);
        _credentialValidator.Validate(credentialCreate);

        var passwordLevel =
            await _passwordLevelCalculatorService.CalculateLevelAsync(credentialCreate.UserLogin,
                credentialCreate.ResourcePassword);
        var newCredentialEntity =
            credentialCreate.ToCredentialEntity(passwordLevel, DateTimeOffset.UtcNow, Guid.NewGuid());
        await _credentialRepository.CreateCredentialAsync(newCredentialEntity);

        return newCredentialEntity.ToCredential();
    }

    public async Task<Credential[]> GenerateCredentialsAsync(string userLogin, int count)
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

        return newCredentials;
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

    public async Task<CredentialUpdated> UpdateCredentialAsync(CredentialUpdate credentialUpdate)
    {
        _credentialValidator.Validate(credentialUpdate);

        // TODO: Maybe you want to get rid of checking for the existence of a credential before modifying,
        // and just validate the result that will return the CQL itself in the driver
        var credential = await _credentialRepository.GetCredentialAsync(credentialUpdate.UserLogin,
            credentialUpdate.ResourceName, credentialUpdate.ResourceLogin);

        var passwordLevel = await _passwordLevelCalculatorService.CalculateLevelAsync(credentialUpdate.UserLogin,
            credentialUpdate.NewResourcePassword);

        // TODO: maybe changeAt time should be calculated on repository level
        var updateTime = DateTimeOffset.UtcNow;
        var updatedCredentialEntity = credentialUpdate.ToCredentialEntity(
            passwordLevel, credential.CreatedAt, updateTime);
        await _credentialRepository.UpdateCredentialAsync(updatedCredentialEntity);

        return updatedCredentialEntity.ToCredentialUpdated(updateTime);
    }
}