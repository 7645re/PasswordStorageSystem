using Cassandra.Data.Linq;
using Domain.Factories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.CredentialRepository;

public class CredentialRepository : CassandraRepositoryBase<CredentialEntity>
{
    public CredentialRepository(ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<CredentialEntity>> logger) : base(sessionFactory, logger)
    {
    }

    public CredentialRepository(Table<CredentialEntity> table,
        ILogger<CassandraRepositoryBase<CredentialEntity>> logger) : base(table, logger)
    {
    }

    public async Task<CredentialEntity[]> GetAllCredentialsByLoginAsync(string login)
    {
        return (await ExecuteQueryAsync(Table.Where(r => r.UserLogin == login))).ToArray();
    }

    public async Task CreateCredentialAsync(CredentialEntity credentialEntity)
    {
        await AddAsync(credentialEntity);
    }

    public async Task DeleteCredentialAsync()
    {
        
    }
}