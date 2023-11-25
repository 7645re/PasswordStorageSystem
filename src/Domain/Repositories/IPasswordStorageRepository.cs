using Domain.Models;

namespace Domain.Repositories;

public interface IPasswordStorageRepository
{
    public Task<PasswordStorage[]> GetByLoginAndResourceNameAsync(string login, string resourceName);
    public Task<PasswordStorage[]> GetAllPasswordsByLoginAsync(string login);
}