using Domain.Models;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public class PasswordStorageRepository : CassandraRepositoryBase<PasswordStorage>, IPasswordStorageRepository
{
    public PasswordStorageRepository(IOptions<CassandraOptions> cassandraOptions) : base(cassandraOptions)
    {
    }

    public async Task<PasswordStorage[]> GetByLoginAndResourceNameAsync(string login, string resourceName)
    {
        return await GetByFilterAsync(passwordStorage =>
            passwordStorage.Login == login && passwordStorage.ResourceName == resourceName);
    }

    public async Task<PasswordStorage[]> GetAllPasswordsByLoginAsync(string login)
    {
        return await GetByFilterAsync(passwordStorage =>
            passwordStorage.Login == login);
    }
}