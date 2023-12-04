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

    public CredentialRepository(IOptions<CassandraOptions> cassandraOptions,
        ILogger<CredentialRepository> logger, ICredentialHistoryRepository credentialHistoryRepository) : base(
        cassandraOptions, logger)
    {
        _credentialHistoryRepository = credentialHistoryRepository;
    }

    public async Task<IEnumerable<CredentialEntity>> GetCredentialsAsync(string userLogin)
    {
        return await ExecuteQueryAsync(Table.Where(r => r.UserLogin == userLogin));
    }

    public async Task<IEnumerable<CredentialEntity>> GetCredentialsAsync(string userLogin, string resourceName)
    {
        return await ExecuteQueryAsync(Table.Where(r => r.UserLogin == userLogin && r.ResourceName == resourceName));
    }

    public async Task<CredentialEntity?> GetCredentialAsync(string userLogin, string resourceName,
        string resourceLogin)
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
        await AddAsync(credentialEntity);
    }

    public async Task DeleteCredentialAsync(string userLogin, string resourceName, string resourceLogin)
    {
        await ExecuteQueryAsync(Table
            .Where(r => r.UserLogin == userLogin && r.ResourceName == resourceName && r.ResourceLogin == resourceLogin)
            .Delete());
        await _credentialHistoryRepository.DeleteAllHistoryItemsByCredentialAsync(userLogin, resourceName,
            resourceLogin);
    }

    public async Task DeleteAllCredentialsAsync(string userLogin)
    {
        await ExecuteQueryAsync(Table
            .Where(r => r.UserLogin == userLogin)
            .Delete());
        await _credentialHistoryRepository.DeleteAllUserHistoryItemsAsync(userLogin);
    }

    public async Task DeleteResourceCredentialsAsync(string userLogin, string resourceName)
    {
        await ExecuteQueryAsync(Table
            .Where(r => r.UserLogin == userLogin && r.ResourceName == resourceName)
            .Delete());

        await _credentialHistoryRepository.DeleteAllUserHistoryItemsByResourceAsync(userLogin, resourceName);
    }

    public async Task<IEnumerable<string>> FindPasswordDuplicatesAsync(string password)
    {
        return await ExecuteQueryAsync(Table
            .Where(r => r.ResourcePassword == password)
            .Select(r => r.UserLogin));
    }

    public async Task UpdateCredentialAsync(CredentialEntity newCredentialEntity)
    {
        var oldCredential = (await ExecuteQueryAsync(Table
                .Where(r => r.UserLogin == newCredentialEntity.UserLogin &&
                            r.ResourceName == newCredentialEntity.ResourceName &&
                            r.ResourceLogin == newCredentialEntity.ResourceLogin)))
            .Single() ?? throw new DataException("Scheme error");

        await UpdateAsync(Table
            .Where(r => r.UserLogin == newCredentialEntity.UserLogin &&
                        r.ResourceName == newCredentialEntity.ResourceName &&
                        r.ResourceLogin == newCredentialEntity.ResourceLogin)
            .Select(r => new CredentialEntity
            {
                ResourcePassword = newCredentialEntity.ResourcePassword,
                ChangeAt = newCredentialEntity.ChangeAt,
                PasswordSecurityLevel = newCredentialEntity.PasswordSecurityLevel
            })
            .Update());

        await _credentialHistoryRepository.CreateHistoryItemAsync(oldCredential);
    }

    public async Task<long> GetCountAsync(string userLogin)
    {
        // ReSharper disable once ReplaceWithSingleCallToCount
        return await ExecuteScalarQueryAsync(Table
            .Where(r => r.UserLogin == userLogin)
            .Count());
    }

    public async Task<Dictionary<PasswordSecurityLevel, long>> GetPasswordsLevelsInfoAsync(string userLogin)
    {
        var result = new Dictionary<PasswordSecurityLevel, long>();
        // ReSharper disable once ReplaceWithSingleCallToCount
        var secureLevel = await ExecuteScalarQueryAsync(Table
            .Where(r => r.PasswordSecurityLevel == PasswordSecurityLevel.Secure
                && r.UserLogin == userLogin)
            .Count());
        // ReSharper disable once ReplaceWithSingleCallToCount
        var insecureLevel = await ExecuteScalarQueryAsync(Table
            .Where(r => r.PasswordSecurityLevel == PasswordSecurityLevel.Insecure
                && r.UserLogin == userLogin)
            .Count());
        // ReSharper disable once ReplaceWithSingleCallToCount
        var compromisedLevel = await ExecuteScalarQueryAsync(Table
            .Where(r => r.PasswordSecurityLevel == PasswordSecurityLevel.Compromised
                && r.UserLogin == userLogin)
            .Count());

        result[PasswordSecurityLevel.Secure] = secureLevel;
        result[PasswordSecurityLevel.Insecure] = insecureLevel;
        result[PasswordSecurityLevel.Compromised] = compromisedLevel;
        return result;
    }
}