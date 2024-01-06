using Cassandra.Data.Linq;
using Domain.Factories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.CredentialHistoryRepository;

public class CredentialHistoryRepository : CassandraRepositoryBase<CredentialHistoryItemEntity>
{
    public CredentialHistoryRepository(ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<CredentialHistoryItemEntity>> logger) : base(sessionFactory,
        logger)
    {
    }
}