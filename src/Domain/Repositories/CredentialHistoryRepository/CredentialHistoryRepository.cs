using Cassandra.Data.Linq;
using Domain.Factories;
using Domain.Mappers;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.CredentialHistoryRepository;

public class CredentialHistoryRepository : CassandraRepositoryBase<CredentialHistoryItemEntity>,
    ICredentialHistoryRepository
{
    public CredentialHistoryRepository(ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<CredentialHistoryItemEntity>> logger) : base(sessionFactory,
        logger)
    {
    }

    public CqlCommand CreateCredentialHistoryItemQuery(CredentialEntity credentialEntity)
    {
        return Table.Insert(credentialEntity.ToCredentialHistoryItem());
    }

    public CqlCommand DeleteCredentialHistoriesItemsQuery(Guid id)
    {
        return Table.Where(e => e.CredentialId == id).Delete();
    }

    public CqlCommand DeleteCredentialsHistoriesItemsQuery(IEnumerable<Guid> ids)
    {
        return Table.Where(e => ids.Contains(e.CredentialId)).Delete();
    }
}