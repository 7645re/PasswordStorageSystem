using System;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Repositories;

public class ResourceCredentialRepository : CassandraRepositoryBase<ResourceCredential>, IResourceCredentialRepository
{
    public ResourceCredentialRepository(IOptions<CassandraOptions> cassandraOptions, ILogger<ResourceCredentialRepository> logger) : base(cassandraOptions, logger)
    {
    }

    public async Task<ResourceCredential[]> GetByLoginAndResourceNameAsync(string login, string resourceName)
    {
        var resourceCredential = await GetByFilterAsync(passwordStorage =>
            passwordStorage.Login == login && passwordStorage.ResourceName == resourceName);
        return resourceCredential;
    }

    public async Task<ResourceCredential[]> GetCredentialsByAtDate(DateTimeOffset dateTimeOffset)
    {
        return await GetByFilterAsync(c => c.LastUpdate == dateTimeOffset);
    }

    public async Task<ResourceCredential[]> GetAllPasswordsByLoginAsync(string login)
    {
        return await GetByFilterAsync(passwordStorage =>
            passwordStorage.Login == login);
    }

    public async Task CreateCredential(ResourceCredential resourceCredential)
    {
        await AddAsync(resourceCredential);
    }
    
    public async Task ChangeCredential(ResourceCredential resourceCredential)
    {
        await AddAsync(resourceCredential);
    }
}