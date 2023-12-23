using Cassandra.Data.Linq;
using Domain.Mappers;
using Domain.Models;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories.CredentialHistoryRepository;

public class CredentialHistoryRepository : CassandraRepositoryBase<CredentialHistoryItemEntity>,
    ICredentialHistoryRepository
{
    public CredentialHistoryRepository(IOptions<CassandraOptions> cassandraOptions,
        ILogger<CassandraRepositoryBase<CredentialHistoryItemEntity>> logger) 
        : base(cassandraOptions, logger)
    {
    }

    public async Task CreateHistoryItemAsync(CredentialEntity credentialEntity)
    {
        await AddAsync(credentialEntity.ToCredentialHistoryItemEntity());
    }
    
    public CqlCommand CreateHistoryItemQuery(CredentialEntity credentialEntity)
    {
        return AddQuery(credentialEntity.ToCredentialHistoryItemEntity());
    }

    public async Task<IEnumerable<CredentialHistoryItemEntity>> GetHistoryByCredentialIdAsync(Guid credentialId)
    {
        return await ExecuteQueryAsync(Table.Where(r => r.CredentialId == credentialId));
    }
    
    public async Task DeleteHistoryByCredentialIdAsync(Guid credentialId)
    {
        await ExecuteQueryAsync(Table.Where(r => r.CredentialId == credentialId));
    }
    
    public CqlCommand DeleteHistoryByCredentialIdQuery(Guid credentialId)
    {
        return Table.Where(r => r.CredentialId == credentialId).Delete();
    }
}