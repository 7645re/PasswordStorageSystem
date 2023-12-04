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
        ICredentialHistoryRepository credentialHistoryRepository,
        IPasswordLevelCalculatorService passwordLevelCalculatorService,
        ILogger<CredentialService> logger, IUserService userService)
    {
        _credentialRepository = credentialRepository;
        _credentialValidator = credentialValidator;
        _credentialHistoryRepository = credentialHistoryRepository;
        _passwordLevelCalculatorService = passwordLevelCalculatorService;
        _logger = logger;
        _userService = userService;
    }

    public async Task<OperationResult<long>> GetCredentialsCountAsync(string userLogin)
    {
        var result = await _credentialRepository.GetCountAsync(userLogin);
        return new OperationResult<long>
        {
            IsSuccess = true,
            Result = result
        };
    }

    public async Task<OperationResult<IDictionary<PasswordSecurityLevel, long>>> GetPasswordsLevelsInfoAsync(
        string userLogin)
    {
        var result = await _credentialRepository.GetPasswordsLevelsInfoAsync(userLogin);
        return new OperationResult<IDictionary<PasswordSecurityLevel, long>>
        {
            IsSuccess = true,
            Result = result
        };
    }

    public async Task<OperationResult<IEnumerable<Credential>>> GetCredentialsAsync(string userLogin)
    {
        var user = await _userService.GetUserAsync(userLogin);
        if (!user.IsSuccess)
            return new OperationResult<IEnumerable<Credential>>
            {
                IsSuccess = false,
                ErrorMessage = user.ErrorMessage
            };
        var credentialsEntity = await _credentialRepository.GetCredentialsAsync(userLogin);
        var credentialsHistoriesItemsEntities = await _credentialHistoryRepository.GetHistoryItemByUserAsync(userLogin);

        var credentialDictionary = credentialsEntity.ToDictionary(
            credential => (credential.ResourceName, credential.ResourceLogin),
            credential => credential.ToCredential()
        );

        var credentialHistoryItemDictionary = credentialsHistoriesItemsEntities
            .GroupBy(
                historyItemEntity => (historyItemEntity.ResourceName, historyItemEntity.ResourceLogin),
                historyItemEntity => historyItemEntity.ToCredentialHistoryItem()
            )
            .ToDictionary(
                groupedItem => groupedItem.Key,
                groupedItem => groupedItem.ToList()
            );

        var joinedCredentials = credentialDictionary
            .GroupJoin(
                credentialHistoryItemDictionary,
                credential => credential.Key,
                history => history.Key,
                (credential, history) =>
                {
                    credential.Value.History = history.SelectMany(h => h.Value).ToList();
                    return credential.Value;
                }
            )
            .ToArray();

        return new OperationResult<IEnumerable<Credential>>
        {
            IsSuccess = true,
            Result = joinedCredentials
        };
    }

    public async Task<OperationResult> DeleteCredentialAsync(CredentialDelete credentialDelete)
    {
        var user = await _userService.GetUserAsync(credentialDelete.UserLogin);
        if (!user.IsSuccess)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = user.ErrorMessage
            };

        var credential = await _credentialRepository.GetCredentialAsync(
            credentialDelete.UserLogin,
            credentialDelete.ResourceName,
            credentialDelete.ResourceLogin);
        if (credential == null)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "Credential doesnt exist"
            };

        await _credentialRepository.DeleteCredentialAsync(
            credentialDelete.UserLogin,
            credentialDelete.ResourceName,
            credentialDelete.ResourceLogin);
        return new OperationResult
        {
            IsSuccess = true
        };
    }

    public async Task<OperationResult> DeleteResourceCredentialsAsync(string userLogin, string resourceName)
    {
        var user = await _userService.GetUserAsync(userLogin);
        if (!user.IsSuccess)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = user.ErrorMessage
            };

        var credential = await _credentialRepository.GetCredentialsAsync(userLogin, resourceName);
        if (!credential.Any())
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = $"User {userLogin} have not credentials for resource {resourceName}"
            };

        await _credentialRepository.DeleteResourceCredentialsAsync(userLogin, resourceName);
        return new OperationResult
        {
            IsSuccess = true
        };
    }

    public async Task<OperationResult> DeleteAllCredentialsAsync(string userLogin)
    {
        var user = await _userService.GetUserAsync(userLogin);
        if (!user.IsSuccess)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = user.ErrorMessage
            };

        var credential = await _credentialRepository.GetCredentialsAsync(userLogin);
        if (!credential.Any())
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = $"User {userLogin} have not credentials"
            };

        await _credentialRepository.DeleteAllCredentialsAsync(userLogin);
        return new OperationResult
        {
            IsSuccess = true
        };
    }

    public async Task<OperationResult<Credential>> CreateCredentialAsync(CredentialCreate credentialCreate)
    {
        var user = await _userService.GetUserAsync(credentialCreate.UserLogin);
        if (!user.IsSuccess)
            return new OperationResult<Credential>
            {
                IsSuccess = false,
                ErrorMessage = user.ErrorMessage
            };

        var validateResult = _credentialValidator.Validate(credentialCreate);
        if (!validateResult.IsSuccess)
            return new OperationResult<Credential>
            {
                IsSuccess = false,
                ErrorMessage = validateResult.ErrorMessage
            };

        var credential = await _credentialRepository.GetCredentialAsync(
            credentialCreate.UserLogin,
            credentialCreate.ResourceName,
            credentialCreate.ResourceLogin);
        if (credential != null)
            return new OperationResult<Credential>
            {
                IsSuccess = false,
                ErrorMessage = "Credential already exist"
            };

        var passwordLevel =
            await _passwordLevelCalculatorService.CalculateLevelAsync(credentialCreate.UserLogin,
                credentialCreate.ResourcePassword);
        var newCredentialEntity = credentialCreate.ToCredentialEntity(passwordLevel, DateTimeOffset.UtcNow);
        await _credentialRepository.CreateCredentialAsync(newCredentialEntity);

        return new OperationResult<Credential>
        {
            IsSuccess = true,
            Result = newCredentialEntity.ToCredential()
        };
    }

    public async Task<OperationResult<Credential[]>> GenerateCredentialsAsync(string userLogin, int count)
    {
        var user = await _userService.GetUserAsync(userLogin);
        if (!user.IsSuccess)
            return new OperationResult<Credential[]>
            {
                IsSuccess = false,
                ErrorMessage = user.ErrorMessage
            };

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

        return new OperationResult<Credential[]>
        {
            IsSuccess = true,
            Result = newCredentials
        };
    }

    private async Task<CredentialEntity> GenerateCredentialAsync(string userLogin)
    {
        var createdAt = DateTimeOffset
            .Now
            .AddTicks(-_random.NextInt64(0, 77760000000001));
        var password = new string(Enumerable
            .Repeat(Resources.AllSymbols, _random.Next(8, 15))
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
            CreatedAt = createdAt
        };

        var credential = await _credentialRepository.GetCredentialAsync(
            newCredential.UserLogin,
            newCredential.ResourceName,
            newCredential.ResourceLogin);
        if (credential != null) throw new ArgumentException("Failed to generate unique credentials");

        await _credentialRepository.CreateCredentialAsync(newCredential);
        return newCredential;
    }

    public async Task<OperationResult<Credential>> UpdateCredentialAsync(CredentialUpdate credentialUpdate)
    {
        var user = await _userService.GetUserAsync(credentialUpdate.UserLogin);
        if (!user.IsSuccess)
            return new OperationResult<Credential>
            {
                IsSuccess = false,
                ErrorMessage = user.ErrorMessage
            };

        var validateResult = _credentialValidator.Validate(credentialUpdate);
        if (!validateResult.IsSuccess)
            return new OperationResult<Credential>
            {
                IsSuccess = false,
                ErrorMessage = validateResult.ErrorMessage
            };

        var credential = await _credentialRepository.GetCredentialAsync(
            credentialUpdate.UserLogin,
            credentialUpdate.ResourceName,
            credentialUpdate.ResourceLogin);
        if (credential == null)
            return new OperationResult<Credential>
            {
                IsSuccess = false,
                ErrorMessage = "Credential doesnt exist"
            };

        if (credential.ResourcePassword == credentialUpdate.NewResourcePassword)
            return new OperationResult<Credential>
            {
                IsSuccess = false,
                ErrorMessage = "Credential already have this password",
            };

        var passwordLevel =
            await _passwordLevelCalculatorService.CalculateLevelAsync(credentialUpdate.UserLogin,
                credentialUpdate.NewResourcePassword);

        // TODO: maybe changeAt time should be calculated on repository level
        var updatedCredentialEntity = credentialUpdate.ToCredentialEntity(
            passwordLevel,
            credential.CreatedAt,
            DateTimeOffset.UtcNow);
        await _credentialRepository.UpdateCredentialAsync(updatedCredentialEntity);

        return new OperationResult<Credential>
        {
            IsSuccess = true,
            Result = updatedCredentialEntity.ToCredential()
        };
    }
}