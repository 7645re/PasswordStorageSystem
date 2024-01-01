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

    record PagedQuery(byte[]? State, bool PageIsEmpty);

    record CredentialPagination(int PageCount, PagedQuery[] PagedQueries);

    public async Task<CredentialEntity[]> GetCredentialsByLoginPagedAsync(string login, int pageSize, int pageNumber)
    {
        if (pageNumber <= 0)
            throw new ArgumentException("Page number cannot be less or equal zero");
        if (pageSize <= 0)
            throw new ArgumentException("Page size cannot be less or equal zero");

        var cacheKey = $"{login}_{typeof(CredentialPagination)}_{pageSize}";
        _memoryCache.TryGetValue(cacheKey,
            out CredentialPagination cachedCredentialPagination);
        if (cachedCredentialPagination == null)
        {
            var count = await ExecuteQueryAsync(
                Table
                    .Where(e => e.UserLogin == login)
                    .Count());
            var pageCount = (int)Math.Ceiling((decimal) (count / pageSize));
            cachedCredentialPagination = new CredentialPagination(pageNumber, new PagedQuery[pageCount]);
            _memoryCache.Set(cacheKey, cachedCredentialPagination);
        }

        if (pageNumber > cachedCredentialPagination.PageCount)
            throw new ArgumentException($"Page with number {pageNumber} doesnt exist");
        
        IPage<CredentialEntity>? credentials;
        CredentialEntity[]? result;
        if (pageNumber == 1)
        {
            credentials = await ExecuteQueryPagedAsync(Table
                .Where(r => r.UserLogin == login)
                .SetPageSize(pageSize));
            result = credentials.ToArray();
            _memoryCache.Set(cacheKey,
                new PagedQuery(
                    credentials.PagingState,
                    result.Length <= 0));
            return result;
        }

        var previousCacheKey = $"{login}_{pageSize}_{pageNumber - 1}";
        _memoryCache.TryGetValue(previousCacheKey, out PagedQuery previousPagedQuery);

        if (previousPagedQuery is null)
            throw new Exception("Pages can only be requested sequentially");
        if (previousPagedQuery.PageIsEmpty)
            return Array.Empty<CredentialEntity>();

        credentials = await ExecuteQueryPagedAsync(Table
            .Where(r => r.UserLogin == login)
            .SetPageSize(pageSize)
            .SetPagingState(previousPagedQuery.State));

        result = credentials.ToArray();

        _memoryCache.Set(currentCacheKey,
            new PagedQuery(
                credentials.PagingState,
                result.Length <= 0));
        return result;
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