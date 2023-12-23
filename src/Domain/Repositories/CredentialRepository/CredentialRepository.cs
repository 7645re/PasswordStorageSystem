using Cassandra.Data.Linq;
using Domain.Enums;
using Domain.Models;
using Domain.Options;
using Domain.Repositories.CredentialByPasswordRepository;
using Domain.Repositories.CredentialBySecurityLevelRepository;
using Domain.Repositories.CredentialHistoryRepository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories.CredentialRepository;

public class CredentialRepository : CassandraRepositoryBase<CredentialEntity>, ICredentialRepository
{
    private readonly ICredentialHistoryRepository _credentialHistoryRepository;
    private readonly ICredentialByPasswordRepository _credentialByPasswordRepository;
    private readonly ICredentialBySecurityLevelRepository _credentialBySecurityLevelRepository;

    public CredentialRepository(IOptions<CassandraOptions> cassandraOptions, ILogger<CredentialRepository> logger,
        ICredentialHistoryRepository credentialHistoryRepository,
        ICredentialByPasswordRepository credentialByPasswordRepository,
        ICredentialBySecurityLevelRepository credentialBySecurityLevelRepository) : base(cassandraOptions, logger)
    {
        _credentialHistoryRepository = credentialHistoryRepository;
        _credentialByPasswordRepository = credentialByPasswordRepository;
        _credentialBySecurityLevelRepository = credentialBySecurityLevelRepository;
    }

    public async Task<IEnumerable<CredentialEntity>> GetCredentialsAsync(string userLogin)
    {
        return await ExecuteQueryAsync(Table.Where(r => r.UserLogin == userLogin));
    }

    public async Task<CredentialEntity> GetCredentialAsync(string userLogin, string resourceName, string resourceLogin)
    {
        var credential = await TryGetCredentialAsync(userLogin, resourceName, resourceLogin);
        if (credential == null)
            throw new Exception($"Credential with (user_login, resource_name, resource_login) doesnt exist");
        return credential;
    }

    public async Task<CredentialEntity?> TryGetCredentialAsync(string userLogin, string resourceName,
        string resourceLogin)
    {
        var credentials = (await ExecuteQueryAsync(Table.Where(r =>
            r.UserLogin == userLogin
            && r.ResourceName == resourceName
            && r.ResourceLogin == resourceLogin))).ToArray();
        if (credentials.Length > 1)
            throw new Exception("Key (user_login, resource_name, resource_login) in Credentials doesnt unique");
        return credentials.FirstOrDefault();
    }

    public async Task CreateCredentialAsync(CredentialEntity credentialEntity)
    {
        var existedCredential = await TryGetCredentialAsync(credentialEntity.UserLogin, credentialEntity.ResourceName,
            credentialEntity.ResourceLogin);
        if (existedCredential != null)
            throw new Exception(
                $"Credential with key ({credentialEntity.UserLogin} {credentialEntity.ResourceName}" +
                $" {credentialEntity.ResourceLogin}) already exist");
        var createCredentialByPasswordQuery =
            _credentialByPasswordRepository.CreateCredentialByPasswordQuery(credentialEntity);
        var createCredentialBySecurityLevelQuery =
            _credentialBySecurityLevelRepository.CreateCredentialBySecurityLevelQuery(credentialEntity);
        var createCredential = AddQuery(credentialEntity);
        var batch = new[]
        {
            createCredential,
            createCredentialByPasswordQuery,
            createCredentialBySecurityLevelQuery
        };
        await ExecuteAsBatchAsync(batch);
    }

