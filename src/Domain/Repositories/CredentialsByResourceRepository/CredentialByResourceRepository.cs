using Cassandra.Data.Linq;
using Domain.Factories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.CredentialsByResourceRepository;

public class CredentialByResourceRepository : CassandraRepositoryBase<CredentialByResourceEntity>,
    ICredentialByResourceRepository
{
    public CredentialByResourceRepository(ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<CredentialByResourceEntity>> logger) : base(sessionFactory, logger)
    {
    }

    public CredentialByResourceRepository(Table<CredentialByResourceEntity> table,
        ILogger<CassandraRepositoryBase<CredentialByResourceEntity>> logger) : base(table, logger)
    {
    }

    public CqlCommand CreateCredentialByResourceQuery(CredentialByResourceEntity credentialByResourceEntity)
    {
        return AddQuery(credentialByResourceEntity);
    }

    public CqlCommand DeleteCredentialByResourceQuery(CredentialEntity credentialEntity)
    {
        return DeleteQuery(e =>
            e.UserLogin == credentialEntity.UserLogin
            && e.ResourceLogin == credentialEntity.ResourceLogin
            && e.ResourceName == credentialEntity.ResourceName);
    }
}