using Cassandra.Data.Linq;
using Domain.DTO;
using Domain.Models;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories.UserRepository;

public class UserRepository : CassandraRepositoryBase<UserEntity>, IUserRepository
{
    public UserRepository(IOptions<CassandraOptions> cassandraOptions, ILogger<UserRepository> logger) : base(
        cassandraOptions, logger)
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

    public async Task CheckExistAsync(string login)
    {
        await GetUserAsync(login);
    }

    public async Task DeleteUserAsync(string login)
    {
        await CheckExistAsync(login);
        await ExecuteQueryAsync(Table.Where(r => r.Login == login).Delete());
    }

    public async Task CreateUserAsync(UserEntity userEntity)
    {
        await CheckExistAsync(userEntity.Login);
        await AddAsync(userEntity);
    }

    public async Task ChangePasswordAsync(string login, string newPassword)
    {
        await CheckExistAsync(login);
        await UpdateAsync(Table.Where(r => r.Login == login).Select(r => new { Password = newPassword }).Update());
    }

    public async Task ChangeAccessTokenAsync(string login, TokenInfo tokenInfo)
    {
        await CheckExistAsync(login);
        await UpdateAsync(Table.Where(r => r.Login == login)
            .Select(r => new { AccessToken = tokenInfo.Token, AccessTokenExpire = tokenInfo.Expire }).Update());
    }
}