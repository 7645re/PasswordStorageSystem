using Cassandra.Data.Linq;
using Domain.Factories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.CredentialCountBySecurityLevelRepository;

public class CredentialCountBySecurityLevelRepository : CassandraRepositoryBase<CredentialCountBySecurityLevelEntity>,
    ICredentialCountBySecurityLevelRepository
{
    public CredentialCountBySecurityLevelRepository(ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<CredentialCountBySecurityLevelEntity>> logger) : base(sessionFactory, logger)
    {
    }

    public CredentialCountBySecurityLevelRepository(Table<CredentialCountBySecurityLevelEntity> table,
        ILogger<CassandraRepositoryBase<CredentialCountBySecurityLevelEntity>> logger) : base(table, logger)
    {
    }

    public CqlCommand CreateCredentialCountBySecurityLevelQuery(
        CredentialCountBySecurityLevelEntity credentialCountBySecurityLevelEntity)
    {
        return AddQuery(credentialCountBySecurityLevelEntity);
    }

    public CqlCommand DeleteCredentialCountBySecurityLevelQuery(CredentialEntity credentialEntity)
    {
        return DeleteQuery(e =>
            e.UserLogin == credentialEntity.UserLogin
            && e.PasswordSecurityLevel == credentialEntity.PasswordSecurityLevel);
    }

    public CqlCommand IncrementCredentialCountBySecurityLevelQuery(CredentialEntity credentialEntity)
    {
        return Table
            .Where(e =>
                e.UserLogin == credentialEntity.UserLogin
                && e.PasswordSecurityLevel == credentialEntity.PasswordSecurityLevel)
            .Select(e => e.Count + 1)
            .Update();
    }

    public CqlCommand DecrementCredentialCountBySecurityLevelQuery(CredentialEntity credentialEntity)
    {
        return Table
            .Where(e =>
                e.UserLogin == credentialEntity.UserLogin
                && e.PasswordSecurityLevel == credentialEntity.PasswordSecurityLevel)
            .Select(e => e.Count - 1)
            .Update();
    }
}