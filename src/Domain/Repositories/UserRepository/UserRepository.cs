using Cassandra.Data.Linq;
using Domain.DTO;
using Domain.Factories;
using Domain.Models;
using Domain.Repositories.CredentialCountBySecurityLevelRepository;
using Domain.Repositories.CredentialRepository;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.UserRepository;

public class UserRepository : CassandraRepositoryBase<UserEntity>, IUserRepository
{
    private readonly ICredentialCountBySecurityLevelRepository
        _credentialCountBySecurityLevelRepository;

    private readonly ICredentialRepository _credentialRepository;

    public UserRepository(ICassandraSessionFactory sessionFactory, ILogger<UserRepository> logger,
        ICredentialCountBySecurityLevelRepository credentialCountBySecurityLevelRepository,
        ICredentialRepository credentialRepository) : base(
        sessionFactory, logger)
    {
        _credentialCountBySecurityLevelRepository = credentialCountBySecurityLevelRepository;
        _credentialRepository = credentialRepository;
    }

    public async Task<UserEntity> GetUserAsync(string login)
    {
        var user = await TryGetUserAsync(login);
        if (user == null) throw new Exception($"User with login {login} doesnt exist");
        return user;
    }

    public async Task<UserEntity?> TryGetUserAsync(string login)
    {
        var users = (await ExecuteQueryAsync(Table.Where(r => r.Login == login))).ToArray();
        if (users.Length > 1) throw new Exception("Key Login in User doesnt unique");
        return users.FirstOrDefault();
    }

    public async Task DeleteUserAsync(string login)
    {
        await GetUserAsync(login);

        var batch = new List<CqlCommand>
        {
            Table.Where(r => r.Login == login).Delete()
        };
        var credBatch =
            await _credentialRepository.DeleteUserCredentialsWithDependenciesQueriesAsync(login);
        batch.AddRange(credBatch);
        await ExecuteAsBatchAsync(batch);
    }

    public async Task CreateUserAsync(UserEntity userEntity)
    {
        var userEntityExist = await TryGetUserAsync(userEntity.Login);
        if (userEntityExist is not null) throw new Exception($"User already exist");

        await ExecuteQueryAsync(Table.Insert(userEntity));
        await _credentialCountBySecurityLevelRepository.CreateCountersForEachSecurityLevelAsync(
            userEntity.Login);
    }

    public async Task ChangePasswordAsync(string login, string newPassword)
    {
        await GetUserAsync(login);
        await ExecuteQueryAsync(Table
            .Where(r => r.Login == login)
            .Select(r => new {Password = newPassword})
            .Update());
    }

    public async Task ChangeAccessTokenAsync(string login, TokenInfo tokenInfo)
    {
        await GetUserAsync(login);
        await ExecuteQueryAsync(Table
            .Where(r => r.Login == login)
            .Select(r => new {AccessToken = tokenInfo.Token, AccessTokenExpire = tokenInfo.Expire})
            .Update());
    }
}