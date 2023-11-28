using Cassandra.Data.Linq;
using Domain.Models;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public class CredentialHistoryRepository : CassandraRepositoryBase<CredentialHistoryItemEntity>, ICredentialHistoryRepository
{
    public CredentialHistoryRepository(IOptions<CassandraOptions> cassandraOptions,
        ILogger<CassandraRepositoryBase<CredentialHistoryItemEntity>> logger) : base(cassandraOptions, logger)
    {
    }

    public async Task CreateHistoryItemAsync(CredentialEntity credentialEntity)
    {
        await AddAsync(new CredentialHistoryItemEntity
        {
            UserLogin = credentialEntity.UserLogin,
            ResourceName = credentialEntity.ResourceName,
            ResourceLogin = credentialEntity.ResourceLogin,
            ResourcePassword = credentialEntity.ResourcePassword,
            ChangeAt = credentialEntity.ChangeAt
        });
    }

    public async Task<IEnumerable<CredentialHistoryItemEntity>> GetHistoryItemByCredentialAsync(CredentialEntity credentialEntity)
    {
        return await ExecuteQueryAsync(Table.Where(r => r.UserLogin == credentialEntity.UserLogin
                                                 && r.ResourceName == credentialEntity.ResourceName
                                                 && r.ResourceLogin == credentialEntity.ResourceLogin
                                                 && r.ResourcePassword == credentialEntity.ResourcePassword));
    }
    
    public async Task<IEnumerable<CredentialHistoryItemEntity>> GetHistoryItemByUserAsync(string userLogin)
    {
        return await ExecuteQueryAsync(Table.Where(r => r.UserLogin == userLogin));
    }

    public async Task DeleteHistoryItemAsync(string userLogin, string resourceName, string resourceLogin,
        string resourcePassword)
    {
        await ExecuteQueryAsync(Table
            .Where(r => r.UserLogin == userLogin
                        && r.ResourceName == resourceName
                        && r.ResourceLogin == resourceLogin
                        && r.ResourcePassword == resourcePassword)
            .Delete());
    }

    public async Task DeleteAllUserHistoryItemsAsync(string userLogin)
    {
        await ExecuteQueryAsync(Table
            .Where(r => r.UserLogin == userLogin)
            .Delete());
    }

    public async Task DeleteAllUserHistoryItemsByResourceAsync(string userLogin, string resourceName)
    {
        await ExecuteQueryAsync(Table
            .Where(r => r.UserLogin == userLogin
                        && r.ResourceName == resourceName)
            .Delete());
    }

    public async Task DeleteAllHistoryItemsByCredentialAsync(string userLogin, string resourceName,
        string resourceLogin)
    {
        await ExecuteQueryAsync(Table
            .Where(r => r.UserLogin == userLogin
                        && r.ResourceName == resourceName
                        && r.ResourceLogin == resourceLogin)
            .Delete());
    }
}