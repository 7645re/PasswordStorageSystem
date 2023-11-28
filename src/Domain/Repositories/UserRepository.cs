using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Cassandra.Data.Linq;
using Domain.Models;
using Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public class UserRepository : CassandraRepositoryBase<UserEntity>, IUserRepository
{
    public UserRepository(IOptions<CassandraOptions> cassandraOptions, ILogger<UserRepository> logger) : base(
        cassandraOptions, logger)
    {
    }

    public async Task<IEnumerable<UserEntity>> GetAllUsersAsync()
    {
        return await ExecuteQueryAsync(Table);
    }

    public async Task<UserEntity?> GetUserAsync(string login)
    {
        var result = await ExecuteQueryAsync(Table.Where(r => r.Login == login));
        result = result.ToArray();
        if (!result.Any()) return null;
        if (result.Count() > 1) throw new DataException("Scheme error");
        return result.Single();
    }

    public async Task DeleteUserAsync(string login)
    {
        await ExecuteQueryAsync(Table.Where(r => r.Login == login).Delete());
    }

    public async Task CreateUserAsync(UserEntity userEntity)
    {
        await AddAsync(userEntity);
    }

    public async Task ChangePasswordAsync(string login, string newPassword)
    {
        await UpdateAsync(Table.Where(r => r.Login == login).Select(r => new {Password = newPassword}).Update());
    }
}