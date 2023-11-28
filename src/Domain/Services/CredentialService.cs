using Domain.Constants;
using Domain.DTO;
using Domain.Models;
using Domain.Repositories;
using Domain.Validators;

namespace Domain.Services;

public class CredentialService : ICredentialService
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly ICredentialHistoryRepository _credentialHistoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICredentialValidator _credentialValidator;
    private readonly Random _random = new();

    public CredentialService(ICredentialRepository credentialRepository,
        IUserRepository userRepository, ICredentialValidator credentialValidator,
        ICredentialHistoryRepository credentialHistoryRepository)
    {
        _credentialRepository = credentialRepository;
        _userRepository = userRepository;
        _credentialValidator = credentialValidator;
        _credentialHistoryRepository = credentialHistoryRepository;
    }

    public async Task<OperationResult<IEnumerable<Credential>>> GetCredentialsAsync(string userLogin)
    {
        var user = await _userRepository.GetUserAsync(userLogin);
        if (user == null)
            return new OperationResult<IEnumerable<Credential>>
            {
                IsSuccess = false,
                ErrorMessage = "User doesnt exist"
            };
        var credentialsEntity = await _credentialRepository.GetCredentialsAsync(userLogin);
        var credentialsHistoriesItemsEntities = await _credentialHistoryRepository.GetHistoryItemByUserAsync(userLogin);
        
        var credentialDictionary = credentialsEntity.ToDictionary(
            credential => (credential.ResourceName, credential.ResourceLogin),
            credential => new Credential
            {
                ResourceName = credential.ResourceName,
                ResourceLogin = credential.ResourceLogin,
                ResourcePassword = credential.ResourcePassword,
                CreateAt = credential.CreatedAt,
                ChangeAt = credential.ChangeAt.ToLocalTime(),
                History = Array.Empty<CredentialHistoryItem>()
            }
        );

        var credentialHistoryItemDictionary = credentialsHistoriesItemsEntities
            .GroupBy(
                historyItem => (historyItem.ResourceName, historyItem.ResourceLogin),
                historyItem => new CredentialHistoryItem(
                    historyItem.ResourceName,
                    historyItem.ResourceLogin,
                    historyItem.ResourcePassword,
                    historyItem.ChangeAt.ToLocalTime()
                )
            )
            .ToDictionary(
                groupedItem => groupedItem.Key,
                groupedItem => groupedItem.ToList() as IEnumerable<CredentialHistoryItem>
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

    public async Task<OperationResult> DeleteCredentialAsync(string userLogin, string resourceName,
        string resourceLogin)
    {
        var user = await _userRepository.GetUserAsync(userLogin);
        if (user == null)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = $"User {userLogin} doesnt exist"
            };

        var credential = await _credentialRepository.GetCredentialAsync(userLogin, resourceName, resourceLogin);
        if (credential == null)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "Credential doesnt exist"
            };

        await _credentialRepository.DeleteCredentialAsync(userLogin, resourceName, resourceLogin);
        return new OperationResult
        {
            IsSuccess = true
        };
    }

    public async Task<OperationResult> DeleteResourceCredentialsAsync(string userLogin, string resourceName)
    {
        var user = await _userRepository.GetUserAsync(userLogin);
        if (user == null)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = $"User {userLogin} doesnt exist"
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
        var user = await _userRepository.GetUserAsync(userLogin);
        if (user == null)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = $"User {userLogin} doesnt exist"
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

    public async Task<OperationResult<CredentialEntity>> CreateCredentialAsync(CredentialCreate credentialCreate)
    {
        var user = await _userRepository.GetUserAsync(credentialCreate.UserLogin);
        if (user == null)
            return new OperationResult<CredentialEntity>
            {
                IsSuccess = false,
                ErrorMessage = $"User {credentialCreate.UserLogin} doesnt exist"
            };

        var validateResult = _credentialValidator.Validate(credentialCreate);
        if (!validateResult.IsSuccess)
            return new OperationResult<CredentialEntity>
            {
                IsSuccess = false,
                ErrorMessage = validateResult.ErrorMessage
            };

        var credential = await _credentialRepository.GetCredentialAsync(credentialCreate.UserLogin,
            credentialCreate.ResourceName, credentialCreate.ResourceLogin);
        if (credential != null)
            return new OperationResult<CredentialEntity>
            {
                IsSuccess = false,
                ErrorMessage = "Credential already exist"
            };

        var newCredential = new CredentialEntity
        {
            UserLogin = credentialCreate.UserLogin,
            ResourceName = credentialCreate.ResourceName,
            ResourceLogin = credentialCreate.ResourceLogin,
            ResourcePassword = credentialCreate.ResourcePassword,
            CreatedAt = DateTimeOffset.Now,
            ChangeAt = DateTimeOffset.Now
        };
        await _credentialRepository.CreateCredentialAsync(newCredential);

        return new OperationResult<CredentialEntity>
        {
            IsSuccess = true,
            Result = newCredential
        };
    }

    public async Task<OperationResult<List<CredentialEntity>>> GenerateCredentialsAsync(string userLogin, int count)
    {
        var user = await _userRepository.GetUserAsync(userLogin);
        if (user == null)
            return new OperationResult<List<CredentialEntity>>
            {
                IsSuccess = false,
                ErrorMessage = $"User {userLogin} doesnt exist"
            };

        var newCredentials = new List<CredentialEntity>();
        for (var i = 0; i < count; i++)
        {
            var credential = await GenerateCredentialAsync(userLogin);
            newCredentials.Add(credential);
        }

        return new OperationResult<List<CredentialEntity>>
        {
            IsSuccess = true,
            Result = newCredentials
        };
    }

    private async Task<CredentialEntity> GenerateCredentialAsync(string userLogin)
    {
        var createdAt = DateTimeOffset.Now.AddTicks(-_random.NextInt64(0, 77760000000001));
        var newCredential = new CredentialEntity
        {
            UserLogin = userLogin,
            ResourceName = Resources.Names[_random.Next(0, Resources.Names.Length)],
            ResourceLogin = Resources.Logins[_random.Next(0, Resources.Logins.Length)],
            ResourcePassword = Guid.NewGuid().ToString(),
            CreatedAt = createdAt,
            ChangeAt = createdAt
        };

        var credential = await _credentialRepository.GetCredentialAsync(newCredential.UserLogin,
            newCredential.ResourceName, newCredential.ResourceLogin);
        if (credential != null) throw new ArgumentException("Failed to generate unique credentials");

        await _credentialRepository.CreateCredentialAsync(newCredential);

        return newCredential;
    }

    public async Task<OperationResult<CredentialEntity>> UpdateCredentialAsync(CredentialUpdate credentialUpdate)
    {
        var user = await _userRepository.GetUserAsync(credentialUpdate.UserLogin);
        if (user == null)
            return new OperationResult<CredentialEntity>
            {
                IsSuccess = false,
                ErrorMessage = $"User {credentialUpdate.UserLogin} doesnt exist"
            };

        var validateResult = _credentialValidator.Validate(credentialUpdate);
        if (!validateResult.IsSuccess)
            return new OperationResult<CredentialEntity>
            {
                IsSuccess = false,
                ErrorMessage = validateResult.ErrorMessage
            };

        var credential = await _credentialRepository.GetCredentialAsync(credentialUpdate.UserLogin,
            credentialUpdate.ResourceName, credentialUpdate.ResourceLogin);
        if (credential == null)
            return new OperationResult<CredentialEntity>
            {
                IsSuccess = false,
                ErrorMessage = "Credential doesnt exist"
            };

        if (credential.ResourcePassword == credentialUpdate.NewResourcePassword)
            return new OperationResult<CredentialEntity>
            {
                IsSuccess = false,
                ErrorMessage = "Credential already have this password",
            };
        
        var newCredential = new CredentialEntity
        {
            UserLogin = credentialUpdate.UserLogin,
            ResourceName = credentialUpdate.ResourceName,
            ResourceLogin = credentialUpdate.ResourceLogin,
            ResourcePassword = credentialUpdate.NewResourcePassword,
            CreatedAt = credential.CreatedAt,
            ChangeAt = DateTimeOffset.Now
        };
        await _credentialRepository.UpdateCredentialAsync(newCredential);

        return new OperationResult<CredentialEntity>
        {
            IsSuccess = true,
            Result = newCredential
        };
    }
}