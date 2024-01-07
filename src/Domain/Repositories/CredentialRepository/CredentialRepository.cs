using Cassandra.Data.Linq;
using Domain.Factories;
using Domain.Mappers;
using Domain.Models;
using Domain.Repositories.CredentialCountBySecurityLevelRepository;
using Domain.Repositories.CredentialHistoryRepository;
using Domain.Repositories.CredentialsByResourceRepository;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.CredentialRepository;

public class CredentialRepository : CassandraRepositoryBase<CredentialEntity>, ICredentialRepository
{
    private readonly ICredentialCountBySecurityLevelRepository
        _credentialCountBySecurityLevelRepository;

    private readonly ICredentialByResourceRepository _credentialByResourceRepository;
    private readonly ICredentialHistoryRepository _credentialHistoryRepository;
    private readonly CredentialPagingStateManager _credentialPagingStateManager = new();

    public CredentialRepository(ICassandraSessionFactory sessionFactory,
        ILogger<CassandraRepositoryBase<CredentialEntity>> logger,
        ICredentialCountBySecurityLevelRepository credentialCountBySecurityLevelRepository,
        ICredentialByResourceRepository credentialByResourceRepository,
        ICredentialHistoryRepository credentialHistoryRepository) : base(sessionFactory, logger)
    {
        _credentialCountBySecurityLevelRepository = credentialCountBySecurityLevelRepository;
        _credentialByResourceRepository = credentialByResourceRepository;
        _credentialHistoryRepository = credentialHistoryRepository;
    }

    public async Task<CredentialEntity?> TryGetCredentialAsync(
        string userLogin,
        DateTimeOffset createdAt,
        Guid id)
    {
        var existedCredentialsEntities = (await ExecuteQueryAsync(Table
            .Where(e => e.UserLogin == userLogin
                        && e.CreatedAt == createdAt
                        && e.Id == id))).ToArray();
        if (existedCredentialsEntities.Length > 1)
            throw new Exception("Keys UserLogin, CreatedAt, Id in Credentials doesnt unique");
        return existedCredentialsEntities.FirstOrDefault();
    }

    public async Task<CredentialEntity> GetCredentialAsync(CredentialEntity credentialEntity)
    {
        var credential = await TryGetCredentialAsync(
            credentialEntity.UserLogin,
            credentialEntity.CreatedAt,
            credentialEntity.Id);
        if (credential is null)
            throw new Exception($"Credential with values {credentialEntity.UserLogin}," +
                                $" {credentialEntity.CreatedAt}," +
                                $" {credentialEntity.Id} doesnt exist");
        return credential;
    }

    public async Task<CredentialEntity[]> GetCredentialsByLoginPagedAsync(
        string login,
        int pageSize,
        int pageNumber)
    {
        if (pageNumber <= 0)
            throw new ArgumentException("Page number cannot be less or equal zero");
        if (pageSize <= 0)
            throw new ArgumentException("Page size cannot be less or equal zero");

        var result = Array.Empty<CredentialEntity>();
        var pagingStatesByPageSize = _credentialPagingStateManager
            .GetPagingStatesByPageSize(login, pageSize);

        if (pagingStatesByPageSize.LastPageNumber >= pageNumber)
        {
            var pagingState = pagingStatesByPageSize.GetPagingState(pageNumber);
            if (pagingState is null && pageNumber != 1)
                throw new ArgumentException("Pages Out");

            return (await ExecuteQueryPagedAsync(
                    Table
                        .Where(r => r.UserLogin == login)
                        .SetPageSize(pageSize)
                        .SetPagingState(pagingState)))
                .ToArray();
        }

        while (pagingStatesByPageSize.LastPageNumber < pageNumber + 1)
        {
            var pagingState = pagingStatesByPageSize
                .GetPagingState(pagingStatesByPageSize.LastPageNumber);
            if (pagingState is null && pagingStatesByPageSize.LastPageNumber != 1)
                throw new ArgumentException("Pages Out");

            var credentials = await ExecuteQueryPagedAsync(
                Table
                    .Where(r => r.UserLogin == login)
                    .SetPageSize(pageSize)
                    .SetPagingState(pagingState));

            pagingStatesByPageSize.AddLastPagingState(
                pagingStatesByPageSize.LastPageNumber + 1,
                credentials.PagingState);

            if (pagingStatesByPageSize.LastPageNumber == pageNumber + 1)
                result = credentials.ToArray();
        }

        return result;
    }

