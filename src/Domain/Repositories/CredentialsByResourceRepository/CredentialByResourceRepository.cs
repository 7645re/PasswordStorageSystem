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

    public async Task<CredentialByResourceEntity> GetCredentialByResourceAsync(
        CredentialByResourceEntity credentialByResourceEntity)
    {
        var existedCredentialByResourceEntities = await TryGetCredentialByResourceAsync(credentialByResourceEntity);
        if (existedCredentialByResourceEntities is null) 
            throw new Exception($"Credential {credentialByResourceEntity.UserLogin}, " +
                                $"{credentialByResourceEntity.ResourceName}, " +
                                $"{credentialByResourceEntity.ResourceLogin} doesnt exist");
        return existedCredentialByResourceEntities;
    }
    
    public async Task<CredentialByResourceEntity?> TryGetCredentialByResourceAsync(
        CredentialByResourceEntity credentialByResourceEntity)
    {
        var existedCredentialByResourceEntities = (await ExecuteQueryAsync(Table
            .Where(e => e.UserLogin == credentialByResourceEntity.UserLogin
                        && e.ResourceName == credentialByResourceEntity.ResourceName
                        && e.ResourceLogin == credentialByResourceEntity.ResourceLogin))).ToArray();
        if (existedCredentialByResourceEntities.Length > 1)
            throw new Exception("Keys UserLogin, ResourceName, ResourceLogin" +
                                " in CredentialByResource doesnt unique");
        return existedCredentialByResourceEntities.FirstOrDefault();
    }

    public CqlCommand CreateCredentialByResourceQuery(CredentialByResourceEntity credentialByResourceEntity)
    {
        return Table.Insert(credentialByResourceEntity);
    }

    public CqlCommand DeleteCredentialByResourceQuery(CredentialEntity credentialEntity)
    {
        return Table
            .Where(e =>
                e.UserLogin == credentialEntity.UserLogin
                && e.ResourceLogin == credentialEntity.ResourceLogin
                && e.ResourceName == credentialEntity.ResourceName)
            .Delete();
    }

    public CqlCommand DeleteCredentialsByResourceQuery(string userLogin)
    {
        return Table
            .Where(e => e.UserLogin == userLogin)
            .Delete();
    }
}