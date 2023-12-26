using Cassandra.Data.Linq;
using Domain.DTO;
using Domain.Factories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Domain.Repositories.UserRepository;

public class UserRepository : CassandraRepositoryBase<UserEntity>, IUserRepository
{
    public UserRepository(ICassandraSessionFactory sessionFactory, ILogger<UserRepository> logger) : base(
        sessionFactory, logger)
    {
    }

    public UserRepository(Table<UserEntity> table, ILogger<UserRepository> logger) : base(table, logger)
    {
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
        await ExecuteQueryAsync(Table.Where(r => r.Login == login).Delete());
    }

    public async Task CreateUserAsync(UserEntity userEntity)
    {
        var userEntityExist = await TryGetUserAsync(userEntity.Login);
        if (userEntityExist is not null) throw new Exception($"User already exist");
        await AddAsync(userEntity);
    }

    public async Task ChangePasswordAsync(string login, string newPassword)
    {
        await GetUserAsync(login);
        await UpdateAsync(Table.Where(r => r.Login == login).Select(r => new {Password = newPassword}).Update());
    }

    public async Task ChangeAccessTokenAsync(string login, TokenInfo tokenInfo)
    {
        await GetUserAsync(login);
        await UpdateAsync(Table.Where(r => r.Login == login)
            .Select(r => new {AccessToken = tokenInfo.Token, AccessTokenExpire = tokenInfo.Expire}).Update());
    }
}