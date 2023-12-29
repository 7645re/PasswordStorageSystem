using Cassandra.Data.Linq;
using Domain.Factories;
using Domain.Mappers;
using Domain.Models;
using Domain.Repositories.CredentialCountBySecurityLevelRepository;
using Domain.Repositories.CredentialsByResourceRepository;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.CredentialRepository;

public class CredentialRepository : CassandraRepositoryBase<CredentialEntity>, ICredentialRepository
{
    private readonly ICredentialCountBySecurityLevelRepository _credentialCountBySecurityLevelRepository;
    private readonly ICredentialByResourceRepository _credentialByResourceRepository;

    public CredentialRepository(ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<CredentialEntity>> logger,
        ICredentialCountBySecurityLevelRepository credentialCountBySecurityLevelRepository,
        ICredentialByResourceRepository credentialByResourceRepository) : base(sessionFactory, logger)
    {
        _credentialCountBySecurityLevelRepository = credentialCountBySecurityLevelRepository;
        _credentialByResourceRepository = credentialByResourceRepository;
    }

    public CredentialRepository(Table<CredentialEntity> table,
        ILogger<CassandraRepositoryBase<CredentialEntity>> logger,
        ICredentialCountBySecurityLevelRepository credentialCountBySecurityLevelRepository,
        ICredentialByResourceRepository credentialByResourceRepository) : base(table, logger)
    {
        _credentialCountBySecurityLevelRepository = credentialCountBySecurityLevelRepository;
        _credentialByResourceRepository = credentialByResourceRepository;
    }

    public async Task<CredentialEntity[]> GetAllCredentialsByLoginAsync(string login)
    {
        return (await ExecuteQueryAsync(Table.Where(r => r.UserLogin == login))).ToArray();
    }

    public async Task CreateCredentialAsync(CredentialEntity credentialEntity)
    {
        var batch = new[]
        {
            AddQuery(credentialEntity),
            _credentialByResourceRepository.CreateCredentialByResourceQuery(credentialEntity
                .ToCredentialByResourceEntity()),
            _credentialCountBySecurityLevelRepository.IncrementCredentialCountBySecurityLevelQuery(credentialEntity)
        };

        await ExecuteAsBatchAsync(batch);
    }

    public async Task DeleteCredentialAsync(CredentialEntity credentialEntity)
    {
        var batch = new[]
        {
            DeleteQuery(e =>
                e.UserLogin == credentialEntity.UserLogin
                && e.CreatedAt == credentialEntity.CreatedAt
                && e.Id == credentialEntity.Id),
            _credentialByResourceRepository.DeleteCredentialByResourceQuery(credentialEntity),
            _credentialCountBySecurityLevelRepository.DeleteCredentialCountBySecurityLevelQuery(credentialEntity)
        };
        
        await ExecuteAsBatchAsync(batch);
    }
}