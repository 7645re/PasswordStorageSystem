using System;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Repositories;

public interface IResourceCredentialRepository
{
    public Task<ResourceCredential[]> GetByLoginAndResourceNameAsync(string login, string resourceName);
    public Task<ResourceCredential[]> GetAllPasswordsByLoginAsync(string login);
    public Task CreateCredential(ResourceCredential resourceCredential);
    public Task ChangeCredential(ResourceCredential resourceCredential);
    public Task<ResourceCredential[]> GetCredentialsByAtDate(DateTimeOffset dateTimeOffset);
    public Task<ResourceCredential[]> GetAllAsync();
}