using System.Data;
using Domain.Models;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public class UserRepository : CassandraRepositoryBase<User>, IUserRepository
{

    public UserRepository(IOptions<CassandraOptions> cassandraOptions) : base(cassandraOptions)
    {
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        var result = await GetByFilterAsync(user => user.Login == login);
        return result.Length switch
        {
            0 => null,
            1 => result.Single(),
            _ => throw new DataException()
        };
    }

    public async Task<bool> ExistsIsUserAsync(User user)
    {
        var result = await GetByLoginAsync(user.Login);
        return result?.Password == user.Password;
    }

    public async Task<string> GetPasswordByLoginAsync(string login)
    {
        var result = await GetByFilterAsync(user => user.Login == login);
        return result.Single().Login ?? throw new DataException();
    }

    public async Task ChangePasswordByLoginAsync(User user)
    {
        await AddAsync(user);
    }

    public async Task CreateUserAsync(User user)
    {
        await AddAsync(user);
    }
}