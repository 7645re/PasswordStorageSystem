using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Domain.DTO.Credential;
using Domain.Factories;
using Domain.Mappers;
using Domain.Models;
using Domain.Repositories.CredentialCountBySecurityLevelRepository;
using Domain.Repositories.CredentialsByResourceRepository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.CredentialRepository;

public class CredentialRepository : CassandraRepositoryBase<CredentialEntity>, ICredentialRepository
{
    private readonly ICredentialCountBySecurityLevelRepository _credentialCountBySecurityLevelRepository;
    private readonly ICredentialByResourceRepository _credentialByResourceRepository;
    private readonly IMemoryCache _memoryCache;

    public CredentialRepository(ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<CredentialEntity>> logger,
        ICredentialCountBySecurityLevelRepository credentialCountBySecurityLevelRepository,
        ICredentialByResourceRepository credentialByResourceRepository,
        IMemoryCache memoryCache) : base(sessionFactory, logger)
    {
        _credentialCountBySecurityLevelRepository = credentialCountBySecurityLevelRepository;
        _credentialByResourceRepository = credentialByResourceRepository;
        _memoryCache = memoryCache;
    }

    public CredentialRepository(Table<CredentialEntity> table,
        ILogger<CassandraRepositoryBase<CredentialEntity>> logger,
        ICredentialCountBySecurityLevelRepository credentialCountBySecurityLevelRepository,
        ICredentialByResourceRepository credentialByResourceRepository, IMemoryCache memoryCache) : base(table, logger)
    {
        _credentialCountBySecurityLevelRepository = credentialCountBySecurityLevelRepository;
        _credentialByResourceRepository = credentialByResourceRepository;
        _memoryCache = memoryCache;
    }

    public async Task<CredentialEntity?> TryGetCredentialAsync(CredentialEntity credentialEntity)
    {
        var existedCredentialsEntities = (await ExecuteQueryAsync(Table
            .Where(e => e.UserLogin == credentialEntity.UserLogin
                        && e.CreatedAt == credentialEntity.CreatedAt
                        && e.Id == credentialEntity.Id))).ToArray();
        if (existedCredentialsEntities.Length > 1)
            throw new Exception("Keys UserLogin, CreatedAt, Id in Credentials doesnt unique");
        return existedCredentialsEntities.FirstOrDefault();
    }

    public async Task<CredentialEntity> GetCredentialAsync(CredentialEntity credentialEntity)
    {
        var credential = await TryGetCredentialAsync(credentialEntity);
        if (credential is null)
            throw new Exception($"Credential with values {credentialEntity.UserLogin}," +
                                $" {credentialEntity.CreatedAt}," +
                                $" {credentialEntity.Id} doesnt exist");
        return credential;
    }

    public async Task<CredentialEntity[]> GetCredentialsByLoginPagedAsync(string login, int pageSize, int pageNumber)
    {
        if (pageNumber <= 0)
            throw new ArgumentException("Page number cannot be less or equal zero");
        if (pageSize <= 0)
            throw new ArgumentException("Page size cannot be less or equal zero");

        var previousCacheKey = $"{login}_credential_pagination_{pageSize}_{pageNumber - 1}";
        var currentCacheKey = $"{login}_credential_pagination_{pageSize}_{pageNumber}";

        if (pageNumber == 1)
        {
            var credentialsFirstPage = await ExecuteQueryPagedAsync(
                Table
                    .Where(r => r.UserLogin == login)
                    .SetPageSize(pageSize));
            _memoryCache.Set(currentCacheKey, credentialsFirstPage.PagingState);
            return credentialsFirstPage.ToArray();
        }

        _memoryCache.TryGetValue(previousCacheKey, out byte[] previousState);
        if (previousState == null)
            throw new InvalidOperationException("Pages can only be requested sequentially");

        var credentials = await ExecuteQueryPagedAsync(
            Table
                .Where(r => r.UserLogin == login)
                .SetPageSize(pageSize)
                .SetPagingState(previousState));
        _memoryCache.Set(currentCacheKey, credentials.PagingState);
        return credentials.ToArray();
    }

    public async Task CreateCredentialAsync(CredentialEntity credentialEntity)
    {
        var existedCredentialByResourceEntity = await _credentialByResourceRepository
            .TryGetCredentialByResourceAsync(credentialEntity.ToCredentialByResourceEntity());
        if (existedCredentialByResourceEntity is not null)
            throw new Exception($"Credential {credentialEntity.UserLogin}," +
                                $" {credentialEntity.ResourceName}, {credentialEntity.ResourceLogin} already exist");

        var batch = new[]
        {
            Table.Insert(credentialEntity),
            _credentialByResourceRepository.CreateCredentialByResourceQuery(credentialEntity
                .ToCredentialByResourceEntity())
        };

        await ExecuteAsBatchAsync(batch);
        await _credentialCountBySecurityLevelRepository
            .IncrementCredentialCountBySecurityLevelAsync(credentialEntity);
    }

    public async Task DeleteCredentialAsync(CredentialEntity credentialEntity)
    {
        await _credentialByResourceRepository
            .GetCredentialByResourceAsync(credentialEntity.ToCredentialByResourceEntity());

        await ExecuteAsBatchAsync(new List<CqlCommand>
        {
            Table
                .Where(e =>
                    e.UserLogin == credentialEntity.UserLogin
                    && e.CreatedAt == credentialEntity.CreatedAt
                    && e.Id == credentialEntity.Id)
                .Delete(),
            _credentialByResourceRepository.DeleteCredentialByResourceQuery(credentialEntity)
        });
        await _credentialCountBySecurityLevelRepository
            .DecrementCredentialCountBySecurityLevelAsync(credentialEntity);
    }

    public async Task DeleteCredentialsAsync(string userLogin)
    {
        await ExecuteAsBatchAsync(new List<CqlCommand>
        {
            Table
                .Where(e => e.UserLogin == userLogin)
                .Delete(),
            _credentialByResourceRepository.DeleteCredentialsByResourceQuery(userLogin)
        });
        await _credentialCountBySecurityLevelRepository.ResetAllUserSecurityLevelCounterAsync(userLogin);
    }
}