    public async Task DeleteCredentialAsync(
        string userLogin,
        string resourceName,
        string resourceLogin)
    {
        var credentialToDelete = await GetCredentialAsync(userLogin, resourceName, resourceLogin);
        var deleteUserQuery = Table.Where(r =>
                r.UserLogin == userLogin
                && r.ResourceName == resourceName
                && r.ResourceLogin == resourceLogin)
            .Delete();
        var deleteCredentialHistoryItemsQuery = _credentialHistoryRepository
            .DeleteHistoryByCredentialIdQuery(credentialToDelete.Id);
        var deleteCredentialByPasswordQuery = _credentialByPasswordRepository
            .DeleteCredentialByPasswordQuery(credentialToDelete);
        var deleteCredentialBySecurityLevelQuery = _credentialBySecurityLevelRepository
            .DeleteCredentialBySecurityLevelQuery(credentialToDelete);
        var batchQueries = new[]
        {
            deleteUserQuery,
            deleteCredentialHistoryItemsQuery,
            deleteCredentialByPasswordQuery,
            deleteCredentialBySecurityLevelQuery
        };

        await ExecuteAsBatchAsync(batchQueries);
    }

    public async Task<IEnumerable<string>> FindUsersWithSamePasswordAsync(string password)
    {
        return (await _credentialByPasswordRepository.GetCredentialsByPasswordAsync(password)).Select(c =>
            c.ResourceLogin);
    }

    public async Task UpdateCredentialAsync(CredentialEntity newCredentialEntity)
    {
        var oldCredentialEntity = await GetCredentialAsync(newCredentialEntity.UserLogin, newCredentialEntity.ResourceName,
            newCredentialEntity.ResourceLogin);
        if (oldCredentialEntity.ResourcePassword == newCredentialEntity.ResourcePassword)
            throw new Exception("Credential state hasn't changed in any way, so it can't be updated");
        
        var batch = new List<CqlCommand>
        {
            Table
                .Where(r => r.UserLogin == newCredentialEntity.UserLogin &&
                            r.ResourceName == newCredentialEntity.ResourceName &&
                            r.ResourceLogin == newCredentialEntity.ResourceLogin)
                .Select(r => new CredentialEntity
                {
                    ResourcePassword = newCredentialEntity.ResourcePassword,
                    ChangedAt = newCredentialEntity.ChangedAt,
                    PasswordSecurityLevel = newCredentialEntity.PasswordSecurityLevel
                })
                .Update(),
            _credentialHistoryRepository
                .CreateHistoryItemQuery(oldCredentialEntity),
            _credentialByPasswordRepository
                .DeleteCredentialByPasswordQuery(oldCredentialEntity),
            _credentialByPasswordRepository
                .CreateCredentialByPasswordQuery(newCredentialEntity)
        };
        if (oldCredentialEntity.PasswordSecurityLevel != newCredentialEntity.PasswordSecurityLevel)
            batch.AddRange(new []
            {
                _credentialBySecurityLevelRepository.DeleteCredentialBySecurityLevelQuery(oldCredentialEntity),
                _credentialBySecurityLevelRepository.CreateCredentialBySecurityLevelQuery(newCredentialEntity)
            });

        await ExecuteAsBatchAsync(batch);
    }

    public async Task<long> GetCountAsync(string userLogin)
    {
        // ReSharper disable once ReplaceWithSingleCallToCount
        return await ExecuteScalarQueryAsync(Table.Where(r => r.UserLogin == userLogin).Count());
    }

    public async Task<Dictionary<PasswordSecurityLevel, long>> GetPasswordsLevelsInfoAsync(string userLogin)
    {
        var result = new Dictionary<PasswordSecurityLevel, long>();
        var secureLevel =
            await _credentialBySecurityLevelRepository.GetCountOfUserPasswordWithSecurityLevelAsync(userLogin,
                (int)PasswordSecurityLevel.Secure);
        var insecureLevel =
            await _credentialBySecurityLevelRepository.GetCountOfUserPasswordWithSecurityLevelAsync(userLogin,
                (int)PasswordSecurityLevel.Insecure);
        var compromisedLevel =
            await _credentialBySecurityLevelRepository.GetCountOfUserPasswordWithSecurityLevelAsync(userLogin,
                (int)PasswordSecurityLevel.Compromised);

        result[PasswordSecurityLevel.Secure] = secureLevel;
        result[PasswordSecurityLevel.Insecure] = insecureLevel;
        result[PasswordSecurityLevel.Compromised] = compromisedLevel;
        return result;
    }
}

public record CredentialForCompareByFields(string UserLogin, string ResourceName, string ResourceLogin,
    string ResourcePassword);