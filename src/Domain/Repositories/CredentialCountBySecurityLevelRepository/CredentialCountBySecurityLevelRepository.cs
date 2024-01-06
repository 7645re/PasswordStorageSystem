using Cassandra.Data.Linq;
using Domain.Enums;
using Domain.Factories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.CredentialCountBySecurityLevelRepository;

public class CredentialCountBySecurityLevelRepository : CassandraRepositoryBase<CredentialCountBySecurityLevelEntity>, ICredentialCountBySecurityLevelRepository
{
    public CredentialCountBySecurityLevelRepository(ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<CredentialCountBySecurityLevelEntity>> logger) : base(sessionFactory, logger)
    {
    }

    public async Task CreateCountersForEachSecurityLevelAsync(string userLogin)
    {
        var batch = new List<CqlCommand>();

        foreach (PasswordSecurityLevel level in Enum.GetValues(typeof(PasswordSecurityLevel)))
        {
            var levelInt = (int) level;
            batch.Add(Table
                .Where(e => e.UserLogin == userLogin
                            && e.PasswordSecurityLevel == levelInt)
                .Select(e => new {Count = 0L})
                .Update());
        }

        await ExecuteAsBatchAsync(batch);
    }

    public async Task ResetAllUserSecurityLevelCounterAsync(string userLogin)
    {
        var counters = new List<CredentialCountBySecurityLevelEntity>();
        foreach (PasswordSecurityLevel level in Enum.GetValues(typeof(PasswordSecurityLevel)))
        {
            var levelInt = (int) level;
            var queryResult = await ExecuteQueryAsync(Table
                .Where(e =>
                    e.UserLogin == userLogin
                    && e.PasswordSecurityLevel == levelInt));
            counters.Add(queryResult.Single());
        }

        var batch = counters
            .Select(c =>
            {
                var countForReset = c.Count * -1L;
                return Table
                    .Where(e =>
                        e.UserLogin == userLogin
                        && e.PasswordSecurityLevel == c.PasswordSecurityLevel)
                    .Select(e => new {Count = countForReset})
                    .Update();
            });
        await ExecuteAsBatchAsync(batch);
    }

    public async Task IncrementCredentialCountBySecurityLevelAsync(CredentialEntity credentialEntity)
    {
        await ExecuteQueryAsync(IncrementCredentialCountBySecurityLevelQuery(credentialEntity));
    }
    
    public CqlUpdate IncrementCredentialCountBySecurityLevelQuery(CredentialEntity credentialEntity)
    {
        return Table
            .Where(e =>
                e.UserLogin == credentialEntity.UserLogin
                && e.PasswordSecurityLevel == credentialEntity.PasswordSecurityLevel)
            .Select(c => new {Count = 1L})
            .Update();
    }

    public async Task DecrementCredentialCountBySecurityLevelAsync(CredentialEntity credentialEntity)
    {
        await ExecuteQueryAsync(DecrementCredentialCountBySecurityLevelQuery(credentialEntity));
    }
    
    public CqlUpdate DecrementCredentialCountBySecurityLevelQuery(CredentialEntity credentialEntity)
    {
        return Table
            .Where(e =>
                e.UserLogin == credentialEntity.UserLogin
                && e.PasswordSecurityLevel == credentialEntity.PasswordSecurityLevel)
            .Select(c => new {Count = -1L})
            .Update();
    }
}