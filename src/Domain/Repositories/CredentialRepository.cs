using System.Data;
using Cassandra.Data.Linq;
using Domain.Enums;
using Domain.Models;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

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

    public async Task<CredentialEntity?> GetCredentialAsync(string userLogin, string resourceName, string resourceLogin)
    {
        var result = await ExecuteQueryAsync(Table.Where(r =>
            r.UserLogin == userLogin && r.ResourceName == resourceName && r.ResourceLogin == resourceLogin));
        result = result.ToArray();
        if (!result.Any()) return null;
        if (result.Count() > 1) throw new DataException("Scheme error");
        return result.Single();
    }

    public async Task CreateCredentialAsync(CredentialEntity credentialEntity)
    {
        var createCredentialByPasswordQuery =
            _credentialByPasswordRepository.CreateCredentialByPasswordQuery(credentialEntity);
        var createCredentialBySecurityLevelQuery =
            _credentialBySecurityLevelRepository.CreateCredentialBySecurityLevelQuery(credentialEntity);
        var batch = new[]
        {
            createCredentialByPasswordQuery,
            createCredentialBySecurityLevelQuery
        };
        await ExecuteAsBatchAsync(batch);
        // TODO: reside create credentialEntity to batch
        await AddAsync(credentialEntity);
    }

    public async Task DeleteCredentialAsync(
        string userLogin,
        string resourceName,
        string resourceLogin)
    {
        var credentialToDelete = (await ExecuteQueryAsync(Table.Where(r =>
                r.UserLogin == userLogin
                && r.ResourceName == resourceName
                && r.ResourceLogin == resourceLogin)))
            .Single() ?? throw new DataException("Scheme error");
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
        var oldCredential = (await ExecuteQueryAsync(Table.Where(r =>
                                r.UserLogin == newCredentialEntity.UserLogin
                                && r.ResourceName == newCredentialEntity.ResourceName
                                && r.ResourceLogin == newCredentialEntity.ResourceLogin))).Single()
                            ?? throw new DataException("Scheme error");

        var updateCredentialQuery = Table
            .Where(r => r.UserLogin == newCredentialEntity.UserLogin &&
                        r.ResourceName == newCredentialEntity.ResourceName &&
                        r.ResourceLogin == newCredentialEntity.ResourceLogin)
            .Select(r => new CredentialEntity
            {
                ResourcePassword = newCredentialEntity.ResourcePassword,
                ChangedAt = newCredentialEntity.ChangedAt,
                PasswordSecurityLevel = newCredentialEntity.PasswordSecurityLevel
            })
            .Update();
        var createCredentialHistoryItemQuery = _credentialHistoryRepository
            .CreateHistoryItemQuery(oldCredential);

        var batchQueries = new[]
        {
            updateCredentialQuery,
            createCredentialHistoryItemQuery
        };

        await ExecuteAsBatchAsync(batchQueries);
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