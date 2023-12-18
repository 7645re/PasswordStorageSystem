using Cassandra.Data.Linq;
using Domain.Enums;
using Domain.Mappers;
using Domain.Models;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public class CredentialBySecurityLevelRepository : CassandraRepositoryBase<CredentialBySecurityLevelEntity>, ICredentialBySecurityLevelRepository
{
    public CredentialBySecurityLevelRepository(IOptions<CassandraOptions> cassandraOptions,
        ILogger<CassandraRepositoryBase<CredentialBySecurityLevelEntity>> logger) : base(cassandraOptions, logger)
    {
    }

    public CqlCommand CreateCredentialBySecurityLevelQuery(CredentialEntity credentialEntity)
    {
        return AddQuery(credentialEntity.ToCredentialSecurityLevelEntity());
    }

    public CqlCommand DeleteCredentialBySecurityLevelQuery(CredentialEntity credentialEntity)
    {
        var passwordSecurityLevel = (int)credentialEntity.PasswordSecurityLevel;
        return Table.Where(r =>
                r.PasswordSecurityLevel == passwordSecurityLevel
                && r.UserLogin == credentialEntity.UserLogin
                && r.ResourceName == credentialEntity.ResourceName
                && r.ResourceLogin == credentialEntity.ResourceLogin)
            .Delete();
    }

    public async Task<long> GetCountOfUserPasswordWithSecurityLevelAsync(string userLogin,
        int passwordSecurityLevel)
    {
        //TODO: Figure out why the converter is not used for such a request
        // ReSharper disable once ReplaceWithSingleCallToCount
        return await ExecuteScalarQueryAsync(Table.Where(r =>
            r.PasswordSecurityLevel == passwordSecurityLevel
            && r.UserLogin == userLogin).Count());
    }
}