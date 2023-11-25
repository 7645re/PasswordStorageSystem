using Domain.Models;

namespace Domain.Repositories;

public interface IUserRepository
{
    public Task<User?> GetByLoginAsync(string login);
    public Task<bool> ExistsIsUserAsync(User user);
    public Task<string> GetPasswordByLoginAsync(string login);
    public Task ChangePasswordByLoginAsync(User user);
    public Task CreateUserAsync(User user);
}
