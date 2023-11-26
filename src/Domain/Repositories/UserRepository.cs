using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public class UserRepository : CassandraRepositoryBase<User>, IUserRepository
{

    public UserRepository(IOptions<CassandraOptions> cassandraOptions, ILogger<UserRepository> logger) : base(cassandraOptions, logger)
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

    public async Task<bool> UserExistAsync(string login, string password)
    {
        var result = await GetByLoginAsync(login);
        return result?.Password == password;
    }

    public async Task<bool> UserExistByLoginAsync(string login)
    {
        var result = await GetByLoginAsync(login);
        return result != null;
    }

    public async Task<string> GetPasswordByLoginAsync(string login)
    {
        var result = await GetByFilterAsync(user => user.Login == login);
        return result.Single().Login ?? throw new DataException();
    }

    public async Task ChangePasswordByLoginAsync(string login, string password)
    {
        await AddAsync(new User
        {
            Login = login,
            Password = password
        });
    }

    public async Task CreateUserAsync(string login, string password)
    {
        await AddAsync(new User
        {
            Login = login,
            Password = password
        });
    }
}