    public async Task CreateCredentialAsync(CredentialEntity credentialEntity)
    {
        var existedCredentialByResourceEntity = await _credentialByResourceRepository
            .TryGetCredentialByResourceAsync(credentialEntity.ToCredentialByResourceEntity());
        if (existedCredentialByResourceEntity is not null)
            throw new Exception($"Credential {credentialEntity.UserLogin}," +
                                $" {credentialEntity.ResourceName}, " +
                                $"{credentialEntity.ResourceLogin} already exist");

        var batch = new[]
        {
            Table.Insert(credentialEntity),
            _credentialByResourceRepository.CreateCredentialByResourceQuery(credentialEntity
                .ToCredentialByResourceEntity())
        };

        await ExecuteAsBatchAsync(batch);
        await _credentialCountBySecurityLevelRepository
            .IncrementCredentialCountBySecurityLevelAsync(credentialEntity);
        _credentialPagingStateManager.Clear(credentialEntity.UserLogin);
    }

    public async Task UpdateCredentialAsync(CredentialEntity newCredentialEntity)
    {
        var oldCredentialEntity = await GetCredentialAsync(newCredentialEntity);
        if (oldCredentialEntity.ResourcePassword == newCredentialEntity.ResourcePassword)
            throw new Exception(
                "Credential state hasn't changed in any way, so it can't be updated");

        var batch = new[]
        {
            Table
                .Where(r => r.UserLogin == newCredentialEntity.UserLogin &&
                            r.CreatedAt == newCredentialEntity.CreatedAt &&
                            r.Id == newCredentialEntity.Id)
                .Select(r => new CredentialEntity
                {
                    ResourcePassword = newCredentialEntity.ResourcePassword,
                    ChangedAt = newCredentialEntity.ChangedAt,
                    PasswordSecurityLevel = newCredentialEntity.PasswordSecurityLevel
                })
                .Update(),
            _credentialHistoryRepository.CreateCredentialHistoryItemQuery(newCredentialEntity)
        };
        await ExecuteAsBatchAsync(batch);

        if (oldCredentialEntity.PasswordSecurityLevel != newCredentialEntity.PasswordSecurityLevel)
        {
            var counterBatch = new CqlCommand[]
            {
                _credentialCountBySecurityLevelRepository
                    .DecrementCredentialCountBySecurityLevelQuery(oldCredentialEntity),
                _credentialCountBySecurityLevelRepository
                    .IncrementCredentialCountBySecurityLevelQuery(newCredentialEntity)
            };
            await ExecuteAsBatchAsync(counterBatch);
        }
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
            _credentialByResourceRepository.DeleteCredentialByResourceQuery(credentialEntity),
            _credentialHistoryRepository.DeleteCredentialHistoriesItemsQuery(credentialEntity.Id)
        });
        await _credentialCountBySecurityLevelRepository
            .DecrementCredentialCountBySecurityLevelAsync(credentialEntity);
        _credentialPagingStateManager.Clear(credentialEntity.UserLogin);
    }

    private CqlCommand DeleteUserCredentialsQuery(string userLogin)
    {
        return Table
            .Where(e => e.UserLogin == userLogin)
            .Delete();
    }

    public async Task<IEnumerable<CqlCommand>> DeleteUserCredentialsWithDependenciesQueriesAsync(
        string userLogin)
    {
        var allUserCredentials = await ExecuteQueryAsync(Table
            .Where(e => e.UserLogin == userLogin)
            .Select(e => e.Id));

        return new[]
        {
            DeleteUserCredentialsQuery(userLogin),
            _credentialByResourceRepository.DeleteCredentialsByResourceQuery(userLogin),
            _credentialHistoryRepository.DeleteCredentialsHistoriesItemsQuery(allUserCredentials)
        };
    }

    public async Task DeleteCredentialsAsync(string userLogin)
    {
        var batch = await DeleteUserCredentialsWithDependenciesQueriesAsync(userLogin);
        await ExecuteAsBatchAsync(batch);
        await _credentialCountBySecurityLevelRepository.ResetAllUserSecurityLevelCounterAsync(
            userLogin);
        _credentialPagingStateManager.Clear(userLogin);
    }
}